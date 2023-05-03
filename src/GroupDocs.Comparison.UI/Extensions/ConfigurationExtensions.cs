using GroupDocs.Comparison.UI;
using GroupDocs.Comparison.UI.Core.Configuration;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public static Config BindUISettings
            (this IConfiguration configuration, Config config)
        {
            configuration
                .GetSection(Keys.GROUPDOCSCOMPARISONUI_SECTION_SETTING_KEY)
                .Bind(config, c => c.BindNonPublicProperties = true);

            return config;
        }
    }
}