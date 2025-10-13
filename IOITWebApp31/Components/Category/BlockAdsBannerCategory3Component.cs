using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Category
{

    [ViewComponent(Name = "BlockAdsBannerCategory3")]
    public class BlockAdsBannerCategory3Component : ViewComponent
    {
        public BlockAdsBannerCategory3Component()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Id, int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<TypeAttributeItem> data = (from cm in db.TypeAttributeItem
                                                       where cm.TypeAttributeId == Id
                                                        && cm.Status != (int)Const.Status.DELETED
                                                       select cm).OrderBy(e => e.Location).Take(Number).ToList();
                return await Task.FromResult((IViewComponentResult)View("BlockAdsBannerCategory3", data));
            }
        }

    }
}
