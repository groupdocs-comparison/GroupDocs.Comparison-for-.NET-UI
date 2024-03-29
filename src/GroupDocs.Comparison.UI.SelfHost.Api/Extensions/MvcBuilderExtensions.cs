﻿using System;
using System.Reflection;
using GroupDocs.Comparison.UI.Api;
using GroupDocs.Comparison.UI.Api.Controllers;
using GroupDocs.Comparison.UI.Core;
using GroupDocs.Comparison.UI.Core.Caching.Implementation;
using GroupDocs.Comparison.UI.Core.FileCaching;
using GroupDocs.Comparison.UI.SelfHost.Api;
using GroupDocs.Comparison.UI.SelfHost.Api.Configuration;
using GroupDocs.Comparison.UI.SelfHost.Api.InternalCaching;
using GroupDocs.Comparison.UI.SelfHost.Api.Licensing;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
    {
        public static GroupDocsComparisonUIApiBuilder AddGroupDocsComparisonSelfHostApi(this IMvcBuilder builder,
            Action<Config> setupConfig = null)
        {
            var config = new Config();
            setupConfig?.Invoke(config);

            // GroupDocs.Comparison API Registration
            builder.PartManager.ApplicationParts.Add(new AssemblyPart(
                Assembly.GetAssembly(typeof(ComparisonController))));

            builder.Services
                .AddOptions<Config>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.BindSelfHostApiSettings(settings);
                    setupConfig?.Invoke(settings);
                });

            builder.Services.AddSingleton<IComparisonLicenser, ComparisonLicenser>();
            builder.Services.AddTransient<IFileCache, NoopFileCache>();
            builder.Services.AddTransient<IAsyncLock, AsyncDuplicateLock>();
            builder.Services.TryAddSingleton<IFileNameResolver, FilePathFileNameResolver>();
            builder.Services.TryAddSingleton<IFileTypeResolver, FileExtensionFileTypeResolver>();
            builder.Services.TryAddSingleton<IUIConfigProvider, UIConfigProvider>();
            builder.Services.AddTransient<IUIConfigProvider, UIConfigProvider>();

            if (config.InternalCacheOptions.IsCacheEnabled)
            {
                builder.Services.TryAddSingleton<IMemoryCache, MemoryCache>();
                builder.Services.AddOptions<InternalCacheOptions>();
                builder.Services.TryAddSingleton<IInternalCache>(factory =>
                {
                    var memoryCache = factory.GetRequiredService<IMemoryCache>();
                    return new InMemoryInternalCache(memoryCache, config.InternalCacheOptions);
                });
            }
            else
            {
                builder.Services.TryAddSingleton<IInternalCache, NoopInternalCache>();
            }

            return new GroupDocsComparisonUIApiBuilder(builder.Services);
        }
    }
}
