using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace GroupDocs.Comparison.UI.Api
{
    class GroupDocsComparisonApiConventionBuilder : IEndpointConventionBuilder
    {
        private readonly IEnumerable<IEndpointConventionBuilder> _endpoints;

        public GroupDocsComparisonApiConventionBuilder(IEnumerable<IEndpointConventionBuilder> endpoints)
        {
            _endpoints = endpoints ?? throw new ArgumentNullException(nameof(endpoints));
        }

        public void Add(Action<EndpointBuilder> convention)
        {
            foreach (var endpoint in _endpoints)
            {
                endpoint.Add(convention);
            }
        }
    }
}
