using System.Threading.Tasks;

namespace GroupDocs.Comparison.UI.Api
{
    public class SearchTermResolver : ISearchTermResolver
    {
        public Task<string> ResolveSearchTermAsync(string filepath)
        {
            string searchTerm = string.Empty;
            return Task.FromResult(searchTerm);
        }
    }
}
