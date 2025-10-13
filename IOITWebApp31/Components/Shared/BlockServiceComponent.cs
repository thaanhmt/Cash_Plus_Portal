using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "BlockService")]
    public class BlockServiceComponent : ViewComponent
    {
        public BlockServiceComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<News> data = (from n in db.News
                                          where n.Status != (int)Const.Status.DELETED
                                          select n).OrderBy(e => e.Location).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockService", data));
            }
        }
    }
}
