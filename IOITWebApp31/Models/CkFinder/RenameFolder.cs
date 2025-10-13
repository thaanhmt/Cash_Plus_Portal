namespace IOITWebApp31.Models
{
    public class RenameFolder
    {
        public string resourceType { get; set; }
        public CurrentFolder currentFolder { get; set; }
        public string newName { get; set; }
        public string newPath { get; set; }
        public int renamed { get; set; }
    }
}
