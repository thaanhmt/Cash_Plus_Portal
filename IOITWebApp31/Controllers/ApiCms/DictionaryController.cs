using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiCMS
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private static string functionCode = "QLDIC";

        // GET: api/Dictionary
        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<Dictionary> data = db.Dictionary.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("DictionaryId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("DictionaryId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = await data.Select(paging.select).ToDynamicListAsync();
                    }
                    else
                        def.data = await data.ToListAsync();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }
        // GET detail: api/Dictionary/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDictionary(int id)
        {
            DefaultResponse def = new DefaultResponse();

            try
            {
                using (var db = new IOITDataContext())
                {
                    Dictionary data = await db.Dictionary.FindAsync(id);

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
            catch (Exception)
            {
                //log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }
        // POST: api/Dictionary
        [HttpPost]
        public async Task<IActionResult> PostDictionary(Dictionary data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Dictionary Dic = new Dictionary();
                        Dic.StringVn = data.StringVn;
                        Dic.StringEn = data.StringEn;
                        Dic.Note = data.Note;
                        Dic.CreatedAt = DateTime.Now;
                        Dic.UpdatedAt = DateTime.Now;
                        Dic.Status = (int)Const.Status.NORMAL;
                        db.Dictionary.Add(Dic);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.DictionaryId = Dic.DictionaryId;
                            if (data.DictionaryId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException)
                        {

                            transaction.Rollback();
                            if (DictionaryExists(data.DictionaryId))
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
            catch (Exception)
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // PUT: api/Dictionary/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBank(int id, Dictionary data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Dictionary Dic = db.Dictionary.Find(id);
                        if (Dic == null)
                        {
                            def.meta = new Meta(404, "Không tìm thấy dữ liệu");
                            return Ok(def);
                        }
                        Dic.StringVn = data.StringVn;
                        Dic.StringEn = data.StringEn;
                        Dic.Note = data.Note;
                        Dic.UpdatedAt = DateTime.Now;
                        Dic.Status = data.Status;
                        db.Dictionary.Update(Dic);
                        try
                        {
                            await db.SaveChangesAsync();
                            data.DictionaryId = Dic.DictionaryId;
                            if (data.DictionaryId > 0)
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
                            if (!DictionaryExists(data.DictionaryId))
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
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }
        // DELETE: api/Dictionary/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            DefaultResponse def = new DefaultResponse();

            try
            {
                using (var db = new IOITDataContext())
                {
                    Dictionary data = await db.Dictionary.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy từ điển");
                        return Ok(def);
                    }
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.Status = (int)Const.Status.DELETED;

                        db.Dictionary.Update(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DictionaryId > 0)
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
                            if (!DictionaryExists(data.DictionaryId))
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
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }
        private bool DictionaryExists(int id)
        {
            using (var db = new IOITDataContext())
            {
                return db.Dictionary.Count(e => e.DictionaryId == id) > 0;
            }
        }
    }
}