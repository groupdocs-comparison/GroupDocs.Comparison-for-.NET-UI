
namespace GroupDocs.Comparison.UI.Api.Entity
{

    /// <summary>
    /// File description entity
    /// </summary>
    public class FileDescriptionEntity
    {
        public string guid { get; set; }
        public string name { get; set; }
        public string docType { get; set; }
        public bool isDirectory { get; set; }
        public long size { get; set; }
    }
}