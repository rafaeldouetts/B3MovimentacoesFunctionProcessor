using CarteiraInvestimento.Domain.Interfaces;
using CarteiraInvestimento.Domain.Entities;
using CarteiraInvestimento.Infrastructure.Data;

namespace CarteiraInvestimento.Infrastructure.Repositories
{
    public class IndicadorRepository : IIndicadoresRepository
    {
        private readonly AppDbContext _context;
        public IndicadorRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddRange(List<Indicadores> result)
        {
            _context.Indicadores.AddRange(result);

            _context.SaveChanges();
        }
    }
}
