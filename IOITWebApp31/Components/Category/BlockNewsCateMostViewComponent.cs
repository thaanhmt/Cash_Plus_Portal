using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockNewsCateMostView")]
    public class BlockNewsCateMostViewComponent : ViewComponent
    {
        public BlockNewsCateMostViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<News> data = (from n in db.News
                                          where n.CompanyId == Const.COMPANYID
                                          && n.Status == (int)Const.Status.NORMAL
                                          select n).OrderByDescending(e => e.ViewNumber).ThenByDescending(e => e.DateStartActive).ThenByDescending(e => e.CreatedAt).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockNewsCateMostView", data));
            }
        }

    }
}
