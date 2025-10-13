using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Web;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("productAPIWeb", "productAPIWeb");
        private readonly IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;

        public ProductController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        //[HttpGet("GetProductReviews/{ProductId}")]
        //public async Task<IActionResult> GetProductReviews([FromQuery] FilteredPagination paging, int ProductId)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    if (paging != null)
        //    {
        //        using (var db = new IOITDataContext())
        //        {
        //            def.meta = new Meta(200, "Success");
        //            DetailRatingStar obj = new DetailRatingStar();
        //            IQueryable<ProductReview> data = db.ProductReview.Where(c =>c.ProductId == ProductId && c.Status == (int)Const.Status.OK);

        //            if (paging.query != null)
        //            {
        //                paging.query = HttpUtility.UrlDecode(paging.query);
        //            }

        //            data = data.Where(paging.query);
        //            obj.item_count = data.Count();
        //            if(obj.item_count > 0)
        //            {
        //                obj.countStar1 = data.Where(d => d.NumberStar == 1).Count();
        //                obj.countStar2 = data.Where(d => d.NumberStar == 2).Count();
        //                obj.countStar3 = data.Where(d => d.NumberStar == 3).Count();
        //                obj.countStar4 = data.Where(d => d.NumberStar == 4).Count();
        //                obj.countStar5 = data.Where(d => d.NumberStar == 5).Count();
        //                obj.star1 = (int)(((float)obj.countStar1 / (float)obj.item_count) * 100);
        //                obj.star2 = (int)(((float)obj.countStar2 / (float)obj.item_count) * 100);
        //                obj.star3 = (int)(((float)obj.countStar3 / (float)obj.item_count) * 100);
        //                obj.star4 = (int)(((float)obj.countStar4 / (float)obj.item_count) * 100);
        //                obj.star5 = (int)(((float)obj.countStar5 / (float)obj.item_count) * 100);
        //                float star = (float)(obj.countStar1 + (obj.countStar2 * 2) + (obj.countStar3 * 3) + (obj.countStar4 * 4) + (obj.countStar5 * 5)) / (float)obj.item_count;
        //                obj.star = (float)(Math.Round(star * 2, MidpointRounding.AwayFromZero) / 2);
        //            }
        //            else
        //            {
        //                obj.countStar1 = 0;
        //                obj.countStar2 = 0;
        //                obj.countStar3 = 0;
        //                obj.countStar4 = 0;
        //                obj.countStar5 = 0;
        //                obj.star1 = 0;
        //                obj.star2 = 0;
        //                obj.star3 = 0;
        //                obj.star4 = 0;
        //                obj.star5 = 0;
        //                obj.star = 0;
        //            }
        //            def.metadata = obj;

        //            if (paging.page_size > 0)
        //            {
        //                if (paging.order_by != null)
        //                {
        //                    data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                }
        //                else
        //                {
        //                    data = data.OrderBy("ProductReviewId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                }
        //            }
        //            else
        //            {
        //                if (paging.order_by != null)
        //                {
        //                    data = data.OrderBy(paging.order_by);
        //                }
        //                else
        //                {
        //                    data = data.OrderBy("ProductReviewId desc");
        //                }
        //            }

        //            if (paging.select != null && paging.select != "")
        //            {
        //                paging.select = "new(" + paging.select + ")";
        //                paging.select = HttpUtility.UrlDecode(paging.select);
        //                def.data = data.Select(paging.select);
        //            }
        //            else
        //                def.data = data.ToList();

        //            return Ok(def);
        //        }
        //    }
        //    else
        //    {
        //        def.meta = new Meta(400, "Bad Request");
        //        return Ok(def);
        //    }
        //}

        //[HttpPost("PostProductReviews/{ProductId}")]
        //public async Task<IActionResult> PostProductReviews(ProductReviewDTO data, int ProductId)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            def.meta = new Meta(400, "Lỗi dữ liệu!");
        //            return Ok(def);
        //        }

        //        if (data.NumberStar == null)
        //        {
        //            def.meta = new Meta(211, "Bạn chưa nhập Điểm đánh giá!");
        //            return Ok(def);
        //        }
        //        if (data.Contents == null || data.Contents == "")
        //        {
        //            def.meta = new Meta(211, "Bạn chưa nhập Nội dung đánh giá!");
        //            return Ok(def);
        //        }
        //        if (data.Name == null || data.Name == "")
        //        {
        //            def.meta = new Meta(211, "Bạn chưa nhập Tên!");
        //            return Ok(def);
        //        }

        //        if (data.Email == null || data.Email == "")
        //        {
        //            def.meta = new Meta(211, "Bạn chưa nhập Email!");
        //            return Ok(def);
        //        }

        //        using (var db = new IOITDataContext())
        //        {
        //            using (var transaction = db.Database.BeginTransaction())
        //            {
        //                //Check xem đã đánh giá chưa
        //                var pr = await db.ProductReview.Where(e => e.ProductId == ProductId 
        //                && e.Email.Trim().ToLower().Equals(data.Email.Trim().ToLower())
        //                && e.Status == (int)Const.Status.OK).FirstOrDefaultAsync();
        //                if(pr != null)
        //                {
        //                    def.meta = new Meta(212, "Bạn đã đánh giá sản phẩm!");
        //                    return Ok(def);
        //                }
        //                ProductReview productReview = new ProductReview();
        //                productReview.ProductId = ProductId;
        //                productReview.Contents = data.Contents;
        //                productReview.NumberStar = data.NumberStar;
        //                productReview.Email = data.Email;
        //                productReview.Name = data.Name;
        //                productReview.CreatedAt = DateTime.Now;
        //                productReview.UpdatedAt = DateTime.Now;
        //                productReview.Status = (int)Const.Status.NORMAL;
        //                db.ProductReview.Add(productReview);
        //                try
        //                {
        //                    await db.SaveChangesAsync();
        //                    data.ProductReviewId = productReview.ProductReviewId;

        //                    transaction.Commit();
        //                    def.meta = new Meta(200, "Thêm đánh giá sản phẩm thành công!");
        //                    def.data = data;
        //                    return Ok(def);
        //                }
        //                catch (DbUpdateException e)
        //                {
        //                    log.Error("DbUpdateException:" + e);
        //                    transaction.Rollback();
        //                    def.meta = new Meta(400, "Đã xảy ra lỗi. Xin vui lòng thử lại!");
        //                    return Ok(def);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error("Error:" + e);
        //        def.meta = new Meta(500, "Hệ thống xảy ra lỗi. Xin vui lòng thử lại sau!");
        //        return Ok(def);
        //    }
        //}

        [HttpGet("GetProductByCate/{CategoryId}")]
        public IActionResult GetByPageProduct([FromQuery] FilteredPagination paging, int CategoryId)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    //IQueryable<Product> data = db.Product.Where(c => c.Status == (int)Const.Status.NORMAL);\
                    IQueryable<Product> data = (from cm in db.CategoryMapping
                                                join p in db.Product on cm.TargetId equals p.ProductId
                                                where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                                                && cm.CategoryId == CategoryId
                                                && cm.Status != (int)Const.Status.DELETED
                                                && p.Status == (int)Const.Status.NORMAL
                                                select p);

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
                            data = data.OrderBy("ProductId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("ProductId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select);
                    }
                    else
                        def.data = data.Select(e => new
                        {
                            e.ProductId,
                            e.Code,
                            e.Name,
                            e.StockQuantity,
                            e.PriceSale,
                            e.PriceImport,
                            e.PriceSpecial,
                            e.PriceOther,
                            e.Discount,
                            e.Image,
                            e.Url,
                            e.ManufacturerId,
                            e.UpdatedAt,
                            e.PointStar
                        }).ToList();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        [HttpGet("GetByPageCateProduct/{CategoryId}")]
        public IActionResult GetByPageCateProduct([FromQuery] FilteredPagination paging, int CategoryId)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    //IQueryable<Product> data = db.Product.Where(c => c.Status == (int)Const.Status.NORMAL);\
                    IQueryable<Product> data = (from cm in db.CategoryMapping
                                                join p in db.Product on cm.TargetId equals p.ProductId
                                                where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                                                && cm.CategoryId == CategoryId
                                                && cm.Status != (int)Const.Status.DELETED
                                                && p.Status == (int)Const.Status.NORMAL
                                                select p);

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
                            data = data.OrderBy("ProductId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("ProductId desc");
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
                        //var result = data.Select(e => new {
                        //    e.ProductId,
                        //    e.Code,
                        //    e.Name,
                        //    e.StockQuantity,
                        //    e.PriceSale,
                        //    e.PriceImport,
                        //    e.PriceSpecial,
                        //    e.PriceOther,
                        //    e.Discount,
                        //    e.Image,
                        //    e.Url,
                        //    e.ManufacturerId,
                        //    Status = e.Status
                        //}).ToList();

                        var CustomerId = HttpContext.Session.GetInt32("CustomerId");

                        foreach (var item in data)
                        {
                            if (CustomerId != null)
                            {
                                ProductCustomer productCustomer = db.ProductCustomer.Where(pc => pc.TargetId == item.ProductId && pc.CustomerId == CustomerId && pc.TargetType == (int)Const.TypeProductCustomer.LOVE && pc.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (productCustomer != null) item.Status = 10;
                            }

                            var listProductReView = db.ProductReview.Where(pr => pr.ProductId == item.ProductId && pr.Status != (int)Const.Status.DELETED);
                            int item_count = listProductReView.Count();
                            if (item_count > 0)
                            {
                                int countStar1 = listProductReView.Where(d => d.NumberStar == 1).Count();
                                int countStar2 = listProductReView.Where(d => d.NumberStar == 2).Count();
                                int countStar3 = listProductReView.Where(d => d.NumberStar == 3).Count();
                                int countStar4 = listProductReView.Where(d => d.NumberStar == 4).Count();
                                int countStar5 = listProductReView.Where(d => d.NumberStar == 5).Count();

                                float star = (float)(countStar1 + (countStar2 * 2) + (countStar3 * 3) + (countStar4 * 4) + (countStar5 * 5)) / (float)item_count;
                                item.PointStar = (float)(Math.Round(star * 2, MidpointRounding.AwayFromZero) / 2);
                            }
                            else
                            {
                                item.PointStar = 0;
                            }
                        }

                        def.data = data.ToList();
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
    }
}