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

namespace IOITWebApp31.Controllers.ApiCms
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DataSetController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("dataset", "dataset");
        private static string functionCode = "QLDL";

        // GET: api/DataSet
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
                using (var db = new IOITDataContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<DataSet> data = db.DataSet.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("DataSetId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("DataSetId desc");
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

        [HttpPost("GetByPagePost")]
        public async Task<IActionResult> GetByPagePost([FromBody] FilterReport paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (access_key != "")
            {
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
                {
                    def.meta = new Meta(222, "No permission");
                    return Ok(def);
                }
            }
            if (paging != null)
            {
                try
                {
                    using (var db = new IOITDataContext())
                    {

                        //string cat = "CategoryId";
                        def.meta = new Meta(200, "Success");
                        //var dateStart = new DateTime(2000, 1, 1);
                        //var dateEnd = DateTime.Now;
                        //if (paging.DateStart != null)
                        //    dateStart = new DateTime(paging.DateStart.Value.Year, paging.DateStart.Value.Month, paging.DateStart.Value.Day, 0, 0, 0);
                        //if (paging.DateEnd != null)
                        //    dateEnd = new DateTime(paging.DateEnd.Value.Year, paging.DateEnd.Value.Month, paging.DateEnd.Value.Day, 23, 59, 59);

                        IQueryable<DataSetDTO> data = db.DataSet.Where(c =>
                        //c.CreatedAt >= dateStart && c.CreatedAt <= dateEnd
                        c.Status != (int)Const.Status.DELETED).Select(e => new DataSetDTO
                        {
                            DataSetId = e.DataSetId,
                            Title = e.Title,
                            AuthorName = e.AuthorName,
                            Type = e.Type,
                            UserCreatedId = e.UserCreatedId,
                            Status = e.Status,
                            IsHot = e.IsHot,
                        });

                        if (paging.ApplicationRangeId != -1 || paging.ResearchAreaId != -1
                            || paging.Extention != -1 || paging.UnitId != -1)
                        {
                            data = (from ds in db.DataSet
                                    join dar in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE)
                                    on ds.DataSetId equals dar.DataSetId
                                    join dra in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA)
                                    on ds.DataSetId equals dra.DataSetId
                                    join du in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_UNIT)
                                    on ds.DataSetId equals du.DataSetId
                                    join at in db.Attactment.Where(e => e.TargetType == (int)Const.TypeAttachment.FILE_DATASET)
                                    on ds.DataSetId equals at.TargetId
                                    where ds.Status != (int)Const.Status.DELETED
                                    && dar.Status != (int)Const.Status.DELETED
                                    && dra.Status != (int)Const.Status.DELETED
                                    && du.Status != (int)Const.Status.DELETED
                                    && at.Status != (int)Const.Status.DELETED
                                    //&& ds.CreatedAt >= dateStart && ds.CreatedAt <= dateEnd
                                    && ((dar.TargetId == paging.ApplicationRangeId || paging.ApplicationRangeId == -1))
                                    && ((dra.TargetId == paging.ResearchAreaId || paging.ResearchAreaId == -1))
                                    && ((du.TargetId == paging.UnitId || paging.UnitId == -1))
                                    && ((at.Extension == paging.Extention || paging.Extention == -1))
                                    group ds by new
                                    {
                                        ds.DataSetId,
                                        ds.Title,
                                        ds.AuthorName,
                                        ds.Type,
                                        ds.UserCreatedId,
                                        ds.Status,
                                        ds.IsHot,
                                    } into e
                                    select new DataSetDTO
                                    {
                                        DataSetId = e.Key.DataSetId,
                                        Title = e.Key.Title,
                                        AuthorName = e.Key.AuthorName,
                                        Type = e.Key.Type,
                                        UserCreatedId = e.Key.UserCreatedId,
                                        Status = e.Key.Status,
                                        IsHot = e.Key.IsHot,
                                    }).AsQueryable();
                        }

                        if (paging.query != null)
                        {
                            paging.query = HttpUtility.UrlDecode(paging.query);
                        }
                        data = data.Where(paging.query);

                        MetaDataDT metaDataDT = new MetaDataDT();
                        metaDataDT.Sum = data.Count();
                        //metaDataDT.Normal = data.Where(e => e.Status == 1).Count();
                        //metaDataDT.Temp = data.Where(e => e.Status == 10).Count();
                        //metaDataDT.Lock = data.Where(e => e.Status == 4).Count();

                        def.metadata = data.Count();

                        if (paging.page_size > 0)
                        {
                            if (paging.order_by != null && paging.order_by != "")
                            {
                                data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                            }
                            else
                            {
                                data = data.OrderBy("DataSetId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                            }
                        }
                        else
                        {
                            if (paging.order_by != null && paging.order_by != "")
                            {
                                data = data.OrderBy(paging.order_by);
                            }
                            else
                            {
                                data = data.OrderBy("DataSetId desc");
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
                            var listDatas = await data.ToListAsync();

                            foreach (var itemD in listDatas)
                            {
                                var itemData = await db.DataSet.Where(e => e.DataSetId == itemD.DataSetId).FirstOrDefaultAsync();
                                itemD.DataSetId = itemData.DataSetId;
                                itemD.Title = itemData.Title;
                                itemD.Description = itemData.Description;
                                itemD.Contents = itemData.Contents;
                                itemD.Image = itemData.Image;
                                itemD.Url = itemData.Url;
                                itemD.LinkVideo = itemData.LinkVideo;
                                itemD.AuthorName = itemData.AuthorName;
                                itemD.AuthorEmail = itemData.AuthorEmail;
                                itemD.AuthorPhone = itemData.AuthorPhone;
                                itemD.Version = itemData.Version;
                                itemD.Note = itemData.Note;
                                itemD.DateStartActive = itemData.DateStartActive;
                                itemD.DateStartOn = itemData.DateStartOn;
                                itemD.DateEndOn = itemData.DateEndOn;
                                itemD.DownNumber = itemData.DownNumber;
                                itemD.ViewNumber = itemData.ViewNumber;
                                itemD.Location = itemData.Location;
                                itemD.IsHot = itemData.IsHot;
                                itemD.Type = itemData.Type;
                                itemD.ApplicationRangeId = itemData.ApplicationRangeId;
                                itemD.ResearchAreaId = itemData.ResearchAreaId;
                                itemD.IsPublish = itemData.IsPublish;
                                itemD.ConfirmsPrivate = itemData.ConfirmsPrivate;
                                itemD.ConfirmsPublish = itemData.ConfirmsPublish;
                                itemD.MetaTitle = itemData.MetaTitle;
                                itemD.MetaKeyword = itemData.MetaKeyword;
                                itemD.MetaDescription = itemData.MetaDescription;
                                itemD.LanguageId = itemData.LanguageId;
                                itemD.WebsiteId = itemData.WebsiteId;
                                itemD.CompanyId = itemData.CompanyId;
                                itemD.UserCreatedId = itemData.UserCreatedId;
                                itemD.CreatedAt = itemData.CreatedAt;
                                itemD.UserEditedId = itemData.UserEditedId;
                                itemD.EditedAt = itemData.EditedAt;
                                itemD.UserApprovedId = itemData.UserApprovedId;
                                itemD.ApprovingAt = itemData.ApprovingAt;
                                itemD.ApprovedAt = itemData.ApprovedAt;
                                itemD.UserPublishedId = itemData.UserPublishedId;
                                itemD.PublishingAt = itemData.PublishingAt;
                                itemD.PublishedAt = itemData.PublishedAt;
                                itemD.UserId = itemData.UserId;
                                itemD.UpdatedAt = itemData.UpdatedAt;
                                itemD.Status = itemData.Status;

                                itemD.userApproved = db.Customer.Where(c => c.CustomerId == itemD.UserApprovedId).Select(c => new CustomerDT
                                {
                                    UserId = c.CustomerId,
                                    FullName = c.FullName,
                                    UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
                                }).FirstOrDefault();
                                itemD.userPublished = db.Customer.Where(c => c.CustomerId == itemD.UserPublishedId).Select(c => new CustomerDT
                                {
                                    UserId = c.CustomerId,
                                    FullName = c.FullName,
                                    UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
                                }).FirstOrDefault();
                                itemD.userCreated = db.Customer.Where(c => c.CustomerId == itemD.UserCreatedId).Select(c => new CustomerDT
                                {
                                    UserId = c.CustomerId,
                                    FullName = c.FullName,
                                    Email = c.Email,
                                    UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
                                }).FirstOrDefault();
                                itemD.applicationRange = await (from dsm in db.DataSetMapping.Where(c => c.DataSetId == itemData.DataSetId
                                                        && c.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE)
                                                                join cat in db.Category on dsm.TargetId equals cat.CategoryId
                                                                where dsm.DataSetId == itemData.DataSetId
                                                         && dsm.Status != (int)Const.Status.DELETED
                                                                select new CategoryDTL
                                                                {
                                                                    CategoryId = cat.CategoryId,
                                                                    Name = cat.Name,
                                                                    Code = cat.Code,
                                                                }).ToListAsync();

                                itemD.researchArea = await (from dsm in db.DataSetMapping.Where(c => c.DataSetId == itemData.DataSetId
                                                        && c.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA)
                                                            join cat in db.Category on dsm.TargetId equals cat.CategoryId
                                                            where dsm.DataSetId == itemData.DataSetId
                                                     && dsm.Status != (int)Const.Status.DELETED
                                                            select new CategoryDTL
                                                            {
                                                                CategoryId = cat.CategoryId,
                                                                Name = cat.Name,
                                                                Code = cat.Code,
                                                            }).ToListAsync();
                                itemD.unit = await (from dsm in db.DataSetMapping.Where(c => c.DataSetId == itemData.DataSetId
                                                        && c.TargetType == (int)Const.DataSetMapping.DATA_UNIT)
                                                    join cat in db.Unit on dsm.TargetId equals cat.UnitId
                                                    where dsm.DataSetId == itemData.DataSetId
                                             && dsm.Status != (int)Const.Status.DELETED
                                                    select new UnitDT
                                                    {
                                                        UnitId = cat.UnitId,
                                                        Name = cat.Name,
                                                        Code = cat.Code,
                                                    }).ToListAsync();

                                itemD.listFiles = await db.Attactment.Where(c => c.TargetId == itemData.DataSetId
                                                        && c.TargetType == (int)Const.TypeAttachment.FILE_DATASET
                                                        && c.Status != (int)Const.Status.DELETED).Select(c => new AttactmentDTO
                                                        {
                                                            AttactmentId = c.AttactmentId,
                                                            Name = c.Name,
                                                            TargetId = c.TargetId,
                                                            TargetType = c.TargetType,
                                                            Url = c.Url,
                                                            Thumb = c.Thumb,
                                                            Note = c.Note,
                                                            Extension = c.Extension,
                                                            ExtensionName = c.ExtensionName != null ? c.ExtensionName.Substring(1, c.ExtensionName.Length - 1).ToUpper() : "",
                                                            Storage = c.Storage,
                                                            CreatedId = c.CreatedId,
                                                            UpdatedId = c.UpdatedId,
                                                            CreatedAt = c.CreatedAt,
                                                            UpdatedAt = c.UpdatedAt,
                                                            Status = c.Status,
                                                        }).ToListAsync();


                            }
                            def.data = listDatas;
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

        // GET: api/DataSet/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataSet(long id)
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
                    DataSet data = await db.DataSet.FindAsync(id);

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

        // PUT: api/DataSet/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataSet(long id, DataSetDTO data)
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
                if (userId != data.UserId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                if (data.Title == null || data.Title == "")
                {
                    def.meta = new Meta(211, "Title Null!");
                    return Ok(def);
                }
                if (data.ApplicationRangeId == null || data.ApplicationRangeId == -1)
                {
                    def.meta = new Meta(2112, "ApplicationRange Null!");
                    return Ok(def);
                }
                if (data.ResearchAreaId == null || data.ResearchAreaId == -1)
                {
                    def.meta = new Meta(2112, "ResearchArea Null!");
                    return Ok(def);
                }
                if (data.Description == null || data.Description == "")
                {
                    def.meta = new Meta(2111, "Description Null!");
                    return Ok(def);
                }
                if (data.AuthorName == null || data.AuthorName == "")
                {
                    def.meta = new Meta(2111, "AuthorName Null!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        DataSet dataSet = await db.DataSet.FindAsync(id);
                        if (dataSet == null)
                        {
                            def.meta = new Meta(404, "Not found!");
                            return Ok(def);
                        }

                        string url = data.Url == null ? Utils.NonUnicode(data.Title) : data.Url;
                        url = url.Trim().ToLower();
                        if (dataSet.Url != url)
                        {
                            //check xem trùng link ko
                            var pLink = await db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                            while (pLink != null)
                            {
                                url += Utils.RandomString(5).Trim().ToLower();
                                pLink = await db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                            }
                            //cập nhật thay link cũ
                            var permaLink = db.PermaLink.Where(e => e.Slug == dataSet.Url && e.TargetId == dataSet.DataSetId
                            && e.TargetType == (int)Const.TypePermaLink.PERMALINK_DETAIL_DATASET).FirstOrDefault();
                            if (permaLink != null)
                            {
                                permaLink.TargetId = dataSet.DataSetId;
                                permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAIL_DATASET;
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
                                permaLink1.Slug = url;
                                permaLink1.TargetId = dataSet.DataSetId;
                                permaLink1.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAIL_DATASET;
                                permaLink1.CreatedAt = DateTime.Now;
                                permaLink1.UpdatedAt = DateTime.Now;
                                permaLink1.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink1);
                                await db.SaveChangesAsync();
                            }

                        }
                        else
                        {
                            var permaLink = db.PermaLink.Where(e => e.Slug == url && e.TargetId == dataSet.DataSetId
                            && e.TargetType == (int)Const.TypePermaLink.PERMALINK_DETAIL_DATASET).FirstOrDefault();
                            if (permaLink == null)
                            {
                                PermaLink permaLink1 = new PermaLink();
                                permaLink1.PermaLinkId = Guid.NewGuid();
                                permaLink1.Slug = url;
                                permaLink1.TargetId = dataSet.DataSetId;
                                permaLink1.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAIL_DATASET;
                                permaLink1.CreatedAt = DateTime.Now;
                                permaLink1.UpdatedAt = DateTime.Now;
                                permaLink1.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink1);
                                await db.SaveChangesAsync();
                            }
                        }

                        dataSet.Title = data.Title;
                        dataSet.Description = data.Description;
                        //dataSet.Contents = data.Contents;
                        dataSet.Image = data.Image;
                        dataSet.Image = data.Image;
                        dataSet.Url = url;
                        dataSet.LinkVideo = data.LinkVideo;
                        dataSet.AuthorName = data.AuthorName;
                        dataSet.AuthorEmail = data.AuthorEmail;
                        dataSet.AuthorPhone = data.AuthorPhone;
                        dataSet.Version = data.Version;
                        dataSet.Note = data.Note;
                        dataSet.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        dataSet.DateStartOn = data.DateStartOn != null ? data.DateStartOn : dataSet.DateStartActive;
                        dataSet.DateEndOn = data.DateEndOn != null ? data.DateEndOn : dataSet.DateStartActive.Value.AddYears(100);
                        dataSet.IsHot = data.IsHot != null ? data.IsHot : false;
                        dataSet.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                        dataSet.Location = data.Location;
                        dataSet.Type = data.Type;
                        dataSet.ApplicationRangeId = data.ApplicationRangeId;
                        dataSet.ResearchAreaId = data.ResearchAreaId;
                        dataSet.UnitId = data.UnitId;
                        dataSet.IsPublish = data.IsPublish;
                        dataSet.MetaTitle = data.MetaTitle;
                        dataSet.MetaKeyword = data.MetaKeyword;
                        dataSet.MetaDescription = data.MetaDescription;
                        dataSet.UpdatedAt = DateTime.Now;
                        dataSet.UserId = userId;
                        dataSet.Status = data.Status;

                        //if ((dataSet.Status == (int)Const.DataSetStatus.EDITING
                        //    || dataSet.Status == (int)Const.dataSetStatus.EDITED
                        //    || dataSet.Status == (int)Const.dataSetStatus.RE_EDITED)
                        //    && dataSet.EditingAt != null)
                        //{
                        //    dataSet.UserEditedId = userId;
                        //    dataSet.EditedAt = DateTime.Now;
                        //}
                        //else if ((dataSet.Status == (int)Const.dataSetStatus.APPROVING
                        //    || dataSet.Status == (int)Const.dataSetStatus.PUBLISHING
                        //    || dataSet.Status == (int)Const.dataSetStatus.NOT_APPROVED)
                        //    && dataSet.ApprovingAt != null)
                        //{
                        //    dataSet.UserApprovedId = userId;
                        //    dataSet.ApprovedAt = DateTime.Now;
                        //}
                        //else if ((dataSet.Status == (int)Const.dataSetStatus.NORMAL
                        //    || dataSet.Status == (int)Const.dataSetStatus.PUBLISHING
                        //    || dataSet.Status == (int)Const.dataSetStatus.UN_PUBLISH)
                        //    && dataSet.PublishingAt != null)
                        //{
                        //    dataSet.UserPublishedId = userId;
                        //    dataSet.PublishedAt = DateTime.Now;
                        //}
                        //if (dataSet.Status == (int)Const.dataSetStatus.EDITING && dataSet.EditingAt == null)
                        //{
                        //    dataSet.EditingAt = DateTime.Now;
                        //}
                        //if (dataSet.Status == (int)Const.dataSetStatus.APPROVING && dataSet.ApprovingAt == null)
                        //{
                        //    dataSet.ApprovingAt = DateTime.Now;
                        //}
                        //if (dataSet.Status == (int)Const.dataSetStatus.PUBLISHING && dataSet.PublishingAt == null)
                        //{
                        //    dataSet.PublishingAt = DateTime.Now;
                        //}

                        db.Entry(dataSet).State = EntityState.Modified;

                        //remove category mapping

                        //add category mapping
                        //if (data.listCategory != null)
                        //{
                        //    foreach (var item in data.listCategory)
                        //    {
                        //        CategoryMapping exist = db.CategoryMapping.Where(cm => cm.CategoryId == item.CategoryId && cm.TargetId == id && cm.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        //        if (exist == null)
                        //        {
                        //            if (item.Check == true)
                        //            {
                        //                CategoryMapping categorydataSetMapping = new CategoryMapping();
                        //                categorydataSetMapping.CategoryId = item.CategoryId;
                        //                categorydataSetMapping.TargetId = dataSet.dataSetId;
                        //                categorydataSetMapping.TargetType = (int)Const.TypeCategoryMapping.CATEGORY_dataSet;
                        //                categorydataSetMapping.Location = db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList() != null ? db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList().Count : 1;
                        //                categorydataSetMapping.Status = (int)Const.Status.NORMAL;
                        //                db.CategoryMapping.Add(categorydataSetMapping);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            if (item.Check != true)
                        //            {
                        //                exist.Status = (int)Const.Status.DELETED;
                        //                db.Entry(exist).State = EntityState.Modified;
                        //            }
                        //        }
                        //    }
                        //}

                        if (data.listFiles != null)
                        {
                            foreach (var item in data.listFiles)
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
                                    attactment.TargetId = dataSet.DataSetId;
                                    attactment.IsImageMain = item.IsImageMain;
                                    attactment.TargetType = (int)Const.TypeAttachment.FILE_DATASET;
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
                                        dataSet.Image = item.Url;
                                        db.Entry(dataSet).State = EntityState.Modified;
                                    }
                                }

                                if (item.IsImageMain == true && item.Status != (int)Const.Status.DELETED)
                                {
                                    dataSet.Image = item.Url;
                                }
                                db.Entry(dataSet).State = EntityState.Modified;
                            }
                        }

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DataSetId > 0)
                            {
                                ////Thêm vào đóng góp ý kiến
                                //if (data.Note != null && data.Note != "")
                                //{
                                //    dataSetNote dataSetNote = new dataSetNote();
                                //    dataSetNote.dataSetNoteId = Guid.NewGuid();
                                //    dataSetNote.dataSetId = data.dataSetId;
                                //    dataSetNote.Note = data.Note;
                                //    dataSetNote.UserId = userId;
                                //    dataSetNote.CreatedAt = DateTime.Now;
                                //    dataSetNote.UpdatedAt = DateTime.Now;
                                //    dataSetNote.Status = (int)Const.Status.NORMAL;
                                //    await db.dataSetNote.AddAsync(dataSetNote);
                                //    await db.SaveChangesAsync();
                                //}
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Biên tập, chỉnh sửa bộ dataset “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.DataSetId.ToString();
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
                            if (!DataSetExists(data.DataSetId, db))
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

        // POST: api/DataSet
        [HttpPost]
        public async Task<IActionResult> PostDataSet(DataSetDTO data)
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
                if (data.Title == null || data.Title == "")
                {
                    def.meta = new Meta(211, "Title Null!");
                    return Ok(def);
                }
                if (data.ApplicationRangeId == null || data.ApplicationRangeId == -1)
                {
                    def.meta = new Meta(2112, "ApplicationRange Null!");
                    return Ok(def);
                }
                if (data.ResearchAreaId == null || data.ResearchAreaId == -1)
                {
                    def.meta = new Meta(2112, "ResearchArea Null!");
                    return Ok(def);
                }
                if (data.Description == null || data.Description == "")
                {
                    def.meta = new Meta(2111, "Description Null!");
                    return Ok(def);
                }
                if (data.AuthorName == null || data.AuthorName == "")
                {
                    def.meta = new Meta(2111, "AuthorName Null!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //check xem trùng link ko, nếu trùng tự random ra link mới
                        string url = data.Url == null ? Utils.NonUnicode(data.Title) : data.Url;
                        url = url.Trim().ToLower();
                        var pLink = await db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                        while (pLink != null)
                        {
                            url += Utils.RandomString(5).Trim().ToLower();
                            pLink = await db.PermaLink.Where(e => e.Slug == url && url != "#" && url != "" && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                        }

                        //check đã thêm dữ liệu cho ngôn ngữ này chưa
                        var checkLang = await db.LanguageMapping.Where(e => e.LanguageId1 == languageId && e.LanguageId2 == data.LanguageId
                          && e.TargetId1 == data.DataSetRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_DATASET
                          && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();

                        if (checkLang != null)
                        {
                            def.meta = new Meta(228, "Đã thêm dữ liệu cho ngôn ngữ này!");
                            return Ok(def);
                        }

                        //add data
                        DataSet dataSet = new DataSet();
                        dataSet.Title = data.Title;
                        dataSet.Description = data.Description == null ? "" : data.Description;
                        dataSet.Contents = data.Contents;
                        dataSet.Image = data.Image;
                        dataSet.Url = url;
                        dataSet.LinkVideo = data.LinkVideo;
                        dataSet.AuthorName = data.AuthorName;
                        dataSet.AuthorEmail = data.AuthorEmail;
                        dataSet.AuthorPhone = data.AuthorPhone;
                        dataSet.Version = data.Version;
                        dataSet.Note = data.Note;
                        dataSet.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        dataSet.DateStartOn = data.DateStartOn == null ? DateTime.Now : data.DateStartOn;
                        dataSet.DateEndOn = data.DateEndOn == null ? DateTime.Now.AddYears(100) : data.DateEndOn;
                        dataSet.DownNumber = data.DownNumber != null ? data.DownNumber : 0;
                        dataSet.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                        dataSet.Location = data.Location != null ? data.Location : 1;
                        dataSet.IsHot = data.IsHot != null ? data.IsHot : false;
                        dataSet.Type = data.Type != null ? data.Type : 1;
                        dataSet.ApplicationRangeId = data.ApplicationRangeId;
                        dataSet.ResearchAreaId = data.ResearchAreaId;
                        dataSet.UnitId = data.UnitId;
                        dataSet.IsPublish = data.IsPublish;
                        dataSet.MetaTitle = data.MetaTitle != null ? data.MetaTitle : data.Title;
                        dataSet.MetaKeyword = data.MetaKeyword != null ? data.MetaKeyword : data.Title;
                        dataSet.MetaDescription = data.MetaDescription != null ? data.MetaDescription : data.Description;
                        dataSet.LanguageId = data.LanguageId != null ? data.LanguageId : languageId;
                        dataSet.WebsiteId = data.WebsiteId != null ? data.WebsiteId : websiteId;
                        dataSet.CompanyId = data.CompanyId != null ? data.CompanyId : companyId;
                        dataSet.CreatedAt = DateTime.Now;
                        dataSet.UpdatedAt = DateTime.Now;
                        dataSet.UserCreatedId = userId;
                        dataSet.UserId = userId;
                        dataSet.Status = data.Status;
                        await db.DataSet.AddAsync(dataSet);
                        await db.SaveChangesAsync();

                        data.DataSetId = dataSet.DataSetId;
                        if (data.DataSetRootId == null) data.DataSetRootId = dataSet.DataSetId;


                        //add list files

                        if (data.listFiles != null)
                        {
                            foreach (var item in data.listFiles)
                            {
                                Attactment attactment = new Attactment();
                                attactment.Name = item.Name;
                                attactment.TargetId = dataSet.DataSetId;
                                attactment.IsImageMain = item.IsImageMain;
                                attactment.TargetType = (int)Const.TypeAttachment.FILE_DATASET;
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
                                    dataSet.Image = item.Url;
                                }
                            }
                            db.Entry(dataSet).State = EntityState.Modified;
                        }

                        try
                        {
                            await db.SaveChangesAsync();
                            data.listLanguage = new List<LanguageMappingDTO>();
                            if (data.DataSetId > 0)
                            {
                                //Thêm permalink
                                PermaLink permaLink = new PermaLink();
                                permaLink.PermaLinkId = Guid.NewGuid();
                                permaLink.Slug = dataSet.Url;
                                permaLink.TargetId = dataSet.DataSetId;
                                permaLink.TargetType = (int)Const.TypePermaLink.PERMALINK_DETAIL_DATASET;
                                permaLink.CreatedAt = DateTime.Now;
                                permaLink.UpdatedAt = DateTime.Now;
                                permaLink.Status = (int)Const.Status.NORMAL;
                                await db.PermaLink.AddAsync(permaLink);
                                await db.SaveChangesAsync();
                                //Thêm ngôn ngữ (nếu bài viết mới thêm ko phải là ngôn ngữ mạc định)
                                //và có id bài viết gốc
                                if (data.LanguageId != languageId && data.LanguageId != null && data.LanguageId > 0
                                    && data.DataSetRootId != null && data.DataSetRootId > 0)
                                {
                                    var listLang = db.LanguageMapping.Where(e => e.LanguageId1 == languageId
                                      && e.TargetId1 == data.DataSetRootId && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_DATASET
                                      && e.Status != (int)Const.Status.DELETED).ToList();
                                    if (listLang.Count > 0)
                                    {
                                        LanguageMapping languageMapping = new LanguageMapping();
                                        languageMapping.LanguageId1 = languageId;
                                        languageMapping.LanguageId2 = data.LanguageId;
                                        languageMapping.TargetId1 = data.DataSetRootId;
                                        languageMapping.TargetId2 = data.DataSetId;
                                        languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_DATASET;
                                        languageMapping.CreatedAt = DateTime.Now;
                                        languageMapping.Status = (int)Const.Status.NORMAL;
                                        await db.LanguageMapping.AddAsync(languageMapping);

                                        foreach (var item in listLang)
                                        {
                                            LanguageMapping languageMapping2 = new LanguageMapping();
                                            languageMapping2.LanguageId1 = item.LanguageId2;
                                            languageMapping2.LanguageId2 = data.LanguageId;
                                            languageMapping2.TargetId1 = item.TargetId2;
                                            languageMapping2.TargetId2 = data.DataSetId;
                                            languageMapping2.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_DATASET;
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
                                        languageMapping.TargetId1 = data.DataSetRootId;
                                        languageMapping.TargetId2 = data.DataSetId;
                                        languageMapping.TargetType = (int)Const.TypeLanguageMapping.LANGUAGE_DATASET;
                                        languageMapping.CreatedAt = DateTime.Now;
                                        languageMapping.Status = (int)Const.Status.NORMAL;
                                        await db.LanguageMapping.AddAsync(languageMapping);
                                    }
                                    await db.SaveChangesAsync();
                                    //numLang = listLang.Count + 2;
                                }
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Thêm bộ dữ liệu “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.CREATE;
                                action.TargetId = data.DataSetId.ToString();
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
                                data.listLanguage = db.LanguageMapping.Where(a => a.LanguageId1 == languageId && a.TargetId1 == (int)data.DataSetRootId
                                   && a.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_DATASET && a.Status != (int)Const.Status.DELETED).Select(a => new LanguageMappingDTO
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
                            if (DataSetExists(data.DataSetId, db))
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

        // DELETE: api/DataSet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDataSet(long id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "Bạn không có quyền xóa bộ dữ liệu!");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    DataSet data = await db.DataSet.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    //if ((userId != data.UserId))
                    //{
                    //    def.meta = new Meta(404, "Not Found");
                    //    return Ok(def);
                    //}
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UserId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.DataSet.Update(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DataSetId > 0)
                            {
                                //Xóa các dữ liệu liên quan
                                //Xóa view
                                var listViews = await db.DataSetView.Where(e => e.DataSetId == id && e.Status != (int)Const.Status.DELETED).ToListAsync();
                                foreach (var item in listViews)
                                {
                                    item.UpdatedId = userId;
                                    item.UpdatedAt = DateTime.Now;
                                    item.Status = (int)Const.Status.DELETED;
                                }
                                db.DataSetView.UpdateRange(listViews);
                                //Xóa  down
                                var listDowns = await db.DataSetDown.Where(e => e.DataSetId == id && e.Status != (int)Const.Status.DELETED).ToListAsync();
                                foreach (var item in listDowns)
                                {
                                    item.UpdatedId = userId;
                                    item.UpdatedAt = DateTime.Now;
                                    item.Status = (int)Const.Status.DELETED;
                                }
                                db.DataSetDown.UpdateRange(listDowns);
                                //Xóa map
                                var listMaps = await db.DataSetMapping.Where(e => e.DataSetId == id && e.Status != (int)Const.Status.DELETED).ToListAsync();
                                foreach (var item in listMaps)
                                {
                                    item.UserId = userId;
                                    //item.UpdatedAt = DateTime.Now;
                                    item.Status = (int)Const.Status.DELETED;
                                }
                                db.DataSetMapping.UpdateRange(listMaps);
                                //Xóa duyệt
                                var listApproved = await db.DataSetApproved.Where(e => e.DataSetId == id && e.Status != (int)Const.Status.DELETED).ToListAsync();
                                foreach (var item in listApproved)
                                {
                                    item.UpdatedId = userId;
                                    item.UpdatedAt = DateTime.Now;
                                    item.Status = (int)Const.Status.DELETED;
                                }
                                db.DataSetApproved.UpdateRange(listApproved);
                                //Xóa file dính kèm
                                var listAttactments = await db.Attactment.Where(e => e.TargetId == id
                                && e.TargetType == (int)Const.TypeAttachment.FILE_DATASET && e.Status != (int)Const.Status.DELETED).ToListAsync();
                                foreach (var item in listAttactments)
                                {
                                    item.UpdatedId = userId;
                                    item.UpdatedAt = DateTime.Now;
                                    item.Status = (int)Const.Status.DELETED;
                                }
                                db.Attactment.UpdateRange(listAttactments);
                                await db.SaveChangesAsync();

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
                            if (!DataSetExists(data.DataSetId, db))
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

        [HttpPut("updateHot/{id}")]
        public async Task<IActionResult> UpdateHot(long id, DataSetDTO data)
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
                if (userId != data.UserId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        DataSet dataSet = await db.DataSet.FindAsync(id);
                        if (dataSet == null)
                        {
                            def.meta = new Meta(404, "Not found!");
                            return Ok(def);
                        }
                        dataSet.IsHot = data.IsHot != null ? data.IsHot : false;
                        db.Entry(dataSet).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DataSetId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Cập nhật bộ dataset “" + data.Title + "” là: " + (data.IsHot == true ? "Nổi bật" : "Không nổi bật");
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.DataSetId.ToString();
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
                            if (!DataSetExists(data.DataSetId, db))
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

        private bool DataSetExists(long id, IOITDataContext db)
        {
            return db.DataSet.Count(e => e.DataSetId == id) > 0;
        }
    }
}
