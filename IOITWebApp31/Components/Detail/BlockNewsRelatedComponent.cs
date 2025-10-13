using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Detail
{
    [ViewComponent(Name = "BlockNewsRelated")]
    public class BlockNewsRelatedComponent : ViewComponent
    {
        public BlockNewsRelatedComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int NewsId, int Number, int Language)
        {
            using (var db = new IOITDataContext())
            {
                var categoryMappings = db.CategoryMapping.Where(cm => cm.TargetId == NewsId && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS && cm.Status != (int)Const.Status.DELETED).FirstOrDefault();
                if (categoryMappings != null)
                {
                    IEnumerable<News> data = (from n in db.News
                                              join cn in db.CategoryMapping on n.NewsId equals cn.TargetId
                                              where cn.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                              && n.Status == (int)Const.Status.NORMAL
                                              && n.NewsId != NewsId
                                              && n.LanguageId == Language
                                              && cn.CategoryId == categoryMappings.CategoryId
                                              && cn.Status == (int)Const.Status.NORMAL
                                              //&& (categoryMappings.Where(x => x.CategoryId == cn.CategoryId).FirstOrDefault() != null ? true : false)
                                              select n).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();
                    ViewBag.CategoryId = categoryMappings.CategoryId;
                    return await Task.FromResult((IViewComponentResult)View("BlockNewsRelated", data));
                }
                else
                {
                    IEnumerable<News> data = (from n in db.News
                                              where n.Status == (int)Const.Status.NORMAL
                                              && n.NewsId != NewsId
                                              && n.LanguageId == Language
                                              select n).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();
                    return await Task.FromResult((IViewComponentResult)View("BlockNewsRelated", data));
                }
            }
        }
    }
}
