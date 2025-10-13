using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Authorize]
    [Route("web/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("notification", "notification");
        private static string functionCode = "QLTB";
        private static string functionCode2 = "CHTB";

        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
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
                    IQueryable<Notification> data = db.Notification.Where(c => c.UserReadId == userId && c.Status != (int)Const.Status.DELETED);
                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    MetaDataDT metaDataDT = new MetaDataDT();
                    metaDataDT.Sum = data.Count();
                    metaDataDT.Normal = data.Where(e => e.Status == 1).Count();
                    metaDataDT.Temp = data.Where(e => e.Status == 10).Count();

                    //Cập nhật lại số thống báo chưa đọc
                    HttpContext.Session.SetInt32("NunberNotification", (int)metaDataDT.Temp);


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
                                ShortName = c.FullName.Length > 1 ? c.FullName.Substring(0, 1) : c.FullName,
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

        [HttpGet("getSetting/{id}")]
        public async Task<IActionResult> GetSetting(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode2, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "Bạn không có quyền xem thông tin cấu hình thông báo!");
                return Ok(def);
            }
            try
            {
                if (userId != id)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    CustomerSetting data = await db.Customer.Where(e => e.CustomerId == id).Select(e => new CustomerSetting
                    {
                        CustomerId = e.CustomerId,
                        IsNotificationMail = e.IsNotificationMail,
                        IsNotificationWeb = e.IsNotificationWeb,
                    }).FirstOrDefaultAsync();

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

        [HttpPut("changeSetting/{id}")]
        public async Task<IActionResult> ChangeSetting(int id, [FromBody] CustomerSetting data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode2, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền sửa cài đặt thông báo!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                if (userId != id)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
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

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        customer.IsNotificationWeb = data.IsNotificationWeb;
                        customer.IsNotificationMail = data.IsNotificationMail;
                        customer.UserId = userId;
                        customer.UpdatedAt = DateTime.Now;
                        db.Entry(customer).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.CustomerId > 0)
                            {
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

        [HttpPut("readNotification/{id}/{idn}")]
        public async Task<IActionResult> ReadNotification(int id, Guid idn)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền đọc thông báo!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                if (userId != id)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    var customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (customer == null)
                    {
                        def.meta = new Meta(212, "Người dùng không tồn tại!");
                        return Ok(def);
                    }

                    var notification = db.Notification.Where(c => c.NotificationId == idn && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (notification == null)
                    {
                        def.meta = new Meta(212, "Thông báo không tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        notification.UpdatedAt = DateTime.Now;
                        notification.Status = (int)Const.Status.NORMAL;
                        db.Entry(notification).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (notification.NotificationId != null)
                            {
                                transaction.Commit();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = notification;
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

        [HttpPut("readNotifications/{id}")]
        public async Task<IActionResult> ReadNotifications(int id, List<Guid> data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền đọc thông báo!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                if (userId != id)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    var customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (customer == null)
                    {
                        def.meta = new Meta(212, "Người dùng không tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        foreach (var item in data)
                        {
                            var notification = db.Notification.Where(c => c.NotificationId == item && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (notification != null)
                            {
                                notification.UpdatedAt = DateTime.Now;
                                notification.Status = (int)Const.Status.NORMAL;
                                db.Entry(notification).State = EntityState.Modified;
                            }
                        }

                        try
                        {
                            await db.SaveChangesAsync();

                            if (customer.CustomerId > 0)
                            {
                                //Tính toán lại số thông báo chưa đọc
                                var listNoti = await db.Notification.Where(e => e.UserReadId == id && e.Status == (int)Const.Status.TEMP).ToListAsync();
                                HttpContext.Session.SetInt32("NunberNotification", listNoti.Count());
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

        [HttpPut("readAllNotifications/{id}")]
        public async Task<IActionResult> ReadAllNotifications(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền đọc thông báo!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                if (userId != id)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    var customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (customer == null)
                    {
                        def.meta = new Meta(212, "Người dùng không tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        var data = await db.Notification.Where(e => e.UserReadId == id && e.Status == (int)Const.Status.TEMP).ToListAsync();
                        foreach (var item in data)
                        {
                            //var notification = db.Notification.Where(c => c.NotificationId == item && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            //if (notification != null)
                            //{
                            item.UpdatedAt = DateTime.Now;
                            item.Status = (int)Const.Status.NORMAL;
                            db.Entry(item).State = EntityState.Modified;
                            //}
                        }

                        try
                        {
                            await db.SaveChangesAsync();

                            if (customer.CustomerId > 0)
                            {
                                //Tính toán lại số thông báo chưa đọc
                                var listNoti = await db.Notification.Where(e => e.UserReadId == id && e.Status == (int)Const.Status.TEMP).ToListAsync();
                                HttpContext.Session.SetInt32("NunberNotification", listNoti.Count());
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

        [HttpPut("deleteNotifications/{id}")]
        public async Task<IActionResult> DeleteNotifications(int id, List<Guid> data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "Bạn không có quyền xóa thông báo!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                if (userId != id)
                {
                    def.meta = new Meta(400, "Lỗi sai dữ liệu");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    var customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (customer == null)
                    {
                        def.meta = new Meta(212, "Người dùng không tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        foreach (var item in data)
                        {
                            var notification = db.Notification.Where(c => c.NotificationId == item && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (notification != null)
                            {
                                notification.UpdatedAt = DateTime.Now;
                                notification.Status = (int)Const.Status.DELETED;
                                db.Entry(notification).State = EntityState.Modified;
                            }
                        }

                        try
                        {
                            await db.SaveChangesAsync();

                            if (customer.CustomerId > 0)
                            {
                                //Tính toán lại số thông báo chưa đọc
                                var listNoti = await db.Notification.Where(e => e.UserReadId == id && e.Status == (int)Const.Status.TEMP).ToListAsync();
                                HttpContext.Session.SetInt32("NunberNotification", listNoti.Count());
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

    }
}
