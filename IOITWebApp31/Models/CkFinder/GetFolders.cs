using System.Collections.Generic;

namespace IOITWebApp31.Models
{
    public class GetFolders
    {
        public string resourceType { get; set; }
        public CurrentFolder currentFolder { get; set; }
        public List<Folders> folders { get; set; }
    }
}
