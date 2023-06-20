using System.Linq;

namespace GroupDocs.Comparison.UI.Core.Configuration
{
    public class Config
    {
        public bool Download { get; set; } = true;
        public string FilesDirectory { get; set; } = string.Empty;

        public Config SetDownload(bool download)
        {
            Download = download;
            return this;
        }
        public Config SetFilesDirectory(string filesDirectory)
        {
            FilesDirectory = filesDirectory;
            return this;
        }
    }
}
