using System.Text.Json.Serialization;
using CarteiraInvestimento.Domain.Entities.Base;

namespace CarteiraInvestimento.Domain.Entities
{
    public class Movimentacoes : EntityBase
    {
        public Movimentacoes()
        {
            
        }
        public Movimentacoes(int id, string entradaSaida, DateTime data, string movimentacao, string produto, string instituicao, decimal quantidade, string precoUnitario, string valorOperacao)
        {
            Id = id;
            EntradaSaida = entradaSaida;
            Data = data;
            Movimentacao = movimentacao;
            Produto = produto;
            Instituicao = instituicao;
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
            ValorOperacao = valorOperacao;
        }

        public int Id { get; set; }
        [JsonPropertyName("Entrada/Saída")]
        public string EntradaSaida { get; set; }

        [JsonPropertyName("Data")]
        public DateTime Data { get; set; }

        [JsonPropertyName("Movimentação")]
        public string Movimentacao { get; set; }

        [JsonPropertyName("Produto")]
        public string Produto { get; set; }

        [JsonPropertyName("Instituição")]
        public string Instituicao { get; set; }

        [JsonPropertyName("Quantidade")]
        public decimal Quantidade { get; set; }

        [JsonPropertyName("Preço unitário")]
        public string PrecoUnitario { get; set; }

        [JsonPropertyName("Valor da Operação")]
        public string ValorOperacao { get; set; }
        public int? TickerId { get; set; }  // Chave estrangeira para Ticker
        public Ticker? Ticker { get; set; }  // Navegação para o objeto Ticker
    }
}
