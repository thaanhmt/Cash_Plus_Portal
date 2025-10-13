using IOITWebApp31.Models.Localization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace IOITWebApp31.Languages
{
    public class vi_VN
    {
        public static List<Resource> GetList()
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            return JsonConvert.DeserializeObject<List<Resource>>(File.ReadAllText("Languages/vi-VN.json"), jsonSerializerSettings);
        }
    }
}
