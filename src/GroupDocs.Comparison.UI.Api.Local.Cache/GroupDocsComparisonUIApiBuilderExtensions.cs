using System.Linq;
using GroupDocs.Comparison.UI.Core;
using GroupDocs.Comparison.UI.Api.Local.Cache;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GroupDocsComparisonUIApiBuilderExtensions
    {
        public static GroupDocsComparisonUIApiBuilder AddLocalCache(this GroupDocsComparisonUIApiBuilder builder, string cachePath)
        {
            ServiceDescriptor registeredServiceDescriptor = builder.Services.FirstOrDefault(
                s => s.ServiceType == typeof(IFileCache));

            if (registeredServiceDescriptor != null)
            {
                builder.Services.Remove(registeredServiceDescriptor);
            }

            // NOTE: Replace is used here as by default we've registered Noop cache 
            builder.Services.AddTransient<IFileCache>(_ => new LocalFileCache(cachePath));

            return builder;
        }
    }
}
