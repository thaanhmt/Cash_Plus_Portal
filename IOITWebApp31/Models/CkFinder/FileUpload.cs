namespace IOITWebApp31.Models
{
    public class FileUpload
    {
        public string resourceType { get; set; }
        public CurrentFolder currentFolder { get; set; }
        public string fileName { get; set; }
        public int uploaded { get; set; }
    }
}
