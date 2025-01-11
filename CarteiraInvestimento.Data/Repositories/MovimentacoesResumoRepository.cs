using CarteiraInvestimento.Domain.Interfaces;
using CarteiraInvestimento.Domain.Entities;
using CarteiraInvestimento.Infrastructure.Data;

namespace CarteiraInvestimento.Infrastructure.Repositories
{
    public class MovimentacoesResumoRepository : IMovimentacoesResumoRepository
    {
        private readonly AppDbContext _context;
        public MovimentacoesResumoRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddRange(List<MovimentacoesResumo> list)
        {
            _context.MovimentacoesResumo.AddRange(list);

            _context.SaveChanges();
        }

        public MovimentacoesResumo? Get(int id)
        {
            return _context.MovimentacoesResumo.Where(x => x.Id == id).FirstOrDefault();
        }

        public MovimentacoesResumo? Get(string ticker)
        {
            return _context.MovimentacoesResumo.Where(x => x.Ticker.Chave == ticker).FirstOrDefault();
        }
    }
} 
