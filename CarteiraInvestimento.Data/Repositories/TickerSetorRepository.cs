using CarteiraInvestimento.Domain.Interfaces;
using CarteiraInvestimento.Domain.Entities;
using CarteiraInvestimento.Infrastructure.Data;

namespace CarteiraInvestimento.Infrastructure.Repositories
{
    public class TickerSetorRepository : ITickerSetorRepository
    {
        private readonly AppDbContext _context;
        public TickerSetorRepository(AppDbContext context)
        {
            _context = context;
        }
        public void AddRange(List<TickerSetor> list)
        {
            _context.TickerSetor.AddRange(list);

            _context.SaveChanges();
        }
    }
}
