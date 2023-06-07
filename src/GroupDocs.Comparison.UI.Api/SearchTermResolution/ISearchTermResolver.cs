using System.Threading.Tasks;

namespace GroupDocs.Comparison.UI.Api
{
    public interface ISearchTermResolver
    {
        Task<string> ResolveSearchTermAsync(string filepath);
    }
}
