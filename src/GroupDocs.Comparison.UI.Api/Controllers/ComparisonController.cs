using GroupDocs.Comparison.Common.Exceptions;
using GroupDocs.Comparison.UI.Api.Config;
using GroupDocs.Comparison.UI.Api.Entity;
using GroupDocs.Comparison.UI.Api.Models;
using GroupDocs.Comparison.UI.Api.Util;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Resources;


namespace GroupDocs.Comparison.UI.Api.Controllers
{
    /// <summary>
    /// SignatureApiController
    /// </summary>
    public class ComparisonController : ControllerBase
    {
        private readonly IComparisonService comparisonService;
        private static readonly GlobalConfiguration globalConfiguration = new GlobalConfiguration();
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="globalConfiguration">GlobalConfiguration</param>
        public ComparisonController()
        {
            comparisonService = new ComparisonServiceImpl(globalConfiguration);
        }

        /// <summary>
        /// Load Comparison configuration
        /// </summary>
        /// <returns>Comparison configuration</returns>
        [HttpGet]
        public ComparisonConfiguration LoadConfig()
        {
            return globalConfiguration.Comparison;
        }

        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        public ActionResult LoadFileTree(PostedDataEntity fileTreeRequest)
        {
            var files = comparisonService.LoadFiles(fileTreeRequest);
            return Ok(files);
        }


        /// <summary>
        /// Download results
        /// </summary>
        /// <param name=""></param>
        [HttpGet]
        public IActionResult DownloadDocument(string guid)
        {
            string filePath = guid;
            if (!string.IsNullOrEmpty(filePath))
            {
                if (System.IO.File.Exists(filePath))
                {
                    var fileStream = new FileStream(filePath, FileMode.Open);
                    var fileContent = new byte[fileStream.Length];
                    fileStream.Read(fileContent, 0, (int)fileStream.Length);

                    var contentTypeProvider = new FileExtensionContentTypeProvider();
                    if (!contentTypeProvider.TryGetContentType(filePath, out var contentType))
                    {
                        contentType = "application/octet-stream";
                    }

                    return File(fileContent, contentType, Path.GetFileName(filePath));
                }
            }
            return NotFound();
        }


        /// <summary>
        /// Upload document
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadDocument()
        {
            try
            {
                string url = Request.Form["url"];
                // get documents storage path
                string documentStoragePath = globalConfiguration.Comparison.GetFilesDirectory();
                bool rewrite = bool.Parse(Request.Form["rewrite"]);
                string fileSavePath = "";
                if (string.IsNullOrEmpty(url))
                {
                    if (Request.Form.Files.Count > 0)
                    {
                        var httpPostedFile = Request.Form.Files[0];
                        if (httpPostedFile != null)
                        {
                            if (rewrite)
                            {
                                // Get the complete file path
                                fileSavePath = Path.Combine(documentStoragePath, httpPostedFile.FileName);
                            }
                            else
                            {
                                fileSavePath = Resources.GetFreeFileName(documentStoragePath, httpPostedFile.FileName);
                            }

                            using (var fileStream = new FileStream(fileSavePath, FileMode.Create))
                            {
                                httpPostedFile.CopyTo(fileStream);
                            }
                        }
                    }
                }
                else
                {
                    using (WebClient client = new WebClient())
                    {
                        // get file name from the URL
                        Uri uri = new Uri(url);
                        string fileName = Path.GetFileName(uri.LocalPath);
                        if (rewrite)
                        {
                            // Get the complete file path
                            fileSavePath = Path.Combine(documentStoragePath, fileName);
                        }
                        else
                        {
                            fileSavePath = Resources.GetFreeFileName(documentStoragePath, fileName);
                        }
                        // Download the Web resource and save it into the current filesystem folder.
                        client.DownloadFile(url, fileSavePath);
                    }
                }
                UploadedDocumentEntity uploadedDocument = new UploadedDocumentEntity();
                uploadedDocument.guid = fileSavePath;
                return Ok(uploadedDocument);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return StatusCode(500, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Compare files from local storage
        /// </summary>
        /// <param name="compareRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Compare(CompareRequest compareRequest)
        {
            try
            {
                // Проверяем форматы
                if (comparisonService.CheckFiles(compareRequest))
                {
                    // Выполняем сравнение
                    CompareResultResponse result = comparisonService.Compare(compareRequest);
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ContractResolver = new LowercaseContractResolver();
                    string json = JsonConvert.SerializeObject(result, Formatting.Indented, settings);
                    var compareResult = JsonConvert.DeserializeObject(json);
                    return Ok(compareResult);
                }
                else
                {
                    return Ok(new Resources().GenerateException(new Exception("Document types are different")));
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }


        /// Set new changes in result file
        /// </summary>
        /// <param name="compareRequest"></param>
        /// <param name="listOfChanges"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Changes(SetChangesRequest setChangesRequest)
        {
            try
            {
                CompareResultResponse result = comparisonService.SetChanges(setChangesRequest);
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ContractResolver = new LowercaseContractResolver();
                string json = JsonConvert.SerializeObject(result, Formatting.Indented, settings);
                var compareResult = JsonConvert.DeserializeObject(json);
                return Ok(compareResult);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Get result page
        /// </summary>
        /// <param name="loadResultPageRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadDocumentDescription(PostedDataEntity loadResultPageRequest)
        {
            try
            {
                LoadDocumentEntity document = ComparisonServiceImpl.LoadDocumentPages(loadResultPageRequest.guid, loadResultPageRequest.password, globalConfiguration.Comparison.GetPreloadResultPageCount() == 0);
                return Ok(document);
            }
            catch (PasswordProtectedFileException ex)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new Resources().GenerateException(ex, loadResultPageRequest.password));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new Resources().GenerateException(ex, loadResultPageRequest.password));
            }
        }


        /// <summary>
        /// Get document page
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>Document page object</returns>
        [HttpPost]
        public async Task<IActionResult> LoadDocumentPage(PostedDataEntity postedData)
        {
            try
            {
                var documentPage = comparisonService.LoadDocumentPage(postedData);
                return Ok(documentPage);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

    }
}