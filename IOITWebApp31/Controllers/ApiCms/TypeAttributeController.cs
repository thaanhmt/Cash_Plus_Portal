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
    public class TypeAttributeController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("typeattribute", "typeattribute");
        private static string functionCode = "QLLH";

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
                    IQueryable<TypeAttribute> data = db.TypeAttribute.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("TypeAttributeId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("TypeAttributeId desc");
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
                            e.TypeAttributeId,
                            e.Name,
                            e.IsUpdate,
                            e.IsDelete,
                            e.TypeAttribuiteParentId,
                            e.UserId,
                            e.CreatedAt,
                            e.UpdatedAt,
                            e.Location,
                            e.Size,
                            e.Status,
                            e.Image,
                            e.Description,
                            e.IsGroup,
                            listAttributeItem = db.TypeAttributeItem.Where(c => c.TypeAttributeId == e.TypeAttributeId && c.Status != (int)Const.Status.DELETED).Select(c => new
                            {
                                c.TypeAttributeItemId,
                                c.Name,
                                c.TypeAttributeId,
                                c.Location,
                                c.Code,
                                c.Image,
                                c.CreatedAt,
                                c.UpdatedAt,
                                c.Status
                            }).ToList(),
                            listCustomer = db.Customer.Where(c => c.TypeAttributeId == e.TypeAttributeId && c.Status != (int)Const.Status.DELETED).Select(c => new
                            {
                                //thêm gì thì tự viết vào
                                c.TypeAttributeId,
                                c.FullName,
                                c.Avata,
                                c.CustomerId,
                                c.StudentCode,
                                //c.FullName,
                                c.CreatedAt,
                                c.UpdatedAt,
                                c.Status
                            }).ToList()
                        }).ToList();
                    }
                    //def.data = data.ToList();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/TypeAttribute/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeAttribute(int id)
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
                    TypeAttribute data = await db.TypeAttribute.FindAsync(id);

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

        // PUT: api/TypeAttribute/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeAttribute(int id, [FromBody] TypeAttributeDTO data)
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
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        TypeAttribute typeAttribute = db.TypeAttribute.Where(e => e.TypeAttributeId == id).FirstOrDefault();
                        typeAttribute.Name = data.Name;
                        typeAttribute.Description = data.Description;
                        typeAttribute.Image = data.Image;
                        typeAttribute.Location = data.Location;
                        typeAttribute.TypeAttribuiteParentId = data.TypeAttribuiteParentId;
                        typeAttribute.IsUpdate = data.IsUpdate;
                        typeAttribute.IsDelete = data.IsDelete;
                        typeAttribute.UpdatedAt = DateTime.Now;
                        typeAttribute.UserId = userId;
                        typeAttribute.Status = data.Status;
                        typeAttribute.IsGroup = data.IsGroup;

                        db.Entry(typeAttribute).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.TypeAttributeId > 0)
                            {
                                ////Xóa cũ
                                //var listTypeAttributeItem = db.TypeAttributeItem.Where(e => e.TypeAttributeId == id).ToList();
                                //foreach(var item in listTypeAttributeItem)
                                //{
                                //    item.Status = (int)Const.Status.DELETED;
                                //    db.Entry(item).State = EntityState.Modified;
                                //}
                                //await db.SaveChangesAsync();
                                //Thêm mới
                                if (data.listAttributeItem != null)
                                {
                                    foreach (var item in data.listAttributeItem)
                                    {

                                        if (item.Status != (int)Const.Status.DELETED && item.TypeAttributeItemId == null)
                                        {
                                            TypeAttributeItem typeAttributeItem = new TypeAttributeItem();
                                            typeAttributeItem.Name = item.Name;
                                            typeAttributeItem.TypeAttributeId = typeAttribute.TypeAttributeId;
                                            typeAttributeItem.Code = item.Code;
                                            typeAttributeItem.Location = item.Location;
                                            typeAttributeItem.UserId = userId;
                                            typeAttributeItem.Image = item.Image;
                                            typeAttributeItem.Description = item.Description;
                                            typeAttributeItem.CreatedAt = DateTime.Now;
                                            typeAttributeItem.UpdatedAt = DateTime.Now;
                                            typeAttributeItem.Status = (int)Const.Status.NORMAL;
                                            await db.TypeAttributeItem.AddAsync(typeAttributeItem);
                                        }
                                        else if (item.TypeAttributeItemId != null)
                                        {
                                            TypeAttributeItem exist = db.TypeAttributeItem.Find(item.TypeAttributeItemId);
                                            if (exist != null)
                                            {
                                                if (item.Status == (int)Const.Status.DELETED)
                                                {
                                                    exist.Status = (int)Const.Status.DELETED;
                                                }
                                                else
                                                {
                                                    exist.Code = item.Code;
                                                    exist.Name = item.Name;
                                                    exist.Image = item.Image;
                                                    exist.Description = item.Description;
                                                    exist.Location = item.Location;
                                                }
                                                db.Entry(exist).State = EntityState.Modified;
                                            }
                                        }
                                    }
                                }
                                await db.SaveChangesAsync();
                                var list = db.Customer.Where(x => x.TypeAttributeId == data.TypeAttributeId).ToList();
                                if (data.listCustomer != null)
                                {
                                    foreach (var item in data.listCustomer)
                                    {
                                        Customer exist = list.Where(x => x.CustomerId == item.CustomerId).FirstOrDefault();
                                        if (exist == null)
                                        {
                                            Customer customer = db.Customer.Where(x => x.CustomerId == item.CustomerId).FirstOrDefault();
                                            customer.TypeAttributeId = typeAttribute.TypeAttributeId;
                                            customer.UpdatedAt = DateTime.Now;
                                            db.Entry(customer).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            list.Remove(exist);
                                        }
                                    }
                                    if (list.Count() > 0)
                                    {
                                        foreach (var i in list)
                                        {
                                            i.TypeAttributeId = 0;
                                            i.UpdatedAt = DateTime.Now;
                                            db.Entry(i).State = EntityState.Modified;
                                        }
                                    }
                                }
                                await db.SaveChangesAsync();
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Sửa loại hình “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.TypeAttributeId.ToString();
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
                            if (!TypeAttributeExists(data.TypeAttributeId))
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

        // POST: api/TypeAttribute
        [HttpPost]
        public async Task<IActionResult> PostTypeAttribute([FromBody] TypeAttributeDTO data)
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

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        TypeAttribute typeAttribute = new TypeAttribute();
                        typeAttribute.Name = data.Name;
                        typeAttribute.Description = data.Description;
                        typeAttribute.Image = data.Image;
                        typeAttribute.Location = data.Location;
                        typeAttribute.TypeAttribuiteParentId = data.TypeAttribuiteParentId;
                        typeAttribute.IsUpdate = data.IsUpdate;
                        typeAttribute.IsDelete = data.IsDelete;
                        typeAttribute.CreatedAt = DateTime.Now;
                        typeAttribute.UpdatedAt = DateTime.Now;
                        typeAttribute.UserId = userId;
                        typeAttribute.IsGroup = data.IsGroup;
                        typeAttribute.Status = (int)Const.Status.NORMAL;

                        db.TypeAttribute.Add(typeAttribute);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (typeAttribute.TypeAttributeId > 0)
                            {
                                if (data.listAttributeItem.Count != 0)
                                {
                                    foreach (var item in data.listAttributeItem)
                                    {
                                        TypeAttributeItem typeAttributeItem = new TypeAttributeItem();
                                        typeAttributeItem.Name = item.Name;
                                        typeAttributeItem.TypeAttributeId = typeAttribute.TypeAttributeId;
                                        typeAttributeItem.Code = item.Code;
                                        typeAttributeItem.Location = item.Location;
                                        typeAttributeItem.UserId = userId;
                                        typeAttributeItem.Image = item.Image;
                                        typeAttributeItem.Description = item.Description;
                                        typeAttributeItem.CreatedAt = DateTime.Now;
                                        typeAttributeItem.UpdatedAt = DateTime.Now;
                                        typeAttributeItem.Status = (int)Const.Status.NORMAL;
                                        await db.TypeAttributeItem.AddAsync(typeAttributeItem);
                                    }
                                }
                                await db.SaveChangesAsync();
                                if (data.listCustomer != null)
                                {
                                    foreach (var item in data.listCustomer)
                                    {
                                        Customer customer = db.Customer.Where(x => x.CustomerId == item.CustomerId).FirstOrDefault();
                                        customer.TypeAttributeId = typeAttribute.TypeAttributeId;
                                        customer.UpdatedAt = DateTime.Now;
                                        db.Entry(customer).State = EntityState.Modified;
                                    }
                                }
                                await db.SaveChangesAsync();
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Thêm loại hình “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.TypeAttributeId.ToString();
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
                            if (TypeAttributeExists(data.TypeAttributeId))
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

        // DELETE: api/TypeAttribute/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeAttribute(int id)
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
                    TypeAttribute data = await db.TypeAttribute.FindAsync(id);
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
                        var listCustomer = db.Customer.Where(x => x.TypeAttributeId == id).ToList();
                        if (listCustomer.Count > 0)
                        {
                            foreach (var item in listCustomer)
                            {
                                item.TypeAttributeId = 0;
                                item.UpdatedAt = DateTime.Now;
                                db.Entry(item).State = EntityState.Modified;
                            }
                        }

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.TypeAttributeId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Xoá loại hình “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.DELETE;
                                action.TargetId = data.TypeAttributeId.ToString();
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
                            if (!TypeAttributeExists(data.TypeAttributeId))
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

        //API xóa danh sách website
        [HttpPut("deletes")]
        public async Task<IActionResult> DeleteTypeAttributes([FromBody] int[] data)
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
                        TypeAttribute typeAttribute = await db.TypeAttribute.FindAsync(data[i]);

                        if (typeAttribute == null)
                        {
                            continue;
                        }

                        typeAttribute.Status = (int)Const.Status.DELETED;
                        db.Entry(typeAttribute).State = EntityState.Modified;
                        Models.EF.Action action = new Models.EF.Action();
                        action.ActionName = "Xoá loại hình “" + typeAttribute.Name + "”";
                        action.ActionType = (int)Const.ActionType.DELETE;
                        action.TargetId = typeAttribute.TypeAttributeId.ToString();
                        action.TargetName = typeAttribute.Name;
                        action.CompanyId = companyId;
                        action.Logs = JsonConvert.SerializeObject(typeAttribute);
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

        private bool TypeAttributeExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.TypeAttribute.Count(e => e.TypeAttributeId == id) > 0;
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
