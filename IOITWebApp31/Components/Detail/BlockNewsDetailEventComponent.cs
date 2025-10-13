using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Detail
{
    [ViewComponent(Name = "BlockNewsDetailEvent")]
    public class BlockNewsDetailEventComponent : ViewComponent
    {
        public BlockNewsDetailEventComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int CategoryId, int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<News> data = (from cm in db.CategoryMapping
                                          join n in db.News on cm.TargetId equals n.NewsId
                                          where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                          && n.TypeNewsId == (int)Const.TypeNews.NEWS_TEXT
                                          && cm.CategoryId == CategoryId
                                          && n.CompanyId == Const.COMPANYID
                                          && n.WebsiteId == Const.WEBSITEID
                                          && n.Status == (int)Const.Status.NORMAL
                                          && cm.Status != (int)Const.Status.DELETED
                                          select n).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockNewsDetailEvent", data));
            }
        }

    }
}
