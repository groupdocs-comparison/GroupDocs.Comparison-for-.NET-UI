using System.Text.Json.Serialization;

namespace GroupDocs.Comparison.UI.Api.Models
{
    public class ErrorResponse
    {
        /// <summary>
        /// The error message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        public ErrorResponse(string message)
        {
            this.Message = message;
        }
    }
}