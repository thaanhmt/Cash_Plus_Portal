using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Detail
{
    [ViewComponent(Name = "BlockEventRelated")]
    public class BlockEventRelatedComponent : ViewComponent
    {
        public BlockEventRelatedComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int NewsId)
        {
            using (var db = new IOITDataContext())
            {
                ViewBag.EventId = NewsId;
                ViewBag.DescriptionEvent = db.News.Where(n => n.NewsId == NewsId && n.Status == 1).FirstOrDefault().Description;
                //tin liên quan sự kiện
                IEnumerable<News> data = (from n in db.News
                                          join rl in db.Related on n.NewsId equals rl.TargetRelatedId
                                          where n.Status == (int)Const.Status.NORMAL
                                          && rl.Status == (int)Const.Status.NORMAL
                                          && rl.TargetId == NewsId
                                          select n).OrderByDescending(e => e.DateStartActive).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockEventRelated", data));
            }
        }
    }
}
