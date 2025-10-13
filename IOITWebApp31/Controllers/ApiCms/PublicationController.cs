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
    public class PublicationController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("publication", "publication");
        private static string functionCode = "QLAP";
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
                        IQueryable<Publication> data = db.Publication.Where(c => c.Status != (int)Const.Status.DELETED);

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
                        //string cat = "CategoryId";
                        def.meta = new Meta(200, "Success");
                        IQueryable<Publication> data = db.Publication.Where(c => c.Status != (int)Const.Status.DELETED);

                        if (paging.query != null)
                        {
                            paging.query = HttpUtility.UrlDecode(paging.query);
                        }
                        data = data.Where(paging.query);

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
                                data = data.OrderBy("PublicationId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                                data = data.OrderBy("PublicationId desc");
                            }
                        }

                        if (paging.select != null && paging.select != "")
                        {
                            paging.select = "publication(" + paging.select + ")";
                            paging.select = HttpUtility.UrlDecode(paging.select);
                            def.data = await data.Select(paging.select).ToDynamicListAsync();
                        }
                        else
                        {
                            def.data = await data.Select(e => new
                            {
                                e.PublicationId,
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
                                e.ViewNumber,
                                e.Location,
                                e.MetaTitle,
                                e.MetaKeyword,
                                e.MetaDescription,
                                e.CreatedAt,
                                e.UpdatedAt,
                                e.UserId,
                                e.Status,
                                e.Author,
                                e.NumberOfTopic,
                                e.PublishingYear,
                                e.Department,
                                e.IsLanguage,
                                e.TitleEn,
                                e.DescriptionEn,
                                e.ContentsEn,
                                e.DatePublic,
                                AuthorName = db.Author.Where(u => u.AuthorId == e.Author && u.Status != (int)Const.Status.DELETED).FirstOrDefault().Name,

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
                    var data = await db.Publication.Where(e => e.PublicationId == id).Select(e => new
                    {
                        e.PublicationId,
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
                        e.ViewNumber,
                        e.Location,
                        e.MetaTitle,
                        e.MetaKeyword,
                        e.MetaDescription,
                        e.CreatedAt,
                        e.UpdatedAt,
                        e.UserId,
                        e.Status,
                        e.Author,
                        e.NumberOfTopic,
                        e.PublishingYear,
                        e.Department,
                        e.IsLanguage,
                        e.TitleEn,
                        e.DescriptionEn,
                        e.ContentsEn,
                        e.DatePublic,
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
        // PUT: api/publication/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublication(int id, [FromBody] PublicationDTO data)
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
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Publication publication = await db.Publication.FindAsync(id);
                        if (publication == null)
                        {
                            def.meta = new Meta(404, "Not found!");
                            return Ok(def);
                        }

                        string url = data.Url == null ? Utils.NonUnicode(data.Title) : data.Url;
                        url = url.Trim().ToLower();
                        if (publication.Url != url)
                        {
                            //check xem trùng link ko
                            var pLink = db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (pLink != null)
                            {
                                def.meta = new Meta(232, "Link đã tồn tại!");
                                return Ok(def);
                            }
                            //cập nhật thay link cũ
                            var permaLink = db.PermaLink.Where(e => e.Slug == publication.Url).FirstOrDefault();
                            if (permaLink != null)
                            {
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
                                permaLink1.Slug = publication.Url;
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
                            var permaLink = db.PermaLink.Where(e => e.Slug == publication.Url).FirstOrDefault();
                            if (permaLink == null)
                            {
                                PermaLink permaLink1 = new PermaLink();
                                permaLink1.PermaLinkId = Guid.NewGuid();
                                permaLink1.Slug = publication.Url;
                                permaLink1.CreatedAt = DateTime.Now;
                                permaLink1.UpdatedAt = DateTime.Now;
                                permaLink1.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink1);
                                await db.SaveChangesAsync();
                            }
                        }

                        publication.Title = data.Title != null ? data.Title : "";
                        publication.Description = data.Description;
                        publication.Contents = data.Contents;
                        publication.Image = data.Image;
                        publication.Url = data.Url;
                        publication.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        publication.DateStartOn = data.DateStartOn != null ? data.DateStartOn : publication.DateStartActive;
                        publication.DateEndOn = data.DateEndOn != null ? data.DateEndOn : publication.DateStartActive.Value.AddYears(100);
                        publication.IsHome = data.IsHome != null ? data.IsHome : false;
                        publication.IsHot = data.IsHot != null ? data.IsHot : false;
                        publication.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                        publication.Location = data.Location;
                        publication.MetaTitle = data.MetaTitle;
                        publication.MetaKeyword = data.MetaKeyword;
                        publication.MetaDescription = data.MetaDescription;
                        publication.UpdatedAt = DateTime.Now;
                        publication.UserId = userId;
                        publication.Status = data.Status;
                        publication.Author = data.Author;
                        publication.NumberOfTopic = data.NumberOfTopic;
                        publication.PublishingYear = publication.DateStartActive.Value.Year;
                        publication.Department = data.Department;

                        publication.IsLanguage = data.IsLanguage;
                        publication.TitleEn = data.TitleEn;
                        publication.DescriptionEn = data.DescriptionEn;
                        publication.ContentsEn = data.ContentsEn;
                        publication.DatePublic = data.DatePublic;
                        db.Entry(publication).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();
                            if (publication.PublicationId > 0)
                            {
                                transaction.Commit();
                                //Create log
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Sửa ấn phẩm “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.PublicationId.ToString();
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
                            if (!NewsExists(data.PublicationId))
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
        // POST: api/Publication
        [HttpPost]
        public async Task<IActionResult> PostPublication(PublicationDTO data)
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

                if (data.Title == null || data.Title == "")
                {
                    def.meta = new Meta(211, "Title Null!");
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
                          && e.TargetId1 == data.PublicatioRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                          && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();

                        if (checkLang != null)
                        {
                            def.meta = new Meta(228, "Language Exits!");
                            return Ok(def);
                        }

                        //add news
                        Publication publication = new Publication();
                        publication.Title = data.Title != null ? data.Title : "";
                        publication.Description = data.Description == null ? "" : data.Description;
                        publication.Contents = data.Contents;
                        publication.Image = data.Image;
                        publication.Url = data.Url != null ? data.Url : Utils.NonUnicode(publication.Title);
                        publication.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        publication.DateStartOn = data.DateStartOn == null ? DateTime.Now : data.DateStartOn;
                        publication.DateEndOn = data.DateEndOn == null ? DateTime.Now.AddYears(100) : data.DateEndOn;
                        publication.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                        publication.IsHome = data.IsHome != null ? data.IsHome : false;
                        publication.IsHot = data.IsHot != null ? data.IsHot : false;
                        publication.Location = data.Location != null ? data.Location : 1;
                        publication.MetaTitle = data.MetaTitle != null ? data.MetaTitle : data.Title;
                        publication.MetaKeyword = data.MetaKeyword != null ? data.MetaKeyword : data.Title;
                        publication.MetaDescription = data.MetaDescription != null ? data.MetaDescription : data.Description;
                        publication.LanguageId = data.LanguageId != null ? data.LanguageId : languageId;
                        publication.CreatedAt = DateTime.Now;
                        publication.UpdatedAt = DateTime.Now;
                        publication.UserId = data.UserId;
                        publication.Status = data.Status;
                        publication.NumberOfTopic = data.NumberOfTopic;
                        publication.PublishingYear = publication.DateStartActive.Value.Year;
                        publication.Department = data.Department;
                        publication.Author = data.Author;//db.User.Where(u => u.UserId == publication.UserId).Select(u => u.FullName).FirstOrDefault();

                        publication.IsLanguage = data.IsLanguage;
                        publication.TitleEn = data.TitleEn;
                        publication.DescriptionEn = data.DescriptionEn;
                        publication.ContentsEn = data.ContentsEn;
                        publication.DatePublic = data.DatePublic;
                        data.PublicationId = publication.PublicationId;
                        await db.Publication.AddAsync(publication);
                        await db.SaveChangesAsync();

                        if (data.PublicatioRootId == null)
                            data.PublicatioRootId = publication.PublicationId;
                        try
                        {
                            await db.SaveChangesAsync();
                            data.PublicationId = publication.PublicationId;
                            if (data.PublicationId > 0)
                            {
                                //Thêm permalink
                                PermaLink permaLink = new PermaLink();
                                permaLink.PermaLinkId = Guid.NewGuid();
                                permaLink.Slug = publication.Url;
                                permaLink.TargetId = publication.PublicationId;
                                permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS;
                                permaLink.CreatedAt = DateTime.Now;
                                permaLink.UpdatedAt = DateTime.Now;
                                permaLink.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink);
                                await db.SaveChangesAsync();
                                //Thêm ngôn ngữ (nếu bài viết mới thêm ko phải là ngôn ngữ mạc định)
                                //và có id bài viết gốc
                                if (data.LanguageId != languageId && data.LanguageId != null && data.LanguageId > 0
                                    && data.PublicatioRootId != null && data.PublicatioRootId > 0)
                                {
                                    var listLang = db.LanguageMapping.Where(e => e.LanguageId1 == languageId
                                      && e.TargetId1 == data.PublicatioRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                                      && e.Status != (int)Const.Status.DELETED).ToList();
                                    if (listLang.Count > 0)
                                    {
                                        LanguageMapping languageMapping = new LanguageMapping();
                                        languageMapping.LanguageId1 = languageId;
                                        languageMapping.LanguageId2 = data.LanguageId;
                                        languageMapping.TargetId1 = data.PublicatioRootId;
                                        languageMapping.TargetId2 = data.PublicationId;
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
                                            languageMapping2.TargetId2 = data.PublicationId;
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
                                        languageMapping.TargetId1 = data.PublicatioRootId;
                                        languageMapping.TargetId2 = data.PublicationId;
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
                                action.ActionName = "Thêm ấn phẩm “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.PublicationId.ToString();
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
                                //data.listLanguage = db.LanguageMapping.Where(a => a.LanguageId1 == languageId && a.TargetId1 == (int)data.NewsRootId
                                //   && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS && a.Status != (int)Const.Status.DELETED).Select(a => new LanguageMappingDTO
                                //   {
                                //       LanguageMappingId = a.LanguageMappingId,
                                //       LanguageId1 = a.LanguageId1,
                                //       LanguageId2 = a.LanguageId2,
                                //       TargetId1 = a.TargetId1,
                                //       TargetId2 = a.TargetId2,
                                //       TargetType = a.TargetType,
                                //       CreatedAt = a.CreatedAt,
                                //       Status = a.Status
                                //   }).ToList();

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
                            if (NewsExists(data.PublicationId))
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

        // DELETE: api/Publication/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublication(int id)
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
                    Publication data = await db.Publication.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.Publication.Update(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.PublicationId > 0)
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
                                action.ActionName = "Xoá ấn phẩm “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.PublicationId.ToString();
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
                            if (!NewsExists(data.PublicationId))
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
                    //var UserRole = db.UserRole.Where(e => e.UserId == userId && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    //var CodeRoleId = UserRole.RoleId;
                    //var role = db.Role.Find(CodeRoleId);
                    //if (role.Code.Trim() != "ADMIN")
                    //{
                    //    def.meta = new Meta(222, "No permission");
                    //    return Ok(def);
                    //}
                    for (int i = 0; i < data.Count(); i++)
                    {


                        Publication publication = await db.Publication.FindAsync(data[i]);

                        if (publication == null)
                        {
                            continue;
                        }

                        publication.UserId = userId;
                        publication.UpdatedAt = DateTime.Now;
                        publication.Status = (int)Const.Status.DELETED;
                        db.Entry(publication).State = EntityState.Modified;

                        //Xóa ngôn ngữ map cùng
                        var mapLang = await db.LanguageMapping.Where(e => (e.TargetId1 == publication.PublicationId || e.TargetId2 == publication.PublicationId)
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
                        var permaLink = db.PermaLink.Where(e => e.Slug == publication.Url && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (permaLink != null)
                        {
                            permaLink.UpdatedAt = DateTime.Now;
                            permaLink.Status = (int)Const.Status.DELETED;
                            db.PermaLink.Update(permaLink);
                            await db.SaveChangesAsync();
                        }
                        Models.EF.Action action = new Models.EF.Action();
                        action.ActionName = "Xoá ấn phẩm “" + publication.Title + "”";
                        action.ActionType = (int)Const.ActionType.DELETE;
                        action.TargetId = publication.PublicationId.ToString();
                        action.TargetName = publication.Title;
                        action.CompanyId = companyId;
                        action.Logs = JsonConvert.SerializeObject(publication);
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

        private bool NewsExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.News.Count(e => e.NewsId == id) > 0;
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
                    Publication data = await db.Publication.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        if (isAll == 1)
                        {
                            data.UpdatedAt = DateTime.Now;
                            data.UserId = userId;
                            data.Status = (byte)stt;
                            db.Publication.Update(data);
                        }
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
                                    Publication dataLang = await db.Publication.Where(e => e.PublicationId == item.TargetId2 && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                                    if (dataLang != null)
                                    {
                                        dataLang.UpdatedAt = DateTime.Now;
                                        //dataLang.UserId = userId;
                                        dataLang.Status = (byte)stt;
                                        db.Publication.Update(dataLang);
                                    }
                                }
                                await db.SaveChangesAsync();
                            }

                            if (data.PublicationId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Đổi trạng thái ấn phẩm “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.PublicationId.ToString();
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
                            if (!NewsExists(data.PublicationId))
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
        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}