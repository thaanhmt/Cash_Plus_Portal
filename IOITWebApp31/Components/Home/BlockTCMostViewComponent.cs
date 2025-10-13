using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockTCMostView")]
    public class BlockTCMostViewComponent : ViewComponent
    {
        public BlockTCMostViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                var data = await db.News.Where(n =>
                                    (n.TypeNewsId == (int)Const.TypeNews.NEWS_TEXT
                                    || n.TypeNewsId == (int)Const.TypeNews.NEWS_NEWS)
                                        && n.CompanyId == Const.COMPANYID
                                             && n.WebsiteId == Const.WEBSITEID
                                             && n.Status == (int)Const.Status.NORMAL
                                            && n.LanguageId == 1)
                    .Select(n => new NewsDTO
                    {
                        NewsId = n.NewsId,
                        Title = n.Title,
                        Url = n.Url,
                        CreatedAt = n.CreatedAt,
                        Image = n.Image,
                        ViewNumber = n.ViewNumber,
                    }).OrderByDescending(d => d.ViewNumber).Skip(1).Take(Number).ToListAsync();

                foreach (var item in data)
                {
                    var itemCM = await (from cm in db.CategoryMapping
                                        join c in db.Category on cm.CategoryId equals c.CategoryId
                                        where cm.Status != (int)Const.Status.DELETED
                                        && c.Status == (int)Const.Status.NORMAL
                                        && c.LanguageId == 1
                                        && cm.TargetId == item.NewsId
                                        && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                        select c).FirstOrDefaultAsync();
                    if (itemCM != null)
                    {
                        item.NameCategory = itemCM.Name;
                        item.LinkCategory = itemCM.Url;
                    }
                }

                return await Task.FromResult((IViewComponentResult)View("BlockTCMostView", data));
            }
        }

    }
}
