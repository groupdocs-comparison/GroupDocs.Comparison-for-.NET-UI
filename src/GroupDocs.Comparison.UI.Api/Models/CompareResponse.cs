using Newtonsoft.Json;
using GroupDocs.Comparison.Result;
using GroupDocs.Comparison.UI.Api.Entity;
using System.Text.Json.Serialization;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class CompareResponse
    {

        /// <summary>
        /// File unique ID.
        /// </summary>
        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// File type e.g "docx".
        /// </summary>
        [JsonPropertyName("fileType")]
        public string FileType { get; set; }

        /// <summary>
        /// Document pages.
        /// </summary>
        [JsonPropertyName("pages")]
        public List<PageDescription> Pages { get; set; }

        /// <summary>
        /// List of changes
        /// </summary>
        [JsonPropertyName("changes")]
        public List<CompareChangeInfo> Changes { get; set; }

    }
}