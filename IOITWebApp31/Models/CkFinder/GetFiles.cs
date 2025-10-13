using System.Collections.Generic;

namespace IOITWebApp31.Models
{
    public class GetFiles
    {
        public string resourceType { get; set; }
        public CurrentFolder currentFolder { get; set; }
        public List<Files> files { get; set; }
    }
}
