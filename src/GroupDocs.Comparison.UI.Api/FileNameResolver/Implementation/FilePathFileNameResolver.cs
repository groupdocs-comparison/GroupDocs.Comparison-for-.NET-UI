
// ReSharper disable once CheckNamespace
namespace GroupDocs.Comparison.UI.Api
{
    public class FilePathFileNameResolver : IFileNameResolver
    {
        public Task<string> ResolveFileNameAsync(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            return Task.FromResult(fileName);
        }
    }
}