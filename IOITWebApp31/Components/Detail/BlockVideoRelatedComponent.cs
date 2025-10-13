using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Detail
{
    [ViewComponent(Name = "BlockVideoRelated")]
    public class BlockVideoRelatedComponent : ViewComponent
    {
        public BlockVideoRelatedComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int NewsId, int Number)
        {
            using (var db = new IOITDataContext())
            {
                //lấy tên danh mục
                List<CategoryMapping> categoryMappings = db.CategoryMapping.Where(cm => cm.TargetId == NewsId && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS && cm.Status != (int)Const.Status.DELETED).ToList();

                //IEnumerable<News> data = (from n in db.News
                //                          join cn in db.CategoryMapping on n.NewsId equals cn.TargetId
                //                          where cn.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                //                          && n.TypeNewsId == (int)Const.TypeNews.NEWS_VIDEO
                //                          && n.Status == (int)Const.Status.NORMAL
                //                          && n.NewsId != NewsId
                //                          && cn.Status == (int)Const.Status.NORMAL
                //                          && (categoryMappings.Where(x => x.CategoryId == cn.CategoryId).FirstOrDefault() != null ? true : false)
                //                          select n).Distinct().OrderByDescending(e => e.DateStartActive).Take(Number).ToList();
                IEnumerable<News> data = (from n in db.News
                                          join cn in db.CategoryMapping on n.NewsId equals cn.TargetId
                                          where cn.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                          && n.TypeNewsId == (int)Const.TypeNews.NEWS_VIDEO
                                          && n.Status == (int)Const.Status.NORMAL
                                          && n.NewsId != NewsId
                                          && cn.Status == (int)Const.Status.NORMAL
                                          //&& (categoryMappings.Where(x => x.CategoryId == cn.CategoryId).FirstOrDefault() != null ? true : false)

                                          select n).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();
                return await Task.FromResult((IViewComponentResult)View("BlockVideoRelated", data));
            }
        }
    }
}
