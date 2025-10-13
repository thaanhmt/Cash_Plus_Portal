using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockTCXDPT")]
    public class BlockTCXDPTComponent : ViewComponent
    {
        public BlockTCXDPTComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int CategoryId, int Number)
        {
            using (var db = new IOITDataContext())
            {
                var cate = db.Category.Where(e => e.CategoryId == CategoryId && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                if (cate != null)
                {
                    ViewBag.CategoryId = CategoryId;
                    ViewBag.CategoryUrl = cate.Url;
                    ViewBag.CategoryName = cate.Name;
                    ViewBag.ListCategoryChild = db.Category.Where(e => e.CategoryParentId == CategoryId && e.Status == (int)Const.Status.NORMAL).ToList();
                }
                IEnumerable<News> data = (from cm in db.CategoryMapping
                                          join n in db.News on cm.TargetId equals n.NewsId
                                          where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                          && (n.TypeNewsId != (int)Const.TypeNews.NEWS_VIDEO && n.TypeNewsId != (int)Const.TypeNews.NEWS_IMAGE)
                                          && cm.CategoryId == CategoryId
                                          && n.CompanyId == Const.COMPANYID
                                          && n.WebsiteId == Const.WEBSITEID
                                          && n.IsFirst != true
                                          && n.Status == (int)Const.Status.NORMAL
                                          && cm.Status != (int)Const.Status.DELETED
                                          && n.DateStartActive <= DateTime.Now
                                          select n).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockTCXDPT", data));
            }
        }

    }
}
