using CarteiraInvestimento.Domain.Entities;

namespace CarteiraInvestimento.Domain.Interfaces
{
    public interface ITickerRepository
    {
        Task AddRange(List<Ticker> tickers);
        bool Any(Ticker ticker);
        List<Ticker> GetAll();
        List<Ticker> GetAll(List<string> chaves);
    }
}
