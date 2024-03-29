﻿using System;
using GroupDocs.Comparison.UI.Api;
using GroupDocs.Comparison.UI.Api.Configuration;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapGroupDocsComparisonApi(this IEndpointRouteBuilder builder,
            Action<Options> setupOptions = null)
        {
            var options = new Options();
            setupOptions?.Invoke(options);

            EnsureValidApiOptions(options);

            MapControllerRoutes(builder, options);

            return new GroupDocsComparisonApiConventionBuilder(new IEndpointConventionBuilder[0]);
        }

        private static void MapControllerRoutes(IEndpointRouteBuilder builder, Options options)
        {
            var relativeApiPath = options.ApiPath.AsRelativeResource();

            var actions = new[]
            {
                Constants.LOAD_CONFIG_ACTION_NAME,
                Constants.LOAD_FILE_TREE_ACTION_NAME,
                Constants.DOWNLOAD_DOCUMENT_ACTION_NAME,
                Constants.UPLOAD_DOCUMENT_ACTION_NAME,
                Constants.COMPARE_ACTION_NAME,
                Constants.CHANGES_ACTION_NAME,
                Constants.LOAD_DOCUMENT_DESCRIPTION_ACTION_NAME,
                Constants.LOAD_DOCUMENT_PAGE_ACTION_NAME,
            };

            foreach (var action in actions)
            {
                builder.MapControllerRoute(
                    name: action, $"{relativeApiPath}/{action}",
                    new { controller = Constants.CONTROLLER_NAME, action = action });
            }
        }

        private static void EnsureValidApiOptions(Options options)
        {
            Action<string, string> ensureValidPath = (string path, string argument) =>
            {
                if (string.IsNullOrEmpty(path) || !path.StartsWith("/"))
                {
                    throw new ArgumentException(
                        "The value for customized path can't be null and need to start with / character.", argument);
                }
            };

            ensureValidPath(options.ApiPath, nameof(Options.ApiPath));
        }
    }
}