using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    public class ProductController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("product", "product");
        private static string functionCode = "QLSP";

        // GET: api/Product
        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
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
                    string cat = "CategoryId";
                    def.meta = new Meta(200, "Success");
                    IQueryable<Product> data = db.Product.Where(c => c.Status != (int)Const.Status.DELETED);

                    //foreach (var item in data)
                    //{
                    //    var pLink = db.PermaLink.Where(e => e.Slug == item.Url.Trim().ToLower() && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    //    if (pLink == null)
                    //    {
                    //        //Thêm permalink
                    //        PermaLink permaLink = new PermaLink();
                    //        permaLink.PermaLinkId = Guid.NewGuid();
                    //        permaLink.Slug = item.Url.Trim().ToLower();
                    //        permaLink.TargetId = item.ProductId;
                    //        permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_PRODUCT;
                    //        permaLink.CreatedAt = DateTime.Now;
                    //        permaLink.UpdatedAt = DateTime.Now;
                    //        permaLink.Status = (int)Const.Status.NORMAL;
                    //        await db.PermaLink.AddAsync(permaLink);
                    //        await db.SaveChangesAsync();
                    //    }
                    //}

                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    var aaa = paging.query.IndexOf(cat);
                    if (aaa != -1)
                    {
                        string[] arrListStr = paging.query.Split("and");
                        string[] arrListStr1 = arrListStr[1].Split("=");

                        data = (from cm in db.CategoryMapping
                                join pro in db.Product on cm.TargetId equals pro.ProductId
                                where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                                && cm.CategoryId == Int16.Parse(arrListStr1[1])
                                && pro.Status == (int)Const.Status.NORMAL
                                && cm.Status != (int)Const.Status.DELETED
                                select pro
                                );
                    }
                    else
                    {
                        data = data.Where(paging.query);
                    }
                    MetaDataDT metaDataDT = new MetaDataDT();
                    metaDataDT.Sum = data.Count();
                    metaDataDT.Normal = data.Where(e => e.Status == 1).Count();
                    metaDataDT.Temp = data.Where(e => e.Status == 10).Count();

                    def.metadata = metaDataDT;

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
                        def.data = data.Select(paging.select).ToDynamicList();
                    }
                    else
                        //def.data = data.ToList();
                        def.data = data.Select(e => new
                        {
                            e.ProductId,
                            e.Code,
                            e.Name,
                            e.Description,
                            e.Contents,
                            e.Image,
                            e.Url,
                            e.DateStartActive,
                            e.DateStartOn,
                            e.DateEndOn,
                            e.IsHome,
                            e.IsHot,
                            e.IsSale,
                            e.StockQuantity,
                            e.PriceSale,
                            e.PriceImport,
                            e.PriceSpecial,
                            e.PriceOther,
                            e.ManufacturerId,
                            e.ProductAttributes,
                            e.ProductNote,
                            e.NoteTech,
                            e.NotePromotion,
                            e.ViewNumber,
                            e.LikeNumber,
                            e.CommentNumber,
                            e.LanguageId,
                            e.CompanyId,
                            e.WebsiteId,
                            e.MetaTitle,
                            e.MetaKeyword,
                            e.MetaDescription,
                            e.CreatedAt,
                            e.UpdatedAt,
                            e.UserId,
                            e.Status,
                            e.TypeProduct,
                            e.Introduce,
                            e.Feature,
                            e.GuaranteeProduct,
                            e.OriginProduct,
                            e.Configuration,
                            e.Discount,
                            listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.ProductId && cp.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT && cp.Status != (int)Const.Status.DELETED).Select(p => new
                            {
                                p.CategoryId,
                                Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
                                Check = true
                            }).ToList(),
                            listTag = db.Tag.Where(t => t.TargetId == e.ProductId && t.Status != (int)Const.Status.DELETED).Select(p => new
                            {
                                p.TagId,
                                p.Name,
                                Check = true
                            }).ToList(),
                            listImage = db.ProductImage.Where(pi => pi.ProductId == e.ProductId && pi.Status != (int)Const.Status.DELETED).Select(pi => new
                            {
                                pi.ProductImageId,
                                pi.Name,
                                pi.Image,
                                pi.Location,
                                pi.IsImageMain,
                                pi.Status
                            }).ToList(),
                            listDocument = db.Attactment.Where(pi => pi.TargetId == e.ProductId && pi.TargetType == (int)Const.TypeAttachment.FILE_DOC && pi.Status != (int)Const.Status.DELETED).Select(pi => new
                            {
                                pi.Name,
                                pi.Url,
                                pi.Status,
                                pi.TargetId,
                                pi.TargetType,
                                pi.IsImageMain,
                                pi.Thumb,
                                pi.CreatedId,
                                pi.AttactmentId,
                            }).ToList(),
                            listProductAttribute = db.ProductAttribuite.Where(pa => pa.ProductId == e.ProductId && pa.Status != (int)Const.Status.DELETED).Select(pa => new
                            {
                                pa.ProductAttributeId,
                                pa.ProductId,
                                pa.Code,
                                pa.Image,
                                pa.IsDownload,
                                pa.IsVirtual,
                                pa.IsBranch,
                                pa.Price,
                                pa.PriceSpecial,
                                pa.PriceSpecialStart,
                                pa.PriceSpecialEnd,
                                pa.BranchStatus,
                                pa.Description,
                                pa.Weight,
                                pa.Length,
                                pa.Width,
                                pa.Height,
                                pa.MinStock,
                                pa.MaxStock,
                                pa.Location,
                                pa.CreatedAt,
                                pa.UserId,
                                pa.Status,
                                listAttribute = db.AttributeMapping.Where(am => am.ProductAttributeId == pa.ProductAttributeId && e.Status != (int)Const.Status.DELETED).Select(am => new
                                {
                                    am.AttributeMappingId,
                                    am.AttributeId,
                                    am.ProductAttributeId,
                                    am.AttributeValueId,
                                    am.IsMain,
                                    am.IsView,
                                    am.CreatedAt,
                                    am.UserId,
                                    am.Status,
                                    listAttributeChild = db.Attribute.Where(a => a.AttributeParentId == am.AttributeId).Select(a => new
                                    {
                                        a.AttributeId,
                                        a.Name,
                                    }).ToList(),
                                }).ToList(),
                            }).ToList(),
                            trademark = db.Manufacturer.Where(c => c.ManufacturerId == e.TrademarkId && c.Status != (int)Const.Status.DELETED).Select(c => new
                            {
                                c.ManufacturerId,
                                c.Name
                            }).FirstOrDefault(),
                            TrademarkId = db.Manufacturer.Where(c => c.ManufacturerId == e.TrademarkId && c.Status != (int)Const.Status.DELETED).FirstOrDefault() != null ? e.TrademarkId : null,
                            listRelated = db.Related.Where(r => r.TargetId == e.ProductId && r.TargetType == (int)Const.TypeRelated.PRODUCT_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new
                            {
                                r.TargetRelatedId
                            }).ToList(),
                            SumProductReviewInit = db.ProductReview.Where(pr => pr.ProductId == e.ProductId && pr.Status == (int)Const.Status.NORMAL).Count(),
                            SumProductReview = db.ProductReview.Where(pr => pr.ProductId == e.ProductId && pr.Status != (int)Const.Status.DELETED).Count(),
                            language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new
                            {
                                l.LanguageId,
                                l.Flag,
                                l.Name,
                                l.Code
                            }).FirstOrDefault(),
                            listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.ProductId || a.TargetId2 == e.ProductId)
                            && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_PRODUCT && a.Status != (int)Const.Status.DELETED).Select(a => new
                            {
                                lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new
                                {
                                    l.LanguageId,
                                    l.Name,
                                    l.Flag
                                }).FirstOrDefault(),
                                product = db.Product.Where(l => (l.ProductId == a.TargetId1 || l.ProductId == a.TargetId2) && l.ProductId != e.ProductId).Select(l => new
                                {
                                    l.ProductId,
                                    l.Name,
                                    l.Url
                                }).FirstOrDefault(),
                            }).ToList(),

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

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
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
                    Product data = await db.Product.FindAsync(id);

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

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] ProductDTO data)
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

                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    //if(data.TypeProduct == (int)Const.TypeProduct.KOI)
                    //{
                    //    var CheckCode = db.Product.Where(p => p.Code == data.Code && p.TypeProduct == data.TypeProduct && p.ProductId != data.ProductId && p.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    //    if (CheckCode != null)
                    //    {
                    //        def.meta = new Meta(212, "Code Exist!");
                    //        return Ok(def);
                    //    }
                    //}

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Product product = await db.Product.FindAsync(id);
                        if (product == null)
                        {
                            def.meta = new Meta(404, "Not found!");
                            return Ok(def);
                        }
                        string url = data.Url == null ? Utils.NonUnicode(data.Name) : data.Url;
                        url = url.Trim().ToLower();
                        if (product.Url != url)
                        {
                            //check xem trùng link ko
                            var pLink = db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (pLink != null)
                            {
                                def.meta = new Meta(232, "Link đã tồn tại!");
                                return Ok(def);
                            }
                            //cập nhật thay link cũ
                            var permaLink = db.PermaLink.Where(e => e.Slug == product.Url).FirstOrDefault();
                            if (permaLink != null)
                            {
                                permaLink.Slug = url;
                                permaLink.TargetId = product.ProductId;
                                permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_PRODUCT;
                                permaLink.UpdatedAt = DateTime.Now;
                                permaLink.Status = (int)Const.Status.NORMAL;
                                db.PermaLink.Update(permaLink);
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                PermaLink permaLink1 = new PermaLink();
                                permaLink1.PermaLinkId = Guid.NewGuid();
                                permaLink1.Slug = product.Url;
                                permaLink1.TargetId = product.ProductId;
                                permaLink1.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_PRODUCT;
                                permaLink1.CreatedAt = DateTime.Now;
                                permaLink1.UpdatedAt = DateTime.Now;
                                permaLink1.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink1);
                                await db.SaveChangesAsync();
                            }

                        }
                        else
                        {
                            var permaLink = db.PermaLink.Where(e => e.Slug == product.Url).FirstOrDefault();
                            if (permaLink == null)
                            {
                                PermaLink permaLink1 = new PermaLink();
                                permaLink1.PermaLinkId = Guid.NewGuid();
                                permaLink1.Slug = product.Url;
                                permaLink1.TargetId = product.ProductId;
                                permaLink1.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_PRODUCT;
                                permaLink1.CreatedAt = DateTime.Now;
                                permaLink1.UpdatedAt = DateTime.Now;
                                permaLink1.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink1);
                                await db.SaveChangesAsync();
                            }
                        }

                        product.Code = data.Code;
                        product.Name = data.Name;
                        product.Description = data.Description;
                        product.Contents = data.Contents;
                        product.IsHome = data.IsHome;
                        product.IsHot = data.IsHot;
                        product.IsSale = data.IsSale;
                        product.StockQuantity = data.StockQuantity;
                        product.PriceSale = data.PriceSale;
                        product.PriceImport = data.PriceImport;
                        product.Discount = data.Discount;
                        product.Introduce = data.Introduce;
                        product.Feature = data.Feature;
                        product.Configuration = data.Configuration;
                        product.OriginProduct = data.OriginProduct;
                        product.GuaranteeProduct = data.GuaranteeProduct;
                        if (product.PriceSale != null && product.Discount != null)
                        {
                            product.PriceSpecial = product.PriceSale * (100 - product.Discount) / 100;
                        }
                        else
                        {
                            product.PriceSpecial = product.PriceSale;
                        }
                        product.PriceOther = data.PriceOther;
                        product.Image = null;
                        product.TypeProduct = data.TypeProduct;
                        product.Url = data.Url != null ? data.Url : Utils.NonUnicode(data.Name);
                        product.DateStartActive = data.DateStartActive != null ? data.DateStartActive : DateTime.Now;
                        product.DateStartOn = data.DateStartOn != null ? data.DateStartOn : DateTime.Now;
                        product.DateEndOn = data.DateEndOn != null ? data.DateEndOn : DateTime.Now;
                        product.ProductAttributes = data.ProductAttributes;
                        product.ProductNote = data.ProductNote;
                        product.NoteTech = data.NoteTech;
                        product.NotePromotion = data.NotePromotion;
                        product.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 0;
                        product.LikeNumber = data.LikeNumber != null ? data.LikeNumber : 0;
                        product.CommentNumber = data.CommentNumber != null ? data.CommentNumber : 0;
                        product.MetaTitle = data.MetaTitle;
                        product.MetaKeyword = data.MetaKeyword;
                        product.MetaDescription = data.MetaDescription;
                        product.TypeImagePromotionId = data.TypeImagePromotionId;
                        product.ManufacturerId = data.ManufacturerId;
                        product.TrademarkId = data.TrademarkId;

                        product.WebsiteId = data.WebsiteId;

                        product.UserId = userId;
                        product.UpdatedAt = DateTime.Now;
                        product.Status = data.Status;
                        db.Product.Update(product);

                        //Category mapping
                        if (data.listCategory != null)
                        {
                            foreach (var item in data.listCategory)
                            {
                                CategoryMapping exist = db.CategoryMapping.Where(cm => cm.CategoryId == item.CategoryId && cm.TargetId == id && cm.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (exist == null)
                                {
                                    if (item.Check == true)
                                    {
                                        CategoryMapping categoryNewsMapping = new CategoryMapping();
                                        categoryNewsMapping.CategoryId = item.CategoryId;
                                        categoryNewsMapping.TargetId = product.ProductId;
                                        categoryNewsMapping.TargetType = (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT;
                                        categoryNewsMapping.Location = db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList() != null ? db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList().Count : 1;
                                        categoryNewsMapping.Status = (int)Const.Status.NORMAL;
                                        categoryNewsMapping.CreatedAt = DateTime.Now;
                                        await db.CategoryMapping.AddAsync(categoryNewsMapping);
                                    }
                                }
                                else
                                {
                                    if (item.Check != true)
                                    {
                                        exist.Status = (int)Const.Status.DELETED;
                                        db.CategoryMapping.Update(exist);
                                    }
                                }
                            }
                        }

                        //Tag
                        if (data.listTag != null)
                        {
                            foreach (var item in data.listTag)
                            {
                                if (item.TagId == null)
                                {
                                    Tag tag = new Tag();
                                    tag.Name = item.Name;
                                    tag.TargetId = product.ProductId;
                                    tag.TargetType = (int)Const.TypeTag.TAG_PRODUCT;
                                    tag.Url = Utils.NonUnicode(item.Name);
                                    tag.UserId = data.UserId;
                                    tag.CreatedAt = DateTime.Now;
                                    tag.Status = (int)Const.Status.NORMAL;
                                    await db.Tag.AddAsync(tag);
                                }
                                else
                                {
                                    Tag exist = db.Tag.Where(t => t.TagId == item.TagId && t.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        if (item.Check == false)
                                        {
                                            exist.Status = (int)Const.Status.DELETED;
                                            db.Tag.Update(exist);
                                        }
                                    }
                                }
                            }
                        }

                        // file doc cument
                        if (data.listDocument != null)
                        {
                            foreach (var item in data.listDocument)
                            {
                                var pa = await db.Attactment.FindAsync(item.AttactmentId);
                                if (pa != null)
                                {
                                    pa.TargetId = item.TargetId;
                                    pa.TargetType = item.TargetType;
                                    pa.Name = item.Name;
                                    pa.Thumb = item.Thumb;
                                    pa.Url = item.Url;
                                    pa.Status = item.Status;
                                    pa.CreatedId = item.CreatedId;
                                    pa.CreatedAt = item.CreatedAt;
                                    db.Attactment.Update(pa);
                                }
                                else
                                {
                                    if (item.Status != (int)Const.Status.DELETED)
                                    {
                                        Attactment atm = new Attactment();
                                        atm.Name = item.Name;
                                        atm.TargetType = (int)Const.TypeAttachment.FILE_DOC;
                                        atm.TargetId = product.ProductId;
                                        atm.Url = item.Url;
                                        atm.CreatedAt = DateTime.Now;
                                        atm.CreatedId = userId;
                                        atm.Status = (int)Const.Status.NORMAL;
                                        await db.Attactment.AddAsync(atm);
                                        await db.SaveChangesAsync();


                                    }

                                }


                            }
                        }


                        //add list Image Product
                        if (data.listImage != null)
                        {
                            int? k = data.listImage.Max(r => r.Location);
                            foreach (var item in data.listImage)
                            {
                                if (item.ProductImageId != null)
                                {
                                    var imageExist = db.ProductImage.Find(item.ProductImageId);
                                    if (imageExist != null)
                                    {
                                        if (item.Status == (int)Const.Status.DELETED)
                                        {
                                            imageExist.Status = (int)Const.Status.DELETED;
                                        }
                                        else
                                        {
                                            imageExist.IsImageMain = item.IsImageMain;
                                        }
                                    }
                                    db.ProductImage.Update(imageExist);
                                }
                                else
                                {
                                    ProductImage productImage = new ProductImage();
                                    productImage.Name = product.Name + "-" + k;
                                    productImage.Image = item.Image;
                                    productImage.ProductId = product.ProductId;
                                    productImage.IsImageMain = item.IsImageMain;
                                    productImage.Location = k;
                                    productImage.UserId = data.UserId;
                                    productImage.CreatedAt = DateTime.Now;
                                    productImage.UserId = userId;
                                    productImage.Status = (int)Const.Status.NORMAL;
                                    await db.ProductImage.AddAsync(productImage);
                                    k++;
                                }
                                if (item.IsImageMain == true && item.Status != (int)Const.Status.DELETED)
                                {
                                    product.Image = item.Image;
                                    db.Product.Update(product);
                                }
                            }
                        }

                        //Attribute
                        if (data.listProductAttribute != null)
                        {
                            foreach (var item in data.listProductAttribute)
                            {
                                int productAttributeId = 0;
                                if (item.ProductAttributeId != null)
                                {
                                    var pa = await db.ProductAttribuite.FindAsync(item.ProductAttributeId);
                                    if (pa != null)
                                    {
                                        pa.Code = item.Code;
                                        pa.Image = item.Image;
                                        pa.IsDownload = item.IsDownload;
                                        pa.IsVirtual = item.IsVirtual;
                                        pa.IsBranch = item.IsBranch;
                                        pa.Price = item.Price;
                                        pa.PriceSpecial = item.PriceSpecial;
                                        pa.PriceSpecialStart = item.PriceSpecialStart;
                                        pa.PriceSpecialEnd = item.PriceSpecialEnd;
                                        pa.BranchStatus = item.BranchStatus;
                                        pa.Description = item.Description;
                                        pa.Weight = item.Weight;
                                        pa.Length = item.Length;
                                        pa.Width = item.Width;
                                        pa.Height = item.Height;
                                        pa.MinStock = item.MinStock;
                                        pa.MaxStock = item.MaxStock;
                                        pa.Location = item.Location;
                                        pa.UserId = userId;
                                        pa.Status = item.Status;
                                        db.ProductAttribuite.Update(pa);
                                        productAttributeId = pa.ProductAttributeId;
                                    }
                                }
                                else
                                {
                                    if (item.Status != (int)Const.Status.DELETED)
                                    {
                                        ProductAttribuite productAttribuite = new ProductAttribuite();
                                        productAttribuite.ProductId = product.ProductId;
                                        productAttribuite.Code = item.Code;
                                        productAttribuite.Image = item.Image;
                                        productAttribuite.IsDownload = item.IsDownload;
                                        productAttribuite.IsVirtual = item.IsVirtual;
                                        productAttribuite.IsBranch = item.IsBranch;
                                        productAttribuite.Price = item.Price;
                                        productAttribuite.PriceSpecial = item.PriceSpecial;
                                        productAttribuite.PriceSpecialStart = item.PriceSpecialStart;
                                        productAttribuite.PriceSpecialEnd = item.PriceSpecialEnd;
                                        productAttribuite.BranchStatus = item.BranchStatus;
                                        productAttribuite.Description = item.Description;
                                        productAttribuite.Weight = item.Weight;
                                        productAttribuite.Length = item.Length;
                                        productAttribuite.Width = item.Width;
                                        productAttribuite.Height = item.Height;
                                        productAttribuite.MinStock = item.MinStock;
                                        productAttribuite.MaxStock = item.MaxStock;
                                        productAttribuite.Location = 1;
                                        productAttribuite.CreatedAt = DateTime.Now;
                                        productAttribuite.UserId = userId;
                                        productAttribuite.Status = (int)Const.Status.NORMAL;
                                        await db.ProductAttribuite.AddAsync(productAttribuite);
                                        await db.SaveChangesAsync();
                                        productAttributeId = productAttribuite.ProductAttributeId;

                                    }
                                }

                                //map attribute
                                if (productAttributeId != 0)
                                {
                                    foreach (var itemA in item.listAttribute)
                                    {
                                        if (itemA.AttributeMappingId != null)
                                        {
                                            var am = await db.AttributeMapping.FindAsync(itemA.AttributeMappingId);
                                            if (am != null)
                                            {
                                                am.ProductAttributeId = productAttributeId;
                                                am.AttributeId = itemA.AttributeId;
                                                am.AttributeValueId = itemA.AttributeValueId;
                                                am.IsMain = itemA.IsMain;
                                                am.IsView = itemA.IsView;
                                                am.UserId = userId;
                                                am.Status = (int)Const.Status.NORMAL;
                                                db.AttributeMapping.Update(am);
                                            }
                                            else if (itemA.Status != (int)Const.Status.DELETED)
                                            {
                                                AttributeMapping attributeMapping = new AttributeMapping();
                                                attributeMapping.AttributeMappingId = Guid.NewGuid();
                                                attributeMapping.ProductAttributeId = productAttributeId;
                                                attributeMapping.AttributeId = itemA.AttributeId;
                                                attributeMapping.AttributeValueId = itemA.AttributeValueId;
                                                attributeMapping.IsMain = itemA.IsMain;
                                                attributeMapping.IsView = itemA.IsView;
                                                attributeMapping.CreatedAt = DateTime.Now;
                                                attributeMapping.UserId = userId;
                                                attributeMapping.Status = (int)Const.Status.NORMAL;
                                                await db.AttributeMapping.AddAsync(attributeMapping);
                                            }
                                        }
                                        else
                                        {
                                            if (itemA.Status != (int)Const.Status.DELETED)
                                            {
                                                AttributeMapping attributeMapping = new AttributeMapping();
                                                attributeMapping.AttributeMappingId = Guid.NewGuid();
                                                attributeMapping.ProductAttributeId = productAttributeId;
                                                attributeMapping.AttributeId = itemA.AttributeId;
                                                attributeMapping.AttributeValueId = itemA.AttributeValueId;
                                                attributeMapping.IsMain = itemA.IsMain;
                                                attributeMapping.IsView = itemA.IsView;
                                                attributeMapping.CreatedAt = DateTime.Now;
                                                attributeMapping.UserId = userId;
                                                attributeMapping.Status = (int)Const.Status.NORMAL;
                                                await db.AttributeMapping.AddAsync(attributeMapping);
                                            }
                                        }
                                    }
                                }
                            }
                            await db.SaveChangesAsync();
                        }

                        //Sản phẩm gợi ý
                        List<Related> listRelated = db.Related.Where(r => r.TargetId == product.ProductId && r.TargetType == (int)Const.TypeRelated.PRODUCT_PRODUCT && r.Status != (int)Const.Status.DELETED).ToList();
                        if (listRelated != null)
                        {
                            listRelated.ForEach(lr => lr.Status = (int)Const.Status.DELETED);
                        }

                        if (data.listRelated != null)
                        {
                            foreach (var item in data.listRelated)
                            {
                                Related related = new Related();
                                related.TargetId = product.ProductId;
                                related.TargetRelatedId = item.TargetRelatedId;
                                related.TargetType = (int)Const.TypeRelated.PRODUCT_PRODUCT;
                                related.Location = item.Location;
                                related.CreatedAt = DateTime.Now;
                                related.UserId = userId;
                                related.Status = (int)Const.Status.NORMAL;
                                await db.Related.AddAsync(related);
                            }
                        }

                        CategoryMapping categoryMapping = db.CategoryMapping.Where(cm => cm.CategoryId == -1 && cm.TargetId == id && cm.TargetType == (int)Const.TypeOrderBy.PRODUCT_IS_HOME && cm.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (product.IsHome == true)
                        {
                            if (categoryMapping == null)
                            {
                                CategoryMapping cm = new CategoryMapping();
                                cm.CategoryId = -1;
                                cm.TargetId = data.ProductId;
                                cm.TargetType = (int)Const.TypeOrderBy.PRODUCT_IS_HOME;
                                cm.Location = 99;
                                cm.CreatedAt = DateTime.Now;
                                cm.Status = (int)Const.Status.NORMAL;
                                await db.CategoryMapping.AddAsync(cm);
                            }
                        }
                        else
                        {
                            if (categoryMapping != null)
                            {
                                categoryMapping.Status = (int)Const.Status.DELETED;
                                db.Update(categoryMapping);
                            }
                        }
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.ProductId > 0)
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
                            if (!ProductExists(data.ProductId))
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

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> PostProduct(ProductDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
            int websiteId = int.Parse(identity.Claims.Where(c => c.Type == "WebsiteId").Select(c => c.Value).SingleOrDefault());
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

                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    if (data.Code != null && data.Code != "")
                    {
                        var CheckCode = db.Product.Where(p => p.Code.Trim().Equals(data.Code.Trim()) && p.LanguageId == languageId
                            && p.TypeProduct == data.TypeProduct && p.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (CheckCode != null)
                        {
                            def.meta = new Meta(212, "Code Exist!");
                            return Ok(def);
                        }
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //check xem trùng link ko
                        string url = data.Url == null ? Utils.NonUnicode(data.Name) : data.Url;
                        url = url.Trim().ToLower();
                        var pLink = db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (pLink != null)
                        {
                            def.meta = new Meta(232, "Link đã tồn tại!");
                            return Ok(def);
                        }

                        Product product = new Product();
                        product.Code = data.Code;
                        product.Name = data.Name;
                        product.Description = data.Description == null ? "" : data.Description;
                        product.Contents = data.Contents;
                        product.IsHome = data.IsHome;
                        product.IsHot = data.IsHot;
                        product.IsSale = data.IsSale;
                        product.StockQuantity = data.StockQuantity;
                        product.PriceSale = data.PriceSale;
                        product.PriceImport = data.PriceImport;
                        product.Discount = data.Discount;
                        if (product.PriceSale != null && product.Discount != null)
                        {
                            product.PriceSpecial = product.PriceSale * (100 - product.Discount) / 100;
                        }
                        else
                        {
                            product.PriceSpecial = product.PriceSale;
                        }
                        product.PriceSpecial = data.PriceSpecial;
                        product.Introduce = data.Introduce;
                        product.Feature = data.Feature;
                        product.Configuration = data.Configuration;
                        product.OriginProduct = data.OriginProduct;
                        product.GuaranteeProduct = data.GuaranteeProduct;
                        product.PriceOther = data.PriceOther;
                        product.Image = data.Image;
                        product.TypeProduct = data.TypeProduct;
                        product.Url = data.Url != null ? data.Url : Utils.NonUnicode(data.Name);
                        product.DateStartActive = data.DateStartActive != null ? data.DateStartActive : DateTime.Now;
                        product.DateStartOn = data.DateStartOn != null ? data.DateStartOn : DateTime.Now;
                        product.DateEndOn = data.DateEndOn != null ? data.DateEndOn : DateTime.Now;
                        product.ProductAttributes = data.ProductAttributes;
                        product.ProductNote = data.ProductNote;
                        product.NoteTech = data.NoteTech;
                        product.NotePromotion = data.NotePromotion;
                        product.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 0;
                        product.LikeNumber = data.LikeNumber != null ? data.LikeNumber : 0;
                        product.CommentNumber = data.CommentNumber != null ? data.CommentNumber : 0;
                        product.MetaTitle = data.MetaTitle;
                        product.MetaKeyword = data.MetaKeyword;
                        product.MetaDescription = data.MetaDescription;
                        product.TypeImagePromotionId = data.TypeImagePromotionId;
                        product.ManufacturerId = data.ManufacturerId;
                        product.TrademarkId = data.TrademarkId;
                        product.LanguageId = data.LanguageId != null ? data.LanguageId : languageId;

                        //Nếu ko truyền vào website thì chọn website mạc định
                        if (data.WebsiteId == null)
                        {
                            //Nếu website mạc định = 0 thì cảnh báo tạo website
                            if (websiteId == 0)
                            {
                                def.meta = new Meta(210, "Website default is null");
                                return Ok(def);
                            }
                            else
                                data.WebsiteId = websiteId;
                        }
                        else
                            product.WebsiteId = data.WebsiteId;
                        product.CompanyId = companyId;
                        product.UserId = userId;
                        product.CreatedAt = DateTime.Now;
                        product.UpdatedAt = DateTime.Now;
                        product.Status = (int)Const.Status.NORMAL;

                        await db.Product.AddAsync(product);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.ProductId = product.ProductId;

                            if (product.IsHome == true)
                            {
                                CategoryMapping categoryMapping = new CategoryMapping();
                                categoryMapping.CategoryId = -1;
                                categoryMapping.TargetId = data.ProductId;
                                categoryMapping.TargetType = (int)Const.TypeOrderBy.PRODUCT_IS_HOME;
                                categoryMapping.Location = 99;
                                categoryMapping.CreatedAt = DateTime.Now;
                                categoryMapping.Status = (int)Const.Status.NORMAL;
                                await db.CategoryMapping.AddAsync(categoryMapping);
                            }

                            //add category mapping
                            if (data.listCategory != null)
                            {
                                foreach (var item in data.listCategory)
                                {
                                    CategoryMapping categoryNewsMapping = new CategoryMapping();
                                    categoryNewsMapping.CategoryId = item.CategoryId;
                                    categoryNewsMapping.TargetId = data.ProductId;
                                    categoryNewsMapping.TargetType = (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT;
                                    categoryNewsMapping.Location = db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList() != null ? db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList().Count : 1;
                                    categoryNewsMapping.CreatedAt = DateTime.Now;
                                    categoryNewsMapping.Status = (int)Const.Status.NORMAL;
                                    await db.CategoryMapping.AddAsync(categoryNewsMapping);
                                }
                            }
                            // add document
                            if (data.listDocument != null)
                            {
                                foreach (var item in data.listDocument)
                                {
                                    Attactment DocumentProduct = new Attactment();
                                    DocumentProduct.Name = item.Name;
                                    DocumentProduct.Url = item.Url;
                                    DocumentProduct.TargetId = data.ProductId;
                                    DocumentProduct.TargetType = (int)Const.TypeAttachment.FILE_DOC;
                                    DocumentProduct.CreatedId = data.UserId;
                                    DocumentProduct.CreatedAt = DateTime.Now;
                                    DocumentProduct.Status = (int)Const.Status.NORMAL;

                                    await db.Attactment.AddAsync(DocumentProduct);
                                }
                            }

                            //add tag
                            if (data.listTag != null)
                            {
                                foreach (var item in data.listTag)
                                {
                                    Tag tag = new Tag();
                                    tag.Name = item.Name;
                                    tag.Url = Utils.NonUnicode(item.Name);
                                    tag.TargetId = data.ProductId;
                                    tag.TargetType = (int)Const.TypeTag.TAG_PRODUCT;
                                    tag.WebsiteId = product.WebsiteId;
                                    tag.CompanyId = companyId;
                                    tag.UserId = data.UserId;
                                    tag.CreatedAt = DateTime.Now;
                                    tag.Status = (int)Const.Status.NORMAL;
                                    await db.Tag.AddAsync(tag);
                                }
                            }

                            //add list Image Product
                            if (data.listImage != null)
                            {
                                int k = 1;
                                foreach (var item in data.listImage)
                                {
                                    ProductImage productImage = new ProductImage();
                                    productImage.Name = product.Name + "-" + k;
                                    productImage.Image = item.Image;
                                    productImage.ProductId = product.ProductId;
                                    productImage.IsImageMain = item.IsImageMain;
                                    productImage.Location = k;
                                    productImage.UserId = data.UserId;
                                    productImage.CreatedAt = DateTime.Now;
                                    productImage.UserId = userId;
                                    productImage.Status = (int)Const.Status.NORMAL;
                                    await db.ProductImage.AddAsync(productImage);
                                    k++;

                                    if (item.IsImageMain == true)
                                    {
                                        product.Image = item.Image;
                                        db.Product.Update(product);
                                    }
                                }
                            }

                            //add product related
                            if (data.listRelated != null)
                            {
                                foreach (var item in data.listRelated)
                                {
                                    Related related = new Related();
                                    related.TargetId = product.ProductId;
                                    related.TargetRelatedId = item.TargetRelatedId;
                                    related.TargetType = (int)Const.TypeRelated.PRODUCT_PRODUCT;
                                    related.Location = item.Location;
                                    related.CreatedAt = DateTime.Now;
                                    related.UserId = userId;
                                    related.Status = (int)Const.Status.NORMAL;
                                    await db.Related.AddAsync(related);
                                }
                            }

                            //add product attribuite
                            if (data.listProductAttribute != null)
                            {
                                foreach (var item in data.listProductAttribute)
                                {
                                    if (item.Status != (int)Const.Status.DELETED)
                                    {
                                        ProductAttribuite productAttribuite = new ProductAttribuite();
                                        productAttribuite.ProductId = product.ProductId;
                                        productAttribuite.Code = item.Code;
                                        productAttribuite.Image = item.Image;
                                        productAttribuite.IsDownload = item.IsDownload;
                                        productAttribuite.IsVirtual = item.IsVirtual;
                                        productAttribuite.IsBranch = item.IsBranch;
                                        productAttribuite.Price = item.Price;
                                        productAttribuite.PriceSpecial = item.PriceSpecial;
                                        productAttribuite.PriceSpecialStart = item.PriceSpecialStart;
                                        productAttribuite.PriceSpecialEnd = item.PriceSpecialEnd;
                                        productAttribuite.BranchStatus = item.BranchStatus;
                                        productAttribuite.Description = item.Description;
                                        productAttribuite.Weight = item.Weight;
                                        productAttribuite.Length = item.Length;
                                        productAttribuite.Width = item.Width;
                                        productAttribuite.Height = item.Height;
                                        productAttribuite.MinStock = item.MinStock;
                                        productAttribuite.MaxStock = item.MaxStock;
                                        productAttribuite.Location = 1;
                                        productAttribuite.CreatedAt = DateTime.Now;
                                        productAttribuite.UserId = userId;
                                        productAttribuite.Status = (int)Const.Status.NORMAL;
                                        await db.ProductAttribuite.AddAsync(productAttribuite);
                                        await db.SaveChangesAsync();
                                        //map attribute
                                        foreach (var itemA in item.listAttribute)
                                        {
                                            if (itemA.Status != (int)Const.Status.DELETED)
                                            {
                                                AttributeMapping attributeMapping = new AttributeMapping();
                                                attributeMapping.AttributeMappingId = Guid.NewGuid();
                                                attributeMapping.ProductAttributeId = productAttribuite.ProductAttributeId;
                                                attributeMapping.AttributeId = itemA.AttributeId;
                                                attributeMapping.AttributeValueId = itemA.AttributeValueId;
                                                attributeMapping.IsMain = itemA.IsMain;
                                                attributeMapping.IsView = itemA.IsView;
                                                attributeMapping.CreatedAt = DateTime.Now;
                                                attributeMapping.UserId = userId;
                                                attributeMapping.Status = (int)Const.Status.NORMAL;
                                                await db.AttributeMapping.AddAsync(attributeMapping);
                                            }
                                        }
                                    }
                                }
                            }

                            await db.SaveChangesAsync();
                            data.listLanguage = new List<LanguageMappingDTO>();
                            if (data.ProductId > 0)
                            {
                                //Thêm permalink
                                PermaLink permaLink = new PermaLink();
                                permaLink.PermaLinkId = Guid.NewGuid();
                                permaLink.Slug = product.Url;
                                permaLink.TargetId = product.ProductId;
                                permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_PRODUCT;
                                permaLink.CreatedAt = DateTime.Now;
                                permaLink.UpdatedAt = DateTime.Now;
                                permaLink.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink);
                                await db.SaveChangesAsync();

                                //Thêm ngôn ngữ (nếu bài viết mới thêm ko phải là ngôn ngữ mạc định)
                                //và có id bài viết gốc
                                if (data.LanguageId != languageId && data.LanguageId != null && data.LanguageId > 0
                                    && data.ProductRootId != null && data.ProductRootId > 0)
                                {
                                    var listLang = db.LanguageMapping.Where(e => e.LanguageId1 == languageId
                                      && e.TargetId1 == data.ProductRootId).ToList();
                                    if (listLang.Count > 0)
                                    {
                                        LanguageMapping languageMapping = new LanguageMapping();
                                        languageMapping.LanguageId1 = languageId;
                                        languageMapping.LanguageId2 = data.LanguageId;
                                        languageMapping.TargetId1 = data.ProductRootId;
                                        languageMapping.TargetId2 = data.ProductId;
                                        languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_PRODUCT;
                                        languageMapping.CreatedAt = DateTime.Now;
                                        languageMapping.Status = (int)Const.Status.NORMAL;
                                        await db.LanguageMapping.AddAsync(languageMapping);

                                        foreach (var item in listLang)
                                        {
                                            LanguageMapping languageMapping2 = new LanguageMapping();
                                            languageMapping2.LanguageId1 = item.LanguageId2;
                                            languageMapping2.LanguageId2 = data.LanguageId;
                                            languageMapping2.TargetId1 = item.TargetId2;
                                            languageMapping2.TargetId2 = data.ProductId;
                                            languageMapping2.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_PRODUCT;
                                            languageMapping2.CreatedAt = DateTime.Now;
                                            languageMapping2.Status = (int)Const.Status.NORMAL;
                                            await db.LanguageMapping.AddAsync(languageMapping2);
                                        }
                                    }
                                    else
                                    {
                                        LanguageMapping languageMapping = new LanguageMapping();
                                        languageMapping.LanguageId1 = languageId;
                                        languageMapping.LanguageId2 = data.LanguageId;
                                        languageMapping.TargetId1 = data.ProductRootId;
                                        languageMapping.TargetId2 = data.ProductId;
                                        languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_PRODUCT;
                                        languageMapping.CreatedAt = DateTime.Now;
                                        languageMapping.Status = (int)Const.Status.NORMAL;
                                        await db.LanguageMapping.AddAsync(languageMapping);
                                    }
                                    await db.SaveChangesAsync();
                                }
                                transaction.Commit();

                                data.listLanguage = db.LanguageMapping.Where(a => a.LanguageId1 == languageId && a.TargetId1 == (int)data.ProductRootId
                                  && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_PRODUCT && a.Status != (int)Const.Status.DELETED).Select(a => new LanguageMappingDTO
                                  {
                                      LanguageMappingId = a.LanguageMappingId,
                                      LanguageId1 = a.LanguageId1,
                                      LanguageId2 = a.LanguageId2,
                                      TargetId1 = a.TargetId1,
                                      TargetId2 = a.TargetId2,
                                      TargetType = a.TargetType,
                                      CreatedAt = a.CreatedAt,
                                      Status = a.Status
                                  }).ToList();
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
                            if (ProductExists(data.ProductId))
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

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
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
                    Product data = await db.Product.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    if ((userId != data.UserId) || (companyId != data.CompanyId))
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
                        try
                        {
                            await db.SaveChangesAsync();
                            //Xóa tag
                            var tag = db.Tag.Where(e => e.TargetId == data.ProductId
                            && e.TargetType == (int)Const.TypeTag.TAG_PRODUCT
                            && e.Status != (int)Const.Status.DELETED).ToList();
                            foreach (var item in tag)
                            {
                                item.UserId = userId;
                                item.Status = (int)Const.Status.DELETED;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            //Xóa ảnh slide
                            var slide = db.Slide.Where(e => e.TargetId == data.ProductId
                            && e.TypeSlideId == (int)Const.TypeSlide.SLIDE_PRODUCT
                            && e.Status != (int)Const.Status.DELETED).ToList();
                            foreach (var item in slide)
                            {
                                item.UserId = userId;
                                item.Status = (int)Const.Status.DELETED;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            //Xóa danh mục
                            var categoryMapping = db.CategoryMapping.Where(e => e.TargetId == data.ProductId
                            && e.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                            && e.Status != (int)Const.Status.DELETED).ToList();
                            foreach (var item in categoryMapping)
                            {
                                item.Status = (int)Const.Status.DELETED;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            //Xóa thuộc tính
                            var productAttribute = db.ProductAttribuite.Where(e => e.ProductId == data.ProductId
                            && e.Status != (int)Const.Status.DELETED).ToList();
                            foreach (var item in productAttribute)
                            {
                                item.Status = (int)Const.Status.DELETED;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            //Xóa ngôn ngữ map cùng
                            var mapLang = await db.LanguageMapping.Where(e => (e.TargetId1 == id || e.TargetId2 == id)
                             && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_PRODUCT && e.Status != (int)Const.Status.DELETED).ToListAsync();
                            if (mapLang.Count > 0)
                            {
                                foreach (var item in mapLang)
                                {
                                    item.Status = (int)Const.Status.DELETED;
                                }
                                db.LanguageMapping.UpdateRange(mapLang);
                                await db.SaveChangesAsync();
                            }

                            //Xóa link
                            var permaLink = db.PermaLink.Where(e => e.Slug == data.Url).FirstOrDefault();
                            if (permaLink != null)
                            {
                                permaLink.UpdatedAt = DateTime.Now;
                                permaLink.Status = (int)Const.Status.DELETED;
                                db.PermaLink.Update(permaLink);
                                await db.SaveChangesAsync();
                            }

                            if (data.ProductId > 0)
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
                            if (!ProductExists(data.ProductId))
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

        // API xóa danh sách sản phẩm
        [HttpPut("deletes")]
        public async Task<IActionResult> DeleteProducts([FromBody] int[] data)
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
                        Product product = await db.Product.FindAsync(data[i]);

                        if (product == null)
                        {
                            continue;
                        }

                        if ((userId != product.UserId) || (companyId != product.CompanyId))
                        {
                            continue;
                        }

                        product.UserId = userId;
                        product.UpdatedAt = DateTime.Now;
                        product.Status = (int)Const.Status.DELETED;
                        db.Entry(product).State = EntityState.Modified;

                        //Xóa tag
                        var tag = db.Tag.Where(e => e.TargetId == product.ProductId
                        && e.TargetType == (int)Const.TypeTag.TAG_PRODUCT
                        && e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in tag)
                        {
                            item.UserId = userId;
                            item.Status = (int)Const.Status.DELETED;
                            db.Entry(item).State = EntityState.Modified;
                        }

                        //Xóa ảnh slide
                        var slide = db.Slide.Where(e => e.TargetId == product.ProductId
                        && e.TypeSlideId == (int)Const.TypeSlide.SLIDE_PRODUCT
                        && e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in slide)
                        {
                            item.UserId = userId;
                            item.Status = (int)Const.Status.DELETED;
                            db.Entry(item).State = EntityState.Modified;
                        }

                        //Xóa danh mục
                        var categoryMapping = db.CategoryMapping.Where(e => e.TargetId == product.ProductId
                        && e.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                        && e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in categoryMapping)
                        {
                            item.Status = (int)Const.Status.DELETED;
                            db.Entry(item).State = EntityState.Modified;
                        }

                        //Xóa thuộc tính
                        var productAttribute = db.ProductAttribuite.Where(e => e.ProductId == product.ProductId
                        && e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in productAttribute)
                        {
                            item.Status = (int)Const.Status.DELETED;
                            db.Entry(item).State = EntityState.Modified;
                        }

                        //Xóa ngôn ngữ map cùng
                        var mapLang = await db.LanguageMapping.Where(e => (e.TargetId1 == product.ProductId || e.TargetId2 == product.ProductId)
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_PRODUCT && e.Status != (int)Const.Status.DELETED).ToListAsync();
                        if (mapLang.Count > 0)
                        {
                            foreach (var item in mapLang)
                            {
                                item.Status = (int)Const.Status.DELETED;
                            }
                            db.LanguageMapping.UpdateRange(mapLang);
                        }

                        //Xóa link
                        var permaLink = db.PermaLink.Where(e => e.Slug == product.Url).FirstOrDefault();
                        if (permaLink != null)
                        {
                            permaLink.UpdatedAt = DateTime.Now;
                            permaLink.Status = (int)Const.Status.DELETED;
                            db.PermaLink.Update(permaLink);
                            await db.SaveChangesAsync();
                        }

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

        [HttpPut("ShowHide/{id}/{stt}")]
        public async Task<ActionResult> ShowHide(int id, int stt)
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
                using (var db = new IOITDataContext())
                {
                    Product data = await db.Product.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //db.Comment.Remove(data);
                        data.Status = (byte)stt;
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.ProductId > 0)
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
                            if (!ProductExists(data.ProductId))
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

        //API get danh sách đánh giá của sản phẩm
        [HttpGet("ProductReview/GetByPage")]
        public IActionResult GetByPageProductReview([FromQuery] FilteredPagination paging)
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
                    //IQueryable<ProductReview> data = db.ProductReview.Where(c => c.Status != (int)Const.Status.DELETED);
                    IQueryable<ProductReviewDT> data = (from p in db.Product
                                                        join pr in db.ProductReview on p.ProductId equals pr.ProductId
                                                        where p.Status != (int)Const.Status.DELETED
                                                        && pr.Status != (int)Const.Status.DELETED
                                                        select new ProductReviewDT
                                                        {
                                                            ProductReviewId = pr.ProductReviewId,
                                                            CustomerId = pr.CustomerId,
                                                            ProductId = pr.ProductId,
                                                            ProductName = p.Name,
                                                            Contents = pr.Contents,
                                                            NumberStar = pr.NumberStar,
                                                            Status = pr.Status,
                                                            Name = pr.Name,
                                                            Email = pr.Email
                                                        }).AsQueryable();

                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    MetaDataDT metaDataDT = new MetaDataDT();
                    metaDataDT.Sum = data.Count();
                    metaDataDT.NotApproved = data.Where(e => e.Status == 2).Count();
                    metaDataDT.Approving = data.Where(e => e.Status == 3).Count();

                    def.metadata = metaDataDT;

                    if (paging.page_size > 0)
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                        else
                        {
                            data = data.OrderBy("ProductReviewId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("ProductReviewId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select).ToDynamicList();
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

        //API đổi trạng thái đánh giá sản phẩm khởi tạo - duyệt - không duyệt
        //[HttpPut("ChangeStatusProductReview/{ProductReviewId}/{Stt}")]
        //public async Task<ActionResult> ChangeStatusProductReview(int ProductReviewId, int Stt)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    //check role
        //    var identity = (ClaimsIdentity)User.Identity;
        //    string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
        //    if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
        //    {
        //        def.meta = new Meta(222, "No permission");
        //        return Ok(def);
        //    }
        //    try
        //    {
        //        using (var db = new IOITDataContext())
        //        {
        //            ProductReview data = await db.ProductReview.FindAsync(ProductReviewId);
        //            if (data == null)
        //            {
        //                def.meta = new Meta(404, "Not Found");
        //                return Ok(def);
        //            }

        //            using (var transaction = db.Database.BeginTransaction())
        //            {
        //                var product = await db.Product.Where(e => e.ProductId == data.ProductId).FirstOrDefaultAsync();
        //                if (product != null)
        //                {
        //                    //db.Comment.Remove(data);
        //                    data.Status = (byte)Stt;
        //                    db.Entry(data).State = EntityState.Modified;
        //                    try
        //                    {
        //                        await db.SaveChangesAsync();
        //                        //Tính toán lại số điểm star
        //                        var dataReviews = db.ProductReview.Where(pr => pr.ProductId == data.ProductId && pr.Status == (int)Const.Status.OK).ToList();
        //                        if (dataReviews.Count() > 0)
        //                        {
        //                            var PointStar = Math.Round((decimal)(dataReviews.Sum(e => e.NumberStar) / dataReviews.Count()), 0, MidpointRounding.AwayFromZero);
        //                            product.PointStar = (int)PointStar;
        //                        }
        //                        else
        //                        {
        //                            product.PointStar = 0;
        //                        }

        //                        db.Product.Update(product);
        //                        await db.SaveChangesAsync();

        //                        if (data.ProductReviewId > 0)
        //                            transaction.Commit();
        //                        else
        //                            transaction.Rollback();

        //                        def.meta = new Meta(200, "Success");
        //                        def.data = data;
        //                        return Ok(def);
        //                    }
        //                    catch (DbUpdateException e)
        //                    {
        //                        transaction.Rollback();
        //                        log.Error("DbUpdateException:" + e);
        //                        if (!ProductReviewExists(data.ProductReviewId))
        //                        {
        //                            def.meta = new Meta(404, "Not Found");
        //                            return Ok(def);
        //                        }
        //                        else
        //                        {
        //                            def.meta = new Meta(500, "Internal Server Error");
        //                            return Ok(def);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    def.meta = new Meta(404, "Not Found");
        //                    return Ok(def);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error("Error:" + e);
        //        def.meta = new Meta(500, "Internal Server Error");
        //        return Ok(def);
        //    }
        //}

        //API xóa đánh giá sản phẩm
        [HttpDelete("deleteProductReview/{id}")]
        public async Task<IActionResult> DeleteProductReview(int id)
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
                    ProductReview data = await db.ProductReview.FindAsync(id);
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

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.ProductReviewId > 0)
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
                            if (!ProductReviewExists(data.ProductReviewId))
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

        //API xóa danh sách đánh gián sản phẩm
        [HttpPut("deleteProductReviews")]
        public async Task<IActionResult> DeleteProductReviews([FromBody] int[] data)
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
                        ProductReview productReview = await db.ProductReview.FindAsync(data[i]);

                        if (productReview == null)
                        {
                            continue;
                        }

                        productReview.UserId = userId;
                        productReview.UpdatedAt = DateTime.Now;
                        productReview.Status = (int)Const.Status.DELETED;
                        db.Entry(productReview).State = EntityState.Modified;
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

        private bool ProductExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Product.Count(e => e.ProductId == id) > 0;
            }
        }

        private bool ProductReviewExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.ProductReview.Count(e => e.ProductReviewId == id) > 0;
            }
        }
    }
}


