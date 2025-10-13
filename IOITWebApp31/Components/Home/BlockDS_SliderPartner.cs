using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Home
{

    [ViewComponent(Name = "BlockDS_SliderPartner")]
    public class BlockDS_SliderPartnerComponent : ViewComponent
    {
        public BlockDS_SliderPartnerComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Id, int LanguageId)
        {
            using (var db = new IOITDataContext())
            {
                ViewBag.Slideshow = db.TypeSlide.Where(ts => ts.Status != (int)Const.Status.DELETED && ts.TypeSlideId == Id).FirstOrDefault().NumberImage;
                ViewBag.TimeAuto = db.TypeSlide.Where(ts => ts.Status != (int)Const.Status.DELETED && ts.TypeSlideId == Id).FirstOrDefault().TimeReset;
                ViewBag.Width = db.TypeSlide.Where(ts => ts.Status != (int)Const.Status.DELETED && ts.TypeSlideId == Id).FirstOrDefault().Width;
                ViewBag.Height = db.TypeSlide.Where(ts => ts.Status != (int)Const.Status.DELETED && ts.TypeSlideId == Id).FirstOrDefault().Height;
                IEnumerable<Slide> data = (from s in db.Slide
                                           where s.TypeSlideId == Id
                                                        && s.Status != (int)Const.Status.DELETED
                                                        && s.LanguageId == LanguageId
                                           select s).OrderBy(e => e.Location).ToList();
                return await Task.FromResult((IViewComponentResult)View("BlockDS_SliderPartner", data));
            }
        }

    }
}
