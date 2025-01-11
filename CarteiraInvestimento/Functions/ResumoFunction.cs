using System.IO;
using System.Threading.Tasks;
using CarteiraInvestimento.Domain.Entities;
using CarteiraInvestimento.Domain.Interfaces;
using CarteiraInvestimento.Domain.Interfaces.IServices;
using CarteiraInvestimento.Infrastructure.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CarteiraInvestimento.Functions
{
    public class ResumoFunction
    {
        private readonly ILogger<ResumoFunction> _logger;
        private readonly ITickerRepository _tickerRepository;
        private readonly IMovimentacoesResumoRepository _movimentacoesResumoRepository;
        private readonly IMovimentacoesResumoService _movimentacoesResumoService;


        public ResumoFunction(ILogger<ResumoFunction> logger, ITickerRepository tickerRepository, IMovimentacoesResumoService movimentacoesResumoService, IMovimentacoesResumoRepository movimentacoesResumoRepository)
        {
            _logger = logger;
            _tickerRepository = tickerRepository;
            _movimentacoesResumoService = movimentacoesResumoService;
            _movimentacoesResumoRepository = movimentacoesResumoRepository;
        }

        [Function(nameof(ResumoFunction))]
        public async Task Run([BlobTrigger("archive-file/{name}", Connection = "blobstorage")] Stream stream, string name)
        {
            var tickers = _tickerRepository.GetAll();
            var movimentacoesResumo = new List<MovimentacoesResumo>();

            foreach (var ticker in tickers)
            {
                var resumo = await _movimentacoesResumoService.Resumir(ticker);

                movimentacoesResumo.Add(resumo);
            }

            _movimentacoesResumoRepository.AddRange(movimentacoesResumo);
            //_logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        }
    }
}
