using System.Collections.Generic;

namespace GroupDocs.Comparison.UI.Core.Entities
{
    public class DocumentInfo
    {
        public string FileType { get; set; }

        public IEnumerable<PageInfo> Pages { get; set; }
    }
}