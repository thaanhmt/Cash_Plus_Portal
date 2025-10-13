using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class NewsController : Controller
    {
        private static readonly ILog log = LogMaster.GetLogger("news", "news");
        private static string functionCode = "QLND";

        // Quang - 15.09.2023
        [HttpGet("GetNews")]
        public async Task<IActionResult> GetNews()
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                using (var db = new IOITDataContext())
                {
                    var data = (db.News
                         .Where(w => w.Status == 1) 
                            .OrderByDescending(x => x.NewsId)
                            .Take(34))
                            .ToArray();

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    def.meta = new Meta(200, "Success");
                    def.data = data;
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }
        [HttpPost("GetNewById/{id}")]
        public async Task<IActionResult> GetNewById(long id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                using (var db = new IOITDataContext())
                {
                    var data = db.News.FirstOrDefault(w => w.Status == 1 && w.NewsId == id);

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    def.meta = new Meta(200, "Success");
                    def.data = data;
                    ViewBag.SeoTitle = data.Title;
                    ViewBag.SeoDescription = data.Description;
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }
    }
}
