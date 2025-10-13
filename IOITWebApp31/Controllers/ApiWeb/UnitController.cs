using IOITWebApp31.Models;
using IOITWebApp31.Models.Common;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("unit", "unit");
        private static string functionCode = "CQTC";
        private readonly IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;

        public UnitController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        // GET: api/Unit
        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
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

        [Authorize]
        [HttpGet("listUnit")]
        public IActionResult listUnits()
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            string listUnits = identity.Claims.Where(c => c.Type == "ListUnits").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    string[] listU = listUnits.Split('-');
                    List<SmallUnitDTO> units = new List<SmallUnitDTO>();
                    def.data = listUnit(units, 0, 0, db, listU);
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private List<SmallUnitDTO> listUnit(List<SmallUnitDTO> dt, int UnitId, int level,
            IOITDataContext db, string[] listU)
        {
            var index = level + 1;
            var data = db.Unit.Where(e => e.UnitParentId == UnitId && e.Status != (int)Const.Status.DELETED).ToList();
            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    SmallUnitDTO function = new SmallUnitDTO();
                    function.UnitId = item.UnitId;
                    function.Code = item.Code;
                    function.Name = item.Name + " (" + item.Code + ")";
                    function.Level = level;
                    var checkU = listU.Where(e => e.Equals(item.UnitId.ToString())).FirstOrDefault();
                    if (checkU != null)
                        dt.Add(function);
                    if (item.UnitId != item.UnitParentId)
                    {
                        listUnit(dt, item.UnitId, index, db, listU);
                    }

                }
            }
            return dt;
        }

        [HttpGet("listUnitPublish")]
        public IActionResult listUnitPublishs()
        {
            DefaultResponse def = new DefaultResponse();

            try
            {
                using (var db = new IOITDataContext())
                {
                    List<SmallUnitDTO> units = new List<SmallUnitDTO>();
                    def.data = listUnitPublish(units, 0, 0, db);
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private List<SmallUnitDTO> listUnitPublish(List<SmallUnitDTO> dt, int UnitId, int level,
            IOITDataContext db)
        {
            var index = level + 1;
            var data = db.Unit.Where(e => e.UnitParentId == UnitId && e.Status != (int)Const.Status.DELETED).ToList();
            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    SmallUnitDTO function = new SmallUnitDTO();
                    function.UnitId = item.UnitId;
                    function.Code = item.Code;
                    function.Name = item.Name + " (" + item.Code + ")";
                    function.Level = level;
                    dt.Add(function);
                    if (item.UnitId != item.UnitParentId)
                    {
                        listUnitPublish(dt, item.UnitId, index, db);
                    }

                }
            }
            return dt;
        }

        //Phần quản trị
        [Authorize]
        [HttpPost("GetByPagePost")]
        public async Task<IActionResult> GetByPagePost([FromBody] FilterReport paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            //int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            string listUnits = identity.Claims.Where(c => c.Type == "ListUnits").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "Bạn không có quyền xem danh sách cơ quan/tổ chức");
                return Ok(def);
            }
            if (paging != null)
            {
                try
                {
                    using (var db = new IOITDataContext())
                    {

                        def.meta = new Meta(200, "Success");
                        var dateStart = new DateTime(2000, 1, 1);
                        var dateEnd = DateTime.Now;
                        if (paging.DateStart != null)
                            dateStart = new DateTime(paging.DateStart.Value.Year, paging.DateStart.Value.Month, paging.DateStart.Value.Day, 0, 0, 0);
                        if (paging.DateEnd != null)
                            dateEnd = new DateTime(paging.DateEnd.Value.Year, paging.DateEnd.Value.Month, paging.DateEnd.Value.Day, 23, 59, 59);

                        string[] listU = listUnits.Split('-');

                        IQueryable<UnitDTO> data = db.Unit.Where(c =>
                        c.Status != (int)Const.Status.DELETED
                        && c.CreatedAt >= dateStart && c.CreatedAt <= dateEnd
                        && listU.Contains(c.UnitId.ToString())).Select(e => new UnitDTO
                        {
                            UnitId = e.UnitId,
                            Code = e.Code,
                            Name = e.Name,
                            ShortName = e.ShortName,
                            NameEn = e.NameEn,
                            Email = e.Email,
                            Phone = e.Phone,
                            Fax = e.Fax,
                            Website = e.Website,
                            IdNumber = e.IdNumber,
                            DateNumber = e.DateNumber,
                            AddressNumber = e.AddressNumber,
                            UnitParentId = e.UnitParentId,
                            Description = e.Description,
                            Contents = e.Contents,
                            Url = e.Url,
                            Image = e.Image,
                            Icon = e.Icon,
                            IconFa = e.IconFa,
                            IconText = e.IconText,
                            Location = e.Location,
                            Type = e.Type,
                            ProvinceId = e.ProvinceId,
                            DistrictId = e.DistrictId,
                            WardId = e.WardId,
                            Address = e.Address,
                            AdminId = e.AdminId,
                            EmailAdmin = e.EmailAdmin,
                            NameAdmin = e.NameAdmin,
                            LanguageId = e.LanguageId,
                            WebsiteId = e.WebsiteId,
                            CompanyId = e.CompanyId,
                            MetaTitle = e.MetaTitle,
                            MetaKeyword = e.MetaKeyword,
                            MetaDescription = e.MetaDescription,
                            CreatedId = e.CreatedId,
                            UpdatedId = e.UpdatedId,
                            CreatedAt = e.CreatedAt,
                            UpdatedAt = e.UpdatedAt,
                            Status = e.Status
                        });

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
                            if (paging.order_by != null && paging.order_by != "")
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
                            if (paging.order_by != null && paging.order_by != "")
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
                        {
                            //var listDatas = data.ToList();
                            //foreach (var item in listDatas)
                            //{
                            //var unit = db.Unit.Where(e => e.UnitId == item.UnitId).FirstOrDefault();
                            //item.UnitName = unit != null ? unit.Name : "";
                            //var listMap = await db.CustomerMapping.Where(e => e.CustomerId == item.CustomerId
                            //&& e.Status != (int)Const.Status.DELETED).ToListAsync();
                            //item.ListRoles = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_ROLE)
                            //    .Select(e => (int)e.TargetId).ToList();
                            //item.ListResearchArea = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION)
                            //    .Select(e => (int)e.TargetId).ToList();
                            //item.ListUnitManager = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT)
                            //    .Select(e => (int)e.TargetId).ToList();
                            //}
                            def.data = await data.ToListAsync();
                        }

                        return Ok(def);
                    }
                }
                catch (Exception e)
                {
                    log.Error("Exception:" + e);
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        [Authorize]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetUnit(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "Bạn không có quyền xem chi tiết cơ quan/tổ chức");
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

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnit(int id, UnitDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
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
                                action.CompanyId = 1;
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostUnit(UnitDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            string listUnits = identity.Claims.Where(c => c.Type == "ListUnits").Select(c => c.Value).SingleOrDefault();
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

                                CustomerLogin userLogin = db.Customer.Where(e => e.CustomerId == userId && e.Status != (int)Const.Status.DELETED).Select(e => new CustomerLogin()
                                {
                                    CustomerId = e.CustomerId,
                                    Username = e.Username,
                                    Email = e.Email,
                                    FullName = e.FullName,
                                    Avata = e.Avata,
                                    Address = e.Address,
                                    Password = e.Password,
                                    PhomeNumber = e.Phone,
                                    Status = e.Status,
                                    Type = e.Type,
                                    RoleId = e.RoleId,
                                    Sex = e.Sex,
                                    IsEmailConfirm = e.IsEmailConfirm,
                                    CreatedAt = e.CreatedAt
                                }).FirstOrDefault();

                                listUnits += "-" + data.UnitId;

                                var claims = new List<Claim>
                                {
                                    new Claim(JwtRegisteredClaimNames.Email, userLogin.Email),
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    new Claim(ClaimTypes.NameIdentifier, userLogin.CustomerId.ToString()),
                                    new Claim(ClaimTypes.Name, userLogin.FullName),
                                        new Claim("CustomerId", userLogin.CustomerId != null ? userLogin.CustomerId.ToString() : ""),
                                        new Claim("ListUnits", listUnits),
                                        //new Claim("RoleMax", userLogin.roleMax != null ? userLogin.roleMax.ToString() : ""),
                                        //new Claim("RoleLevel", userLogin.roleLevel != null ? userLogin.roleLevel.ToString() : ""),
                                        new Claim("AccessKey", access_key != null ? access_key : ""),
                                        //new Claim("LanguageId", access_key != null ? languageId.ToString() : ""),
                                        new Claim("Type", userLogin.Type != null ? userLogin.Type.ToString() : ""),
                                };
                                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:JwtKey"]));
                                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                                var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["AppSettings:JwtExpireDays"]));
                                var token = new JwtSecurityToken(
                                    _configuration["AppSettings:JwtIssuer"],
                                    _configuration["AppSettings:JwtIssuer"],
                                    claims,
                                    expires: expires,
                                    signingCredentials: creds
                                );
                                userLogin.access_token = new JwtSecurityTokenHandler().WriteToken(token);
                                //HttpContext.Session.SetInt32("CustomerId", userLogin.CustomerId);
                                //HttpContext.Session.SetString("access_token", userLogin.access_token);
                                //var option = new CookieOptions();
                                //option.Expires = DateTime.Now.AddMinutes(600);
                                //Response.Cookies.Append("CustomerId", userLogin.CustomerId.ToString(), option);
                                InitSession(userLogin);

                                //create log
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Tạo Cơ quan/tổ chức " + data.Name;
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.UnitId.ToString();
                                action.TargetName = data.Name;
                                action.CompanyId = 1;
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

        private void InitSession(CustomerLogin userLogin)
        {
            HttpContext.Session.SetInt32("CustomerId", userLogin.CustomerId);
            if (userLogin.Email != null)
            {
                HttpContext.Session.SetString("CustomerEmail", userLogin.Email);
            }
            if (userLogin.FullName != null)
            {
                HttpContext.Session.SetString("CustomerFullName", userLogin.FullName);
            }
            if (userLogin.Avata != null)
            {
                HttpContext.Session.SetString("CustomerAvata", userLogin.Avata);
            }
            if (userLogin.Address != null)
            {
                HttpContext.Session.SetString("CustomerAddress", userLogin.Address);
            }
            if (userLogin.Password != null)
            {
                HttpContext.Session.SetString("CustomerPassword", userLogin.Password);
            }
            if (userLogin.PhomeNumber != null)
            {
                HttpContext.Session.SetString("CustomerPhoneNumber", userLogin.PhomeNumber);
            }
            if (userLogin.access_token != null)
            {
                HttpContext.Session.SetString("access_token", userLogin.access_token);
            }
            if (userLogin.Sex != null)
            {
                HttpContext.Session.SetString("CustomerSex", userLogin.Sex + "");
            }
            if (userLogin.Status != null)
            {
                HttpContext.Session.SetString("Status", userLogin.Status + "");
            }
            if (userLogin.CreatedAt != null)
            {
                HttpContext.Session.SetString("CreatedAt", userLogin.CreatedAt + "");
            }
            if (userLogin.access_key != null)
            {
                HttpContext.Session.SetString("access_key", userLogin.access_key + "");
            }
            if (userLogin.listMenus != null)
            {
                HttpContext.Session.SetObject("listMenus", userLogin.listMenus);
            }
            if (userLogin.listUnits != null)
            {
                HttpContext.Session.SetObject("listUnits", userLogin.listUnits);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
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
                                action.CompanyId = 1;
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
