namespace IOITWebApp31.Models
{
    public class CreateFolder
    {
        public string resourceType { get; set; }
        public CurrentFolder currentFolder { get; set; }
        public string newFolder { get; set; }
        public int created { get; set; }
    }
}
