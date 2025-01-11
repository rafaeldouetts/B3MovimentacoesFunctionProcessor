using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;
using CarteiraInvestimento.Application.Dtos;
using CarteiraInvestimento.Domain.Entities;
using CarteiraInvestimento.Domain.Interfaces;
using CarteiraInvestimento.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace CarteiraInvestimento.Application.Services
{
    public abstract class ExcelGneric
    {
        public List<T> ReadExcelAsList<T>(Stream stream, bool hasHeader = true) where T : new()
        {
            try
            {
                //if (!File.Exists(filePath))
                //{
                //    throw new FileNotFoundException("O arquivo Excel não foi encontrado.", filePath);
                //}

                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0]; // Lê a primeira planilha

                if (worksheet == null)
                {
                    throw new InvalidOperationException("A planilha está vazia ou não foi encontrada.");
                }

                var result = new List<T>();
                var properties = typeof(T).GetProperties();

                // Define a cultura como pt-BR (para vírgulas como separador decimal)
                var culture = CultureInfo.GetCultureInfo("pt-BR");

                // Mapeamento das colunas
                Dictionary<string, int> columnMapping = new();
                if (hasHeader)
                {
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        var columnName = worksheet.Cells[1, col].Text.Trim();
                        if (!string.IsNullOrEmpty(columnName))
                        {
                            columnMapping[columnName] = col;
                        }
                    }
                }

                // Leitura das linhas de dados
                for (int row = hasHeader ? 2 : 1; row <= worksheet.Dimension.Rows; row++)
                {
                    var item = new T();

                    foreach (var prop in properties)
                    {
                        // Obtém o nome personalizado da coluna usando o atributo JsonPropertyName
                        var attribute = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
                        var columnName = attribute != null ? attribute.Name : prop.Name;

                        if (columnMapping.ContainsKey(columnName))
                        {
                            int colIndex = columnMapping[columnName];
                            var cellValue = worksheet.Cells[row, colIndex].Text.Trim();

                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                try
                                {
                                    // Obtém o tipo real da propriedade (considerando Nullable<>)
                                    var propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                                    object value;
                                    if (propertyType == typeof(decimal))
                                    {
                                        value = decimal.Parse(cellValue, culture);
                                    }
                                    else if (propertyType == typeof(double))
                                    {
                                        value = double.Parse(cellValue, culture);
                                    }
                                    else if (propertyType == typeof(int))
                                    {
                                        value = int.Parse(cellValue, culture);
                                    }
                                    else if (propertyType == typeof(DateTime))
                                    {
                                        value = DateTime.Parse(cellValue, culture);
                                    }
                                    else
                                    {
                                        value = Convert.ChangeType(cellValue, propertyType, culture);
                                    }

                                    prop.SetValue(item, value);
                                }
                                catch (FormatException ex)
                                {
                                    throw new FormatException($"Erro ao converter o valor '{cellValue}' na coluna '{prop.Name}' para o tipo '{prop.PropertyType.Name}'.", ex);
                                }
                            }
                        }
                    }

                    result.Add(item);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler o arquivo", ex.InnerException);
            }
        }
    }

    //Herança Polimorfismo
    public class ExcelService : ExcelGneric, IExcelService
    {
        private readonly ITickerRepository _tickerRepository;

        public ExcelService(ITickerRepository tickerRepository)
        {
            _tickerRepository = tickerRepository;
        }

        public List<Movimentacoes> ReadMovimentacoes(Stream filePath, bool hasHeader = true)
        {
            var model = ReadExcelAsList<MovimentacoesDto>(filePath, hasHeader);

            var chaves = model.Where(x => x.Produto.Split("-").Count() > 1).Select(x => x.Produto.Split("-").First()).ToList();

            var tickers = _tickerRepository.GetAll(chaves);

            var result = model.Select(x => new Movimentacoes()
            {
                Data = x.Data,
                DataInclusao = DateTime.UtcNow,
                EntradaSaida = x.EntradaSaida,
                Instituicao = x.Instituicao,
                Movimentacao = x.Movimentacao,
                PrecoUnitario = x.PrecoUnitario,
                Produto = x.Produto,
                Quantidade = x.Quantidade,
                ValorOperacao = x.ValorOperacao,
                TickerId = tickers.Where(t => t.Chave == x.Produto.Split("-").First().Trim()).Select(x => x.Id).FirstOrDefault()
            }).ToList();

            return result;
        }
    }
}
