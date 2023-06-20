using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GroupDocs.Comparison.UI.Core;
using GroupDocs.Comparison.UI.SelfHost.Api.Configuration;
using Microsoft.Extensions.Options;

namespace GroupDocs.Comparison.UI.SelfHost.Api.Licensing
{
    internal class ComparisonLicenser : IComparisonLicenser
    {
        private readonly Config _config;
        private readonly object _lock = new object();
        private bool _licenseSet;

        public ComparisonLicenser(IOptions<Config> config)
        {
            _config = config.Value;
        }

        public void SetLicense()
        {
            bool isUseEvaluationLic = true;

            if (_licenseSet)
                return;

            StringBuilder sbErrors = new StringBuilder();

            if (!string.IsNullOrEmpty(_config.LicensePath))
            {
                isUseEvaluationLic = false;
                try
                {
                    SetLicense(_config.LicensePath);
                }
                catch (Exception ex)
                {
                    sbErrors.Append($"Check config license path error {Environment.NewLine}");
                    sbErrors.Append(ex.Message);
                    sbErrors.Append(Environment.NewLine);
                }

                if (_licenseSet)
                {
                    return;
                }
            }

            string licensePath = Environment.GetEnvironmentVariable(Keys.GROUPDOCSCOMPARISONUI_LIC_PATH_ENVIRONMENT_VARIABLE_KEY);
            if (!string.IsNullOrEmpty(licensePath))
            {
                try
                {
                    SetLicense(licensePath);
                }
                catch (Exception ex)
                {
                    sbErrors.Append($"Check environment variable {Keys.GROUPDOCSCOMPARISONUI_LIC_PATH_ENVIRONMENT_VARIABLE_KEY} error {Environment.NewLine}");
                    sbErrors.Append(ex.Message);
                    sbErrors.Append(Environment.NewLine);
                }

                if (_licenseSet)
                {
                    return;
                }
            }

            List<string> licFileNames = new List<string>()
                      {
                        Keys.GROUPDOCSCOMPARISONUI_LIC_FILE_DEFAULT_NAME,
                        Keys.GROUPDOCSCOMPARISONUI_TEMPORARY_LIC_FILE_DEFAULT_NAME
                      };

            foreach (string licFileName in licFileNames)
            {
                string licPath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, licFileName);
                try
                {
                     SetLicense(licPath);
                }
                catch (Exception ex)
                {
                    sbErrors.Append($"Check {licPath} error {Environment.NewLine}");
                    sbErrors.Append(ex.Message);
                    sbErrors.Append(Environment.NewLine);
                }

                if (_licenseSet)
                {
                    return;
                }
            }
#if DEBUG
            if (!isUseEvaluationLic)
            {
                throw new FileNotFoundException(sbErrors.ToString());
            }
#endif
        }

        private void SetLicense(string licensePath)
        {
            lock (_lock)
            {
                if (!_licenseSet)
                {
                    License license = new License();
                    license.SetLicense(licensePath);

                    _licenseSet = true;
                }
            }
        }
    }
}