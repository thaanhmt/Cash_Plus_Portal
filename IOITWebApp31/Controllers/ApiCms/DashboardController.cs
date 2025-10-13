using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace IOITWebApp31.Controllers.ApiCms
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        [HttpGet("GetDataSet")]
        public async Task<IActionResult> GetDataSet()
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            //var identity = (ClaimsIdentity)User.Identity;
            //string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            //if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            //{
            //    def.meta = new Meta(222, "Bạn không có quyền xem bộ dữ liệu!");
            //    return Ok(def);
            //}
            try
            {
                using (var db = new IOITDataContext())
                {
                    DashboardDT dashboard = new DashboardDT();
                    var data = await (from ds in db.DataSet
                                      where ds.Status != (int)Const.Status.DELETED
                                      select new DataSetDTO
                                      {
                                          DataSetId = ds.DataSetId,
                                          ViewNumber = ds.ViewNumber,
                                          DownNumber = ds.DownNumber,
                                      }).ToListAsync();
                    var dataUser = await (from ds in db.Customer
                                          where ds.Status != (int)Const.Status.DELETED
                                          select ds).ToListAsync();
                    dashboard.UserNumber = dataUser != null ? dataUser.Count : 0;
                    if (data == null)
                    {
                        dashboard.DataSetNumber = 0;
                        dashboard.ViewNumber = 0;
                        dashboard.DownNumber = 0;
                    }
                    else
                    {
                        dashboard.DataSetNumber = data.Count;
                        dashboard.ViewNumber = data.Sum(e => e.ViewNumber);
                        dashboard.DownNumber = data.Sum(e => e.DownNumber);
                    }

                    def.meta = new Meta(200, "Success");
                    def.data = dashboard;
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
