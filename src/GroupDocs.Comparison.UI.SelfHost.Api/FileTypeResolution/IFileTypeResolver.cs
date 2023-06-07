using System.Threading.Tasks;

namespace GroupDocs.Comparison.UI.SelfHost.Api
{
    public interface IFileTypeResolver
    {
        Task<Result.FileType> ResolveFileTypeAsync(string filePath);
    }
}