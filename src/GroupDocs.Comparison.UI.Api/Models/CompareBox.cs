using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class CompareBox
    {
        /// <summary>
        /// Page X coord.
        /// </summary>
        [JsonPropertyName("x")]
        public double X { get; set; }

        /// <summary>
        /// Page Y coord.
        /// </summary>
        [JsonPropertyName("y")]
        public double Y { get; set; }

        /// <summary>
        /// Page width in pixels.
        /// </summary>
        [JsonPropertyName("width")]
        public double Width { get; set; }

        /// <summary>
        /// Page height in pixels.
        /// </summary>
        [JsonPropertyName("height")]
        public double Height { get; set; }
    }
}
