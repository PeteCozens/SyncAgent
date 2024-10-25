using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;

namespace Infrastructure.Services.File
{


    internal class AzureBlobServiceConfig
    {
        public string ContainerName { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountKey { get; set; } = string.Empty;
    }

    internal class AzureBlobService : IFileService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobService(AzureBlobServiceConfig config)
        {
            var connectionString = $"DefaultEndpointsProtocol=https;AccountName={config.AccountName};AccountKey={config.AccountKey};EndpointSuffix=core.windows.net";
            var containerClient = new BlobServiceClient(connectionString);

            _containerClient = containerClient.GetBlobContainerClient(config.ContainerName);
            _containerClient.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task Delete(string path)
        {
            var blobClient = _containerClient.GetBlobClient(path);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<bool> Exists(string path)
        {
            var blobClient = _containerClient.GetBlobClient(path);
            return await blobClient.ExistsAsync();
        }

        public async Task<Stream> OpenStream(string path, bool readOnly)
        {
            await Task.Delay(0);
            var blobClient = _containerClient.GetBlobClient(path);
            if (readOnly)
                return blobClient.OpenRead();
            else
                return blobClient.OpenWrite(false);
        }

        public async Task<byte[]> ReadDataAsync(string path)
        {
            var blobClient = _containerClient.GetBlobClient(path);
            using var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task<string> ReadTextAsync(string path)
        {
            var blobClient = _containerClient.GetBlobClient(path);

            using var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream);
            memoryStream.Position = 0;

            using var reader = new StreamReader(memoryStream, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        public async Task WriteDataAsync(string path, byte[] content)
        {
            var blobClient = _containerClient.GetBlobClient(path);
            using var memoryStream = new MemoryStream(content);
            await blobClient.UploadAsync(memoryStream, overwrite: true);
        }

        public async Task WriteTextAsync(string path, string content, string? encoding = null)
        {
            var blobClient = _containerClient.GetBlobClient(path);
            using var memoryStream = new MemoryStream(Encoding.GetEncoding(string.IsNullOrEmpty(encoding) ? "UTF8" : encoding).GetBytes(content));
            await blobClient.UploadAsync(memoryStream, overwrite: true);
        }
    }
}
