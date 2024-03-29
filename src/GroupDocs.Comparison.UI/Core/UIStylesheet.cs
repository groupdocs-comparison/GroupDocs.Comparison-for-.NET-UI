﻿using System.IO;
using GroupDocs.Comparison.UI.Configuration;

namespace GroupDocs.Comparison.UI.Core
{
    public class UIStylesheet
    {
        private const string StylesheetsPath = "css";
        public string FileName { get; }
        public byte[] Content { get; }
        public string ResourcePath { get; }
        public string ResourceRelativePath { get; }

        private UIStylesheet(UI.Configuration.Options options, string filePath)
        {
            FileName = Path.GetFileName(filePath);
            Content = File.ReadAllBytes(filePath);
            ResourcePath = $"{options.UIPath}/{StylesheetsPath}/{FileName}";
            ResourceRelativePath = $"{StylesheetsPath}/{FileName}";
        }

        public static UIStylesheet Create(UI.Configuration.Options options, string filePath)
        {
            return new UIStylesheet(options, filePath);
        }
    }
}
