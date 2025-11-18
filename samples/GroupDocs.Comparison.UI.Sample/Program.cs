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
        endpoints.MapGet("/", async context =>
        {
            await context.Response.SendFileAsync("index.html");
        });

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