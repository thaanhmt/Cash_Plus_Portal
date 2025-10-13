using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "CustomizeCss")]
    public class CustomizeCss : ViewComponent
    {
        public CustomizeCss()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            using (var db = new IOITDataContext())
            {
                var DataConfig = db.Config.Where(c => c.Status != (int)Const.Status.DELETED && c.ConfigId == 1).FirstOrDefault();
                ViewBag.CustomizeCss = DataConfig.CustomCss;
                return await Task.FromResult((IViewComponentResult)View("CustomizeCss", DataConfig));
            }
        }
    }
}
