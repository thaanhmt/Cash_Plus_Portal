namespace IOITWebApp31.Models
{
    public class RenameFile
    {
        public string resourceType { get; set; }
        public CurrentFolder currentFolder { get; set; }
        public string name { get; set; }
        public string newName { get; set; }
        public int renamed { get; set; }
    }
}
