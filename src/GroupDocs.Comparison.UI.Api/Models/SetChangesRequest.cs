using GroupDocs.Comparison.UI.Api.Models;
using System.Collections.Generic;
using System.IO;
using GroupDocs.Comparison.Result;
using System.Text.Json.Serialization;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class SetChangesRequest
    {
        /// <summary>
        /// File unique IDs.
        /// </summary>
        [JsonPropertyName("guids")]
        public List<CompareFileDataRequest> guids { get; set; }

        /// <summary>
        /// Contains list of the documents paths
        /// </summary>
        [JsonPropertyName("changes")]
        public int[] changeTypes { get; set; }
    }
}