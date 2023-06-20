using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class CompareChangeInfo
    {
        /// <summary>
        /// Page with in pixels.
        /// </summary>
        [JsonPropertyName("pageInfo")]
        public ComparePageInfo PageInfo { get; set; }

        /// <summary>
        /// Page with in pixels.
        /// </summary>
        [JsonPropertyName("box")]
        public CompareBox Box { get; set; }
    }
}
