using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "BlockNewsMostView")]
    public class BlockNewsMostViewComponent : ViewComponent
    {
        public BlockNewsMostViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<News> data = (from n in db.News
                                          where (n.TypeNewsId == (int)Const.TypeNews.NEWS_TEXT || n.TypeNewsId == (int)Const.TypeNews.NEWS_NOTIFICATION)
                                          && n.Status == (int)Const.Status.NORMAL
                                          select n).OrderByDescending(e => e.ViewNumber).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockNewsMostView", data));
            }
        }
    }
}
