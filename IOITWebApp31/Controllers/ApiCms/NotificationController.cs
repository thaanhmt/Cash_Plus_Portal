using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiCms
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {

        private static readonly ILog log = LogMaster.GetLogger("notification", "notification");
        private static string functionCode = "QLTB";

        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "Bạn không có quyền xem danh sách thông báo!");
                return Ok(def);
            }
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<Notification> data = db.Notification.Where(c => c.Status != (int)Const.Status.DELETED);
                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    MetaDataDT metaDataDT = new MetaDataDT();
                    metaDataDT.Sum = data.Count();
                    metaDataDT.Normal = data.Where(e => e.Status == 1).Count();
                    metaDataDT.Temp = data.Where(e => e.Status == 10).Count();

                    def.metadata = metaDataDT;

                    if (paging.page_size > 0)
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                        else
                        {
                            data = data.OrderBy("CreatedAt desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                    }
                    else
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by);
                        }
                        else
                        {
                            data = data.OrderBy("CreatedAt desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = await data.Select(paging.select).ToDynamicListAsync();
                    }
                    else
                        def.data = await data.Select(e => new
                        {
                            e.NotificationId,
                            e.Title,
                            e.Description,
                            e.Contents,
                            e.UserPushId,
                            e.UserReadId,
                            e.Status,
                            e.CreatedAt,
                            e.UpdatedAt,
                            userPush = db.Customer.Where(c => c.CustomerId == e.UserPushId).Select(c => new
                            {
                                c.CustomerId,
                                c.FullName,
                                c.Avata
                            }).FirstOrDefault(),
                            userRead = db.Customer.Where(c => c.CustomerId == e.UserReadId).Select(c => new
                            {
                                c.CustomerId,
                                c.FullName,
                                c.Avata
                            }).FirstOrDefault(),
                        }).ToListAsync();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }


    }
}
