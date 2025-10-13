using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
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
    public class RoleController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("role", "role");

        // GET: api/Role
        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            int roleMax = int.Parse(identity.Claims.Where(c => c.Type == "RoleMax").Select(c => c.Value).SingleOrDefault());
            int roleLevel = int.Parse(identity.Claims.Where(c => c.Type == "RoleLevel").Select(c => c.Value).SingleOrDefault());
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    try
                    {
                        def.meta = new Meta(200, "Success");
                        IQueryable<Role> data = db.Role.Where(c => c.Status != (int)Const.Status.DELETED);
                        //if (roleMax != 1 && roleMax != 8)
                        //{
                        //    paging.query = "LevelRole > " + roleLevel;
                        //}
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
                                data = data.OrderBy("RoleId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                                data = data.OrderBy("RoleId desc");
                            }
                        }

                        if (paging.select != null && paging.select != "")
                        {
                            paging.select = "new(" + paging.select + ")";
                            paging.select = HttpUtility.UrlDecode(paging.select);
                            def.data = data.Select(paging.select);
                        }
                        else
                        {
                            def.data = data.Select(e => new
                            {
                                e.RoleId,
                                e.Name,
                                e.Code,
                                e.Note,
                                e.LevelRole,
                                e.Status,
                                listFunction = db.FunctionRole.Where(fr => fr.TargetId == e.RoleId).Select(fr => new
                                {
                                    fr.FunctionId,
                                    fr.ActiveKey
                                }).ToList(),
                            }).ToList();
                        }
                        return Ok(def);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/Role/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            DefaultResponse def = new DefaultResponse();
            using (var db = new IOITDataContext())
            {
                Role role = await db.Role.FindAsync(id);

                if (role == null)
                {
                    def.meta = new Meta(404, "Not Found");
                    return Ok(def);
                }

                def.meta = new Meta(200, "Success");
                def.data = role;
                return Ok(def);
            }
        }

        // PUT: api/Role/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, RoleDTO role)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (id != role.RoleId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    Role checkItemExist = db.Role.Where(f => f.RoleId != role.RoleId && f.Code == role.Code && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkItemExist != null)
                    {
                        def.meta = new Meta(211, "Code Exist!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Role data = await db.Role.FindAsync(id);
                        data.Code = role.Code;
                        data.Name = role.Name;
                        data.Note = role.Note;

                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.RoleId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Sửa phân quyền “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.RoleId.ToString();
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
                            if (RoleExists(data.RoleId))
                            {
                                def.meta = new Meta(212, "Exist");
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
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // POST: api/Role
        [HttpPost]
        public async Task<IActionResult> PostRole(RoleDTO role)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    Role checkItemExist = db.Role.Where(f => f.Code == role.Code && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkItemExist != null)
                    {
                        def.meta = new Meta(211, "Code Exist!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {

                        Role roles = new Role();
                        roles.Code = role.Code;
                        roles.Note = role.Note;
                        roles.Name = role.Name;
                        roles.Status = (int)Const.Status.NORMAL;

                        db.Role.Add(roles);
                        role.RoleId = roles.RoleId;

                        try
                        {
                            await db.SaveChangesAsync();

                            if (roles.RoleId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Thêm phân quyền “" + role.Name + "”";
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = role.RoleId.ToString();
                                action.TargetName = role.Name;
                                action.CompanyId = companyId;
                                action.Logs = JsonConvert.SerializeObject(role);
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
                            def.data = role;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (RoleExists(roles.RoleId))
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
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // DELETE: api/Role/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Role data = await db.Role.FindAsync(id);
                        data.Status = (int)Const.Status.DELETED;

                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.RoleId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Xoá phân quyền “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.DELETE;
                                action.TargetId = data.RoleId.ToString();
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
                            if (RoleExists(data.RoleId))
                            {
                                def.meta = new Meta(212, "Exist");
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
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private bool RoleExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Role.Count(e => e.RoleId == id) > 0;
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
