using System.IO;
using CarteiraInvestimento.Application.Dtos;
using CarteiraInvestimento.Application.Services;
using CarteiraInvestimento.Domain.Interfaces;
using CarteiraInvestimento.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CarteiraInvestimento.Functions
{
    public class ReadFileFunction
    {
        private readonly ILogger<ReadFileFunction> _logger;
        private readonly IBlobService _blobService;
        private readonly IExcelService _excelService;
        private readonly IMovimentacoesRepository _movimentacoesRepository;
        private readonly ITickerService _tickerService;
        private readonly ITickerRepository _tickerRepository;

        private const string _ContainerName = "archive-file";
        public ReadFileFunction(ILogger<ReadFileFunction> logger, IBlobService blobService, IExcelService excelService, IMovimentacoesRepository movimentacoesRepository, ITickerService tickerService, ITickerRepository tickerRepository)
        {
            _logger = logger;
            _blobService = blobService;
            _excelService = excelService;
            _movimentacoesRepository = movimentacoesRepository;
            _tickerService = tickerService;
            _tickerRepository = tickerRepository;
        }

        [Function(nameof(ReadFileFunction))]
        public async Task Run([BlobTrigger("processing-file/{fileName}", Connection = "blobstorage")] Stream stream, string fileName)
        {
            try
            {
                using var blobStreamReader = new StreamReader(stream);
                var content = await blobStreamReader.ReadToEndAsync();

                var movimentacoes = _excelService.ReadMovimentacoes(stream);

                var tickers = _tickerService.Get(movimentacoes);

                await _tickerRepository.AddRange(tickers);

                await _movimentacoesRepository.AddRange(movimentacoes);

                await _blobService.Upload(stream, fileName, _ContainerName);

                await _blobService.Delete(fileName, "processing-file");

                //mover o arquivo de blob 
                _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {fileName} \n Data: {content}");
            }
            catch (Exception ex )
            {

                throw;
            }
        }
    }
}
