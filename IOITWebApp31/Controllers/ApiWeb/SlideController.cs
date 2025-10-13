using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class SlideController : ControllerBase
    {
        [HttpGet("GetSlide")]
        public IActionResult GetSlide()
        {
            DefaultResponse def = new DefaultResponse();
            using (var db = new IOITDataContext())
            {
                def.meta = new Meta(200, "Success");
                IQueryable<Slide> data = db.Slide.Where(c => c.Status == (int)Const.Status.NORMAL);
                def.data = data.Select(e => new
                {
                    e.SlideId,
                    e.Description,
                    e.Image,
                    e.Url,
                    e.TypeSlideId,
                    e.Location,
                    e.CreatedAt
                }).OrderByDescending(d => d.CreatedAt).ToList();
                return Ok(def);
            }
        }
    }
}
