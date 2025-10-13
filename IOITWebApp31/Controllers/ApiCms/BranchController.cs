using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
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
    public class BranchController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("branch", "branch");
        private static string functionCode = "QLCN";

        // GET: api/Branch
        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
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
                    IQueryable<Branch> data = db.Branch.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("BranchId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("BranchId desc");
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
                            e.BranchId,
                            e.Code,
                            e.Name,
                            e.Avatar,
                            e.Email,
                            e.Phone,
                            e.Address,
                            e.Contents,
                            e.Location,
                            e.Lat,
                            e.Long,
                            e.LanguageId,
                            e.CreatedAt,
                            e.UpdatedAt,
                            e.UserId,
                            e.Status,
                            language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new
                            {
                                l.LanguageId,
                                l.Flag,
                                l.Name
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

        // PUT: api/Branch/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranch(int id, BranchDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền sửa chi nhánh!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                if ((userId != data.UserId))
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    Branch branch = db.Branch.Where(b => b.BranchId == id && b.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (branch == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy chi nhánh!");
                        return Ok(def);
                    }


                    Branch exist = db.Branch.Where(b => b.Code == data.Code && b.BranchId != id && b.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (exist != null)
                    {
                        def.meta = new Meta(212, "Mã chi nhánh đã tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        branch.Code = data.Code;
                        branch.Name = data.Name;
                        branch.Avatar = data.Avatar;
                        branch.Email = data.Email;
                        branch.Phone = data.Phone;
                        branch.Address = data.Address;
                        branch.Contents = data.Contents;
                        branch.UserId = userId;
                        branch.LanguageId = data.LanguageId;
                        branch.Location = data.Location;
                        branch.Lat = data.Lat;
                        branch.Long = data.Long;
                        branch.UpdatedAt = DateTime.Now;

                        db.Entry(branch).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.BranchId > 0)
                            {
                                transaction.Commit();
                                //create log
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Sửa chi nhánh " + data.Name;
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.BranchId.ToString();
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

                            def.meta = new Meta(200, "Sửa thành công!");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!BranchExists(branch.BranchId))
                            {
                                def.meta = new Meta(404, "Không tìm thấy chi nhánh!");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Lỗi máy chủ!");
                                return Ok(def);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(def);
            }
        }

        // POST: api/Branch
        [HttpPost]
        public async Task<IActionResult> PostBranch(BranchDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền thêm mới chi nhánh!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                if (userId != data.UserId)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    Branch exist = db.Branch.Where(b => b.Code == data.Code && b.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (exist != null)
                    {
                        def.meta = new Meta(212, "Mã chi nhánh đã tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Branch branch = new Branch();
                        branch.Code = data.Code;
                        branch.Name = data.Name;
                        branch.Avatar = data.Avatar;
                        branch.Email = data.Email;
                        branch.Phone = data.Phone;
                        branch.Address = data.Address;
                        branch.Contents = data.Contents;
                        branch.UserId = userId;
                        branch.LanguageId = data.LanguageId;
                        branch.Location = data.Location;
                        branch.Lat = data.Lat;
                        branch.Long = data.Long;
                        branch.CreatedAt = DateTime.Now;
                        branch.UpdatedAt = DateTime.Now;
                        branch.Status = (int)Const.Status.NORMAL;
                        db.Branch.Add(branch);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.BranchId = branch.BranchId;

                            if (data.BranchId > 0)
                            {
                                transaction.Commit();
                                //create log
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Tạo chi nhánh " + data.Name;
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.BranchId.ToString();
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

                            def.meta = new Meta(200, "Thêm mới thành công!");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (BranchExists(branch.BranchId))
                            {
                                def.meta = new Meta(211, "Mã chi nhánh đã tồn tại!");
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
                def.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(def);
            }
        }

        // DELETE: api/Branch/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "Bạn không có quyền xóa chi nhánh!");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    Branch data = await db.Branch.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy chi nhánh");
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

                            if (data.BranchId > 0)
                            {
                                transaction.Commit();
                                //create log
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Xóa chi nhánh " + data.Name;
                                action.ActionType = (int)Const.ActionType.DELETE;
                                action.TargetId = data.BranchId.ToString();
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
                            if (!BranchExists(data.BranchId))
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

        //API xóa danh sách chi nhánh
        [HttpPut("deletes")]
        public async Task<IActionResult> DeleteBranchs([FromBody] int[] data)
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
                        Branch branch = await db.Branch.FindAsync(data[i]);

                        if (branch == null)
                        {
                            continue;
                        }

                        branch.Status = (int)Const.Status.DELETED;
                        db.Entry(branch).State = EntityState.Modified;

                        //create log
                        Models.EF.Action action = new Models.EF.Action();
                        action.ActionName = "Xóa chi nhánh " + branch.Name;
                        action.ActionType = (int)Const.ActionType.DELETE;
                        action.TargetId = branch.BranchId.ToString();
                        action.TargetName = branch.Name;
                        action.CompanyId = companyId;
                        action.Logs = JsonConvert.SerializeObject(branch);
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

        private bool BranchExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Branch.Count(e => e.BranchId == id) > 0;
            }
        }

        private string IpAddress()
        {
            return "192.168.1.1";
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

    }
}


