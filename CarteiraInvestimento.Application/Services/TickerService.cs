using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarteiraInvestimento.Domain.Entities;
using CarteiraInvestimento.Domain.Interfaces;
using CarteiraInvestimento.Domain.Interfaces.IServices;

namespace CarteiraInvestimento.Application.Services
{
    public class TickerService : ITickerService
    {
        private readonly ITickerRepository _tickerRepository;
        public TickerService(ITickerRepository tickerRepository)
        {
            _tickerRepository = tickerRepository;
        }

        public List<Ticker> Get(List<Movimentacoes> movimentacoes)
        {
            List<Ticker> tickers = new List<Ticker>();

            foreach (var produto in movimentacoes)
            {
                if (produto.Produto.Split("-").Count() > 1)
                {
                    var splitResult = produto.Produto.Split("-");

                    var ticker = new Ticker(splitResult.Reverse().First().Trim(), splitResult.First().Trim());

                    if (!_tickerRepository.Any(ticker) && !tickers.Where(x => x.Chave == ticker.Chave).Any())
                        tickers.Add(ticker);
                }
                else
                {
                    var ticker = new Ticker(produto.Produto.Trim(), produto.Produto.Trim());

                    if (!_tickerRepository.Any(ticker) && !tickers.Where(x => x.Chave == ticker.Chave).Any())
                        tickers.Add(ticker);
                }

            }

            return tickers;
        }
    }
}
