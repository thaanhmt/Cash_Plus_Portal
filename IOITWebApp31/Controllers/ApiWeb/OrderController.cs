using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiWeb
{

    [Route("web/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("order-web", "order-web");

        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration _configuration { get; }

        public OrderController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        //Trả về đơn hàng đã làm
        //[Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> ListOrder([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<Order> data = db.Order.Where(c => c.Status != (int)Const.Status.DELETED);
                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    //def.metadata = data.Count();
                    int TotalInit = data.Where(e => e.OrderStatusId == (int)Const.OrderStatus.INIT).ToList().Count();
                    int TotalConfirm = data.Where(e => e.OrderStatusId == (int)Const.OrderStatus.CONFIRM).ToList().Count();
                    int TotalDelivery = data.Where(e => e.OrderStatusId == (int)Const.OrderStatus.DELIVERY).ToList().Count();
                    int TotalDelived = data.Where(e => e.OrderStatusId == (int)Const.OrderStatus.DELIVED).ToList().Count();
                    int TotalCancel = data.Where(e => e.OrderStatusId == (int)Const.OrderStatus.ORDER_RETURNED).ToList().Count();
                    def.metadata = new MetadataTotal(data.Count(), TotalInit, TotalConfirm, TotalDelivery, TotalDelived, TotalCancel);

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
                    {
                        var listData = await data.Select(e => new OrderWebDTO
                        {
                            OrderId = e.OrderId,
                            Code = e.Code,
                            CustomerId = e.CustomerId,
                            CustomerAddressId = e.CustomerAddressId,
                            PaymentMethodId = e.PaymentMethodId,
                            PaymentStatusId = e.PaymentStatusId,
                            ShippingMethodId = e.ShippingMethodId,
                            ShippingStatusId = e.ShippingStatusId,
                            OrderStatusId = e.OrderStatusId,
                            OrderTax = e.OrderTax,
                            OrderDiscount = e.OrderDiscount,
                            OrderTotal = e.OrderTotal,
                            CustomerNote = e.CustomerNote,
                            UserId = e.UserId,
                            CreatedAt = e.CreatedAt,
                            UpdatedAt = e.UpdatedAt,
                            Status = e.Status,
                        }).ToListAsync();

                        foreach (var item in listData)
                        {
                            item.customerAddress = await db.CustomerAddress.Where(e => e.CustomerAddressId == item.CustomerAddressId).Select(e => new CustomerAddressDTO
                            {
                                CustomerAddressId = e.CustomerAddressId,
                                CustomerId = e.CustomerId,
                                Name = e.Name,
                                Email = e.Email,
                                Phone = e.Phone,
                                ProvinceId = e.ProvinceId,
                                ProvinceName = db.Province.Where(p => p.ProvinceId == e.ProvinceId).FirstOrDefault() != null ? db.Province.Where(p => p.ProvinceId == e.ProvinceId).FirstOrDefault().Name : "",
                                DistrictId = e.DistrictId,
                                DistrictName = db.District.Where(p => p.DistrictId == e.DistrictId).FirstOrDefault() != null ? db.District.Where(p => p.DistrictId == e.DistrictId).FirstOrDefault().Name : "",
                                Address = e.Address,
                                Note = e.Note,
                                UpdatedAt = e.UpdatedAt,
                                Status = e.Status,
                            }).FirstOrDefaultAsync();

                            var ovt = await db.OrderItem.Where(o => o.OrderId == item.OrderId && o.Status != (int)Const.Status.DELETED).ToListAsync();
                            List<OrderItemDTO> orderVisaItems = new List<OrderItemDTO>();
                            foreach (var itemO in ovt)
                            {
                                OrderItemDTO orderVisaItem = new OrderItemDTO();
                                orderVisaItem.ProductId = itemO.ProductId;
                                var product = await db.Product.Where(e => e.ProductId == itemO.ProductId).FirstOrDefaultAsync();
                                orderVisaItem.ProductName = product != null ? product.Name : "";
                                orderVisaItem.ProductImage = product != null ? product.Image : "";
                                orderVisaItem.ProductUrl = product != null ? product.Url : "";
                                orderVisaItem.PriceOld = product != null ? product.PriceSale : 0;
                                orderVisaItem.Discount = product != null ? product.Discount : 0;
                                orderVisaItem.Quantity = itemO.Quantity;
                                orderVisaItem.Price = itemO.Price;
                                orderVisaItem.PriceTax = itemO.PriceTax;
                                orderVisaItem.PriceDiscount = itemO.PriceDiscount;
                                orderVisaItem.PriceTotal = itemO.PriceTotal;
                                orderVisaItem.Status = itemO.Status;
                                orderVisaItems.Add(orderVisaItem);
                            }

                            item.listOrderItem = orderVisaItems;
                        }

                        def.data = listData.ToList();
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

        //Trả về 1 đơn hàng 
        [HttpGet("getById")]
        public async Task<IActionResult> Order([FromQuery] string id)
        {
            DefaultResponse def = new DefaultResponse();
            using (var db = new IOITDataContext())
            {
                var data = await db.Order.Where(c => c.Code == id && c.Status != (int)Const.Status.DELETED).Select(e => new OrderWebDTO
                {
                    OrderId = e.OrderId,
                    Code = e.Code,
                    CustomerId = e.CustomerId,
                    CustomerAddressId = e.CustomerAddressId,
                    PaymentMethodId = e.PaymentMethodId,
                    PaymentStatusId = e.PaymentStatusId,
                    ShippingMethodId = e.ShippingMethodId,
                    ShippingStatusId = e.ShippingStatusId,
                    OrderStatusId = e.OrderStatusId,
                    OrderTax = e.OrderTax,
                    OrderDiscount = e.OrderDiscount,
                    OrderTotal = e.OrderTotal,
                    CustomerNote = e.CustomerNote,
                    UserId = e.UserId,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    Status = e.Status,
                }).FirstOrDefaultAsync();

                if (data != null)
                {
                    var cus = await db.Customer.Where(e => e.CustomerId == data.CustomerId).FirstOrDefaultAsync();
                    data.Email = cus != null ? cus.Email : "";

                    data.customerAddress = await db.CustomerAddress.Where(e => e.CustomerAddressId == data.CustomerAddressId).Select(e => new CustomerAddressDTO
                    {
                        CustomerAddressId = e.CustomerAddressId,
                        CustomerId = e.CustomerId,
                        Name = e.Name,
                        Email = e.Email,
                        Phone = e.Phone,
                        ProvinceId = e.ProvinceId,
                        ProvinceName = db.Province.Where(p => p.ProvinceId == e.ProvinceId).FirstOrDefault() != null ? db.Province.Where(p => p.ProvinceId == e.ProvinceId).FirstOrDefault().Name : "",
                        DistrictId = e.DistrictId,
                        DistrictName = db.District.Where(p => p.DistrictId == e.DistrictId).FirstOrDefault() != null ? db.District.Where(p => p.DistrictId == e.DistrictId).FirstOrDefault().Name : "",
                        Address = e.Address,
                        Note = e.Note,
                        UpdatedAt = e.UpdatedAt,
                        Status = e.Status,
                    }).FirstOrDefaultAsync();

                    var ovt = await db.OrderItem.Where(o => o.OrderId == data.OrderId && o.Status != (int)Const.Status.DELETED).ToListAsync();
                    List<OrderItemDTO> orderVisaItems = new List<OrderItemDTO>();
                    foreach (var itemO in ovt)
                    {
                        OrderItemDTO orderVisaItem = new OrderItemDTO();
                        orderVisaItem.ProductId = itemO.ProductId;
                        var product = await db.Product.Where(e => e.ProductId == itemO.ProductId).FirstOrDefaultAsync();
                        orderVisaItem.ProductName = product != null ? product.Name : "";
                        orderVisaItem.ProductImage = product != null ? product.Image : "";
                        orderVisaItem.ProductUrl = product != null ? product.Url : "";
                        orderVisaItem.PriceOld = product != null ? product.PriceSale : 0;
                        orderVisaItem.Discount = product != null ? product.Discount : 0;
                        orderVisaItem.Quantity = itemO.Quantity;
                        orderVisaItem.Price = itemO.Price;
                        orderVisaItem.PriceTax = itemO.PriceTax;
                        orderVisaItem.PriceDiscount = itemO.PriceDiscount;
                        orderVisaItem.PriceTotal = itemO.PriceTotal;
                        orderVisaItem.Status = itemO.Status;
                        orderVisaItems.Add(orderVisaItem);
                    }

                    data.listOrderItem = orderVisaItems;
                }
                def.data = data;
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
        }

        //Trả về chi tiết đơn hàng theo tên sản phẩm
        [HttpGet("search")]
        public async Task<IActionResult> SearchOrder([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    try
                    {
                        def.meta = new Meta(200, "Success");
                        IQueryable<OrderWebDTO> data = (
                            from ov in db.Order
                            join ovi in db.OrderItem on ov.OrderId equals ovi.OrderId
                            join pp in db.Product on ovi.ProductId equals pp.ProductId
                            where ovi.Status != (int)Const.Status.DELETED
                            select new OrderWebDTO
                            {
                                OrderId = ov.OrderId,
                                Code = ov.Code,
                                CustomerId = ov.CustomerId,
                                CustomerAddressId = ov.CustomerAddressId,
                                PaymentMethodId = ov.PaymentMethodId,
                                PaymentStatusId = ov.PaymentStatusId,
                                ShippingMethodId = ov.ShippingMethodId,
                                ShippingStatusId = ov.ShippingStatusId,
                                OrderStatusId = ov.OrderStatusId,
                                OrderTax = ov.OrderTax,
                                OrderDiscount = ov.OrderDiscount,
                                OrderTotal = ov.OrderTotal,
                                CustomerNote = ov.CustomerNote,
                                UserId = ov.UserId,
                                CreatedAt = ov.CreatedAt,
                                UpdatedAt = ov.UpdatedAt,
                                Status = ov.Status,
                                ProductName = pp.Name
                            }).ToList().AsQueryable();
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
                                data = data.OrderBy("OrderVisaItemId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                                data = data.OrderBy("OrderVisaItemId desc");
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
                            def.data = data.ToList();
                        }

                        return Ok(def);
                    }
                    catch (Exception e)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        [HttpPost("PostOrder")]
        public async Task<IActionResult> PostOrder(OrderWebDTO data)
        {
            DefaultResponse def = new DefaultResponse();

            var identity = (ClaimsIdentity)User.Identity;
            //string CustomerId = identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault();

            //if (data.CustomerId != int.Parse(CustomerId))
            //{
            //    def.meta = new Meta(222, "Lỗi tài khoản đăng nhập!");
            //    return Ok(def);
            //}

            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }

                if (data.listOrderItem == null)
                {
                    def.meta = new Meta(211, "Không có sản phẩm nào trong đơn hàng!");
                    return Ok(def);
                }


                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //string pass = "";
                        //Kiểm tra nếu khách hàng đã login thì kiểm tra xem khách hàng đã chọn địa chỉ giao hàng chưa
                        if (data.CustomerId > 0)
                        {
                            if (data.CustomerAddressId == null)
                            {
                                def.meta = new Meta(211, "Chưa chọn địa chỉ giao hàng!");
                                return Ok(def);
                            }
                            else
                            {
                                var cusAdd = db.CustomerAddress.Where(e => e.CustomerAddressId == data.CustomerAddressId).FirstOrDefault();

                                if (cusAdd != null)
                                {
                                    data.FullName = cusAdd.Name;
                                    data.Phone = cusAdd.Phone;
                                    data.Email = cusAdd.Email;
                                    data.Address = cusAdd.Address;
                                    data.DistrictId = cusAdd.DistrictId;
                                    data.ProvinceId = cusAdd.ProvinceId;
                                }
                                else
                                {
                                    def.meta = new Meta(215, "Địa chỉ giao hàng không chinh xác!");
                                    return Ok(def);
                                }
                            }
                        }
                        else
                        {
                            //Nếu khách hàng chưa login thì kiểm tra các thông tin giao hàng có chưa
                            if (data.FullName == null || data.FullName == "")
                            {
                                def.meta = new Meta(211, "Họ tên người nhận không được để trống!");
                                return Ok(def);
                            }
                            if (data.Phone == null || data.Phone == "")
                            {
                                def.meta = new Meta(211, "Số điện thoại người nhận không được để trống!");
                                return Ok(def);
                            }
                            if (data.Email == null || data.Email == "")
                            {
                                def.meta = new Meta(211, "Email người nhận không được để trống!");
                                return Ok(def);
                            }
                            if (data.ProvinceId == null || data.ProvinceId == -1 || data.DistrictId == null || data.DistrictId == -1 || data.Address == null || data.Address == "")
                            {
                                def.meta = new Meta(211, "Địa chỉ người nhận không được để trống!");
                                return Ok(def);
                            }
                            //Kiểm tra xem email có trùng với tài khoản nào không, nếu ko trùng thì tạo mới

                            var cus = db.Customer.Where(e => e.Email.Trim().ToLower().Equals(data.Email.Trim().ToLower())
                            && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (cus != null)
                            {
                                data.CustomerId = cus.CustomerId;

                                //Kiểm tra xem danh sách địa chỉ giao hàng đã có chưa, nếu chưa có thì tạo mới
                                var cusAdd = db.CustomerAddress.Where(e => e.CustomerId == data.CustomerId
                                 && e.Name.Trim().ToLower().Equals(data.FullName.Trim().ToLower())
                                 && e.Phone.Trim().ToLower().Equals(data.Phone.Trim().ToLower())
                                 && e.ProvinceId == data.ProvinceId && e.DistrictId == data.DistrictId
                                 && e.Address.Trim().ToLower().Equals(data.Address.Trim().ToLower())).FirstOrDefault();

                                if (cusAdd != null)
                                {
                                    data.CustomerAddressId = cusAdd.CustomerAddressId;
                                    if (cusAdd.Status == (int)Const.Status.DELETED)
                                    {
                                        cusAdd.Status = (int)Const.Status.NORMAL;
                                        db.CustomerAddress.Update(cusAdd);
                                        await db.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    CustomerAddress customerAddress = new CustomerAddress();
                                    customerAddress.CustomerAddressId = Guid.NewGuid();
                                    customerAddress.CustomerId = data.CustomerId;
                                    customerAddress.Name = data.FullName;
                                    customerAddress.Phone = data.Phone;
                                    customerAddress.Email = data.Email;
                                    customerAddress.Phone = data.Phone;
                                    customerAddress.Address = data.Address;
                                    customerAddress.ProvinceId = data.ProvinceId;
                                    customerAddress.IsMain = false;
                                    customerAddress.DistrictId = data.DistrictId;
                                    customerAddress.Note = data.CustomerNote;
                                    customerAddress.CreatedAt = DateTime.Now;
                                    customerAddress.UpdatedAt = DateTime.Now;
                                    customerAddress.Status = (int)Const.Status.NORMAL;
                                    await db.CustomerAddress.AddAsync(customerAddress);
                                    await db.SaveChangesAsync();
                                    data.CustomerAddressId = customerAddress.CustomerAddressId;
                                }
                            }
                            else
                            {
                                //Tạo khách hàng
                                //random pass
                                data.PassHash = Utils.RandomString(6);
                                string keyRandom = Utils.RandomString(8);
                                string passHash = Utils.GetMD5Hash(data.PassHash) + keyRandom;
                                Customer customer = new Customer();
                                customer.Username = data.Email;
                                customer.Password = Utils.GetMD5Hash(passHash);
                                customer.FullName = data.FullName;
                                customer.Email = data.Email;
                                customer.Phone = data.Phone;
                                //customer.Avata = data.Avata;
                                //customer.Sex = data.Sex;
                                //customer.Birthday = data.Birthday;
                                customer.Address = data.Address;
                                //customer.Note = data.Note;
                                customer.KeyRandom = keyRandom;
                                customer.IsEmailConfirm = true;
                                customer.IsSentEmailConfirm = false;
                                customer.IsPhoneConfirm = false;
                                customer.Type = 1;
                                customer.WebsiteId = Const.WEBSITEID;
                                customer.CompanyId = Const.COMPANYID;
                                customer.TypeThirdId = 1;
                                customer.LastLoginAt = DateTime.Now;
                                customer.CreatedAt = DateTime.Now;
                                customer.UpdatedAt = DateTime.Now;
                                customer.Status = (int)Const.Status.NORMAL;
                                await db.Customer.AddAsync(customer);
                                await db.SaveChangesAsync();
                                data.CustomerId = customer.CustomerId;
                                //string passWordHash = customer.Password + customer.KeyRandom;
                                //customer.Password = Utils.GetMD5Hash(passWordHash);
                                //db.Customer.Update(customer);
                                //Tạo địa chỉ giao hàng
                                CustomerAddress customerAddress = new CustomerAddress();
                                customerAddress.CustomerAddressId = Guid.NewGuid();
                                customerAddress.CustomerId = data.CustomerId;
                                customerAddress.Name = data.FullName;
                                customerAddress.Phone = data.Phone;
                                customerAddress.Email = data.Email;
                                customerAddress.Phone = data.Phone;
                                customerAddress.Address = data.Address;
                                customerAddress.ProvinceId = data.ProvinceId;
                                customerAddress.IsMain = false;
                                customerAddress.DistrictId = data.DistrictId;
                                customerAddress.Note = data.CustomerNote;
                                customerAddress.CreatedAt = DateTime.Now;
                                customerAddress.UpdatedAt = DateTime.Now;
                                customerAddress.Status = (int)Const.Status.NORMAL;
                                await db.CustomerAddress.AddAsync(customerAddress);
                                await db.SaveChangesAsync();
                                data.CustomerAddressId = customerAddress.CustomerAddressId;
                            }
                        }

                        Order order = new Order();
                        //order.Code = Utils.GenCodeOrder((int)data.CustomerId);
                        order.CustomerId = data.CustomerId;
                        order.CustomerAddressId = data.CustomerAddressId;
                        order.PaymentMethodId = data.PaymentMethodId;
                        order.PaymentStatusId = (int)Const.PaymentStatus.INIT;
                        order.ShippingMethodId = data.ShippingMethodId;
                        order.ShippingStatusId = (int)Const.ShippingStatus.INIT;
                        order.OrderStatusId = (int)Const.OrderStatus.INIT;
                        order.OrderTax = data.OrderTax;
                        order.OrderDelivery = data.OrderDelivery != null ? data.OrderDelivery : 0;
                        order.OrderDiscount = data.OrderDiscount != null ? data.OrderDiscount : 0;
                        order.OrderPaid = data.OrderPaid != null ? data.OrderPaid : 0;
                        order.OrderTotal = data.OrderTotal;
                        order.CustomerNote = data.CustomerNote;
                        order.StepSent = 0;
                        order.IsSentMail = false;
                        order.WebsiteId = Const.WEBSITEID;
                        order.CompanyId = Const.COMPANYID;
                        order.CreatedAt = DateTime.Now;
                        order.UpdatedAt = DateTime.Now;
                        order.Status = (int)Const.Status.NORMAL;
                        db.Order.Add(order);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.OrderId = order.OrderId;

                            //Thêm danh sách sản phẩm trong đơn hàng
                            if (data.listOrderItem.Count > 0)
                            {
                                foreach (var item in data.listOrderItem)
                                {
                                    var oi = await db.OrderItem.Where(e => e.OrderId == order.OrderId
                                     && e.ProductId == item.ProductId && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();

                                    if (oi == null)
                                    {
                                        OrderItem orderItem = new OrderItem();
                                        orderItem.OrderId = order.OrderId;
                                        orderItem.ProductId = item.ProductId;
                                        orderItem.Quantity = item.Quantity;
                                        orderItem.Price = item.Price;
                                        orderItem.PriceTax = item.PriceTax != null ? item.PriceTax : 0;
                                        orderItem.PriceDiscount = item.PriceDiscount != null ? item.PriceDiscount : 0;
                                        orderItem.PriceTotal = (item.Price * item.Quantity) + orderItem.PriceTax - orderItem.PriceDiscount;
                                        orderItem.Status = (int)Const.Status.NORMAL;

                                        await db.OrderItem.AddAsync(orderItem);

                                        item.PriceTax = orderItem.PriceTax;
                                        item.PriceDiscount = orderItem.PriceDiscount;
                                        item.PriceTotal = orderItem.PriceTotal;
                                    }
                                    else
                                    {
                                        item.Status = (int)Const.Status.DELETED;
                                    }
                                }
                            }

                            await db.SaveChangesAsync();

                            if (order.OrderId > 0)
                            {

                                var config = await db.Config.FindAsync(1);
                                if (config != null)
                                {
                                    //Kiểm tra xem có bật thanh toán không
                                    if (config.IsOnePay == true && data.PaymentMethodId != (int)Const.PaymentMethod.COD)
                                    {
                                        decimal exchangeRate = config.ExchangRate != null ? (decimal)config.ExchangRate : 1;

                                        data.OrderId = order.OrderId;
                                        data.Code = order.Code;
                                        //data.OrderTotal = orderVisa.OrderTotal;
                                        if (data.IpAdress == "::1") data.IpAdress = "171.255.72.78";
                                        decimal totalOV = (decimal)order.OrderTotal * exchangeRate;
                                        data.OrderTotal = Math.Round(totalOV, 0);

                                        //order.OrderTotalVnd = data.OrderTotal;
                                        db.Order.Update(order);
                                        await db.SaveChangesAsync();
                                        //Tạo lịch sử thanh toán
                                        PaymentHistory paymentHistory = new PaymentHistory();
                                        paymentHistory.PaymentHistoryId = Guid.NewGuid();
                                        paymentHistory.Version = 2;
                                        paymentHistory.Currency = "VND";
                                        paymentHistory.Command = "pay";
                                        paymentHistory.AccessCode = config.OpAccessCode;
                                        paymentHistory.Merchant = config.OpMerchant;
                                        paymentHistory.Locale = data.Locale;
                                        paymentHistory.ReturnUrl = data.ReturnUrl;
                                        paymentHistory.MerchTxnRef = paymentHistory.PaymentHistoryId + "";
                                        paymentHistory.OrderInfo = order.Code;
                                        paymentHistory.Amount = data.OrderTotal.ToString().Trim().Split('.')[0];
                                        paymentHistory.TicketNo = data.IpAdress;
                                        paymentHistory.CardList = data.CardList;
                                        paymentHistory.AgainLink = data.AgainLink;
                                        paymentHistory.PayType = (byte)data.PaymentMethodId;
                                        paymentHistory.CustomerPhone = data.Phone;
                                        paymentHistory.CustomerEmail = data.Email;
                                        paymentHistory.CustomerId = data.PassHash;

                                        //Tạo hashkey ở đây
                                        string messege = "";
                                        string key = config.OpKey;
                                        if (data.PaymentMethodId == (int)Const.PaymentMethod.ONEPAY_IN)
                                        {
                                            messege = "vpc_AccessCode=" + paymentHistory.AccessCode
                                            + "&vpc_Amount=" + paymentHistory.Amount + "00"
                                            + "&vpc_CardList=" + data.CardList
                                            + "&vpc_Command=" + paymentHistory.Command
                                            + "&vpc_Locale=" + paymentHistory.Locale
                                            + "&vpc_MerchTxnRef=" + paymentHistory.PaymentHistoryId
                                            + "&vpc_Merchant=" + paymentHistory.Merchant
                                            + "&vpc_OrderInfo=" + paymentHistory.OrderInfo
                                            + "&vpc_ReturnURL=" + paymentHistory.ReturnUrl
                                            + "&vpc_TicketNo=" + paymentHistory.TicketNo
                                            + "&vpc_Version=" + paymentHistory.Version;
                                        }
                                        else
                                        {
                                            messege = "vpc_AccessCode=" + paymentHistory.AccessCode
                                            + "&vpc_Amount=" + paymentHistory.Amount + "00"
                                            + "&vpc_Command=" + paymentHistory.Command
                                            + "&vpc_Locale=" + paymentHistory.Locale
                                            + "&vpc_MerchTxnRef=" + paymentHistory.PaymentHistoryId
                                            + "&vpc_Merchant=" + paymentHistory.Merchant
                                            + "&vpc_OrderInfo=" + paymentHistory.OrderInfo
                                            + "&vpc_ReturnURL=" + paymentHistory.ReturnUrl
                                            + "&vpc_TicketNo=" + paymentHistory.TicketNo
                                            + "&vpc_Version=" + paymentHistory.Version;
                                        }

                                        data.HashKey = Security.HashHMACHex(key, messege);
                                        data.PaymentRequest = messege;
                                        data.PaymentHistoryId = paymentHistory.PaymentHistoryId + "";

                                        paymentHistory.SecureHash = data.HashKey;
                                        paymentHistory.CreatedAt = DateTime.Now;
                                        paymentHistory.UpdatedAt = DateTime.Now;
                                        paymentHistory.Status = (int)Const.Status.NORMAL;
                                        await db.PaymentHistory.AddAsync(paymentHistory);
                                        await db.SaveChangesAsync();

                                        transaction.Commit();
                                        HttpContext.Session.Remove("Cart");

                                        def.meta = new Meta(200, "Success");
                                        def.data = data;
                                        return Ok(def);
                                    }
                                    else
                                    {

                                        bool check = false;
                                        try
                                        {
                                            var customer = await db.Customer.Where(e => e.CustomerId == data.CustomerId).FirstOrDefaultAsync();
                                            if (customer != null)
                                            {
                                                data.PaymentStatusId = order.PaymentStatusId;
                                                data.Code = order.Code;
                                                data.CreatedAt = order.CreatedAt != null ? order.CreatedAt : DateTime.Now;
                                                data.OrderDelivery = order.OrderDelivery != null ? order.OrderDelivery : 0;
                                                data.OrderDiscount = order.OrderDiscount != null ? order.OrderDiscount : 0;
                                                data.OrderPaid = order.OrderPaid != null ? order.OrderPaid : 0;
                                                //Lấy địa chỉ
                                                var district = await db.District.Where(e => e.DistrictId == data.DistrictId).FirstOrDefaultAsync();
                                                if (district != null) data.Address += ", " + district.Name;
                                                var province = await db.Province.Where(e => e.ProvinceId == data.ProvinceId).FirstOrDefaultAsync();
                                                if (province != null) data.Address += ", " + province.Name;
                                                foreach (var item in data.listOrderItem.Where(e => e.Status != (int)Const.Status.DELETED))
                                                {
                                                    var pro = await db.Product.Where(e => e.ProductId == item.ProductId).FirstOrDefaultAsync();
                                                    if (pro != null)
                                                    {
                                                        item.ProductUrl = pro.Url;
                                                        item.ProductImage = pro.Image;
                                                        item.ProductName = pro.Name;
                                                    }
                                                }
                                                string url_temp = "";
                                                string subject = "";
                                                string title = "";
                                                string link = "";


                                                url_temp = "order-success.html";
                                                subject = config.EmailSender + " - Thông tin đơn hàng " + order.Code;
                                                check = Utils.sendEmail(config, data, url_temp, subject, 1, customer.FullName, customer.Email);

                                                try
                                                {
                                                    //Bắn thông báo lên firebase
                                                    //Lấy danh sách người nhận là tất cả admin và manager or chi nhánh
                                                    var userRoles = db.UserRole.Where(e => (e.RoleId == 1 || e.RoleId == 2) && e.Status != (int)Const.Status.DELETED).ToList();
                                                    //if (order.BranchId != null)
                                                    //{
                                                    //    userRoles = (from ur in db.UserRole
                                                    //                 join u in db.User on ur.UserId equals u.UserId
                                                    //                 where (ur.RoleId == 1 || ur.RoleId == 3 || (ur.RoleId == 2 && u.BranchId == orderVisa.BranchId))
                                                    //                 && u.Status != (int)Const.Status.DELETED
                                                    //                 && ur.Status != (int)Const.Status.DELETED
                                                    //                 select ur).ToList();
                                                    //}

                                                    if (userRoles.Count > 0)
                                                    {
                                                        foreach (var item in userRoles)
                                                        {
                                                            //create action
                                                            Models.EF.Action action = new Models.EF.Action();
                                                            action.ActionName = "Tạo đơn hàng";
                                                            action.ActionType = (int)Const.ActionType.CREATE;
                                                            action.TargetId = order.OrderId + "";
                                                            action.TargetName = "ORDER";
                                                            action.Logs = JsonConvert.SerializeObject(order);
                                                            action.Time = 0;
                                                            action.Type = (int)Const.TypeAction.ACTION;
                                                            action.CreatedAt = DateTime.Now;
                                                            action.UserPushId = order.CustomerId;
                                                            action.UserId = item.UserId;
                                                            action.Status = (int)Const.Status.NORMAL;
                                                            await db.Action.AddAsync(action);
                                                            await db.SaveChangesAsync();

                                                            //push action
                                                            Models.Data.Firebase.pushAction(action);
                                                        }
                                                    }
                                                }
                                                catch { }
                                            }
                                        }
                                        catch
                                        {
                                            transaction.Rollback();
                                        }

                                        if (check)
                                        {
                                            order.IsSentMail = true;
                                        }
                                        else
                                        {
                                            order.IsSentMail = false;
                                        }
                                        db.Order.Update(order);
                                        await db.SaveChangesAsync();
                                        transaction.Commit();
                                        HttpContext.Session.Remove("Cart");
                                    }

                                }
                                else
                                {
                                    transaction.Rollback();
                                    def.meta = new Meta(400, "Lỗi hệ thống, bạn vui lòng thử lại sau!");
                                    return Ok(def);
                                }

                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Đơn hàng của bạn đã được tạo!");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (OrderExists(data.OrderId))
                            {
                                def.meta = new Meta(212, "Đơn hàng đã tồn tại!");
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
                def.meta = new Meta(500, "Lỗi kết nối tới máy chủ!");
                return Ok(def);
            }
        }

        [HttpPost("createPayment/{id}")]
        public async Task<IActionResult> CreatePayment([FromRoute] int id, [FromBody] OrderWebDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //check tính toàn vẹn dữ liệu
                            var orderVisa = db.Order.Where(e => e.OrderId == id && e.Status != (int)Const.Status.DELETED
                            && e.PaymentStatusId != (int)Const.PaymentStatus.FULL).FirstOrDefault();
                            if (orderVisa.OrderId > 0)
                            {
                                var config = await db.Config.FindAsync(1);
                                if (config != null)
                                {
                                    data.Code = orderVisa.Code;
                                    if (data.IpAdress == "::1" || data.IpAdress == null || data.IpAdress == "") data.IpAdress = "171.255.72.78";
                                    if (data.CardList == "" || data.CardList == null) data.CardList = "970436";
                                    decimal exchangeRate = config.ExchangRate != null ? (decimal)config.ExchangRate : 23230;
                                    decimal totalOV = (decimal)orderVisa.OrderTotal * exchangeRate;
                                    data.OrderTotal = Math.Round(totalOV, 0);

                                    //check đơn hàng đã được thanh toán chưa
                                    var ph = await db.PaymentHistory.Where(e => e.OrderInfo == orderVisa.Code
                                        && e.TransactionNo == "0" && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                                    if (ph == null)
                                    {
                                        //Tạo lịch sử thanh toán
                                        PaymentHistory paymentHistory = new PaymentHistory();
                                        paymentHistory.PaymentHistoryId = Guid.NewGuid();
                                        paymentHistory.Version = 2;
                                        paymentHistory.Currency = "VND";
                                        paymentHistory.Command = "pay";
                                        paymentHistory.AccessCode = config.OpAccessCode;
                                        paymentHistory.Merchant = config.OpMerchant;
                                        paymentHistory.Locale = data.Locale;
                                        paymentHistory.ReturnUrl = data.ReturnUrl;
                                        paymentHistory.MerchTxnRef = orderVisa.OrderId + "";
                                        paymentHistory.OrderInfo = orderVisa.Code;
                                        paymentHistory.Amount = data.OrderTotal.ToString().Trim().Split('.')[0];
                                        paymentHistory.TicketNo = data.IpAdress;
                                        paymentHistory.CardList = data.CardList;
                                        paymentHistory.AgainLink = data.AgainLink;
                                        paymentHistory.PayType = (byte)data.PaymentMethodId;

                                        //Tạo hashkey ở đây
                                        string key = config.OpKey;
                                        string messege = "";
                                        if (data.PaymentMethodId == (int)Const.PaymentMethod.ONEPAY_IN)
                                        {
                                            messege = "vpc_AccessCode=" + paymentHistory.AccessCode
                                                + "&vpc_Amount=" + paymentHistory.Amount + "00"
                                                + "&vpc_CardList=" + data.CardList
                                                + "&vpc_Command=" + paymentHistory.Command
                                                + "&vpc_Locale=" + paymentHistory.Locale
                                                + "&vpc_MerchTxnRef=" + paymentHistory.PaymentHistoryId
                                                + "&vpc_Merchant=" + paymentHistory.Merchant
                                                + "&vpc_OrderInfo=" + paymentHistory.OrderInfo
                                                + "&vpc_ReturnURL=" + paymentHistory.ReturnUrl
                                                + "&vpc_TicketNo=" + paymentHistory.TicketNo
                                                + "&vpc_Version=" + paymentHistory.Version;
                                        }
                                        else
                                        {
                                            messege = "vpc_AccessCode=" + paymentHistory.AccessCode
                                                + "&vpc_Amount=" + paymentHistory.Amount + "00"
                                                + "&vpc_Command=" + paymentHistory.Command
                                                + "&vpc_Locale=" + paymentHistory.Locale
                                                + "&vpc_MerchTxnRef=" + paymentHistory.PaymentHistoryId
                                                + "&vpc_Merchant=" + paymentHistory.Merchant
                                                + "&vpc_OrderInfo=" + paymentHistory.OrderInfo
                                                + "&vpc_ReturnURL=" + paymentHistory.ReturnUrl
                                                + "&vpc_TicketNo=" + paymentHistory.TicketNo
                                                + "&vpc_Version=" + paymentHistory.Version;
                                        }

                                        data.HashKey = Security.HashHMACHex(key, messege);
                                        //data.PaymentRequest = messege;
                                        data.PaymentHistoryId = paymentHistory.PaymentHistoryId + "";

                                        paymentHistory.SecureHash = data.HashKey;
                                        paymentHistory.CreatedAt = DateTime.Now;
                                        paymentHistory.UpdatedAt = DateTime.Now;
                                        paymentHistory.Status = (int)Const.Status.NORMAL;
                                        await db.PaymentHistory.AddAsync(paymentHistory);
                                        await db.SaveChangesAsync();

                                        ////update order
                                        //orderVisa.PaymentMethodId = data.PaymentMethodId;
                                        //orderVisa.UpdatedAt = DateTime.Now;
                                        //db.OrderVisa.Update(orderVisa);

                                        if (paymentHistory.PaymentHistoryId != null)
                                        {
                                            transaction.Commit();
                                        }
                                    }
                                    else
                                    {
                                        def.meta = new Meta(201, "Order Payment Full");
                                        def.data = data;
                                        return Ok(def);
                                    }

                                    def.meta = new Meta(200, "Success");
                                    def.data = data;
                                    return Ok(def);
                                }
                                else
                                {
                                    transaction.Rollback();
                                    def.meta = new Meta(400, "Bad Request");
                                    return Ok(def);
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                                def.meta = new Meta(404, "Order Visa Not Found");
                                return Ok(def);
                            }
                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
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

        [HttpPost("updatePayment/{id}")]
        public async Task<IActionResult> UpdatePayment([FromRoute] string id, [FromBody] PaymentHistory data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //check tính toàn vẹn dữ liệu
                            var order = db.Order.Where(e => e.Code.Trim().ToUpper().Equals(id.Trim().ToUpper()) && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (order.OrderId > 0)
                            {
                                if (order.PaymentStatusId != (int)Const.PaymentStatus.FULL)
                                {
                                    var paymentHistory = await db.PaymentHistory.Where(e => e.PaymentHistoryId.ToString() == data.MerchTxnRef && e.OrderInfo == data.OrderInfo
                                    && e.Status != (int)Const.Status.DELETED && e.TransactionNo != "0").FirstOrDefaultAsync();
                                    if (paymentHistory != null)
                                    {
                                        paymentHistory.TxnResponseCode = data.TxnResponseCode;
                                        paymentHistory.TransactionNo = data.TransactionNo;
                                        paymentHistory.Message = data.Message;
                                        paymentHistory.Card = data.Card;
                                        paymentHistory.PayChannel = data.PayChannel;
                                        paymentHistory.CardUid = data.CardUid;
                                        paymentHistory.UpdatedAt = DateTime.Now;
                                        db.PaymentHistory.Update(paymentHistory);
                                        await db.SaveChangesAsync();

                                        if (data.TxnResponseCode == "0")
                                        {
                                            order.OrderPaid = order.OrderTotal;
                                            if (data.Amount == paymentHistory.Amount + "00")
                                            {
                                                order.PaymentStatusId = (int)Const.PaymentStatus.FULL;
                                                def.meta = new Meta(200, "Transaction is successful Full");
                                            }
                                            else
                                            {
                                                order.PaymentStatusId = (int)Const.PaymentStatus.NOT_ENOUGH;
                                                def.meta = new Meta(200, "Transaction is successful not Full");
                                            }
                                        }
                                        else if (data.TxnResponseCode == "1")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(201, "The transaction is unsuccessful. This transaction has been declined by"
                                                + "issuer bank or card have been not registered online payment services."
                                                + "Please contact your bank for further clarification.");
                                        }
                                        else if (data.TxnResponseCode == "2")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(202, "The transaction is unsuccessful. This transaction has been declined by"
                                                + "issuer bank. Please contact your bank for further clarification.");
                                        }
                                        else if (data.TxnResponseCode == "3")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(203, "The transaction is unsuccessful. OnePAY did not received payment result from Issuer bank"
                                                + "Please contact your bank for details and try again.");
                                        }
                                        else if (data.TxnResponseCode == "4")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(204, "The transaction is unsuccessful. Your card is expired or You have entered incorrect expired date."
                                                + "Please check and try again.");
                                        }
                                        else if (data.TxnResponseCode == "5")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(205, "The transaction is unsuccessful. This transaction cannot be processed due to insufficient funds."
                                                + "Please try another card.");
                                        }
                                        else if (data.TxnResponseCode == "6")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(206, "The transaction is unsuccessful. An error was encountered while processing your transaction."
                                                + "Please contact your bank for further clarification.");
                                        }
                                        else if (data.TxnResponseCode == "7")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(207, "The transaction is unsuccessful. An error was encountered while processing your transaction."
                                                + "Please try again.");
                                        }
                                        else if (data.TxnResponseCode == "8")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(208, "The transaction is unsuccessful. You have entered incorrect card number."
                                                + "Please try again.");
                                        }
                                        else if (data.TxnResponseCode == "9")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(209, "The transaction is unsuccessful. You have entered incorrect card holder name."
                                                + "Please try again.");
                                        }
                                        else if (data.TxnResponseCode == "10")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(210, "The transaction is unsuccessful. The card is expired/locked."
                                                + "Please try again.");
                                        }
                                        else if (data.TxnResponseCode == "11")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(211, "The transaction is unsuccessful. You have been not registered online payment services."
                                                + "Please contact your bank for details.");
                                        }
                                        else if (data.TxnResponseCode == "12")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(212, "The transaction is unsuccessful. You have entered incorrect Issue date or Expire date."
                                                + "Please try again.");
                                        }
                                        else if (data.TxnResponseCode == "13")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(213, "The transaction is unsuccessful. The transaction amount exceeds the maximum transaction/amount limit."
                                                + "Please try another card.");
                                        }
                                        else if (data.TxnResponseCode == "21")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(221, "The transaction is unsuccessful. This transaction cannot be processed due to insufficient funds in your account."
                                                + "Please try another card.");
                                        }
                                        else if (data.TxnResponseCode == "22")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(222, "The transaction is unsuccessful. This transaction cannot be processed due to invalid account."
                                                + "Please try again.");
                                        }
                                        else if (data.TxnResponseCode == "23")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(223, "The transaction is unsuccessful. This transaction cannot be processed due to account locked."
                                                + "Please contact your bank for further clarification.");
                                        }
                                        else if (data.TxnResponseCode == "24")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(224, "The transaction is unsuccessful. You have entered incorrect card number."
                                                + "Please try again.");
                                        }
                                        else if (data.TxnResponseCode == "25")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(225, "The transaction is unsuccessful. You have entered incorrect OTP."
                                                + "Please try again.");
                                        }
                                        else if (data.TxnResponseCode == "253")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(226, "The transaction is unsuccessful. Transaction timed out."
                                                + "Please try again.");
                                        }
                                        else if (data.TxnResponseCode == "99")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(227, "The transaction is unsuccessful. The transaction has been cancelled by card holder."
                                                + "Please try again.");
                                        }
                                        else if (data.TxnResponseCode == "B")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(228, "The transaction is unsuccessful. The card used in this transaction is not authorized 3D-Secure complete."
                                                + "Please contact your bank for further clarification.");
                                        }
                                        else if (data.TxnResponseCode == "E")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(229, "The transaction is unsuccessful. You have entered wrong CSC or Issuer Bank declided transaction."
                                                + "Please contact your bank for further clarification.");
                                        }
                                        else if (data.TxnResponseCode == "F")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(230, "The transaction is unsuccessful. Due to 3D Secure Authentication Failed."
                                                + "Please contact your bank for further clarification.");
                                        }
                                        else if (data.TxnResponseCode == "Z")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(231, "The transaction is unsuccessful. Transaction restricted due to OFD’s policies."
                                                + "Please contact OnePAY for details (Hotline 1900 633 927).");
                                        }
                                        else if (data.TxnResponseCode == "Other")
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(232, "The transaction is unsuccessful."
                                                + "Please contact OnePAY for details (Hotline 1900 633 927).");
                                        }
                                        else
                                        {
                                            order.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                            def.meta = new Meta(232, "The transaction is unsuccessful."
                                                + "Please contact OnePAY for details (Hotline 1900 633 927).");
                                        }

                                        if (data.TxnResponseCode == "0")
                                        {
                                            //Gửi mail tạo đơn hàng và thanh toán thành công
                                            try
                                            {
                                                OrderWebDTO orderWeb = new OrderWebDTO();
                                                var config = await db.Config.FindAsync(1);
                                                if (config != null)
                                                {
                                                    var customer = await db.Customer.Where(e => e.CustomerId == order.CustomerId).FirstOrDefaultAsync();
                                                    if (customer != null)
                                                    {
                                                        orderWeb.PassHash = paymentHistory.CustomerId;
                                                        orderWeb.OrderStatusId = order.OrderStatusId;
                                                        orderWeb.PaymentMethodId = order.PaymentMethodId;
                                                        orderWeb.PaymentStatusId = order.PaymentStatusId;
                                                        orderWeb.Code = order.Code;
                                                        orderWeb.CreatedAt = order.CreatedAt != null ? order.CreatedAt : DateTime.Now;
                                                        orderWeb.OrderDelivery = order.OrderDelivery != null ? order.OrderDelivery : 0;
                                                        orderWeb.OrderDiscount = order.OrderDiscount != null ? order.OrderDiscount : 0;
                                                        orderWeb.OrderPaid = order.OrderPaid != null ? order.OrderPaid : 0;
                                                        orderWeb.OrderTotal = order.OrderTotal != null ? order.OrderTotal : 0;

                                                        //Lấy địa chỉ
                                                        orderWeb.Address = "";
                                                        var ca = await db.CustomerAddress.Where(e => e.CustomerAddressId == order.CustomerAddressId).FirstOrDefaultAsync();
                                                        if (ca != null)
                                                        {
                                                            orderWeb.FullName = ca.Name;
                                                            orderWeb.Phone = ca.Phone;
                                                            orderWeb.Address = ca.Address;
                                                            var district = await db.District.Where(e => e.DistrictId == ca.DistrictId).FirstOrDefaultAsync();
                                                            if (district != null) orderWeb.Address += ", " + district.Name;
                                                            var province = await db.Province.Where(e => e.ProvinceId == ca.ProvinceId).FirstOrDefaultAsync();
                                                            if (province != null) orderWeb.Address += ", " + province.Name;
                                                        }
                                                        //Lấy chi tiết đơn hàng
                                                        var orderItem = await db.OrderItem.Where(e => e.OrderId == order.OrderId
                                                        && e.Status != (int)Const.Status.DELETED).Select(e => new OrderItemDTO
                                                        {
                                                            ProductId = e.ProductId,
                                                            Quantity = e.Quantity,
                                                            Price = e.Price,
                                                            PriceTotal = e.PriceTotal
                                                        }).ToListAsync();
                                                        foreach (var item in orderItem)
                                                        {
                                                            var pro = await db.Product.Where(e => e.ProductId == item.ProductId).FirstOrDefaultAsync();
                                                            if (pro != null)
                                                            {
                                                                item.ProductUrl = pro.Url;
                                                                item.ProductImage = pro.Image;
                                                                item.ProductName = pro.Name;
                                                            }
                                                        }
                                                        orderWeb.listOrderItem = orderItem;
                                                        string url_temp = "";
                                                        string subject = "";
                                                        string title = "";
                                                        string link = "";
                                                        bool check = false;

                                                        url_temp = "order-success.html";
                                                        subject = config.EmailSender + " - Thông tin đơn hàng " + order.Code;
                                                        check = Utils.sendEmail(config, orderWeb, url_temp, subject, 1, customer.FullName, customer.Email);

                                                        if (check)
                                                        {
                                                            order.IsSentMail = true;
                                                        }
                                                        else
                                                        {
                                                            order.IsSentMail = false;
                                                        }
                                                        db.Order.Update(order);
                                                    }
                                                }
                                            }
                                            catch { }

                                            if (order.IsSentMail == false)
                                                def.meta = new Meta(233, "Transaction is successful Full but not sent email");
                                        }

                                        order.PaymentMethodId = (int)paymentHistory.PayType;
                                        order.UpdatedAt = DateTime.Now;
                                        db.Order.Update(order);
                                        await db.SaveChangesAsync();

                                        transaction.Commit();

                                        def.data = data;
                                        return Ok(def);
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        def.meta = new Meta(200, "Transaction is successful Full");
                                        def.data = data;
                                        return Ok(def);
                                    }
                                }
                                else
                                {
                                    transaction.Rollback();
                                    def.meta = new Meta(200, "Transaction is successful Full");
                                    def.data = data;
                                    return Ok(def);
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                                def.meta = new Meta(404, "Order Visa not found");
                                def.data = data;
                                return Ok(def);
                            }
                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
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

        private bool OrderExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Order.Count(e => e.OrderId == id) > 0;
            }
        }
    }
}