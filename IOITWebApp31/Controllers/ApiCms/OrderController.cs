using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class OrderController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("error", "error");
        private static string functionCode = "QLDH";

        // GET: api/Order
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
                    IQueryable<Order> data = db.Order.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("OrderId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("OrderId desc");
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
                            e.OrderId,
                            e.Code,
                            e.CustomerId,
                            e.PaymentMethodId,
                            e.PaymentStatusId,
                            e.ShippingMethodId,
                            e.ShippingStatusId,
                            e.OrderStatusId,
                            e.OrderTax,
                            e.OrderDiscount,
                            e.OrderTotal,
                            e.CustomerNote,
                            e.WebsiteId,
                            e.CompanyId,
                            e.UserId,
                            e.CreatedAt,
                            e.UpdatedAt,
                            e.Status,
                            customerName = db.Customer.Where(c => c.CustomerId == e.CustomerId).Select(c => new
                            {
                                c.CustomerId,
                                c.FullName,
                                c.Phone
                            }).FirstOrDefault(),
                            listOrderItems = db.OrderItem.Where(oi => oi.OrderId == e.OrderId && oi.Status != (int)Const.Status.DELETED).Select(oi => new
                            {
                                oi.OrderId,
                                oi.OrderItemId,
                                oi.ProductId,
                                oi.Quantity,
                                oi.Price,
                                oi.PriceTax,
                                oi.PriceDiscount,
                                oi.PriceTotal,
                                oi.Status,
                                product = db.Product.Where(p => p.ProductId == oi.ProductId).Select(p => new
                                {
                                    p.Image,
                                    p.Name
                                }).FirstOrDefault()
                            }).ToList(),
                            customerAddress = db.CustomerAddress.Where(ca => ca.CustomerAddressId == e.CustomerAddressId).Select(ca => new
                            {
                                ca.Name,
                                ca.Phone,
                                ca.Email,
                                ca.Address,
                                ca.Note
                            }).FirstOrDefault()
                        }).ToList();
                    }
                    //def.data = data

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
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
                    Order data = await db.Order.FindAsync(id);

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

        [HttpPut("ChangeOrderStatus/{OrderId}/{Status}")]
        public async Task<IActionResult> ChangeOrderStatus(int OrderId, byte Status)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền thực hiện thao tác này!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Order order = db.Order.Where(o => o.OrderId == OrderId && o.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (order == null)
                        {
                            def.meta = new Meta(404, "Không tìm thấy đơn hàng. Xin vui lòng thử lại sau!");
                            return Ok(def);
                        }
                        order.OrderStatusId = Status;
                        db.Update(order);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (order.OrderStatusId != (int)Const.OrderStatus.DELIVERY)
                            {
                                //Gửi mail thay đổi trạng thái đơn hàng trừ trạng thái đang giao hàng
                                try
                                {
                                    OrderWebDTO orderWeb = new OrderWebDTO();
                                    var config = await db.Config.FindAsync(1);
                                    if (config != null)
                                    {
                                        var customer = await db.Customer.Where(e => e.CustomerId == order.CustomerId).FirstOrDefaultAsync();
                                        if (customer != null)
                                        {
                                            orderWeb.PassHash = null;
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

                                            url_temp = "order-change-status.html";
                                            subject = config.EmailSender + " - Cập nhật thông tin đơn hàng " + order.Code;
                                            check = Utils.sendEmail(config, orderWeb, url_temp, subject, 2, customer.FullName, customer.Email);

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
                                        }
                                    }
                                }
                                catch { }

                                if (order.IsSentMail == false)
                                    def.meta = new Meta(233, "Không gửi được Email");
                            }
                            if (order.OrderId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Thay đổi trạng thái đơn hàng thành công!");
                            def.data = OrderId;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!OrderExists(order.OrderId))
                            {
                                def.meta = new Meta(404, "Không tìm thấy đơn hàng. Xin vui lòng thử lại sau!");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Lỗi xảy ra trên hệ thống. Xin vui lòng thử lại sau!");
                                return Ok(def);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Lỗi xảy ra trên hệ thống. Xin vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        [HttpPut("ChangePaymentOrderStatus/{OrderId}/{Status}")]
        public async Task<IActionResult> ChangePaymentOrderStatus(int OrderId, byte Status)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền thực hiện thao tác này!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Order order = db.Order.Where(o => o.OrderId == OrderId && o.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (order == null)
                        {
                            def.meta = new Meta(404, "Không tìm thấy đơn hàng. Xin vui lòng thử lại sau!");
                            return Ok(def);
                        }
                        order.PaymentStatusId = Status;
                        db.Update(order);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (order.OrderId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Thay đổi trạng thái thanh toán đơn hàng thành công!");
                            def.data = OrderId;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!OrderExists(order.OrderId))
                            {
                                def.meta = new Meta(404, "Không tìm thấy đơn hàng. Xin vui lòng thử lại sau!");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Lỗi xảy ra trên hệ thống. Xin vui lòng thử lại sau!");
                                return Ok(def);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Lỗi xảy ra trên hệ thống. Xin vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, [FromBody] Order data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
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

                //if (data.CustomerId == null || data.CustomerId == "")
                //{
                //    def.meta = new Meta(211, "CustomerId Null!");
                //    return Ok(def);
                //}

                //if (data.OrderStatusId == null || data.OrderStatusId == "")
                //{
                //    def.meta = new Meta(211, "OrderStatusId Null!");
                //    return Ok(def);
                //}

                //if (data.CreatedAt == null || data.CreatedAt == "")
                //{
                //    def.meta = new Meta(211, "CreatedAt Null!");
                //    return Ok(def);
                //}
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.OrderId > 0)
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
                            if (!OrderExists(data.OrderId))
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

        // POST: api/Order
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] Order data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
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

                //if (data.CustomerId == null || data.CustomerId == "")
                //{
                //    def.meta = new Meta(211, "CustomerId Null!");
                //    return Ok(def);
                //}

                //if (data.OrderStatusId == null || data.OrderStatusId == "")
                //{
                //    def.meta = new Meta(211, "OrderStatusId Null!");
                //    return Ok(def);
                //}

                //if (data.CreatedAt == null || data.CreatedAt == "")
                //{
                //    def.meta = new Meta(211, "CreatedAt Null!");
                //    return Ok(def);
                //}
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Order.Add(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.OrderId > 0)
                                transaction.Commit();
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
                            if (OrderExists(data.OrderId))
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

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
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
                    Order data = await db.Order.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Order.Remove(data);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.OrderId > 0)
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
                            if (!OrderExists(data.OrderId))
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

        //API xóa danh sách đơn hàng
        [HttpPut("deletes")]
        public async Task<IActionResult> DeleteOrders([FromBody] int[] data)
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
                        Order order = await db.Order.FindAsync(data[i]);

                        if (order == null)
                        {
                            continue;
                        }

                        order.UserId = userId;
                        order.UpdatedAt = DateTime.Now;
                        order.Status = (int)Const.Status.DELETED;
                        db.Entry(order).State = EntityState.Modified;
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

        private bool OrderExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Order.Count(e => e.OrderId == id) > 0;
            }
        }
    }
}


