using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

namespace IOITWebApp31.Controllers.ApiCMS
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LegalDocController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("legaldoc", "legaldoc");
        private static string functionCode = "QLCH";

        // GET: api/LegalDoc
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
                    def.meta = new Meta(200, "Success");
                    IQueryable<LegalDoc> data = db.LegalDoc.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("LegalDocId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("LegalDocId desc");
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
                            e.LegalDocId,
                            e.LegalDocRootId,
                            e.Code,
                            e.Name,
                            e.Url,
                            e.Contents,
                            e.DateIssue,
                            e.DateEffect,
                            e.Signer,
                            e.AgencyIssue,
                            e.YearIssue,
                            e.TypeText,
                            e.Field,
                            e.LanguageId,
                            e.Attactment,
                            e.CreatedAt,
                            e.UpdatedAt,
                            e.UserId,
                            e.Status,
                            e.Note,
                            e.AgencyIssued,
                            e.TichYeu,
                            listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.LegalDocId && cp.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_LEGAL_DOC && cp.Status != (int)Const.Status.DELETED).Select(p => new
                            {
                                p.CategoryId,
                                Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
                                Check = true
                            }).ToList(),
                            language = db.Language.Where(l => l.LanguageId == e.LanguageId).Select(l => new
                            {
                                l.LanguageId,
                                l.Flag,
                                l.Name,
                                l.Code
                            }).FirstOrDefault(),
                            listLanguage = db.LanguageMapping.Where(a => a.LanguageId1 == languageId && a.TargetId1 == e.LegalDocId
                                && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_LEGALDOC && a.Status != (int)Const.Status.DELETED).Select(a => new
                                {
                                    a.LanguageId2,
                                    a.TargetId2,
                                    Flag = db.Language.Where(l => l.LanguageId == a.LanguageId2).FirstOrDefault().Flag,
                                    //legalDoc = db.LegalDoc.Where(n => n.LegalDocId == a.TargetId2).Select(n => new {
                                    //    n.LegalDocId,
                                    //    n.LegalDocRootId,
                                    //    n.Code,
                                    //    n.Name,
                                    //    n.Contents,
                                    //    e.DateIssue,
                                    //    n.DateEffect,
                                    //    n.Signer,
                                    //    n.AgencyIssue,
                                    //    n.YearIssue,
                                    //    n.TypeText,
                                    //    n.Field,
                                    //    n.LanguageId,
                                    //    n.Attactment,
                                    //    n.CreatedAt,
                                    //    n.UpdatedAt,
                                    //    n.UserId,
                                    //    n.Status,
                                    //    n.Note,
                                    //    n.TichYeu,
                                    //    listCategory = db.CategoryMapping.Where(cp => cp.TargetId == n.LegalDocId && cp.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_LEGAL_DOC && cp.Status != (int)Const.Status.DELETED).Select(p => new {
                                    //        p.CategoryId,
                                    //        Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
                                    //        Check = true
                                    //    }).ToList(),
                                    //}).FirstOrDefault(),
                                }).ToList(),
                        }).ToListAsync();
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLegalDoc(int id, [FromBody] LegalDocDTO data)
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

                //if (data.Code == null || data.Code == "")
                //{
                //    def.meta = new Meta(211, "Code Null!");
                //    return Ok(def);
                //}
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        LegalDoc legalDoc = await db.LegalDoc.FindAsync(id);

                        if (legalDoc == null)
                        {
                            def.meta = new Meta(404, "Not Found");
                            return Ok(def);
                        }
                        string url = data.Url == null ? Utils.NonUnicode(data.Name) : data.Url;
                        url = url.Trim().ToLower();
                        if (legalDoc.Url != url)
                        {
                            //check xem trùng link ko
                            var pLink = db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (pLink != null)
                            {
                                def.meta = new Meta(232, "Link đã tồn tại!");
                                return Ok(def);
                            }
                            //cập nhật thay link cũ
                            var permaLink = db.PermaLink.Where(e => e.Slug == legalDoc.Url && e.TargetId == legalDoc.LegalDocId
                            && e.TargetType == (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS).FirstOrDefault();
                            if (permaLink != null)
                            {
                                permaLink.TargetId = legalDoc.LegalDocId;
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
                                permaLink1.Slug = legalDoc.Url;
                                permaLink1.TargetId = legalDoc.LegalDocId;
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
                            var permaLink = db.PermaLink.Where(e => e.Slug == legalDoc.Url && e.TargetId == legalDoc.LegalDocId
                            && e.TargetType == (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS).FirstOrDefault();
                            if (permaLink == null)
                            {
                                PermaLink permaLink1 = new PermaLink();
                                permaLink1.PermaLinkId = Guid.NewGuid();
                                permaLink1.Slug = legalDoc.Url;
                                permaLink1.TargetId = legalDoc.LegalDocId;
                                permaLink1.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAI_NEWS;
                                permaLink1.CreatedAt = DateTime.Now;
                                permaLink1.UpdatedAt = DateTime.Now;
                                permaLink1.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink1);
                                await db.SaveChangesAsync();
                            }
                        }
                        legalDoc.Code = data.Code;
                        legalDoc.Name = data.Name != null ? data.Name : "";
                        legalDoc.Url = data.Url;
                        legalDoc.DateIssue = data.DateIssue;
                        legalDoc.DateEffect = data.DateEffect;
                        legalDoc.Signer = data.Signer;
                        legalDoc.AgencyIssue = data.AgencyIssue;
                        legalDoc.YearIssue = data.YearIssue;
                        legalDoc.TypeText = data.TypeText;
                        legalDoc.Field = data.Field;
                        legalDoc.Attactment = data.Attactment;
                        legalDoc.AttactmentBit = data.AttactmentBit;
                        legalDoc.Contents = data.Contents;
                        legalDoc.Note = data.Note;
                        legalDoc.TichYeu = data.TichYeu != null ? data.TichYeu : "";
                        legalDoc.UserId = userId;
                        legalDoc.UpdatedAt = DateTime.Now;
                        legalDoc.Status = data.Status;
                        legalDoc.AgencyIssued = data.AgencyIssued;
                        legalDoc.Extension = data.Extension;
                        db.Entry(legalDoc).State = EntityState.Modified;

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
                                        categoryNewsMapping.TargetId = legalDoc.LegalDocId;
                                        categoryNewsMapping.TargetType = (int)Const.TypeCategoryMapping.CATEGORY_LEGAL_DOC;
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

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.LegalDocId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Sửa câu hỏi “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.LegalDocId.ToString();
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
                            if (!LegalDocExists(data.LegalDocId))
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

        [HttpPost]
        public async Task<IActionResult> PostLegalDoc([FromBody] LegalDocDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
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

                //if (data.Code == null || data.Code == "")
                //{
                //    def.meta = new Meta(211, "Name Null!");
                //    return Ok(def);
                //}
                using (var db = new IOITDataContext())
                {
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
                        //check đã thêm bài viết cho ngôn ngữ này chưa
                        var checkLang = await db.LanguageMapping.Where(e => e.LanguageId1 == languageId && e.LanguageId2 == data.LanguageId
                          && e.TargetId1 == data.LegalDocRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_LEGALDOC
                          && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();

                        if (checkLang != null)
                        {
                            def.meta = new Meta(228, "Language Exits!");
                            return Ok(def);
                        }
                        LegalDoc legalDoc = new LegalDoc();
                        legalDoc.LegalDocRootId = data.LegalDocRootId;
                        legalDoc.Code = data.Code;
                        legalDoc.Name = data.Name != null ? data.Name : "";
                        legalDoc.Url = data.Url;
                        legalDoc.DateIssue = data.DateIssue;
                        legalDoc.DateEffect = data.DateEffect;
                        legalDoc.Signer = data.Signer;
                        legalDoc.AgencyIssue = data.AgencyIssue;
                        legalDoc.YearIssue = data.YearIssue;
                        legalDoc.TypeText = data.TypeText;
                        legalDoc.Field = data.Field;
                        legalDoc.Attactment = data.Attactment;
                        legalDoc.AttactmentBit = data.AttactmentBit;
                        legalDoc.Contents = data.Contents;
                        legalDoc.Note = data.Note;
                        legalDoc.TichYeu = data.TichYeu != null ? data.TichYeu : "";
                        legalDoc.AgencyIssued = data.AgencyIssued;
                        legalDoc.Extension = data.Extension;
                        legalDoc.LanguageId = data.LanguageId;
                        legalDoc.UserId = userId;
                        legalDoc.CreatedAt = DateTime.Now;
                        legalDoc.UpdatedAt = DateTime.Now;
                        legalDoc.Status = (int)Const.Status.NORMAL;
                        await db.LegalDoc.AddAsync(legalDoc);
                        await db.SaveChangesAsync();
                        data.LegalDocId = legalDoc.LegalDocId;
                        if (data.LegalDocRootId == null) data.LegalDocRootId = legalDoc.LegalDocId;
                        try
                        {
                            await db.SaveChangesAsync();

                            //add category mapping
                            if (data.listCategory != null)
                            {
                                foreach (var item in data.listCategory)
                                {
                                    CategoryMapping categoryNewsMapping = new CategoryMapping();
                                    categoryNewsMapping.CategoryId = item.CategoryId;
                                    categoryNewsMapping.TargetId = data.LegalDocId;
                                    categoryNewsMapping.TargetType = (int)Const.TypeCategoryMapping.CATEGORY_LEGAL_DOC;
                                    categoryNewsMapping.Location = db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList() != null ? db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList().Count : 1;
                                    categoryNewsMapping.CreatedAt = DateTime.Now;
                                    categoryNewsMapping.Status = (int)Const.Status.NORMAL;
                                    await db.CategoryMapping.AddAsync(categoryNewsMapping);
                                }
                            }

                            data.listLanguage = new List<LanguageMappingDTO>();
                            if (data.LegalDocId > 0)
                            {
                                await db.SaveChangesAsync();
                                //Thêm ngôn ngữ (nếu bài viết mới thêm ko phải là ngôn ngữ mạc định)
                                //và có id bài viết gốc
                                if (data.LanguageId != languageId && data.LanguageId != null && data.LanguageId > 0
                                    && data.LegalDocRootId != null && data.LegalDocRootId > 0)
                                {
                                    var listLang = db.LanguageMapping.Where(e => e.LanguageId1 == languageId
                                      && e.TargetId1 == data.LegalDocRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                                      && e.Status != (int)Const.Status.DELETED).ToList();
                                    if (listLang.Count > 0)
                                    {
                                        LanguageMapping languageMapping = new LanguageMapping();
                                        languageMapping.LanguageId1 = languageId;
                                        languageMapping.LanguageId2 = data.LanguageId;
                                        languageMapping.TargetId1 = data.LegalDocRootId;
                                        languageMapping.TargetId2 = data.LegalDocId;
                                        languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_LEGALDOC;
                                        languageMapping.CreatedAt = DateTime.Now;
                                        languageMapping.Status = (int)Const.Status.NORMAL;
                                        await db.LanguageMapping.AddAsync(languageMapping);

                                        foreach (var item in listLang)
                                        {
                                            LanguageMapping languageMapping2 = new LanguageMapping();
                                            languageMapping2.LanguageId1 = item.LanguageId2;
                                            languageMapping2.LanguageId2 = data.LanguageId;
                                            languageMapping2.TargetId1 = item.TargetId2;
                                            languageMapping2.TargetId2 = data.LegalDocId;
                                            languageMapping2.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_LEGALDOC;
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
                                        languageMapping.TargetId1 = data.LegalDocRootId;
                                        languageMapping.TargetId2 = data.LegalDocId;
                                        languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_LEGALDOC;
                                        languageMapping.CreatedAt = DateTime.Now;
                                        languageMapping.Status = (int)Const.Status.NORMAL;
                                        await db.LanguageMapping.AddAsync(languageMapping);
                                    }
                                    await db.SaveChangesAsync();
                                    //numLang = listLang.Count + 2;
                                }
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Thêm câu hỏi “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.LegalDocId.ToString();
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
                                data.listLanguage = db.LanguageMapping.Where(a => a.LanguageId1 == languageId && a.TargetId1 == (int)data.LegalDocRootId
                                   && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_LEGALDOC && a.Status != (int)Const.Status.DELETED).Select(a => new LanguageMappingDTO
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
                            if (LegalDocExists(data.LegalDocId))
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
        public async Task<IActionResult> DeleteLegalDoc(int id)
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
                    LegalDoc data = await db.LegalDoc.FindAsync(id);
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

                            if (data.LegalDocId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Xoá câu hỏi “" + data.Name + "”";
                                action.ActionType = (int)Const.ActionType.DELETE;
                                action.TargetId = data.LegalDocId.ToString();
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
                            if (!LegalDocExists(data.LegalDocId))
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

        //API xóa danh sách văn bản pháp quy
        [HttpPut("deletes")]
        public async Task<IActionResult> DeleteLegalDocs([FromBody] int[] data)
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
                        LegalDoc legalDoc = await db.LegalDoc.FindAsync(data[i]);

                        if (legalDoc == null)
                        {
                            continue;
                        }

                        legalDoc.Status = (int)Const.Status.DELETED;
                        db.Entry(legalDoc).State = EntityState.Modified;

                        Models.EF.Action action = new Models.EF.Action();
                        action.ActionName = "Xoá câu hỏi“" + legalDoc.Name + "”";
                        action.ActionType = (int)Const.ActionType.DELETE;
                        action.TargetId = legalDoc.LegalDocId.ToString();
                        action.TargetName = legalDoc.Name;
                        action.CompanyId = companyId;
                        action.Logs = JsonConvert.SerializeObject(legalDoc);
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

        private bool LegalDocExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.LegalDoc.Count(e => e.LegalDocId == id) > 0;
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