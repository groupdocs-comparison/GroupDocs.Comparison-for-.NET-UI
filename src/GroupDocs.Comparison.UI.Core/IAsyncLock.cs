using System;
using System.Threading.Tasks;

namespace GroupDocs.Comparison.UI.Core
{
    public interface IAsyncLock
    {
        Task<IDisposable> LockAsync(object key);
    }
}