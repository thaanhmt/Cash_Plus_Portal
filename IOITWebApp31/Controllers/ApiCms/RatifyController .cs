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
    public class RatifyController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("news", "news");
        private static string functionCode = "QLTT";

        // GET: api/Ratify
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
                try
                {
                    using (var db = new IOITDataContext())
                    {
                        string cat = "CategoryId";
                        def.meta = new Meta(200, "Success");
                        IQueryable<Ratify> data = db.Ratify.Where(c => c.Status != (int)Const.Status.DELETED);

                        //foreach (var item in data)
                        //{
                        //    var pLink = db.PermaLink.Where(e => e.Slug == item.Url.Trim().ToLower() && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        //    if (pLink == null)
                        //    {
                        //        //Thêm permalink
                        //        PermaLink permaLink = new PermaLink();
                        //        permaLink.PermaLinkId = Guid.NewGuid();
                        //        permaLink.Slug = item.Url.Trim().ToLower();
                        //        permaLink.TargetId = item.NewsId;
                        //        permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS;
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
                        //var aaa = paging.query.IndexOf(cat);
                        //string[] arrListQ = paging.query.Split("and");
                        //if (arrListQ.Count() > 0)
                        //{
                        //    foreach (var item in arrListQ)
                        //    {
                        //        string[] arrListStr = item.Split('=');
                        //        if (arrListStr[0].Trim() == cat)
                        //        {
                        //            data = (from cm in db.CategoryMapping
                        //                    join ne in db.News on cm.TargetId equals ne.NewsId
                        //                    where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                        //                    && cm.CategoryId == int.Parse(arrListStr[1])
                        //                    && ne.Status == (int)Const.Status.NORMAL
                        //                    && cm.Status != (int)Const.Status.DELETED
                        //                    select ne
                        //                    );
                        //            break;
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        data = data.Where(paging.query);
                        //}

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
                                data = data.OrderBy("RatifyId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                                data = data.OrderBy("RatifyId desc");
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
                            def.data = await data.Select(e => new
                            {
                                e.RatifyId,
                                e.Title,
                                e.Description,
                                e.Contents,
                                e.Image,
                                e.Url,
                                e.DateStartActive,
                                e.DateStartOn,
                                e.DateEndOn,
                                e.IsHome,
                                e.IsHot,
                                e.LanguageId,
                                e.CompanyId,
                                e.WebsiteId,
                                e.Introduce,
                                e.SystemDiagram,
                                e.ViewNumber,
                                e.Location,
                                e.TypeNewsId,
                                e.MetaTitle,
                                e.MetaKeyword,
                                e.MetaDescription,
                                e.CreatedAt,
                                e.UpdatedAt,
                                e.UserId,
                                e.Status,
                                e.YearTimeline,
                                e.LinkVideo,
                                e.Author,
                                listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.RatifyId && cp.CategoryId != -1
                                    && cp.Status != (int)Const.Status.DELETED).Select(p => new
                                    {
                                        p.CategoryId,
                                        Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
                                        Check = true
                                    }).ToList(),
                                listTag = db.TagMapping.Where(tm => tm.TargetId == e.RatifyId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).Select(p => new
                                {
                                    p.TagId,
                                    Name = db.Tag.Where(t => t.TagId == p.TagId).FirstOrDefault().Name
                                }).ToList(),
                                listAttachment = db.Attactment.Where(a => a.TargetId == e.RatifyId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
                                listRelated = db.Related.Where(r => r.TargetId == e.RatifyId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new
                                {
                                    r.TargetRelatedId
                                }).ToList(),
                                listProductRelated = db.Related.Where(r => r.TargetId == e.RatifyId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new
                                {
                                    r.TargetRelatedId
                                }).ToList(),
                                language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new
                                {
                                    l.LanguageId,
                                    l.Flag,
                                    l.Name,
                                    l.Code
                                }).FirstOrDefault(),
                                listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.RatifyId || a.TargetId2 == e.RatifyId)
                                && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new
                                {
                                    lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new
                                    {
                                        l.LanguageId,
                                        l.Name,
                                        l.Flag
                                    }).FirstOrDefault(),
                                    ratifies = db.Ratify.Where(l => (l.RatifyId == a.TargetId1 || l.RatifyId == a.TargetId2) && l.RatifyId != e.RatifyId).Select(l => new
                                    {
                                        l.RatifyId,
                                        l.Title,
                                        l.Url
                                    }).FirstOrDefault(),
                                }).ToList(),

                            }).ToListAsync();
                        }

                        return Ok(def);
                    }
                }
                catch (Exception e)
                {
                    log.Error("Exception:" + e);
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }



        // Get Bài viết chưa duyệt theo user
        // GET: api/Ratify/1
        [HttpGet("GetByPage/{id}")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging, int id)
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
                try
                {
                    using (var db = new IOITDataContext())
                    {
                        string cat = "CategoryId";
                        def.meta = new Meta(200, "Success");
                        IQueryable<Ratify> data = db.Ratify.Where(c => c.Status != (int)Const.Status.DELETED && c.UserId == id);

                        //foreach (var item in data)
                        //{
                        //    var pLink = db.PermaLink.Where(e => e.Slug == item.Url.Trim().ToLower() && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        //    if (pLink == null)
                        //    {
                        //        //Thêm permalink
                        //        PermaLink permaLink = new PermaLink();
                        //        permaLink.PermaLinkId = Guid.NewGuid();
                        //        permaLink.Slug = item.Url.Trim().ToLower();
                        //        permaLink.TargetId = item.NewsId;
                        //        permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS;
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
                        //var aaa = paging.query.IndexOf(cat);
                        //string[] arrListQ = paging.query.Split("and");
                        //if (arrListQ.Count() > 0)
                        //{
                        //    foreach (var item in arrListQ)
                        //    {
                        //        string[] arrListStr = item.Split('=');
                        //        if (arrListStr[0].Trim() == cat)
                        //        {
                        //            data = (from cm in db.CategoryMapping
                        //                    join ne in db.News on cm.TargetId equals ne.NewsId
                        //                    where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                        //                    && cm.CategoryId == int.Parse(arrListStr[1])
                        //                    && ne.Status == (int)Const.Status.NORMAL
                        //                    && cm.Status != (int)Const.Status.DELETED
                        //                    select ne
                        //                    );
                        //            break;
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        data = data.Where(paging.query);
                        //}

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
                                data = data.OrderBy("RatifyId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                                data = data.OrderBy("Ratify desc");
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
                            def.data = await data.Select(e => new
                            {
                                e.RatifyId,
                                e.Title,
                                e.Description,
                                e.Contents,
                                e.Image,
                                e.Url,
                                e.DateStartActive,
                                e.DateStartOn,
                                e.DateEndOn,
                                e.IsHome,
                                e.IsHot,
                                e.LanguageId,
                                e.CompanyId,
                                e.WebsiteId,
                                e.Introduce,
                                e.SystemDiagram,
                                e.ViewNumber,
                                e.Location,
                                e.TypeNewsId,
                                e.MetaTitle,
                                e.MetaKeyword,
                                e.MetaDescription,
                                e.CreatedAt,
                                e.UpdatedAt,
                                e.UserId,
                                e.Status,
                                e.YearTimeline,
                                e.LinkVideo,
                                e.Author,
                                listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.RatifyId && cp.CategoryId != -1
                                    && cp.Status != (int)Const.Status.DELETED).Select(p => new
                                    {
                                        p.CategoryId,
                                        Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
                                        Check = true
                                    }).ToList(),
                                listTag = db.TagMapping.Where(tm => tm.TargetId == e.RatifyId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).Select(p => new
                                {
                                    p.TagId,
                                    Name = db.Tag.Where(t => t.TagId == p.TagId).FirstOrDefault().Name
                                }).ToList(),
                                listAttachment = db.Attactment.Where(a => a.TargetId == e.RatifyId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
                                listRelated = db.Related.Where(r => r.TargetId == e.RatifyId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new
                                {
                                    r.TargetRelatedId
                                }).ToList(),
                                listProductRelated = db.Related.Where(r => r.TargetId == e.RatifyId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new
                                {
                                    r.TargetRelatedId
                                }).ToList(),
                                language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new
                                {
                                    l.LanguageId,
                                    l.Flag,
                                    l.Name,
                                    l.Code
                                }).FirstOrDefault(),
                                listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.RatifyId || a.TargetId2 == e.RatifyId)
                                && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new
                                {
                                    lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new
                                    {
                                        l.LanguageId,
                                        l.Name,
                                        l.Flag
                                    }).FirstOrDefault(),
                                    ratiies = db.Ratify.Where(l => (l.RatifyId == a.TargetId1 || l.RatifyId == a.TargetId2) && l.RatifyId != e.RatifyId).Select(l => new
                                    {
                                        l.RatifyId,
                                        l.Title,
                                        l.Url
                                    }).FirstOrDefault(),
                                }).ToList(),

                            }).ToListAsync();
                        }

                        return Ok(def);
                    }
                }
                catch (Exception e)
                {
                    log.Error("Exception:" + e);
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }
        // GET: api/Ratify/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRatify(int id)
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
                    var data = await db.Ratify.Where(e => e.RatifyId == id).Select(e => new
                    {
                        e.RatifyId,
                        e.Title,
                        e.Description,
                        e.Contents,
                        e.Image,
                        e.Url,
                        e.DateStartActive,
                        e.DateStartOn,
                        e.DateEndOn,
                        e.IsHome,
                        e.IsHot,
                        e.LanguageId,
                        e.CompanyId,
                        e.WebsiteId,
                        e.ViewNumber,
                        e.Location,
                        e.TypeNewsId,
                        e.MetaTitle,
                        e.MetaKeyword,
                        e.MetaDescription,
                        e.CreatedAt,
                        e.UpdatedAt,
                        e.UserId,
                        e.Status,
                        e.LinkVideo,
                        e.Author,
                        listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.RatifyId
                            && cp.Status != (int)Const.Status.DELETED).Select(p => new
                            {
                                p.CategoryId,
                                Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
                                Check = true
                            }).ToList(),
                        listTag = db.Tag.Where(t => t.TargetId == e.RatifyId && t.Status != (int)Const.Status.DELETED).Select(p => new
                        {
                            p.TagId,
                            p.Name,
                            Check = true
                        }).ToList(),
                        listAttachment = db.Attactment.Where(a => a.TargetId == e.RatifyId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
                        listRelated = db.Related.Where(r => r.TargetId == e.RatifyId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new
                        {
                            r.TargetRelatedId
                        }).ToList(),
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



        // GET: api/GetAuthor
        [HttpGet("GetAuthor")]
        public async Task<IActionResult> GetAuthor()
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
                    var data = await db.Ratify.Where(e => e.Status != (int)Const.Status.DELETED).Select(e => new
                    {
                        e.UserId,
                        e.Author,
                    }).Distinct().ToListAsync();

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
        // PUT: api/Ratify/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRatify(int id, [FromBody] RatifyDTO data)
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

                if (data.Title == null || data.Title == "")
                {
                    def.meta = new Meta(211, "Title Null!");
                    return Ok(def);
                }

                //if (data.Contents == null || data.Contents == "")
                //{
                //    def.meta = new Meta(2111, "Contents Null!");
                //    return Ok(def);
                //}

                if (data.TypeNewsId == null || data.TypeNewsId == -1)
                {
                    def.meta = new Meta(2112, "TypeRatifyId Null!");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Ratify ratify = await db.Ratify.FindAsync(id);
                        if (ratify == null)
                        {
                            def.meta = new Meta(404, "Not found!");
                            return Ok(def);
                        }

                        string url = data.Url == null ? Utils.NonUnicode(data.Title) : data.Url;
                        url = url.Trim().ToLower();
                        if (ratify.Url != url)
                        {
                            //check xem trùng link ko
                            var pLink = db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (pLink != null)
                            {
                                def.meta = new Meta(232, "Link đã tồn tại!");
                                return Ok(def);
                            }
                            //cập nhật thay link cũ
                            var permaLink = db.PermaLink.Where(e => e.Slug == ratify.Url && e.TargetId == ratify.RatifyId
                            && e.TargetType == (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS).FirstOrDefault();
                            if (permaLink != null)
                            {
                                permaLink.TargetId = ratify.RatifyId;
                                permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS;
                                permaLink.Slug = url;
                                permaLink.UpdatedAt = DateTime.Now;
                                permaLink.Status = (int)Const.Status.NORMAL;
                                db.PermaLink.Update(permaLink);
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                PermaLink permaLink1 = new PermaLink();
                                permaLink1.PermaLinkId = Guid.NewGuid();
                                permaLink1.Slug = ratify.Url;
                                permaLink1.TargetId = ratify.RatifyId;
                                permaLink1.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS;
                                permaLink1.CreatedAt = DateTime.Now;
                                permaLink1.UpdatedAt = DateTime.Now;
                                permaLink1.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink1);
                                await db.SaveChangesAsync();
                            }

                        }
                        else
                        {
                            var permaLink = db.PermaLink.Where(e => e.Slug == ratify.Url && e.TargetId == ratify.RatifyId
                            && e.TargetType == (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS).FirstOrDefault();
                            if (permaLink == null)
                            {
                                PermaLink permaLink1 = new PermaLink();
                                permaLink1.PermaLinkId = Guid.NewGuid();
                                permaLink1.Slug = ratify.Url;
                                permaLink1.TargetId = ratify.RatifyId;
                                permaLink1.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS;
                                permaLink1.CreatedAt = DateTime.Now;
                                permaLink1.UpdatedAt = DateTime.Now;
                                permaLink1.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink1);
                                await db.SaveChangesAsync();
                            }
                        }

                        ratify.Title = data.Title;
                        ratify.Description = data.Description;
                        ratify.Contents = data.Contents;
                        ratify.Image = data.Image;
                        ratify.Url = data.Url;
                        ratify.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        ratify.DateStartOn = data.DateStartOn != null ? data.DateStartOn : ratify.DateStartActive;
                        ratify.DateEndOn = data.DateEndOn != null ? data.DateEndOn : ratify.DateStartActive.Value.AddYears(100);
                        ratify.IsHome = data.IsHome != null ? data.IsHome : false;
                        ratify.IsHot = data.IsHot != null ? data.IsHot : false;
                        ratify.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                        ratify.Location = data.Location;
                        ratify.TypeNewsId = data.TypeNewsId;
                        ratify.MetaTitle = data.MetaTitle;
                        ratify.MetaKeyword = data.MetaKeyword;
                        ratify.MetaDescription = data.MetaDescription;
                        ratify.UpdatedAt = DateTime.Now;
                        ratify.UserId = data.UserId;
                        ratify.Status = data.Status;
                        ratify.LinkVideo = data.LinkVideo;
                        ratify.Introduce = data.Introduce;
                        ratify.SystemDiagram = data.SystemDiagram;
                        ratify.Author = data.Author;
                        ratify.YearTimeline = data.YearTimeline;

                        db.Entry(ratify).State = EntityState.Modified;

                        //remove category mapping

                        //add category mapping
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
                                        categoryNewsMapping.TargetId = ratify.RatifyId;
                                        categoryNewsMapping.TargetType = (int)Const.TypeCategoryMapping.CATEGORY_NEWS;
                                        categoryNewsMapping.Location = db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList() != null ? db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList().Count : 1;
                                        categoryNewsMapping.Status = (int)Const.Status.NORMAL;
                                        db.CategoryMapping.Add(categoryNewsMapping);
                                    }
                                }
                                else
                                {
                                    if (item.Check != true)
                                    {
                                        exist.Status = (int)Const.Status.DELETED;
                                        db.Entry(exist).State = EntityState.Modified;
                                    }
                                }
                            }
                        }

                        //remove tag
                        var tapMappings = db.TagMapping.Where(tm => tm.TargetId == data.RatifyId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).ToList();
                        tapMappings.ForEach(l => l.Status = (int)Const.Status.DELETED);

                        //add tag
                        if (data.listTag != null)
                        {
                            foreach (var item in data.listTag)
                            {
                                TagMapping tagMapping = new TagMapping();
                                tagMapping.TagId = item.TagId;
                                tagMapping.TargetId = data.RatifyId;
                                tagMapping.Type = (int)Const.TypeTag.TAG_NEWS;
                                tagMapping.UserId = data.UserId;
                                tagMapping.CreatedAt = DateTime.Now;
                                tagMapping.Status = (int)Const.Status.NORMAL;
                                db.TagMapping.Add(tagMapping);
                            }
                        }

                        if (data.TypeNewsId == (int)Const.TypeNews.NEWS_IMAGE || data.TypeNewsId == 7)
                        {
                            if (data.listAttachment != null)
                            {
                                foreach (var item in data.listAttachment)
                                {
                                    if (item.AttactmentId != null)
                                    {
                                        var attachmentExist = db.Attactment.Find(item.AttactmentId);
                                        if (attachmentExist != null)
                                        {
                                            if (item.Status == (int)Const.Status.DELETED)
                                            {
                                                attachmentExist.Status = (int)Const.Status.DELETED;
                                            }
                                            else
                                            {
                                                attachmentExist.IsImageMain = item.IsImageMain;
                                                attachmentExist.Note = item.Note;
                                            }
                                        }
                                        db.Entry(attachmentExist).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        Attactment attactment = new Attactment();
                                        attactment.Name = item.Name;
                                        attactment.TargetId = ratify.RatifyId;
                                        attactment.IsImageMain = item.IsImageMain;
                                        attactment.TargetType = (int)Const.TypeAttachment.NEWS_IMAGE;
                                        attactment.Url = item.Url;
                                        attactment.Note = item.Note;
                                        attactment.Thumb = item.Thumb;
                                        attactment.CreatedAt = DateTime.Now;
                                        attactment.CreatedId = data.UserId;
                                        attactment.UpdatedId = data.UserId;
                                        attactment.Status = (int)Const.Status.NORMAL;
                                        db.Attactment.Add(attactment);

                                        if (item.IsImageMain == true)
                                        {
                                            ratify.Image = item.Url;
                                            db.Entry(ratify).State = EntityState.Modified;
                                        }
                                    }

                                    if (item.IsImageMain == true && item.Status != (int)Const.Status.DELETED)
                                    {
                                        ratify.Image = item.Url;
                                    }
                                    db.Entry(ratify).State = EntityState.Modified;
                                }
                            }
                        }

                        //bai viet  gợi ý
                        List<Related> listRelated = db.Related.Where(r => r.TargetId == ratify.RatifyId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).ToList();
                        if (listRelated != null)
                        {
                            listRelated.ForEach(lr => lr.Status = (int)Const.Status.DELETED);
                        }

                        if (data.listRelated != null)
                        {
                            foreach (var item in data.listRelated)
                            {
                                Related related = new Related();
                                related.TargetId = ratify.RatifyId;
                                related.TargetRelatedId = item.TargetRelatedId;
                                related.TargetType = (int)Const.TypeRelated.NEWS_NEWS;
                                related.Location = item.Location;
                                related.CreatedAt = DateTime.Now;
                                related.UserId = data.UserId;
                                related.Status = (int)Const.Status.NORMAL;
                                db.Related.Add(related);
                            }
                        }

                        //Sản phẩm lien quan den bai vet
                        List<Related> listProductRelated = db.Related.Where(r => r.TargetId == ratify.RatifyId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).ToList();
                        if (listProductRelated != null)
                        {
                            listProductRelated.ForEach(lr => lr.Status = (int)Const.Status.DELETED);
                        }

                        if (data.listProductRelated != null)
                        {
                            foreach (var item in data.listProductRelated)
                            {
                                Related related = new Related();
                                related.TargetId = ratify.RatifyId;
                                related.TargetRelatedId = item.TargetRelatedId;
                                related.TargetType = (int)Const.TypeRelated.NEWS_PRODUCT;
                                related.Location = item.Location;
                                related.CreatedAt = DateTime.Now;
                                related.UserId = data.UserId;
                                related.Status = (int)Const.Status.NORMAL;
                                db.Related.Add(related);
                            }
                        }

                        CategoryMapping categoryMapping = db.CategoryMapping.Where(cm => cm.CategoryId == -1 && cm.TargetId == id && cm.TargetType == (int)Const.TypeOrderBy.NEWS_IS_HOME && cm.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (ratify.IsHome == true)
                        {
                            if (categoryMapping == null)
                            {
                                CategoryMapping cm = new CategoryMapping();
                                cm.CategoryId = -1;
                                cm.TargetId = ratify.RatifyId;
                                cm.TargetType = (int)Const.TypeOrderBy.NEWS_IS_HOME;
                                cm.Location = 99;
                                cm.CreatedAt = DateTime.Now;
                                cm.Status = (int)Const.Status.NORMAL;
                                db.CategoryMapping.Add(cm);
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

                            if (data.RatifyId > 0)
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
                            if (!NewsExists(data.RatifyId))
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

        // POST: api/News
        [HttpPost]
        public async Task<IActionResult> PostRatify(RatifyDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            int websiteId = int.Parse(identity.Claims.Where(c => c.Type == "WebsiteId").Select(c => c.Value).SingleOrDefault());
            int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
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

                if (data.UserId == null || data.UserId != userId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (data.CompanyId == null || data.CompanyId != companyId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (data.Title == null || data.Title == "")
                {
                    def.meta = new Meta(211, "Title Null!");
                    return Ok(def);
                }

                //if (data.Contents == null || data.Contents == "")
                //{
                //    def.meta = new Meta(2111, "Contents Null!");
                //    return Ok(def);
                //}

                if (data.TypeNewsId == null || data.TypeNewsId == -1)
                {
                    def.meta = new Meta(2112, "TypeNewsId Null!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //check xem trùng link ko
                        string url = data.Url == null ? Utils.NonUnicode(data.Title) : data.Url;
                        url = url.Trim().ToLower();
                        var pLink = db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (pLink != null)
                        {
                            def.meta = new Meta(232, "Link đã tồn tại!");
                            return Ok(def);
                        }

                        //check đã thêm bài viết cho ngôn ngữ này chưa
                        var checkLang = await db.LanguageMapping.Where(e => e.LanguageId1 == languageId && e.LanguageId2 == data.LanguageId
                          && e.TargetId1 == data.RatifyRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                          && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();

                        if (checkLang != null)
                        {
                            def.meta = new Meta(228, "Language Exits!");
                            return Ok(def);
                        }

                        //add news
                        Ratify ratify = new Ratify();
                        ratify.Title = data.Title;
                        ratify.Description = data.Description == null ? "" : data.Description;
                        ratify.Contents = data.Contents;
                        ratify.Image = data.Image;
                        ratify.Url = data.Url != null ? data.Url : Utils.NonUnicode(ratify.Title);
                        ratify.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        ratify.DateStartOn = data.DateStartOn == null ? DateTime.Now : data.DateStartOn;
                        ratify.DateEndOn = data.DateEndOn == null ? DateTime.Now.AddYears(100) : data.DateEndOn;
                        ratify.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                        ratify.IsHome = data.IsHome != null ? data.IsHome : false;
                        ratify.IsHot = data.IsHot != null ? data.IsHot : false;
                        ratify.Location = data.Location != null ? data.Location : 1;
                        ratify.TypeNewsId = data.TypeNewsId != null ? data.TypeNewsId : (int)Const.TypeNews.NEWS_TEXT;
                        ratify.MetaTitle = data.MetaTitle != null ? data.MetaTitle : data.Title;
                        ratify.MetaKeyword = data.MetaKeyword != null ? data.MetaKeyword : data.Title;
                        ratify.MetaDescription = data.MetaDescription != null ? data.MetaDescription : data.Description;
                        ratify.LanguageId = data.LanguageId != null ? data.LanguageId : languageId;
                        ratify.WebsiteId = data.WebsiteId != null ? data.WebsiteId : websiteId;
                        ratify.CompanyId = data.CompanyId != null ? data.CompanyId : companyId;
                        ratify.CreatedAt = DateTime.Now;
                        ratify.UpdatedAt = DateTime.Now;
                        ratify.UserId = data.UserId;
                        ratify.Status = data.Status;
                        ratify.Introduce = data.Introduce;
                        ratify.LinkVideo = data.LinkVideo;
                        ratify.SystemDiagram = data.SystemDiagram;
                        ratify.Author = db.User.Where(u => u.UserId == ratify.UserId).Select(u => u.FullName).FirstOrDefault();
                        ratify.YearTimeline = data.YearTimeline;

                        await db.Ratify.AddAsync(ratify);
                        await db.SaveChangesAsync();

                        data.RatifyId = ratify.RatifyId;
                        if (data.RatifyRootId == null) data.RatifyRootId = ratify.RatifyId;

                        if (ratify.IsHome == true)
                        {
                            CategoryMapping categoryMapping = new CategoryMapping();
                            categoryMapping.CategoryId = -1;
                            categoryMapping.TargetId = ratify.RatifyId;
                            categoryMapping.TargetType = (int)Const.TypeOrderBy.NEWS_IS_HOME;
                            categoryMapping.Location = 99;
                            categoryMapping.CreatedAt = DateTime.Now;
                            categoryMapping.Status = (int)Const.Status.NORMAL;
                            db.CategoryMapping.Add(categoryMapping);
                        }

                        //add category mapping
                        if (data.listCategory != null)
                        {
                            foreach (var item in data.listCategory)
                            {
                                CategoryMapping categoryNewsMapping = new CategoryMapping();
                                categoryNewsMapping.CategoryId = item.CategoryId;
                                categoryNewsMapping.TargetId = ratify.RatifyId;
                                categoryNewsMapping.TargetType = (int)Const.TypeCategoryMapping.CATEGORY_NEWS;
                                categoryNewsMapping.Location = db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList() != null ? db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList().Count : 1;
                                categoryNewsMapping.CreatedAt = DateTime.Now;
                                categoryNewsMapping.Status = (int)Const.Status.NORMAL;
                                db.CategoryMapping.Add(categoryNewsMapping);
                            }
                        }

                        //add tag
                        if (data.listTag != null)
                        {
                            foreach (var item in data.listTag)
                            {
                                TagMapping tagMapping = new TagMapping();
                                tagMapping.TagId = item.TagId;
                                tagMapping.TargetId = data.RatifyId;
                                tagMapping.Type = (int)Const.TypeTag.TAG_NEWS;
                                tagMapping.UserId = userId;
                                tagMapping.CreatedAt = DateTime.Now;
                                tagMapping.Status = (int)Const.Status.NORMAL;
                                db.TagMapping.Add(tagMapping);
                            }
                        }

                        //add list Image Product
                        if (data.TypeNewsId == (int)Const.TypeNews.NEWS_IMAGE || data.TypeNewsId == 7)
                        {
                            if (data.listAttachment != null)
                            {
                                foreach (var item in data.listAttachment)
                                {
                                    Attactment attactment = new Attactment();
                                    attactment.Name = item.Name;
                                    attactment.TargetId = ratify.RatifyId;
                                    attactment.IsImageMain = item.IsImageMain;
                                    attactment.TargetType = (int)Const.TypeAttachment.NEWS_IMAGE;
                                    attactment.Url = item.Url;
                                    attactment.Note = item.Note;
                                    attactment.Thumb = item.Thumb;
                                    attactment.CreatedAt = DateTime.Now;
                                    attactment.CreatedId = userId;
                                    attactment.UpdatedId = userId;
                                    attactment.Status = (int)Const.Status.NORMAL;
                                    db.Attactment.Add(attactment);

                                    if (item.IsImageMain == true)
                                    {
                                        ratify.Image = item.Url;
                                    }
                                }
                                db.Entry(ratify).State = EntityState.Modified;
                            }
                        }

                        //add product related
                        if (data.listRelated != null)
                        {
                            foreach (var item in data.listRelated)
                            {
                                Related related = new Related();
                                related.TargetId = ratify.RatifyId;
                                related.TargetRelatedId = item.TargetRelatedId;
                                related.TargetType = (int)Const.TypeRelated.NEWS_NEWS;
                                related.Location = item.Location;
                                related.CreatedAt = DateTime.Now;
                                related.UserId = userId;
                                related.Status = (int)Const.Status.NORMAL;
                                db.Related.Add(related);
                            }
                        }

                        //add product related
                        if (data.listProductRelated != null)
                        {
                            foreach (var item in data.listProductRelated)
                            {
                                Related related = new Related();
                                related.TargetId = ratify.RatifyId;
                                related.TargetRelatedId = item.TargetRelatedId;
                                related.TargetType = (int)Const.TypeRelated.NEWS_PRODUCT;
                                related.Location = item.Location;
                                related.CreatedAt = DateTime.Now;
                                related.UserId = userId;
                                related.Status = (int)Const.Status.NORMAL;
                                db.Related.Add(related);
                            }
                        }

                        try
                        {
                            await db.SaveChangesAsync();
                            data.listLanguage = new List<LanguageMappingDTO>();
                            if (data.RatifyId > 0)
                            {
                                //Thêm permalink
                                PermaLink permaLink = new PermaLink();
                                permaLink.PermaLinkId = Guid.NewGuid();
                                permaLink.Slug = ratify.Url;
                                permaLink.TargetId = ratify.RatifyId;
                                permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS;
                                permaLink.CreatedAt = DateTime.Now;
                                permaLink.UpdatedAt = DateTime.Now;
                                permaLink.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink);
                                await db.SaveChangesAsync();
                                //Thêm ngôn ngữ (nếu bài viết mới thêm ko phải là ngôn ngữ mạc định)
                                //và có id bài viết gốc
                                if (data.LanguageId != languageId && data.LanguageId != null && data.LanguageId > 0
                                    && data.RatifyRootId != null && data.RatifyRootId > 0)
                                {
                                    var listLang = db.LanguageMapping.Where(e => e.LanguageId1 == languageId
                                      && e.TargetId1 == data.RatifyRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                                      && e.Status != (int)Const.Status.DELETED).ToList();
                                    if (listLang.Count > 0)
                                    {
                                        LanguageMapping languageMapping = new LanguageMapping();
                                        languageMapping.LanguageId1 = languageId;
                                        languageMapping.LanguageId2 = data.LanguageId;
                                        languageMapping.TargetId1 = data.RatifyRootId;
                                        languageMapping.TargetId2 = data.RatifyId;
                                        languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_NEWS;
                                        languageMapping.CreatedAt = DateTime.Now;
                                        languageMapping.Status = (int)Const.Status.NORMAL;
                                        await db.LanguageMapping.AddAsync(languageMapping);

                                        foreach (var item in listLang)
                                        {
                                            LanguageMapping languageMapping2 = new LanguageMapping();
                                            languageMapping2.LanguageId1 = item.LanguageId2;
                                            languageMapping2.LanguageId2 = data.LanguageId;
                                            languageMapping2.TargetId1 = item.TargetId2;
                                            languageMapping2.TargetId2 = data.RatifyId;
                                            languageMapping2.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_NEWS;
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
                                        languageMapping.TargetId1 = data.RatifyRootId;
                                        languageMapping.TargetId2 = data.RatifyId;
                                        languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_NEWS;
                                        languageMapping.CreatedAt = DateTime.Now;
                                        languageMapping.Status = (int)Const.Status.NORMAL;
                                        await db.LanguageMapping.AddAsync(languageMapping);
                                    }
                                    await db.SaveChangesAsync();
                                    //numLang = listLang.Count + 2;
                                }
                                transaction.Commit();

                                data.listLanguage = db.LanguageMapping.Where(a => a.LanguageId1 == languageId && a.TargetId1 == (int)data.RatifyRootId
                                   && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new LanguageMappingDTO
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
                            if (NewsExists(data.RatifyId))
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

        // DELETE: api/News/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRatify(int id)
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
                    Ratify data = await db.Ratify.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.Ratify.Update(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.RatifyId > 0)
                            {
                                //Xóa ngôn ngữ map cùng
                                var mapLang = await db.LanguageMapping.Where(e => (e.TargetId1 == id || e.TargetId2 == id)
                                 && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && e.Status != (int)Const.Status.DELETED).ToListAsync();
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
                                var permaLink = db.PermaLink.Where(e => e.Slug == data.Url && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (permaLink != null)
                                {
                                    permaLink.UpdatedAt = DateTime.Now;
                                    permaLink.Status = (int)Const.Status.DELETED;
                                    db.PermaLink.Update(permaLink);
                                    await db.SaveChangesAsync();
                                }

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
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!NewsExists(data.RatifyId))
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

        //API xóa danh sách bài viết
        [HttpPut("deletes")]
        public async Task<IActionResult> DeleteMultiNews([FromBody] int[] data)
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
                        News news = await db.News.FindAsync(data[i]);

                        if (news == null)
                        {
                            continue;
                        }

                        news.UserId = userId;
                        news.UpdatedAt = DateTime.Now;
                        news.Status = (int)Const.Status.DELETED;
                        db.Entry(news).State = EntityState.Modified;

                        //Xóa ngôn ngữ map cùng
                        var mapLang = await db.LanguageMapping.Where(e => (e.TargetId1 == news.NewsId || e.TargetId2 == news.NewsId)
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && e.Status != (int)Const.Status.DELETED).ToListAsync();
                        if (mapLang.Count > 0)
                        {
                            foreach (var item in mapLang)
                            {
                                item.Status = (int)Const.Status.DELETED;
                            }
                            db.LanguageMapping.UpdateRange(mapLang);
                        }

                        //Xóa link
                        var permaLink = db.PermaLink.Where(e => e.Slug == news.Url && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
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

        [HttpPut("ShowHide/{id}/{stt}/{isAll}")]
        public async Task<ActionResult> ShowHide(int id, int stt, int isAll)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
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
                    News data = await db.News.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UpdatedAt = DateTime.Now;
                        data.UserId = userId;
                        data.Status = (byte)stt;
                        db.News.Update(data);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (isAll == 1)
                            {
                                //Update những bài viết có ngôn ngữ khác
                                var listLanguage = await db.LanguageMapping.Where(a => a.LanguageId1 == languageId && a.TargetId1 == id
                                         && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).ToListAsync();
                                foreach (var item in listLanguage)
                                {
                                    News dataLang = await db.News.Where(e => e.NewsId == item.TargetId2 && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                                    if (dataLang != null)
                                    {
                                        dataLang.UpdatedAt = DateTime.Now;
                                        dataLang.UserId = userId;
                                        dataLang.Status = (byte)stt;
                                        db.News.Update(dataLang);
                                    }
                                }
                                await db.SaveChangesAsync();
                            }

                            if (data.NewsId > 0)
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
                            if (!NewsExists(data.NewsId))
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

        private bool NewsExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.News.Count(e => e.NewsId == id) > 0;
            }
        }
    }
}


