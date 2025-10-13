using System.Collections.Generic;

namespace IOITWebApp31.Models
{
    public class JsonData
    {
        public List<JsonDataFile> files { get; set; }
        public string ckCsrfToken { get; set; }
    }
}
