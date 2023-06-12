using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class CompareFileDataRequest
    {
        /// <summary>
        /// File unique ID.
        /// </summary>
        [JsonPropertyName("guid")]
        public string guid { get; set; }

        /// <summary>
        /// The password to open a document.
        /// </summary>
        [JsonPropertyName("password")]
        public string password { get; set; }

    }
}