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
using System.Collections.Generic;
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
    public class UnitController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("unit", "unit");
        private static string functionCode = "CQTC";

        // GET: api/Unit
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
                    IQueryable<Unit> data = db.Unit.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("UnitId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("UnitId desc");
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
                            e.UnitId,
                            e.Code,
                            e.Name,
                            e.ShortName,
                            e.NameEn,
                            e.Email,
                            e.Phone,
                            e.Fax,
                            e.Website,
                            e.IdNumber,
                            e.DateNumber,
                            e.AddressNumber,
                            e.UnitParentId,
                            e.Description,
                            e.Contents,
                            e.Url,
                            e.Image,
                            e.Icon,
                            e.IconFa,
                            e.IconText,
                            e.Location,
                            e.Type,
                            e.ProvinceId,
                            e.DistrictId,
                            e.WardId,
                            e.Address,
                            e.AdminId,
                            e.EmailAdmin,
                            e.NameAdmin,
                            e.LanguageId,
                            e.WebsiteId,
                            e.CompanyId,
                            e.MetaTitle,
                            e.MetaKeyword,
                            e.MetaDescription,
                            e.CreatedAt,
                            e.UpdatedAt,
                            e.CreatedId,
                            e.UpdatedId,
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

        // PUT: api/Unit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnit(int id, UnitDTO data)
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
                if ((userId != data.UpdatedId))
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    Unit unit = db.Unit.Where(b => b.UnitId == id && b.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (unit == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy Cơ quan/tổ chức!");
                        return Ok(def);
                    }
                    Unit exist = db.Unit.Where(b => b.Code.Trim() == data.Code.Trim() && b.UnitId != id && b.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (exist != null)
                    {
                        def.meta = new Meta(212, "Mã Cơ quan/tổ chức đã tồn tại!");
                        return Ok(def);
                    }
                    if (id == data.UnitParentId)
                    {
                        def.meta = new Meta(212, "Không được chọn chính mình làm danh mục cha!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        unit.Code = data.Code;
                        unit.Name = data.Name;
                        unit.ShortName = data.ShortName;
                        unit.NameEn = data.NameEn;
                        unit.Email = data.Email;
                        unit.Phone = data.Phone;
                        unit.Fax = data.Fax;
                        unit.Website = data.Website;
                        unit.IdNumber = data.IdNumber;
                        unit.DateNumber = data.DateNumber;
                        unit.AddressNumber = data.AddressNumber;
                        unit.UnitParentId = data.UnitParentId;
                        unit.Description = data.Description;
                        unit.Contents = data.Contents;
                        unit.Url = data.Url != null ? data.Url : Utils.NonUnicode(data.Name);
                        unit.Image = data.Image != null ? data.Image : "";
                        unit.Icon = data.Icon;
                        unit.IconFa = data.IconFa;
                        unit.IconText = data.IconText;
                        unit.Location = data.Location;
                        unit.Type = data.Type;
                        unit.ProvinceId = data.ProvinceId;
                        unit.DistrictId = data.DistrictId;
                        unit.WardId = data.WardId;
                        unit.Address = data.Address;
                        unit.AdminId = data.AdminId;
                        unit.EmailAdmin = data.EmailAdmin;
                        unit.NameAdmin = data.NameAdmin;
                        unit.LanguageId = data.LanguageId;
                        unit.WebsiteId = data.WebsiteId;
                        unit.CompanyId = data.CompanyId;
                        unit.MetaTitle = data.MetaTitle;
                        unit.MetaKeyword = data.MetaKeyword;
                        unit.MetaDescription = data.MetaDescription;
                        unit.UpdatedId = userId;
                        unit.UpdatedAt = DateTime.Now;
                        unit.Status = data.Status;

                        db.Entry(unit).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.UnitId > 0)
                            {
                                transaction.Commit();
                                //create log
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Sửa Cơ quan/tổ chức " + data.Name;
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.UnitId.ToString();
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
                            if (!UnitExists(unit.UnitId))
                            {
                                def.meta = new Meta(404, "Không tìm thấy cơ quan/tổ chức!");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Lỗi hệ thống!");
                                return Ok(def);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Lỗi hệ thống!");
                return Ok(def);
            }
        }

        // POST: api/unit
        [HttpPost]
        public async Task<IActionResult> PostUnit(UnitDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền thêm mới cơ quan/tổ chức!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                if (userId != data.CreatedId)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    Unit exist = db.Unit.Where(b => b.Code.Trim() == data.Code.Trim() && b.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (exist != null)
                    {
                        def.meta = new Meta(212, "Mã Cơ quan/tổ chức đã tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Unit unit = new Unit();
                        unit.Code = data.Code;
                        unit.Name = data.Name;
                        unit.ShortName = data.ShortName;
                        unit.NameEn = data.NameEn;
                        unit.Email = data.Email;
                        unit.Phone = data.Phone;
                        unit.Fax = data.Fax;
                        unit.Website = data.Website;
                        unit.IdNumber = data.IdNumber;
                        unit.DateNumber = data.DateNumber;
                        unit.AddressNumber = data.AddressNumber;
                        unit.UnitParentId = data.UnitParentId;
                        unit.Description = data.Description;
                        unit.Contents = data.Contents;
                        unit.Url = data.Url != null ? data.Url : Utils.NonUnicode(data.Name);
                        unit.Image = data.Image != null ? data.Image : "";
                        unit.Icon = data.Icon;
                        unit.IconFa = data.IconFa;
                        unit.IconText = data.IconText;
                        unit.Location = data.Location;
                        unit.Type = data.Type == null ? data.Type : 1;
                        unit.ProvinceId = data.ProvinceId;
                        unit.DistrictId = data.DistrictId;
                        unit.WardId = data.WardId;
                        unit.Address = data.Address;
                        unit.AdminId = data.AdminId;
                        unit.EmailAdmin = data.EmailAdmin;
                        unit.NameAdmin = data.NameAdmin;
                        unit.LanguageId = data.LanguageId;
                        unit.WebsiteId = data.WebsiteId;
                        unit.CompanyId = data.CompanyId;
                        unit.MetaTitle = data.MetaTitle != null ? data.MetaTitle : data.Name;
                        unit.MetaKeyword = data.MetaKeyword != null ? data.MetaKeyword : data.Name;
                        unit.MetaDescription = data.MetaDescription != null ? data.MetaDescription : data.Name;
                        unit.CreatedId = userId;
                        unit.UpdatedId = userId;
                        unit.CreatedAt = DateTime.Now;
                        unit.UpdatedAt = DateTime.Now;
                        unit.Status = data.Status;
                        await db.Unit.AddAsync(unit);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.UnitId = unit.UnitId;

                            if (data.UnitId > 0)
                            {
                                transaction.Commit();
                                //create log
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Tạo Cơ quan/tổ chức " + data.Name;
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.UnitId.ToString();
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
                            if (UnitExists(unit.UnitId))
                            {
                                def.meta = new Meta(211, "Mã cơ quan/tổ chức đã tồn tại!");
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

        // DELETE: api/unit/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "Bạn không có quyền xóa Cơ quan/tổ chức!");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    Unit data = await db.Unit.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy Cơ quan/tổ chức");
                        return Ok(def);
                    }
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UpdatedId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.Entry(data).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.UnitId > 0)
                            {
                                transaction.Commit();
                                //create log
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Xóa Cơ quan/tổ chức " + data.Name;
                                action.ActionType = (int)Const.ActionType.DELETE;
                                action.TargetId = data.UnitId.ToString();
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
                            if (!UnitExists(data.UnitId))
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

        //API xóa danh sách Cơ quan/tổ chức
        [HttpPut("deletes")]
        public async Task<IActionResult> DeleteUnits([FromBody] int[] data)
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
                        Unit unit = await db.Unit.FindAsync(data[i]);

                        if (unit == null)
                        {
                            continue;
                        }

                        unit.UpdatedId = userId;
                        unit.Status = (int)Const.Status.DELETED;
                        db.Entry(unit).State = EntityState.Modified;

                        //create log
                        Models.EF.Action action = new Models.EF.Action();
                        action.ActionName = "Xóa Cơ quan/tổ chức " + unit.Name;
                        action.ActionType = (int)Const.ActionType.DELETE;
                        action.TargetId = unit.UnitId.ToString();
                        action.TargetName = unit.Name;
                        action.CompanyId = companyId;
                        action.Logs = JsonConvert.SerializeObject(unit);
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

        [HttpPut("showHide/{id}/{stt}")]
        public async Task<ActionResult> ShowHide(int id, int stt)
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
                using (var db = new IOITDataContext())
                {
                    Unit data = await db.Unit.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //db.Comment.Remove(data);
                        data.Status = (byte)stt;
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.UnitId > 0)
                            {
                                transaction.Commit();
                                //Create log
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Sửa cơ quan/tổ chức “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.UnitId.ToString();
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
                            if (!UnitExists(data.UnitId))
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

        // GET by Tree
        [HttpGet("GetByTree")]
        public IActionResult GetByTree([FromQuery] int[] arr, [FromQuery] int langId = -1)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            try
            {
                List<SmallUnitDTO> list = new List<SmallUnitDTO>();
                var query = "";
                //foreach (var type in arr)
                //{
                //    query += "TypeCategoryId=" + type + " OR ";
                //}

                var data = GetByTreeFunction(list, 0, 1, query, "", langId);
                def.data = data;
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
            catch (Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private List<SmallUnitDTO> GetByTreeFunction(List<SmallUnitDTO> list, int UnitParentId, int level, string query, string genealogy, int languageId)
        {
            var index = level + 1;
            var q = "";
            if (query != "" && UnitParentId == 0)
            {
                int lastIndexOf = query.LastIndexOf(" OR ");
                q = query.Substring(0, lastIndexOf);
                if (languageId > 0)
                    q = "UnitParentId=" + UnitParentId + " AND LanguageId = " + languageId + " AND Status!=99 AND (" + q + ")";
                else
                    q = "UnitParentId=" + UnitParentId + " AND Status!=99 AND (" + q + ")";
            }
            else
            {
                if (languageId > 0)
                    q = "UnitParentId=" + UnitParentId + " AND LanguageId = " + languageId + " AND Status!=99";
                else
                    q = "UnitParentId=" + UnitParentId + " AND Status!=99";
            }

            using (var db = new IOITDataContext())
            {
                var data = db.Unit.Where(q).Select(e => new SmallUnitDTO
                {
                    UnitId = e.UnitId,
                    Code = e.Code,
                    Name = e.Name,
                    UnitParentId = e.UnitParentId,
                    Status = e.Status,
                    Level = level,
                    Location = e.Location,
                    Check = false
                }).OrderBy(e => e.Location).ToList();

                foreach (SmallUnitDTO dt in data)
                {
                    String strg = genealogy;
                    strg += dt.UnitParentId.ToString() + "_";
                    dt.Genealogy = strg;
                    list.Add(dt);
                    if (dt.UnitId != dt.UnitParentId)
                    {
                        GetByTreeFunction(list, dt.UnitId, index, query, strg, languageId);
                    }
                }
            }

            return list;
        }


        #region Sắp xếp Cơ quan/tổ chức bằng cách kéo thả

        [HttpGet("GetUnitSort")]
        public async Task<IActionResult> GetUnitSort([FromQuery] string txtSearch, [FromQuery] int langId = -1,
            [FromQuery] int status = -1)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            try
            {
                var query = "";
                int cs = 1;
                int idx = 0;
                //foreach (var type in arr)
                //{
                //    query += "TypeCategoryId=" + type + " OR ";
                //}
                //int lastIndexOf = query.LastIndexOf(" OR ");
                //query = query.Substring(0, lastIndexOf);
                //query += ")";
                //if (langId > 0)
                //{
                //    if (query != "")
                //    {
                //        query += " AND LanguageId=" + langId;
                //    }
                //    else
                //    {
                //        query = "LanguageId=" + langId;
                //    }
                //}
                //if (status > 0)
                //{
                //    if (query != "")
                //    {
                //        query += " AND Status=" + status;
                //    }
                //    else
                //    {
                //        query += "Status=" + status;
                //    }
                //}
                if (txtSearch != null && txtSearch != "")
                {
                    //int lastIndexOf = query.LastIndexOf(" OR ");
                    //query = query.Substring(0, lastIndexOf);
                    query = "(Name.Contains(\"" + txtSearch + "\") OR Code.Contains(\"" + txtSearch + "\")" +
                        " OR ShortName.Contains(\"" + txtSearch + "\"))";
                    cs = 2;
                }

                var data = await GetUnitSortFunction(0, query, 0, "—", cs, idx, langId, status);
                def.data = data.categorySorts;
                def.metadata = data.Sum;
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
            catch (Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private async Task<FullUnitSort> GetUnitSortFunction(int UnitParentId, string query, int Level, string UnitParentName,
            int cs, int index, int languageId, int status)
        {
            var q = "";
            if (cs == 1)
            {
                if (query != "" && UnitParentId == 0)
                {
                    //int lastIndexOf = query.LastIndexOf(" OR ");
                    //q = query.Substring(0, lastIndexOf);
                    if (q != "")
                        q += "UnitParentId=" + UnitParentId + " AND Status!=99 AND (" + q + ")";
                    if (languageId > 0)
                        q += " AND LanguageId=" + languageId;
                    if (status > 0)
                        q += " AND Status=" + status;
                }
                else
                {
                    q = "UnitParentId=" + UnitParentId + " AND Status!=99";
                    if (languageId > 0)
                        q += " AND LanguageId=" + languageId;
                    if (status > 0)
                        q += " AND Status=" + status;
                }
            }
            else
            {
                if (query != "" && UnitParentId == 0)
                {
                    q = query + " AND Status!=99";
                }
                else
                {
                    q = "UnitParentId=" + UnitParentId + " AND Status!=99";
                }
                if (languageId > 0)
                    q += " AND LanguageId=" + languageId;
                if (status > 0)
                    q += " AND Status=" + status;
            }

            using (var db = new IOITDataContext())
            {
                FullUnitSort obj = new FullUnitSort();
                var data = await db.Unit.Where(q).Select(e => new UnitSort
                {
                    UnitId = e.UnitId,
                    LanguageId = e.LanguageId,
                    Code = e.Code,
                    Name = e.Name,
                    ShortName = e.ShortName,
                    Location = e.Location,
                    Level = Level,
                    UnitParentName = UnitParentName,
                    Image = e.Image,
                    Url = e.Url,
                    IsShow = e.Status == 1 ? true : false,
                    Descriptions = e.Description,
                    CreatedAt = e.CreatedAt,
                    //listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.CategoryId || a.TargetId2 == e.CategoryId)
                    //                && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY && a.Status != (int)Const.Status.DELETED).Select(a => new LanguageCategoryDT
                    //                {
                    //                    lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new LanguageDT
                    //                    {
                    //                        LanguageId = l.LanguageId,
                    //                        Name = l.Name,
                    //                        Flag = l.Flag
                    //                    }).FirstOrDefault(),
                    //                    category = db.Category.Where(l => (l.CategoryId == a.TargetId1 || l.CategoryId == a.TargetId2) && l.CategoryId != e.CategoryId).Select(l => new CategoryDTL
                    //                    {
                    //                        CategoryId = l.CategoryId,
                    //                        Name = l.Name,
                    //                        Url = l.Url
                    //                    }).FirstOrDefault(),
                    //                }).ToList(),
                    //language = db.Language.Where(a => a.LanguageId == e.LanguageId
                    //                 && a.Status != (int)Const.Status.DELETED).Select(a => new LanguageDT
                    //                 {
                    //                     LanguageId = a.LanguageId,
                    //                     Name = a.Name,
                    //                     Flag = a.Flag,
                    //                     Code = a.Code,
                    //                 }).FirstOrDefault(),

                }).OrderBy(e => e.Location).ToListAsync();

                List<UnitSort> listData = new List<UnitSort>();

                foreach (var item in data)
                {
                    //if (item.LanguageId == languageId)
                    //{
                    if (item.UnitId != item.UnitParentId)
                    {
                        index = index + 1;
                        var child = await GetUnitSortFunction(item.UnitId, query, Level + 1, item.Name, cs, index, languageId, status);
                        index = (int)child.Sum;
                        item.categorySorts = child.categorySorts;
                        listData.Add(item);
                    }
                    //}
                    //else
                    //{
                    //    //Nếu danh mục đó ko phải là danh mục có ngôn ngữ gốc thì check xem danh mục đó có map vs danh mục nào ko, 
                    //    //nếu không map thì cho hiện bình con nhà thường
                    //    var mapLang = await db.LanguageMapping.Where(e => e.LanguageId2 == item.LanguageId && e.TargetId2 == item.CategoryId).FirstOrDefaultAsync();
                    //    if (mapLang == null)
                    //    {
                    //        index = index + 1;
                    //        var child = await GetCategorySortFunction(item.CategoryId, query, Level + 1, item.Name, cs, index, languageId);
                    //        index = (int)child.Sum;
                    //        item.categorySorts = child.categorySorts;
                    //        listData.Add(item);
                    //    }
                    //}
                }

                //log.Error("Index:" + index);
                obj.categorySorts = listData;
                obj.Sum = index;
                return obj;
            }
        }

        [HttpPost("SaveUnitSort")]
        public async Task<IActionResult> SaveUnitSort([FromBody] List<UnitSort> data)
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

            if (data == null)
            {
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }

            try
            {
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        foreach (var item in data)
                        {
                            Unit unit = await db.Unit.Where(c => c.UnitId == item.UnitId && c.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                            if (unit != null)
                            {
                                unit.UnitParentId = item.UnitParentId != null ? (int)item.UnitParentId : 0;
                                unit.Location = item.Location;
                                db.Unit.Update(unit);
                            }
                        }

                        await db.SaveChangesAsync();
                        transaction.Commit();
                        def.meta = new Meta(200, "Sắp xếp thành công!");
                        def.data = "Success";
                        return Ok(def);
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

        #endregion

        private bool UnitExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Unit.Count(e => e.UnitId == id) > 0;
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
