using IOITWebApp31.Models;
using IOITWebApp31.Models.Common;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("contact", "contact");
        private readonly IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;

        public ContactController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [HttpPost("SendContact")]
        public async Task<IActionResult> SendContact(ContactDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {

                using (var db = new IOITDataContext())
                {
                    string email = data.Email.Trim().ToLower();

                    using (var transaction = db.Database.BeginTransaction())
                    {

                        //Tạo liên hệ


                        var Cur = db.Contact.Where(l => l.Email == data.Email && l.Phone == data.Phone).FirstOrDefault();
                        if (Cur != null)
                        {
                            Cur.UpdatedAt = DateTime.Now;
                            Cur.Title = data.Title;
                            Cur.FullName = data.FullName;
                            Cur.Attactment = data.Attactment;
                            Cur.Contents = data.Contents;
                            Cur.NewsId = data.NewsId;
                            Cur.Address = data.Address;
                            Cur.Note = data.Note;

                            db.Entry(Cur).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            transaction.Commit();

                            def.meta = new Meta(200, "Sửa thành công!");
                            return Ok(def);
                        }
                        else
                        {
                            Contact contact = new Contact();
                            contact.NewsId = data.NewsId;
                            contact.Title = data.Title;
                            contact.FullName = data.FullName;
                            contact.Email = email;
                            contact.CustomerId = data.CustomerId;
                            contact.Phone = data.Phone;
                            contact.Address = data.Address;
                            contact.Attactment = data.Attactment;
                            contact.Contents = data.Contents;
                            contact.Note = data.Note;
                            contact.TypeContact = data.TypeContact;
                            contact.CompanyId = Const.COMPANYID;
                            contact.BranchId = data.BranchId;
                            contact.Status = (int)Const.Status.NORMAL;
                            contact.CreatedAt = DateTime.Now;
                            await db.Contact.AddAsync(contact);
                            await db.SaveChangesAsync();

                            if (contact.ContactId > 0)
                            {
                                transaction.Commit();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Thêm thành công!");
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


        [HttpPost("SendContactGopY")]
        public async Task<IActionResult> SendContactGopY(ContactDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }


                using (var db = new IOITDataContext())
                {
                    string email = data.Email.Trim().ToLower();

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //Tự động tạo tk khách hàng theo Email nếu chưa có tk

                        var checkCus = await db.Customer.Where(e => e.Email.Equals(email)).FirstOrDefaultAsync();
                        if (checkCus == null)
                        {
                            Customer customer = new Customer();
                            customer.FullName = data.FullName;
                            customer.Email = email;
                            customer.Phone = data.Phone;
                            customer.Username = customer.Email;
                            customer.Type = 1;
                            customer.WebsiteId = 1;
                            customer.CompanyId = 1;
                            customer.CountryId = 1;
                            customer.TypeThirdId = 1;
                            customer.IsEmailConfirm = true;
                            customer.IsSentEmailConfirm = true;
                            customer.IsPhoneConfirm = true;
                            customer.KeyRandom = Utils.RandomString(8);
                            customer.Password = Utils.GetMD5Hash("123456" + customer.KeyRandom);
                            customer.LastLoginAt = DateTime.Now;
                            customer.CreatedAt = DateTime.Now;
                            customer.UpdatedAt = DateTime.Now;
                            customer.Status = (int)Const.Status.NORMAL;
                            await db.Customer.AddAsync(customer);
                            await db.SaveChangesAsync();
                            data.CustomerId = customer.CustomerId;
                        }
                        else
                            data.CustomerId = checkCus.CustomerId;
                        //Tạo liên hệ
                        Contact contact = new Contact();
                        contact.Title = data.Title;
                        contact.FullName = data.FullName;
                        contact.Email = email;
                        contact.CustomerId = data.CustomerId;
                        contact.Phone = data.Phone;
                        contact.Address = data.Address;
                        contact.Attactment = data.Attactment;
                        contact.Contents = data.Contents; // địa chỉ công ty
                        contact.Note = data.Note;
                        contact.Contents = data.Contents;
                        contact.TypeContact = data.TypeContact;
                        contact.CompanyId = Const.COMPANYID;
                        contact.BranchId = data.BranchId;
                        contact.CreatedAt = DateTime.Now;
                        contact.UpdatedAt = DateTime.Now;
                        contact.Status = (int)Const.Status.NORMAL;
                        await db.Contact.AddAsync(contact);

                        var sm = db.Config.Where(e => e.ConpanyId == Const.COMPANYID && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                        var mainEmail = db.Website.Where(t => t.WebsiteId == 1).FirstOrDefault().Hotmail;
                        var mailCustomer = email;
                        //EmailService email = new EmailService();
                        string userName = sm.EmailUserName;
                        string password = sm.EmailPasswordHash;
                        string smtpHost = sm.EmailHost;
                        int smtpPort = (int)sm.EmailPort;
                        string mainSendMail = mainEmail;
                        SentContact es = new SentContact();
                        es.Content = "<span style='font-size:150%;margin-bottom:15px;color:red;'>Bạn đã gửi một thông tin liên hệ:</span> <br><strong>Họ tên: </strong>" + contact.FullName + "<br><strong>Số điện thoại: </strong> " + contact.Phone + "<br><strong>Nội dung: </strong>" + data.Note;
                        es.Subject = "Góp ý bạn đọc";
                        es.ToEmail = mailCustomer;
                        string body = string.Format(es.Content);
                        bool result = EmailService.Send(userName, password, smtpHost, smtpPort, es.ToEmail, es.Subject, body); // Gửi cho khách hàng
                        bool result_main = EmailService.Send(userName, password, smtpHost, smtpPort, mainSendMail, es.Subject, "<span style='font-size:150%;margin-bottom:15px;color:red;'>Bạn đã nhận một thông tin liên hệ:</span> <br><strong>Họ tên: </strong>" + contact.FullName + "<br><strong>Số điện thoại: </strong> " + contact.Phone + "<br><strong>Nội dung: </strong>" + data.Note); // Gửi cho quản trị viên

                        try
                        {
                            await db.SaveChangesAsync();
                            data.ContactId = contact.ContactId;

                            if (data.ContactId > 0)
                            {
                                transaction.Commit();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Thêm liên hệ thành công!");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (ContactExists(data.ContactId))
                            {
                                def.meta = new Meta(212, "Tài khoản đã tồn tại! Xin vui lòng thử lại!");
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

        [HttpPost("SendContactAution")]
        public async Task<IActionResult> SendContactAution(ContactDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }


                using (var db = new IOITDataContext())
                {
                    //var checkExistEmail = db.Customer.Where(e => e.Email == data.Email && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    //if (checkExistEmail != null)
                    //{
                    //    def.meta = new Meta(212, "Email đã tồn tại!");
                    //    return Ok(def);
                    //}

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Contact contact = new Contact();
                        contact.Title = data.Title;
                        contact.FullName = data.FullName;
                        contact.Email = data.Email;
                        contact.CustomerId = data.CustomerId;
                        contact.Phone = data.Phone;
                        contact.Note = data.Note;
                        contact.TypeContact = data.TypeContact;
                        contact.CompanyId = Const.COMPANYID;
                        contact.BranchId = data.BranchId;
                        contact.CreatedAt = DateTime.Now;
                        contact.UpdatedAt = DateTime.Now;
                        contact.Status = (int)Const.Status.NORMAL;
                        db.Contact.Add(contact);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.ContactId = contact.ContactId;

                            if (data.ContactId > 0)
                            {
                                transaction.Commit();
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Thêm liên hệ thành công!");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (ContactExists(data.ContactId))
                            {
                                def.meta = new Meta(212, "Tài khoản đã tồn tại! Xin vui lòng thử lại!");
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

        [HttpGet("getBranch")]
        public async Task<IActionResult> getBranch()
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                using (var db = new IOITDataContext())
                {
                    var data = db.Branch.Where(e => e.Status == (int)Const.Status.NORMAL).ToList();
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

        private bool ContactExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Contact.Count(e => e.ContactId == id) > 0;
            }
        }
    }
}