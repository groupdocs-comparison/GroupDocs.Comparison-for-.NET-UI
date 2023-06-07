using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class LoadFileTreeRequest
    {
        /// <summary>
        /// Folder path.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;
    }
}
