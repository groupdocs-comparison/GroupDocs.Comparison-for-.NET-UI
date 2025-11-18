# UI for GroupDocs.Comparison for .NET

![Build Packages](https://github.com/groupdocs-comparison/GroupDocs.Comparison-for-.NET-UI/actions/workflows/build_packages.yml/badge.svg)

![GroupDocs.Comparison.UI](https://raw.githubusercontent.com/groupdocs-comparison/groupdocs-comparison.github.io/master/resources/image/ui/comparison-ui.png)

GroupDocs.Comparison UI is a rich UI interface that designed to work in conjunction with [GroupDocs.Comparison for .NET](https://products.groupdocs.com/comparison/net) to compare file and document formats in a browser.

To integrate GroupDocs.Comparison UI in your ASP.NET Core project you just need to add services and middlewares into your `Startup` class that provided in `GroupDocs.Comparison.UI` and related packages.

Include packages in your project:

```bash
dotnet add package GroupDocs.Comparison.UI
dotnet add package GroupDocs.Comparison.UI.SelfHost.Api
dotnet add package GroupDocs.Comparison.UI.Api.Local.Storage
dotnet add package GroupDocs.Comparison.UI.Api.Local.Cache
```

Add configuration to your `Startup` class:

```cs
using GroupDocs.Comparison.UI.Core;
using GroupDocs.Comparison.UI.Extensions;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddGroupDocsComparisonUI(config =>
            {
                config.SetFilesDirectory("./Files");
            });

        services
            .AddControllers()
            .AddGroupDocsComparisonSelfHostApi(config =>
            {
                //Trial limitations https://docs.groupdocs.com/comparison/net/licensing-and-evaluation-limitations/
                //Temporary license can be requested at https://purchase.groupdocs.com/temp-license/100078
                //config.SetLicensePath("c:\\licenses\\GroupDocs.Comparison.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
            })
            .AddLocalStorage("./Temp")
            .AddLocalCache("./Cache");
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapGroupDocsComparisonUI(options =>
                {
                    options.UIPath = "/comparison";
                    options.APIEndpoint = "/comparison-api";
                });

                endpoints.MapGroupDocsComparisonApi(options =>
                {
                    options.ApiPath = "/comparison-api";
                });
            });
    }
}
```

Or, if you’re using [new program](https://docs.microsoft.com/en-us/dotnet/core/tutorials/top-level-templates) style with top-level statements, global using directives, and implicit using directives the Program.cs will be a bit shorter.

```cs
using GroupDocs.Comparison.UI.Core;
using GroupDocs.Comparison.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
   .AddGroupDocsComparisonUI(config =>
   {
       config.SetFilesDirectory("./Files");
   });
   
builder.Services
   .AddControllers()
   .AddGroupDocsComparisonSelfHostApi(config =>
   {
       //Trial limitations https://docs.groupdocs.com/comparison/net/licensing-and-evaluation-limitations/
       //Temporary license can be requested at https://purchase.groupdocs.com/temp-license/100078
       //config.SetLicensePath("c:\\licenses\\GroupDocs.Comparison.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
   })
   .AddLocalStorage("./Temp")
   .AddLocalCache("./Cache");

var app = builder.Build();

app
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapGroupDocsComparisonUI(options =>
        {
            options.UIPath = "/comparison";
            options.APIEndpoint = "/comparison-api";
        });

        endpoints.MapGroupDocsComparisonApi(options =>
        {
            options.ApiPath = "/comparison-api";
        });
    });

await app.RunAsync();
```

This code registers **/comparison** middleware that will serve SPA and **/comparison-api** middleware that will serve content for the UI to display.

 **Please note that Comparison does not create `Files` and `Temp` folders, please make sure to create `Files` and `Temp` folders manually before running the application.**

## UI

The UI is Angular SPA that is build upon [@groupdocs.examples.angular/comparison](https://www.npmjs.com/package/@groupdocs.examples.angular/comparison) package. You can change the path where the SPA will be available by setting `UIPath` property e.g.

```cs
endpoints.MapGroupDocsComparisonUI(options =>
{
    options.UIPath = "/my-comparison-app";
});
```

## API

The API is used to serve content such as information about a document, document pages in HTML/PNG/JPG format and PDF file for printing. The API can be hosted in the same or a separate application. The following API implementations available at the moment:

- GroupDocs.Comparison.UI.SelfHost.Api

All the API implementations are extensions of `IMvcBuilder`:

### Self-Host

Self-Host API uses [GroupDocs.Comparison for .NET](https://www.nuget.org/packages/groupdocs.comparison) to convert documents to HTML, PNG, JPG, and PDF. All the conversions are performed on the host where the application is running.

```cs
services
    .AddControllers()
    .AddGroupDocsComparisonSelfHostApi();
```

GroupDocs.Comparison for .NET requires license to skip [trial limitations](https://docs.groupdocs.com/comparison/net/licensing-and-evaluation-limitations/). A temporary license can be requested at [Get a Temporary License](https://purchase.groupdocs.com/temporary-license).

Use the following code to set a license:

```cs
services
    .AddControllers()
    .AddGroupDocsComparisonSelfHostApi(config =>
    {
        config.SetLicensePath("./GroupDocs.Comparison.lic");
    })
```

#### Linux dependencies

When running Self-Host API on Linux the following packages have to be installed:

- `apt-transport-https`
- `dirmngr`
- `gnupg`
- `libc6-dev`
- `libgdiplus`
- `libx11-dev`
- `ttf-mscorefonts-installer`

As an example the following commands should be executed to install the dependencies on [Ubuntu 18.04.5 LTS (Bionic Beaver)](https://releases.ubuntu.com/18.04.5/):

- `apt-get update`
- `apt-get install -y apt-transport-https`
- `apt-get install -y dirmngr`
- `apt-get install -y gnupg`
- `apt-get install -y ca-certificates`
- `apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys $ 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF`
- `echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic $ main" >> /etc/apt/sources.list.d/mono-official-stable.list`
- `apt-get update`
- `apt-get install -y --allow-unauthenticated libc6-dev libgdiplus libx11-dev`
- `apt-get install -y ttf-mscorefonts-installer`

### API Storage Providers

Storage providers are used to read/write file from/to the storage. The storage provider is mandatory.

- GroupDocs.Comparison.UI.Api.Local.Storage

All the storage providers are extensions of `GroupDocsComparisonUIApiBuilder`:

#### Local Storage

To render files from your local drive use local file storage.

```cs
services
    .AddControllers()
    .AddGroupDocsComparisonSelfHostApi()
    .AddLocalStorage("./Files");
```

### API Cache Providers

In case you would like to cache the output files produced by GroupDocs.Comparison you can use one of the cache providers:

- GroupDocs.Comparison.UI.Api.Local.Cache

All the cache providers are extensions of `GroupDocsComparisonUIApiBuilder`:

#### Local Cache

```cs
services
    .AddControllers()
    .AddGroupDocsComparisonSelfHostApi()
    .AddLocalStorage("./Files")
    .AddLocalCache("./Cache");
```

## Contributing

Your contributions are welcome when you want to make the project better by adding new feature, improvement or a bug-fix.

1. Read and follow the [Don't push your pull requests](https://www.igvita.com/2011/12/19/dont-push-your-pull-requests/)
2. Follow the code guidelines and conventions.
3. Make sure to describe your pull requests well and add documentation.
