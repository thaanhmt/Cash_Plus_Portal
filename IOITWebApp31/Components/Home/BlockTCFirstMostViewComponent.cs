using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockTCFirstMostView")]
    public class BlockTCFirstMostViewComponent : ViewComponent
    {
        public BlockTCFirstMostViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<NewsDTO> data = (from n in db.News
                                             join cm in db.CategoryMapping on n.NewsId equals cm.TargetId
                                             join c in db.Category on cm.CategoryId equals c.CategoryId
                                             where (n.TypeNewsId == (int)Const.TypeNews.NEWS_TEXT || n.TypeNewsId == (int)Const.TypeNews.NEWS_NEWS)
                                             && cm.Status != (int)Const.Status.DELETED
                                             && c.Status != (int)Const.Status.DELETED
                                             && n.CompanyId == Const.COMPANYID
                                             && n.WebsiteId == Const.WEBSITEID
                                             && n.Status == (int)Const.Status.NORMAL
                                             && n.LanguageId == 1
                                             select new NewsDTO
                                             {
                                                 NewsId = n.NewsId,
                                                 Title = n.Title,
                                                 Url = n.Url,
                                                 CreatedAt = n.CreatedAt,
                                                 NameCategory = c.Name,
                                                 LinkCategory = c.Url,
                                                 Image = n.Image,
                                             }).OrderByDescending(d => d.ViewNumber).GroupBy(e => e.NewsId).Select(e => new NewsDTO
                                             {
                                                 NewsId = e.Key,
                                                 Title = e.FirstOrDefault().Title,
                                                 Url = e.FirstOrDefault().Url,
                                                 CreatedAt = e.FirstOrDefault().CreatedAt,
                                                 NameCategory = e.FirstOrDefault().NameCategory,
                                                 LinkCategory = e.FirstOrDefault().LinkCategory,
                                                 Image = e.FirstOrDefault().Image
                                             }).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockTCFirstMostView", data));
            }
        }

    }
}
