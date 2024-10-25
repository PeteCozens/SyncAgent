using System.Text;

namespace Infrastructure.Services.File
{
    public class FileSystemServiceConfig
    {
        public string Root { get; set; } = string.Empty;
    }

    internal class FileSystemService(FileSystemServiceConfig config) : IFileService
    {
        private string FullPath(string path) => Path.Combine(config.Root, path);

        public async Task Delete(string path)
        {
            await Task.Delay(0);
            System.IO.File.Delete(FullPath(path));
        }

        public async Task<bool> Exists(string path)
        {
            await Task.Delay(0);
            return System.IO.File.Exists(FullPath(path));
        }

        public async Task<Stream> OpenStream(string path, bool readOnly)
        {
            await Task.Delay(0);
            return System.IO.File.Open(FullPath(path), FileMode.OpenOrCreate, readOnly ? FileAccess.Read : FileAccess.ReadWrite, FileShare.Read);
        }

        public async Task<byte[]> ReadDataAsync(string path)
        {
            return await System.IO.File.ReadAllBytesAsync(FullPath(path));
        }

        public async Task<string> ReadTextAsync(string path)
        {
            return await System.IO.File.ReadAllTextAsync(FullPath(path));
        }

        public async Task WriteDataAsync(string path, byte[] content)
        {
            await System.IO.File.WriteAllBytesAsync(FullPath(path), content);
        }

        public async Task WriteTextAsync(string path, string content, string? encoding = null)
        {

            await System.IO.File.WriteAllTextAsync(FullPath(path), content, Encoding.GetEncoding(string.IsNullOrEmpty(encoding) ? "UTF8" : encoding));
        }
    }
}
