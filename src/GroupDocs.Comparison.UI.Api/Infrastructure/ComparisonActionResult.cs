using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace GroupDocs.Comparison.UI.Api.Infrastructure
{
    internal class ComparisonActionResult : ActionResult, IStatusCodeActionResult
    {
        public ComparisonActionResult(object value)
        {
            Value = value;
        }

        public string ContentType { get; set; }

        public int? StatusCode { get; set; }

        public object Value { get; set; }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = new ComparisonActionResultExecutor();
            return executor.ExecuteAsync(context, this);
        }
    }
}