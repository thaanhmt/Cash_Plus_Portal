using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Category
{
    [ViewComponent(Name = "BlockNewsTopCategory")]
    public class BlockNewsTopCategoryComponent : ViewComponent
    {
        public BlockNewsTopCategoryComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int CategoryId, int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<News> data = (from cm in db.CategoryMapping
                                          join n in db.News on cm.TargetId equals n.NewsId
                                          where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                          && cm.CategoryId == CategoryId
                                          && n.CompanyId == Const.COMPANYID
                                          && n.IsFirst == true
                                          && n.WebsiteId == Const.WEBSITEID
                                          && n.Status == (int)Const.Status.NORMAL
                                          && cm.Status != (int)Const.Status.DELETED
                                          select n).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();
                return await Task.FromResult((IViewComponentResult)View("BlockNewsTopCategory", data));
                //var GetParent = db.Category.Where(cc => cc.CategoryId == CategoryId && cc.Status != (int)Const.Status.DELETED).FirstOrDefault();
                //var ParentId = GetParent.CategoryParentId;
                //if(ParentId == 0)
                //{
                //    IEnumerable<News> data = (from cm in db.CategoryMapping
                //                              join n in db.News on cm.TargetId equals n.NewsId
                //                              where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS                                             
                //                              && cm.CategoryId == CategoryId
                //                              && n.CompanyId == Const.COMPANYID
                //                              && n.WebsiteId == Const.WEBSITEID
                //                              && n.Status == (int)Const.Status.NORMAL
                //                              && cm.Status != (int)Const.Status.DELETED
                //                              select n).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();
                //    return await Task.FromResult((IViewComponentResult)View("BlockNewsTopCategory", data));
                //}
                //else
                //{
                //    IEnumerable<News> data = (from cm in db.CategoryMapping
                //                              join n in db.News on cm.TargetId equals n.NewsId
                //                              where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                //                              && cm.CategoryId == ParentId
                //                              && n.CompanyId == Const.COMPANYID
                //                              && n.WebsiteId == Const.WEBSITEID
                //                              && n.Status == (int)Const.Status.NORMAL
                //                              && cm.Status != (int)Const.Status.DELETED
                //                              select n).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();
                //    return await Task.FromResult((IViewComponentResult)View("BlockNewsTopCategory", data));
                //}               
            }
        }
    }
}
