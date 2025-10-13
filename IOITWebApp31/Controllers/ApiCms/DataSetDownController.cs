using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiCms
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DataSetDownController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("dataset-down", "dataset-down");
        private static string functionCode = "LTDL";

        // GET: api/DataSetDown
        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
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
                try
                {
                    using (var db = new IOITDataContext())
                    {
                        def.meta = new Meta(200, "Success");
                        var data = (from dsv in db.DataSetDown
                                    join ds in db.DataSet on dsv.DataSetId equals ds.DataSetId
                                    where dsv.Status != (int)Const.Status.DELETED
                                    && ds.Status != (int)Const.Status.DELETED
                                    select new
                                    {
                                        dsv.DataSetDownId,
                                        dsv.DataSetId,
                                        dsv.ApplicationRangeId,
                                        dsv.ResearchAreaId,
                                        dsv.UnitId,
                                        dsv.CreatedAt,
                                        dsv.UpdatedAt,
                                        dsv.CreatedId,
                                        dsv.UpdatedId,
                                        dsv.Status,
                                        ds.Title,
                                        ds.AuthorName,
                                        ds.AuthorEmail,
                                        ds.AuthorPhone,
                                        ds.UserCreatedId,
                                        CreatedAtDS = ds.CreatedAt
                                    }).AsQueryable();
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
                                data = data.OrderBy("CreatedAt desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                                data = data.OrderBy("CreatedAt desc");
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
                                e.DataSetDownId,
                                e.DataSetId,
                                e.ApplicationRangeId,
                                e.ResearchAreaId,
                                e.UnitId,
                                e.CreatedAt,
                                e.UpdatedAt,
                                e.CreatedId,
                                e.UpdatedId,
                                e.Status,
                                e.Title,
                                e.AuthorName,
                                e.AuthorEmail,
                                e.AuthorPhone,
                                e.UserCreatedId,
                                e.CreatedAtDS,
                                userDown = db.Customer.Where(c => c.CustomerId == e.CreatedId).Select(c => new CustomerDT
                                {
                                    UserId = c.CustomerId,
                                    FullName = c.FullName,
                                }).FirstOrDefault(),
                                applicationRange = db.Category.Where(c => c.CategoryId == e.ApplicationRangeId
                                && c.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE).Select(c => new
                                {
                                    c.CategoryId,
                                    c.Name,
                                }).FirstOrDefault(),
                                researchArea = db.Category.Where(c => c.CategoryId == e.ResearchAreaId
                               && c.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA).Select(c => new
                               {
                                   c.CategoryId,
                                   c.Name,
                               }).FirstOrDefault(),
                                unit = db.Unit.Where(c => c.UnitId == e.UnitId).Select(c => new
                                {
                                    c.UnitId,
                                    c.Name,
                                }).FirstOrDefault(),
                                userCreated = db.Customer.Where(c => c.CustomerId == e.UserCreatedId).Select(c => new CustomerDT
                                {
                                    UserId = c.CustomerId,
                                    FullName = c.FullName,
                                }).FirstOrDefault(),
                            }).ToListAsync();


                        return Ok(def);
                    }
                }
                catch (Exception ex)
                {
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

        // GET: api/DataSetDown/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataSetDown(long id)
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
                    DataSetDown data = await db.DataSetDown.FindAsync(id);

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

        // PUT: api/DataSetDown/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataSetDown(Guid id, DataSetDown data)
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
                if ((userId != data.UpdatedId))
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        data.UpdatedId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = data.Status;
                        db.DataSetDown.Update(data);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DataSetDownId != null)
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
                            if (!DataSetDownExists(data.DataSetDownId))
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

        // POST: api/DataSetDown
        [HttpPost]
        public async Task<IActionResult> PostDataSetDown(DataSetDown data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
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
                if (userId != data.CreatedId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.DataSetDownId = Guid.NewGuid();
                        data.CreatedId = userId;
                        data.CreatedAt = DateTime.Now;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.NORMAL;
                        await db.DataSetDown.AddAsync(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DataSetDownId != null)
                                transaction.Commit();
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
                            if (DataSetDownExists(data.DataSetDownId))
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

        // DELETE: api/DataSetDown/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDataSetDown(Guid id)
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
                    DataSetDown data = await db.DataSetDown.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    if (userId != data.UpdatedId)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UpdatedId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.DataSetDown.Update(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DataSetDownId != null)
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
                            if (!DataSetDownExists(data.DataSetDownId))
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

        private bool DataSetDownExists(Guid id)
        {
            using (var db = new IOITDataContext())
            {
                return db.DataSetDown.Count(e => e.DataSetDownId == id) > 0;
            }
        }
    }
}
