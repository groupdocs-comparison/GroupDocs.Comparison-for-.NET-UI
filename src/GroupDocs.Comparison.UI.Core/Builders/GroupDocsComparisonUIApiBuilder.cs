using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public class GroupDocsComparisonUIApiBuilder
    {
        public IServiceCollection Services { get; }

        public GroupDocsComparisonUIApiBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}