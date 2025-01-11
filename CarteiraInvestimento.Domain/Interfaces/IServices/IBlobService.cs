using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CarteiraInvestimento.Domain.Interfaces.IServices
{
    public interface IBlobService
    {
        Task<string> Upload(Stream formFile, string fileName, string containerName);
        Task<string> Upload(IFormFile formFile, string containerName);
        string GetSpecific(string blobName, string containerName);
        List<string> GetMany(string stringBlobNameList, string containerName);
        Task<string> GetBlobUrlWithSas(string blobName, string containerName);
        Task<bool> Delete(string fileName, string containerName);
    }
}