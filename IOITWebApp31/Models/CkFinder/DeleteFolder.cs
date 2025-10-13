namespace IOITWebApp31.Models
{
    public class DeleteFolder
    {
        public string resourceType { get; set; }
        public CurrentFolder currentFolder { get; set; }
        public int deleted { get; set; }
    }
}
