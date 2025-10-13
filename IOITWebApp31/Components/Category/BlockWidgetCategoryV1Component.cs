using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Category
{
    [ViewComponent(Name = "BlockWidgetCategoryV1")]
    public class BlockWidgetCategoryV1Component : ViewComponent
    {
        public BlockWidgetCategoryV1Component()
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
                    var DBFirstChilren = db.Category.Where(tc => tc.CategoryParentId == CategoryChildrenId && tc.CategoryId != 4647 && tc.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (DBFirstChilren != null)
                    {
                        ViewBag.FirstNameCateWidget = DBFirstChilren.Name;
                        ViewBag.UrlCateWidget = DBFirstChilren.Url;
                        var FirstChilrenId = DBFirstChilren.CategoryId;
                        IEnumerable<News> data = (from cm in db.CategoryMapping
                                                  join n in db.News on cm.TargetId equals n.NewsId
                                                  where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                                  && cm.CategoryId == FirstChilrenId
                                                  && n.CompanyId == Const.COMPANYID
                                                  && n.WebsiteId == Const.WEBSITEID
                                                  && n.Status == (int)Const.Status.NORMAL
                                                  && cm.Status != (int)Const.Status.DELETED
                                                  select n).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();
                        return await Task.FromResult((IViewComponentResult)View("BlockWidgetCategoryV1", data));
                    }
                    else
                    {
                        IEnumerable<News> data = null;
                        return await Task.FromResult((IViewComponentResult)View("BlockWidgetCategoryV1", data));
                    }

                }
                else
                {
                    var GetThisParent = db.Category.Where(cb => cb.CategoryId == ParentId && cb.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    var CategoryChildrenId = GetThisParent.CategoryId;
                    var DBFirstChilren = db.Category.Where(tc => tc.CategoryParentId == CategoryChildrenId && tc.Status != (int)Const.Status.DELETED && tc.CategoryId != CategoryId).FirstOrDefault();
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
                                                  select n).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();
                        return await Task.FromResult((IViewComponentResult)View("BlockWidgetCategoryV1", data));
                    }
                    else
                    {
                        IEnumerable<News> data = null;
                        return await Task.FromResult((IViewComponentResult)View("BlockWidgetCategoryV1", data));
                    }

                }
            }
        }
    }
}
