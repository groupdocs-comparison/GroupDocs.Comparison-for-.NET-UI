using GroupDocs.Comparison.UI.Api.Models;
using GroupDocs.Comparison.UI.Core.Entities;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class CompareRequest
    {
        /// <summary>
        /// Contains list of the documents paths
        /// </summary>
        public List<CompareFileDataRequest> guids { get; set; }

    }
}