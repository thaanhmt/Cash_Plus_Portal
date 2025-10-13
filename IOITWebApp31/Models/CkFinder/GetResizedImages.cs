namespace IOITWebApp31.Models
{
    public class GetResizedImages
    {
        public string resourceType { get; set; }
        public CurrentFolder currentFolder { get; set; }
        public string originalSize { get; set; }
        public Resized resized { get; set; }
    }
}
