using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GroupDocs.Comparison.Common.Exceptions;
using GroupDocs.Comparison.Interfaces;
using GroupDocs.Comparison.UI.Api.Entity;
using GroupDocs.Comparison.UI.Api.Infrastructure;
using GroupDocs.Comparison.UI.Api.Models;
using GroupDocs.Comparison.UI.Core;
using GroupDocs.Comparison.UI.Core.Configuration;
using GroupDocs.Comparison.Options;
using GroupDocs.Comparison.UI.Core.Entities;
//using GroupDocs.Comparison.UI.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GroupDocs.Comparison.UI.Api.Util.Comparator;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Drawing2D;
using GroupDocs.Comparison.Result;

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
            // get request body
            string relDirPath = request.Path;
            // get file list from storage path
            try
            {
                // get all the files from a directory
                if (string.IsNullOrEmpty(relDirPath))
                {
                    relDirPath = _config.FilesDirectory;
                }
                else
                {
                    relDirPath = Path.Combine(_config.FilesDirectory, relDirPath);
                }

                List<string> allFiles = new List<string>(Directory.GetFiles(relDirPath));
                allFiles.AddRange(Directory.GetDirectories(relDirPath));
                List<FileDescriptionEntity> fileList = new List<FileDescriptionEntity>();

                allFiles.Sort(new FileNameComparator());
                allFiles.Sort(new FileTypeComparator());

                foreach (string file in allFiles)
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
                    // check if current file/folder is hidden
                    if (!(fileInfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                        Path.GetFileName(file).StartsWith(".") ||
                        Path.GetFileName(file).Equals(Path.GetFileName(_config.FilesDirectory)) ||
                        Path.GetFileName(file).Equals(Path.GetFileName(_config.FilesDirectory))))
                    {
                        FileDescriptionEntity fileDescription = new FileDescriptionEntity
                        {
                            guid = Path.GetFullPath(file),
                            name = Path.GetFileName(file),
                            // set is directory true/false
                            isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory)
                        };
                        // set file size
                        if (!fileDescription.isDirectory)
                        {
                            fileDescription.size = fileInfo.Length;
                        }
                        // add object to array list
                        fileList.Add(fileDescription);
                    }
                }
                return OkJsonResult(fileList);
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
        public IActionResult LoadDocumentDescription([FromBody] LoadDocumentDescriptionRequest request)
        {
            try
            {
                LoadDocumentEntity loadDocumentEntity = new LoadDocumentEntity();

                using (Comparer comparer = new Comparer(request.Guid, GetLoadOptions(request.Password)))
                {
                    IDocumentInfo documentInfo = comparer.Source.GetDocumentInfo();
                    if (documentInfo.PagesInfo == null)
                    {
                        throw new GroupDocs.Comparison.Common.Exceptions.ComparisonException("File is corrupted.");
                    }
                    List<PageDescription> PageDescriptions = new List<PageDescription>();
                    for (int i = 0; i < documentInfo.PageCount; i++)
                    {
                        string encodedImage = GetPageData(i, request.Guid, request.Password);
                        PageDescription page = new PageDescription()
                        {
                            Height = documentInfo.PagesInfo[i].Height,
                            Width = documentInfo.PagesInfo[i].Width,
                            Data = encodedImage
                        };
                        PageDescriptions.Add(page);
                    }
                    var result = new LoadDocumentDescriptionResponse
                    {
                        Guid = request.Guid,
                        FileType = request.FileType,
                        /*to do PrintAllowed = request.PrintAllowed,*/
                        Pages = PageDescriptions,
                        /*to do SearchTerm = searchTerm*/
                    };

                    return OkJsonResult(result);
                }
            }
            catch (PasswordProtectedFileException ex)
            {
                var message = string.IsNullOrEmpty(request.Password)
                        ? "Password Required"
                        : "Incorrect Password";

                return ForbiddenJsonResult(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read document description.");

                return ErrorJsonResult(ex.Message);
            }
        }


        /*   [HttpPost]
           public async Task<IActionResult> LoadDocumentPage([FromBody] LoadDocumentPageRequest request)
           {
               try
               {
                   PageDescriptionEntity loadedPage = new PageDescriptionEntity();

                   // get/set parameters
                   string documentGuid = request.Guid;
                   int pageNumber = request.Page;
                   string password = (string.IsNullOrEmpty(request.Password)) ? null : request.Password;

                   using (Comparer comparer = new Comparer(documentGuid, GetLoadOptions(password)))
                   {
                       IDocumentInfo info = comparer.Source.GetDocumentInfo();

                       string encodedImage = GetPageData(pageNumber - 1, documentGuid, password);
                       loadedPage.SetData(encodedImage);

                       loadedPage.height = info.PagesInfo[pageNumber - 1].Height;
                       loadedPage.width = info.PagesInfo[pageNumber - 1].Width;
                       loadedPage.number = pageNumber;
                   }
                   return OkJsonResult(loadedPage);
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
           }*/
        [HttpPost]
        public IActionResult Compare([FromBody] CompareRequest request)
        {
            try
            {
                CompareResponse compareResultResponse = CompareTwoDocuments(request);
                return OkJsonResult(compareResultResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compare documents.");

                return ErrorJsonResult(ex.Message);
            }
        }

        private CompareResponse CompareTwoDocuments(CompareRequest compareRequest)
        {
            // to get correct coordinates we will compare document twice
            // this is a first comparing to get correct coordinates of the insertions and style changes
            string extension = Path.GetExtension(compareRequest.guids[0].guid);
            string guid = Guid.NewGuid().ToString();
            //save all results in file
            string resultGuid = Path.Combine(_config.ResultDirectory, guid + extension);

            Comparer compareResult = CompareFiles(compareRequest, resultGuid);
            ChangeInfo[] changes = compareResult.GetChanges();

            CompareResponse compareResultResponse = GetCompareResultResponse(changes, resultGuid);
            compareResultResponse.SetExtension(extension);
            return compareResultResponse;
        }

        private static Comparer CompareFiles(CompareRequest compareRequest, string resultGuid)
        {
            string firstPath = compareRequest.guids[0].guid;
            string secondPath = compareRequest.guids[1].guid;

            // create new comparer
            Comparer comparer = new Comparer(firstPath, GetLoadOptions(compareRequest.guids[0].password));

            comparer.Add(secondPath, GetLoadOptions(compareRequest.guids[1].password));
            CompareOptions compareOptions = new CompareOptions { CalculateCoordinates = true };

            if (Path.GetExtension(resultGuid) == ".pdf")
            {
                compareOptions.DetalisationLevel = DetalisationLevel.High;
            }

            using (FileStream outputStream = System.IO.File.Create(Path.Combine(resultGuid)))
            {
                comparer.Compare(outputStream, compareOptions);
            }

            return comparer;
        }

        private static CompareResponse GetCompareResultResponse(ChangeInfo[] changes, string resultGuid)
        {
            CompareResponse compareResultResponse = new CompareResponse();
            compareResultResponse.SetChanges(changes);

            List<PageDescriptionEntity> pages = LoadDocumentPages(resultGuid, "", true).GetPages();

            compareResultResponse.SetPages(pages);
            compareResultResponse.SetGuid(resultGuid);
            return compareResultResponse;
        }

        public static LoadDocumentEntity LoadDocumentPages(string documentGuid, string password, bool loadAllPages)
        {
            LoadDocumentEntity loadDocumentEntity = new LoadDocumentEntity();

            using (Comparer comparer = new Comparer(documentGuid, GetLoadOptions(password)))
            {
                Dictionary<int, string> pagesContent = new Dictionary<int, string>();
                IDocumentInfo documentInfo = comparer.Source.GetDocumentInfo();
                if (documentInfo.PagesInfo == null)
                {
                    throw new GroupDocs.Comparison.Common.Exceptions.ComparisonException("File is corrupted.");
                }

                if (loadAllPages)
                {
                    for (int i = 0; i < documentInfo.PageCount; i++)
                    {
                        string encodedImage = GetPageData(i, documentGuid, password);

                        pagesContent.Add(i, encodedImage);
                    }
                }

                for (int i = 0; i < documentInfo.PageCount; i++)
                {
                    PageDescriptionEntity pageData = new PageDescriptionEntity
                    {
                        height = documentInfo.PagesInfo[i].Height,
                        width = documentInfo.PagesInfo[i].Width,
                        number = i + 1
                    };

                    if (pagesContent.Count > 0)
                    {
                        pageData.SetData(pagesContent[i]);
                    }

                    loadDocumentEntity.SetPages(pageData);
                }

                return loadDocumentEntity;
            }
        }

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
        private static string GetPageData(int pageNumber, string documentGuid, string password)
        {
            string encodedImage = "";

            using (Comparer comparer = new Comparer(documentGuid, GetLoadOptions(password)))
            {
                byte[] bytes = RenderPageToMemoryStream(comparer, pageNumber).ToArray();
                encodedImage = Convert.ToBase64String(bytes);
            }

            return encodedImage;
        }
        private static LoadOptions GetLoadOptions(string password)
        {
            LoadOptions loadOptions = new LoadOptions
            {
                Password = password
            };

            return loadOptions;
        }
        static MemoryStream RenderPageToMemoryStream(Comparer comparer, int pageNumberToRender)
        {
            MemoryStream result = new MemoryStream();
            IDocumentInfo documentInfo = comparer.Source.GetDocumentInfo();

            PreviewOptions previewOptions = new PreviewOptions(pageNumber => result)
            {
                PreviewFormat = PreviewFormats.PNG,
                PageNumbers = new[] { pageNumberToRender + 1 },
                Height = documentInfo.PagesInfo[pageNumberToRender].Height,
                Width = documentInfo.PagesInfo[pageNumberToRender].Width
            };

            comparer.Source.GeneratePreview(previewOptions);

            return result;
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
