using GroupDocs.Comparison.UI.Api.Models;
using System.Collections.Generic;
using System.IO;
using GroupDocs.Comparison.Result;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class SetChangesRequest
    {
        /// <summary>
        /// Contains list of the documents paths
        /// </summary>
        public List<CompareFileDataRequest> guids { get; set; }

        public int[] changes { get; set; }
    }
}