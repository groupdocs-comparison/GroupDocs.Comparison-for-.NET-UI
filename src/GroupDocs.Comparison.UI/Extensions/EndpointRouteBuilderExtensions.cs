﻿using GroupDocs.Comparison.UI;
using GroupDocs.Comparison.UI.Core;
using GroupDocs.Comparison.UI.Middleware;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using Options = GroupDocs.Comparison.UI.Configuration.Options;

namespace GroupDocs.Comparison.UI.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapGroupDocsComparisonUI(this IEndpointRouteBuilder builder,
            Action<Configuration.Options> setupOptions = null)
        {
            var options = new Configuration.Options();
            setupOptions?.Invoke(options);

            EnsureValidApiOptions(options);

            var settingsDelegate = builder.CreateApplicationBuilder()
                .UseMiddleware<UISettingsMiddleware>()
                .Build();

            var embeddedResourcesAssembly = typeof(UIResource).Assembly;

            var resourcesEndpoints =
                new UIEndpointsResourceMapper(new UIEmbeddedResourcesReader(embeddedResourcesAssembly))
                    .Map(builder, options);

            var settingsEndpoint =
                builder.Map(options.UIConfigEndpoint, settingsDelegate);

            var endpointConventionBuilders =
                new List<IEndpointConventionBuilder>(
                    new[] { settingsEndpoint }.Union(resourcesEndpoints));

            return new GroupDocsComparisonUIConventionBuilder(endpointConventionBuilders);
        }

        private static void EnsureValidApiOptions(Configuration.Options options)
        {
            Action<string, string> ensureValidPath = (string path, string argument) =>
            {
                if (string.IsNullOrEmpty(path) || !path.StartsWith("/"))
                {
                    throw new ArgumentException(
                        "The value for customized path can't be null and need to start with / character.", argument);
                }
            };

            Action<string, string> ensureNotEmpty = (string endpoint, string argument) =>
            {
                if (string.IsNullOrEmpty(endpoint))
                {
                    throw new ArgumentException("The value can't be null or empty.", argument);
                }
            };


            ensureValidPath(options.UIPath, nameof(Configuration.Options.UIPath));
            ensureNotEmpty(options.APIEndpoint, nameof(Configuration.Options.APIEndpoint));
        }
    }
}
