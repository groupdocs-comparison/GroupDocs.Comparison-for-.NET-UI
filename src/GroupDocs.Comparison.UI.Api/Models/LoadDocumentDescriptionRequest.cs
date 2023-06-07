using System.Text.Json.Serialization;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class LoadDocumentDescriptionRequest
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
        /// The password to open a document.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}