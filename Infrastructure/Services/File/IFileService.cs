namespace Infrastructure.Services.File
{
    internal interface IFileService
    {
        Task<string> ReadTextAsync(string path);
        Task WriteTextAsync(string path, string content, string? encoding = null);
        Task<byte[]> ReadDataAsync(string path);
        Task WriteDataAsync(string path, byte[] content);
        Task<Stream> OpenStream(string path, bool readOnly);
        Task<bool> Exists(string path);
        Task Delete(string path);
    }
}
