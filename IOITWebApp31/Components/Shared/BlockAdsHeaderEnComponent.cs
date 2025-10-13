using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{

    [ViewComponent(Name = "BlockAdsHeaderEn")]
    public class BlockAdsHeaderEnComponent : ViewComponent
    {
        public BlockAdsHeaderEnComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Id, int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<Slide> data = (from cm in db.Slide
                                           where cm.TypeSlideId == Id
                                                        && cm.Status != (int)Const.Status.DELETED
                                                        && cm.LanguageId == 1008
                                           select cm).OrderBy(e => e.Location).Take(Number).ToList();
                return await Task.FromResult((IViewComponentResult)View("BlockAdsHeaderEn", data));
            }
        }

    }
}
