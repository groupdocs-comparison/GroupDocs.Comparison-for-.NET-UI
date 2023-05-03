using GroupDocs.Comparison.UI.Core.Configuration;
using GroupDocs.Comparison.UI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static GroupDocsComparisonUIBuilder AddGroupDocsComparisonUI(this IServiceCollection services,
            Action<Config> setupSettings = null)
        {
            services
                .AddOptions<Config>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.BindUISettings(settings);
                    setupSettings?.Invoke(settings);
                });

            services.TryAddSingleton<ServerAddressesService>();

            return new GroupDocsComparisonUIBuilder(services);
        }
    }
}