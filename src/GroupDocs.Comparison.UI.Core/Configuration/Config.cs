using System.Linq;

namespace GroupDocs.Comparison.UI.Core.Configuration
{
    public class Config
    {
        public bool PageSelector { get; set; } = true;
        public bool Download { get; set; } = true;
        public bool Upload { get; set; } = true;
        public bool Print { get; set; } = true;
        public bool Browse { get; set; } = true;
        public bool Rewrite { get; set; } = false;
        public bool EnableRightClick { get; set; } = true;
        public string FilesDirectory { get; set; } = "DocumentSamples/Comparison";
        public string ResultDirectory { get; set; } = string.Empty;
        public int PreloadResultPageCount { get; set; } = 0;
    }
}
