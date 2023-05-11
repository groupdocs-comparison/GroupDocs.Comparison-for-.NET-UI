using System.Collections.Generic;

namespace GroupDocs.Comparison.UI.Core
{
    internal interface IUIResourcesReader
    {
        IEnumerable<UIResource> UIResources { get; }
    }
}
