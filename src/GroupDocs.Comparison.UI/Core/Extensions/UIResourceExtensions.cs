﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroupDocs.Comparison.UI.Configuration;
using Microsoft.AspNetCore.Http;

namespace GroupDocs.Comparison.UI.Core.Extensions
{
    internal static class UIResourceExtensions
    {
        public static UIResource GetIndexPage(this IEnumerable<UIResource> resources)
        {
            var index = resources
                .FirstOrDefault(r =>
                    r.ContentType == ContentType.HTML && r.FileName == Keys.GROUPDOCSCOMPARISONUI_MAIN_UI_RESOURCE);

            return index;
        }

        public static string GetIndexPageHtml(this UIResource index,
            UI.Configuration.Options options, PathString pathBase, Dictionary<string, string> routeValues)
        {
            var uiPath = options.UIPath
                .AppendPathBase(pathBase)
                .ReplacePatternsWithRouteValues(routeValues)
                .WithTrailingSlash();

            var html = new StringBuilder(index.Content);

            html.Replace(Keys.GROUPDOCSCOMPARISONUI_MAIN_UI_PATH, uiPath);

            var apiPath = options.APIEndpoint
                .AppendPathBase(pathBase)
                .ReplacePatternsWithRouteValues(routeValues)
                .TrimTrailingSlash();

            html.Replace(Keys.GROUPDOCSCOMPARISONUI_MAIN_UI_API_TARGET, apiPath);

            var uiConfigPath =
                options.UIConfigEndpoint
                    .AppendPathBase(pathBase)
                    .ReplacePatternsWithRouteValues(routeValues);

            html.Replace(Keys.GROUPDOCSCOMPARISONUI_MAIN_UI_SETTINGS_PATH_TARGET, uiConfigPath);

            return html.ToString();
        }
        public static ICollection<UIStylesheet> GetCustomStylesheets(this UIResource resource, UI.Configuration.Options options)
        {
            List<UIStylesheet> styleSheets = new List<UIStylesheet>();

            if (!options.CustomStylesheets.Any())
            {
                resource.Content = resource.Content.Replace(Keys.GROUPDOCSCOMPARISONUI_STYLESHEETS_TARGET, string.Empty);
                return styleSheets;
            }

            foreach (var stylesheet in options.CustomStylesheets)
            {
                styleSheets.Add(UIStylesheet.Create(options, stylesheet));
            }

            var htmlStyles = styleSheets.Select(s =>
            {
                var linkHref = s.ResourceRelativePath.AsRelativeResource();
                return $"<link rel='stylesheet' href='{linkHref}'/>";
            });

            resource.Content = resource.Content.Replace(Keys.GROUPDOCSCOMPARISONUI_STYLESHEETS_TARGET,
                string.Join("\n", htmlStyles));

            return styleSheets;
        }

    }

}
