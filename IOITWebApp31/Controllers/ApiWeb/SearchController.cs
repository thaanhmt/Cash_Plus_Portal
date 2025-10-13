using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet("news/{type}")]
        public IActionResult GetByPageNews([FromRoute] int type, [FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    //IQueryable<News> data = db.News.Where(c => c.Status == (int)Const.Status.NORMAL);
                    int CategoryId = 3219;
                    IQueryable<News> data = db.News.Where(c => c.Status == (int)Const.Status.NORMAL);
                    if (type == 1)
                    {
                        data = (from n in db.News
                                join cn in db.CategoryMapping on n.NewsId equals cn.TargetId
                                where cn.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                && n.Status == (int)Const.Status.NORMAL
                                && cn.Status == (int)Const.Status.NORMAL
                                && cn.CategoryId != CategoryId
                                select n).OrderByDescending(e => e.CreatedAt);
                    }
                    else if (type == 4)
                    {
                        data = (from n in db.News
                                join cn in db.CategoryMapping on n.NewsId equals cn.TargetId
                                where cn.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                && n.Status == (int)Const.Status.NORMAL
                                && cn.Status == (int)Const.Status.NORMAL
                                && cn.CategoryId == CategoryId
                                select n).OrderByDescending(e => e.CreatedAt);
                    }

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
                            data = data.OrderBy("NewsId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("NewsId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select);
                    }
                    else
                        def.data = data.ToList();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        [HttpGet("product")]
        public async Task<IActionResult> GetByPageProduct([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    //var result = await db.Product.Where(c => EF.Functions.FreeText(c.Name, paging.search)).ToListAsync();

                    def.meta = new Meta(200, "Success");
                    IQueryable<ProductDT> data = (from cm in db.CategoryMapping
                                                  join pro in db.Product on cm.TargetId equals pro.ProductId
                                                  where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                                                  //&& cm.CategoryId == search.sCategory
                                                  && pro.Name.ToLower().Contains(paging.search.ToLower())
                                                  && pro.Status == (int)Const.Status.NORMAL
                                                  && cm.Status != (int)Const.Status.DELETED
                                                  select new ProductDT
                                                  {
                                                      ProductId = pro.ProductId,
                                                      Code = pro.Code,
                                                      Name = pro.Name,
                                                      PriceSale = pro.PriceSale,
                                                      PriceSpecial = pro.PriceSpecial,
                                                      Discount = pro.Discount,
                                                      Image = pro.Image,
                                                      Url = pro.Url,
                                                      ManufacturerId = pro.ManufacturerId,
                                                      TrademarkId = pro.TrademarkId,
                                                      Status = pro.Status,
                                                      PointStar = pro.PointStar,
                                                      DateStartActive = pro.DateStartActive,
                                                      UpdatedAt = pro.UpdatedAt,
                                                      CategoryId = cm.CategoryId
                                                  }).GroupBy(e => e.ProductId).Select(e => e.First());

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
                        var listData = data.ToList();
                        int customerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
                        if (customerId != -1)
                        {
                            foreach (var item in listData)
                            {
                                ProductCustomer productCustomer = db.ProductCustomer.Where(pc => pc.TargetId == item.ProductId && pc.CustomerId == customerId && pc.TargetType == (int)Const.TypeProductCustomer.LOVE && pc.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (productCustomer != null) item.Status = 10;
                            }
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


    }
}