using IOITWebApp31.Models;
using IOITWebApp31.Models.Common;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class CustomerController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("customer", "customer");
        private static string functionCode = "QLND";
        private IHostingEnvironment _hostingEnvironment;

        public CustomerController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }


        // GET: api/Customer
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
                    IQueryable<CustomerDTO> data = db.Customer.Where(c => c.Status != (int)Const.Status.DELETED).Select(e =>
                    new CustomerDTO
                    {
                        CustomerId = e.CustomerId,
                        Username = e.Username,
                        Password = e.Password,
                        FullName = e.FullName,
                        Email = e.Email,
                        Phone = e.Phone,
                        Avata = e.Avata,
                        Sex = e.Sex,
                        Birthday = e.Birthday,
                        Address = e.Address,
                        Note = e.Note,
                        KeyRandom = e.KeyRandom,
                        IsEmailConfirm = e.IsEmailConfirm,
                        IsSentEmailConfirm = e.IsSentEmailConfirm,
                        IsPhoneConfirm = e.IsPhoneConfirm,
                        Type = e.Type,
                        UnitId = e.UnitId,
                        CountryId = e.CountryId,
                        TypeId = e.TypeId,
                        IdNumber = e.IdNumber,
                        DateNumber = e.DateNumber,
                        AddressNumber = e.AddressNumber,
                        PositionId = e.PositionId,
                        AcademicRankId = e.AcademicRankId,
                        DegreeId = e.DegreeId,
                        WebsiteId = e.WebsiteId,
                        CompanyId = e.CompanyId,
                        TypeThirdId = e.TypeThirdId,
                        UserId = e.UserId,
                        LastLoginAt = e.LastLoginAt,
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
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                        else
                        {
                            data = data.OrderBy("CustomerId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("CustomerId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select).ToDynamicList();
                    }
                    else
                    {
                        var listDatas = data.ToList();
                        foreach (var item in listDatas)
                        {
                            var unit = db.Unit.Where(e => e.UnitId == item.UnitId).FirstOrDefault();
                            item.UnitName = unit != null ? unit.Name : "";
                            var listMap = await db.CustomerMapping.Where(e => e.CustomerId == item.CustomerId
                            && e.Status != (int)Const.Status.DELETED).ToListAsync();
                            item.ListRoles = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_ROLE)
                                .Select(e => (int)e.TargetId).ToList();
                            item.ListResearchArea = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION)
                                .Select(e => (int)e.TargetId).ToList();
                            item.ListUnitManager = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT)
                                .Select(e => (int)e.TargetId).ToList();
                        }
                        def.data = listDatas;
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

        [HttpPost("GetByPagePost")]
        public async Task<IActionResult> GetByPagePost([FromBody] FilterReport paging)
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
            if (paging != null)
            {
                try
                {
                    using (var db = new IOITDataContext())
                    {

                        //string cat = "CategoryId";
                        def.meta = new Meta(200, "Success");
                        var dateStart = new DateTime(2000, 1, 1);
                        var dateEnd = DateTime.Now;
                        if (paging.DateStart != null)
                            dateStart = new DateTime(paging.DateStart.Value.Year, paging.DateStart.Value.Month, paging.DateStart.Value.Day, 0, 0, 0);
                        if (paging.DateEnd != null)
                            dateEnd = new DateTime(paging.DateEnd.Value.Year, paging.DateEnd.Value.Month, paging.DateEnd.Value.Day, 23, 59, 59);

                        IQueryable<CustomerDTO> data = db.Customer.Where(c =>
                        c.Status != (int)Const.Status.DELETED
                        && c.CreatedAt >= dateStart && c.CreatedAt <= dateEnd).Select(e => new CustomerDTO
                        {
                            CustomerId = e.CustomerId,
                            Username = e.Username,
                            Password = e.Password,
                            FullName = e.FullName,
                            Email = e.Email,
                            Phone = e.Phone,
                            Avata = e.Avata,
                            Sex = e.Sex,
                            Birthday = e.Birthday,
                            Address = e.Address,
                            Note = e.Note,
                            KeyRandom = e.KeyRandom,
                            IsEmailConfirm = e.IsEmailConfirm,
                            IsSentEmailConfirm = e.IsSentEmailConfirm,
                            IsPhoneConfirm = e.IsPhoneConfirm,
                            Type = e.Type,
                            UnitId = e.UnitId,
                            CountryId = e.CountryId,
                            TypeId = e.TypeId,
                            IdNumber = e.IdNumber,
                            DateNumber = e.DateNumber,
                            AddressNumber = e.AddressNumber,
                            PositionId = e.PositionId,
                            AcademicRankId = e.AcademicRankId,
                            DegreeId = e.DegreeId,
                            RoleId = e.RoleId,
                            WebsiteId = e.WebsiteId,
                            CompanyId = e.CompanyId,
                            TypeThirdId = e.TypeThirdId,
                            LastLoginAt = e.LastLoginAt,
                            UserId = e.UserId,
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
                        metaDataDT.Lock = data.Where(e => e.Status == 98).Count();

                        def.metadata = metaDataDT;

                        if (paging.page_size > 0)
                        {
                            if (paging.order_by != null && paging.order_by != "")
                            {
                                data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                            }
                            else
                            {
                                data = data.OrderBy("CustomerId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                                data = data.OrderBy("CustomerId desc");
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
                            var listDatas = data.ToList();
                            foreach (var item in listDatas)
                            {
                                var unit = db.Unit.Where(e => e.UnitId == item.UnitId).FirstOrDefault();
                                item.UnitName = unit != null ? unit.Name : "";
                                var listMap = await db.CustomerMapping.Where(e => e.CustomerId == item.CustomerId
                                && e.Status != (int)Const.Status.DELETED).ToListAsync();
                                item.ListRoles = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_ROLE)
                                    .Select(e => (int)e.TargetId).ToList();
                                item.ListResearchArea = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION)
                                    .Select(e => (int)e.TargetId).ToList();
                                item.ListUnitManager = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT)
                                    .Select(e => (int)e.TargetId).ToList();
                            }
                            def.data = listDatas;
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

        // GET: api/Customer/5
        [HttpGet("id")]
        public async Task<IActionResult> GetCustomer(int id)
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
                    Customer data = await db.Customer.FindAsync(id);

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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, [FromBody] CustomerDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }

                if (data.Email == null || data.Email == "")
                {
                    def.meta = new Meta(211, "Email không được để trống!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    Customer customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (customer == null)
                    {
                        def.meta = new Meta(212, "Người dùng không tồn tại!");
                        return Ok(def);
                    }
                    Customer checkEmail = db.Customer.Where(c => c.Email == data.Email.ToLower().Trim() && c.CustomerId != id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkEmail != null)
                    {
                        def.meta = new Meta(212, "Email đã tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        customer.Username = data.Email.ToLower().Trim();
                        customer.FullName = data.FullName;
                        customer.Email = data.Email.ToLower().Trim();
                        //customer.Password = "123456";
                        //customer.KeyRandom = Utils.RandomString(8);
                        //customer.Password = Utils.GetMD5Hash(customer.Password + customer.KeyRandom);
                        customer.Phone = data.Phone;
                        customer.Avata = data.Avata;
                        customer.Sex = data.Sex;
                        customer.Birthday = data.Birthday;
                        customer.Address = data.Address;
                        customer.Note = data.Note;
                        //customer.IsEmailConfirm = true;
                        //customer.IsSentEmailConfirm = true;
                        //customer.IsPhoneConfirm = true;
                        customer.Type = data.Type;
                        customer.UnitId = data.UnitId;
                        customer.CountryId = data.CountryId;
                        customer.TypeId = data.TypeId;
                        customer.IdNumber = data.IdNumber;
                        customer.DateNumber = data.DateNumber;
                        customer.AddressNumber = data.AddressNumber;
                        customer.PositionId = data.PositionId;
                        customer.AcademicRankId = data.AcademicRankId;
                        customer.DegreeId = data.DegreeId;
                        customer.RoleId = data.RoleId;
                        //customer.WebsiteId = data.WebsiteId != null ? data.WebsiteId : 1;
                        //customer.CompanyId = data.CompanyId != null ? data.CompanyId : 1;
                        //customer.TypeThirdId = (int)Const.TypeThird.CUSTOMER_ADMIN;
                        //customer.LastLoginAt = DateTime.Now;
                        customer.UserId = userId;
                        customer.UpdatedAt = DateTime.Now;
                        customer.Status = (int)Const.Status.NORMAL;
                        db.Entry(customer).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.CustomerId > 0)
                            {
                                //Nhóm quyền
                                List<CustomerMapping> customerRoles = db.CustomerMapping.Where(e => e.CustomerId == data.CustomerId
                                && e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_ROLE
                                && e.Status != (int)Const.Status.DELETED).ToList();
                                if (customerRoles != null)
                                {
                                    foreach (var item in data.ListRoles)
                                    {
                                        CustomerMapping customerRole = customerRoles.Where(cf => cf.TargetId == item).FirstOrDefault();
                                        if (customerRole != null)
                                        {
                                            customerRole.TargetId = item;
                                            customerRole.UpdatedId = userId;
                                            customerRole.UpdatedAt = DateTime.Now;
                                            db.CustomerMapping.Update(customerRole);

                                            customerRoles.Remove(customerRole);
                                        }
                                        else
                                        {
                                            CustomerMapping mapping = new CustomerMapping();
                                            mapping.CustomerId = data.CustomerId;
                                            mapping.TargetId = item;
                                            mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_ROLE;
                                            mapping.CreatedId = userId;
                                            mapping.UpdatedId = userId;
                                            mapping.CreatedAt = DateTime.Now;
                                            mapping.UpdatedAt = DateTime.Now;
                                            mapping.Status = (int)Const.Status.NORMAL;
                                            await db.CustomerMapping.AddAsync(mapping);
                                        }
                                    }
                                    customerRoles.ForEach(x => x.Status = (int)Const.Status.DELETED);
                                    await db.SaveChangesAsync();
                                }
                                //Lĩnh vực nghiên cứu
                                List<CustomerMapping> customerApplications = db.CustomerMapping.Where(e => e.CustomerId == data.CustomerId
                                && e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION
                                && e.Status != (int)Const.Status.DELETED).ToList();
                                if (customerApplications != null)
                                {
                                    foreach (var item in data.ListResearchArea)
                                    {
                                        CustomerMapping customerApplication = customerApplications.Where(cf => cf.TargetId == item).FirstOrDefault();
                                        if (customerApplication != null)
                                        {
                                            customerApplication.TargetId = item;
                                            customerApplication.UpdatedId = userId;
                                            customerApplication.UpdatedAt = DateTime.Now;
                                            db.CustomerMapping.Update(customerApplication);

                                            customerApplications.Remove(customerApplication);
                                        }
                                        else
                                        {
                                            CustomerMapping mapping = new CustomerMapping();
                                            mapping.CustomerId = data.CustomerId;
                                            mapping.TargetId = item;
                                            mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION;
                                            mapping.CreatedId = userId;
                                            mapping.UpdatedId = userId;
                                            mapping.CreatedAt = DateTime.Now;
                                            mapping.UpdatedAt = DateTime.Now;
                                            mapping.Status = (int)Const.Status.NORMAL;
                                            await db.CustomerMapping.AddAsync(mapping);
                                        }
                                    }
                                    customerApplications.ForEach(x => x.Status = (int)Const.Status.DELETED);
                                }
                                //Cơ quan tổ chức quản trị
                                List<CustomerMapping> customerUnits = db.CustomerMapping.Where(e => e.CustomerId == data.CustomerId
                                && e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                                && e.Status != (int)Const.Status.DELETED).ToList();
                                if (customerUnits != null)
                                {
                                    foreach (var item in data.ListUnitManager)
                                    {
                                        CustomerMapping customerUnit = customerUnits.Where(cf => cf.TargetId == item).FirstOrDefault();
                                        if (customerUnit != null)
                                        {
                                            customerUnit.TargetId = item;
                                            customerUnit.UpdatedId = userId;
                                            customerUnit.UpdatedAt = DateTime.Now;
                                            db.CustomerMapping.Update(customerUnit);

                                            customerUnits.Remove(customerUnit);
                                        }
                                        else
                                        {
                                            CustomerMapping mapping = new CustomerMapping();
                                            mapping.CustomerId = data.CustomerId;
                                            mapping.TargetId = item;
                                            mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_UNIT;
                                            mapping.CreatedId = userId;
                                            mapping.UpdatedId = userId;
                                            mapping.CreatedAt = DateTime.Now;
                                            mapping.UpdatedAt = DateTime.Now;
                                            mapping.Status = (int)Const.Status.NORMAL;
                                            await db.CustomerMapping.AddAsync(mapping);
                                        }
                                    }
                                    customerUnits.ForEach(x => x.Status = (int)Const.Status.DELETED);
                                }
                                await db.SaveChangesAsync();
                                transaction.Commit();
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
                            if (!CustomerExists(data.CustomerId))
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

        [HttpPost]
        public async Task<IActionResult> PostCustomer(CustomerDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                if (data.Email == null || data.Email == "")
                {
                    def.meta = new Meta(211, "Email không được để trống!");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    Customer checkEmail = db.Customer.Where(c => c.Email == data.Email.ToLower().Trim() && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkEmail != null)
                    {
                        def.meta = new Meta(212, "Email đã tồn tại!");
                        return Ok(def);
                    }
                    Config config = db.Config.Where(c => c.ConpanyId == Const.WEBSITEID).FirstOrDefault();
                    if (config == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy cấu hình để gửi Email xác nhận tài khoản!");
                        return Ok(def);
                    }

                    if (config.EmailSender == null || config.EmailSender == "" || config.EmailHost == null || config.EmailHost == "" || config.EmailUserName == null || config.EmailUserName == "" || config.EmailPasswordHash == null || config.EmailPasswordHash == "" || config.EmailPort == null)
                    {
                        def.meta = new Meta(404, "Thông tin cấu hình để gửi Email xác nhận tài khoản không chính xác!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Customer customer = new Customer();
                        customer.Username = data.Email.ToLower().Trim();
                        customer.FullName = data.FullName;
                        customer.Email = data.Email.ToLower().Trim();
                        customer.Password = "Open@123";
                        customer.KeyRandom = Utils.RandomString(8);
                        customer.Password = Utils.GetMD5Hash(customer.Password + customer.KeyRandom);
                        customer.Phone = data.Phone;
                        customer.Avata = data.Avata;
                        customer.Sex = data.Sex;
                        customer.Birthday = data.Birthday;
                        customer.Address = data.Address;
                        customer.Note = data.Note;
                        customer.IsEmailConfirm = true;
                        customer.IsSentEmailConfirm = true;
                        customer.IsPhoneConfirm = true;
                        customer.Type = data.Type;
                        customer.UnitId = data.UnitId;
                        customer.CountryId = data.CountryId;
                        customer.TypeId = data.TypeId;
                        customer.IdNumber = data.IdNumber;
                        customer.DateNumber = data.DateNumber;
                        customer.AddressNumber = data.AddressNumber;
                        customer.PositionId = data.PositionId;
                        customer.AcademicRankId = data.AcademicRankId;
                        customer.DegreeId = data.DegreeId;
                        customer.RoleId = data.RoleId;
                        customer.WebsiteId = data.WebsiteId != null ? data.WebsiteId : 1;
                        customer.CompanyId = data.CompanyId != null ? data.CompanyId : 1;
                        customer.TypeThirdId = (int)Const.TypeThird.CUSTOMER_ADMIN;
                        customer.LastLoginAt = DateTime.Now;
                        customer.UserId = userId;
                        customer.CreatedAt = DateTime.Now;
                        customer.UpdatedAt = DateTime.Now;
                        customer.Status = data.Status != null ? data.Status : (int)Const.Status.NORMAL;
                        await db.Customer.AddAsync(customer);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.CustomerId = customer.CustomerId;
                            if (data.CustomerId > 0)
                            {
                                //Nhóm quyền
                                if (data.ListRoles != null)
                                {
                                    foreach (var item in data.ListRoles)
                                    {
                                        CustomerMapping mapping = new CustomerMapping();
                                        mapping.CustomerId = data.CustomerId;
                                        mapping.TargetId = item;
                                        mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_ROLE;
                                        mapping.CreatedId = userId;
                                        mapping.UpdatedId = userId;
                                        mapping.CreatedAt = DateTime.Now;
                                        mapping.UpdatedAt = DateTime.Now;
                                        mapping.Status = (int)Const.Status.NORMAL;
                                        await db.CustomerMapping.AddAsync(mapping);
                                    }
                                }
                                //Lĩnh vực nghiên cứu
                                if (data.ListResearchArea != null)
                                {
                                    foreach (var item in data.ListResearchArea)
                                    {
                                        CustomerMapping mapping = new CustomerMapping();
                                        mapping.CustomerId = data.CustomerId;
                                        mapping.TargetId = item;
                                        mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION;
                                        mapping.CreatedId = userId;
                                        mapping.UpdatedId = userId;
                                        mapping.CreatedAt = DateTime.Now;
                                        mapping.UpdatedAt = DateTime.Now;
                                        mapping.Status = (int)Const.Status.NORMAL;
                                        await db.CustomerMapping.AddAsync(mapping);
                                    }
                                }
                                //Cơ quan tổ chức quản trị
                                if (data.ListUnitManager != null)
                                {
                                    foreach (var item in data.ListUnitManager)
                                    {
                                        CustomerMapping mapping = new CustomerMapping();
                                        mapping.CustomerId = data.CustomerId;
                                        mapping.TargetId = item;
                                        mapping.TargetType = (int)Const.TargetCustomerMapping.CUSTOMER_UNIT;
                                        mapping.CreatedId = userId;
                                        mapping.UpdatedId = userId;
                                        mapping.CreatedAt = DateTime.Now;
                                        mapping.UpdatedAt = DateTime.Now;
                                        mapping.Status = (int)Const.Status.NORMAL;
                                        await db.CustomerMapping.AddAsync(mapping);
                                    }
                                }

                                //Gửi Email xác nhận
                                if (customer.Email != null && customer.Email != "")
                                {
                                    String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/mail-confirm-resgister-customer.html");

                                    string key = Utils.GetMD5Hash(customer.KeyRandom + customer.Password);
                                    string linkConfirm = "'" + config.Website + "thiet-lap-mat-khau-" + key + "-" + customer.CustomerId + "'";
                                    //linkConfirm = "<a href=\"" + linkConfirm + "\">Link xác nhận.</a>";
                                    data.KeyRandom = "/thiet-lap-mat-khau-" + key + "-" + customer.CustomerId;
                                    sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                        config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                        config.Website, customer.FullName, linkConfirm, customer.KeyRandom);
                                    string subject = config.EmailSender + " - Xác thực tài khoản";
                                    //EmailService emailService = new EmailService();
                                    customer.IsSentEmailConfirm = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, customer.Email, subject, sBody);
                                }
                                db.Entry(customer).State = EntityState.Modified;

                                await db.SaveChangesAsync();

                                transaction.Commit();
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
                            if (CustomerExists(data.CustomerId))
                            {
                                def.meta = new Meta(211, "Khách hàng đã tồn tại!");
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
                def.meta = new Meta(500, "Lỗi server!");
                return Ok(def);
            }
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            using (var db = new IOITDataContext())
            {
                Customer data = await db.Customer.FindAsync(id);
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
                    db.Customer.Update(data);
                    try
                    {
                        //delete customer
                        var user = await db.User.Where(e => e.UserMapId == data.CustomerId && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                        if (user != null)
                        {
                            if (user.UserId != 1)
                            {
                                user.Status = (int)Const.Status.DELETED;
                                user.UpdatedAt = DateTime.Now;
                                db.Entry(user).State = EntityState.Modified;
                            }
                            else
                            {
                                def.meta = new Meta(222, "Bạn không có quyền xóa người dùng này!");
                                return Ok(def);
                            }
                        }
                        await db.SaveChangesAsync();

                        if (data.CustomerId > 0)
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
                        if (!CustomerExists(data.CustomerId))
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

        //API xóa danh sách khách hàng
        [HttpPut("deletes")]
        public async Task<IActionResult> DeleteCustomers([FromBody] int[] data)
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
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            Customer customer = await db.Customer.FindAsync(data[i]);

                            if (customer == null)
                            {
                                continue;
                            }

                            customer.UserId = userId;
                            customer.UpdatedAt = DateTime.Now;
                            customer.Status = (int)Const.Status.DELETED;
                            db.Entry(customer).State = EntityState.Modified;
                            //delete customer
                            var user = await db.User.Where(e => e.UserMapId == customer.CustomerId && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                            if (user != null)
                            {
                                if (user.UserId != 1)
                                {
                                    user.Status = (int)Const.Status.DELETED;
                                    user.UpdatedAt = DateTime.Now;
                                    db.Entry(user).State = EntityState.Modified;
                                }
                                else
                                {
                                    def.meta = new Meta(222, "Bạn không có quyền xóa người dùng này!");
                                    return Ok(def);
                                }
                            }
                        }
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

        //API đổi mật khẩu khách hàng và gửi email tới cho khách hàng
        [HttpPost("ResetPassword/{CustomerId}")]
        public async Task<IActionResult> ResetPasswordCustomer([FromBody] ResetPasswordCustomerDTO data, int CustomerId)
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
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }

                if (data.PasswordInit == null || data.PasswordInit == "")
                {
                    def.meta = new Meta(211, "Password khởi tạo không được để trống!");
                    return Ok(def);
                }

                if (data.Password == null || data.Password == "")
                {
                    def.meta = new Meta(211, "Password không được để trống!");
                    return Ok(def);
                }

                if (data.ConfirmPassword == null || data.ConfirmPassword == "")
                {
                    def.meta = new Meta(211, "Password xác nhận không được để trống!");
                    return Ok(def);
                }

                if (data.ConfirmPassword != data.Password)
                {
                    def.meta = new Meta(211, "Password xác nhận không giống với mật khẩu!");
                    return Ok(def);
                }
                if (!Security.IsPasswordValid(data.Password))
                {
                    def.meta = new Meta(211, "Mật khẩu phải đảm bảo 8 ký tự bao gồm chữ hoa, chữ thường và ít nhất 1 ký tự đặc biệt!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    Config config = db.Config.Where(c => c.ConpanyId == Const.WEBSITEID).FirstOrDefault();
                    if (config == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy cấu hình để gửi Email xác nhận đăng ký!");
                        return Ok(def);
                    }

                    Customer customer = db.Customer.Where(c => c.CustomerId == CustomerId && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (customer == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy khách hàng!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        customer.Password = Utils.GetMD5Hash(data.Password + customer.KeyRandom);
                        customer.UserId = userId;
                        customer.UpdatedAt = DateTime.Now;
                        db.Customer.Update(customer);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (customer.CustomerId > 0)
                            {
                                String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/mail-reset-password.html");
                                sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                    config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                    config.Website, customer.FullName, data.PasswordInit);
                                string subject = config.EmailSender + " - Cấp lại mật khẩu";
                                bool SendEmail = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, customer.Email, subject, sBody);
                                if (SendEmail == true)
                                {
                                    transaction.Commit();
                                }
                                else
                                {
                                    def.meta = new Meta(218, "Lỗi gửi mail!");
                                    return Ok(def);
                                }
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Reset mật khẩu thành công. Đã gửi mail mật khẩu tới email khách hàng!");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (!CustomerExists(customer.CustomerId))
                            {
                                def.meta = new Meta(211, "Không tìm thấy Khách hàng!");
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
                def.meta = new Meta(500, "Lỗi server!");
                return Ok(def);
            }
        }

        [HttpPut("lockUser/{id}/{k}")]
        public async Task<ActionResult> LockUser(int id, byte k)
        {
            DefaultResponse def = new DefaultResponse();
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

                using (var db = new IOITDataContext())
                {
                    Customer user = await db.Customer.FindAsync(id);
                    if (user == null)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }
                    if (user.CustomerId != id)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        user.UpdatedAt = DateTime.Now;
                        user.UserId = userId;
                        user.Status = k;
                        db.Entry(user).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();
                            if (user.CustomerId > 0)
                            {
                                transaction.Commit();

                                //create action
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Thay đổi trạng thái tài khoản người dùng";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = user.CustomerId + "";
                                action.TargetName = "Thay đổi trạng thái tài khoản " + k;
                                action.Logs = "Thay đổi trạng thái tài khoản “" + user.FullName + "” - " + k;
                                action.Time = 0;
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserId = id;
                                action.UserPushId = userId;
                                action.Status = (int)Const.Status.NORMAL;
                                db.Action.Add(action);
                                await db.SaveChangesAsync();

                                //    //push action firebase
                                //    Models.Data.Firebase.pushAction(action);

                                //    //push user firebase
                                //    var tasks = new[]
                                //    {
                                //    Task.Run(() => Models.Data.Firebase.updateUser(user))
                                //};
                            }
                            else
                                transaction.Rollback();
                            def.meta = new Meta(200, "Success");
                            return Ok(def);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateConcurrencyException:" + e);
                            if (!CustomerExists(id))
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
            catch (Exception e)
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private bool CustomerExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Customer.Count(e => e.CustomerId == id) > 0;
            }
        }


        // haohv 
        [HttpPost("GetAllStudent")]
        public async Task<IActionResult> GetAllStudent([FromBody] FilterReport paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                try
                {
                    using (var db = new IOITDataContext())
                    {

                        //string cat = "CategoryId";
                        def.meta = new Meta(200, "Success");
                        IQueryable<CustomerDTO> data = db.Customer.Where(c =>
                        c.Status != (int)Const.Status.DELETED
                        && c.StudentCode != null && c.StudentCode != "").Select(e => new CustomerDTO
                        {
                            CustomerId = e.CustomerId,
                            Username = e.Username,
                            Password = e.Password,
                            FullName = e.FullName,
                            Email = e.Email,
                            Phone = e.Phone,
                            Avata = e.Avata,
                            Sex = e.Sex,
                            Birthday = e.Birthday,
                            Address = e.Address,
                            Note = e.Note,
                            KeyRandom = e.KeyRandom,
                            IsEmailConfirm = e.IsEmailConfirm,
                            IsSentEmailConfirm = e.IsSentEmailConfirm,
                            IsPhoneConfirm = e.IsPhoneConfirm,
                            Type = e.Type,
                            UnitId = e.UnitId,
                            CountryId = e.CountryId,
                            TypeId = e.TypeId,
                            IdNumber = e.IdNumber,
                            DateNumber = e.DateNumber,
                            AddressNumber = e.AddressNumber,
                            PositionId = e.PositionId,
                            AcademicRankId = e.AcademicRankId,
                            DegreeId = e.DegreeId,
                            RoleId = e.RoleId,
                            WebsiteId = e.WebsiteId,
                            CompanyId = e.CompanyId,
                            TypeThirdId = e.TypeThirdId,
                            LastLoginAt = e.LastLoginAt,
                            UserId = e.UserId,
                            CreatedAt = e.CreatedAt,
                            UpdatedAt = e.UpdatedAt,
                            Status = e.Status,
                            StudentCode = e.StudentCode,
                            StudentClass = e.StudentClass,
                            StudentYear = e.StudentYear,
                            SchoolCode = e.SchoolCode,
                            AchievementNote = e.AchievementNote,
                            HobbyNote = e.HobbyNote,
                            PersonSummary = e.PersonSummary,
                            SocialNetworks = e.SocialNetworks,
                            IsViewInfo = e.IsViewInfo,
                            StepTwo = e.StepTwo,
                            StepFour = e.StepFour,
                            StepFive = e.StepFive,
                            TopThree = e.TopThree,
                            TypeAttributeId = e.TypeAttributeId,
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
                        metaDataDT.Lock = data.Where(e => e.Status == 98).Count();

                        def.metadata = metaDataDT;

                        if (paging.page_size > 0)
                        {
                            if (paging.order_by != null && paging.order_by != "")
                            {
                                data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                            }
                            else
                            {
                                data = data.OrderBy("CustomerId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                                data = data.OrderBy("CustomerId desc");
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
                            var listDatas = data.ToList();
                            def.data = listDatas;
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

        [HttpPut("putStudent/{id}")]
        public async Task<IActionResult> PutStudent(int id, [FromBody] CustomerDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            //if (!ModelState.IsValid)
            //{
            //    def.meta = new Meta(400, "Lỗi sai dữ liệu");
            //    return Ok(def);
            //}

            using (var db = new IOITDataContext())
            {
                Customer customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                if (customer == null)
                {
                    def.meta = new Meta(212, "Người đăng ký không tồn tại!");
                    return Ok(def);
                }
                Customer checkEmail = db.Customer.Where(c => c.StudentCode == data.StudentCode.ToLower().Trim() && c.CustomerId != id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                if (checkEmail != null)
                {
                    def.meta = new Meta(212, "Mã sinh viên đã tồn tại!");
                    return Ok(def);
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    customer.Username = data.Email.ToLower().Trim();
                    customer.FullName = data.FullName;
                    customer.Email = data.Email.ToLower().Trim();
                    customer.Phone = data.Phone;
                    customer.Avata = data.Avata;
                    customer.Sex = data.Sex;
                    customer.Birthday = data.Birthday;
                    customer.Address = data.Address;
                    customer.Note = data.Note;
                    customer.Type = data.Type;
                    customer.UnitId = data.UnitId;
                    customer.CountryId = data.CountryId;
                    customer.TypeId = data.TypeId;
                    customer.IdNumber = data.IdNumber;
                    customer.DateNumber = data.DateNumber;
                    customer.AddressNumber = data.AddressNumber;
                    customer.PositionId = data.PositionId;
                    customer.AcademicRankId = data.AcademicRankId;
                    customer.DegreeId = data.DegreeId;
                    customer.RoleId = data.RoleId;
                    customer.UserId = userId;
                    customer.UpdatedAt = DateTime.Now;

                    // nếu cho hiển thị thì trạng thái là 1
                    if (data.IsViewInfo == true)
                    {
                        customer.Status = (int)Const.Status.NORMAL;
                    }
                    else
                    {
                        customer.Status = (int)Const.Status.TEMP;
                    }
                    customer.StepTwo = data.StepTwo;
                    customer.StepFour = data.StepFour;
                    customer.StepFive = data.StepFive;
                    customer.TopThree = data.TopThree;
                    customer.StudentClass = data.StudentClass;
                    customer.StudentCode = data.StudentCode;
                    customer.StudentYear = data.StudentYear;
                    customer.SchoolCode = data.SchoolCode;
                    customer.AchievementNote = data.AchievementNote;
                    customer.HobbyNote = data.HobbyNote;
                    customer.PersonSummary = data.PersonSummary;
                    customer.IsViewInfo = data.IsViewInfo;
                    customer.SocialNetworks = data.SocialNetworks;
                    try
                    {
                        db.Entry(customer).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        transaction.Commit();

                        def.meta = new Meta(200, "Success");
                        def.data = data;
                        return Ok(def);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error:" + ex);
                        def.meta = new Meta(500, "Internal Server Error");
                        return Ok(def);
                    }

                }
            }
        }

        [HttpPost("ExportExcel")]
        public async Task<IActionResult> ExportExcel()
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            if (!ModelState.IsValid)
            {
                def.meta = new Meta(400, "Lỗi sai dữ liệu");
                return Ok(def);
            }
            using (var db = new IOITDataContext())
            {
                List<CustomerDTO> data = db.Customer.Where(c => c.Status != (int)Const.Status.DELETED).Select(e => new CustomerDTO
                {
                    CustomerId = e.CustomerId,
                    FullName = e.FullName,
                    Email = e.Email,
                    Phone = e.Phone,
                    Avata = e.Avata,
                    Sex = e.Sex,
                    Birthday = e.Birthday,
                    Address = e.Address,
                    Note = e.Note,
                    Type = e.Type,
                    UnitId = e.UnitId,
                    TypeId = e.TypeId,
                    DateNumber = e.DateNumber,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    Status = e.Status,
                    StudentCode = e.StudentCode,
                    StudentClass = e.StudentClass,
                    StudentYear = e.StudentYear,
                    SchoolCode = e.SchoolCode,
                    HobbyNote = e.HobbyNote,
                    AchievementNote = e.AchievementNote,
                    PersonSummary = e.PersonSummary,
                    AcademicRankId = e.AcademicRankId,
                }).ToList();

                var listDatas = data.ToList();
                foreach (var item in listDatas)
                {
                    var unit = db.Unit.Where(e => e.UnitId == item.UnitId).FirstOrDefault();
                    item.UnitName = unit != null ? unit.Name : "";
                    var listMap = await db.CustomerMapping.Where(e => e.CustomerId == item.CustomerId
                    && e.Status != (int)Const.Status.DELETED).ToListAsync();
                    item.ListRoles = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_ROLE)
                        .Select(e => (int)e.TargetId).ToList();
                    item.ListResearchArea = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION)
                        .Select(e => (int)e.TargetId).ToList();
                    item.ListUnitManager = listMap.Where(e => e.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT)
                        .Select(e => (int)e.TargetId).ToList();
                }

                // khởi tạo wb rỗng
                XSSFWorkbook wb = new XSSFWorkbook();
                // Tạo ra 1 sheet
                ISheet sheet = wb.CreateSheet();

                string template = @"template/export/baocao.xlsx"; // tu tao template r gan vao day 
                string webRootPath = _hostingEnvironment.WebRootPath;
                string templatePath = Path.Combine(webRootPath, template);

                MemoryStream ms = WriteDataToExcel(templatePath, 0, listDatas);
                byte[] byteArrayContent = ms.ToArray();
                return new FileContentResult(byteArrayContent, "application/octet-stream");
            }

        }

        private static MemoryStream WriteDataToExcel(string templatePath, int sheetnumber, List<CustomerDTO> data)
        {
            FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
            XSSFWorkbook workbook = new XSSFWorkbook(file);
            ISheet sheet = workbook.GetSheetAt(sheetnumber);
            IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();

            int rowStart = 1;
            if (sheet != null)
            {
                int datacol = 15; // may cot thi dien datacol vao
                try
                {
                    //style body
                    List<ICellStyle> rowStyle = new List<ICellStyle>();

                    for (int i = 0; i < datacol; i++)
                    {
                        rowStyle.Add(sheet.GetRow(0).GetCell(i).CellStyle);
                    }
                    //Thêm row
                    int k = 0;

                    foreach (var item in data)
                    {
                        try
                        {
                            XSSFRow row = (XSSFRow)sheet.CreateRow(rowStart);
                            for (int i = 0; i < datacol; i++)
                            {
                                row.CreateCell(i).CellStyle = rowStyle[i];

                                if (i == 0)// cot dau cac cot sau tuong tu
                                {
                                    row.GetCell(i).SetCellValue(k + 1); //STT
                                }
                                else if (i == 1)
                                {
                                    row.GetCell(i).SetCellValue(item.FullName);
                                }
                                else if (i == 2)
                                {
                                    row.GetCell(i).SetCellValue(item.StudentCode);
                                }
                                else if (i == 3)
                                {
                                    if (item.AcademicRankId != null)
                                    {
                                        row.GetCell(i).SetCellValue((double)item.AcademicRankId);
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                }
                                else if (i == 4)
                                {
                                    if (item.UnitId != null)
                                    {
                                        row.GetCell(i).SetCellValue((double)item.UnitId);
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                }
                                else if (i == 5)
                                {
                                    if (item.TypeId != null)
                                    {
                                        row.GetCell(i).SetCellValue((double)item.TypeId);
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                }
                                else if (i == 6)
                                {
                                    if (item.Sex == 1)
                                    {
                                        row.GetCell(i).SetCellValue("Nam");
                                    }
                                    if (item.Sex == 2)
                                    {
                                        row.GetCell(i).SetCellValue("Nữ");
                                    }
                                    if (item.Sex == 3)
                                    {
                                        row.GetCell(i).SetCellValue("Khác");
                                    }
                                }
                                else if (i == 7)
                                {
                                    row.GetCell(i).SetCellValue((DateTime)item.Birthday);
                                }
                                else if (i == 8)
                                {
                                    row.GetCell(i).SetCellValue(item.Phone);
                                }
                                else if (i == 9)
                                {
                                    row.GetCell(i).SetCellValue(item.Email);
                                }
                                else if (i == 10)
                                {
                                    row.GetCell(i).SetCellValue(item.StudentYear);
                                }
                                else if (i == 11)
                                {
                                    row.GetCell(i).SetCellValue(item.StudentClass);
                                }
                                else if (i == 12)
                                {
                                    if (item.SchoolCode == 1)
                                    {
                                        row.GetCell(i).SetCellValue("Trường cao đẳng du lịch và thương mại Hà Nội");
                                    }
                                    if (item.SchoolCode == 2)
                                    {
                                        row.GetCell(i).SetCellValue("Trường cao đẳng công thương Việt Nam");
                                    }
                                    if (item.SchoolCode == 3)
                                    {
                                        row.GetCell(i).SetCellValue(item.UnitName);
                                    }
                                }
                                else if (i == 13)
                                {
                                    row.GetCell(i).SetCellValue(item.AchievementNote);
                                }
                                else if (i == 14)
                                {
                                    row.GetCell(i).SetCellValue(item.HobbyNote);
                                }
                                else if (i == 15)
                                {
                                    row.GetCell(i).SetCellValue(item.PersonSummary);
                                }
                            }
                            rowStart++;
                            k++;
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            sheet.ForceFormulaRecalculation = true;

            MemoryStream ms = new MemoryStream();

            workbook.Write(ms);
            return ms;
        }
    }
}

