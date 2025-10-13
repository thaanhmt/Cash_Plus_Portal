using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockNewsHomeMainRightEn")]
    public class BlockNewsHomeMainRightEnComponent : ViewComponent
    {
        public BlockNewsHomeMainRightEnComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<News> data = (from n in db.News
                                          where n.CompanyId == Const.COMPANYID
                                          && n.WebsiteId == Const.WEBSITEID
                                          && (n.TypeNewsId == (int)Const.TypeNews.NEWS_TEXT || n.TypeNewsId == (int)Const.TypeNews.NEWS_NEWS)
                                          && n.Status == (int)Const.Status.NORMAL
                                          && n.DateStartActive <= DateTime.Now
                                          && n.IsHot == true
                                          && n.LanguageId == 1008
                                          select n).OrderByDescending(e => e.DateStartActive).ThenByDescending(e => e.CreatedAt).Skip(1).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockNewsHomeMainRightEn", data));
            }
        }

    }
}
