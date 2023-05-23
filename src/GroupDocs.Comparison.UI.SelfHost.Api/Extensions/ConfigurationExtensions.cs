using GroupDocs.Comparison.UI.SelfHost.Api;
using GroupDocs.Comparison.UI.SelfHost.Api.Configuration;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public static Config BindSelfHostApiSettings
            (this IConfiguration configuration, Config config)
        {
            configuration
                .GetSection(Keys.GROUPDOCSCOMPARISONUI_SELF_HOST_API_SECTION_SETTING_KEY)
                .Bind(config, c => c.BindNonPublicProperties = true);

            return config;
        }
    }
}