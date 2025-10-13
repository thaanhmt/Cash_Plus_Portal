using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.ApiCMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TypeAttributeItemController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("type-attribute-item", "type-attribute-item");
        private static string functionCode = "QLLH";

        // GET: api/TypeAttributeItem
        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            //var identity = (ClaimsIdentity)User.Identity;
            //string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            //if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            //{
            //    def.meta = new Meta(222, "No permission");
            //    return Ok(def);
            //}
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<TypeAttributeItem> data = db.TypeAttributeItem.Where(c => c.Status != (int)Const.Status.DELETED);
                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    def.metadata = data.Count();

                    if (paging.page_size > 0)
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                        else
                        {
                            data = data.OrderBy("TypeAttributeItemId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("TypeAttributeItemId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select);
                    }
                    else
                        def.data = data.ToList();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/TypeAttributeItem/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeAttributeItem(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    TypeAttributeItem data = await db.TypeAttributeItem.FindAsync(id);

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
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // PUT: api/TypeAttributeItem/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeAttributeItem(int id, [FromBody] TypeAttributeItem data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                if (userId != data.UserId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }

                if (data.TypeAttributeId == null || data.TypeAttributeId > 0)
                {
                    def.meta = new Meta(211, "TypeAttributeId Null or TypeAttributeId > 0");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UserId = userId;
                        data.UpdatedAt = DateTime.Now;
                        db.Entry(data).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.TypeAttributeItemId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Sửa Item loại hình “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.TypeAttributeItemId.ToString();
                                action.TargetName = data.Name;
                                action.CompanyId = companyId;
                                action.Logs = JsonConvert.SerializeObject(data);
                                action.Time = 0;
                                action.Ipaddress = IpAddress();
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserPushId = userId;
                                action.UserId = userId;
                                action.Status = (int)Const.Status.NORMAL;
                                await db.Action.AddAsync(action);
                                await db.SaveChangesAsync();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!TypeAttributeItemExists(data.TypeAttributeItemId))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // POST: api/TypeAttributeItem
        [HttpPost]
        public async Task<IActionResult> PostTypeAttributeItem(TypeAttributeItem data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }

                if (data.TypeAttributeId == null || data.TypeAttributeId < 0)
                {
                    def.meta = new Meta(211, "TypeAttributeId Null!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UserId = userId;
                        data.CreatedAt = DateTime.Now;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.NORMAL;
                        db.TypeAttributeItem.Add(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.TypeAttributeItemId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Thêm Item loại hình “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.TypeAttributeItemId.ToString();
                                action.TargetName = data.Name;
                                action.CompanyId = companyId;
                                action.Logs = JsonConvert.SerializeObject(data);
                                action.Time = 0;
                                action.Ipaddress = IpAddress();
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserPushId = userId;
                                action.UserId = userId;
                                action.Status = (int)Const.Status.NORMAL;
                                await db.Action.AddAsync(action);
                                await db.SaveChangesAsync();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (TypeAttributeItemExists(data.TypeAttributeItemId))
                            {
                                def.meta = new Meta(211, "Exist");
                                return Ok(def);
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // DELETE: api/TypeAttributeItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeAttributeItem(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    TypeAttributeItem data = await db.TypeAttributeItem.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UserId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.Entry(data).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.TypeAttributeItemId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Xoá Item loại hình “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.DELETE;
                                action.TargetId = data.TypeAttributeItemId.ToString();
                                action.TargetName = data.Name;
                                action.CompanyId = companyId;
                                action.Logs = JsonConvert.SerializeObject(data);
                                action.Time = 0;
                                action.Ipaddress = IpAddress();
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserPushId = userId;
                                action.UserId = userId;
                                action.Status = (int)Const.Status.NORMAL;
                                await db.Action.AddAsync(action);
                                await db.SaveChangesAsync();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!TypeAttributeItemExists(data.TypeAttributeItemId))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private bool TypeAttributeItemExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.TypeAttributeItem.Count(e => e.TypeAttributeItemId == id) > 0;
            }
        }
        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}


