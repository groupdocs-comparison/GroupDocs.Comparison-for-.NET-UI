using GroupDocs.Comparison.UI.Core.Entities;

// ReSharper disable once CheckNamespace
namespace GroupDocs.Comparison.UI.SelfHost.Api.InternalCaching
{
    public class NoopInternalCache : IInternalCache
    {
        public bool TryGet(FileCredentials fileCredentials, out Comparer comparer)
        {
            comparer = null;
            return false;
        }

        public void Set(FileCredentials fileCredentials, Comparer entry) { }
    }
}