using CarteiraInvestimento.Domain.Entities;
using CarteiraInvestimento.Domain.Interfaces;
using CarteiraInvestimento.Domain.Interfaces.IServices;

namespace CarteiraInvestimento.Application.Services
{
    public class MovimentacoesResumoService : IMovimentacoesResumoService
    {

        private readonly IMovimentacoesRepository _movimentacoesRepository;

        public MovimentacoesResumoService(IMovimentacoesRepository movimentacoesRepository)
        {
            _movimentacoesRepository = movimentacoesRepository;
        }

        public async Task<MovimentacoesResumo> Resumir(Ticker ticker)
        {
            var ignorar = new[] { "Dividendo", "Grupamento", "Desdobro", "Leilão de Fração", "Juros Sobre Capital Próprio", "Rendimento" };

            var movimentacoesDividendos = new[] { "Dividendo", "Rendimento" };
            decimal valor;

            var entradas = await _movimentacoesRepository.GetMovimentacoes(ticker.Chave, "Credito", ignorar, "R$");

            var saidas = await _movimentacoesRepository.GetMovimentacoes(ticker.Chave, "Debito", ignorar, "R$");

            var total = (int)Math.Ceiling(saidas.Count() > 0 ? Convert.ToInt32(entradas.Sum(x => x.Quantidade) - saidas.Sum(x => x.Quantidade)) : entradas.Sum(x => x.Quantidade));

            var totalInvestidoEntrada = entradas.Sum(x => Convert.ToDecimal(x.ValorOperacao.Replace("R$", "")));

            var totalInvestidoSaida = saidas.Sum(x => Convert.ToDecimal(x.ValorOperacao.Replace("R$", "")));

            var totalInvestido = totalInvestidoEntrada - totalInvestidoSaida;

            var dividendos = await _movimentacoesRepository.GetMovimentacoes(ticker.Chave, movimentacoesDividendos, "Debito", ignorar, "R$");

            var totalDividendos = dividendos.Sum(x => Convert.ToDecimal(x.ValorOperacao.Replace("R$", "")));

            decimal dividendYield = 0;
            
            if (totalInvestido > 0 && totalDividendos > 0)
                dividendYield = CalcularDividendYield(totalInvestido, totalDividendos);

            var resumo = new MovimentacoesResumo()
            {
                DataAtualizacao = DateTime.UtcNow,
                TickerId = ticker.Id,
                Total = total,
                ValorInvestido = totalInvestido,
                DividendoTotal = totalDividendos,
                DY = dividendYield
            };

            return resumo;
        }

        private decimal CalcularDividendYield(decimal valorInvestido, decimal valorDividendos)
        {
            if (valorInvestido == 0)
            {
                throw new ArgumentException("O valor total investido não pode ser zero.");
            }

            // Calcula o Dividend Yield
            return (valorDividendos / valorInvestido) * 100;
        }
    }
}
