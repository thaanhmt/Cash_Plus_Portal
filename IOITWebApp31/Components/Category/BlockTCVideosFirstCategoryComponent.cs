using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Category
{
    [ViewComponent(Name = "BlockTCVideosFirstCategory")]
    public class BlockTCVideosFirstCategoryComponent : ViewComponent
    {
        public BlockTCVideosFirstCategoryComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<News> data = (from cm in db.CategoryMapping
                                          join n in db.News on cm.TargetId equals n.NewsId
                                          where cm.CategoryId == 4628
                                          && n.CompanyId == Const.COMPANYID
                                          && n.TypeNewsId == (int)Const.TypeNews.NEWS_VIDEO
                                          && n.WebsiteId == Const.WEBSITEID
                                          && n.Status == (int)Const.Status.NORMAL
                                          && cm.Status != (int)Const.Status.DELETED
                                          select n).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();
                ViewBag.NameCategoryBottom = db.Category.Find(4629).Name;
                return await Task.FromResult((IViewComponentResult)View("BlockTCVideosFirstCategory", data));
            }
        }
    }
}
