using GroupDocs.Comparison.UI.Core;
using GroupDocs.Comparison.UI.Api.Local.Storage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GroupDocsComparisonUIApiBuilderExtensions
    {
        public static GroupDocsComparisonUIApiBuilder AddLocalStorage(
            this GroupDocsComparisonUIApiBuilder builder, string storagePath)
        {
            builder.Services.AddTransient<IFileStorage>(_ =>
                new LocalFileStorage(storagePath));

            return builder;
        }
    }
}
