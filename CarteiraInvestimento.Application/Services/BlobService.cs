using CarteiraInvestimento.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CarteiraInvestimento.Application.Services
{
    public class BlobService : IBlobService
    {
        private readonly string _ConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        public BlobService()
        {
        }

        public async Task<string> GetBlobUrlWithSas(string blobName, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(_ConnectionString);

            // Cria um objeto CloudBlobClient
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlobReference(blobName);

            var exists = await blob.ExistsAsync();
            if (!exists) return string.Empty;

            var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(1),
                Permissions = SharedAccessBlobPermissions.Read
            });

            return blob.Uri + sasToken;
        }

        public List<string> GetMany(string stringBlobNameList, string containerName)
        {
            var blobNames = stringBlobNameList.Split(',').ToList();
            var blobUrls = new List<string>();

            foreach (var blobName in blobNames)
            {
                var blobUrl = GetBlobUrlWithSas(blobName, containerName).Result;

                if (!string.IsNullOrEmpty(blobUrl))
                {
                    blobUrls.Add(blobUrl);
                }
            }

            return blobUrls;
        }

        public string GetSpecific(string blobName, string containerName)
        {
            var blobUrl = GetBlobUrlWithSas(blobName, containerName).Result;
            return blobUrl;
        }

        public async Task<string> Upload(IFormFile formFile, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(_ConnectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            var blobName = formFile.FileName; // Gera um ID único
            var blob = container.GetBlockBlobReference(blobName);

            using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            await blob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);

            // Gera o SAS Token para o blob
            var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(1), // Expira em 1 hora
                Permissions = SharedAccessBlobPermissions.Read // Apenas leitura
            });

            // Retorna a URL completa com o SAS Token
            var blobUrlWithSas = blob.Uri + sasToken + formFile.ContentType;
            return blobUrlWithSas;
        }

        public async Task<string> Upload(Stream fileStream, string fileName, string containerName)
        {
            // Criando a referência para a conta de armazenamento e o blob client
            var storageAccount = CloudStorageAccount.Parse(_ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Criando ou acessando o container
            var container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            // Usando o nome do arquivo para o blob (pode ser modificado para gerar um ID único)
            var blob = container.GetBlockBlobReference(fileName);

            // Fazer o upload do arquivo diretamente do stream
            await blob.UploadFromStreamAsync(fileStream);

            // Gerar o SAS Token para acesso temporário ao blob
            var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(1), // Expira em 1 hora
                Permissions = SharedAccessBlobPermissions.Read // Apenas leitura
            });

            // Construir a URL do blob com o SAS Token
            var blobUrlWithSas = blob.Uri + sasToken;

            // Retornar a URL completa com o SAS Token
            return blobUrlWithSas;
        }

        public async Task<bool> Delete(string fileName, string containerName)
        {
            try
            {
                // Criando a referência para a conta de armazenamento e o blob client
                var storageAccount = CloudStorageAccount.Parse(_ConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Criando ou acessando o container
                var container = blobClient.GetContainerReference(containerName);

                // Referência para o blob que será removido
                var blob = container.GetBlockBlobReference(fileName);

                // Tenta excluir o blob, caso ele exista
                bool deleted = await blob.DeleteIfExistsAsync();

                if (deleted)
                {
                    // Se o arquivo foi excluído com sucesso
                    return true;
                }
                else
                {
                    // Caso o arquivo não tenha sido encontrado ou não tenha sido excluído
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log ou tratamento de exceção
                Console.WriteLine($"Erro ao tentar excluir o arquivo {fileName}: {ex.Message}");
                return false;
            }
        }

    }
}
