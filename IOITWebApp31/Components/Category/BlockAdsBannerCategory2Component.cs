using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Category
{

    [ViewComponent(Name = "BlockAdsBannerCategory2")]
    public class BlockAdsBannerCategory2Component : ViewComponent
    {
        public BlockAdsBannerCategory2Component()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Id, int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<Slide> data = (from cm in db.Slide
                                           where cm.TypeSlideId == Id
                                                        && cm.Status != (int)Const.Status.DELETED
                                           select cm).OrderBy(e => e.Location).Take(Number).ToList();
                return await Task.FromResult((IViewComponentResult)View("BlockAdsBannerCategory2", data));
            }
        }

    }
}
