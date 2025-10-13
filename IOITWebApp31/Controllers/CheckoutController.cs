using IOITWebApp31.Models;
using IOITWebApp31.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IOITWebApp31.Controllers
{
    public class CheckoutController : Controller
    {
        private IHttpContextAccessor _accessor;

        public CheckoutController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        // GET: /Checkout/
        public ActionResult Index()
        {
            var CustomerId = HttpContext.Session.GetInt32("CustomerId");
            if (CustomerId == null)
            {
                return Redirect("/Home/Error");
            }
            ViewBag.IP = "'" + _accessor.HttpContext.Connection.RemoteIpAddress.ToString() + "'";

            ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
            if (ViewBag.LanguageId == "1007")
            {
                ViewBag.SeoTitle = "Make a purchase";
                ViewBag.SeoDescription = "Make a purchase";
                ViewBag.SeoKeywords = "Make a purchase";
            }
            else
            {
                ViewBag.SeoTitle = "Thực hiện mua hàng";
                ViewBag.SeoDescription = "Thực hiện mua hàng";
                ViewBag.SeoKeywords = "Thực hiện mua hàng";
            }
            return View();
        }

        public ActionResult ShippingAddress()
        {
            //var CustomerId = HttpContext.Session.GetInt32("CustomerId");
            //if (CustomerId == null)
            //{
            //    return Redirect("/dang-nhap.html");
            //}

            //CustomerLogin customerLogin = new CustomerLogin();
            //customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            //customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
            //customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
            //customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
            //customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
            //customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
            //customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
            //customerLogin.access_token = HttpContext.Session.GetString("access_token");
            //customerLogin.Sex = HttpContext.Session.GetString("CustomerSex");

            ViewBag.IP = "'" + _accessor.HttpContext.Connection.RemoteIpAddress.ToString() + "'";
            ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
            if (ViewBag.LanguageId == "1007")
            {
                ViewBag.SeoTitle = "Address of purchase";
                ViewBag.SeoDescription = "Address of purchase";
                ViewBag.SeoKeywords = "Address of purchase";
            }
            else
            {
                ViewBag.SeoTitle = "Địa chỉ mua hàng";
                ViewBag.SeoDescription = "Địa chỉ mua hàng";
                ViewBag.SeoKeywords = "Địa chỉ mua hàng";
            }

            int customerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            ViewBag.CustomerId = customerId;
            string token = HttpContext.Session.GetString("access_token");
            ViewBag.Token = "'" + token + "'";

            ShoppingCart objCart = Models.Common.SessionExtensions.GetObject<ShoppingCart>(HttpContext.Session, "Cart");
            if (objCart == null)
            {
                objCart = new ShoppingCart();
            }

            ViewBag.TotalItem = objCart.ListItem.Count();
            ViewBag.TotalPrice = objCart.ListItem.Sum(e => e.Total).HasValue ? objCart.ListItem.Sum(e => e.Total).Value : 0;
            ViewBag.TotalPrice = "'" + ViewBag.TotalPrice + "'";
            return View(objCart);
        }

        public ActionResult Payments()
        {

            ViewBag.IP = "'" + _accessor.HttpContext.Connection.RemoteIpAddress.ToString() + "'";
            ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
            if (ViewBag.LanguageId == "1007")
            {
                ViewBag.SeoTitle = "Payments methods";
                ViewBag.SeoDescription = "Payments methods";
                ViewBag.SeoKeywords = "Payments methods";
            }
            else
            {
                ViewBag.SeoTitle = "Hình thức thanh toán";
                ViewBag.SeoDescription = "Hình thức thanh toán";
                ViewBag.SeoKeywords = "Hình thức thanh toán";
            }

            int customerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
            ViewBag.CustomerId = customerId;
            string token = HttpContext.Session.GetString("access_token");
            ViewBag.Token = "'" + token + "'";

            ShoppingCart objCart = Models.Common.SessionExtensions.GetObject<ShoppingCart>(HttpContext.Session, "Cart");
            if (objCart == null)
            {
                objCart = new ShoppingCart();
            }

            ViewBag.TotalItem = objCart.ListItem.Count();
            ViewBag.TotalPrice = objCart.ListItem.Sum(e => e.Total).HasValue ? objCart.ListItem.Sum(e => e.Total).Value : 0;
            ShoppingCartModels cart = new ShoppingCartModels();
            cart.Cart = objCart;
            return View(cart);
        }

        public ActionResult RePayments()
        {
            ViewBag.IP = "'" + _accessor.HttpContext.Connection.RemoteIpAddress.ToString() + "'";

            ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
            if (ViewBag.LanguageId == "1007")
            {
                ViewBag.SeoTitle = "Payments methods";
                ViewBag.SeoDescription = "Payments methods";
                ViewBag.SeoKeywords = "Payments methods";
            }
            else
            {
                ViewBag.SeoTitle = "Hình thức thanh toán";
                ViewBag.SeoDescription = "Hình thức thanh toán";
                ViewBag.SeoKeywords = "Hình thức thanh toán";
            }

            ShoppingCart objCart = Models.Common.SessionExtensions.GetObject<ShoppingCart>(HttpContext.Session, "Cart");
            if (objCart == null)
            {
                objCart = new ShoppingCart();
            }
            ViewBag.Cart = objCart;
            ViewBag.TotalItem = objCart.ListItem.Count();
            ViewBag.TotalPrice = objCart.ListItem.Sum(e => e.Total).HasValue ? objCart.ListItem.Sum(e => e.Total).Value : 0;

            return View();
        }

        public ActionResult OrderResults()
        {
            ViewBag.IP = "'" + _accessor.HttpContext.Connection.RemoteIpAddress.ToString() + "'";
            ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
            if (ViewBag.LanguageId == "1007")
            {
                ViewBag.SeoTitle = "Order results";
                ViewBag.SeoDescription = "Order results";
                ViewBag.SeoKeywords = "Order results";
            }
            else
            {
                ViewBag.SeoTitle = "Kết quả đơn hàng";
                ViewBag.SeoDescription = "Kết quả đơn hàng";
                ViewBag.SeoKeywords = "Kết quả đơn hàng";
            }

            ShoppingCart objCart = Models.Common.SessionExtensions.GetObject<ShoppingCart>(HttpContext.Session, "Cart");
            if (objCart == null)
            {
                objCart = new ShoppingCart();
            }
            ViewBag.Cart = objCart;
            ViewBag.TotalItem = objCart.ListItem.Count();
            ViewBag.TotalPrice = objCart.ListItem.Sum(e => e.Total).HasValue ? objCart.ListItem.Sum(e => e.Total).Value : 0;


            return View();
        }

        //public ActionResult OrderProduct(FormCollection collection)
        //{
        //    using (var db = new IOITDataContext())
        //    {
        //        string cart = HttpContext.Session.GetString("Cart");
        //        if (cart != null)
        //        {
        //            string email1 = "";// collection["Email"].Trim();
        //            string phone = collection["Mobile"];
        //            var cus = db.Customer.Where(e => e.Email.Trim() == email1 && e.Phone.Trim() == phone).ToList();
        //            int idCus = 0;
        //            string addCus = "";
        //            if (cus.Count <= 0)
        //            {
        //                //Thêm khách hàng
        //                Customer cm = new Customer();
        //                string code = "";
        //                do
        //                {
        //                    code = Utils.RandomString(8);
        //                } while (db.Customer.Where(e => e.KeyRandom == code).Count() > 0);

        //                //cm.Id = Public.GetID();
        //                //idCus = cm.Id;
        //                cm.Username = collection["Email"];
        //                cm.KeyRandom = code;
        //                string pass = "12345678" + code;
        //                cm.Password = Utils.GetMD5Hash(pass);
        //                cm.FullName = collection["Name"];
        //                cm.Email = collection["Email"];
        //                cm.Phone = phone;
        //                cm.Address = collection["Adress"];
        //                addCus = cm.Address;
        //                cm.Birthday = DateTime.Parse("01/01/1990");
        //                cm.CreatedAt = DateTime.Now;
        //                cm.IsEmailConfirm = true;
        //                cm.UpdatedAt = DateTime.Now;
        //                cm.LastLoginAt = DateTime.Now;
        //                cm.Status = (int)Const.Status.NORMAL;
        //                cm.Note = collection["CustomerOrderNote"];
        //                cm.Avata = "";
        //                cm.Sex = collection["SexCheckout"];
        //                db.Customer.Add(cm);
        //                db.SaveChanges();
        //            }
        //            else
        //            {
        //                idCus = cus.FirstOrDefault().CustomerId;
        //            }

        //            ShoppingCartModels sc = new ShoppingCartModels();
        //            //sc.Cart = (ShoppingCart)cart;

        //            //Thêm đơn hàng
        //            int count = 0;
        //            decimal total = 0;
        //            foreach (var item in sc.Cart.ListItem)
        //            {
        //                count += item.Quantity;
        //                total += (decimal)item.Total;
        //            }

        //            Order model = new Order();
        //            //model.Id = Public.GetID();
        //            model.CustomerId = idCus;
        //            //model.BillingAddress = collection["pmAddress"];
        //            model.CreatedAt = DateTime.Now;
        //            model.CustomerNote = collection["CustomerOrderNote"];
        //            model.OrderDiscount = count;
        //            model.OrderStatusId = (int)Const.Status.NORMAL;
        //            model.OrderTax = 0;
        //            model.OrderTotal = total;
        //            //model.NumberOrder = sc.Cart.ListItem.Count;
        //            //model.PaymentMethodId = int.Parse(collection["Payment"].Trim());
        //            //model.ShippingAddress = collection["spAddress"];
        //            //model.ShippingMethodId = int.Parse(collection["Shipping"].Trim());
        //            model.UpdatedAt = DateTime.Now;
        //            db.Order.Add(model);
        //            db.SaveChanges();

        //            //Thêm chi tiết đơn hàng
        //            foreach (var item in sc.Cart.ListItem)
        //            {
        //                OrderItem ot = new OrderItem();
        //                //ot.Id = Public.GetID();
        //                //ot.OrderId = model.Id;
        //                ot.Price = item.Price;
        //                ot.PriceDiscount = 0;
        //                ot.PriceTax = 0;
        //                ot.ProductId = item.ProductId;
        //                ot.Quantity = item.Quantity;
        //                db.OrderItem.Add(ot);
        //                db.SaveChanges();
        //            }

        //            var sm = db.Config.Find(1);
        //            string userName = sm.EmailUserName;
        //            string password = sm.EmailPasswordHash;
        //            string smtpHost = sm.EmailHost;
        //            int smtpPort = (int)sm.EmailPort;
        //            //thong tin email được gởi
        //            string toEmail = collection["Email"];//các liên hệ sẽ gởi về email này

        //            SentContact es = new SentContact();
        //            es.Content = "Chúc mừng khách hàng " + collection["Fullname"] + " đã đặt hàng thành công! Cửa hàng Tâm Đức sẽ liên hệ xác nhận đơn hàng của bạn trong thời gian sớm nhất, Xin cảm ơn!";
        //            es.Subject = "Thông tin đặt hàng";
        //            es.ToEmail = toEmail;

        //            string body = string.Format(es.Content);

        //            //EmailService email = new EmailService();

        //            bool result = EmailService.Send(userName, password, smtpHost, smtpPort, es.ToEmail, es.Subject, body);

        //            if (result)
        //            {
        //                TempData["success"] = "Chúc mừng bạn đã đặt hàng thành công!";
        //                return Redirect("/hoan-tat-dat-hang");
        //            }
        //            else
        //            {
        //                TempData["error"] = "Đặt hàng không thành công!";
        //                return Redirect("/");
        //            }
        //        }
        //        else
        //        {
        //            TempData["info"] = "Bạn đã hết phiên mua hàng!";
        //            return Redirect("/");
        //        }
        //    }
        //}

        //public ActionResult OrderProductFast(FormCollection collection)
        //{
        //    using (var db = new IOITDataContext())
        //    {
        //        string email1 = "";//collection["Email"].Trim();
        //        string mobile = "";//collection["Mobile"].Trim();

        //        var cus = db.Customer.Where(e => e.Email.Trim() == email1 && e.Phone == mobile).ToList();
        //        int idCus = 0;
        //        string addCus = "";
        //        if (cus.Count <= 0)
        //        {
        //            //Thêm khách hàng
        //            Customer cm = new Customer();
        //            string code = "";
        //            do
        //            {
        //                code = Utils.RandomString(8);
        //            } while (db.Customer.Where(e => e.KeyRandom == code).Count() > 0);

        //            //cm.Id = Public.GetID();
        //            //idCus = cm.Id;
        //            cm.Username = email1;
        //            cm.KeyRandom = code;
        //            string pass = "12345678" + code;
        //            cm.Password = Utils.GetMD5Hash(pass);
        //            cm.FullName = collection["Name"];
        //            cm.Email = email1;
        //            cm.Phone = mobile;
        //            cm.Address = collection["Adress"];
        //            addCus = cm.Address;
        //            cm.Birthday = DateTime.Parse("01/01/1990");
        //            cm.CreatedAt = DateTime.Now;
        //            cm.IsEmailConfirm = true;
        //            cm.UpdatedAt = DateTime.Now;
        //            cm.LastLoginAt = DateTime.Now;
        //            cm.Status = (int)Const.Status.NORMAL;
        //            cm.Note = "";
        //            cm.Avata = "";
        //            cm.Sex = "";
        //            db.Customer.Add(cm);
        //            db.SaveChangesAsync();
        //            idCus = cm.CustomerId;
        //        }
        //        else
        //        {
        //            idCus = cus.FirstOrDefault().CustomerId;
        //        }

        //        //Thêm đơn hàng
        //        Order model = new Order();
        //        //model.Id = Public.GetID();
        //        model.CustomerId = idCus;
        //        //model.BillingAddress = addCus;
        //        model.CreatedAt = DateTime.Now;
        //        model.CustomerNote = collection["CustomerOrderNote"];
        //        model.OrderDiscount = int.Parse(collection["OrderDiscount"]);
        //        model.OrderStatusId = (int)Const.Status.NORMAL;
        //        model.OrderTax = 0;
        //        model.OrderTotal = decimal.Parse(collection["Price"]);
        //        //model.NumberOrder = 1;
        //        model.PaymentMethodId = (int)Const.Status.NORMAL;
        //        //model.ShippingAddress = addCus;
        //        model.ShippingMethodId = (int)Const.Status.NORMAL;
        //        model.UpdatedAt = DateTime.Now;
        //        db.Order.Add(model);
        //        db.SaveChanges();

        //        //Thêm chi tiết đơn hàng
        //        OrderItem ot = new OrderItem();
        //        //ot.Id = Public.GetID();
        //        ot.OrderId = model.OrderId;
        //        ot.Price = decimal.Parse(collection["Price"]);
        //        ot.PriceDiscount = 0;
        //        ot.PriceTax = 0;
        //        ot.ProductId = int.Parse(collection["ProductId"]);
        //        ot.Quantity = int.Parse(collection["OrderDiscount"]);
        //        db.OrderItem.Add(ot);
        //        db.SaveChanges();

        //        var sm = db.Config.Find(1);
        //        string userName = sm.EmailUserName;
        //        string password = sm.EmailPasswordHash;
        //        string smtpHost = sm.EmailHost;
        //        int smtpPort = (int)sm.EmailPort;
        //        //thong tin email được gởi
        //        string toEmail = email1;//các liên hệ sẽ gởi về email này

        //        SentContact es = new SentContact();
        //        es.Content = "Chúc mừng khách hàng " + collection["Name"] + " đã đặt hàng thành công! Cửa hàng Tâm Đức sẽ liên hệ xác nhận đơn hàng của bạn trong thời gian sớm nhất, Xin cảm ơn!";
        //        es.Subject = "Thông tin đặt hàng";
        //        es.ToEmail = toEmail;

        //        string body = string.Format(es.Content);

        //        //EmailService email = new EmailService();

        //        bool result = EmailService.Send(userName, password, smtpHost, smtpPort, es.ToEmail, es.Subject, body);

        //        if (result)
        //        {
        //            TempData["success"] = "Chúc mừng bạn đã đặt hàng thành công!";
        //            return Redirect("/hoan-tat-dat-hang");
        //        }

        //        return Redirect("/hoan-tat-dat-hang");
        //    }
        //}


        //public ActionResult ComplateOrder(int id)
        //{
        //    var CustomerId = HttpContext.Session.GetInt32("CustomerId");
        //    if (CustomerId == null)
        //    {
        //        return Redirect("/Home/Error");
        //    }
        //    ViewBag.SeoTitle = "Hoàn tất mua hàng";
        //    ViewBag.SeoDescription = "Hoàn tất mua hàng";
        //    ViewBag.SeoKeywords = "Hoàn tất mua hàng";
        //    try
        //    {
        //        using (var db = new IOITDataContext())
        //        {
        //            var order = db.Order.Where(o => o.OrderId == id).FirstOrDefault();
        //            if(order != null)
        //            {
        //                OrderWebDTO data = new OrderWebDTO();
        //                data.OrderId = order.OrderId;
        //                data.Code = order.Code;
        //                data.CustomerId = order.CustomerId;
        //                //data.ReceiverName = order.ReceiverName;
        //                //data.ReceiverEmail = order.ReceiverEmail;
        //                //data.ReceiverPhone = order.ReceiverPhone;
        //                data.PaymentMethodId = order.PaymentMethodId;
        //                data.PaymentStatusId = order.PaymentStatusId;
        //                data.ShippingMethodId = order.ShippingMethodId;
        //                data.ShippingStatusId = order.ShippingStatusId;
        //                data.OrderStatusId = order.OrderStatusId;
        //                data.OrderTax = order.OrderTax;
        //                data.OrderDiscount = order.OrderDiscount;
        //                data.OrderTotal = order.OrderTotal;
        //                data.CustomerNote = order.CustomerNote;
        //                data.listOrderItem = new List<OrderItemDTO>();
        //                data.CreatedAt = order.CreatedAt;
        //                data.UpdatedAt = order.UpdatedAt;
        //                var listOrderItem = db.OrderItem.Where(oi => oi.OrderId == data.OrderId && oi.Status != (int)Const.Status.DELETED).ToList();
        //                if(listOrderItem != null)
        //                {
        //                    foreach(var item in listOrderItem)
        //                    {
        //                        OrderItemDTO orderItem = new OrderItemDTO();
        //                        orderItem.OrderItemId = item.OrderItemId;
        //                        orderItem.OrderId = item.OrderId;
        //                        orderItem.ProductId = item.ProductId;
        //                        orderItem.ProductName = db.Product.Where(p => p.ProductId == item.ProductId).FirstOrDefault().Name;
        //                        orderItem.Quantity = item.Quantity;
        //                        orderItem.Price = item.Price;
        //                        orderItem.PriceTax = item.PriceTax;
        //                        orderItem.PriceDiscount = item.PriceDiscount;
        //                        orderItem.PriceTotal = item.PriceTotal;

        //                        data.listOrderItem.Add(orderItem);
        //                    }
        //                }

        //                return View(data);
        //            }
        //            else
        //            {
        //                return Redirect("/Home/Error");
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        return Redirect("/Home/Error");
        //    }
        //}


        //public ActionResult LoadPaymentMethod()
        //{
        //    using (var db = new IOITDataContext())
        //    {
        //        var data = db.TypeAttributeItems.Where(e => e.TypeAttributeId == 1);
        //        return PartialView("_PaymentMethod", data);
        //    }
        //}

        //public ActionResult LoadShippingMethod()
        //{
        //    var data = db.TypeAttributeItems.Where(e => e.TypeAttributeId == 1);
        //    return PartialView("_ShippingMethod", data);
        //}

        //public ActionResult LoadDetailOrder()
        //{
        //    ShoppingCartModels model = new ShoppingCartModels();
        //    //model.Cart = (ShoppingCart)Session["Cart"];
        //    //model.Cart = HttpContext.Session.GetString("Cart");
        //    return PartialView("_DetailOrder", model);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}