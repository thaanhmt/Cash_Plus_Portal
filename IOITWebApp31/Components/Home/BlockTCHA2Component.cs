using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockTCHA2")]
    public class BlockTCHA2Component : ViewComponent
    {
        public BlockTCHA2Component()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int CategoryId, int Number, int Skip)
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
                                          && cm.CategoryId == CategoryId
                                          && n.CompanyId == Const.COMPANYID
                                          && n.WebsiteId == Const.WEBSITEID
                                          && n.Status == (int)Const.Status.NORMAL
                                          && cm.Status != (int)Const.Status.DELETED
                                          && n.TypeNewsId == 3
                                          && n.DateStartActive <= DateTime.Now
                                          select n).OrderByDescending(e => e.DateStartActive).Skip(Skip).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockTCHA2", data));
            }
        }

    }
}
