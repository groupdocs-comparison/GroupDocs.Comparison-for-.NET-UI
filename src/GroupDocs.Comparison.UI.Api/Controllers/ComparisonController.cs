using GroupDocs.Comparison.Common.Exceptions;
using GroupDocs.Comparison.Interfaces;
using GroupDocs.Comparison.UI.Api.Entity;
using GroupDocs.Comparison.UI.Api.Infrastructure;
using GroupDocs.Comparison.UI.Api.Models;
using GroupDocs.Comparison.UI.Core;
using GroupDocs.Comparison.UI.Core.Configuration;
using GroupDocs.Comparison.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GroupDocs.Comparison.UI.Api.Util.Comparator;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Drawing2D;
using GroupDocs.Comparison.Result;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Resources;

namespace GroupDocs.Comparison.UI.Api.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ComparisonController : ControllerBase
    {
        private readonly IFileStorage _fileStorage;
        private readonly IFileNameResolver _fileNameResolver;
        private readonly IComparisonLicenser _comparisonLicenser;
        private readonly IUIConfigProvider _uiConfigProvider;
        private readonly ILogger<ComparisonController> _logger;
        private readonly Config _config;

        public ComparisonController(IFileStorage fileStorage,
            IFileNameResolver fileNameResolver,
            IComparisonLicenser comparisonLicenser,
            IUIConfigProvider uiConfigProvider,
            IOptions<Config> config,
            ILogger<ComparisonController> logger)
        {
            _fileStorage = fileStorage;
            _fileNameResolver = fileNameResolver;
            _comparisonLicenser = comparisonLicenser;
            _logger = logger;
            _config = config.Value;
            _uiConfigProvider = uiConfigProvider;
        }

        [HttpGet]
        public IActionResult LoadConfig()
        {
            var config = new LoadConfigResponse
            {
                Download = _config.Download,
                FilesDirectory = _config.FilesDirectory
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
        public async Task<IActionResult> DownloadDocument([FromQuery] DownloadDocumentRequest request)
        {
            if (!_config.Download)
                return ErrorJsonResult("Downloading files is disabled.");

            try
            {
                var fileName = await _fileNameResolver.ResolveFileNameAsync("Result" + Path.GetExtension(request.Guid));
                var bytes = await _fileStorage.ReadFileAsync(Path.GetFileName(request.Guid));

                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download a document.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult LoadDocumentDescription([FromBody] LoadDocumentDescriptionRequest request)
        {
            try
            {
                _comparisonLicenser.SetLicense();
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
                            Data = encodedImage,
                            Number = documentInfo.PagesInfo[i].PageNumber
                        };
                        PageDescriptions.Add(page);
                    }
                    var result = new LoadDocumentDescriptionResponse
                    {
                        Guid = request.Guid,
                        FileType = request.FileType,
                        Pages = PageDescriptions,
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

        [HttpPost]
        public IActionResult Changes([FromBody]SetChangesRequest setChangesRequest)
        {
            try
            {
                CompareResponse compareResultResponse = SetChanges(setChangesRequest);
                return OkJsonResult(compareResultResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compare documents.");

                return ErrorJsonResult(ex.Message);
            }
        }

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
            string resultGuid = Path.Combine(_fileStorage.GetFileStoragePath(), guid + extension);

            Comparer compareResult = CompareFiles(compareRequest, resultGuid);
            ChangeInfo[] changes = compareResult.GetChanges();

            CompareResponse compareResultResponse = GetCompareResultResponse(changes, resultGuid);
            compareResultResponse.FileType = extension;
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
            List<CompareChangeInfo> ChangeList = new List<CompareChangeInfo>();
            for (int i = 0; i < changes.Length; i++)
            {
                var comparePageInfo = new ComparePageInfo()
                {
                    PageNumber = changes[i].PageInfo.PageNumber,
                    Width = changes[i].PageInfo.Width,
                    Height = changes[i].PageInfo.Height
                };

                var compareBox = new CompareBox()
                {
                    Height = changes[i].Box.Height,
                    Width = changes[i].Box.Width,
                    X = changes[i].Box.X,
                    Y = changes[i].Box.Y,
                };

                var compareChangeInfo = new CompareChangeInfo()
                {
                    PageInfo = comparePageInfo,
                    Box = compareBox
                };

                ChangeList.Add(compareChangeInfo);
            }


            compareResultResponse.Changes = ChangeList;
            compareResultResponse.Pages = (LoadDocumentPages(resultGuid, "", true));
            compareResultResponse.Guid = resultGuid;
            return compareResultResponse;
        }

        public static List<PageDescription> LoadDocumentPages(string documentGuid, string password, bool loadAllPages)
        {
            List<PageDescription> pagesDescription = new List<PageDescription>();

            using (Comparer comparer = new Comparer(documentGuid, GetLoadOptions(password)))
            {
                Dictionary<int, string> pagesContent = new Dictionary<int, string>();
                IDocumentInfo documentInfo = comparer.Source.GetDocumentInfo();
                if (documentInfo.PagesInfo == null)
                {
                    throw new GroupDocs.Comparison.Common.Exceptions.ComparisonException("File is corrupted.");
                }

                for (int i = 0; i < documentInfo.PageCount; i++)
                {
                    string encodedImage = GetPageData(i, documentGuid, password);
                    PageDescription page = new PageDescription()
                    {
                        Height = documentInfo.PagesInfo[i].Height,
                        Width = documentInfo.PagesInfo[i].Width,
                        Data = encodedImage,
                        Number = documentInfo.PagesInfo[i].PageNumber
                    };
                    pagesDescription.Add(page);
                }

                return pagesDescription;
            }
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

        public CompareResponse SetChanges(SetChangesRequest setChangesRequest)
        {
            string extension = Path.GetExtension(setChangesRequest.guids[0].guid);
            string guid = Guid.NewGuid().ToString();
            string resultGuid = Path.Combine(_fileStorage.GetFileStoragePath(), guid + extension);

            string firstPath = setChangesRequest.guids[0].guid;
            string secondPath = setChangesRequest.guids[1].guid;

            Comparer compareResult = new Comparer(firstPath, GetLoadOptions(setChangesRequest.guids[0].password));

            compareResult.Add(secondPath, GetLoadOptions(setChangesRequest.guids[1].password));
            CompareOptions compareOptions = new CompareOptions { CalculateCoordinates = true };
            if (Path.GetExtension(resultGuid) == ".pdf")
            {
                compareOptions.DetalisationLevel = DetalisationLevel.High;
            }
            using (FileStream outputStream = System.IO.File.Create(Path.Combine(resultGuid)))
            {
                compareResult.Compare(outputStream, compareOptions);
            }
            ChangeInfo[] changes = compareResult.GetChanges();

            for (int i = 0; i < setChangesRequest.changeTypes.Length; i++)
            {
                ComparisonAction action = ComparisonAction.None;
                switch (setChangesRequest.changeTypes[i])
                {
                    case 1:
                        action = ComparisonAction.Accept;
                        break;
                    case 2:
                        action = ComparisonAction.Reject;
                        break;
                    case 3:
                        action = ComparisonAction.None;
                        break;
                }
                changes[i].ComparisonAction = action;
            }

            compareResult.ApplyChanges(resultGuid, new SaveOptions(), new ApplyChangeOptions() { Changes = changes });

            CompareResponse compareResultResponse = GetCompareResultResponse(changes, resultGuid);
            compareResultResponse.FileType = extension;

            return compareResultResponse;
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

        private IActionResult OkJsonResult(object result) =>
            new ComparisonActionResult(result);
    }
}