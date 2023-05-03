using System;
using GroupDocs.Comparison.Options;
using GroupDocs.Comparison.UI.Core;

namespace GroupDocs.Comparison.UI.SelfHost.Api.Configuration
{
    public class Config
    {
        internal string LicensePath = string.Empty;

        public Config SetLicensePath(string licensePath)
        {
            LicensePath = licensePath;
            return this;
        }
    }
}
