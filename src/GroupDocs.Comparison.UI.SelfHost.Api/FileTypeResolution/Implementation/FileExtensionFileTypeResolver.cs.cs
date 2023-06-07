using GroupDocs.Comparison.Result;
using System.IO;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace GroupDocs.Comparison.UI.SelfHost.Api
{
    public class FileExtensionFileTypeResolver : IFileTypeResolver
    {
        public Task<FileType> ResolveFileTypeAsync(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            FileType fileType = FileType.FromFileNameOrExtension(extension);

            return Task.FromResult(fileType);
        }
    }
}