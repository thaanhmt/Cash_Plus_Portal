using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;


namespace IOITWebApp31.Controllers
{
    public class CustomerController : Controller
    {
        private static string functionCodeDSDL = "DSDL";
        private static string functionCodeDNB = "DNB";
        private static string functionCodeDCK = "DCK";
        private static string functionCodeDSTC = "CQTC";
        private static string functionCodeNDTC = "QLND";
        private static string functionCodeCEPH = "CEPH";
        private static readonly ILog log = LogMaster.GetLogger("customer", "customer");
        private readonly IConfiguration _configuration;

        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult GuideDisableAccount()
        {
            return View();
        }

        public ActionResult Login()
        {
            ViewBag.Class = "Login-page customer-page";
            //string ckMatKhau = Request.Cookies["userName"];
            //string ckTenDangNhap = Request.Cookies["passWord"];
            ////ckMatKhau = Request.Cookies["U_MatKhau"];
            ////ckTenDangNhap = Request.Cookies["U_TenDangNhap"];
            //if (ckTenDangNhap != null)
            //{
            //    if (CheckLogin(ckTenDangNhap, ckMatKhau, true))
            //    {
            //        TempData["success"] = "Bạn đã đăng nhập thành công.";
            //        ViewBag.SeoTitle = "Đăng nhập khách hàng";
            //        ViewBag.SeoDescription = "Đăng nhập khách hàng";
            //        ViewBag.SeoKeywords = "Đăng nhập khách hàng";
            //        return RedirectToAction("Index", "Home", null);
            //    }
            //}
            //return View();
            var CustomerId = HttpContext.Session.GetInt32("CustomerId");
            if (CustomerId != null)
            {
                return Redirect("/");
            }

            ViewBag.current_url = HttpContext.Request.Headers["Referer"].ToString();

            if (ViewBag.current_url.Contains("xac-nhan-dang-ky"))
            {
                var current_url = HttpContext.Session.GetString("current_url");
                if (current_url != null)
                {
                    ViewBag.current_url = current_url;
                }
                else
                {
                    ViewBag.current_url = "/";
                }

            }

            string title = "Đăng nhập thành viên";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            return View();
        }

        //public ActionResult EditUser()
        //{
        //    if (HttpContext.Session.GetInt32("CustomerId") == null)
        //    {
        //        return Redirect("/dang-nhap.html?key=dn");
        //    }
        //    ViewBag.Class = "login-update-page customer-page";
        //    ViewBag.ActionMenu = 5;
        //    CustomerLogin customerLogin = new CustomerLogin();
        //    customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
        //    customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
        //    customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
        //    customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
        //    customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
        //    customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
        //    customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
        //    customerLogin.access_token = HttpContext.Session.GetString("access_token");
        //    customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

        //    ViewBag.SeoTitle = "Chỉnh sửa tài khoản";
        //    ViewBag.SeoDescription = "Chỉnh sửa tài khoản";
        //    ViewBag.SeoKeywords = "Chỉnh sửa tài khoản";
        //    return View(customerLogin);
        //}

        //public ActionResult MyOrder()
        //{
        //    var CustomerId = HttpContext.Session.GetInt32("CustomerId");
        //    if (CustomerId == null)
        //    {
        //        return Redirect("/dang-nhap.html?key=dn");
        //    }

        //    CustomerLogin customerLogin = new CustomerLogin();
        //    customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
        //    customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
        //    customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
        //    customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
        //    customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
        //    customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
        //    customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
        //    customerLogin.access_token = HttpContext.Session.GetString("access_token");
        //    customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

        //    ViewBag.CustomerId = CustomerId;
        //    ViewBag.SeoTitle = "Đơn hàng của tôi";
        //    ViewBag.SeoDescription = "Đơn hàng của tôi";
        //    ViewBag.SeoKeywords = "Đơn hàng của tôi";
        //    return View(customerLogin);
        //}

        //public ActionResult DetailOrder(int OrderId)
        //{
        //    ViewBag.SeoTitle = "Chi tiết đơn hàng";
        //    ViewBag.SeoDescription = "Chi tiết đơn hàng";
        //    ViewBag.SeoKeywords = "Chi tiết đơn hàng";

        //    var CustomerId = HttpContext.Session.GetInt32("CustomerId");
        //    if (CustomerId == null)
        //    {
        //        return Redirect("/dang-nhap.html?key=dn");
        //    }

        //    using (var db = new IOITDataContext())
        //    {
        //        Order order = db.Order.Where(c => c.OrderId == OrderId && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
        //        if (order == null)
        //        {
        //            return Redirect("/Home/Error");
        //        }

        //        if(order.CustomerId != CustomerId)
        //        {
        //            return Redirect("/Home/Error");
        //        }

        //        OrderWebDTO data = new OrderWebDTO();
        //        data.OrderId = order.OrderId;
        //        data.Code = order.Code;
        //        data.CustomerId = order.CustomerId;
        //        //data.ReceiverName = order.ReceiverName;
        //        //data.ReceiverEmail = order.ReceiverEmail;
        //        //data.ReceiverPhone = order.ReceiverPhone;
        //        data.PaymentMethodId = order.PaymentMethodId;
        //        data.PaymentStatusId = order.PaymentStatusId;
        //        data.ShippingMethodId = order.ShippingMethodId;
        //        data.ShippingStatusId = order.ShippingStatusId;
        //        data.OrderStatusId = order.OrderStatusId;
        //        data.OrderTax = order.OrderTax;
        //        data.OrderDiscount = order.OrderDiscount;
        //        data.OrderTotal = order.OrderTotal;
        //        data.CustomerNote = order.CustomerNote;
        //        data.WebsiteId = order.WebsiteId;
        //        data.CompanyId = order.CompanyId;
        //        data.UserId = order.UserId;
        //        data.CreatedAt = order.CreatedAt;
        //        data.UpdatedAt = order.UpdatedAt;
        //        data.Status = order.Status;
        //        data.listOrderItem = db.OrderItem.Where(oi => oi.OrderId == order.OrderId && oi.Status != (int)Const.Status.DELETED).Select(oi => new OrderItemDTO
        //        {
        //            OrderItemId = oi.OrderItemId,
        //            OrderId = oi.OrderId,
        //            ProductId = oi.ProductId,
        //            ProductName = db.Product.Where(p => p.ProductId == oi.ProductId).FirstOrDefault().Name,
        //            Quantity = oi.Quantity,
        //            Price = oi.Price,
        //            PriceTax = oi.PriceTax,
        //            PriceDiscount = oi.PriceDiscount,
        //            PriceTotal = oi.PriceTotal,
        //            Status = oi.Status
        //        }).ToList();

        //        return View(data);
        //    }

        //}

        //public ActionResult AddressCustomer()
        //{
        //    if (HttpContext.Session.GetInt32("CustomerId") == null)
        //    {
        //        return Redirect("/dang-nhap.html?key=dn");
        //    }
        //    //CustomerLogin customerLogin = new CustomerLogin();
        //    //customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;

        //    //customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
        //    //customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
        //    //customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
        //    //customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
        //    //customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
        //    //customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
        //    //customerLogin.access_token = HttpContext.Session.GetString("access_token");
        //    //customerLogin.Sex = HttpContext.Session.GetString("CustomerSex");
        //    ViewBag.ActionMenu = 6;
        //    ViewBag.SeoTitle = "Địa chỉ khách hàng";
        //    ViewBag.SeoDescription = "Địa chỉ khách hàng";
        //    ViewBag.SeoKeywords = "Địa chỉ khách hàng";

        //    int customerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
        //    ViewBag.CustomerId = customerId;
        //    string token = HttpContext.Session.GetString("access_token");
        //    ViewBag.Token = "'" + token + "'";

        //    return View();
        //}

        //public ActionResult FollowOrder()
        //{
        //    var CustomerId = HttpContext.Session.GetInt32("CustomerId");
        //    if (CustomerId == null)
        //    {
        //        return Redirect("/dang-nhap.html?key=dn");
        //    }

        //    ViewBag.ActionMenu = 1;

        //    CustomerLogin customerLogin = new CustomerLogin();
        //    customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;

        //    customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
        //    customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
        //    customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
        //    customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
        //    customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
        //    customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
        //    customerLogin.access_token = HttpContext.Session.GetString("access_token");
        //    customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

        //    ViewBag.SeoTitle = "Theo dõi đơn hàng";
        //    ViewBag.SeoDescription = "Theo dõi đơn hàng";
        //    ViewBag.SeoKeywords = "Theo dõi đơn hàng";
        //    return View(customerLogin);
        //}


        //bool CheckLogin(string TenDangNhap, string MatKhau, bool remember = false)
        //{
        //    if (TenDangNhap != null && MatKhau != null)
        //    {
        //        using (var db = new IOITDataContext())
        //        {
        //            var data = db.Customer.Where(e => e.Username == TenDangNhap).ToList();
        //            if (data.Count() > 0)
        //            {
        //                string key = MatKhau + data.FirstOrDefault().KeyRandom;
        //                string MatKhauMd5 = Utils.GetMD5Hash(key);

        //                var users = (from p in db.Customer
        //                             where p.Username == TenDangNhap && p.Password == MatKhauMd5
        //                             select p
        //                            );

        //                if (users.Count() == 0)
        //                {
        //                    TempData["error"] = "Tên đăng nhập hoặc mật khẩu không đúng!";
        //                    return false;
        //                }

        //                if (users.First().Status == (int)Const.Status.LOCK)
        //                {
        //                    TempData["error"] = "Tài khoản của bạn bị khóa!";
        //                    return false;
        //                }

        //                if (users.First().IsEmailConfirm == false)
        //                {
        //                    TempData["error"] = "Tài khoản của bạn chưa được kích hoạt!";
        //                    return false;
        //                }

        //                if (remember)
        //                {

        //                    //var option = new CookieOptions();
        //                    //option.Expires = DateTime.Now.AddMinutes(10);
        //                    //Response.Cookies.Append("EmailEnviado", "true", option);
        //                    //var boh = Request.Cookies["EmailEnviado"];

        //                    //HttpCookie ckTenDangNhap = null;

        //                    //HttpCookie ckMatKhau = null;
        //                    SetCookie("userName", TenDangNhap, 15 * 24 * 60);
        //                    SetCookie("passWord", MatKhau, 15 * 24 * 60);

        //                    //ckTenDangNhap = new HttpCookie("U_TenDangNhap");
        //                    //ckMatKhau = new HttpCookie("U_MatKhau");

        //                    //ckTenDangNhap.Value = TenDangNhap;
        //                    //ckMatKhau.Value = MatKhau;


        //                    //ckTenDangNhap.Expires = DateTime.Today.AddDays(15);
        //                    //ckMatKhau.Expires = DateTime.Today.AddDays(15);

        //                    //Response.Cookies.Add(ckTenDangNhap);
        //                    //Response.Cookies.Add(ckMatKhau);
        //                }
        //                HttpContext.Session.SetString("U_TenDangNhap", users.First().Username);
        //                HttpContext.Session.SetInt32("U_Id", users.First().CustomerId);
        //                //Session["U_TenDangNhap"] = users.First().Username;
        //                //Session["U_Id"] = users.First().CustomerId;
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return false;
        //}

        public void SetCookie(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);

            Response.Cookies.Append(key, value, option);
        }

        public void RemoveCookie(string key)
        {
            Response.Cookies.Delete(key);
        }

        public ActionResult Register()
        {
            var CustomerId = HttpContext.Session.GetInt32("CustomerId");
            if (CustomerId != null)
            {
                return Redirect("/");
            }

            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "register-page customer-page";
            ViewBag.SeoTitle = "Đăng ký thành viên";
            ViewBag.SeoDescription = "Đăng ký thành viên";
            ViewBag.SeoKeywords = "Đăng ký thành viên";
            return View();
        }

        public ActionResult ListRegister()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "listregister-page customer-page";
            ViewBag.SeoTitle = "Danh sách Chiến Binh Khởi Nghiệp CASHPLUS";
            ViewBag.SeoDescription = "Danh sách Chiến Binh Khởi Nghiệp CASHPLUS";
            ViewBag.SeoKeywords = "Danh sách Chiến Binh Khởi Nghiệp CASHPLUS";
            return View();
        }

        public ActionResult ListMerchant()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "listregister-page customer-page";
            ViewBag.SeoTitle = "Danh sách Đối tác";
            ViewBag.SeoDescription = "Danh sách Đối tác";
            ViewBag.SeoKeywords = "Danh sách Đối tác";
            return View();
        }

        public ActionResult ProvincePartner()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "provincepartner-page customer-page";
            ViewBag.SeoTitle = "Đăng ký đại diện tỉnh thành CashPlus";
            ViewBag.SeoDescription = "Đăng ký đại diện tỉnh thành CashPlus";
            ViewBag.SeoKeywords = "Đăng ký đại diện tỉnh thành CashPlus";
            return View();
        }

        public ActionResult Dowloadnow()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "dowloadnow-page customer-page";
            ViewBag.SeoTitle = "Tải CashPlus ngay trên Android, iOS";
            ViewBag.SeoDescription = "Tải CashPlus ngay. Nền tảng tiêu dùng Hoàn Tiền ngay hàng đầu Việt Nam.";
            ViewBag.SeoKeywords = "Tải CashPlus, CashPlus trên Android, CashPlus trên iOS";
            return View();
        }


        public ActionResult DowloadMernow()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "dowloadnow-page customer-page";
            ViewBag.SeoTitle = "Tải CashPlus Merchant ngay trên Android, iOS";
            ViewBag.SeoDescription = "Tải CashPlus Merchant ngay. Nền tảng tiêu dùng Hoàn Tiền ngay hàng đầu Việt Nam.";
            ViewBag.SeoKeywords = "Tải CashPlus Merchant, CashPlus Merchant trên Android, CashPlus Merchant trên iOS";
            return View();
        }

        public ActionResult RegisterPartner()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "registerpartner-page registerpartner-page";
            ViewBag.SeoTitle = "Đăng ký đối tác CashPlus ";
            ViewBag.SeoDescription = "Đăng ký để trở thành đối tác trên CashPlus. Bạn không phải lo tìm khách hàng, CashPlus định tuyến và đưa khách hàng tới tận điểm kinh doanh của bạn.";
            ViewBag.SeoKeywords = "Đăng ký, đối tác, merchant, cashplus";
            return View();
        }
        public ActionResult RegisterPartnerStepthree()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "registerpartnerstepthree-page registerpartner-page";
            ViewBag.SeoTitle = "Xác nhận đăng ký đối tác CashPlus";
            ViewBag.SeoDescription = "Xác nhận đăng ký đối tác trên CashPlus. Bạn không phải lo tìm khách hàng, CashPlus định tuyến và đưa khách hàng tới tận điểm kinh doanh của bạn.";
            ViewBag.SeoKeywords = "Xác nhận, Đăng ký, đối tác, merchant, cashplus";
            return View();
        }
        public ActionResult PartnerUpdate()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "partnerupdate-page registerpartner-page";
            ViewBag.SeoTitle = "Đăng ký đối tác CashPlus";
            ViewBag.SeoDescription = "Đăng ký để trở thành đối tác trên CashPlus. Bạn không phải lo tìm khách hàng, CashPlus định tuyến và đưa khách hàng tới tận điểm kinh doanh của bạn.";
            ViewBag.SeoKeywords = "Đăng ký, đối tác, merchant, cashplus";
            return View();
        }
        public ActionResult Condition()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "condition-page customer-page";
            ViewBag.SeoTitle = "Điều khoản và điều kiện CashPlus";
            ViewBag.SeoDescription = "Điều khoản và điều kiện CashPlus";
            ViewBag.SeoKeywords = "Điều khoản và điều kiện";
            return View();
        }
        public ActionResult CashplusInfo()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "cashplusinfo-page customer-page";
            ViewBag.SeoTitle = "Về CashPlus - Ứng dụng Hoàn Tiền tiêu dùng thông minh";
            ViewBag.SeoDescription = "Hoàn tiền tiêu dùng ngay lập tức vào tài khoản ngân hàng; Gia tăng thu nhập thụ động hàng tháng cho bạn; ... Tất cả có trên CashPlus!";
            ViewBag.SeoKeywords = "CashPlus, ứng dụng, hoàn tiền, thông minh";
            return View();
        }
        public ActionResult InfoQR()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "InfoQR-page InfoQR-page";
            ViewBag.SeoTitle = "Chào mừng bạn đến với nền tảng CashPlus - Tiêu thoải mái Hoàn Tiền Ngay";
            ViewBag.SeoDescription = "Hoàn tiền tiêu dùng ngay lập tức vào tài khoản ngân hàng; Gia tăng thu nhập thụ động hàng tháng cho bạn; ... Tất cả có trên CashPlus!";
            ViewBag.SeoKeywords = "chia sẻ, affiliate, cashplus, đăng ký, giới thiệu";
            return View();
        }
        public ActionResult Affiliate()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "Affiliate-page Affiliate-page";
            ViewBag.SeoTitle = "Chào mừng bạn đến với nền tảng CashPlus - Tiêu thoải mái Hoàn Tiền Ngay";
            ViewBag.SeoDescription = "Hoàn tiền tiêu dùng ngay lập tức vào tài khoản ngân hàng; Gia tăng thu nhập thụ động hàng tháng cho bạn; ... Tất cả có trên CashPlus!";
            ViewBag.SeoKeywords = "chia sẻ, affiliate, cashplus, đăng ký, giới thiệu";
            return View();
        }
        public ActionResult RegisterCode()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "RegisterCode-page RegisterCode-page";
            ViewBag.SeoTitle = "Đăng ký tài khoản CashPlus - Tiêu thoải mái Hoàn Tiền Ngay";
            ViewBag.SeoDescription = "Hoàn tiền tiêu dùng ngay lập tức vào tài khoản ngân hàng. Gia tăng thu nhập thụ động hàng tháng cho bạn. Tất cả có trên CashPlus!";
            ViewBag.SeoKeywords = "Đăng ký, CashPlus, tài khoản, hoàn tiền ngay, nền tảng";
            return View();
        }
        //su kien
        public ActionResult revenueGrowth()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "RevenueGrowth-page RevenueGrowth-page";
            ViewBag.SeoTitle = "Hỗ trợ tăng trưởng doanh thu - CashPlus";
            ViewBag.SeoDescription = "Hoàn tiền tiêu dùng ngay lập tức vào tài khoản ngân hàng. Gia tăng thu nhập thụ động hàng tháng cho bạn. Tất cả có trên CashPlus!";
            ViewBag.SeoKeywords = "Đăng ký, CashPlus, tài khoản, hoàn tiền ngay, nền tảng";
            return View();
        }

        //Xử lý đăng ký OTP Test
        public ActionResult RecoverOTP()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "RecoverOTP-page RecoverOTP-page";
            ViewBag.SeoTitle = "Đăng ký tài khoản CashPlus - Tiêu thoải mái Hoàn Tiền Ngay";
            ViewBag.SeoDescription = "Hoàn tiền tiêu dùng ngay lập tức vào tài khoản ngân hàng. Gia tăng thu nhập thụ động hàng tháng cho bạn. Tất cả có trên CashPlus!";
            ViewBag.SeoKeywords = "Đăng ký, CashPlus, tài khoản, hoàn tiền ngay, nền tảng";
            return View();
        }
        public ActionResult RecoverPhoneOTP()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "RecoverPhoneOTP-page RecoverPhoneOTP-page";
            ViewBag.SeoTitle = "Thay đổi số điện thoại - CashPlus";
            ViewBag.SeoDescription = "Hoàn tiền tiêu dùng ngay lập tức vào tài khoản ngân hàng. Gia tăng thu nhập thụ động hàng tháng cho bạn. Tất cả có trên CashPlus!";
            ViewBag.SeoKeywords = "Đăng ký, CashPlus, tài khoản, hoàn tiền ngay, nền tảng";
            return View();
        }
        public ActionResult RecoverPassOTP()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "RecoverPassOTP-page RecoverPassOTP-page";
            ViewBag.SeoTitle = "Thay đổi mật khẩu - CashPlus";
            ViewBag.SeoDescription = "Hoàn tiền tiêu dùng ngay lập tức vào tài khoản ngân hàng. Gia tăng thu nhập thụ động hàng tháng cho bạn. Tất cả có trên CashPlus!";
            ViewBag.SeoKeywords = "Đăng ký, CashPlus, tài khoản, hoàn tiền ngay, nền tảng";
            return View();
        }
        public ActionResult RecoverSecurityOTP()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "RecoverSecurityOTP-page RecoverSecurityOTP-page";
            ViewBag.SeoTitle = "Quên mã bảo mật - CashPlus";
            ViewBag.SeoDescription = "Hoàn tiền tiêu dùng ngay lập tức vào tài khoản ngân hàng. Gia tăng thu nhập thụ động hàng tháng cho bạn. Tất cả có trên CashPlus!";
            ViewBag.SeoKeywords = "Đăng ký, CashPlus, tài khoản, hoàn tiền ngay, nền tảng";
            return View();
        }
        public ActionResult RecoverChangeSecurityOTP()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "RecoverChangeSecurityOTP-page RecoverChangeSecurityOTP-page";
            ViewBag.SeoTitle = "Thay đổi mã bảo mật - CashPlus";
            ViewBag.SeoDescription = "Hoàn tiền tiêu dùng ngay lập tức vào tài khoản ngân hàng. Gia tăng thu nhập thụ động hàng tháng cho bạn. Tất cả có trên CashPlus!";
            ViewBag.SeoKeywords = "Đăng ký, CashPlus, tài khoản, hoàn tiền ngay, nền tảng";
            return View();
        }
        public ActionResult RecoverSetupSecurityOTP()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "RecoverSetupSecurityOTP-page RecoverSetupSecurityOTP-page";
            ViewBag.SeoTitle = "Cài đặt mã bảo mật - CashPlus";
            ViewBag.SeoDescription = "Hoàn tiền tiêu dùng ngay lập tức vào tài khoản ngân hàng. Gia tăng thu nhập thụ động hàng tháng cho bạn. Tất cả có trên CashPlus!";
            ViewBag.SeoKeywords = "Đăng ký, CashPlus, tài khoản, hoàn tiền ngay, nền tảng";
            return View();
        }
        //Kết thúc đăng ký OTP Test
        public ActionResult PrivacyPolicy()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "privacypolicy-page customer-page";
            ViewBag.SeoTitle = "Chính sách bảo mật trên CashPlus";
            ViewBag.SeoDescription = "Chính sách bảo mật trên CashPlus";
            ViewBag.SeoKeywords = "Chính sách bảo mật";
            return View();
        }

        public ActionResult DetailCashplus()
        {
            var CustomerId = HttpContext.Session.GetInt32("CustomerId");
            if (CustomerId != null)
            {
                return Redirect("/");
            }

            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "cashplus-page customer-page";
            ViewBag.SeoTitle = "Chiến Binh Khởi Nghiệp CashPlus";
            ViewBag.SeoDescription = "Chiến Binh Khởi Nghiệp CashPlus";
            ViewBag.SeoKeywords = "Chiến Binh Khởi Nghiệp CashPlus";
            return View();
        }

        public ActionResult DetailUser()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "listregister-page customer-page";
            ViewBag.SeoTitle = "Chiến Binh Khởi Nghiệp CashPlus";
            ViewBag.SeoDescription = "Chiến Binh Khởi Nghiệp CashPlus";
            ViewBag.SeoKeywords = "Chiến Binh Khởi Nghiệp CashPlus";
            return View();
        }
        public ActionResult DetailTeam()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "detailteam-page customer-page";
            ViewBag.SeoTitle = "Chi tiết nhóm";
            ViewBag.SeoDescription = "Chi tiết nhóm";
            ViewBag.SeoKeywords = "Chi tiết nhóm";
            return View();
        }
        public ActionResult Contact()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "contact-page contact-page";
            ViewBag.SeoTitle = "Liên hệ - CashPlus";
            ViewBag.SeoDescription = "Liên hệ - CashPlus";
            ViewBag.SeoKeywords = "Liên hệ - CashPlus";
            return View();
        }
        public ActionResult RegisterInfo()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "register-page customer-page";
            ViewBag.SeoTitle = "Đăng ký tham gia chương trình Chiến Binh Khởi Nghiệp CashPlus";
            ViewBag.SeoDescription = "Đăng ký tham gia chương trình Chiến Binh Khởi Nghiệp CashPlus";
            ViewBag.SeoKeywords = "Đăng ký tham gia chương trình Chiến Binh Khởi Nghiệp CashPlus";
            return View();
        }
        public ActionResult DetailInfo(string idmerchant)
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "detailinfo-page customer-page";
            ViewBag.SeoTitle = "Chi tiết Merchant";
            ViewBag.SeoDescription = "Chi tiết đối tác";
            ViewBag.SeoKeywords = "Chi tiết đối tác, Chi tiết merchant";
            return View("infodetail");
        }

        public ActionResult News()
        {
            HttpContext.Session.SetString("current_url", HttpContext.Request.Headers["Referer"].ToString());
            ViewBag.Class = "news-page customer-page";
            ViewBag.SeoTitle = "Chi tiết bài viết";
            ViewBag.SeoDescription = "Chi tiết bài viết";
            ViewBag.SeoKeywords = "Chi tiết bài viết";
            return View();
        }

        public ActionResult ConfirmRegister(string key, int id)
        {
            ViewBag.SeoTitle = "Xác nhận đăng ký thành viên";
            ViewBag.SeoDescription = "Xác nhận đăng ký thành viên";
            ViewBag.SeoKeywords = "Xác nhận đăng ký thành viên";
            ViewBag.Class = "ConfirmRegister-page customer-page";
            using (var db = new IOITDataContext())
            {
                Customer customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                if (customer != null)
                {
                    if (Utils.GetMD5Hash(customer.KeyRandom) == key)
                    {
                        ViewBag.FullName = customer.FullName;
                        ViewBag.Email = customer.Email;
                    }
                    else
                    {
                        return Redirect("/Home/Error");
                    }
                }
                else
                {
                    return Redirect("/Home/Error");
                }

            }

            return View();
        }

        public ActionResult ConfirmEmailRegister(string key, int id)
        {
            ViewBag.SeoTitle = "Xác nhận email đăng ký thành viên";
            ViewBag.SeoDescription = "Xác nhận email nhận đăng ký thành viên";
            ViewBag.SeoKeywords = "Xác nhận email nhận đăng ký thành viên";
            ViewBag.Class = "ConfirmEmail-page customer-page";
            using (var db = new IOITDataContext())
            {
                Customer customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                if (customer != null)
                {
                    if (Utils.GetMD5Hash(customer.KeyRandom + customer.Password) == key)
                    {
                        ViewBag.FullName = customer.FullName;
                        ViewBag.Email = customer.Email;
                        customer.IsEmailConfirm = true;
                        db.Entry(customer).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        return Redirect("/Home/Error");
                    }
                }
                else
                {
                    return Redirect("/Home/Error");
                }

            }

            return View();
        }

        public ActionResult Logout()
        {
            //ckTenDangNhap = new HttpCookie("U_TenDangNhap");
            //ckMatKhau = new HttpCookie("U_MatKhau");

            //ckTenDangNhap.Value = "";
            //ckMatKhau.Value = "";

            //ckTenDangNhap.Expires = DateTime.Today.AddDays(-1);
            //ckMatKhau.Expires = DateTime.Today.AddDays(-1);
            //SetCookie("userName", "", -1);
            //SetCookie("passWord", "", -1);
            //RemoveCookie("userName");
            //RemoveCookie("passWord");

            //Response.Cookies.Add(ckTenDangNhap);
            //Response.Cookies.Add(ckMatKhau);
            //HttpContext.Session.SetString("U_TenDangNhap", null);
            //HttpContext.Session.SetString("U_Id", null);
            //Session["U_TenDangNhap"] = null;
            //Session["U_Id"] = null;
            //TempData["info"] = "Bạn đã đăng xuất thành công!";

            //HttpContext.Session.Clear();
            HttpContext.Session.Remove("CustomerId");
            HttpContext.Session.Remove("CustomerEmail");
            HttpContext.Session.Remove("CustomerFullName");
            HttpContext.Session.Remove("CustomerAvata");
            HttpContext.Session.Remove("CustomerAddress");
            HttpContext.Session.Remove("CustomerPassword");
            HttpContext.Session.Remove("CustomerPhoneNumber");
            HttpContext.Session.Remove("access_token");
            HttpContext.Session.Remove("CustomerSex");
            return Redirect("/dang-nhap");
        }

        public IActionResult RecoverPassword()
        {
            ViewBag.SeoTitle = "Cấp lại mật khẩu";
            ViewBag.SeoDescription = "Cấp lại mật khẩu";
            ViewBag.SeoKeywords = "Cấp lại mật khẩu";
            ViewBag.Class = "RecoverPassword-page customer-page";
            return View();
        }

        public ActionResult UserAuthentication(string username)
        {
            using (var db = new IOITDataContext())
            {
                var user = db.Customer.Where(e => e.Username == username).First();
                ViewBag.SeoTitle = "Xác thực tài khoản " + user.FullName;
                ViewBag.SeoDescription = "Xác thực tài khoản " + user.FullName;
                ViewBag.SeoKeywords = "Xác thực tài khoản " + user.FullName;
                ViewBag.Class = "UserAuthentication-page customer-page";
                return View(user);
            }
        }

        public ActionResult ChangePass()
        {
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }
            if (HttpContext.Session.GetInt32("TypeThirdId") != null)
            {
                if (HttpContext.Session.GetInt32("TypeThirdId") == (int)Const.TypeThird.CUSTOMER_KEYLOCK)
                    return Redirect("/dang-nhap");
            }
            ViewBag.SeoTitle = "Đổi mật khẩu";
            ViewBag.SeoDescription = "Đổi mật khẩu";
            ViewBag.SeoKeywords = "Đổi mật khẩu";
            ViewBag.Class = "ChangePass-page customer-page";
            ViewBag.ActionMenu = 2;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }

        public ActionResult InfoUser(string user)
        {
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }
            ViewBag.ActionMenu = 3;

            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;


            ViewBag.SeoTitle = "Thông tin khách hàng";
            ViewBag.SeoDescription = "Thông tin khách hàng";
            ViewBag.SeoKeywords = "Thông tin khách hàng";
            ViewBag.Class = "InfoUser-page customer-page";
            return View(customerLogin);
        }

        public ActionResult ThankYouPage()
        {
            ViewBag.SeoTitle = "Cảm ơn!!!";
            ViewBag.SeoDescription = "Cảm ơn!!!";
            ViewBag.SeoKeywords = "Cảm ơn!!!";
            ViewBag.Class = "ThankYouPage-page customer-page";
            return View();
        }
        public ActionResult ManageUser()
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.View = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.VIEW);
                ViewBag.Create = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.CREATE);
                ViewBag.Update = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.UPDATE);
                ViewBag.Delete = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.DELETED);
            }
            else
            {
                ViewBag.View = false;
                ViewBag.Create = false;
                ViewBag.Update = false;
                ViewBag.Delete = false;
            }
            string title = "Người dùng tổ chức";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            ViewBag.Class = "ManageData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 10;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }
        public ActionResult AddCustomerPage()
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.View = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.VIEW);
                ViewBag.Create = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.CREATE);
                ViewBag.Update = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.UPDATE);
                ViewBag.Delete = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.DELETED);
            }
            else
            {
                ViewBag.View = false;
                ViewBag.Create = false;
                ViewBag.Update = false;
                ViewBag.Delete = false;
            }
            string title = "Tạo mới người dùng";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            ViewBag.Class = "ManageData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 10;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }

        public ActionResult ViewCustomerPage(int id)
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.View = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.VIEW);
                ViewBag.Create = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.CREATE);
                ViewBag.Update = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.UPDATE);
                ViewBag.Delete = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.DELETED);
            }
            else
            {
                ViewBag.View = false;
                ViewBag.Create = false;
                ViewBag.Update = false;
                ViewBag.Delete = false;
            }
            ViewBag.UserId = id;
            string title = "Xem chi tiết người dùng";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            ViewBag.Class = "ManageData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 10;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }

        public ActionResult UpdateCustomerPage(int id)
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.View = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.VIEW);
                ViewBag.Create = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.CREATE);
                ViewBag.Update = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.UPDATE);
                ViewBag.Delete = CheckRole.CheckRoleByCode(access_key, functionCodeNDTC, (int)Const.Action.DELETED);
            }
            else
            {
                ViewBag.View = false;
                ViewBag.Create = false;
                ViewBag.Update = false;
                ViewBag.Delete = false;
            }
            ViewBag.UserId = id;
            string title = "Cập nhật người dùng";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            ViewBag.Class = "ManageData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 10;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }

        public ActionResult ManageUnit()
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.View = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.VIEW);
                ViewBag.Create = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.CREATE);
                ViewBag.Update = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.UPDATE);
                ViewBag.Delete = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.DELETED);
            }
            else
            {
                ViewBag.View = false;
                ViewBag.Create = false;
                ViewBag.Update = false;
                ViewBag.Delete = false;
            }
            string title = "Danh sách tổ chức";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            ViewBag.Class = "ManageData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 9;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }
        public ActionResult AddCompanyPage()
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.View = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.VIEW);
                ViewBag.Create = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.CREATE);
                ViewBag.Update = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.UPDATE);
                ViewBag.Delete = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.DELETED);
            }
            else
            {
                ViewBag.View = false;
                ViewBag.Create = false;
                ViewBag.Update = false;
                ViewBag.Delete = false;
            }
            string title = "Tạo mới cơ quan/tổ chức";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            ViewBag.Class = "ManageData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 9;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }

        public ActionResult ViewCompanyPage(int id)
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.View = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.VIEW);
                ViewBag.Create = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.CREATE);
                ViewBag.Update = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.UPDATE);
                ViewBag.Delete = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.DELETED);
            }
            else
            {
                ViewBag.View = false;
                ViewBag.Create = false;
                ViewBag.Update = false;
                ViewBag.Delete = false;
            }
            ViewBag.UnitId = id;
            string title = "Xem cơ quan/tổ chức";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            ViewBag.Class = "ManageData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 9;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }

        public ActionResult UpdateCompanyPage(int id)
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.View = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.VIEW);
                ViewBag.Create = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.CREATE);
                ViewBag.Update = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.UPDATE);
                ViewBag.Delete = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.DELETED);
            }
            else
            {
                ViewBag.View = false;
                ViewBag.Create = false;
                ViewBag.Update = false;
                ViewBag.Delete = false;
            }
            ViewBag.UnitId = id;
            string title = "Cập nhật cơ quan/tổ chức";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            ViewBag.Class = "ManageData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 9;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }

        public ActionResult SettingPassword(string key, long id)
        {
            string title = "Xác thực tài khoản";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            ViewBag.Class = "SettingPassword-page customer-page";

            using (var db = new IOITDataContext())
            {
                Customer customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                if (customer != null)
                {
                    if (Utils.GetMD5Hash(customer.KeyRandom + customer.Password) == key)
                    {
                        ViewBag.CustomerId = id;
                        ViewBag.Email = customer.Username;
                        return View();
                    }
                    else
                    {
                        return Redirect("/Home/Error");
                    }
                }
                else
                {
                    return Redirect("/Home/Error");
                }

            }
        }

        public ActionResult ResetPassword(string key, long id)
        {
            string title = "Thiết lập mật khẩu";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            ViewBag.Class = "SettingPassword-page customer-page";

            using (var db = new IOITDataContext())
            {
                Customer customer = db.Customer.Where(c => c.CustomerId == id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                if (customer != null)
                {
                    if (Utils.GetMD5Hash(customer.KeyRandom + customer.Password) == key)
                    {
                        ViewBag.CustomerId = id;
                        ViewBag.Email = customer.Username;
                        return View();
                    }
                    else
                    {
                        return Redirect("/Home/Error");
                    }
                }
                else
                {
                    return Redirect("/Home/Error");
                }

            }
        }

        public ActionResult InfoUserPage()
        {
            ViewBag.SeoTitle = "Thông tin cá nhân";
            ViewBag.SeoDescription = "Thông tin cá nhân";
            ViewBag.SeoKeywords = "Thông tin cá nhân";
            ViewBag.Class = "InfoUserPage-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 2;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }
        public ActionResult InfoGeneralPage()
        {
            ViewBag.SeoTitle = "Thông tin chung";
            ViewBag.SeoDescription = "Thông tin chung";
            ViewBag.SeoKeywords = "Thông tin chung";
            ViewBag.Class = "InfoGeneralPage-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 2;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }
        public ActionResult AccountInformation()
        {
            ViewBag.SeoTitle = "Thông tin tài khoản";
            ViewBag.SeoDescription = "Thông tin tài khoản";
            ViewBag.SeoKeywords = "Thông tin tài khoản";
            ViewBag.Class = "AccountInformation-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 2;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.Status = HttpContext.Session.GetString("Status") != null ? int.Parse(HttpContext.Session.GetString("Status")) : 10;
            customerLogin.CreatedAtStr = HttpContext.Session.GetString("CreatedAt") != null ?
                DateTime.Parse(HttpContext.Session.GetString("CreatedAt")).ToString("dd/MM/yyyy") : "";
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;
            customerLogin.TypeThirdId = HttpContext.Session.GetInt32("TypeThirdId") != null ? (int)HttpContext.Session.GetInt32("TypeThirdId") : 1;

            return View(customerLogin);
        }
        public ActionResult DataAuthor()
        {
            ViewBag.SeoTitle = "Dữ liệu của tác giả";
            ViewBag.SeoDescription = "Dữ liệu của tác giả";
            ViewBag.SeoKeywords = "Dữ liệu của tác giả";
            ViewBag.Class = "DataAuthor-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 4;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }
        public ActionResult ManageData()
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.View = CheckRole.CheckRoleByCode(access_key, functionCodeDSDL, (int)Const.Action.VIEW);
                ViewBag.Create = CheckRole.CheckRoleByCode(access_key, functionCodeDSDL, (int)Const.Action.CREATE);
                ViewBag.Update = CheckRole.CheckRoleByCode(access_key, functionCodeDSDL, (int)Const.Action.UPDATE);
                ViewBag.Delete = CheckRole.CheckRoleByCode(access_key, functionCodeDSDL, (int)Const.Action.DELETED);
                ViewBag.UpdatePrivate = CheckRole.CheckRoleByCode(access_key, functionCodeDSDL, (int)Const.Action.UPDATE);
                ViewBag.UpdatePublish = CheckRole.CheckRoleByCode(access_key, functionCodeDSDL, (int)Const.Action.UPDATE);
                ViewBag.CreateCeph = CheckRole.CheckRoleByCode(access_key, functionCodeCEPH, (int)Const.Action.CREATE);
                //check xem có quản lý tổ chức ko
                ViewBag.RoleQLTC = CheckRole.CheckRoleByCode(access_key, functionCodeDSTC, (int)Const.Action.VIEW);
            }
            else
            {
                ViewBag.View = false;
                ViewBag.Create = false;
                ViewBag.Update = false;
                ViewBag.Delete = false;
                ViewBag.UpdatePrivate = false;
                ViewBag.UpdatePublish = false;
                ViewBag.CreateCeph = false;
                ViewBag.RoleQLTC = false;
            }

            ViewBag.SeoTitle = "Quản lý dữ liệu";
            ViewBag.SeoDescription = "Quản lý dữ liệu";
            ViewBag.SeoKeywords = "Quản lý dữ liệu";
            ViewBag.Class = "ManageData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 5;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }
        public ActionResult CreateNewData()
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.Create = CheckRole.CheckRoleByCode(access_key, functionCodeDSDL, (int)Const.Action.CREATE);
            }
            else
            {
                ViewBag.Create = false;
            }
            ViewBag.SeoTitle = "Tạo mới dữ liệu";
            ViewBag.SeoDescription = "Tạo mới dữ liệu";
            ViewBag.SeoKeywords = "Tạo mới dữ liệu";
            ViewBag.Class = "CreateNewData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 5;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }
        public ActionResult CreateDataFromCeph()
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.Create = CheckRole.CheckRoleByCode(access_key, functionCodeDSDL, (int)Const.Action.CREATE);
            }
            else
            {
                ViewBag.Create = false;
            }
            string title = "Tạo mới dữ liệu từ CEPH";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            ViewBag.Class = "CreateNewData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 5;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }
        public ActionResult UpdateData(long id)
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.Update = CheckRole.CheckRoleByCode(access_key, functionCodeDSDL, (int)Const.Action.UPDATE);
            }
            else
            {
                ViewBag.Update = false;
            }
            ViewBag.SeoTitle = "Cập nhật dữ liệu";
            ViewBag.SeoDescription = "Cập nhật dữ liệu";
            ViewBag.SeoKeywords = "Cập nhật dữ liệu";
            //ViewBag.Class = "UpdateData-page customer-page";
            ViewBag.Class = "CreateNewData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 5;
            ViewBag.DataSetId = id;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }
        public ActionResult ViewData(long id)
        {
            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.View = CheckRole.CheckRoleByCode(access_key, functionCodeDSDL, (int)Const.Action.VIEW);
                ViewBag.UpdatePrivate = CheckRole.CheckRoleByCode(access_key, functionCodeDNB, (int)Const.Action.UPDATE);
                ViewBag.UpdatePublish = CheckRole.CheckRoleByCode(access_key, functionCodeDCK, (int)Const.Action.UPDATE);
            }
            else
            {
                ViewBag.View = false;
                ViewBag.UpdatePrivate = false;
                ViewBag.UpdatePublish = false;
            }
            ViewBag.SeoTitle = "Chi tiết dữ liệu";
            ViewBag.SeoDescription = "Chi tiết dữ liệu";
            ViewBag.SeoKeywords = "Chi tiết dữ liệu";
            ViewBag.Class = "ViewData-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 5;
            ViewBag.DataSetId = id;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }
        public ActionResult ManageNoitify()
        {
            ViewBag.SeoTitle = "Quản lý thông báo";
            ViewBag.SeoDescription = "Quản lý thông báo";
            ViewBag.SeoKeywords = "Quản lý thông báo";
            ViewBag.Class = "ManageNoitify-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 7;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }
        public ActionResult SettingNoitify()
        {
            ViewBag.SeoTitle = "Cài đặt thông báo";
            ViewBag.SeoDescription = "Cài đặt thông báo";
            ViewBag.SeoKeywords = "Cài đặt thông báo";
            ViewBag.Class = "SettingNoitify-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 8;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }
        public async Task<ActionResult> ViewNoitify(Guid id)
        {
            ViewBag.SeoTitle = "Chi tiết thông báo";
            ViewBag.SeoDescription = "Chi tiết thông báo";
            ViewBag.SeoKeywords = "Chi tiết thông báo";
            ViewBag.Class = "ManageNoitify-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.ActionMenu = 7;
            //CustomerLogin customerLogin = new CustomerLogin();
            //customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            //customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            //customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            //customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            //customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            //customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            //customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            //customerLogin.access_token = HttpContext.Session.GetString("access_token");
            //customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;
            using (var db = new IOITDataContext())
            {
                var data = await db.Notification.Where(e => e.NotificationId == id).FirstOrDefaultAsync();
                if (data != null)
                {
                    if (data.Status == (int)Const.Status.TEMP)
                    {
                        data.Status = (byte)Const.Status.NORMAL;
                        data.UpdatedAt = DateTime.Now;
                        db.Notification.Update(data);
                        await db.SaveChangesAsync();
                        int notification = HttpContext.Session.GetInt32("NunberNotification") != null ? (int)HttpContext.Session.GetInt32("NunberNotification") : 0;
                        if (notification > 0)
                        {
                            notification = notification - 1;
                            HttpContext.Session.SetInt32("NunberNotification", notification);
                        }
                    }
                }

                return View(data);
            }
        }

        public ActionResult ManageLink()
        {
            string title = "Liên kết dữ liệu";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            ViewBag.Class = "SettingNoitify-page customer-page";
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return Redirect("/dang-nhap");
            }

            ViewBag.LinkCeph = _configuration["ManagerLink:ceph"];
            ViewBag.LinkRancher = _configuration["ManagerLink:rancher"];
            ViewBag.LinkKuberflow = _configuration["ManagerLink:kuberflow"];
            ViewBag.LinkSparkWorker = _configuration["ManagerLink:sparkWorker"];
            ViewBag.LinkSparkMaster = _configuration["ManagerLink:sparkMaster"];
            ViewBag.LinkNifi = _configuration["ManagerLink:nifi"];
            ViewBag.LinkAirByte = _configuration["ManagerLink:airByte"];
            ViewBag.IsIframe = bool.Parse(_configuration["ManagerLink:isIframe"]);

            string access_key = HttpContext.Session.GetString("access_key");
            if (access_key != null && access_key != "")
            {
                ViewBag.ViewKuberflow = CheckRole.CheckRoleByCode(access_key, "LKKF", (int)Const.Action.VIEW);
                ViewBag.ViewSparkWorker = CheckRole.CheckRoleByCode(access_key, "LKSW", (int)Const.Action.VIEW);
                ViewBag.ViewSparkMaster = CheckRole.CheckRoleByCode(access_key, "LKSM", (int)Const.Action.VIEW);
                ViewBag.ViewNifi = CheckRole.CheckRoleByCode(access_key, "LKNF", (int)Const.Action.VIEW);
                ViewBag.ViewAirByte = CheckRole.CheckRoleByCode(access_key, "LKAB", (int)Const.Action.VIEW);
            }
            else
            {
                ViewBag.ViewKuberflow = false;
                ViewBag.ViewSparkWorker = false;
                ViewBag.ViewSparkMaster = false;
                ViewBag.ViewNifi = false;
                ViewBag.ViewAirByte = false;
            }

            ViewBag.ActionMenu = 10;
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            customerLogin.access_token = HttpContext.Session.GetString("access_token");
            customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;

            return View(customerLogin);
        }



    }

}


