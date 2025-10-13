using HtmlAgilityPack;
using IOITWebApp31.Models;
using IOITWebApp31.Models.Common;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.ApiCMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("news", "news");
        private static string functionCode = "QLTT";
        private static string functionCodeVB = "BVVB";
        private static string functionCodeBT = "BTBV";
        private static string functionCodeKD = "BVKD";
        private static string functionCodeXB = "QLTT";
        private IHostingEnvironment _hostingEnvironment;
        public NewsController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: api/News
        [HttpGet("GetByPageAll")]
        public async Task<IActionResult> GetByPageAll([FromQuery] FilteredPagination paging)
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
                        def.meta = new Meta(200, "Success");
                        IQueryable<News> data = db.News.Where(c => c.Status != (int)Const.Status.DELETED);
                        IQueryable<Publication> publications = db.Publication.Where(c => c.Status == 1);
                        IQueryable<LegalDoc> legalDocs = db.LegalDoc.Where(c => c.Status == 1);

                        MetaDataDT metaDataDT = new MetaDataDT();
                        metaDataDT.Sum = data.Count(); // Tổng
                        metaDataDT.Normal = data.Where(e => e.Status == 1).Count(); // Tin đã xuất bản
                        metaDataDT.Temp = data.Where(e => e.Status == 10).Count(); // Bài viết nháp
                        metaDataDT.New = data.Where(e => e.Status == 11).Count(); // Bài viết mới
                        metaDataDT.ReNew = data.Where(e => e.Status == 12).Count(); // Bài viết mới bị trả lại
                        metaDataDT.Editing = data.Where(e => e.Status == 13).Count(); // Chờ biên tập
                        metaDataDT.Edited = data.Where(e => e.Status == 14).Count(); // Đã biên tập
                        metaDataDT.ReEdited = data.Where(e => e.Status == 15).Count(); // Biên tập lại
                        metaDataDT.Approving = data.Where(e => e.Status == 16).Count(); // Chờ duyệt
                        metaDataDT.NotApproved = data.Where(e => e.Status == 17).Count(); // Không duyệt
                        metaDataDT.Publishing = data.Where(e => e.Status == 18).Count(); // Chờ xuất bản
                        metaDataDT.UnPublish = data.Where(e => e.Status == 19).Count(); // Gỡ xuất bản

                        metaDataDT.BaiViet = data.Where(e => e.Status == 11 || e.Status == 12).Count();
                        metaDataDT.BienTap = data.Where(e => e.Status == 13 || e.Status == 14 || e.Status == 15).Count();
                        metaDataDT.KiemDuyet = data.Where(e => e.Status == 16 || e.Status == 17).Count();
                        metaDataDT.NoPublic = data.Where(e => e.Status == 19 || e.Status == 18).Count();

                        metaDataDT.AnPham = publications.Count();

                        metaDataDT.VanBan = legalDocs.Count();

                        def.metadata = metaDataDT;
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
        // GET: api/News
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
                        IQueryable<News> data = db.News.Where(c => c.Status != (int)Const.Status.DELETED);

                        if (paging.query != null)
                        {
                            paging.query = HttpUtility.UrlDecode(paging.query);
                        }
                        //data = data.Where(paging.query);
                        var aaa = paging.query.IndexOf(cat);
                        string[] arrListQ = paging.query.Split("and");
                        if (arrListQ.Count() > 1)
                        {
                            int kk = 0;
                            foreach (var item in arrListQ)
                            {
                                string[] arrListStr = item.Split('=');
                                if (arrListStr[0].Trim() == cat)
                                {
                                    data = (from cm in db.CategoryMapping
                                            join ne in db.News on cm.TargetId equals ne.NewsId
                                            where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                            && cm.CategoryId == int.Parse(arrListStr[1])
                                            && ne.Status != (int)Const.Status.DELETED
                                            && cm.Status != (int)Const.Status.DELETED
                                            select ne
                                            ).AsQueryable();
                                }
                                else
                                {
                                    if (kk == 0)
                                        paging.query = item;
                                    else
                                        paging.query += " AND " + item;
                                }
                                kk++;
                            }
                        }
                        //else
                        //{
                        data = data.Where(paging.query);
                        //}


                        MetaDataDT metaDataDT = new MetaDataDT();
                        metaDataDT.Sum = data.Count();
                        metaDataDT.Normal = data.Where(e => e.Status == 1).Count();
                        metaDataDT.Temp = data.Where(e => e.Status == 10).Count();
                        metaDataDT.New = data.Where(e => e.Status == 11).Count();
                        metaDataDT.ReNew = data.Where(e => e.Status == 12).Count();
                        metaDataDT.Editing = data.Where(e => e.Status == 13).Count();
                        metaDataDT.Edited = data.Where(e => e.Status == 14).Count();
                        metaDataDT.ReEdited = data.Where(e => e.Status == 15).Count();
                        metaDataDT.Approving = data.Where(e => e.Status == 16).Count();
                        metaDataDT.NotApproved = data.Where(e => e.Status == 17).Count();
                        metaDataDT.Publishing = data.Where(e => e.Status == 18).Count();
                        metaDataDT.UnPublish = data.Where(e => e.Status == 19).Count();

                        def.metadata = metaDataDT;

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
                            def.data = await data.Select(paging.select).ToDynamicListAsync();
                        }
                        else
                        {
                            def.data = await data.Select(e => new
                            {
                                e.NewsId,
                                e.Title,
                                e.Description,
                                e.Contents,
                                e.Image,
                                e.Url,
                                e.Note,
                                e.DateStartActive,
                                e.DateStartOn,
                                e.DateEndOn,
                                e.IsHome,
                                e.IsHot,
                                e.IsAttach,
                                e.FactorPrice,
                                e.ValuePrice,
                                e.TotalPrice,
                                e.IsCash,
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
                                e.NumberWord,
                                e.CreatedAt,
                                e.AuthorId,
                                e.UserCreatedId,
                                e.UserEditedId,
                                e.EditingAt,
                                e.EditedAt,
                                e.UserApprovedId,
                                e.ApprovingAt,
                                e.ApprovedAt,
                                e.UserPublishedId,
                                e.PublishingAt,
                                e.PublishedAt,
                                e.UpdatedAt,
                                e.UserId,
                                e.Status,
                                e.YearTimeline,
                                e.LinkVideo,
                                e.Author,
                                e.IsComment,
                                e.IsFirst,
                                e.IsShowView,
                                listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.NewsId && cp.CategoryId != -1
                                    && cp.Status != (int)Const.Status.DELETED).Select(p => new
                                    {
                                        p.CategoryId,
                                        Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
                                        Check = true
                                    }).ToList(),
                                listTag = db.TagMapping.Where(tm => tm.TargetId == e.NewsId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).Select(p => new
                                {
                                    p.TagId,
                                    Name = db.Tag.Where(t => t.TagId == p.TagId).FirstOrDefault().Name
                                }).ToList(),
                                listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
                                listRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new
                                {
                                    r.TargetRelatedId
                                }).ToList(),
                                listProductRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new
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
                                listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.NewsId || a.TargetId2 == e.NewsId)
                                && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new
                                {
                                    lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new
                                    {
                                        l.LanguageId,
                                        l.Name,
                                        l.Flag
                                    }).FirstOrDefault(),
                                    news = db.News.Where(l => (l.NewsId == a.TargetId1 || l.NewsId == a.TargetId2) && l.NewsId != e.NewsId).Select(l => new
                                    {
                                        l.NewsId,
                                        l.Title,
                                        l.Url
                                    }).FirstOrDefault(),
                                }).ToList(),
                                AuthorName = db.User.Where(u => u.UserId == e.UserCreatedId).Select(u => u.FullName).FirstOrDefault(),
                                EditName = db.User.Where(u => u.UserId == e.UserEditedId).Select(u => u.FullName).FirstOrDefault(),
                                ApproveName = db.User.Where(u => u.UserId == e.UserApprovedId).Select(u => u.FullName).FirstOrDefault(),
                                PublicName = db.User.Where(u => u.UserId == e.UserPublishedId).Select(u => u.FullName).FirstOrDefault(),
                                CreateName = db.Author.Where(u => u.AuthorId == e.AuthorId).Select(u => u.FullName).FirstOrDefault(),

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

        [HttpPost("GetByPageCash")]
        public async Task<IActionResult> GetByPageCash([FromBody] FilterReport paging)
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
                        var dateStart = new DateTime(2000, 1, 1);
                        var dateEnd = DateTime.Now;
                        if (paging.DateStart != null)
                            dateStart = new DateTime(paging.DateStart.Value.Year, paging.DateStart.Value.Month, paging.DateStart.Value.Day, 0, 0, 0);
                        if (paging.DateEnd != null)
                            dateEnd = new DateTime(paging.DateEnd.Value.Year, paging.DateEnd.Value.Month, paging.DateEnd.Value.Day, 23, 59, 59);

                        IQueryable<News> data = db.News.Where(c =>
                        c.Status != (int)Const.Status.DELETED
                        && c.CreatedAt >= dateStart && c.CreatedAt <= dateEnd
                        && ((c.TotalPrice != null && paging.CashStatus == true)
                                    || (c.TotalPrice == null && paging.CashStatus != true)
                                    || paging.CashStatus == null));

                        if (paging.query != null)
                        {
                            paging.query = HttpUtility.UrlDecode(paging.query);
                        }
                        //data = data.Where(paging.query);
                        var aaa = paging.query.IndexOf(cat);
                        string[] arrListQ = paging.query.Split("and");
                        if (arrListQ.Count() > 1)
                        {
                            int kk = 0;
                            foreach (var item in arrListQ)
                            {
                                string[] arrListStr = item.Split('=');
                                if (arrListStr[0].Trim() == cat)
                                {
                                    data = (from cm in db.CategoryMapping
                                            join ne in db.News on cm.TargetId equals ne.NewsId
                                            where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                            && cm.CategoryId == int.Parse(arrListStr[1])
                                            && ne.Status != (int)Const.Status.DELETED
                                            && cm.Status != (int)Const.Status.DELETED
                                            && ne.CreatedAt >= dateStart && ne.CreatedAt <= dateEnd
                                            && ((ne.TotalPrice != null && paging.CashStatus == true)
                                            || (ne.TotalPrice == null && paging.CashStatus != true)
                                            || paging.CashStatus == null)
                                            select ne
                                            ).AsQueryable();
                                }
                                else
                                {
                                    if (kk == 0)
                                        paging.query = item;
                                    else
                                        paging.query += " AND " + item;
                                }
                                kk++;
                            }
                        }
                        //else
                        //{
                        data = data.Where(paging.query);
                        //}


                        MetaDataDT metaDataDT = new MetaDataDT();
                        metaDataDT.Sum = data.Count();
                        metaDataDT.Normal = data.Where(e => e.Status == 1).Count();
                        metaDataDT.Temp = data.Where(e => e.Status == 10).Count();
                        metaDataDT.New = data.Where(e => e.Status == 11).Count();
                        metaDataDT.ReNew = data.Where(e => e.Status == 12).Count();
                        metaDataDT.Editing = data.Where(e => e.Status == 13).Count();
                        metaDataDT.Edited = data.Where(e => e.Status == 14).Count();
                        metaDataDT.ReEdited = data.Where(e => e.Status == 15).Count();
                        metaDataDT.Approving = data.Where(e => e.Status == 16).Count();
                        metaDataDT.NotApproved = data.Where(e => e.Status == 17).Count();
                        metaDataDT.Publishing = data.Where(e => e.Status == 18).Count();
                        metaDataDT.UnPublish = data.Where(e => e.Status == 19).Count();

                        def.metadata = metaDataDT;

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
                            def.data = await data.Select(paging.select).ToDynamicListAsync();
                        }
                        else
                        {
                            def.data = await data.Select(e => new
                            {
                                e.NewsId,
                                e.Title,
                                e.Description,
                                e.Contents,
                                e.Image,
                                e.Url,
                                e.Note,
                                e.DateStartActive,
                                e.DateStartOn,
                                e.DateEndOn,
                                e.IsHome,
                                e.IsHot,
                                e.IsAttach,
                                e.FactorPrice,
                                e.ValuePrice,
                                e.TotalPrice,
                                e.IsCash,
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
                                e.NumberWord,
                                e.CreatedAt,
                                e.AuthorId,
                                e.UserCreatedId,
                                e.UserEditedId,
                                e.EditingAt,
                                e.EditedAt,
                                e.UserApprovedId,
                                e.ApprovingAt,
                                e.ApprovedAt,
                                e.UserPublishedId,
                                e.PublishingAt,
                                e.PublishedAt,
                                e.UpdatedAt,
                                e.UserId,
                                e.Status,
                                e.YearTimeline,
                                e.LinkVideo,
                                e.Author,
                                e.IsComment,
                                e.IsFirst,
                                e.IsShowView,
                                listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.NewsId && cp.CategoryId != -1
                                    && cp.Status != (int)Const.Status.DELETED).Select(p => new
                                    {
                                        p.CategoryId,
                                        Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
                                        Check = true
                                    }).ToList(),
                                listTag = db.TagMapping.Where(tm => tm.TargetId == e.NewsId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).Select(p => new
                                {
                                    p.TagId,
                                    Name = db.Tag.Where(t => t.TagId == p.TagId).FirstOrDefault().Name
                                }).ToList(),
                                listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
                                listRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new
                                {
                                    r.TargetRelatedId
                                }).ToList(),
                                listProductRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new
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
                                listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.NewsId || a.TargetId2 == e.NewsId)
                                && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new
                                {
                                    lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new
                                    {
                                        l.LanguageId,
                                        l.Name,
                                        l.Flag
                                    }).FirstOrDefault(),
                                    news = db.News.Where(l => (l.NewsId == a.TargetId1 || l.NewsId == a.TargetId2) && l.NewsId != e.NewsId).Select(l => new
                                    {
                                        l.NewsId,
                                        l.Title,
                                        l.Url
                                    }).FirstOrDefault(),
                                }).ToList(),
                                AuthorName = db.User.Where(u => u.UserId == e.UserCreatedId).Select(u => u.FullName).FirstOrDefault(),
                                EditName = db.User.Where(u => u.UserId == e.UserEditedId).Select(u => u.FullName).FirstOrDefault(),
                                ApproveName = db.User.Where(u => u.UserId == e.UserApprovedId).Select(u => u.FullName).FirstOrDefault(),
                                PublicName = db.User.Where(u => u.UserId == e.UserPublishedId).Select(u => u.FullName).FirstOrDefault(),
                                CreateName = db.Author.Where(u => u.AuthorId == e.AuthorId).Select(u => u.Name).FirstOrDefault(),

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

        //// GET: api/News
        //[HttpGet("GetByPageUser")]
        //public async Task<IActionResult> GetByPageUser([FromQuery] FilteredPagination paging)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    //check role
        //    var identity = (ClaimsIdentity)User.Identity;
        //    int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
        //    int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
        //    string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
        //    if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
        //    {
        //        def.meta = new Meta(222, "No permission");
        //        return Ok(def);
        //    }
        //    if (paging != null)
        //    {
        //        try
        //        {
        //            using (var db = new IOITDataContext())
        //            {
        //                string cat = "CategoryId";
        //                def.meta = new Meta(200, "Success");
        //                IQueryable<News> data = db.News.Where(c => c.Status != (int)Const.Status.DELETED && c.UserId == userId);

        //                if (paging.query != null)
        //                {
        //                    paging.query = HttpUtility.UrlDecode(paging.query);
        //                }
        //                data = data.Where(paging.query);

        //                MetaDataDT metaDataDT = new MetaDataDT();
        //                metaDataDT.Sum = data.Count();
        //                metaDataDT.isNormal = data.Where(e => e.Status == 1).Count();
        //                metaDataDT.isPending = data.Where(e => e.Status == 11).Count();
        //                metaDataDT.isRatify = data.Where(e => e.Status == 12).Count();
        //                metaDataDT.isDarft = data.Where(e => e.Status == 13).Count();


        //                def.metadata = metaDataDT;

        //                if (paging.page_size > 0)
        //                {
        //                    if (paging.order_by != null)
        //                    {
        //                        data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                    }
        //                    else
        //                    {
        //                        data = data.OrderBy("NewsId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                    }
        //                }
        //                else
        //                {
        //                    if (paging.order_by != null)
        //                    {
        //                        data = data.OrderBy(paging.order_by);
        //                    }
        //                    else
        //                    {
        //                        data = data.OrderBy("NewsId desc");
        //                    }
        //                }

        //                if (paging.select != null && paging.select != "")
        //                {
        //                    paging.select = "new(" + paging.select + ")";
        //                    paging.select = HttpUtility.UrlDecode(paging.select);
        //                    def.data = await data.Select(paging.select).ToDynamicListAsync();
        //                }
        //                else
        //                {
        //                    def.data = await data.Select(e => new
        //                    {
        //                        e.NewsId,
        //                        e.Title,
        //                        e.Description,
        //                        e.Contents,
        //                        e.Image,
        //                        e.Url,
        //                        e.DateStartActive,
        //                        e.DateStartOn,
        //                        e.DateEndOn,
        //                        e.IsHome,
        //                        e.IsHot,
        //                        e.IsAttach,
        //                        e.LanguageId,
        //                        e.CompanyId,
        //                        e.WebsiteId,
        //                        e.Introduce,
        //                        e.SystemDiagram,
        //                        e.ViewNumber,
        //                        e.Location,
        //                        e.TypeNewsId,
        //                        e.MetaTitle,
        //                        e.MetaKeyword,
        //                        e.MetaDescription,
        //                        e.CreatedAt,
        //                        e.UpdatedAt,
        //                        e.UserId,
        //                        e.Status,
        //                        e.YearTimeline,
        //                        e.LinkVideo,
        //                        e.Author,
        //                        listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.NewsId && cp.CategoryId != -1
        //                            && cp.Status != (int)Const.Status.DELETED).Select(p => new
        //                            {
        //                                p.CategoryId,
        //                                Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
        //                                Check = true
        //                            }).ToList(),
        //                        listTag = db.TagMapping.Where(tm => tm.TargetId == e.NewsId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).Select(p => new
        //                        {
        //                            p.TagId,
        //                            Name = db.Tag.Where(t => t.TagId == p.TagId).FirstOrDefault().Name
        //                        }).ToList(),
        //                        listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
        //                        listRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new
        //                        {
        //                            r.TargetRelatedId
        //                        }).ToList(),
        //                        listProductRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new
        //                        {
        //                            r.TargetRelatedId
        //                        }).ToList(),
        //                        language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new
        //                        {
        //                            l.LanguageId,
        //                            l.Flag,
        //                            l.Name,
        //                            l.Code
        //                        }).FirstOrDefault(),
        //                        listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.NewsId || a.TargetId2 == e.NewsId)
        //                        && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new
        //                        {
        //                            lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new
        //                            {
        //                                l.LanguageId,
        //                                l.Name,
        //                                l.Flag
        //                            }).FirstOrDefault(),
        //                            news = db.News.Where(l => (l.NewsId == a.TargetId1 || l.NewsId == a.TargetId2) && l.NewsId != e.NewsId).Select(l => new
        //                            {
        //                                l.NewsId,
        //                                l.Title,
        //                                l.Url
        //                            }).FirstOrDefault(),
        //                        }).ToList(),
        //                        AuthorName = db.User.Where(u => u.UserId == e.UserId && u.Status != (int)Const.Status.DELETED).FirstOrDefault().FullName,
        //                    }).ToListAsync();
        //                }

        //                return Ok(def);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            log.Error("Exception:" + e);
        //            def.meta = new Meta(400, "Bad Request");
        //            return Ok(def);
        //        }
        //    }
        //    else
        //    {
        //        def.meta = new Meta(400, "Bad Request");
        //        return Ok(def);
        //    }
        //}

        //// GET: api/News/GetByPageRatify
        //[HttpGet("GetByPageDraft")]
        //public async Task<IActionResult> GetByPageDraft([FromQuery] FilteredPagination paging)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    //check role
        //    var identity = (ClaimsIdentity)User.Identity;
        //    int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
        //    int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
        //    string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
        //    if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
        //    {
        //        def.meta = new Meta(222, "No permission");
        //        return Ok(def);
        //    }
        //    if (paging != null)
        //    {
        //        try
        //        {
        //            using (var db = new IOITDataContext())
        //            {
        //                string cat = "CategoryId";
        //                def.meta = new Meta(200, "Success");
        //                var UserRole = db.UserRole.Where(e => e.Status != (int)Const.Status.DELETED && e.UserId == userId).FirstOrDefault();
        //                var CodeRoleId = UserRole.RoleId;
        //                var role = db.Role.Find(CodeRoleId);
        //                if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "TKTC" || role.Code.Trim() == "BTV")
        //                {
        //                    IQueryable<News> data = db.News.Where(c => c.Status == (int)Const.Status.DRAFT);
        //                    if (paging.query != null)
        //                    {
        //                        paging.query = HttpUtility.UrlDecode(paging.query);
        //                    }
        //                    data = data.Where(paging.query);

        //                    MetaDataDT metaDataDT = new MetaDataDT();
        //                    metaDataDT.Sum = data.Count();
        //                    metaDataDT.isNormal = data.Where(e => e.Status == 1).Count();
        //                    metaDataDT.isPending = data.Where(e => e.Status == 11).Count();
        //                    metaDataDT.isRatify = data.Where(e => e.Status == 12).Count();
        //                    metaDataDT.isDarft = data.Where(e => e.Status == 13).Count();

        //                    def.metadata = metaDataDT;

        //                    if (paging.page_size > 0)
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc");
        //                        }
        //                    }

        //                    if (paging.select != null && paging.select != "")
        //                    {
        //                        paging.select = "new(" + paging.select + ")";
        //                        paging.select = HttpUtility.UrlDecode(paging.select);
        //                        def.data = await data.Select(paging.select).ToDynamicListAsync();
        //                    }
        //                    else
        //                    {
        //                        def.data = await data.Select(e => new
        //                        {
        //                            e.NewsId,
        //                            e.Title,
        //                            e.Description,
        //                            e.Contents,
        //                            e.Image,
        //                            e.Url,
        //                            e.DateStartActive,
        //                            e.DateStartOn,
        //                            e.DateEndOn,
        //                            e.IsHome,
        //                            e.IsHot,
        //                            e.IsAttach,
        //                            e.LanguageId,
        //                            e.CompanyId,
        //                            e.WebsiteId,
        //                            e.Introduce,
        //                            e.SystemDiagram,
        //                            e.ViewNumber,
        //                            e.Location,
        //                            e.TypeNewsId,
        //                            e.MetaTitle,
        //                            e.MetaKeyword,
        //                            e.MetaDescription,
        //                            e.CreatedAt,
        //                            e.UpdatedAt,
        //                            e.UserId,
        //                            e.Status,
        //                            e.YearTimeline,
        //                            e.LinkVideo,
        //                            e.Author,
        //                            listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.NewsId && cp.CategoryId != -1
        //                                && cp.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                    p.CategoryId,
        //                                    Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
        //                                    Check = true
        //                                }).ToList(),
        //                            listTag = db.TagMapping.Where(tm => tm.TargetId == e.NewsId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                p.TagId,
        //                                Name = db.Tag.Where(t => t.TagId == p.TagId).FirstOrDefault().Name
        //                            }).ToList(),
        //                            listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
        //                            listRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            listProductRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new {
        //                                l.LanguageId,
        //                                l.Flag,
        //                                l.Name,
        //                                l.Code
        //                            }).FirstOrDefault(),
        //                            listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.NewsId || a.TargetId2 == e.NewsId)
        //                            && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new
        //                            {
        //                                lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new {
        //                                    l.LanguageId,
        //                                    l.Name,
        //                                    l.Flag
        //                                }).FirstOrDefault(),
        //                                news = db.News.Where(l => (l.NewsId == a.TargetId1 || l.NewsId == a.TargetId2) && l.NewsId != e.NewsId).Select(l => new {
        //                                    l.NewsId,
        //                                    l.Title,
        //                                    l.Url
        //                                }).FirstOrDefault(),
        //                            }).ToList(),
        //                            AuthorName = db.User.Where(u => u.UserId == e.UserId && u.Status != (int)Const.Status.DELETED).FirstOrDefault().FullName,
        //                        }).ToListAsync();
        //                    }
        //                }
        //                else
        //                {
        //                    IQueryable<News> data = db.News.Where(c => c.Status == (int)Const.Status.DRAFT && c.UserId == userId);
        //                    if (paging.query != null)
        //                    {
        //                        paging.query = HttpUtility.UrlDecode(paging.query);
        //                    }
        //                    data = data.Where(paging.query);

        //                    MetaDataDT metaDataDT = new MetaDataDT();
        //                    metaDataDT.Sum = data.Count();
        //                    metaDataDT.Sum = data.Count();
        //                    metaDataDT.isNormal = data.Where(e => e.Status == 1).Count();
        //                    metaDataDT.isPending = data.Where(e => e.Status == 11).Count();
        //                    metaDataDT.isRatify = data.Where(e => e.Status == 12).Count();
        //                    metaDataDT.isDarft = data.Where(e => e.Status == 13).Count();

        //                    def.metadata = metaDataDT;

        //                    if (paging.page_size > 0)
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc");
        //                        }
        //                    }

        //                    if (paging.select != null && paging.select != "")
        //                    {
        //                        paging.select = "new(" + paging.select + ")";
        //                        paging.select = HttpUtility.UrlDecode(paging.select);
        //                        def.data = await data.Select(paging.select).ToDynamicListAsync();
        //                    }
        //                    else
        //                    {
        //                        def.data = await data.Select(e => new
        //                        {
        //                            e.NewsId,
        //                            e.Title,
        //                            e.Description,
        //                            e.Contents,
        //                            e.Image,
        //                            e.Url,
        //                            e.DateStartActive,
        //                            e.DateStartOn,
        //                            e.DateEndOn,
        //                            e.IsHome,
        //                            e.IsHot,
        //                            e.IsAttach,
        //                            e.LanguageId,
        //                            e.CompanyId,
        //                            e.WebsiteId,
        //                            e.Introduce,
        //                            e.SystemDiagram,
        //                            e.ViewNumber,
        //                            e.Location,
        //                            e.TypeNewsId,
        //                            e.MetaTitle,
        //                            e.MetaKeyword,
        //                            e.MetaDescription,
        //                            e.CreatedAt,
        //                            e.UpdatedAt,
        //                            e.UserId,
        //                            e.Status,
        //                            e.YearTimeline,
        //                            e.LinkVideo,
        //                            e.Author,
        //                            listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.NewsId && cp.CategoryId != -1
        //                                && cp.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                    p.CategoryId,
        //                                    Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
        //                                    Check = true
        //                                }).ToList(),
        //                            listTag = db.TagMapping.Where(tm => tm.TargetId == e.NewsId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                p.TagId,
        //                                Name = db.Tag.Where(t => t.TagId == p.TagId).FirstOrDefault().Name
        //                            }).ToList(),
        //                            listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
        //                            listRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            listProductRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new {
        //                                l.LanguageId,
        //                                l.Flag,
        //                                l.Name,
        //                                l.Code
        //                            }).FirstOrDefault(),
        //                            listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.NewsId || a.TargetId2 == e.NewsId)
        //                            && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new
        //                            {
        //                                lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new {
        //                                    l.LanguageId,
        //                                    l.Name,
        //                                    l.Flag
        //                                }).FirstOrDefault(),
        //                                news = db.News.Where(l => (l.NewsId == a.TargetId1 || l.NewsId == a.TargetId2) && l.NewsId != e.NewsId).Select(l => new {
        //                                    l.NewsId,
        //                                    l.Title,
        //                                    l.Url
        //                                }).FirstOrDefault(),
        //                            }).ToList(),
        //                            AuthorName = db.User.Where(u => u.UserId == e.UserId && u.Status != (int)Const.Status.DELETED).FirstOrDefault().FullName,
        //                        }).ToListAsync();
        //                    }
        //                }
        //                return Ok(def);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            log.Error("Exception:" + e);
        //            def.meta = new Meta(400, "Bad Request");
        //            return Ok(def);
        //        }
        //    }
        //    else
        //    {
        //        def.meta = new Meta(400, "Bad Request");
        //        return Ok(def);
        //    }
        //}


        //// GET: api/News/GetByPageRatify
        //[HttpGet("GetByPageTrack")]
        //public async Task<IActionResult> GetByPageTrack([FromQuery] FilteredPagination paging)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    //check role
        //    var identity = (ClaimsIdentity)User.Identity;
        //    int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
        //    int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
        //    string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
        //    if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
        //    {
        //        def.meta = new Meta(222, "No permission");
        //        return Ok(def);
        //    }
        //    if (paging != null)
        //    {
        //        try
        //        {
        //            using (var db = new IOITDataContext())
        //            {
        //                string cat = "CategoryId";
        //                def.meta = new Meta(200, "Success");
        //                var UserRole = db.UserRole.Where(e => e.Status != (int)Const.Status.DELETED && e.UserId == userId).FirstOrDefault();
        //                var CodeRoleId = UserRole.RoleId;
        //                var role = db.Role.Find(CodeRoleId);
        //                if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "TKTC" || role.Code.Trim() == "BTV")
        //                {
        //                    IQueryable<News> data = db.News.Where(c => c.Status == (int)Const.Status.DELETED);
        //                    if (paging.query != null)
        //                    {
        //                        paging.query = HttpUtility.UrlDecode(paging.query);
        //                    }
        //                    data = data.Where(paging.query);

        //                    MetaDataDT metaDataDT = new MetaDataDT();
        //                    metaDataDT.Sum = data.Count();
        //                    metaDataDT.isNormal = data.Where(e => e.Status == 1).Count();
        //                    metaDataDT.isPending = data.Where(e => e.Status == 11).Count();
        //                    metaDataDT.isRatify = data.Where(e => e.Status == 12).Count();
        //                    metaDataDT.isDarft = data.Where(e => e.Status == 13).Count();

        //                    def.metadata = metaDataDT;

        //                    if (paging.page_size > 0)
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc");
        //                        }
        //                    }

        //                    if (paging.select != null && paging.select != "")
        //                    {
        //                        paging.select = "new(" + paging.select + ")";
        //                        paging.select = HttpUtility.UrlDecode(paging.select);
        //                        def.data = await data.Select(paging.select).ToDynamicListAsync();
        //                    }
        //                    else
        //                    {
        //                        def.data = await data.Select(e => new
        //                        {
        //                            e.NewsId,
        //                            e.Title,
        //                            e.Description,
        //                            e.Contents,
        //                            e.Image,
        //                            e.Url,
        //                            e.DateStartActive,
        //                            e.DateStartOn,
        //                            e.DateEndOn,
        //                            e.IsHome,
        //                            e.IsHot,
        //                            e.IsAttach,
        //                            e.LanguageId,
        //                            e.CompanyId,
        //                            e.WebsiteId,
        //                            e.Introduce,
        //                            e.SystemDiagram,
        //                            e.ViewNumber,
        //                            e.Location,
        //                            e.TypeNewsId,
        //                            e.MetaTitle,
        //                            e.MetaKeyword,
        //                            e.MetaDescription,
        //                            e.CreatedAt,
        //                            e.UpdatedAt,
        //                            e.UserId,
        //                            e.Status,
        //                            e.YearTimeline,
        //                            e.LinkVideo,
        //                            e.Author,
        //                            listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.NewsId && cp.CategoryId != -1
        //                                && cp.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                    p.CategoryId,
        //                                    Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
        //                                    Check = true
        //                                }).ToList(),
        //                            listTag = db.TagMapping.Where(tm => tm.TargetId == e.NewsId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                p.TagId,
        //                                Name = db.Tag.Where(t => t.TagId == p.TagId).FirstOrDefault().Name
        //                            }).ToList(),
        //                            listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
        //                            listRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            listProductRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new {
        //                                l.LanguageId,
        //                                l.Flag,
        //                                l.Name,
        //                                l.Code
        //                            }).FirstOrDefault(),
        //                            listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.NewsId || a.TargetId2 == e.NewsId)
        //                            && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new
        //                            {
        //                                lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new {
        //                                    l.LanguageId,
        //                                    l.Name,
        //                                    l.Flag
        //                                }).FirstOrDefault(),
        //                                news = db.News.Where(l => (l.NewsId == a.TargetId1 || l.NewsId == a.TargetId2) && l.NewsId != e.NewsId).Select(l => new {
        //                                    l.NewsId,
        //                                    l.Title,
        //                                    l.Url
        //                                }).FirstOrDefault(),
        //                            }).ToList(),
        //                            AuthorName = db.User.Where(u => u.UserId == e.UserId && u.Status != (int)Const.Status.DELETED).FirstOrDefault().FullName,
        //                        }).ToListAsync();
        //                    }
        //                }                      
        //                return Ok(def);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            log.Error("Exception:" + e);
        //            def.meta = new Meta(400, "Bad Request");
        //            return Ok(def);
        //        }
        //    }
        //    else
        //    {
        //        def.meta = new Meta(400, "Bad Request");
        //        return Ok(def);
        //    }
        //}

        //// GET: api/News/GetByPageRatify
        //[HttpGet("GetByPagePending")]
        //public async Task<IActionResult> GetByPagePending([FromQuery] FilteredPagination paging)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    //check role
        //    var identity = (ClaimsIdentity)User.Identity;
        //    int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
        //    int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
        //    string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
        //    if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
        //    {
        //        def.meta = new Meta(222, "No permission");
        //        return Ok(def);
        //    }
        //    if (paging != null)
        //    {
        //        try
        //        {
        //            using (var db = new IOITDataContext())
        //            {
        //                string cat = "CategoryId";
        //                def.meta = new Meta(200, "Success");
        //                var UserRole = db.UserRole.Where(e => e.UserId == userId && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
        //                var CodeRoleId = UserRole.RoleId;
        //                var role = db.Role.Find(CodeRoleId);
        //                if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "TKTC" || role.Code.Trim() == "BTV")
        //                {
        //                    IQueryable<News> data = db.News.Where(c => c.Status == (int)Const.Status.PENDING);
        //                    if (paging.query != null)
        //                    {
        //                        paging.query = HttpUtility.UrlDecode(paging.query);
        //                    }
        //                    data = data.Where(paging.query);

        //                    MetaDataDT metaDataDT = new MetaDataDT();
        //                    metaDataDT.Sum = data.Count();
        //                    metaDataDT.isNormal = data.Where(e => e.Status == 1).Count();
        //                    metaDataDT.isPending = data.Where(e => e.Status == 11).Count();
        //                    metaDataDT.isRatify = data.Where(e => e.Status == 12).Count();
        //                    metaDataDT.isDarft = data.Where(e => e.Status == 13).Count();

        //                    def.metadata = metaDataDT;

        //                    if (paging.page_size > 0)
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc");
        //                        }
        //                    }

        //                    if (paging.select != null && paging.select != "")
        //                    {
        //                        paging.select = "new(" + paging.select + ")";
        //                        paging.select = HttpUtility.UrlDecode(paging.select);
        //                        def.data = await data.Select(paging.select).ToDynamicListAsync();
        //                    }
        //                    else
        //                    {
        //                        def.data = await data.Select(e => new
        //                        {
        //                            e.NewsId,
        //                            e.Title,
        //                            e.Description,
        //                            e.Contents,
        //                            e.Image,
        //                            e.Url,
        //                            e.DateStartActive,
        //                            e.DateStartOn,
        //                            e.DateEndOn,
        //                            e.IsHome,
        //                            e.IsHot,
        //                            e.IsAttach,
        //                            e.LanguageId,
        //                            e.CompanyId,
        //                            e.WebsiteId,
        //                            e.Introduce,
        //                            e.SystemDiagram,
        //                            e.ViewNumber,
        //                            e.Location,
        //                            e.TypeNewsId,
        //                            e.MetaTitle,
        //                            e.MetaKeyword,
        //                            e.MetaDescription,
        //                            e.CreatedAt,
        //                            e.UpdatedAt,
        //                            e.UserId,
        //                            e.Status,
        //                            e.YearTimeline,
        //                            e.LinkVideo,
        //                            e.Author,
        //                            listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.NewsId && cp.CategoryId != -1
        //                                && cp.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                    p.CategoryId,
        //                                    Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
        //                                    Check = true
        //                                }).ToList(),
        //                            listTag = db.TagMapping.Where(tm => tm.TargetId == e.NewsId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                p.TagId,
        //                                Name = db.Tag.Where(t => t.TagId == p.TagId).FirstOrDefault().Name
        //                            }).ToList(),
        //                            listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
        //                            listRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            listProductRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new {
        //                                l.LanguageId,
        //                                l.Flag,
        //                                l.Name,
        //                                l.Code
        //                            }).FirstOrDefault(),
        //                            listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.NewsId || a.TargetId2 == e.NewsId)
        //                            && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new
        //                            {
        //                                lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new {
        //                                    l.LanguageId,
        //                                    l.Name,
        //                                    l.Flag
        //                                }).FirstOrDefault(),
        //                                news = db.News.Where(l => (l.NewsId == a.TargetId1 || l.NewsId == a.TargetId2) && l.NewsId != e.NewsId).Select(l => new {
        //                                    l.NewsId,
        //                                    l.Title,
        //                                    l.Url
        //                                }).FirstOrDefault(),
        //                            }).ToList(),
        //                            AuthorName = db.User.Where(u => u.UserId == e.UserId && u.Status != (int)Const.Status.DELETED).FirstOrDefault().FullName,
        //                        }).ToListAsync();
        //                    }
        //                }
        //                else
        //                {
        //                    IQueryable<News> data = db.News.Where(c => c.Status == (int)Const.Status.PENDING && c.UserId == userId);
        //                    if (paging.query != null)
        //                    {
        //                        paging.query = HttpUtility.UrlDecode(paging.query);
        //                    }
        //                    data = data.Where(paging.query);

        //                    MetaDataDT metaDataDT = new MetaDataDT();
        //                    metaDataDT.Sum = data.Count();
        //                    metaDataDT.Sum = data.Count();
        //                    metaDataDT.isNormal = data.Where(e => e.Status == 1).Count();
        //                    metaDataDT.isPending = data.Where(e => e.Status == 11).Count();
        //                    metaDataDT.isRatify = data.Where(e => e.Status == 12).Count();
        //                    metaDataDT.isDarft = data.Where(e => e.Status == 13).Count();

        //                    def.metadata = metaDataDT;

        //                    if (paging.page_size > 0)
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc");
        //                        }
        //                    }

        //                    if (paging.select != null && paging.select != "")
        //                    {
        //                        paging.select = "new(" + paging.select + ")";
        //                        paging.select = HttpUtility.UrlDecode(paging.select);
        //                        def.data = await data.Select(paging.select).ToDynamicListAsync();
        //                    }
        //                    else
        //                    {
        //                        def.data = await data.Select(e => new
        //                        {
        //                            e.NewsId,
        //                            e.Title,
        //                            e.Description,
        //                            e.Contents,
        //                            e.Image,
        //                            e.Url,
        //                            e.DateStartActive,
        //                            e.DateStartOn,
        //                            e.DateEndOn,
        //                            e.IsHome,
        //                            e.IsHot,
        //                            e.IsAttach,
        //                            e.LanguageId,
        //                            e.CompanyId,
        //                            e.WebsiteId,
        //                            e.Introduce,
        //                            e.SystemDiagram,
        //                            e.ViewNumber,
        //                            e.Location,
        //                            e.TypeNewsId,
        //                            e.MetaTitle,
        //                            e.MetaKeyword,
        //                            e.MetaDescription,
        //                            e.CreatedAt,
        //                            e.UpdatedAt,
        //                            e.UserId,
        //                            e.Status,
        //                            e.YearTimeline,
        //                            e.LinkVideo,
        //                            e.Author,
        //                            listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.NewsId && cp.CategoryId != -1
        //                                && cp.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                    p.CategoryId,
        //                                    Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
        //                                    Check = true
        //                                }).ToList(),
        //                            listTag = db.TagMapping.Where(tm => tm.TargetId == e.NewsId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                p.TagId,
        //                                Name = db.Tag.Where(t => t.TagId == p.TagId).FirstOrDefault().Name
        //                            }).ToList(),
        //                            listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
        //                            listRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            listProductRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new {
        //                                l.LanguageId,
        //                                l.Flag,
        //                                l.Name,
        //                                l.Code
        //                            }).FirstOrDefault(),
        //                            listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.NewsId || a.TargetId2 == e.NewsId)
        //                            && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new
        //                            {
        //                                lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new {
        //                                    l.LanguageId,
        //                                    l.Name,
        //                                    l.Flag
        //                                }).FirstOrDefault(),
        //                                news = db.News.Where(l => (l.NewsId == a.TargetId1 || l.NewsId == a.TargetId2) && l.NewsId != e.NewsId).Select(l => new {
        //                                    l.NewsId,
        //                                    l.Title,
        //                                    l.Url
        //                                }).FirstOrDefault(),
        //                            }).ToList(),
        //                            AuthorName = db.User.Where(u => u.UserId == e.UserId && u.Status != (int)Const.Status.DELETED).FirstOrDefault().FullName,
        //                        }).ToListAsync();
        //                    }
        //                }              
        //                return Ok(def);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            log.Error("Exception:" + e);
        //            def.meta = new Meta(400, "Bad Request");
        //            return Ok(def);
        //        }
        //    }
        //    else
        //    {
        //        def.meta = new Meta(400, "Bad Request");
        //        return Ok(def);
        //    }
        //}


        //// GET: api/News/GetByPageRatify
        //[HttpGet("GetByPageBrowsing")]
        //public async Task<IActionResult> GetByPageBrowsing([FromQuery] FilteredPagination paging)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    //check role
        //    var identity = (ClaimsIdentity)User.Identity;
        //    int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
        //    int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
        //    string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
        //    if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
        //    {
        //        def.meta = new Meta(222, "No permission");
        //        return Ok(def);
        //    }
        //    if (paging != null)
        //    {
        //        try
        //        {
        //            using (var db = new IOITDataContext())
        //            {
        //                string cat = "CategoryId";
        //                def.meta = new Meta(200, "Success");
        //                var UserRole = db.UserRole.Where(e => e.UserId == userId && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
        //                var CodeRoleId = UserRole.RoleId;
        //                var role = db.Role.Find(CodeRoleId);
        //                if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "TKTC" || role.Code.Trim() == "BTV")
        //                {
        //                    IQueryable<News> data = db.News.Where(c => c.Status == (int)Const.Status.RATIFY);
        //                    if (paging.query != null)
        //                    {
        //                        paging.query = HttpUtility.UrlDecode(paging.query);
        //                    }
        //                    data = data.Where(paging.query);

        //                    MetaDataDT metaDataDT = new MetaDataDT();
        //                    metaDataDT.Sum = data.Count();
        //                    metaDataDT.isNormal = data.Where(e => e.Status == 1).Count();
        //                    metaDataDT.isPending = data.Where(e => e.Status == 11).Count();
        //                    metaDataDT.isRatify = data.Where(e => e.Status == 12).Count();
        //                    metaDataDT.isDarft = data.Where(e => e.Status == 13).Count();

        //                    def.metadata = metaDataDT;

        //                    if (paging.page_size > 0)
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc");
        //                        }
        //                    }

        //                    if (paging.select != null && paging.select != "")
        //                    {
        //                        paging.select = "new(" + paging.select + ")";
        //                        paging.select = HttpUtility.UrlDecode(paging.select);
        //                        def.data = await data.Select(paging.select).ToDynamicListAsync();
        //                    }
        //                    else
        //                    {
        //                        def.data = await data.Select(e => new
        //                        {
        //                            e.NewsId,
        //                            e.Title,
        //                            e.Description,
        //                            e.Contents,
        //                            e.Image,
        //                            e.Url,
        //                            e.DateStartActive,
        //                            e.DateStartOn,
        //                            e.DateEndOn,
        //                            e.IsHome,
        //                            e.IsHot,
        //                            e.IsAttach,
        //                            e.LanguageId,
        //                            e.CompanyId,
        //                            e.WebsiteId,
        //                            e.Introduce,
        //                            e.SystemDiagram,
        //                            e.ViewNumber,
        //                            e.Location,
        //                            e.TypeNewsId,
        //                            e.MetaTitle,
        //                            e.MetaKeyword,
        //                            e.MetaDescription,
        //                            e.CreatedAt,
        //                            e.UpdatedAt,
        //                            e.UserId,
        //                            e.Status,
        //                            e.YearTimeline,
        //                            e.LinkVideo,
        //                            e.Author,
        //                            listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.NewsId && cp.CategoryId != -1
        //                                && cp.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                    p.CategoryId,
        //                                    Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
        //                                    Check = true
        //                                }).ToList(),
        //                            listTag = db.TagMapping.Where(tm => tm.TargetId == e.NewsId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                p.TagId,
        //                                Name = db.Tag.Where(t => t.TagId == p.TagId).FirstOrDefault().Name
        //                            }).ToList(),
        //                            listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
        //                            listRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            listProductRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new {
        //                                l.LanguageId,
        //                                l.Flag,
        //                                l.Name,
        //                                l.Code
        //                            }).FirstOrDefault(),
        //                            listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.NewsId || a.TargetId2 == e.NewsId)
        //                            && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new
        //                            {
        //                                lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new {
        //                                    l.LanguageId,
        //                                    l.Name,
        //                                    l.Flag
        //                                }).FirstOrDefault(),
        //                                news = db.News.Where(l => (l.NewsId == a.TargetId1 || l.NewsId == a.TargetId2) && l.NewsId != e.NewsId).Select(l => new {
        //                                    l.NewsId,
        //                                    l.Title,
        //                                    l.Url
        //                                }).FirstOrDefault(),
        //                            }).ToList(),
        //                            AuthorName = db.User.Where(u => u.UserId == e.UserId && u.Status != (int)Const.Status.DELETED).FirstOrDefault().FullName,
        //                        }).ToListAsync();
        //                    }
        //                }
        //                else
        //                {
        //                    IQueryable<News> data = db.News.Where(c => c.Status == (int)Const.Status.RATIFY && c.UserId == userId);
        //                    if (paging.query != null)
        //                    {
        //                        paging.query = HttpUtility.UrlDecode(paging.query);
        //                    }
        //                    data = data.Where(paging.query);

        //                    MetaDataDT metaDataDT = new MetaDataDT();
        //                    metaDataDT.Sum = data.Count();
        //                    metaDataDT.isNormal = data.Where(e => e.Status == 1).Count();
        //                    metaDataDT.isPending = data.Where(e => e.Status == 11).Count();
        //                    metaDataDT.isRatify = data.Where(e => e.Status == 12).Count();
        //                    metaDataDT.isDarft = data.Where(e => e.Status == 13).Count();

        //                    def.metadata = metaDataDT;

        //                    if (paging.page_size > 0)
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (paging.order_by != null)
        //                        {
        //                            data = data.OrderBy(paging.order_by);
        //                        }
        //                        else
        //                        {
        //                            data = data.OrderBy("NewsId desc");
        //                        }
        //                    }

        //                    if (paging.select != null && paging.select != "")
        //                    {
        //                        paging.select = "new(" + paging.select + ")";
        //                        paging.select = HttpUtility.UrlDecode(paging.select);
        //                        def.data = await data.Select(paging.select).ToDynamicListAsync();
        //                    }
        //                    else
        //                    {
        //                        def.data = await data.Select(e => new
        //                        {
        //                            e.NewsId,
        //                            e.Title,
        //                            e.Description,
        //                            e.Contents,
        //                            e.Image,
        //                            e.Url,
        //                            e.DateStartActive,
        //                            e.DateStartOn,
        //                            e.DateEndOn,
        //                            e.IsHome,
        //                            e.IsHot,
        //                            e.IsAttach,
        //                            e.LanguageId,
        //                            e.CompanyId,
        //                            e.WebsiteId,
        //                            e.Introduce,
        //                            e.SystemDiagram,
        //                            e.ViewNumber,
        //                            e.Location,
        //                            e.TypeNewsId,
        //                            e.MetaTitle,
        //                            e.MetaKeyword,
        //                            e.MetaDescription,
        //                            e.CreatedAt,
        //                            e.UpdatedAt,
        //                            e.UserId,
        //                            e.Status,
        //                            e.YearTimeline,
        //                            e.LinkVideo,
        //                            e.Author,
        //                            listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.NewsId && cp.CategoryId != -1
        //                                && cp.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                    p.CategoryId,
        //                                    Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
        //                                    Check = true
        //                                }).ToList(),
        //                            listTag = db.TagMapping.Where(tm => tm.TargetId == e.NewsId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).Select(p => new {
        //                                p.TagId,
        //                                Name = db.Tag.Where(t => t.TagId == p.TagId).FirstOrDefault().Name
        //                            }).ToList(),
        //                            listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
        //                            listRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            listProductRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new {
        //                                r.TargetRelatedId
        //                            }).ToList(),
        //                            language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new {
        //                                l.LanguageId,
        //                                l.Flag,
        //                                l.Name,
        //                                l.Code
        //                            }).FirstOrDefault(),
        //                            listLanguage = db.LanguageMapping.Where(a => (a.TargetId1 == e.NewsId || a.TargetId2 == e.NewsId)
        //                            && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new
        //                            {
        //                                lang = db.Language.Where(l => (l.LanguageId == a.LanguageId1 || l.LanguageId == a.LanguageId2) && l.LanguageId != e.LanguageId).Select(l => new {
        //                                    l.LanguageId,
        //                                    l.Name,
        //                                    l.Flag
        //                                }).FirstOrDefault(),
        //                                news = db.News.Where(l => (l.NewsId == a.TargetId1 || l.NewsId == a.TargetId2) && l.NewsId != e.NewsId).Select(l => new {
        //                                    l.NewsId,
        //                                    l.Title,
        //                                    l.Url
        //                                }).FirstOrDefault(),
        //                            }).ToList(),
        //                            AuthorName = db.User.Where(u => u.UserId == e.UserId && u.Status != (int)Const.Status.DELETED).FirstOrDefault().FullName,
        //                        }).ToListAsync();
        //                    }
        //                }
        //                return Ok(def);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            log.Error("Exception:" + e);
        //            def.meta = new Meta(400, "Bad Request");
        //            return Ok(def);
        //        }
        //    }
        //    else
        //    {
        //        def.meta = new Meta(400, "Bad Request");
        //        return Ok(def);
        //    }
        //}


        // GET: api/News/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNews(int id)
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
                    var data = await db.News.Where(e => e.NewsId == id).Select(e => new
                    {
                        e.NewsId,
                        e.Title,
                        e.Description,
                        e.Contents,
                        e.Image,
                        e.Url,
                        e.Note,
                        e.DateStartActive,
                        e.DateStartOn,
                        e.DateEndOn,
                        e.IsHome,
                        e.IsHot,
                        e.IsAttach,
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
                        e.IsComment,
                        e.IsFirst,
                        e.IsShowView,
                        listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.NewsId
                            && cp.Status != (int)Const.Status.DELETED).Select(p => new
                            {
                                p.CategoryId,
                                Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
                                Check = true
                            }).ToList(),
                        listTag = db.Tag.Where(t => t.TargetId == e.NewsId && t.Status != (int)Const.Status.DELETED).Select(p => new
                        {
                            p.TagId,
                            p.Name,
                            Check = true
                        }).ToList(),
                        listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
                        listRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new
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

        // GET: api/News/GetAuthor
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
                    var data = await db.User.Where(e => e.Status != (int)Const.Status.DELETED).Select(e => new
                    {
                        e.UserId,
                        e.FullName,
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
        // PUT: api/News/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNews(int id, [FromBody] NewsDTO data)
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
                        News news = await db.News.FindAsync(id);
                        if (news == null)
                        {
                            def.meta = new Meta(404, "Not found!");
                            return Ok(def);
                        }

                        string url = data.Url == null ? Utils.NonUnicode(data.Title) : data.Url;
                        url = url.Trim().ToLower();
                        if (news.Url != url)
                        {
                            //check xem trùng link ko
                            var pLink = db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (pLink != null)
                            {
                                def.meta = new Meta(232, "Link đã tồn tại!");
                                return Ok(def);
                            }
                            //cập nhật thay link cũ
                            var permaLink = db.PermaLink.Where(e => e.Slug == news.Url && e.TargetId == news.NewsId
                            && e.TargetType == (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS).FirstOrDefault();
                            if (permaLink != null)
                            {
                                permaLink.TargetId = news.NewsId;
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
                                permaLink1.Slug = news.Url;
                                permaLink1.TargetId = news.NewsId;
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
                            var permaLink = db.PermaLink.Where(e => e.Slug == news.Url && e.TargetId == news.NewsId
                            && e.TargetType == (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS).FirstOrDefault();
                            if (permaLink == null)
                            {
                                PermaLink permaLink1 = new PermaLink();
                                permaLink1.PermaLinkId = Guid.NewGuid();
                                permaLink1.Slug = news.Url;
                                permaLink1.TargetId = news.NewsId;
                                permaLink1.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS;
                                permaLink1.CreatedAt = DateTime.Now;
                                permaLink1.UpdatedAt = DateTime.Now;
                                permaLink1.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink1);
                                await db.SaveChangesAsync();
                            }
                        }

                        news.Title = data.Title;
                        news.Description = data.Description;
                        news.Contents = data.Contents;
                        news.Image = data.Image;
                        news.Url = data.Url;
                        news.Note = data.Note;
                        news.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        news.DateStartOn = data.DateStartOn != null ? data.DateStartOn : news.DateStartActive;
                        news.DateEndOn = data.DateEndOn != null ? data.DateEndOn : news.DateStartActive.Value.AddYears(100);
                        news.IsHome = data.IsHome != null ? data.IsHome : false;
                        news.IsHot = data.IsHot != null ? data.IsHot : false;
                        news.IsComment = data.IsComment != null ? data.IsComment : false;
                        news.IsFirst = data.IsFirst != null ? data.IsFirst : false;
                        news.IsShowView = data.IsShowView != null ? data.IsShowView : false;
                        news.IsAttach = data.IsAttach != null ? data.IsAttach : false;
                        news.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                        news.Location = data.Location;
                        news.TypeNewsId = data.TypeNewsId;
                        news.FactorPrice = data.FactorPrice;
                        news.ValuePrice = data.ValuePrice;
                        news.TotalPrice = data.TotalPrice;
                        news.IsCash = data.IsCash;
                        if (news.Contents != null)
                        {
                            string[] listWord = news.Contents.Split(' ');
                            news.NumberWord = listWord.Count();
                            if (news.NumberWord < 500)
                                news.NumberPage = 1;
                            else
                            {
                                float page = (float)news.NumberWord / (float)500;
                                int page1 = (int)Math.Floor(page);
                                float page2 = page - page1;
                                if (page2 >= 0 && page2 <= 0.2)
                                    page = page1;
                                else if (page2 > 0.2 && page2 <= 0.7)
                                    page = page1 + (float)0.5;
                                else
                                    page = page1 + (float)1;
                                news.NumberPage = page;
                            }
                            news.NumberImage = CountImage(news.Contents);
                        }
                        news.MetaTitle = data.MetaTitle;
                        news.MetaKeyword = data.MetaKeyword;
                        news.MetaDescription = data.MetaDescription;
                        news.UpdatedAt = DateTime.Now;
                        news.UserId = userId;
                        news.Status = data.Status;
                        news.LinkVideo = data.LinkVideo;
                        news.Introduce = data.Introduce;
                        news.SystemDiagram = data.SystemDiagram;
                        news.Author = data.Author;
                        news.AuthorId = data.AuthorId;
                        news.YearTimeline = data.YearTimeline;

                        if ((news.Status == (int)Const.NewsStatus.EDITING
                            || news.Status == (int)Const.NewsStatus.EDITED
                            || news.Status == (int)Const.NewsStatus.RE_EDITED)
                            && news.EditingAt != null)
                        {
                            news.UserEditedId = userId;
                            news.EditedAt = DateTime.Now;
                        }
                        else if ((news.Status == (int)Const.NewsStatus.APPROVING
                            || news.Status == (int)Const.NewsStatus.PUBLISHING
                            || news.Status == (int)Const.NewsStatus.NOT_APPROVED)
                            && news.ApprovingAt != null)
                        {
                            news.UserApprovedId = userId;
                            news.ApprovedAt = DateTime.Now;
                        }
                        else if ((news.Status == (int)Const.NewsStatus.NORMAL
                            || news.Status == (int)Const.NewsStatus.PUBLISHING
                            || news.Status == (int)Const.NewsStatus.UN_PUBLISH)
                            && news.PublishingAt != null)
                        {
                            news.UserPublishedId = userId;
                            news.PublishedAt = DateTime.Now;
                        }
                        if (news.Status == (int)Const.NewsStatus.EDITING && news.EditingAt == null)
                        {
                            news.EditingAt = DateTime.Now;
                        }
                        if (news.Status == (int)Const.NewsStatus.APPROVING && news.ApprovingAt == null)
                        {
                            news.ApprovingAt = DateTime.Now;
                        }
                        if (news.Status == (int)Const.NewsStatus.PUBLISHING && news.PublishingAt == null)
                        {
                            news.PublishingAt = DateTime.Now;
                        }

                        db.Entry(news).State = EntityState.Modified;

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
                                        categoryNewsMapping.TargetId = news.NewsId;
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
                        var tapMappings = db.TagMapping.Where(tm => tm.TargetId == data.NewsId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).ToList();
                        tapMappings.ForEach(l => l.Status = (int)Const.Status.DELETED);

                        //add tag
                        if (data.listTag != null)
                        {
                            foreach (var item in data.listTag)
                            {
                                TagMapping tagMapping = new TagMapping();
                                tagMapping.TagId = item.TagId;
                                tagMapping.TargetId = data.NewsId;
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
                                        attactment.TargetId = news.NewsId;
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
                                            news.Image = item.Url;
                                            db.Entry(news).State = EntityState.Modified;
                                        }
                                    }

                                    if (item.IsImageMain == true && item.Status != (int)Const.Status.DELETED)
                                    {
                                        news.Image = item.Url;
                                    }
                                    db.Entry(news).State = EntityState.Modified;
                                }
                            }
                        }

                        //bai viet  gợi ý
                        List<Related> listRelated = db.Related.Where(r => r.TargetId == news.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).ToList();
                        if (listRelated != null)
                        {
                            listRelated.ForEach(lr => lr.Status = (int)Const.Status.DELETED);
                        }

                        if (data.listRelated != null)
                        {
                            foreach (var item in data.listRelated)
                            {
                                Related related = new Related();
                                related.TargetId = news.NewsId;
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
                        List<Related> listProductRelated = db.Related.Where(r => r.TargetId == news.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).ToList();
                        if (listProductRelated != null)
                        {
                            listProductRelated.ForEach(lr => lr.Status = (int)Const.Status.DELETED);
                        }

                        if (data.listProductRelated != null)
                        {
                            foreach (var item in data.listProductRelated)
                            {
                                Related related = new Related();
                                related.TargetId = news.NewsId;
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
                        if (news.IsHome == true)
                        {
                            if (categoryMapping == null)
                            {
                                CategoryMapping cm = new CategoryMapping();
                                cm.CategoryId = -1;
                                cm.TargetId = news.NewsId;
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

                            if (data.NewsId > 0)
                            {
                                //Thêm vào đóng góp ý kiến
                                if (data.Note != null && data.Note != "")
                                {
                                    NewsNote newsNote = new NewsNote();
                                    newsNote.NewsNoteId = Guid.NewGuid();
                                    newsNote.NewsId = data.NewsId;
                                    newsNote.Note = data.Note;
                                    newsNote.UserId = userId;
                                    newsNote.CreatedAt = DateTime.Now;
                                    newsNote.UpdatedAt = DateTime.Now;
                                    newsNote.Status = (int)Const.Status.NORMAL;
                                    await db.NewsNote.AddAsync(newsNote);
                                    await db.SaveChangesAsync();
                                }
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Biên tập tin bài “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.NewsId.ToString();
                                action.TargetName = data.Title;
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

        // PUT: api/News/5
        [HttpPut("PutNewsPublic/{id}")]
        public async Task<IActionResult> PutNewsPublic(int id, [FromBody] NewsDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
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
                        var UserRole = db.UserRole.Where(e => e.UserId == userId && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        var CodeRoleId = UserRole.RoleId;
                        var role = db.Role.Find(CodeRoleId);
                        if (role.Code.Trim() != "ADMIN" && role.Code.Trim() != "TKTC")
                        {
                            def.meta = new Meta(222, "No permission");
                            return Ok(def);
                        }
                        News news = await db.News.FindAsync(id);
                        if (news == null)
                        {
                            def.meta = new Meta(404, "Not found!");
                            return Ok(def);
                        }

                        string url = data.Url == null ? Utils.NonUnicode(data.Title) : data.Url;
                        url = url.Trim().ToLower();
                        if (news.Url != url)
                        {
                            //check xem trùng link ko
                            var pLink = db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (pLink != null)
                            {
                                def.meta = new Meta(232, "Link đã tồn tại!");
                                return Ok(def);
                            }
                            //cập nhật thay link cũ
                            var permaLink = db.PermaLink.Where(e => e.Slug == news.Url && e.TargetId == news.NewsId
                            && e.TargetType == (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS).FirstOrDefault();
                            if (permaLink != null)
                            {
                                permaLink.TargetId = news.NewsId;
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
                                permaLink1.Slug = news.Url;
                                permaLink1.TargetId = news.NewsId;
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
                            var permaLink = db.PermaLink.Where(e => e.Slug == news.Url && e.TargetId == news.NewsId
                            && e.TargetType == (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS).FirstOrDefault();
                            if (permaLink == null)
                            {
                                PermaLink permaLink1 = new PermaLink();
                                permaLink1.PermaLinkId = Guid.NewGuid();
                                permaLink1.Slug = news.Url;
                                permaLink1.TargetId = news.NewsId;
                                permaLink1.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS;
                                permaLink1.CreatedAt = DateTime.Now;
                                permaLink1.UpdatedAt = DateTime.Now;
                                permaLink1.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink1);
                                await db.SaveChangesAsync();
                            }
                        }

                        news.Title = data.Title;
                        news.Description = data.Description;
                        news.Contents = data.Contents;
                        news.Image = data.Image;
                        news.Url = data.Url;
                        news.Note = data.Note;
                        news.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        news.DateStartOn = data.DateStartOn != null ? data.DateStartOn : news.DateStartActive;
                        news.DateEndOn = data.DateEndOn != null ? data.DateEndOn : news.DateStartActive.Value.AddYears(100);
                        news.IsHome = data.IsHome != null ? data.IsHome : false;
                        news.IsHot = data.IsHot != null ? data.IsHot : false;
                        news.IsComment = data.IsComment != null ? data.IsComment : false;
                        news.IsFirst = data.IsFirst != null ? data.IsFirst : false;
                        news.IsShowView = data.IsShowView != null ? data.IsShowView : false;
                        news.IsAttach = data.IsAttach != null ? data.IsAttach : false;
                        news.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                        news.Location = data.Location;
                        news.TypeNewsId = data.TypeNewsId;
                        news.MetaTitle = data.MetaTitle;
                        news.MetaKeyword = data.MetaKeyword;
                        news.MetaDescription = data.MetaDescription;
                        news.UpdatedAt = DateTime.Now;
                        news.UserId = data.UserId;
                        news.Status = data.Status;
                        news.LinkVideo = data.LinkVideo;
                        news.Introduce = data.Introduce;
                        news.SystemDiagram = data.SystemDiagram;
                        news.Author = data.Author;
                        news.YearTimeline = data.YearTimeline;

                        db.Entry(news).State = EntityState.Modified;

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
                                        categoryNewsMapping.TargetId = news.NewsId;
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
                        var tapMappings = db.TagMapping.Where(tm => tm.TargetId == data.NewsId && tm.Type == (int)Const.TypeTag.TAG_NEWS && tm.Status != (int)Const.Status.DELETED).ToList();
                        tapMappings.ForEach(l => l.Status = (int)Const.Status.DELETED);

                        //add tag
                        if (data.listTag != null)
                        {
                            foreach (var item in data.listTag)
                            {
                                TagMapping tagMapping = new TagMapping();
                                tagMapping.TagId = item.TagId;
                                tagMapping.TargetId = data.NewsId;
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
                                        attactment.TargetId = news.NewsId;
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
                                            news.Image = item.Url;
                                            db.Entry(news).State = EntityState.Modified;
                                        }
                                    }

                                    if (item.IsImageMain == true && item.Status != (int)Const.Status.DELETED)
                                    {
                                        news.Image = item.Url;
                                    }
                                    db.Entry(news).State = EntityState.Modified;
                                }
                            }
                        }

                        //bai viet  gợi ý
                        List<Related> listRelated = db.Related.Where(r => r.TargetId == news.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).ToList();
                        if (listRelated != null)
                        {
                            listRelated.ForEach(lr => lr.Status = (int)Const.Status.DELETED);
                        }

                        if (data.listRelated != null)
                        {
                            foreach (var item in data.listRelated)
                            {
                                Related related = new Related();
                                related.TargetId = news.NewsId;
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
                        List<Related> listProductRelated = db.Related.Where(r => r.TargetId == news.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT && r.Status != (int)Const.Status.DELETED).ToList();
                        if (listProductRelated != null)
                        {
                            listProductRelated.ForEach(lr => lr.Status = (int)Const.Status.DELETED);
                        }

                        if (data.listProductRelated != null)
                        {
                            foreach (var item in data.listProductRelated)
                            {
                                Related related = new Related();
                                related.TargetId = news.NewsId;
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
                        if (news.IsHome == true)
                        {
                            if (categoryMapping == null)
                            {
                                CategoryMapping cm = new CategoryMapping();
                                cm.CategoryId = -1;
                                cm.TargetId = news.NewsId;
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

                            if (data.NewsId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Sửa tin bài “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.NewsId.ToString();
                                action.TargetName = data.Title;
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
        // POST: api/News
        [HttpPost]
        public async Task<IActionResult> PostNews(NewsDTO data)
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
                          && e.TargetId1 == data.NewsRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                          && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();

                        if (checkLang != null)
                        {
                            def.meta = new Meta(228, "Language Exits!");
                            return Ok(def);
                        }

                        //add news
                        News news = new News();
                        news.Title = data.Title;
                        news.Description = data.Description == null ? "" : data.Description;
                        news.Contents = data.Contents;
                        news.Note = data.Note;
                        news.Image = data.Image;
                        news.Url = data.Url != null ? data.Url : Utils.NonUnicode(news.Title);
                        news.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        news.DateStartOn = data.DateStartOn == null ? DateTime.Now : data.DateStartOn;
                        news.DateEndOn = data.DateEndOn == null ? DateTime.Now.AddYears(100) : data.DateEndOn;
                        news.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                        news.IsHome = data.IsHome != null ? data.IsHome : false;
                        news.IsHot = data.IsHot != null ? data.IsHot : false;
                        news.IsComment = data.IsComment != null ? data.IsComment : false;
                        news.IsFirst = data.IsFirst != null ? data.IsFirst : false;
                        news.IsShowView = data.IsShowView != null ? data.IsShowView : false;
                        news.IsAttach = data.IsAttach != null ? data.IsAttach : false;
                        news.Location = data.Location != null ? data.Location : 1;
                        news.TypeNewsId = data.TypeNewsId != null ? data.TypeNewsId : (int)Const.TypeNews.NEWS_TEXT;
                        news.FactorPrice = data.FactorPrice;
                        news.ValuePrice = data.ValuePrice;
                        news.TotalPrice = data.TotalPrice;
                        news.IsCash = data.IsCash;
                        if (news.Contents != null)
                        {
                            string[] listWord = news.Contents.Split(' ');
                            news.NumberWord = listWord.Count();
                            if (news.NumberWord < 500)
                                news.NumberPage = 1;
                            else
                            {
                                float page = (float)news.NumberWord / (float)500;
                                int page1 = (int)Math.Floor(page);
                                float page2 = page - page1;
                                if (page2 >= 0 && page2 <= 0.2)
                                    page = page1;
                                else if (page2 > 0.2 && page2 <= 0.7)
                                    page = page1 + (float)0.5;
                                else
                                    page = page1 + (float)1;
                                news.NumberPage = page;
                            }

                            news.NumberImage = CountImage(news.Contents);
                        }
                        news.MetaTitle = data.MetaTitle != null ? data.MetaTitle : data.Title;
                        news.MetaKeyword = data.MetaKeyword != null ? data.MetaKeyword : data.Title;
                        news.MetaDescription = data.MetaDescription != null ? data.MetaDescription : data.Description;
                        news.LanguageId = data.LanguageId != null ? data.LanguageId : languageId;
                        news.WebsiteId = data.WebsiteId != null ? data.WebsiteId : websiteId;
                        news.CompanyId = data.CompanyId != null ? data.CompanyId : companyId;
                        news.CreatedAt = DateTime.Now;
                        news.UpdatedAt = DateTime.Now;
                        news.UserCreatedId = userId;
                        news.UserId = userId;
                        news.Status = data.Status;
                        news.Introduce = data.Introduce;
                        news.LinkVideo = data.LinkVideo;
                        news.SystemDiagram = data.SystemDiagram;
                        news.Author = data.Author;
                        news.AuthorId = data.AuthorId;
                        news.YearTimeline = data.YearTimeline;

                        await db.News.AddAsync(news);
                        await db.SaveChangesAsync();

                        data.NewsId = news.NewsId;
                        if (data.NewsRootId == null) data.NewsRootId = news.NewsId;

                        if (news.IsHome == true)
                        {
                            CategoryMapping categoryMapping = new CategoryMapping();
                            categoryMapping.CategoryId = -1;
                            categoryMapping.TargetId = news.NewsId;
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
                                categoryNewsMapping.TargetId = news.NewsId;
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
                                tagMapping.TargetId = data.NewsId;
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
                                    attactment.TargetId = news.NewsId;
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
                                        news.Image = item.Url;
                                    }
                                }
                                db.Entry(news).State = EntityState.Modified;
                            }
                        }

                        //add product related
                        if (data.listRelated != null)
                        {
                            foreach (var item in data.listRelated)
                            {
                                Related related = new Related();
                                related.TargetId = news.NewsId;
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
                                related.TargetId = news.NewsId;
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
                            if (data.NewsId > 0)
                            {
                                //Thêm vào đóng góp ý kiến
                                if (data.Note != null && data.Note != "")
                                {
                                    NewsNote newsNote = new NewsNote();
                                    newsNote.NewsNoteId = Guid.NewGuid();
                                    newsNote.NewsId = data.NewsId;
                                    newsNote.Note = data.Note;
                                    newsNote.UserId = userId;
                                    newsNote.CreatedAt = DateTime.Now;
                                    newsNote.UpdatedAt = DateTime.Now;
                                    newsNote.Status = (int)Const.Status.NORMAL;
                                    await db.NewsNote.AddAsync(newsNote);
                                    await db.SaveChangesAsync();
                                }
                                //Thêm permalink
                                PermaLink permaLink = new PermaLink();
                                permaLink.PermaLinkId = Guid.NewGuid();
                                permaLink.Slug = news.Url;
                                permaLink.TargetId = news.NewsId;
                                permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS;
                                permaLink.CreatedAt = DateTime.Now;
                                permaLink.UpdatedAt = DateTime.Now;
                                permaLink.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink);
                                await db.SaveChangesAsync();
                                //Thêm ngôn ngữ (nếu bài viết mới thêm ko phải là ngôn ngữ mạc định)
                                //và có id bài viết gốc
                                if (data.LanguageId != languageId && data.LanguageId != null && data.LanguageId > 0
                                    && data.NewsRootId != null && data.NewsRootId > 0)
                                {
                                    var listLang = db.LanguageMapping.Where(e => e.LanguageId1 == languageId
                                      && e.TargetId1 == data.NewsRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                                      && e.Status != (int)Const.Status.DELETED).ToList();
                                    if (listLang.Count > 0)
                                    {
                                        LanguageMapping languageMapping = new LanguageMapping();
                                        languageMapping.LanguageId1 = languageId;
                                        languageMapping.LanguageId2 = data.LanguageId;
                                        languageMapping.TargetId1 = data.NewsRootId;
                                        languageMapping.TargetId2 = data.NewsId;
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
                                            languageMapping2.TargetId2 = data.NewsId;
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
                                        languageMapping.TargetId1 = data.NewsRootId;
                                        languageMapping.TargetId2 = data.NewsId;
                                        languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_NEWS;
                                        languageMapping.CreatedAt = DateTime.Now;
                                        languageMapping.Status = (int)Const.Status.NORMAL;
                                        await db.LanguageMapping.AddAsync(languageMapping);
                                    }
                                    await db.SaveChangesAsync();
                                    //numLang = listLang.Count + 2;
                                }
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Thêm tin bài “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.NewsId.ToString();
                                action.TargetName = data.Title;
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
                                data.listLanguage = db.LanguageMapping.Where(a => a.LanguageId1 == languageId && a.TargetId1 == (int)data.NewsRootId
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
                            if (NewsExists(data.NewsId))
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

        // POST: api/News
        [HttpPost("PostNewsPublic")]
        public async Task<IActionResult> PostNewsPublic(NewsDTO data)
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
                        var UserRole = db.UserRole.Where(e => e.UserId == userId && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        var CodeRoleId = UserRole.RoleId;
                        var role = db.Role.Find(CodeRoleId);
                        if (role.Code.Trim() != "ADMIN" && role.Code.Trim() == "TKTC")
                        {
                            def.meta = new Meta(222, "No permission");
                            return Ok(def);
                        }
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
                          && e.TargetId1 == data.NewsRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                          && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();

                        if (checkLang != null)
                        {
                            def.meta = new Meta(228, "Language Exits!");
                            return Ok(def);
                        }

                        //add news
                        News news = new News();
                        news.Title = data.Title;
                        news.Description = data.Description == null ? "" : data.Description;
                        news.Contents = data.Contents;
                        news.Image = data.Image;
                        news.Url = data.Url != null ? data.Url : Utils.NonUnicode(news.Title);
                        news.Note = data.Note;
                        news.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        news.DateStartOn = data.DateStartOn == null ? DateTime.Now : data.DateStartOn;
                        news.DateEndOn = data.DateEndOn == null ? DateTime.Now.AddYears(100) : data.DateEndOn;
                        news.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                        news.IsHome = data.IsHome != null ? data.IsHome : false;
                        news.IsHot = data.IsHot != null ? data.IsHot : false;
                        news.IsComment = data.IsComment != null ? data.IsComment : false;
                        news.IsFirst = data.IsFirst != null ? data.IsFirst : false;
                        news.IsShowView = data.IsShowView != null ? data.IsShowView : false;
                        news.IsAttach = data.IsAttach != null ? data.IsAttach : false;
                        news.Location = data.Location != null ? data.Location : 1;
                        news.TypeNewsId = data.TypeNewsId != null ? data.TypeNewsId : (int)Const.TypeNews.NEWS_TEXT;
                        news.MetaTitle = data.MetaTitle != null ? data.MetaTitle : data.Title;
                        news.MetaKeyword = data.MetaKeyword != null ? data.MetaKeyword : data.Title;
                        news.MetaDescription = data.MetaDescription != null ? data.MetaDescription : data.Description;
                        news.LanguageId = data.LanguageId != null ? data.LanguageId : languageId;
                        news.WebsiteId = data.WebsiteId != null ? data.WebsiteId : websiteId;
                        news.CompanyId = data.CompanyId != null ? data.CompanyId : companyId;
                        news.CreatedAt = DateTime.Now;
                        news.UpdatedAt = DateTime.Now;
                        news.UserId = data.UserId;
                        news.Status = data.Status;
                        news.Introduce = data.Introduce;
                        news.LinkVideo = data.LinkVideo;
                        news.SystemDiagram = data.SystemDiagram;
                        news.Author = db.User.Where(u => u.UserId == news.UserId).Select(u => u.FullName).FirstOrDefault();
                        news.YearTimeline = data.YearTimeline;

                        await db.News.AddAsync(news);
                        await db.SaveChangesAsync();

                        data.NewsId = news.NewsId;
                        if (data.NewsRootId == null) data.NewsRootId = news.NewsId;

                        if (news.IsHome == true)
                        {
                            CategoryMapping categoryMapping = new CategoryMapping();
                            categoryMapping.CategoryId = -1;
                            categoryMapping.TargetId = news.NewsId;
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
                                categoryNewsMapping.TargetId = news.NewsId;
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
                                tagMapping.TargetId = data.NewsId;
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
                                    attactment.TargetId = news.NewsId;
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
                                        news.Image = item.Url;
                                    }
                                }
                                db.Entry(news).State = EntityState.Modified;
                            }
                        }

                        //add product related
                        if (data.listRelated != null)
                        {
                            foreach (var item in data.listRelated)
                            {
                                Related related = new Related();
                                related.TargetId = news.NewsId;
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
                                related.TargetId = news.NewsId;
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
                            if (data.NewsId > 0)
                            {
                                //Thêm permalink
                                PermaLink permaLink = new PermaLink();
                                permaLink.PermaLinkId = Guid.NewGuid();
                                permaLink.Slug = news.Url;
                                permaLink.TargetId = news.NewsId;
                                permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS;
                                permaLink.CreatedAt = DateTime.Now;
                                permaLink.UpdatedAt = DateTime.Now;
                                permaLink.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink);
                                await db.SaveChangesAsync();
                                //Thêm ngôn ngữ (nếu bài viết mới thêm ko phải là ngôn ngữ mạc định)
                                //và có id bài viết gốc
                                if (data.LanguageId != languageId && data.LanguageId != null && data.LanguageId > 0
                                    && data.NewsRootId != null && data.NewsRootId > 0)
                                {
                                    var listLang = db.LanguageMapping.Where(e => e.LanguageId1 == languageId
                                      && e.TargetId1 == data.NewsRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                                      && e.Status != (int)Const.Status.DELETED).ToList();
                                    if (listLang.Count > 0)
                                    {
                                        LanguageMapping languageMapping = new LanguageMapping();
                                        languageMapping.LanguageId1 = languageId;
                                        languageMapping.LanguageId2 = data.LanguageId;
                                        languageMapping.TargetId1 = data.NewsRootId;
                                        languageMapping.TargetId2 = data.NewsId;
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
                                            languageMapping2.TargetId2 = data.NewsId;
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
                                        languageMapping.TargetId1 = data.NewsRootId;
                                        languageMapping.TargetId2 = data.NewsId;
                                        languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_NEWS;
                                        languageMapping.CreatedAt = DateTime.Now;
                                        languageMapping.Status = (int)Const.Status.NORMAL;
                                        await db.LanguageMapping.AddAsync(languageMapping);
                                    }
                                    await db.SaveChangesAsync();
                                    //numLang = listLang.Count + 2;
                                }
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Thêm tin bài “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.NewsId.ToString();
                                action.TargetName = data.Title;
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
                                data.listLanguage = db.LanguageMapping.Where(a => a.LanguageId1 == languageId && a.TargetId1 == (int)data.NewsRootId
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
                            if (NewsExists(data.NewsId))
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
        [HttpDelete("{id}/{type}")]
        public async Task<IActionResult> DeleteNews(int id, int type)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            if (type == 1) functionCode = functionCodeVB;
            else if (type == 2) functionCode = functionCodeBT;
            else if (type == 3) functionCode = functionCodeKD;
            else if (type == 4) functionCode = functionCodeXB;
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
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
                        data.Status = (int)Const.Status.DELETED;
                        db.News.Update(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.NewsId > 0)
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
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Xoá tin bài “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.DELETE;
                                action.TargetId = data.NewsId.ToString();
                                action.TargetName = data.Title;
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

        [HttpDelete("DeleteNewsPublic/{id}/{type}")]
        public async Task<IActionResult> DeleteNewsPublic(int id, int type)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            if (type == 1) functionCode = functionCodeVB;
            else if (type == 2) functionCode = functionCodeBT;
            else if (type == 3) functionCode = functionCodeKD;
            else if (type == 4) functionCode = functionCodeXB;
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    //var UserRole = db.UserRole.Where(e => e.UserId == userId && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    //var CodeRoleId = UserRole.RoleId;
                    //var role = db.Role.Find(CodeRoleId);
                    //if (role.Code.Trim() != "ADMIN")
                    //{
                    //    def.meta = new Meta(222, "No permission");
                    //    return Ok(def);
                    //}
                    News data = await db.News.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {

                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.News.Update(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.NewsId > 0)
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
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Xoá tin bài “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.DELETE;
                                action.TargetId = data.NewsId.ToString();
                                action.TargetName = data.Title;
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

        [HttpDelete("DeleteNewsTrash/{id}")]
        public async Task<IActionResult> DeleteNewsTrash(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    var UserRole = db.UserRole.Where(e => e.UserId == userId && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    var CodeRoleId = UserRole.RoleId;
                    var role = db.Role.Find(CodeRoleId);
                    if (role.Code.Trim() != "ADMIN")
                    {
                        def.meta = new Meta(222, "No permission");
                        return Ok(def);
                    }
                    News data = await db.News.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {

                        //data.UpdatedAt = DateTime.Now;
                        //data.Status = (int)Const.Status.DELETED;
                        //db.News.Update(data);
                        db.News.Remove(data);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.NewsId > 0)
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
                                    db.LanguageMapping.RemoveRange(mapLang);
                                    await db.SaveChangesAsync();
                                }

                                //Xóa link
                                var permaLink = db.PermaLink.Where(e => e.Slug == data.Url && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (permaLink != null)
                                {
                                    permaLink.UpdatedAt = DateTime.Now;
                                    permaLink.Status = (int)Const.Status.DELETED;
                                    db.PermaLink.Remove(permaLink);
                                    await db.SaveChangesAsync();
                                }

                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Xoá vĩnh viễn tin bài “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.DELETE;
                                action.TargetId = data.NewsId.ToString();
                                action.TargetName = data.Title;
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

                        Models.EF.Action action = new Models.EF.Action();
                        action.ActionName = "Xoá tin bài “" + news.Title + "”";
                        action.ActionType = (int)Const.ActionType.DELETE;
                        action.TargetId = news.NewsId.ToString();
                        action.TargetName = news.Title;
                        action.CompanyId = companyId;
                        action.Logs = JsonConvert.SerializeObject(news);
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

        //API xóa danh sách bài viết
        [HttpPut("DeleteMultiNewsPublic")]
        public async Task<IActionResult> DeleteMultiNewsPublic([FromBody] int[] data)
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
                    var UserRole = db.UserRole.Where(e => e.UserId == userId && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    var CodeRoleId = UserRole.RoleId;
                    var role = db.Role.Find(CodeRoleId);
                    if (role.Code.Trim() != "ADMIN")
                    {
                        def.meta = new Meta(222, "No permission");
                        return Ok(def);
                    }
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

                        Models.EF.Action action = new Models.EF.Action();
                        action.ActionName = "Xoá tin bài “" + news.Title + "”";
                        action.ActionType = (int)Const.ActionType.DELETE;
                        action.TargetId = news.NewsId.ToString();
                        action.TargetName = news.Title;
                        action.CompanyId = companyId;
                        action.Logs = JsonConvert.SerializeObject(news);
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
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
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
                    //var UserRole = db.UserRole.Where(e => e.Status != (int)Const.Status.DELETED && e.UserId == userId).FirstOrDefault();
                    //var CodeRoleId = UserRole.RoleId;
                    //var role = db.Role.Find(CodeRoleId);
                    //if (role.Code.Trim() != "ADMIN" && role.Code.Trim() != "TKTC" && role.Code != "BTV")
                    //{
                    //    def.meta = new Meta(222, "No permission");
                    //    return Ok(def);
                    //}
                    News data = await db.News.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    int statusOld = (int)data.Status;
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UpdatedAt = DateTime.Now;
                        data.UserId = userId;
                        data.Status = (byte)stt;
                        if ((data.Status == (int)Const.NewsStatus.EDITING
                            || data.Status == (int)Const.NewsStatus.EDITED
                            || data.Status == (int)Const.NewsStatus.RE_EDITED)
                            && data.EditingAt != null)
                        {
                            data.UserEditedId = userId;
                            data.EditedAt = DateTime.Now;
                        }
                        else if ((data.Status == (int)Const.NewsStatus.APPROVING
                            || data.Status == (int)Const.NewsStatus.PUBLISHING
                            || data.Status == (int)Const.NewsStatus.NOT_APPROVED)
                            && data.ApprovingAt != null)
                        {
                            data.UserApprovedId = userId;
                            data.ApprovedAt = DateTime.Now;
                        }
                        else if ((data.Status == (int)Const.NewsStatus.NORMAL
                            || data.Status == (int)Const.NewsStatus.PUBLISHING
                            || data.Status == (int)Const.NewsStatus.UN_PUBLISH)
                            && data.PublishingAt != null)
                        {
                            data.UserPublishedId = userId;
                            data.PublishedAt = DateTime.Now;
                        }

                        if (data.Status == (int)Const.NewsStatus.EDITING && data.EditingAt == null)
                        {
                            data.EditingAt = DateTime.Now;
                        }
                        if (data.Status == (int)Const.NewsStatus.APPROVING && data.ApprovingAt == null)
                        {
                            data.ApprovingAt = DateTime.Now;
                        }
                        if (data.Status == (int)Const.NewsStatus.PUBLISHING && data.PublishingAt == null)
                        {
                            data.PublishingAt = DateTime.Now;
                        }
                        if ((data.Status == (int)Const.NewsStatus.NORMAL))
                        {
                            data.PublishedAt = DateTime.Now;
                            data.DateStartActive = DateTime.Now;
                        }
                        else if ((data.Status == (int)Const.NewsStatus.PUBLISHING))
                        {
                            data.DateStartActive = DateTime.Now;
                        }
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
                                        //dataLang.UserId = userId;
                                        dataLang.Status = (byte)stt;
                                        db.News.Update(dataLang);
                                    }
                                }
                                await db.SaveChangesAsync();
                            }

                            if (data.NewsId > 0)
                            {
                                transaction.Commit();

                                //gửi email, tạo thông báo và action
                                try
                                {
                                    //Gửi email
                                    //Gửi mail cho phóng viên or ban biên tập khi bị trả lại bài
                                    if (data.Status == 12)
                                    {
                                        var config = await db.Config.FindAsync(1);
                                        if (config != null)
                                        {
                                            //Lấy thông tin user tạo bài viết
                                            var user = db.User.Where(e => e.UserId == data.UserCreatedId).FirstOrDefault();
                                            //Tạo nd
                                            string str = "";
                                            string fullName = "";
                                            string email = "";
                                            bool checkSend = false;
                                            if (statusOld != data.Status)
                                            {
                                                str = "Biên tập viên tạp chí Dân chủ & Pháp luật trả lại tin bài “" + data.Title + "”";
                                                checkSend = true;
                                                fullName = user.FullName;
                                                email = user.Email;
                                            }
                                            if (checkSend)
                                            {
                                                string subject = config.EmailSender + " - Trả lại bài viết";
                                                String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/news-approved.html");
                                                sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                                    config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                                    config.Website, fullName, str);

                                                //email = "long.eds.it@gmail.com";
                                                bool SendEmail = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, email, subject, sBody);
                                            }
                                        }
                                    }

                                    //tạo action
                                    Models.EF.Action action = new Models.EF.Action();
                                    var STT = data.Status;
                                    if (STT == 1)
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài thành xuất bản “" + data.Title + "”";
                                    }
                                    else if (STT == 10)
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài thành nháp “" + data.Title + "”";
                                    }
                                    else if (STT == 11)
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài thành bài viết mới “" + data.Title + "”";
                                    }
                                    else if (STT == 12)
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài thành Bài bị trả lại “" + data.Title + "”";
                                    }
                                    else if (STT == 13)
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài thành chờ biên tập “" + data.Title + "”";
                                    }
                                    else if (STT == 14)
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài thành đã biên tập “" + data.Title + "”";
                                    }
                                    else if (STT == 15)
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài thành biên tập lại “" + data.Title + "”";
                                    }
                                    else if (STT == 16)
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài thành chờ duyệt “" + data.Title + "”";
                                    }
                                    else if (STT == 17)
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài thành không duyệt “" + data.Title + "”";
                                    }
                                    else if (STT == 18)
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài thành chờ xuất bản “" + data.Title + "”";
                                    }
                                    else if (STT == 19)
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài thành gỡ xuất bản “" + data.Title + "”";
                                    }
                                    else if (STT == 98)
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài thành khoá “" + data.Title + "”";
                                    }
                                    else
                                    {
                                        action.ActionName = "Đổi trạng thái tin bài “" + data.Title + "”";
                                    }
                                    action.ActionType = (int)Const.ActionType.UPDATE;
                                    action.TargetId = data.NewsId.ToString();
                                    action.TargetName = data.Title;
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

                                    //Tạo thông báo
                                    if (data.Status == 12)
                                    {
                                        if (data.UserCreatedId != userId)
                                        {
                                            Models.EF.Action action1 = new Models.EF.Action();
                                            action1.ActionName = "Biên tập viên tạp chí Dân chủ & Pháp luật trả lại tin bài “" + data.Title + "”";
                                            action1.ActionType = (int)Const.ActionType.UPDATE;
                                            action1.TargetId = data.NewsId.ToString();
                                            action1.TargetName = action1.ActionName + " của bạn, bạn có thể xem lý do " +
                                                "trả lại bài tại phần trao đổi trong bài viết hoặc liên hệ với Biên tập viên tạp chí Dân chủ & Pháp luật ";
                                            action1.CompanyId = companyId;
                                            action1.Logs = JsonConvert.SerializeObject(data);
                                            action1.Time = 0;
                                            action1.Ipaddress = IpAddress();
                                            action1.Type = (int)Const.TypeAction.NOTIFICATION;
                                            action1.CreatedAt = DateTime.Now;
                                            action1.UserPushId = userId;
                                            action1.UserId = data.UserCreatedId;
                                            action1.Status = (int)Const.Status.NORMAL;
                                            await db.Action.AddAsync(action1);
                                            await db.SaveChangesAsync();
                                        }
                                    }
                                }
                                catch { };
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

        [HttpPost("exportNews")]
        public async Task<HttpResponseMessage> ExportNews([FromBody] FilterReport data)
        {
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                var response3 = new HttpResponseMessage(HttpStatusCode.NotModified);
                return response3;
            }
            using (var db = new IOITDataContext())
            {
                var dateStart = new DateTime(2000, 1, 1);
                var dateEnd = DateTime.Now;
                if (data.DateStart != null)
                    dateStart = new DateTime(data.DateStart.Value.Year, data.DateStart.Value.Month, data.DateStart.Value.Day, 0, 0, 0);
                if (data.DateEnd != null)
                    dateEnd = new DateTime(data.DateEnd.Value.Year, data.DateEnd.Value.Month, data.DateEnd.Value.Day, 23, 59, 59);
                if (data.UserId == null) data.UserId = -1;
                if (data.AuthorId == null) data.AuthorId = -1;
                if (data.LanguageId == null) data.LanguageId = -1;
                if (data.Type == null) data.Type = -1;
                if (data.Status == null) data.Status = -1;
                List<WritetingMoney> report = new List<WritetingMoney>();
                if (data.CategoryId != null)
                {
                    report = await (from cm in db.CategoryMapping
                                    join n in db.News on cm.TargetId equals n.NewsId
                                    where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                    && cm.CategoryId == data.CategoryId
                                    && n.Status != (int)Const.Status.DELETED
                                    && cm.Status != (int)Const.Status.DELETED
                                    && (n.UserCreatedId == data.UserId || data.UserId == -1)
                                    && (n.AuthorId == data.AuthorId || data.AuthorId == -1)
                                    && (n.LanguageId == data.LanguageId || data.LanguageId == -1)
                                    && (n.TypeNewsId == data.Type || data.Type == -1)
                                    select new WritetingMoney
                                    {
                                        NewsId = n.NewsId,
                                        UserId = n.AuthorId,
                                        TypeNewsId = n.TypeNewsId,
                                        Title = n.Title,
                                        Image = n.Image,
                                        ListCategory = "",
                                        DateStart = n.DateStartActive,
                                        TotalPrice = n.TotalPrice,
                                        ViewNumber = n.ViewNumber,
                                        IsCash = n.IsCash,
                                        Status = n.Status,
                                    }).OrderByDescending(e => e.NewsId).ToListAsync();
                }
                else
                {
                    report = await (from n in db.News
                                    where (n.UserCreatedId == data.UserId || data.UserId == -1)
                                    && (n.AuthorId == data.AuthorId || data.AuthorId == -1)
                                    && (n.LanguageId == data.LanguageId || data.LanguageId == -1)
                                    && (n.TypeNewsId == data.Type || data.Type == -1)
                                    select new WritetingMoney
                                    {
                                        NewsId = n.NewsId,
                                        UserId = n.AuthorId,
                                        TypeNewsId = n.TypeNewsId,
                                        Title = n.Title,
                                        Image = n.Image,
                                        ListCategory = "",
                                        DateStart = n.DateStartActive,
                                        TotalPrice = n.TotalPrice,
                                        ViewNumber = n.ViewNumber,
                                        IsCash = n.IsCash,
                                        Status = n.Status,
                                    }).OrderByDescending(e => e.NewsId).ToListAsync();
                }
                if (data.Status != -1)
                {
                    report = report.Where(e => e.Status == data.Status).ToList();
                }
                else
                {
                    if (data.TypeExport == 1)
                    {
                        report = report.Where(n => (n.Status == 10 || n.Status == 11 || n.Status == 12)).ToList();
                    }
                    else if (data.TypeExport == 2)
                    {
                        report = report.Where(n => (n.Status == 13 || n.Status == 14 || n.Status == 15)).ToList();
                    }
                    else if (data.TypeExport == 3)
                    {
                        report = report.Where(n => (n.Status == 16 || n.Status == 17)).ToList();
                    }
                    else if (data.TypeExport == 4)
                    {
                        report = report.Where(n => (n.Status == 1 || n.Status == 18 || n.Status == 19)).ToList();
                    }
                }

                foreach (var item in report)
                {
                    var cate = await (from cm in db.CategoryMapping
                                      join cat in db.Category on cm.CategoryId equals cat.CategoryId
                                      where cm.Status != (int)Const.Status.DELETED
                                      && cm.TargetId == item.NewsId && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                      select cat).ToListAsync();
                    foreach (var itemC in cate)
                    {
                        item.ListCategory += itemC.Name + ",";
                    }
                    var user = await db.Author.Where(e => e.AuthorId == item.UserId).FirstOrDefaultAsync();
                    item.FullName = user != null ? user.Name : "";
                }

                string fPath = _hostingEnvironment.WebRootPath;
                string template = @"/template/export/DSBV.xlsx";

                MemoryStream ms = writeNewsToExcel(fPath, template, 0, report);

                // xong hết thì save file lại
                string fileName = "list-news" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

                if (!string.IsNullOrEmpty(fileName))
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(ms.ToArray())
                    };
                    response.Content.Headers.Add("Access-Control-Allow-Headers", "Authorization,Content-Type,x-filename");
                    response.Content.Headers.Add("Access-Control-Expose-Headers", "Authorization,Content-Type,x-filename");
                    response.Content.Headers.Add("x-filename", fileName);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue
                           ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    response.Content.Headers.ContentDisposition =
                           new ContentDispositionHeaderValue("attachment")
                           {
                               FileName = fileName
                           };

                    return response;
                }


            }
            var response2 = new HttpResponseMessage(HttpStatusCode.NotFound);
            return response2;
        }

        public MemoryStream writeNewsToExcel(string fPath, string tempfile, int sheetnumber, List<WritetingMoney> data)
        {
            FileStream file1 = new FileStream(fPath + tempfile, FileMode.Open, FileAccess.Read);
            XSSFWorkbook workbook = new XSSFWorkbook(file1);
            ISheet sheet = workbook.GetSheetAt(sheetnumber);
            //IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
            int rowStart = 3;

            if (sheet != null)
            {
                //write contract
                //sheet.GetRow(2).GetCell(2).SetCellValue(contractDetail.ProjectName);

                int datasize = data.Count;
                int datacol = 8;
                try
                {
                    //Lấy danh sách style template
                    List<ICellStyle> rowStyle = new List<ICellStyle>();
                    for (int i = 0; i < datacol; i++)
                    {
                        rowStyle.Add(sheet.GetRow(rowStart).GetCell(i).CellStyle);
                    }

                    //Thêm danh sách con
                    for (int rr = 0; rr < datasize; rr++)
                    {
                        try
                        {
                            int k = rowStart + rr;
                            XSSFRow row = (XSSFRow)sheet.CreateRow(k);

                            for (int i = 0; i < datacol; i++)
                            {
                                row.CreateCell(i).CellStyle = rowStyle[i];
                                if (i == 0)
                                {
                                    row.GetCell(i).SetCellValue(rr + 1);
                                }
                                if (i == 1)
                                {
                                    row.GetCell(i).SetCellValue(data[rr].NewsId);
                                }
                                else if (i == 2)
                                {
                                    row.GetCell(i).SetCellValue(data[rr].Title);
                                }
                                else if (i == 3)
                                {
                                    row.GetCell(i).SetCellValue(data[rr].ListCategory);
                                }
                                else if (i == 4)
                                {
                                    row.GetCell(i).SetCellValue(data[rr].FullName);
                                }
                                else if (i == 5)
                                {
                                    if (data[rr].DateStart != null)
                                    {
                                        row.GetCell(i).SetCellValue(DateTime.Parse(data[rr].DateStart.ToString()));
                                    }
                                }
                                else if (i == 6)
                                {
                                    if (data[rr].TotalPrice != null)
                                        row.GetCell(i).SetCellValue(double.Parse(data[rr].TotalPrice.ToString()));
                                }
                                //else if (i == 6)
                                //{
                                //    if (data[rr].ViewNumber != null)
                                //    {
                                //        row.GetCell(i).SetCellValue(int.Parse(data[rr].ViewNumber.ToString()));
                                //    }
                                //}
                                else if (i == 7)
                                {
                                    string status = "";
                                    if (data[rr].Status == (int)Const.NewsStatus.TEMP)
                                        status = "Bản nháp";
                                    else if (data[rr].Status == (int)Const.NewsStatus.NEW)
                                        status = "Bản viết mới";
                                    else if (data[rr].Status == (int)Const.NewsStatus.RE_NEW)
                                        status = "Bài viết bị trả lại";
                                    else if (data[rr].Status == (int)Const.NewsStatus.EDITING)
                                        status = "Chờ biên tập";
                                    else if (data[rr].Status == (int)Const.NewsStatus.EDITED)
                                        status = "Đã biên tập";
                                    else if (data[rr].Status == (int)Const.NewsStatus.RE_EDITED)
                                        status = "Biên tập lại";
                                    else if (data[rr].Status == (int)Const.NewsStatus.APPROVING)
                                        status = "Chờ duyệt";
                                    else if (data[rr].Status == (int)Const.NewsStatus.NOT_APPROVED)
                                        status = "Không duyệt";
                                    else if (data[rr].Status == (int)Const.NewsStatus.PUBLISHING)
                                        status = "Chờ xuất bản";
                                    else if (data[rr].Status == (int)Const.NewsStatus.NORMAL)
                                        status = "Xuất bản";
                                    else if (data[rr].Status == (int)Const.NewsStatus.UN_PUBLISH)
                                        status = "Gỡ xuất bản";
                                    row.GetCell(i).SetCellValue(status);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("Exception:" + ex);
                        }
                    }

                }
                catch (Exception ee)
                {
                    log.Error("Exception:" + ee);
                }
            }

            sheet.ForceFormulaRecalculation = true;

            MemoryStream ms = new MemoryStream();

            workbook.Write(ms);

            return ms;
        }

        [HttpPost("exportNewsCash")]
        public async Task<HttpResponseMessage> ExportNewsCash([FromBody] FilterReport data)
        {
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                var response3 = new HttpResponseMessage(HttpStatusCode.NotModified);
                return response3;
            }
            using (var db = new IOITDataContext())
            {
                var dateStart = new DateTime(2000, 1, 1);
                var dateEnd = DateTime.Now;
                if (data.DateStart != null)
                    dateStart = new DateTime(data.DateStart.Value.Year, data.DateStart.Value.Month, data.DateStart.Value.Day, 0, 0, 0);
                if (data.DateEnd != null)
                    dateEnd = new DateTime(data.DateEnd.Value.Year, data.DateEnd.Value.Month, data.DateEnd.Value.Day, 23, 59, 59);
                if (data.UserId == null) data.UserId = -1;
                if (data.AuthorId == null) data.AuthorId = -1;
                if (data.LanguageId == null) data.LanguageId = -1;
                if (data.Type == null) data.Type = -1;
                if (data.Status == null) data.Status = -1;

                List<WritetingMoney> report = new List<WritetingMoney>();
                if (data.CategoryId != null)
                {
                    report = await (from cm in db.CategoryMapping
                                    join n in db.News on cm.TargetId equals n.NewsId
                                    where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                    && cm.CategoryId == data.CategoryId
                                    && n.Status != (int)Const.Status.DELETED
                                    && cm.Status != (int)Const.Status.DELETED
                                    && n.CreatedAt >= dateStart && n.CreatedAt <= dateEnd
                                    && ((n.TotalPrice != null && data.CashStatus == true)
                                    || (n.TotalPrice == null && data.CashStatus != true)
                                    || data.CashStatus == null)
                                    select new WritetingMoney
                                    {
                                        NewsId = n.NewsId,
                                        UserId = n.AuthorId,
                                        TypeNewsId = n.TypeNewsId,
                                        Title = n.Title,
                                        Image = n.Image,
                                        ListCategory = "",
                                        DateStart = n.DateStartActive,
                                        TotalPrice = n.TotalPrice,
                                        NumberWord = n.NumberWord,
                                        NumberPage = n.NumberPage,
                                        NumberImage = n.NumberImage,
                                        ViewNumber = n.ViewNumber,
                                        IsCash = n.IsCash,
                                        Status = n.Status,
                                    }).OrderByDescending(e => e.NewsId).ToListAsync();
                }
                else
                {
                    report = await (from n in db.News
                                    where n.Status != (int)Const.Status.DELETED
                                    && n.CreatedAt >= dateStart && n.CreatedAt <= dateEnd
                                    && (n.UserCreatedId == data.UserId || data.UserId == -1)
                                    && (n.AuthorId == data.AuthorId || data.AuthorId == -1)
                                    && (n.LanguageId == data.LanguageId || data.LanguageId == -1)
                                    && (n.TypeNewsId == data.Type || data.Type == -1)
                                    && (n.Status == data.Status || data.Status == -1)
                                    && ((n.TotalPrice != null && data.CashStatus == true)
                                    || (n.TotalPrice == null && data.CashStatus != true)
                                    || data.CashStatus == null)
                                    select new WritetingMoney
                                    {
                                        NewsId = n.NewsId,
                                        UserId = n.AuthorId,
                                        TypeNewsId = n.TypeNewsId,
                                        Title = n.Title,
                                        Image = n.Image,
                                        ListCategory = "",
                                        DateStart = n.DateStartActive,
                                        TotalPrice = n.TotalPrice,
                                        NumberWord = n.NumberWord,
                                        NumberPage = n.NumberPage,
                                        NumberImage = n.NumberImage,
                                        ViewNumber = n.ViewNumber,
                                        IsCash = n.IsCash,
                                        Status = n.Status,
                                    }).OrderByDescending(e => e.NewsId).ToListAsync();
                }

                foreach (var item in report)
                {
                    //var cate = await (from cm in db.CategoryMapping
                    //                  join cat in db.Category on cm.CategoryId equals cat.CategoryId
                    //                  where cm.Status != (int)Const.Status.DELETED
                    //                  && cm.TargetId == item.NewsId && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                    //                  select cat).ToListAsync();
                    //foreach (var itemC in cate)
                    //{
                    //    item.ListCategory += itemC.Name + ",";
                    //}
                    var user = await db.Author.Where(e => e.AuthorId == item.UserId).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        item.FullName = user.Name;
                        item.Address = user.Address;
                        item.Cmtnd = user.Cccd;
                    }
                }

                string fPath = _hostingEnvironment.WebRootPath;
                string template = @"/template/export/DSNB.xlsx";

                MemoryStream ms = writeNewsToExcelCash(fPath, template, 0, report);

                // xong hết thì save file lại
                string fileName = "list-news" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

                if (!string.IsNullOrEmpty(fileName))
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(ms.ToArray())
                    };
                    response.Content.Headers.Add("Access-Control-Allow-Headers", "Authorization,Content-Type,x-filename");
                    response.Content.Headers.Add("Access-Control-Expose-Headers", "Authorization,Content-Type,x-filename");
                    response.Content.Headers.Add("x-filename", fileName);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue
                           ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    response.Content.Headers.ContentDisposition =
                           new ContentDispositionHeaderValue("attachment")
                           {
                               FileName = fileName
                           };

                    return response;
                }


            }
            var response2 = new HttpResponseMessage(HttpStatusCode.NotFound);
            return response2;
        }

        public MemoryStream writeNewsToExcelCash(string fPath, string tempfile, int sheetnumber, List<WritetingMoney> data)
        {
            FileStream file1 = new FileStream(fPath + tempfile, FileMode.Open, FileAccess.Read);
            XSSFWorkbook workbook = new XSSFWorkbook(file1);
            ISheet sheet = workbook.GetSheetAt(sheetnumber);
            //IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
            int rowStart = 3;

            if (sheet != null)
            {
                //write contract
                //sheet.GetRow(2).GetCell(2).SetCellValue(contractDetail.ProjectName);

                int datasize = data.Count;
                int datacol = 13;
                try
                {
                    //Lấy danh sách style template
                    List<ICellStyle> rowStyle = new List<ICellStyle>();
                    for (int i = 0; i < datacol; i++)
                    {
                        rowStyle.Add(sheet.GetRow(rowStart).GetCell(i).CellStyle);
                    }

                    //Thêm danh sách con
                    for (int rr = 0; rr < datasize; rr++)
                    {
                        try
                        {
                            int k = rowStart + rr;
                            XSSFRow row = (XSSFRow)sheet.CreateRow(k);

                            for (int i = 0; i < datacol; i++)
                            {
                                row.CreateCell(i).CellStyle = rowStyle[i];
                                if (i == 0)
                                {
                                    row.GetCell(i).SetCellValue(rr + 1);
                                }
                                if (i == 1)
                                {
                                    row.GetCell(i).SetCellValue(data[rr].Title);
                                }
                                else if (i == 2)
                                {
                                    if (data[rr].DateStart != null)
                                        row.GetCell(i).SetCellValue(DateTime.Parse(data[rr].DateStart.ToString()));
                                }
                                else if (i == 3)
                                {
                                    row.GetCell(i).SetCellValue(data[rr].FullName);
                                }
                                else if (i == 4)
                                {
                                    row.GetCell(i).SetCellValue(data[rr].Address);
                                }
                                else if (i == 5)
                                {
                                    string type = "";
                                    if (data[rr].TypeNewsId == (int)Const.TypeNews.NEWS_TEXT)
                                        type = "Tin văn bản";
                                    else if (data[rr].TypeNewsId == (int)Const.TypeNews.NEWS_IMAGE)
                                        type = "Tin hình ảnh";
                                    else if (data[rr].TypeNewsId == (int)Const.TypeNews.NEWS_VIDEO)
                                        type = "Tin video";
                                    else if (data[rr].TypeNewsId == (int)Const.TypeNews.NEWS_EVENT)
                                        type = "Tin sự kiện";
                                    else if (data[rr].TypeNewsId == (int)Const.TypeNews.NEWS_NEWS)
                                        type = "Tin bài viết";
                                    row.GetCell(i).SetCellValue(type);
                                }
                                else if (i == 6)
                                {
                                    if (data[rr].NumberWord != null)
                                        row.GetCell(i).SetCellValue(int.Parse(data[rr].NumberWord.ToString()));
                                }
                                else if (i == 7)
                                {
                                    if (data[rr].NumberPage != null)
                                    {
                                        row.GetCell(i).SetCellValue(double.Parse(data[rr].NumberPage.ToString()));
                                    }
                                }
                                else if (i == 8)
                                {
                                    if (data[rr].NumberImage != null)
                                    {
                                        row.GetCell(i).SetCellValue(int.Parse(data[rr].NumberImage.ToString()));
                                    }
                                }
                                else if (i == 9)
                                {
                                    if (data[rr].TotalPrice != null)
                                    {
                                        row.GetCell(i).SetCellValue(double.Parse(data[rr].TotalPrice.ToString()));
                                    }
                                }
                                else if (i == 10)
                                {
                                    string status = "";
                                    if (data[rr].Status == (int)Const.NewsStatus.TEMP)
                                        status = "Bản nháp";
                                    else if (data[rr].Status == (int)Const.NewsStatus.NEW)
                                        status = "Bản viết mới";
                                    else if (data[rr].Status == (int)Const.NewsStatus.RE_NEW)
                                        status = "Bài viết bị trả lại";
                                    else if (data[rr].Status == (int)Const.NewsStatus.EDITING)
                                        status = "Chờ biên tập";
                                    else if (data[rr].Status == (int)Const.NewsStatus.EDITED)
                                        status = "Đã biên tập";
                                    else if (data[rr].Status == (int)Const.NewsStatus.RE_EDITED)
                                        status = "Biên tập lại";
                                    else if (data[rr].Status == (int)Const.NewsStatus.APPROVING)
                                        status = "Chờ duyệt";
                                    else if (data[rr].Status == (int)Const.NewsStatus.NOT_APPROVED)
                                        status = "Không duyệt";
                                    else if (data[rr].Status == (int)Const.NewsStatus.PUBLISHING)
                                        status = "Chờ xuất bản";
                                    else if (data[rr].Status == (int)Const.NewsStatus.NORMAL)
                                        status = "Xuất bản";
                                    else if (data[rr].Status == (int)Const.NewsStatus.UN_PUBLISH)
                                        status = "Gỡ xuất bản";
                                    row.GetCell(i).SetCellValue(status);
                                }
                                else if (i == 11)
                                {
                                    row.GetCell(i).SetCellValue(data[rr].Cmtnd);
                                }
                                else if (i == 12)
                                {
                                    row.GetCell(i).SetCellValue("");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("Exception:" + ex);
                        }
                    }

                }
                catch (Exception ee)
                {
                    log.Error("Exception:" + ee);
                }
            }

            sheet.ForceFormulaRecalculation = true;

            MemoryStream ms = new MemoryStream();

            workbook.Write(ms);

            return ms;
        }

        private static int CountImage(string htmlString)
        {
            int number = 0;
            //Bóc tách
            var document = new HtmlDocument
            {
                OptionOutputOriginalCase = true
            };
            document.LoadHtml(htmlString);
            try
            {
                var rootImg = document.DocumentNode.SelectNodes("//img").ToList();
                if (rootImg != null) number = rootImg.Count();
            }
            catch { }
            return number;
        }

        private bool NewsExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.News.Count(e => e.NewsId == id) > 0;
            }
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

    }
}


