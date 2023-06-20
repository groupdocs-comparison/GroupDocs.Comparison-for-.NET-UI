using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class ComparePageInfo
    {
        /// <summary>
        /// Number of page.
        /// </summary>
        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Page Width in pixels.
        /// </summary>
        [JsonPropertyName("width")]
        public double Width { get; set; }

        /// <summary>
        /// Page Height in pixels.
        /// </summary>
        [JsonPropertyName("height")]
        public double Height { get; set; }
    }
}
