using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
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
                //var identity = (ClaimsIdentity)User.Identity;
                //int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
                //string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
                //if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
                //{
                //    def.meta = new Meta(222, "No permission");
                //    return Ok(def);
                //}
                if (paging != null)
                {
                    using (var db = new IOITDataContext())
                    {
                        def.meta = new Meta(200, "Success");
                        IQueryable<Category> data = db.Category.Where(c => c.Status != (int)Const.Status.DELETED);

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

        // GET by Tree
        [HttpGet("GetByTree")]
        public IActionResult GetByTree([FromQuery] int[] arr, [FromQuery] int langId = -1)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            //var identity = (ClaimsIdentity)User.Identity;
            //string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            //int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
            //if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            //{
            //    def.meta = new Meta(222, "No permission");
            //    return Ok(def);
            //}

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

    }
}
