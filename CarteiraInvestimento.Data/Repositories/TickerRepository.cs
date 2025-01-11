using CarteiraInvestimento.Domain.Interfaces;
using CarteiraInvestimento.Domain.Entities;
using CarteiraInvestimento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarteiraInvestimento.Infrastructure.Repositories
{
    public class TickerRepository : ITickerRepository
    {
        private readonly AppDbContext _context;
        public TickerRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task AddRange(List<Ticker> tickers)
        {
            _context.Tickers.AddRange(tickers);

            _context.SaveChanges();

            return Task.CompletedTask;
        }

        public bool Any(Ticker ticker)
        {
            return _context.Tickers.Where( x => x.Chave == ticker.Chave).Any();
        }

        public List<Ticker> GetAll()
        {
            return _context.Tickers.ToList();
        }

        public List<Ticker> GetAll(List<string> chaves)
        {
            return _context.Tickers
                .AsNoTracking()
                .Where(x => chaves.Contains(x.Chave))
                .ToList();
        }
    }
}
