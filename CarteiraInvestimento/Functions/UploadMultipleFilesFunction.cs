using CarteiraInvestimento.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CarteiraInvestimento.Functions
{
    public class UploadMultipleFilesFunction
    {
        private readonly ILogger<UploadMultipleFilesFunction> _logger;
        private readonly IBlobService _blobService;

        private const string ContainerName = "processing-file";
        public UploadMultipleFilesFunction(ILogger<UploadMultipleFilesFunction> logger, IBlobService blobService)
        {
            _logger = logger;
            _blobService = blobService;
        }

        [Function("UploadMultipleFiles")]
        public async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
      ILogger log)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to upload multiple files.");

            // Verifica se o conteúdo da requisição é multipart/form-data
            if (!req.HasFormContentType)
            {
                return new BadRequestObjectResult("Conteúdo não é multipart/form-data");
            }

            var formData = await req.ReadFormAsync();
            var files = formData.Files;

            if (files.Count == 0)
            {
                return new BadRequestObjectResult("Nenhum arquivo foi enviado.");
            }

            var filePaths = new List<string>();

            // Processa cada arquivo enviado
            foreach (var file in files)
            {
                // Verifica a extensão do arquivo (apenas .xlsx ou .xls são permitidos)
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    return new BadRequestObjectResult("Apenas arquivos Excel (.xlsx, .xls) são permitidos.");
                }

                var fileName = file.FileName;
                var filePath = Path.Combine(Path.GetTempPath(), fileName);

                await _blobService.Upload(file, ContainerName);
            }

            return new OkObjectResult($"Arquivos recebidos com sucesso: {string.Join(", ", filePaths)}");
        }
    }
}
