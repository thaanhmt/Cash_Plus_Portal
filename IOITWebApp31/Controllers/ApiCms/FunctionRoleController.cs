using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class FunctionRoleController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("error", "error");
        private static string functionCode = "QLQ";

        // GET: api/FunctionRole
        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] FilteredPagination paging)
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
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<Role> data = db.Role.Where(c => c.Status != (int)Const.Status.DELETED);
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
                        def.data = data.Select(c => new
                        {
                            c.RoleId,
                            c.Code,
                            c.Name,
                            c.Note,
                            c.Status,
                            c.Type,
                            c.LevelRole,
                            listFunction = db.FunctionRole.Where(e => e.TargetId == c.RoleId && e.Type == (int)Const.TypeFunction.FUNCTION_ROLE && e.Status != (int)Const.Status.DELETED).Select(e => new
                            {
                                e.FunctionId,
                                e.ActiveKey
                            }).ToList(),
                        }).ToList();
                    }
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/FunctionRole/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFunctionRole(int id)
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
            using (var db = new IOITDataContext())
            {
                FunctionRole data = await db.FunctionRole.FindAsync(id);

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

        // PUT: api/FunctionRole/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFunctionRole(int id, [FromBody] RoleDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
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
                if (data.UpdatedId != userId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                if (id != data.RoleId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    Role current = await db.Role.FindAsync(id);
                    if (current == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        current.Code = data.Code;
                        current.Name = data.Name;
                        current.Type = data.Type;
                        current.LevelRole = data.LevelRole;
                        current.Note = data.Note;
                        current.UpdatedAt = DateTime.Now;
                        current.UpdatedId = data.UpdatedId;
                        try
                        {
                            //update list function
                            foreach (var item in data.listFunction)
                            {
                                var functionNew = db.FunctionRole.Where(e => e.TargetId == data.RoleId
                                && e.FunctionId == item.FunctionId
                                && e.Type == (int)Const.TypeFunction.FUNCTION_ROLE
                                && e.Status != (int)Const.Status.DELETED).ToList();
                                //add new
                                if (functionNew.Count <= 0)
                                {
                                    FunctionRole functionRole = new FunctionRole();
                                    functionRole.TargetId = data.RoleId;
                                    functionRole.FunctionId = item.FunctionId;
                                    functionRole.ActiveKey = item.ActiveKey;
                                    functionRole.Type = (int)Const.TypeFunction.FUNCTION_ROLE;
                                    functionRole.CreatedAt = DateTime.Now;
                                    functionRole.UpdatedAt = DateTime.Now;
                                    functionRole.UserId = data.UpdatedId;
                                    functionRole.Status = (int)Const.Status.NORMAL;
                                    db.FunctionRole.Add(functionRole);
                                }
                                else
                                {
                                    //update
                                    var functionRoleExit = functionNew.FirstOrDefault();
                                    functionRoleExit.ActiveKey = item.ActiveKey;
                                    functionRoleExit.UpdatedAt = DateTime.Now;
                                    functionRoleExit.UserId = data.UpdatedId;
                                    db.Entry(functionRoleExit).State = EntityState.Modified;
                                }
                            }

                            db.Entry(current).State = EntityState.Modified;
                            await db.SaveChangesAsync();

                            if (current.RoleId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!RoleExists(id))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                                throw;
                            }
                        }
                    }
                }
            }
            catch
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // POST: api/FunctionRole
        [HttpPost]
        public async Task<IActionResult> PostFunctionRole([FromBody] RoleDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
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
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Role role = new Role();
                        role.Code = data.Code;
                        role.Name = data.Name;
                        role.Note = data.Note;
                        role.Type = data.Type;
                        role.LevelRole = data.LevelRole;
                        role.CreatedAt = DateTime.Now;
                        role.UpdatedAt = DateTime.Now;
                        role.CreatedId = data.CreatedId;
                        role.UpdatedId = data.UpdatedId;
                        role.Status = (int)Const.Status.NORMAL;
                        await db.Role.AddAsync(role);

                        try
                        {
                            await db.SaveChangesAsync();

                            data.RoleId = role.RoleId;

                            //add function
                            foreach (var item in data.listFunction)
                            {
                                FunctionRole functionRole = new FunctionRole();
                                functionRole.TargetId = data.RoleId;
                                functionRole.FunctionId = item.FunctionId;
                                functionRole.ActiveKey = item.ActiveKey;
                                functionRole.Type = (int)Const.TypeFunction.FUNCTION_ROLE;
                                functionRole.CreatedAt = DateTime.Now;
                                functionRole.UpdatedAt = DateTime.Now;
                                functionRole.UserId = data.CreatedId;
                                functionRole.Status = (int)Const.Status.NORMAL;
                                db.FunctionRole.Add(functionRole);
                            }
                            await db.SaveChangesAsync();

                            if (data.RoleId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;

                            return Ok(def);
                        }
                        catch (DbUpdateException ex)
                        {
                            log.Error("DbUpdateException:" + ex);
                            transaction.Rollback();
                            if (RoleExists(data.RoleId))
                            {
                                def.meta = new Meta(212, "Exist");
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
                log.Error("Exception:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // DELETE: api/FunctionRole/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFunctionRole(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
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
                using (var db = new IOITDataContext())
                {
                    Role data = await db.Role.FindAsync(id);
                    data.Status = (int)Const.Status.DELETED;

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Entry(data).State = EntityState.Modified;

                        var fr = db.FunctionRole.Where(e => e.TargetId == data.RoleId && e.Type == (int)Const.TypeFunction.FUNCTION_ROLE
                            && e.Status != (int)Const.Status.DELETED).ToList();
                        db.FunctionRole.RemoveRange(fr);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.RoleId > 0)
                                transaction.Commit();
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

                //db.Entry(current).State = EntityState.Modified;

                //try
                //{
                //    await db.SaveChangesAsync();
                //    def.meta = new Meta(200, "Success");
                //    return Ok(def);
                //}
                //catch (DbUpdateConcurrencyException)
                //{
                //    if (!RoleExists(id))
                //    {
                //        def.meta = new Meta(500, "Not Found");
                //        return Ok(def);
                //    }
                //    else
                //    {
                //        def.meta = new Meta(500, "Internal Server Error");
                //        return Ok(def);
                //        throw;
                //    }
                //}
            }
            catch (Exception e)
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        //API xóa danh sách quyền
        [HttpPut("deletes")]
        public async Task<IActionResult> DeleteFunctionRoles([FromBody] int[] data)
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
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (data == null)
                {
                    def.meta = new Meta(211, "Array Empty!");
                    return Ok(def);
                }

                if (data.Count() == 0)
                {
                    def.meta = new Meta(211, "Array Empty!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    for (int i = 0; i < data.Count(); i++)
                    {
                        Role role = await db.Role.FindAsync(data[i]);

                        if (role == null)
                        {
                            continue;
                        }

                        role.Status = (int)Const.Status.DELETED;
                        db.Entry(role).State = EntityState.Modified;

                        var fr = db.FunctionRole.Where(e => e.TargetId == role.RoleId && e.Type == (int)Const.TypeFunction.FUNCTION_ROLE
                            && e.Status != (int)Const.Status.DELETED).ToList();
                        fr.ForEach(f => f.Status = (int)Const.Status.DELETED);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            await db.SaveChangesAsync();
                            transaction.Commit();
                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            def.meta = new Meta(500, "Internal Server Error");
                            return Ok(def);
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

        //private List<FunctionRoleDT> ListFunctionRole(List<FunctionRole> list, int functionId)
        //{
        //    List<FunctionDT> functionRoles = new List<FunctionDT>();
        //    var data = db.Functions.Where(e => e.FunctionParent == functionId && e.Status != (int)Const.Status.DELETED).OrderBy(e=>e.Location).ToList();
        //    if (data.Count > 0)
        //    {
        //        foreach (var item in data)
        //        {
        //            FunctionDT functionRole = new FunctionDT();
        //            //check selected
        //            var fr = list.Where(e => e.FunctionId == item.FunctionId).ToList();
        //            var child = db.Functions.Where(e => e.FunctionParent == item.FunctionId && e.Status != (int)Const.Status.DELETED).OrderBy(e => e.Location).ToList();

        //            if (fr.Count > 0)
        //            {
        //                bool check = true;
        //                foreach(var itemChild in child)
        //                {
        //                    var frChild = list.Where(e => e.FunctionId == itemChild.FunctionId).ToList();
        //                    if (frChild.Count <= 0)
        //                    {
        //                        check = false;
        //                        break;
        //                    }
        //                }
        //                functionRole.selected = check;
        //            }
        //            else
        //                functionRole.selected = false;

        //            functionRole.id = item.FunctionId;
        //            functionRole.label = item.Name;
        //            functionRole.icon = item.Icon;
        //            functionRole.location = item.Location;
        //            functionRole.children = ListFunctionRole(list, item.FunctionId);

        //            functionRoles.Add(functionRole);
        //        }
        //    }
        //    return functionRoles;
        //}

        //private void InsertFunctionRole(List<FunctionRoleDT> list, int roleId)
        //{
        //    //var data=list.Where(e=>e.)
        //    foreach (var item in list)
        //    {
        //        if (item. == true)
        //        {
        //            FunctionRole current = new FunctionRole();
        //            current.FunctionId = item.id;
        //            current.TargetId = roleId;
        //            current.ActiveKey = item.A;
        //            current.Type = (int)Const.TypeFunction.FUNCTION_ROLE;
        //            current.Status = (int)Const.Status.NORMAL;

        //            db.FunctionRoles.Add(current);
        //            db.SaveChanges();

        //            if (item.children.Count > 0)
        //                InsertFunctionRole(item.children, roleId);
        //        }
        //        else
        //        {
        //            //check xem có chức năng con nào dc check ko
        //            var check = item.children.Where(e => e.selected == true).ToList();
        //            if(check.Count > 0)
        //            {
        //                //thêm cha
        //                FunctionRole current = new FunctionRole();
        //                current.FunctionId = item.id;
        //                current.TargetId = roleId;
        //                current.Type = (int)Const.TypeFunction.FUNCTION_ROLE;
        //                current.ActiveKey = "111111111";
        //                current.Status = (int)Const.Status.NORMAL;

        //                db.FunctionRoles.Add(current);
        //                db.SaveChanges();

        //                //thêm con
        //                if (item.children.Count > 0)
        //                    InsertFunctionRole(item.children, roleId);
        //            }
        //        }
        //    }

        //}

        private bool RoleExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Role.Count(e => e.RoleId == id) > 0;
            }
        }
    }
}
