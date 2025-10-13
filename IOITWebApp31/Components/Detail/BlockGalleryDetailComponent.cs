using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Detail
{
    [ViewComponent(Name = "BlockGalleryDetail")]
    public class BlockGalleryDetailComponent : ViewComponent
    {
        public BlockGalleryDetailComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int NewsId)
        {
            using (var db = new IOITDataContext())
            {
                //lấy tên danh mục
                List<CategoryMapping> categoryMappings = db.CategoryMapping.Where(cm => cm.TargetId == NewsId && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS && cm.Status != (int)Const.Status.DELETED).ToList();

                IEnumerable<Attactment> data = (from a in db.Attactment
                                                join n in db.News on a.TargetId equals n.NewsId
                                                where a.Status != (int)Const.Status.DELETED
                                                && n.Status != (int)Const.Status.DELETED
                                                && n.TypeNewsId == (int)Const.TypeNews.NEWS_IMAGE
                                                && a.TargetId == NewsId
                                                && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE
                                                select a).OrderByDescending(e => e.CreatedAt).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockGalleryDetail", data));
            }
        }
    }
}
