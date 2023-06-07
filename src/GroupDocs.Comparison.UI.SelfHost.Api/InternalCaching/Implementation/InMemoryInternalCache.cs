using System;
using GroupDocs.Comparison.UI.Core.Entities;
using GroupDocs.Comparison.UI.SelfHost.Api.Configuration;
using Microsoft.Extensions.Caching.Memory;

// ReSharper disable once CheckNamespace
namespace GroupDocs.Comparison.UI.SelfHost.Api.InternalCaching
{
    public class InMemoryInternalCache : IInternalCache
    {
        private readonly IMemoryCache _cache;
        private readonly InternalCacheOptions _options;

        public InMemoryInternalCache(IMemoryCache cache, InternalCacheOptions options)
        {
            _cache = cache;
            _options = options;
        }

        public bool TryGet(FileCredentials fileCredentials, out Comparer comparer)
        {
            var key = GetKey(fileCredentials);
            if (_cache.TryGetValue(key, out object obj))
            {
                comparer = (Comparer)obj;
                return true;
            }

            comparer = null;
            return false;
        }

        public void Set(FileCredentials fileCredentials, Comparer entry)
        {
            var entryKey = GetKey(fileCredentials);
            if (!_cache.TryGetValue(entryKey, out var obj))
            {
                var entryOptions = CreateCacheEntryOptions();
                _cache.Set(entryKey, entry, entryOptions);
            }
        }

        private string GetKey(FileCredentials fileCredentials) =>
            $"{fileCredentials.FilePath}_{fileCredentials.Password}__VC";

        private MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            var entryOptions = new MemoryCacheEntryOptions();

            if (_options.CacheEntryExpirationTimeoutMinutes > 0)
                entryOptions.SlidingExpiration = TimeSpan.FromMinutes(_options.CacheEntryExpirationTimeoutMinutes);

            entryOptions.RegisterPostEvictionCallback(
                callback: (key, value, evictionReason, state) =>
                {
                    if (value is Comparer comparer)
                        comparer.Dispose();
                });

            return entryOptions;
        }
    }
}