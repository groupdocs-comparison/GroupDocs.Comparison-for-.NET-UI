using System.Collections.Generic;
using GroupDocs.Comparison.UI.Api.Entity;
using GroupDocs.Comparison.UI.Api.Models;

namespace GroupDocs.Comparison.UI.Api
{
    public interface IComparisonService
    {
        List<FileDescriptionEntity> LoadFiles(PostedDataEntity fileTreeRequest);

        /// <summary>
        /// Compare two documents, save results in files
        /// </summary>
        /// <param name="compareRequest">PostedDataEntity</param>
        /// <returns>CompareResponse</returns>
        CompareResponse Compare(CompareRequest compareRequest);

        /// <summary>
        /// Compare two documents and accept/reject changes, save results in files
        /// </summary>
        /// <param name="setChangesRequest">PostedDataEntity</param>
        /// <returns>CompareResponse</returns>
        CompareResponse SetChanges(SetChangesRequest setChangesRequest);

        /// <summary>
        ///  Load document page as images
        /// </summary>
        /// <param name="postedData">PostedDataEntity</param>
        /// <returns>LoadDocumentEntity</returns>
        PageDescriptionEntity LoadDocumentPage(PostedDataEntity postedData);

        /// <summary>
        /// Check format files for comparing
        /// </summary>
        /// <param name="file">CompareRequest</param>
        /// <returns>bool</returns>
        bool CheckFiles(CompareRequest files);
    }
}