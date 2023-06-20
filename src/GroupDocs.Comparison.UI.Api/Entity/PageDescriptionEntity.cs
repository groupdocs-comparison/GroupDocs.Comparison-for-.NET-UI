
using Newtonsoft.Json;

namespace GroupDocs.Comparison.UI.Api.Entity
{
    /// <summary>
    /// Page Description Entity
    /// </summary>
    public class PageDescriptionEntity
    {
        public double width { get; set; }
        public double height { get; set; }
        public int number { get; set; }
        public int angle { get; set; }

        [JsonProperty]
        private string data;

        public void SetData(string data)
        {
            this.data = data;
        }

        public string GetData()
        {
            return data;
        }
    }
}