using System.Threading.Tasks;
using GroupDocs.Comparison.UI.Core.Entities;

namespace GroupDocs.Comparison.UI.Core
{
    public interface IComparison
    {
        Task<DocumentInfo> GetDocumentInfoAsync(FileCredentials fileCredentials);

    }
}