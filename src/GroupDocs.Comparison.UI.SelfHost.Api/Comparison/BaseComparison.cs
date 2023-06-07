using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupDocs.Comparison.Options;
using GroupDocs.Comparison.Result;
using GroupDocs.Comparison.UI.Core;
using GroupDocs.Comparison.UI.Core.Entities;
using GroupDocs.Comparison.UI.SelfHost.Api.Configuration;
using GroupDocs.Comparison.UI.SelfHost.Api.InternalCaching;
using GroupDocs.Comparison.UI.SelfHost.Api.Licensing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Options;
using Page = GroupDocs.Comparison.UI.Core.Entities.Page;

namespace GroupDocs.Comparison.UI.SelfHost.Api.Comparison
{
    internal class BaseComparison : IComparison, IDisposable
    {
        private readonly Config _config;
        private readonly IAsyncLock _asyncLock;
        private readonly IComparisonLicenser _comparisonLicenser;
        private readonly IInternalCache _comparisonCache;
        private readonly InternalCacheOptions _internalCacheOptions;
        private readonly IFileStorage _fileStorage;
        private readonly IFileTypeResolver _fileTypeResolver;
       // private readonly IPageFormatter _pageFormatter;
        private Comparer _comparer;

        public BaseComparison(
              IOptions<Config> config,
              IAsyncLock asyncLock,
              IComparisonLicenser comparisonLicenser,
              IInternalCache viewerCache,
              IFileStorage fileStorage,
              IFileTypeResolver fileTypeResolver
             /* IPageFormatter pageFormatter*/)
        {
            _config = config.Value;
            _asyncLock = asyncLock;
            _comparisonLicenser = comparisonLicenser;
            _comparisonCache = viewerCache;
            _internalCacheOptions = config.Value.InternalCacheOptions;
            _fileStorage = fileStorage;
            _fileTypeResolver = fileTypeResolver;
            //_pageFormatter = pageFormatter;
        }

        public async Task<DocumentInfo> GetDocumentInfoAsync(FileCredentials fileCredentials)
        {
            var comparison = await InitComparisonAsync(fileCredentials);
/*            var CompareOptions = CreateCompareInfoOptions();
            var ChangeInfo = comparison.Get(viewInfoOptions);*/

            var documentInfo = ToDocumentInfo(comparison);
/*                Number = page.Number,
                Width = page.Width,
                Height = page.Height,
                Name = page.Name*/
            return documentInfo;
        }

        private async Task<Comparer> InitComparisonAsync(FileCredentials fileCredentials)
        {
            if (_comparer != null)
                return _comparer;

            _comparisonLicenser.SetLicense();

            if (_internalCacheOptions.IsCacheDisabled)
            {
                _comparer = await CreateComparison(fileCredentials);
                return _comparer;
            }

            var key = $"VI__{fileCredentials.FilePath}";
            using (await _asyncLock.LockAsync(key))
            {
                if (_comparisonCache.TryGet(fileCredentials, out var comparer))
                {
                    _comparer = comparer;
                }
                else
                {
                    _comparer = await CreateComparison(fileCredentials);
            _comparisonCache.Set(fileCredentials, _comparer);
                }
            }

            return _comparer;
        }
       
        private async Task<Comparer> CreateComparison(FileCredentials fileCredentials)
        {
            var fileStream = await GetFileStreamAsync(fileCredentials.FilePath);
            var loadOptions = await CreateLoadOptionsAsync(fileCredentials);
            var viewer = new Comparer(fileStream, loadOptions);

            return viewer;
        }
        
       private async Task<MemoryStream> GetFileStreamAsync(string filePath)
       {
           byte[] bytes = await _fileStorage.ReadFileAsync(filePath);
           MemoryStream memoryStream = new MemoryStream(bytes);
           return memoryStream;
       }
        
       private async Task<LoadOptions> CreateLoadOptionsAsync(FileCredentials fileCredentials)
       {
            FileType loadFileType = FileType.FromFileNameOrExtension(fileCredentials.FileType);
            if (loadFileType == FileType.UNKNOWN)
                loadFileType = await _fileTypeResolver.ResolveFileTypeAsync(fileCredentials.FilePath);

            LoadOptions loadOptions = new LoadOptions
            {
                Password = fileCredentials.Password,
            };
            return loadOptions;
        }


       private static DocumentInfo ToDocumentInfo(Comparer comparer)
       {
/*            var printAllowed = true;
            if (viewInfo is PdfViewInfo info)
                printAllowed = info.PrintingAllowed;*/

           // var SourceDocumentInfo = comparer.Source.GetDocumentInfo();
               

           return new DocumentInfo
           {
/*               FileType = SourceDocumentInfo.FileType.FileFormat,
               Pages = SourceDocumentInfo.PagesInfo.Select(page => new Core.Entities.PageInfo
               {
                   Number = page.PageNumber,
                   Width = page.Width,
                   Height = page.Height
               })*/
           };
       }

        public void Dispose()
        {
            // NOTE: dispose when we're not going to reuse the object
            if (_internalCacheOptions.IsCacheDisabled)
            {
                _comparer?.Dispose();
                _comparer = null;
            }
        }
    }
}