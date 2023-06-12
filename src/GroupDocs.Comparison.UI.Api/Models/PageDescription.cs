using System.Text.Json.Serialization;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class PageDescription : PageContent
    {
        /// <summary>
        /// Page with in pixels.
        /// </summary>
        [JsonPropertyName("width")]
        public double Width { get; set; }

        /// <summary>
        /// Page height in pixels.
        /// </summary>
        [JsonPropertyName("height")]
        public double Height { get; set; }

        /// <summary>
        /// Worksheet name for spreadsheets.
        /// </summary>
        [JsonPropertyName("sheetName")]
        public string SheetName { get; set; }
    }
}