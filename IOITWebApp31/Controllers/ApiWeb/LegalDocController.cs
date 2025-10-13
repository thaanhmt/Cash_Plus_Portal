using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class LegalDocController : ControllerBase
    {

        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
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
                            agencyIssue = db.TypeAttributeItem.Where(cp => cp.TypeAttributeItemId == e.AgencyIssue).Select(p => new
                            {
                                p.TypeAttributeItemId,
                                p.Name,
                            }).FirstOrDefault()
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
    }
}