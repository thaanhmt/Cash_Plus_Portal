using IOITWebApp31.Models.Localization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace IOITWebApp31.Languages
{
    public static class en_US
    {
        public static List<Resource> GetList()
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            return JsonConvert.DeserializeObject<List<Resource>>(File.ReadAllText("Languages/en-US.json"), jsonSerializerSettings);
        }
    }
}
