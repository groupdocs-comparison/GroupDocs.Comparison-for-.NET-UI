using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupDocs.Comparison.UI.Configuration
{
    public class Options
    {
        internal ICollection<string> CustomStylesheets { get; } = new List<string>();

        public string UIPath { get; set; } = "/comparison";

        public string UIConfigEndpoint { get; set; } = "/comparison-config";

        public string APIEndpoint { get; set; } = "/comparison-api";

        public Options AddCustomStylesheet(string path)
        {
            string stylesheetPath = path;

            if (!Path.IsPathFullyQualified(stylesheetPath))
                stylesheetPath = Path.Combine(Environment.CurrentDirectory, path);

            if (!File.Exists(stylesheetPath))
                throw new Exception($"Could not find style sheet at path {stylesheetPath}");

            CustomStylesheets.Add(stylesheetPath);

            return this;
        }
    }
}
