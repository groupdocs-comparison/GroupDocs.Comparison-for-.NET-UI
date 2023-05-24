using System.Text.Json.Serialization;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class LoadConfigResponse
    {
        /// <summary>
        /// Enables page selector control.
        /// </summary>
        [JsonPropertyName("pageSelector")]
        public bool PageSelector { get; set; }

        /// <summary>
        /// Enables download button.
        /// </summary>
        [JsonPropertyName("download")]
        public bool Download { get; set; }

        /// <summary>
        /// Enables upload.
        /// </summary>
        [JsonPropertyName("upload")]
        public bool Upload { get; set; }

        /// <summary>
        /// Enables printing.
        /// </summary>
        [JsonPropertyName("print")]
        public bool Print { get; set; }

        /// <summary>
        /// Enables file browser.
        /// </summary>
        [JsonPropertyName("browse")]
        public bool Browse { get; set; }

        /// <summary>
        /// Enables file rewrite.
        /// </summary>
        [JsonPropertyName("rewrite")]
        public bool Rewrite { get; set; }

        /// <summary>
        /// Enables right click.
        /// </summary>
        [JsonPropertyName("enableRightClick")]
        public bool EnableRightClick { get; set; }

        /// <summary>
        /// The file directory.
        /// </summary>
        [JsonPropertyName("filesDirectory")]
        public string FilesDirectory { get; set; }

        /// <summary>
        /// The result diretory.
        /// </summary>
        [JsonPropertyName("resultDirectory")]
        public string ResultDirectory { get; set; }

        /// <summary>
        /// Count result pages to preload.
        /// </summary>
        [JsonPropertyName("preloadResultPageCount")]
        public int PreloadResultPageCount { get; set; }

    }
}