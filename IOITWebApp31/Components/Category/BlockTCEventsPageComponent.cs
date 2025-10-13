using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockTCEventsPage")]
    public class BlockTCEventsPageComponent : ViewComponent
    {
        public BlockTCEventsPageComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<News> data = (from n in db.News
                                          where
                                          n.Status == (int)Const.Status.NORMAL
                                          && n.TypeNewsId == 7
                                          select n).OrderByDescending(e => e.DateStartOn).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockTCEventsPage", data));
            }
        }

    }
}
