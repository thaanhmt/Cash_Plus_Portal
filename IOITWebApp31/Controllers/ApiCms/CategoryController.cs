using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
    public class CategoryController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("category", "category");
        private static string functionCode = "QLDM";

        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
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
                        def.meta = new Meta(200, "Success");
                        IQueryable<Category> data = db.Category.Where(c => c.Status != (int)Const.Status.DELETED);

                        //foreach (var item in data)
                        //{
                        //    var pLink = db.PermaLink.Where(e => e.Slug == item.Url.Trim().ToLower() && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        //    if (pLink == null)
                        //    {
                        //        //Thêm permalink
                        //        PermaLink permaLink = new PermaLink();
                        //        permaLink.PermaLinkId = Guid.NewGuid();
                        //        permaLink.Slug = item.Url.Trim().ToLower();
                        //        permaLink.TargetId = item.CategoryId;
                        //        permaLink.TargetType = (byte)item.TypeCategoryId;
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
                                data = data.OrderBy("CategoryId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                                data = data.OrderBy("CategoryId desc");
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
                                e.CategoryId,
                                e.Name,
                                e.Code,
                                e.CategoryParentId,
                                e.Description,
                                e.Contents,
                                e.Url,
                                e.Image,
                                e.Icon,
                                e.IconFa,
                                e.IconText,
                                e.Location,
                                e.TypeCategoryId,
                                e.LanguageId,
                                e.CreatedAt,
                                e.UpdatedAt,
                                e.UserId,
                                e.MetaTitle,
                                e.MetaKeyword,
                                e.MetaDescription,
                                e.Status,
                                e.IsComment,
                                e.TemplatePage,
                                e.NumberDisplayMobile,
                                categoryParent = db.Category.Where(c => c.CategoryId == e.CategoryParentId).Select(c => new
                                {
                                    c.CategoryId,
                                    c.Name
                                }).FirstOrDefault(),
                                listImage = db.Attactment.Where(c => c.TargetId == e.CategoryId && c.TargetType == (int)Const.TypeAttachment.CATEGORY_IMAGE).Select(pi => new
                                {
                                    pi.AttactmentId,
                                    pi.Name,
                                    pi.Url,
                                    pi.Status,
                                    pi.TargetId,
                                    pi.TargetType,
                                    pi.IsImageMain,
                                    pi.Thumb,
                                    pi.CreatedId,
                                }).ToList(),
                                language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new
                                {
                                    l.LanguageId,
                                    l.Flag,
                                    l.Name,
                                    l.Code
                                }).FirstOrDefault(),
                                listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.CategoryId || a.TargetId2 == e.CategoryId)
                                && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY && a.Status != (int)Const.Status.DELETED).Select(a => new
                                {
                                    lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new
                                    {
                                        l.LanguageId,
                                        l.Name,
                                        l.Flag
                                    }).FirstOrDefault(),
                                    category = db.Category.Where(l => (l.CategoryId == a.TargetId1 || l.CategoryId == a.TargetId2) && l.CategoryId != e.CategoryId).Select(l => new
                                    {
                                        l.CategoryId,
                                        l.Name,
                                        l.Url
                                    }).FirstOrDefault(),
                                }).ToList()
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
            catch (Exception ex)
            {
                log.Error("Exception:" + ex);
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // get all category new
        [HttpGet("GetAllCatNew")]
        public async Task<IActionResult> GetAllCatNew([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
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
                        def.meta = new Meta(200, "Success");
                        IQueryable<Category> data = db.Category.Where(c => c.Status != (int)Const.Status.DELETED && c.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_TEXT);
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
                                data = data.OrderBy(paging.order_by);
                            }
                            else
                            {
                                data = data.OrderBy("CategoryId desc");
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
                                data = data.OrderBy("CategoryId desc");
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
                                e.CategoryId,
                                e.Name,
                                e.Code,
                                e.CategoryParentId,
                                e.Description,
                                e.Contents,
                                e.Url,
                                e.Image,
                                e.Icon,
                                e.IconFa,
                                e.IconText,
                                e.Location,
                                e.TypeCategoryId,
                                e.LanguageId,
                                e.CreatedAt,
                                e.UpdatedAt,
                                e.UserId,
                                e.MetaTitle,
                                e.MetaKeyword,
                                e.MetaDescription,
                                e.Status,
                                e.IsComment,
                                e.TemplatePage,
                                e.NumberDisplayMobile,
                                categoryParent = db.Category.Where(c => c.CategoryId == e.CategoryParentId).Select(c => new
                                {
                                    c.CategoryId,
                                    c.Name
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
            catch (Exception ex)
            {
                log.Error("Exception:" + ex);
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // get all category GetAllProduct
        [HttpGet("GetAllCatProduct")]
        public async Task<IActionResult> GetAllCatProduct([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
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
                        def.meta = new Meta(200, "Success");
                        IQueryable<Category> data = db.Category.Where(c => c.Status != (int)Const.Status.DELETED && c.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PRODUCT);
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
                                data = data.OrderBy(paging.order_by);
                            }
                            else
                            {
                                data = data.OrderBy("CategoryId desc");
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
                                data = data.OrderBy("CategoryId desc");
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
                                e.CategoryId,
                                e.Name,
                                e.Code,
                                e.CategoryParentId,
                                e.Description,
                                e.Contents,
                                e.Url,
                                e.Image,
                                e.Icon,
                                e.IconFa,
                                e.IconText,
                                e.Location,
                                e.TypeCategoryId,
                                e.LanguageId,
                                e.CreatedAt,
                                e.UpdatedAt,
                                e.UserId,
                                e.MetaTitle,
                                e.MetaKeyword,
                                e.MetaDescription,
                                e.Status,
                                e.IsComment,
                                e.TemplatePage,
                                e.NumberDisplayMobile,
                                categoryParent = db.Category.Where(c => c.CategoryId == e.CategoryParentId).Select(c => new
                                {
                                    c.CategoryId,
                                    c.Name
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
            catch (Exception ex)
            {
                log.Error("Exception:" + ex);
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }
        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCategory(int id)
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
            try
            {
                using (var db = new IOITDataContext())
                {
                    var data = await db.Category.Where(e => e.CategoryId == id && e.Status != (int)Const.Status.DELETED).Select(e => new
                    {
                        e.CategoryId,
                        e.Name,
                        e.Code,
                        e.CategoryParentId,
                        e.Description,
                        e.Contents,
                        e.Url,
                        e.Image,
                        e.Icon,
                        e.IconFa,
                        e.IconText,
                        e.Location,
                        e.TypeCategoryId,
                        e.LanguageId,
                        e.CreatedAt,
                        e.UpdatedAt,
                        e.UserId,
                        e.MetaTitle,
                        e.MetaKeyword,
                        e.MetaDescription,
                        e.Status,
                        e.IsComment,
                        e.TemplatePage,
                        e.NumberDisplayMobile,
                        categoryParent = db.Category.Where(c => c.CategoryId == e.CategoryParentId).Select(c => new
                        {
                            c.CategoryId,
                            c.Name
                        }).FirstOrDefault(),
                        listImage = db.Attactment.Where(c => c.TargetId == e.CategoryId && c.TargetType == (int)Const.TypeAttachment.CATEGORY_IMAGE).Select(pi => new
                        {
                            pi.Name,
                            pi.Url,
                            pi.Status,
                            pi.TargetId,
                            pi.TargetType,
                            pi.IsImageMain,
                            pi.Thumb,
                            pi.CreatedId,
                        }).ToList(),
                        language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new
                        {
                            l.LanguageId,
                            l.Flag,
                            l.Name,
                            l.Code
                        }).FirstOrDefault(),
                        //listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.CategoryId || a.TargetId2 == e.CategoryId)
                        //    && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY && a.Status != (int)Const.Status.DELETED).Select(a => new
                        //    {
                        //        lang = db.Language.Where(la => (la.LanguageId == a.LanguageId1 || la.LanguageId == a.LanguageId2) && la.LanguageId != e.LanguageId).Select(la => new
                        //        {
                        //            la.LanguageId,
                        //            la.Name,
                        //            la.Flag
                        //        }).FirstOrDefault(),
                        //        category = db.Category.Where(cc => (cc.CategoryId == a.TargetId1 || cc.CategoryId == a.TargetId2) && cc.CategoryId != e.CategoryId).Select(cc => new
                        //        {
                        //            cc.CategoryId,
                        //            cc.Name,
                        //            cc.Url
                        //        }).FirstOrDefault(),
                        //    }).ToList(),

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

        [HttpGet("listNews/{idc}")]
        public async Task<ActionResult> ListNews([FromRoute] int idc)
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
                    var data = await (from cm in db.CategoryMapping
                                      join n in db.News on cm.TargetId equals n.NewsId
                                      where cm.CategoryId == idc
                                         && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                         && cm.Status != (int)Const.Status.DELETED
                                      select new
                                      {
                                          cm.CategoryMappingId,
                                          cm.CategoryId,
                                          cm.TargetId,
                                          cm.TargetType,
                                          cm.Location,
                                          cm.CreatedAt,
                                          cm.Status,
                                          n.Title
                                      }).OrderByDescending(e => e.Location).ToListAsync();

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    def.meta = new Meta(200, "Success");
                    def.data = data.ToList();
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

        [HttpGet("listProduct/{idc}")]
        public async Task<ActionResult> ListProduct([FromRoute] int idc)
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
                    var data = await (from cm in db.CategoryMapping
                                      join p in db.Product on cm.TargetId equals p.ProductId
                                      where cm.CategoryId == idc
                                         && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                                         && cm.Status != (int)Const.Status.DELETED
                                      select new
                                      {
                                          cm.CategoryMappingId,
                                          cm.CategoryId,
                                          cm.TargetId,
                                          cm.TargetType,
                                          cm.Location,
                                          cm.CreatedAt,
                                          cm.Status,
                                          p.Name
                                      }).OrderByDescending(e => e.Location).ToListAsync();

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    def.meta = new Meta(200, "Success");
                    def.data = data.ToList();
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

        [HttpPut("sortCategoryMapping/{idc}")]
        public async Task<ActionResult> SortCategoryMapping([FromRoute] int idc, [FromBody] List<CategoryMapping> data)
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
                if (!ModelState.IsValid || data.Count <= 0 || data == null)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        var cate = await db.Category.FindAsync(idc);
                        if (cate == null)
                        {
                            def.meta = new Meta(404, "Not found!");
                            return Ok(def);
                        }

                        try
                        {
                            db.UpdateRange(data);
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
                            if (!CategoryExists(cate.CategoryId))
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

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutCategory(int id, [FromBody] CategoryDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
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
                if (data.Code == null || data.Code == "")
                {
                    def.meta = new Meta(211, "Code Null!");
                    return Ok(def);
                }
                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    var checkExist = db.Category.Where(c => c.Code.Trim() == data.Code.Trim()
                    && c.TypeCategoryId == data.TypeCategoryId
                    && c.CategoryId != id && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkExist != null)
                    {
                        def.meta = new Meta(213, "Mã trùng!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        var cate = db.Category.Find(id);
                        if (cate == null)
                        {
                            def.meta = new Meta(404, "Not found!");
                            return Ok(def);
                        }

                        //
                        string url = data.Url == null ? Utils.NonUnicode(data.Name) : data.Url;
                        url = url.Trim().ToLower();

                        if (data.TypeCategoryId != (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA
                            && data.TypeCategoryId != (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE)
                        {
                            if (cate.Url != url)
                            {
                                //check xem trùng link ko
                                var pLink = db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.TargetId != data.CategoryId && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (pLink != null)
                                {
                                    def.meta = new Meta(232, "Link đã tồn tại!");
                                    def.data = pLink;
                                    return Ok(def);
                                }
                                //cập nhật thay link cũ
                                var permaLink = db.PermaLink.Where(e => e.Slug == cate.Url && e.TargetId == cate.CategoryId).FirstOrDefault();
                                if (permaLink != null)
                                {
                                    permaLink.Slug = url;
                                    permaLink.TargetId = cate.CategoryId;
                                    permaLink.TargetType = (byte)cate.TypeCategoryId;
                                    permaLink.UpdatedAt = DateTime.Now;
                                    permaLink.Status = (int)Const.Status.NORMAL;
                                    db.PermaLink.Update(permaLink);
                                    await db.SaveChangesAsync();
                                }
                                else
                                {
                                    PermaLink permaLink1 = new PermaLink();
                                    permaLink1.PermaLinkId = Guid.NewGuid();
                                    permaLink1.Slug = url;
                                    permaLink1.TargetId = cate.CategoryId;
                                    permaLink1.TargetType = (byte)cate.TypeCategoryId;
                                    permaLink1.CreatedAt = DateTime.Now;
                                    permaLink1.UpdatedAt = DateTime.Now;
                                    permaLink1.Status = (int)Const.Status.NORMAL;
                                    await db.PermaLink.AddAsync(permaLink1);
                                    await db.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                //cập nhật thay link cũ
                                var permaLink = db.PermaLink.Where(e => e.Slug == cate.Url && e.TargetId == cate.CategoryId
                                && e.TargetType == (byte)cate.TypeCategoryId).FirstOrDefault();
                                if (permaLink == null)
                                {
                                    PermaLink permaLink1 = new PermaLink();
                                    permaLink1.PermaLinkId = Guid.NewGuid();
                                    permaLink1.Slug = url;
                                    permaLink1.TargetId = cate.CategoryId;
                                    permaLink1.TargetType = (byte)cate.TypeCategoryId;
                                    permaLink1.CreatedAt = DateTime.Now;
                                    permaLink1.UpdatedAt = DateTime.Now;
                                    permaLink1.Status = (int)Const.Status.NORMAL;
                                    await db.PermaLink.AddAsync(permaLink1);
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                        cate.Name = data.Name;
                        cate.Code = data.Code;
                        cate.CategoryParentId = data.CategoryParentId != null ? (int)data.CategoryParentId : 0;
                        cate.Description = data.Description;
                        cate.Contents = data.Contents;
                        cate.Url = data.Url;
                        cate.Image = data.Image;
                        cate.Icon = data.Icon;
                        cate.IconFa = data.IconFa;
                        cate.IconText = data.IconText;
                        cate.TypeCategoryId = data.TypeCategoryId;
                        cate.LanguageId = data.LanguageId;
                        cate.MetaTitle = data.MetaTitle;
                        cate.MetaKeyword = data.MetaKeyword;
                        cate.MetaDescription = data.MetaDescription;
                        cate.Location = data.Location;
                        cate.UserId = data.UserId;
                        cate.UpdatedAt = DateTime.Now;
                        cate.NumberDisplayMobile = data.NumberDisplayMobile;
                        cate.TemplatePage = data.TemplatePage;
                        cate.IsComment = data.IsComment;
                        cate.Status = data.Status;
                        db.Entry(cate).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.listImage != null)
                            {
                                foreach (var item in data.listImage)
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
                                        pa.IsImageMain = item.IsImageMain;
                                        db.Attactment.Update(pa);
                                    }
                                    else
                                    {
                                        if (item.Status != (int)Const.Status.DELETED)
                                        {
                                            Attactment atm = new Attactment();
                                            atm.Name = data.Name;
                                            atm.TargetType = (int)Const.TypeAttachment.CATEGORY_IMAGE;
                                            atm.TargetId = cate.CategoryId;
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
                            transaction.Commit();
                            Models.EF.Action action = new Models.EF.Action();
                            if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_TEXT)
                            {
                                action.ActionName = "Sửa danh mục bài viết “" + data.Name + "”";
                            }
                            else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_LEGAL_DOC)
                            {
                                action.ActionName = "Sửa danh mục văn bản “" + data.Name + "”";
                            }
                            else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PAGE_NORMAL)
                            {
                                action.ActionName = "Sửa trang “" + data.Name + "”";
                            }
                            else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA)
                            {
                                action.ActionName = "Sửa lĩnh vực nghiên cứu “" + data.Name + "”";
                            }
                            else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE)
                            {
                                action.ActionName = "Sửa phạm vi ứng dụng “" + data.Name + "”";
                            }
                            action.ActionType = (int)Const.ActionType.UPDATE;
                            action.TargetId = data.CategoryId.ToString();
                            action.TargetName = data.Name;
                            action.CompanyId = companyId;
                            action.Logs = JsonConvert.SerializeObject(data);
                            action.Time = 0;
                            action.Ipaddress = IpAddress();
                            action.Type = (int)Const.TypeAction.ACTION;
                            action.CreatedAt = DateTime.Now;
                            action.UserPushId = userId;
                            action.UserId = userId;
                            action.Status = (int)Const.Status.NORMAL;
                            await db.Action.AddAsync(action);
                            await db.SaveChangesAsync();
                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!CategoryExists(cate.CategoryId))
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

        //PUT Status
        [HttpPut("ShowHide/{id}/{stt}")]
        public async Task<ActionResult> ShowHide(int id, int stt)
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
                using (var db = new IOITDataContext())
                {
                    Category data = await db.Category.FindAsync(id);
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

                            if (data.CategoryId > 0)
                            {
                                transaction.Commit();
                                //Create log
                                Models.EF.Action action = new Models.EF.Action();
                                if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_TEXT)
                                {
                                    action.ActionName = "Sửa danh mục bài viết “" + data.Name + "”";
                                }
                                else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_LEGAL_DOC)
                                {
                                    action.ActionName = "Sửa danh mục văn bản “" + data.Name + "”";
                                }
                                else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PAGE_NORMAL)
                                {
                                    action.ActionName = "Sửa trang “" + data.Name + "”";
                                }
                                else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA)
                                {
                                    action.ActionName = "Sửa lĩnh vực nghiên cứu “" + data.Name + "”";
                                }
                                else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE)
                                {
                                    action.ActionName = "Sửa phạm vi ứng dụng “" + data.Name + "”";
                                }
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.CategoryId.ToString();
                                action.TargetName = data.Name;
                                action.CompanyId = companyId;
                                action.Logs = JsonConvert.SerializeObject(data);
                                action.Time = 0;
                                action.Ipaddress = IpAddress();
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserPushId = userId;
                                action.UserId = userId;
                                action.Status = (int)Const.Status.NORMAL;
                                await db.Action.AddAsync(action);
                                await db.SaveChangesAsync();
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
                            if (!CategoryExists(data.CategoryId))
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

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> PostCategory([FromBody] CategoryDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            //int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            //int websiteId = int.Parse(identity.Claims.Where(c => c.Type == "WebsiteId").Select(c => c.Value).SingleOrDefault());
            //int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
            int companyId = 1;
            int websiteId = 1;
            int languageId = 1;
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

                if (companyId != data.CompanyId || userId != data.UserId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                if (data.Code == null || data.Code == "")
                {
                    def.meta = new Meta(211, "Code Null!");
                    return Ok(def);
                }
                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }
                if (data.TypeCategoryId == null || data.TypeCategoryId < 0)
                {
                    def.meta = new Meta(211, "TypeCategoryId Null!");
                    return Ok(def);
                }


                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        if (data.TypeCategoryId != (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA
                            && data.TypeCategoryId != (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE)
                        {
                            //check xem trùng link ko
                            int k = 1;
                            string url = data.Url == null ? Utils.NonUnicode(data.Name) : data.Url;
                            url = url.Trim().ToLower();
                            var pLink = db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            while (pLink != null)
                            {
                                url = url + "-" + k;
                                pLink = db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                k++;
                            }
                            data.Url = url;
                        }
                        var checkExist = db.Category.Where(c => c.Code.Trim() == data.Code.Trim()
                            && c.TypeCategoryId == data.TypeCategoryId && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (checkExist != null)
                        {
                            def.meta = new Meta(213, "Mã đã tồn tại!");
                            return Ok(def);
                        }

                        Category cate = new Category();
                        cate.Name = data.Name;
                        cate.Code = data.Code;
                        cate.CategoryParentId = data.CategoryParentId != null ? (int)data.CategoryParentId : 0;
                        cate.Description = data.Description;
                        cate.Contents = data.Contents;
                        cate.Url = data.Url == null ? Utils.NonUnicode(data.Name) : data.Url;
                        cate.Image = data.Image;
                        cate.Icon = data.Icon;
                        cate.IconFa = data.IconFa;
                        cate.IconText = data.IconText;
                        cate.Location = data.Location == null ? db.Category.ToList().Count : data.Location;
                        cate.TypeCategoryId = data.TypeCategoryId;
                        cate.LanguageId = data.LanguageId == null ? languageId : data.LanguageId;
                        cate.WebsiteId = data.WebsiteId == null ? websiteId : data.WebsiteId;
                        cate.CompanyId = data.CompanyId == null ? companyId : data.CompanyId;
                        cate.MetaTitle = data.MetaTitle;
                        cate.MetaKeyword = data.MetaKeyword;
                        cate.MetaDescription = data.MetaDescription;
                        cate.CreatedAt = DateTime.Now;
                        cate.UpdatedAt = DateTime.Now;
                        cate.UserId = data.UserId;
                        cate.NumberDisplayMobile = data.NumberDisplayMobile;
                        cate.Status = data.Status != null ? data.Status : (int)Const.Status.NORMAL;
                        cate.TemplatePage = data.TemplatePage;
                        cate.IsComment = data.IsComment;
                        await db.Category.AddAsync(cate);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.CategoryId = cate.CategoryId;

                            if (data.CategoryId > 0)
                            {
                                //Thêm permalink
                                PermaLink permaLink = new PermaLink();
                                permaLink.PermaLinkId = Guid.NewGuid();
                                permaLink.Slug = cate.Url;
                                permaLink.TargetId = cate.CategoryId;
                                permaLink.TargetType = (byte)cate.TypeCategoryId;
                                permaLink.CreatedAt = DateTime.Now;
                                permaLink.UpdatedAt = DateTime.Now;
                                permaLink.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink);
                                await db.SaveChangesAsync();

                                if (data.listImage != null)
                                {
                                    foreach (var item in data.listImage)
                                    {
                                        Attactment DocumentProduct = new Attactment();
                                        DocumentProduct.Name = data.Name;
                                        DocumentProduct.Url = data.Name;
                                        DocumentProduct.TargetId = data.CategoryId;
                                        DocumentProduct.TargetType = (int)Const.TypeAttachment.CATEGORY_IMAGE;
                                        DocumentProduct.CreatedId = data.UserId;
                                        DocumentProduct.CreatedAt = DateTime.Now;
                                        DocumentProduct.Status = (int)Const.Status.NORMAL;

                                        await db.Attactment.AddAsync(DocumentProduct);
                                    }
                                }

                                //Thêm ngôn ngữ (nếu bài viết mới thêm ko phải là ngôn ngữ mạc định)
                                //và có id danh mục gốc
                                if (data.LanguageId != languageId && data.LanguageId != null && data.LanguageId > 0
                                    && data.CategoryRootId != null && data.CategoryRootId > 0)
                                {
                                    var listLang = db.LanguageMapping.Where(e => e.LanguageId1 == languageId
                                      && e.TargetId1 == data.CategoryRootId).ToList();
                                    if (listLang.Count > 0)
                                    {
                                        LanguageMapping languageMapping = new LanguageMapping();
                                        languageMapping.LanguageId1 = languageId;
                                        languageMapping.LanguageId2 = data.LanguageId;
                                        languageMapping.TargetId1 = data.CategoryRootId;
                                        languageMapping.TargetId2 = data.CategoryId;
                                        languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY;
                                        languageMapping.CreatedAt = DateTime.Now;
                                        languageMapping.Status = (int)Const.Status.NORMAL;
                                        await db.LanguageMapping.AddAsync(languageMapping);

                                        foreach (var item in listLang)
                                        {
                                            LanguageMapping languageMapping2 = new LanguageMapping();
                                            languageMapping2.LanguageId1 = item.LanguageId2;
                                            languageMapping2.LanguageId2 = data.LanguageId;
                                            languageMapping2.TargetId1 = item.TargetId2;
                                            languageMapping2.TargetId2 = data.CategoryId;
                                            languageMapping2.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY;
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
                                        languageMapping.TargetId1 = data.CategoryRootId;
                                        languageMapping.TargetId2 = data.CategoryId;
                                        languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY;
                                        languageMapping.CreatedAt = DateTime.Now;
                                        languageMapping.Status = (int)Const.Status.NORMAL;
                                        await db.LanguageMapping.AddAsync(languageMapping);
                                    }
                                    await db.SaveChangesAsync();
                                    //Kiểm tra xem bài viết này có ngôn ngữ nào khác ko
                                }

                                transaction.Commit();
                                //Create log
                                Models.EF.Action action = new Models.EF.Action();
                                if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_TEXT)
                                {
                                    action.ActionName = "Thêm danh mục bài viết “" + data.Name + "”";
                                }
                                else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_LEGAL_DOC)
                                {
                                    action.ActionName = "Thêm danh mục văn bản “" + data.Name + "”";
                                }
                                else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PAGE_NORMAL)
                                {
                                    action.ActionName = "Thêm trang “" + data.Name + "”";
                                }
                                else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA)
                                {
                                    action.ActionName = "Thêm lĩnh vực nghiên cứu “" + data.Name + "”";
                                }
                                else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE)
                                {
                                    action.ActionName = "Thêm phạm vi ứng dụng “" + data.Name + "”";
                                }
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.CategoryId.ToString();
                                action.TargetName = data.Name;
                                action.CompanyId = companyId;
                                action.Logs = JsonConvert.SerializeObject(data);
                                action.Time = 0;
                                action.Ipaddress = IpAddress();
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserPushId = userId;
                                action.UserId = userId;
                                action.Status = (int)Const.Status.NORMAL;
                                await db.Action.AddAsync(action);
                                await db.SaveChangesAsync();
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
                            if (CategoryExists((int)data.CategoryId))
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
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
                bool check = await DeleteCategoryFunc(id);
                if (check == false)
                {
                    def.meta = new Meta(404, "Not Found");
                    def.data = check;
                    return Ok(def);
                }
                else
                {
                    def.meta = new Meta(200, "Success");
                    def.data = check;
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

        private async Task<bool> DeleteCategoryFunc(int CategoryId)
        {
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            using (var db = new IOITDataContext())
            {
                Category data = db.Category.Find(CategoryId);
                if (data == null)
                {
                    return false;
                }

                try
                {
                    //Xóa danh mục hiện tại
                    data.UpdatedAt = DateTime.Now;
                    data.Status = (int)Const.Status.DELETED;
                    db.Category.Update(data);
                    //Xóa các mapping của danh mục
                    var listMapping = db.CategoryMapping.Where(cm => cm.CategoryId == CategoryId && cm.Status != (int)Const.Status.DELETED).ToList();
                    listMapping.ForEach(cm => cm.Status = (int)Const.Status.DELETED);
                    db.CategoryMapping.UpdateRange(listMapping);
                    //chuyển cách danh mục con về ko có danh mục cha
                    var listChild = db.Category.Where(c => c.CategoryParentId == CategoryId && c.Status != (int)Const.Status.DELETED).ToList();
                    if (listChild.Count() > 0)
                    {
                        foreach (var item in listChild)
                        {
                            item.CategoryParentId = 0;
                            item.UpdatedAt = DateTime.Now;
                        }
                        db.Category.UpdateRange(listChild);
                    }
                    //Xóa ngôn ngữ map cùng
                    var mapLang = await db.LanguageMapping.Where(e => (e.TargetId1 == CategoryId || e.TargetId2 == CategoryId)
                     && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY && e.Status != (int)Const.Status.DELETED).ToListAsync();
                    if (mapLang.Count > 0)
                    {
                        foreach (var item in mapLang)
                        {
                            item.Status = (int)Const.Status.DELETED;
                        }
                        db.LanguageMapping.UpdateRange(mapLang);
                    }

                    //Xóa link
                    var permaLink = db.PermaLink.Where(e => e.Slug == data.Url && e.TargetId == data.CategoryId
                    && e.TargetType == data.TypeCategoryId && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (permaLink != null)
                    {
                        permaLink.UpdatedAt = DateTime.Now;
                        permaLink.Status = (int)Const.Status.DELETED;
                        db.PermaLink.Update(permaLink);
                        await db.SaveChangesAsync();
                    }

                    await db.SaveChangesAsync();
                    //Create log
                    Models.EF.Action action = new Models.EF.Action();
                    if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_TEXT)
                    {
                        action.ActionName = "Sửa danh mục bài viết “" + data.Name + "”";
                    }
                    else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_LEGAL_DOC)
                    {
                        action.ActionName = "Xóa danh mục văn bản “" + data.Name + "”";
                    }
                    else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PAGE_NORMAL)
                    {
                        action.ActionName = "Xóa trang “" + data.Name + "”";
                    }
                    else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA)
                    {
                        action.ActionName = "Xóa lĩnh vực nghiên cứu “" + data.Name + "”";
                    }
                    else if (data.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE)
                    {
                        action.ActionName = "Xóa phạm vi ứng dụng “" + data.Name + "”";
                    }
                    action.ActionType = (int)Const.ActionType.DELETE;
                    action.TargetId = data.CategoryId.ToString();
                    action.TargetName = data.Name;
                    action.CompanyId = companyId;
                    action.Logs = JsonConvert.SerializeObject(data);
                    action.Time = 0;
                    action.Ipaddress = IpAddress();
                    action.Type = (int)Const.TypeAction.ACTION;
                    action.CreatedAt = DateTime.Now;
                    action.UserPushId = userId;
                    action.UserId = userId;
                    action.Status = (int)Const.Status.NORMAL;
                    await db.Action.AddAsync(action);
                    await db.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }

            }
        }

        // GET by Tree
        [HttpGet("GetByTree")]
        public IActionResult GetByTree([FromQuery] int[] arr, [FromQuery] int langId = -1)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            try
            {
                List<SmallCategoryDTO> list = new List<SmallCategoryDTO>();
                var query = "";
                foreach (var type in arr)
                {
                    query += "TypeCategoryId=" + type + " OR ";
                }

                var data = GetByTreeFunction(list, 0, 1, query, "", langId);
                def.data = data;
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
            catch (Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private List<SmallCategoryDTO> GetByTreeFunction(List<SmallCategoryDTO> list, int CategoryParentId, int level, string query, string genealogy, int languageId)
        {
            var index = level + 1;
            var q = "";
            if (query != "" && CategoryParentId == 0)
            {
                int lastIndexOf = query.LastIndexOf(" OR ");
                q = query.Substring(0, lastIndexOf);
                if (languageId > 0)
                    q = "CategoryParentId=" + CategoryParentId + " AND LanguageId = " + languageId + " AND Status!=99 AND (" + q + ")";
                else
                    q = "CategoryParentId=" + CategoryParentId + " AND Status!=99 AND (" + q + ")";
            }
            else
            {
                if (languageId > 0)
                    q = "CategoryParentId=" + CategoryParentId + " AND LanguageId = " + languageId + " AND Status!=99";
                else
                    q = "CategoryParentId=" + CategoryParentId + " AND Status!=99";
            }

            using (var db = new IOITDataContext())
            {
                var data = db.Category.Where(q).Select(e => new SmallCategoryDTO
                {
                    CategoryId = e.CategoryId,
                    Code = e.Code,
                    Name = e.Name,
                    CategoryParentId = e.CategoryParentId,
                    Status = e.Status,
                    Level = level,
                    Location = e.Location,
                    Check = false
                }).OrderBy(e => e.Location).ToList();

                foreach (SmallCategoryDTO dt in data)
                {
                    String strg = genealogy;
                    strg += dt.CategoryParentId.ToString() + "_";
                    dt.Genealogy = strg;
                    list.Add(dt);
                    GetByTreeFunction(list, dt.CategoryId, index, query, strg, languageId);
                }
            }

            return list;
        }

        private bool CategoryExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Category.Count(e => e.CategoryId == id) > 0;
            }
        }

        #region Sắp xếp danh mục menu bằng cách kéo thả

        [HttpGet("GetCategorySort")]
        public async Task<IActionResult> GetCategorySort([FromQuery] int[] arr, string txtSearch, [FromQuery] int langId = -1)
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

            try
            {
                var query = "";
                int cs = 1;
                int idx = 0;
                foreach (var type in arr)
                {
                    query += "TypeCategoryId=" + type + " OR ";
                }
                //int lastIndexOf = query.LastIndexOf(" OR ");
                //query = query.Substring(0, lastIndexOf);
                //query += ")";
                //if (langId > 0)
                //{
                //    if (query != "")
                //    {
                //        query += " AND LanguageId=" + langId;
                //    }
                //    else
                //    {
                //        query = "LanguageId=" + langId;
                //    }
                //}

                if (txtSearch != null && txtSearch != "")
                {
                    int lastIndexOf = query.LastIndexOf(" OR ");
                    query = query.Substring(0, lastIndexOf);
                    query = "(Code.Contains(\"" + txtSearch + "\") OR Name.Contains(\"" + txtSearch + "\")) AND (" + query + ")";
                    cs = 2;
                }

                var data = await GetCategorySortFunction(0, query, 0, "—", cs, idx, langId);
                def.data = data.categorySorts;
                def.metadata = data.Sum;
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
            catch (Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private async Task<FullCategorySort> GetCategorySortFunction(int CategoryParentId, string query, int Level, string CategoryParentName, int cs, int index, int languageId)
        {
            var q = "";
            if (cs == 1)
            {
                if (query != "" && CategoryParentId == 0)
                {
                    int lastIndexOf = query.LastIndexOf(" OR ");
                    q = query.Substring(0, lastIndexOf);
                    if (languageId > 0)
                        q = "CategoryParentId=" + CategoryParentId + " AND LanguageId=" + languageId + " AND Status!=99 AND (" + q + ")";
                    else
                        q = "CategoryParentId=" + CategoryParentId + " AND Status!=99 AND (" + q + ")";
                }
                else
                {
                    if (languageId > 0)
                        q = "CategoryParentId=" + CategoryParentId + " AND LanguageId=" + languageId + " AND Status!=99";
                    else
                        q = "CategoryParentId=" + CategoryParentId + " AND Status!=99";
                }
            }
            else
            {
                if (query != "" && CategoryParentId == 0)
                {
                    q = query + " AND Status!=99";
                }
                else
                {
                    q = "CategoryParentId=" + CategoryParentId + " AND Status!=99";
                }
            }

            using (var db = new IOITDataContext())
            {
                FullCategorySort obj = new FullCategorySort();
                var data = await db.Category.Where(q).Select(e => new CategorySort
                {
                    CategoryId = e.CategoryId,
                    LanguageId = e.LanguageId,
                    Code = e.Code,
                    Name = e.Name,
                    Location = e.Location,
                    Level = Level,
                    CategoryParentName = CategoryParentName,
                    Image = e.Image,
                    Url = e.Url,
                    IsShow = e.Status == 1 ? true : false,
                    Descriptions = e.Description,
                    CreatedAt = e.CreatedAt,
                    listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.CategoryId || a.TargetId2 == e.CategoryId)
                                    && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY && a.Status != (int)Const.Status.DELETED).Select(a => new LanguageCategoryDT
                                    {
                                        lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new LanguageDT
                                        {
                                            LanguageId = l.LanguageId,
                                            Name = l.Name,
                                            Flag = l.Flag
                                        }).FirstOrDefault(),
                                        category = db.Category.Where(l => (l.CategoryId == a.TargetId1 || l.CategoryId == a.TargetId2) && l.CategoryId != e.CategoryId).Select(l => new CategoryDTL
                                        {
                                            CategoryId = l.CategoryId,
                                            Name = l.Name,
                                            Url = l.Url
                                        }).FirstOrDefault(),
                                    }).ToList(),
                    language = db.Language.Where(a => a.LanguageId == e.LanguageId
                                     && a.Status != (int)Const.Status.DELETED).Select(a => new LanguageDT
                                     {
                                         LanguageId = a.LanguageId,
                                         Name = a.Name,
                                         Flag = a.Flag,
                                         Code = a.Code,
                                     }).FirstOrDefault(),

                }).OrderBy(e => e.Location).ToListAsync();

                List<CategorySort> listData = new List<CategorySort>();

                foreach (var item in data)
                {
                    //if (item.LanguageId == languageId)
                    //{
                    index = index + 1;
                    var child = await GetCategorySortFunction(item.CategoryId, query, Level + 1, item.Name, cs, index, languageId);
                    index = (int)child.Sum;
                    item.categorySorts = child.categorySorts;
                    listData.Add(item);
                    //}
                    //else
                    //{
                    //    //Nếu danh mục đó ko phải là danh mục có ngôn ngữ gốc thì check xem danh mục đó có map vs danh mục nào ko, 
                    //    //nếu không map thì cho hiện bình con nhà thường
                    //    var mapLang = await db.LanguageMapping.Where(e => e.LanguageId2 == item.LanguageId && e.TargetId2 == item.CategoryId).FirstOrDefaultAsync();
                    //    if (mapLang == null)
                    //    {
                    //        index = index + 1;
                    //        var child = await GetCategorySortFunction(item.CategoryId, query, Level + 1, item.Name, cs, index, languageId);
                    //        index = (int)child.Sum;
                    //        item.categorySorts = child.categorySorts;
                    //        listData.Add(item);
                    //    }
                    //}
                }

                //log.Error("Index:" + index);
                obj.categorySorts = listData;
                obj.Sum = index;
                return obj;
            }
        }

        [HttpPost("SaveCategorySort")]
        public async Task<IActionResult> SaveCategorySort([FromBody] List<CategorySort> data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            if (data == null)
            {
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }

            try
            {
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        foreach (var item in data)
                        {
                            Category category = await db.Category.Where(c => c.CategoryId == item.CategoryId && c.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                            if (category != null)
                            {
                                category.CategoryParentId = item.CategoryParentId != null ? (int)item.CategoryParentId : 0;
                                category.Location = item.Location;
                                db.Update(category);
                            }
                        }

                        await db.SaveChangesAsync();
                        transaction.Commit();
                        def.meta = new Meta(200, "Sắp xếp thành công!");
                        def.data = "Success";
                        return Ok(def);
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

        #endregion

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}