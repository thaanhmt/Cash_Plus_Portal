using IOITWebApp31.Models;
using IOITWebApp31.Models.Common;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
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

    public class DataSetApprovedController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("dataset-approved", "dataset-approved");
        private static string functionCode = "DNB";
        private static string functionCode1 = "DCK";
        private readonly IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;

        public DataSetApprovedController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        // GET: api/DataSetApproved
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
                    IQueryable<DataSetApproved> data = db.DataSetApproved.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("DataSetApprovedId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("DataSetApprovedId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = await data.Select(paging.select).ToDynamicListAsync();
                    }
                    else
                        def.data = await data.ToListAsync();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/DataSetApproved/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataSetApproved(Guid id)
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
            try
            {
                using (var db = new IOITDataContext())
                {
                    DataSetApproved data = await db.DataSetApproved.FindAsync(id);

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

        // PUT: api/DataSetApproved/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataSetApproved(Guid id, DataSetApproved data)
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
                if ((userId != data.UpdatedId))
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        data.UpdatedId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = data.Status;
                        db.DataSetApproved.Update(data);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DataSetApprovedId != null)
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
                            if (!DataSetApprovedExists(data.DataSetApprovedId))
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

        // POST: api/DataSetApproved
        [HttpPost]
        public async Task<IActionResult> PostDataSetApproved(DataSetApproved data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (data.Type == (int)Const.DataSetConfirmType.CONFIRM_PRIVATE)
            {
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
                {
                    def.meta = new Meta(222, "Bạn không có quyền duyệt nội bộ");
                    return Ok(def);
                }
            }
            else
            {
                if (!CheckRole.CheckRoleByCode(access_key, functionCode1, (int)Const.Action.UPDATE))
                {
                    def.meta = new Meta(222, "Bạn không có quyền duyệt công khai");
                    return Ok(def);
                }
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                if (userId != data.CreatedId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                if (data.DataSetStatus == null || data.DataSetStatus <= 0)
                {
                    def.meta = new Meta(211, "Chưa chọn trạng thái phê duyệt!");
                    return Ok(def);
                }
                if (data.DataSetStatus == 2)
                {
                    if (data.Confirms == null || data.Confirms == "")
                    {
                        def.meta = new Meta(211, "Chưa nhập lý do không duyệt!");
                        return Ok(def);
                    }
                }
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //check xem dataset có đúng
                        var checkData = await db.DataSet.Where(e => e.DataSetId == data.DataSetId
                        && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                        if (checkData == null)
                        {
                            def.meta = new Meta(400, "Không tìm thấy bộ dữ liệu cần duyệt!");
                            return Ok(def);
                        }

                        data.DataSetApprovedId = Guid.NewGuid();
                        data.CreatedId = userId;
                        data.UpdatedId = userId;
                        data.CreatedAt = DateTime.Now;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.NORMAL;
                        await db.DataSetApproved.AddAsync(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DataSetApprovedId != null)
                            {
                                checkData.UpdatedAt = DateTime.Now;
                                checkData.UserId = userId;
                                string str = "được";
                                string str2 = "nội bộ";
                                //Update lại trạng thái dataset
                                if (data.Type == (int)Const.DataSetConfirmType.CONFIRM_PRIVATE)
                                {
                                    checkData.ApprovedAt = DateTime.Now;
                                    checkData.UserApprovedId = userId;
                                    checkData.ConfirmsPrivate = data.Confirms;
                                    if (data.DataSetStatus == (int)Const.DataSetConfirmStatus.NOT_APPROVED)
                                    {
                                        checkData.Status = (int)Const.DataSetStatus.NOT_APPROVED;
                                        str = "không được";
                                    }
                                    else
                                        checkData.Status = (int)Const.DataSetStatus.APPROVED;
                                }
                                else
                                {
                                    str2 = "công khai";
                                    checkData.PublishedAt = DateTime.Now;
                                    checkData.UserPublishedId = userId;
                                    checkData.ConfirmsPublish = data.Confirms;
                                    if (data.DataSetStatus == (int)Const.DataSetConfirmStatus.APPROVED)
                                        checkData.Status = (int)Const.DataSetStatus.NORMAL;
                                    else
                                    {
                                        checkData.Status = (int)Const.DataSetStatus.NOT_APPROVED_PUBLISH;
                                        str = "không được";
                                    }
                                }

                                //Gửi Email vả thông báo
                                try
                                {
                                    Config config = db.Config.Where(c => c.ConpanyId == Const.WEBSITEID).FirstOrDefault();
                                    if (config == null)
                                    {
                                        def.meta = new Meta(404, "Không tìm thấy cấu hình để gửi Email xác nhận đăng ký!");
                                        return Ok(def);
                                    }

                                    if (config.EmailSender == null || config.EmailSender == "" || config.EmailHost == null || config.EmailHost == "" || config.EmailUserName == null || config.EmailUserName == "" || config.EmailPasswordHash == null || config.EmailPasswordHash == "" || config.EmailPort == null)
                                    {
                                        def.meta = new Meta(404, "Thông tin cấu hình để gửi Email xác nhận đăng ký không chính xác!");
                                        return Ok(def);
                                    }
                                    //Lấy thông tin người đăng
                                    var userCreate = await db.Customer.Where(e => e.CustomerId == checkData.UserCreatedId).FirstOrDefaultAsync();
                                    if (userCreate == null)
                                    {
                                        def.meta = new Meta(404, "Thông tin người tạo bộ dữ liệu không đúng!");
                                        return Ok(def);
                                    }
                                    //Lấy thông tin người duyệt
                                    var userApproved = await db.Customer.Where(e => e.CustomerId == userId).FirstOrDefaultAsync();
                                    if (userApproved == null)
                                    {
                                        def.meta = new Meta(404, "Thông tin người duyệt không đúng!");
                                        return Ok(def);
                                    }
                                    //Gửi Email vả thông báo
                                    string subject = config.EmailSender + " - Phê duyệt bộ dữ liệu";
                                    //Tạo thông báo
                                    String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/notification-dataset-accept.html");
                                    string linkConfirm = "xem-chi-tiet-du-lieu-" + checkData.DataSetId;
                                    //Lấy ds người nhận, nếu là cá nhân thì các tk có quyền ql ng dùng,
                                    //nếu tổ chức là các tk quản lý tổ chức đó
                                    linkConfirm = config.Website + linkConfirm;
                                    string linkConfirmUrl = "'" + linkConfirm + "'";
                                    sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                config.Website, userCreate.FullName, str, linkConfirmUrl, linkConfirm, str2);
                                    //Tạo thông báo và gửi mail
                                    Notification notification = new Notification();
                                    if (userCreate.Email != null && userCreate.Email != "" && userCreate.IsNotificationMail == true)
                                        notification.IsSentEmail = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, userCreate.Email, subject, sBody);
                                    if (userCreate.IsNotificationWeb == true)
                                    {
                                        notification.NotificationId = Guid.NewGuid();
                                        notification.Title = subject;
                                        notification.Contents = sBody;
                                        notification.UserPushId = userApproved.CustomerId;
                                        notification.UserReadId = userCreate.CustomerId;
                                        notification.UrlLink = linkConfirm;
                                        notification.TargetId = checkData.DataSetId + "";
                                        notification.TargetType = (int)Const.NotificationTargetType.DATASET;
                                        notification.CreatedAt = DateTime.Now;
                                        notification.UpdatedAt = DateTime.Now;
                                        notification.Status = (int)Const.Status.TEMP;
                                        await db.Notification.AddAsync(notification);
                                    }



                                    await db.SaveChangesAsync();
                                }
                                catch { }

                                db.DataSet.Update(checkData);
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
                            if (DataSetApprovedExists(data.DataSetApprovedId))
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

        // DELETE: api/DataSetApproved/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDataSetApproved(Guid id)
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
                    DataSetApproved data = await db.DataSetApproved.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    if (userId != data.UpdatedId)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UpdatedId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.DataSetApproved.Update(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DataSetApprovedId != null)
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
                            if (!DataSetApprovedExists(data.DataSetApprovedId))
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

        private bool DataSetApprovedExists(Guid id)
        {
            using (var db = new IOITDataContext())
            {
                return db.DataSetApproved.Count(e => e.DataSetApprovedId == id) > 0;
            }
        }
    }
}
