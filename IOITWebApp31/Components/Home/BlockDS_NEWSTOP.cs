using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockDS_NEWSTOP")]
    public class BlockDS_NEWSTOPComponent : ViewComponent
    {
        public BlockDS_NEWSTOPComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int LanguageId, int Number, int Skip, string Template)
        {
            using (var db = new IOITDataContext())
            {
                ViewBag.Template = Template;
                IEnumerable<News> data = (from n in db.News
                                          where (n.TypeNewsId == (int)Const.TypeNews.NEWS_TEXT || n.TypeNewsId == (int)Const.TypeNews.NEWS_NEWS)
                                          && n.CompanyId == Const.COMPANYID
                                          && n.WebsiteId == Const.WEBSITEID
                                          && n.Status == (int)Const.Status.NORMAL
                                          && n.DateStartActive <= DateTime.Now
                                          && n.LanguageId == LanguageId
                                          select n).OrderByDescending(e => e.DateStartActive).Skip(Skip).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockDS_NEWSTOP", data));
            }
        }

    }
}
