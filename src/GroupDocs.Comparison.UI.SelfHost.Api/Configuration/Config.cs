using System;
using GroupDocs.Comparison.Options;
using GroupDocs.Comparison.UI.Core;

namespace GroupDocs.Comparison.UI.SelfHost.Api.Configuration
{
    public class Config
    {
        internal string LicensePath = string.Empty;
        internal readonly CompareOptions comparisonOptions = new CompareOptions();
        internal readonly InternalCacheOptions InternalCacheOptions = InternalCacheOptions.CacheForFiveMinutes;

        public Config SetLicensePath(string licensePath)
        {
            LicensePath = licensePath;
            return this;
        }

        public Config ConfigureCompareOptions(Action<CompareOptions> setupOptions)
        {
            setupOptions?.Invoke(comparisonOptions);
            return this;
        }

        /// <summary>
        /// Call this method to configure internal objects caching.
        /// Internal caching makes objects available between requests to speed up rendering when document is rendered in chunks.
        /// Default cache entry lifetime is 5 minutes.
        /// Internal cache is based on MemoryCache (ConcurrentDictionary), so the object are stored in memory. 
        /// </summary>
        /// <param name="setupOptions">Setup delegate.</param>
        /// <returns>This instance.</returns>
        public Config ConfigureInternalCaching(Action<InternalCacheOptions> setupOptions)
        {
            setupOptions?.Invoke(InternalCacheOptions);
            return this;
        }
    }
}
