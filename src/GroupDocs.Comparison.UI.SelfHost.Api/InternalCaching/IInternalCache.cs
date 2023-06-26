using GroupDocs.Comparison.UI.Core.Entities;

namespace GroupDocs.Comparison.UI.SelfHost.Api.InternalCaching
{
    public interface IInternalCache
    {
        bool TryGet(FileCredentials fileCredentials, out Comparer comparer);

        void Set(FileCredentials fileCredentials, Comparer entry);
    }
}