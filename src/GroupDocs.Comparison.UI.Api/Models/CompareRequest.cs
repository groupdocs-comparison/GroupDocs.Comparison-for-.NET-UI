using GroupDocs.Comparison.UI.Api.Models;
using GroupDocs.Comparison.UI.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class CompareRequest
    {
        /// <summary>
        /// File unique IDs.
        /// </summary>
        [JsonPropertyName("guids")]
        public List<CompareFileDataRequest> guids { get; set; }
    }
}