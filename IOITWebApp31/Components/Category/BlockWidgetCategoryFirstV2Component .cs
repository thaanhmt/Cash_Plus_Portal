using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Category
{
    [ViewComponent(Name = "BlockWidgetCategoryFirstV2")]
    public class BlockWidgetCategoryFirstV2Component : ViewComponent
    {
        public BlockWidgetCategoryFirstV2Component()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int CategoryId, int Number)
        {
            using (var db = new IOITDataContext())
            {
                var GetParent = db.Category.Where(cc => cc.CategoryId == CategoryId && cc.Status != (int)Const.Status.DELETED).FirstOrDefault();
                var ParentId = GetParent.CategoryParentId;
                if (ParentId == 0)
                {
                    var GetThisParent = db.Category.Where(cb => cb.CategoryId == CategoryId && cb.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    var CategoryChildrenId = GetThisParent.CategoryId;
                    var DBFirstChilren = db.Category.Where(tc => tc.CategoryParentId == CategoryChildrenId && tc.Status != (int)Const.Status.DELETED).Skip(1).FirstOrDefault();
                    if (DBFirstChilren != null)
                    {
                        ViewBag.FirstNameCateWidget = DBFirstChilren.Name;
                        var FirstChilrenId = DBFirstChilren.CategoryId;
                        IEnumerable<News> data = (from cm in db.CategoryMapping
                                                  join n in db.News on cm.TargetId equals n.NewsId
                                                  where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                                  && cm.CategoryId == FirstChilrenId
                                                  && n.CompanyId == Const.COMPANYID
                                                  && n.WebsiteId == Const.WEBSITEID
                                                  && n.Status == (int)Const.Status.NORMAL
                                                  && cm.Status != (int)Const.Status.DELETED
                                                  select n).OrderByDescending(e => e.DateStartActive).Take(1).ToList();
                        return await Task.FromResult((IViewComponentResult)View("BlockWidgetCategoryFirstV2", data));
                    }
                    else
                    {
                        ViewBag.FirstNameCateWidget = "Thiên nhiên môi trường";
                        var FirstChilrenId = 4562;
                        IEnumerable<News> data = (from cm in db.CategoryMapping
                                                  join n in db.News on cm.TargetId equals n.NewsId
                                                  where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                                  && cm.CategoryId == FirstChilrenId
                                                  && n.CompanyId == Const.COMPANYID
                                                  && n.WebsiteId == Const.WEBSITEID
                                                  && n.Status == (int)Const.Status.NORMAL
                                                  && cm.Status != (int)Const.Status.DELETED
                                                  select n).OrderByDescending(e => e.DateStartActive).Take(1).ToList();
                        return await Task.FromResult((IViewComponentResult)View("BlockWidgetCategoryFirstV2", data));
                    }

                }
                else
                {
                    var GetThisParent = db.Category.Where(cb => cb.CategoryId == ParentId && cb.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    var CategoryChildrenId = GetThisParent.CategoryId;
                    var DBFirstChilren = db.Category.Where(tc => tc.CategoryParentId == CategoryChildrenId && tc.Status != (int)Const.Status.DELETED && tc.CategoryId != CategoryId).Skip(1).FirstOrDefault();
                    if (DBFirstChilren != null)
                    {
                        ViewBag.FirstNameCateWidget = DBFirstChilren.Name;
                        var FirstChilrenId = DBFirstChilren.CategoryId;
                        IEnumerable<News> data = (from cm in db.CategoryMapping
                                                  join n in db.News on cm.TargetId equals n.NewsId
                                                  where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                                  && cm.CategoryId == FirstChilrenId
                                                  && n.CompanyId == Const.COMPANYID
                                                  && n.WebsiteId == Const.WEBSITEID
                                                  && n.Status == (int)Const.Status.NORMAL
                                                  && cm.Status != (int)Const.Status.DELETED
                                                  select n).OrderByDescending(e => e.DateStartActive).Take(1).ToList();
                        return await Task.FromResult((IViewComponentResult)View("BlockWidgetCategoryFirstV2", data));
                    }
                    else
                    {
                        ViewBag.FirstNameCateWidget = "Thiên nhiên môi trường";
                        var FirstChilrenId = 4562;
                        IEnumerable<News> data = (from cm in db.CategoryMapping
                                                  join n in db.News on cm.TargetId equals n.NewsId
                                                  where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                                  && cm.CategoryId == FirstChilrenId
                                                  && n.CompanyId == Const.COMPANYID
                                                  && n.WebsiteId == Const.WEBSITEID
                                                  && n.Status == (int)Const.Status.NORMAL
                                                  && cm.Status != (int)Const.Status.DELETED
                                                  select n).OrderByDescending(e => e.DateStartActive).Take(1).ToList();
                        return await Task.FromResult((IViewComponentResult)View("BlockWidgetCategoryFirstV2", data));
                    }

                }
            }
        }
    }
}
