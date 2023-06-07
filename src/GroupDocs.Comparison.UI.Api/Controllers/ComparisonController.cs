using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GroupDocs.Comparison.UI.Api.Infrastructure;
using GroupDocs.Comparison.UI.Api.Models;
using GroupDocs.Comparison.UI.Core;
using GroupDocs.Comparison.UI.Core.Configuration;
using GroupDocs.Comparison.UI.Core.Entities;
//using GroupDocs.Comparison.UI.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GroupDocs.Comparison.UI.Api.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ComparisonController : ControllerBase
    {
        private readonly IFileStorage _fileStorage;
        private readonly IFileNameResolver _fileNameResolver;
        private readonly ISearchTermResolver _searchTermResolver;
        private readonly IUIConfigProvider _uiConfigProvider;
        private readonly IComparison _comparison;
        private readonly ILogger<ComparisonController> _logger;
        private readonly Config _config;

        public ComparisonController(IFileStorage fileStorage,
            IFileNameResolver fileNameResolver,
            ISearchTermResolver searchTermResolver,
            IUIConfigProvider uiConfigProvider,
            IComparison comparison,
            IOptions<Config> config,
            ILogger<ComparisonController> logger)
        {
            _fileStorage = fileStorage;
            _fileNameResolver = fileNameResolver;
            _searchTermResolver = searchTermResolver;
            _comparison = comparison;
            _logger = logger;
            _config = config.Value;
            _uiConfigProvider = uiConfigProvider;
        }

        [HttpGet]
        public IActionResult LoadConfig()
        {
            var config = new LoadConfigResponse
            {
                PageSelector = _config.PageSelector,
                Download = _config.Download,
                Upload = _config.Upload,
                Print = _config.Print,
                Browse = _config.Browse,
                Rewrite = _config.Rewrite,
                EnableRightClick = _config.EnableRightClick
            };

            _uiConfigProvider.ConfigureUI(_config);

            return OkJsonResult(config);
        }

        [HttpPost]
        public async Task<IActionResult> LoadFileTree([FromBody] LoadFileTreeRequest request)
        {
            if (!_config.Browse)
                return ErrorJsonResult("Browsing files is disabled.");

            try
            {
                var files =
                    await _fileStorage.ListDirsAndFilesAsync(request.Path);

                var result = files
                    .Select(entity => new FileDescription(entity.FilePath, entity.FilePath, entity.IsDirectory, entity.Size))
                    .ToList();

                return OkJsonResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load file tree.");

                return ErrorJsonResult(ex.Message);
            }
        }

           [HttpGet]
           public async Task<IActionResult> DownloadDocument([FromQuery] string path)
           {
               if (!_config.Download)
                   return ErrorJsonResult("Downloading files is disabled.");

               try
               {
                   var fileName = await _fileNameResolver.ResolveFileNameAsync(path);
                   var bytes = await _fileStorage.ReadFileAsync(path);

                   return File(bytes, "application/octet-stream", fileName);
               }
               catch (Exception ex)
               {
                   _logger.LogError(ex, "Failed to download a document.");

                   return ErrorJsonResult(ex.Message);
               }
           }

           [HttpPost]
           public async Task<IActionResult> UploadDocument()
           {
               if (!_config.Upload)
                   return ErrorJsonResult("Uploading files is disabled.");

               try
               {
                   var (fileName, bytes) = await ReadOrDownloadFile();
                   bool.TryParse(Request.Form["rewrite"], out var rewrite);

                   var filePath = await _fileStorage.WriteFileAsync(fileName, bytes, rewrite);

                   var result = new UploadFileResponse(filePath);

                   return OkJsonResult(result);
               }
               catch (Exception ex)
               {
                   _logger.LogError(ex, "Failed to upload document.");

                   return ErrorJsonResult(ex.Message);
               }
           }

           [HttpPost]
           public async Task<IActionResult> LoadDocumentDescription([FromBody] LoadDocumentDescriptionRequest request)
           {
               try
               {
                   var fileCredentials =
                       new FileCredentials(request.Guid, request.FileType, request.Password);
                   var documentDescription =
                       await _comparison.GetDocumentInfoAsync(fileCredentials);

                   //var pageNumbers = GetPageNumbers(documentDescription.Pages.Count());
                   //var pagesData = await _comparison.GetPagesAsync(fileCredentials, pageNumbers);

                   var pages = new List<PageDescription>();
                   var searchTerm = await _searchTermResolver.ResolveSearchTermAsync(request.Guid);
/*                   foreach (PageInfo pageInfo in documentDescription.Pages)
                   {
                       //var pageData = pagesData.FirstOrDefault(p => p.PageNumber == pageInfo.Number);
                       var pageDescription = new PageDescription
                       {
                           Width = pageInfo.Width,
                           Height = pageInfo.Height,
                           Number = pageInfo.Number,
                           SheetName = pageInfo.Name,
                           //Data = pageData?.GetContent()
                       };

                       pages.Add(pageDescription);
                   }*/

                   var result = new LoadDocumentDescriptionResponse
                   {
                       Guid = request.Guid,
                       FileType = documentDescription.FileType,
                      /* PrintAllowed = documentDescription.PrintAllowed,*/
                       Pages = pages,
                       //SearchTerm = searchTerm
                   };

                   return OkJsonResult(result);
               }
               catch (Exception ex)
               {
                   if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                   {
                       var message = string.IsNullOrEmpty(request.Password)
                               ? "Password Required"
                               : "Incorrect Password";

                       return ForbiddenJsonResult(message);
                   }

                   _logger.LogError(ex, "Failed to read document description.");

                   return ErrorJsonResult(ex.Message);
               }
           }

        /*
           [HttpPost]
           public async Task<IActionResult> LoadDocumentPage([FromBody] LoadDocumentPageRequest request)
           {
               try
               {
                   var fileCredentials =
                       new FileCredentials(request.Guid, request.FileType, request.Password);
                   var page = await _comparison.GetPageAsync(fileCredentials, request.Page);
                   var pageContent = new PageContent { Number = page.PageNumber, Data = page.GetContent() };

                   return OkJsonResult(pageContent);
               }
               catch (Exception ex)
               {
                   if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                   {
                       var message = string.IsNullOrEmpty(request.Password)
                           ? "Password Required"
                           : "Incorrect Password";

                       return ForbiddenJsonResult(message);
                   }

                   _logger.LogError(ex, "Failed to retrieve document page.");

                   return ErrorJsonResult(ex.Message);
               }
           }
*/
           private Task<(string, byte[])> ReadOrDownloadFile()
           {
               var url = Request.Form["url"].ToString();

               return string.IsNullOrEmpty(url)
                   ? ReadFileFromRequest()
                   : DownloadFileAsync(url);
           }

           private async Task<(string, byte[])> ReadFileFromRequest()
           {
               var formFile = Request.Form.Files.First();
               var stream = new MemoryStream();

               await formFile.CopyToAsync(stream);

               return (formFile.FileName, stream.ToArray());
           }

           private async Task<(string, byte[])> DownloadFileAsync(string url)
           {
               using HttpClient httpClient = new HttpClient();
               httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");

               Uri uri = new Uri(url);
               string fileName = Path.GetFileName(uri.LocalPath);
               byte[] bytes = await httpClient.GetByteArrayAsync(uri);

               return (fileName, bytes);
           }
        
        private IActionResult ErrorJsonResult(string message) =>
            new ComparisonActionResult(new ErrorResponse(message))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

        private IActionResult ForbiddenJsonResult(string message) =>
            new ComparisonActionResult(new ErrorResponse(message))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

        private IActionResult NotFoundJsonResult(string message) =>
            new ComparisonActionResult(new ErrorResponse(message))
            {
                StatusCode = StatusCodes.Status404NotFound
            };

        private IActionResult OkJsonResult(object result) =>
            new ComparisonActionResult(result);
    }
}
