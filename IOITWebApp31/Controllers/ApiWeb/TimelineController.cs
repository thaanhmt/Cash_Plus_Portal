using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class TimelineController : ControllerBase
    {
        [HttpGet("GetEventPage/{ofset}/{take}")]
        public IActionResult GetEventPage(int ofset = 0, int take = 10)
        {
            DefaultResponse def = new DefaultResponse();
            using (var db = new IOITDataContext())
            {
                def.meta = new Meta(200, "Success");
                IQueryable<News> dataMeta = db.News.Where(c => c.Status == (int)Const.Status.NORMAL && c.TypeNewsId == 7);
                MetaDataDT metaDataDT = new MetaDataDT();
                metaDataDT.Sum = dataMeta.Count();
                def.metadata = metaDataDT;
                IQueryable<News> data = db.News.Where(c => c.Status == (int)Const.Status.NORMAL && c.TypeNewsId == 7).OrderByDescending(e => e.DateStartOn).Skip(ofset).Take(take);
                def.data = data.Select(e => new
                {
                    e.NewsId,
                    e.Title,
                    e.DateStartOn,
                    V = e.DateStartOn.Value.ToString("dd/MM/yyyy"),
                    e.Description,
                    e.Url,
                    e.Note,
                    Count = data.Count(),
                    ListRelated = (from n in db.News
                                   join rl in db.Related on n.NewsId equals rl.TargetRelatedId
                                   where n.Status == (int)Const.Status.NORMAL
                                   && rl.Status == (int)Const.Status.NORMAL
                                   && rl.TargetId == e.NewsId
                                   select n).OrderByDescending(n => n.DateStartActive).ToList(),
                }).OrderByDescending(d => d.DateStartOn).ToList();
                return Ok(def);
            }
        }

    }
}
