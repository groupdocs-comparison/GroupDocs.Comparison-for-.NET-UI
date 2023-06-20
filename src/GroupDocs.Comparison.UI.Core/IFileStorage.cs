using System.Collections.Generic;
using System.Threading.Tasks;
using GroupDocs.Comparison.UI.Core.Entities;

namespace GroupDocs.Comparison.UI.Core
{
    public interface IFileStorage
    {
        Task<IEnumerable<FileSystemEntry>> ListDirsAndFilesAsync(string dirPath);

        Task<byte[]> ReadFileAsync(string filePath);

        Task<string> WriteFileAsync(string fileName, byte[] bytes, bool rewrite);

        public string GetFileStoragePath();
    }
}