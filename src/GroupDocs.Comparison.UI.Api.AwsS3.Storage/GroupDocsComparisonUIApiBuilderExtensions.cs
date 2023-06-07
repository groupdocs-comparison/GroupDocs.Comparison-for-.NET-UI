using System;
using GroupDocs.Comparison.UI.Api.AwsS3.Storage;
using GroupDocs.Comparison.UI.Core;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public static class GroupDocsComparisonUIApiBuilderExtensions
    {
        public static GroupDocsComparisonUIApiBuilder AddAwsS3Storage(
            this GroupDocsComparisonUIApiBuilder builder, Action<AwsS3Options> setupOptions)
        {
            var options = new AwsS3Options();
            setupOptions?.Invoke(options);

            builder.Services.AddTransient<IFileStorage>(_ =>
                new AwsS3FileStorage(options));

            return builder;
        }
    }
}
