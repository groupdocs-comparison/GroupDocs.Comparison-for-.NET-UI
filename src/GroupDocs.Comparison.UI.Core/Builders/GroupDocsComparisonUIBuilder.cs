using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public class GroupDocsComparisonUIBuilder
    {
        public IServiceCollection Services { get; }

        public GroupDocsComparisonUIBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}