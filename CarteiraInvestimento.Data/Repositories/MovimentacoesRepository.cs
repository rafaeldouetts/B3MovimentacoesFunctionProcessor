using CarteiraInvestimento.Domain.Interfaces;
using CarteiraInvestimento.Domain.Entities;
using CarteiraInvestimento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarteiraInvestimento.Infrastructure.Repositories
{
    public class MovimentacoesRepository : IMovimentacoesRepository
    {
        private readonly AppDbContext _context;
        public MovimentacoesRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddRange(List<Movimentacoes> movimentacoes, string userId)
        {
            if (movimentacoes == null || !movimentacoes.Any())
            {
                throw new ArgumentException("A lista de movimentações não pode estar vazia.");
            }

            List<Movimentacoes> remover = new List<Movimentacoes>();
            //tentar adicionar o ticker
            try
            {
                foreach (var entity in movimentacoes) 
            {

                var existe = _context.Movimentacoes.Where(x => x.Data == entity.Data && x.Produto == entity.Produto && x.Quantidade == entity.Quantidade && x.PrecoUnitario == entity.PrecoUnitario).Any();
            
                if (existe)
                {
                    remover.Add(entity);
                }

               
                    var ticker = entity.Produto.Split("-").Any() ? entity.Produto.Split("-").First() : "";

                    if (!string.IsNullOrEmpty(ticker) && entity.Produto.Split("-").Count() > 1 )
                    {
                        var tickerEntity = _context.Tickers.Where(x => x.Chave == ticker).FirstOrDefault();
                        entity.TickerId = tickerEntity.Id;
                    }
                }

                movimentacoes = movimentacoes
                                .Where(x => !remover.Any(r =>
                                      r.Data.Date == x.Data.Date &&
                                      r.Quantidade == x.Quantidade &&
                                      r.ValorOperacao == x.ValorOperacao))
                                .ToList();

                // Salva as alterações no banco de dados
                _context.Movimentacoes.AddRange(movimentacoes);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }

        public Task AddRange(List<Movimentacoes> movimentacoes)
        {
            _context.Movimentacoes.AddRange(movimentacoes);

            _context.SaveChanges();

            return Task.CompletedTask;
        }

        public Task<List<Movimentacoes>> GetMovimentacoes(string chave, string entradaSaida, string[] ignorar, string contain)
        {
            return _context.Movimentacoes
                .AsNoTracking()
                .Where(x =>
                x.Ticker.Chave == chave &&
                x.EntradaSaida == entradaSaida &&
                !ignorar.Contains(x.Movimentacao) &&
                x.ValorOperacao.Contains(contain)).ToListAsync();
        }

        public Task<List<Movimentacoes>> GetMovimentacoes(string chave, string[] movimentacoes, string entradaSaida, string[] ignorar, string contain)
        {
            return _context.Movimentacoes
                .AsNoTracking()
                .Where(x =>
                x.Ticker.Chave == chave &&
                movimentacoes.Contains(x.Movimentacao) &&
                x.EntradaSaida == entradaSaida &&
                !ignorar.Contains(x.Movimentacao) &&
                x.ValorOperacao.Contains(contain)).ToListAsync();
        }

        public async Task<decimal> GetTotalInvestido()
        {
            //return _context.Movimentacoes.Sum(x => x.to)
            return decimal.Zero;
        }
    }
}
