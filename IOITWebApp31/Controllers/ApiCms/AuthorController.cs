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

namespace IOITWebApp31.Controllers.ApiCms
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("author", "author");
        private static string functionCode = "QLTG";

        // GET: api/author
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
                    IQueryable<Author> data = db.Author.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("AuthorId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("AuthorId desc");
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
                            e.AuthorId,
                            e.Name,
                            e.Avatar,
                            e.UserMapId,
                            e.Type,
                            e.CreatedAt,
                            e.UpdatedAt,
                            e.UserId,
                            e.Status,
                            e.Address,
                            e.Cccd,
                            e.NumberPhone,
                            e.FullName,
                            user = db.User.Where(u => u.UserId == e.UserMapId).Select(u => new
                            {
                                u.FullName,
                            }).FirstOrDefault()
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
        // GET: api/user
        [HttpGet("GetByPageUser")]
        public async Task<IActionResult> GetByPageUser()
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
                def.meta = new Meta(200, "Success");
                IQueryable<User> data = db.User.Where(c => c.Status != (int)Const.Status.DELETED);
                def.data = await data.Select(e => new
                {
                    e.UserId,
                    e.FullName,
                    e.UserName,
                    e.Email,
                    e.Address,
                    e.CreatedAt,
                    e.UpdatedAt,
                    e.Phone,
                    e.Status,
                }).ToListAsync();

                return Ok(def);
            }
        }
        // PUT: api/Author/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền sửa tác giả!");
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
                    Author author = db.Author.Where(b => b.AuthorId == id && b.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (author == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy tác giả!");
                        return Ok(def);
                    }

                    Author exist = db.Author.Where(b => b.Name.Trim() == data.Name.Trim()
                    && b.AuthorId != id && b.UserMapId == userId && b.Type == data.Type
                    && b.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (exist != null)
                    {
                        def.meta = new Meta(212, "Tên tác giả đã tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        author.Name = data.Name;
                        author.Avatar = data.Avatar;
                        author.UserMapId = data.UserMapId;
                        author.Type = data.Type;
                        author.UserId = userId;
                        author.Address = data.Address;
                        author.Cccd = data.Cccd;
                        author.FullName = data.FullName;
                        author.NumberPhone = data.NumberPhone;
                        author.UpdatedAt = DateTime.Now;

                        db.Entry(author).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.AuthorId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Sửa tác giả “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.AuthorId.ToString();
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
                            if (!AuthorExists(author.AuthorId))
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
                def.meta = new Meta(500, "Có lỗi xẩy ra, vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        // POST: api/Author
        [HttpPost]
        public async Task<IActionResult> PostAuthor(AuthorDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền thêm tác giả!");
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
                    Author exist = db.Author.Where(b => b.Name.Trim() == data.Name.Trim()
                    && b.UserMapId == userId && b.Type == data.Type
                    && b.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (exist != null)
                    {
                        def.meta = new Meta(212, "Tên tác giả đã tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Author author = new Author();
                        author.Name = data.Name;
                        author.Avatar = data.Avatar;
                        author.UserMapId = data.UserMapId;
                        author.Type = data.Type;
                        author.UserId = userId;
                        author.Address = data.Address;
                        author.Cccd = data.Cccd;
                        author.FullName = data.FullName;
                        author.NumberPhone = data.NumberPhone;
                        author.CreatedAt = DateTime.Now;
                        author.UpdatedAt = DateTime.Now;
                        author.Status = (int)Const.Status.NORMAL;
                        db.Author.Add(author);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.AuthorId = author.AuthorId;

                            if (data.AuthorId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Thêm tác giả “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.AuthorId.ToString();
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
                            if (AuthorExists(author.AuthorId))
                            {
                                def.meta = new Meta(211, "Tác giả đã tồn tại!");
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
                def.meta = new Meta(500, "Có lỗi xẩy ra, vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        // DELETE: api/Author/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "Bạn không có quyền xóa tác giả!");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    Author data = await db.Author.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy tác giả");
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

                            if (data.AuthorId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Xoá tác giả “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.DELETE;
                                action.TargetId = data.AuthorId.ToString();
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
                            if (!AuthorExists(data.AuthorId))
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
                def.meta = new Meta(500, "Có lỗi xẩy ra, vui lòng thử lại sau");
                return Ok(def);
            }
        }

        //API xóa danh sách tác giả
        [HttpPut("deletes")]
        public async Task<IActionResult> DeleteAuthors([FromBody] int[] data)
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
                        Author author = await db.Author.FindAsync(data[i]);

                        if (author == null)
                        {
                            continue;
                        }

                        author.Status = (int)Const.Status.DELETED;
                        db.Entry(author).State = EntityState.Modified;
                        Models.EF.Action action = new Models.EF.Action();
                        action.ActionName = "Xoá tác giả “" + author.Name + "”";
                        action.ActionType = (int)Const.ActionType.DELETE;
                        action.TargetId = author.AuthorId.ToString();
                        action.TargetName = author.Name;
                        action.CompanyId = companyId;
                        action.Logs = JsonConvert.SerializeObject(author);
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
                            def.meta = new Meta(500, "Có lỗi xẩy ra, vui lòng thử lại sau");
                            return Ok(def);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Có lỗi xẩy ra, vui lòng thử lại sau");
                return Ok(def);
            }
        }

        private bool AuthorExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Author.Count(e => e.AuthorId == id) > 0;
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
