using HtmlAgilityPack;
using IOITWebApp31.Models;
using IOITWebApp31.Models.Common;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers.ApiWeb
{
    //[Authorize]
    [Route("web/[controller]")]
    [ApiController]
    public class DataSetController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("dataset", "dataset");
        private static string functionCode = "DSDL";
        private readonly IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;

        public DataSetController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        // GET: api/DataSet
        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            //var identity = (ClaimsIdentity)User.Identity;
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
                        def.data = await data.Select(e => new
                        {
                            e.DataSetId,
                            e.Title,
                            e.Status,
                            e.Type,
                            e.IsPublish,
                            applicationRange = db.Category.Where(c => c.CategoryId == e.ApplicationRangeId).Select(c => new
                            {
                                c.CategoryId,
                                c.Name,
                                c.Code
                            }).FirstOrDefault(),
                            researchArea = db.Category.Where(c => c.CategoryId == e.ResearchAreaId).Select(c => new
                            {
                                c.CategoryId,
                                c.Name,
                                c.Code
                            }).FirstOrDefault(),
                            unit = db.Unit.Where(c => c.UnitId == e.UnitId).Select(c => new
                            {
                                c.UnitId,
                                c.Name
                            }).FirstOrDefault(),
                            user = db.Customer.Where(c => c.CustomerId == e.UserCreatedId).Select(c => new
                            {
                                UserCreatedId = c.CustomerId,
                                c.FullName
                            }).FirstOrDefault(),
                            listFiles = db.Attactment.Where(c => c.TargetId == e.DataSetId && c.TargetType == (int)Const.TypeAttachment.FILE_DATASET).Select(c => new
                            {
                                c.AttactmentId,
                                c.TargetId,
                                c.TargetType,
                            }).ToList(),
                            e.AuthorName,
                            e.AuthorEmail,
                            e.AuthorPhone,
                            e.CreatedAt,
                            e.UpdatedAt
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

        [HttpPost("GetByPagePost")]
        public async Task<IActionResult> GetByPagePost([FromBody] FilterReport paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            int type = int.Parse(identity.Claims.Where(c => c.Type == "Type").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            string listUnits = identity.Claims.Where(c => c.Type == "ListUnits").Select(c => c.Value).SingleOrDefault();
            if (access_key != "")
            {
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
                {
                    def.meta = new Meta(222, "Bạn không có quyền xem bộ dữ liệu!");
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
                        var dateStart = new DateTime(2000, 1, 1);
                        var dateEnd = DateTime.Now;
                        if (paging.DateStart != null)
                            dateStart = new DateTime(paging.DateStart.Value.Year, paging.DateStart.Value.Month, paging.DateStart.Value.Day, 0, 0, 0);
                        if (paging.DateEnd != null)
                            dateEnd = new DateTime(paging.DateEnd.Value.Year, paging.DateEnd.Value.Month, paging.DateEnd.Value.Day, 23, 59, 59);

                        IQueryable<DataSetDTO> data = db.DataSet.Where(c =>
                        c.CreatedAt >= dateStart && c.CreatedAt <= dateEnd
                        && c.Status != (int)Const.Status.DELETED).Select(e => new DataSetDTO
                        {
                            DataSetId = e.DataSetId,
                            UserCreatedId = e.UserCreatedId,
                            Title = e.Title,
                            AuthorName = e.AuthorName,
                            Type = e.Type,
                            Status = e.Status,
                        });
                        string[] listU = listUnits.Split("-");
                        //nếu là cá nhân thì chỉ lấy bài của mình
                        if (type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL)
                        {
                            data = data.Where(e => e.UserCreatedId == userId).AsQueryable();
                        }
                        else
                        {

                            //Nếu là tổ chức thì lấy thêm bài của các user thuốc tổ chức mình quản lý
                            data = (from ds in db.DataSet.AsQueryable()
                                    join du in db.DataSetMapping.AsQueryable().Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_UNIT)
                                    on ds.DataSetId equals du.DataSetId
                                    where ds.Status != (int)Const.Status.DELETED
                                    && du.Status != (int)Const.Status.DELETED
                                    && ds.CreatedAt >= dateStart && ds.CreatedAt <= dateEnd
                                    && (listU.Contains(du.TargetId.Value.ToString()) || ds.UserCreatedId == userId)
                                    group ds by new
                                    {
                                        ds.DataSetId,
                                        ds.UserCreatedId,
                                        ds.Title,
                                        ds.AuthorName,
                                        ds.Type,
                                        ds.Status,
                                    } into e
                                    select new DataSetDTO
                                    {
                                        DataSetId = e.Key.DataSetId,
                                        UserCreatedId = e.Key.UserCreatedId,
                                        Title = e.Key.Title,
                                        AuthorName = e.Key.AuthorName,
                                        Type = e.Key.Type,
                                        Status = e.Key.Status,
                                    }).AsQueryable();
                        }

                        if (paging.ApplicationRangeId != -1 || paging.ResearchAreaId != -1 || paging.Extention != -1)
                        {
                            if (type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL)
                            {
                                data = (from ds in db.DataSet
                                        join dar in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE)
                                        on ds.DataSetId equals dar.DataSetId
                                        join dra in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA)
                                        on ds.DataSetId equals dra.DataSetId
                                        join at in db.Attactment.Where(e => e.TargetType == (int)Const.TypeAttachment.FILE_DATASET)
                                        on ds.DataSetId equals at.TargetId
                                        where ds.Status != (int)Const.Status.DELETED
                                        && dar.Status != (int)Const.Status.DELETED
                                        && dra.Status != (int)Const.Status.DELETED
                                        && at.Status != (int)Const.Status.DELETED
                                        && ds.UserCreatedId == userId
                                        && ds.CreatedAt >= dateStart && ds.CreatedAt <= dateEnd
                                        && ((dar.TargetId == paging.ApplicationRangeId || paging.ApplicationRangeId == -1))
                                        && ((dra.TargetId == paging.ResearchAreaId || paging.ResearchAreaId == -1))
                                        && ((at.Extension == paging.Extention || paging.Extention == -1))
                                        group ds by new
                                        {
                                            ds.DataSetId,
                                            ds.UserCreatedId,
                                            ds.Title,
                                            ds.AuthorName,
                                            ds.Type,
                                            ds.Status,
                                        } into e
                                        select new DataSetDTO
                                        {
                                            DataSetId = e.Key.DataSetId,
                                            UserCreatedId = e.Key.UserCreatedId,
                                            Title = e.Key.Title,
                                            AuthorName = e.Key.AuthorName,
                                            Type = e.Key.Type,
                                            Status = e.Key.Status,
                                        }).AsQueryable();
                            }
                            else
                            {
                                data = (from ds in db.DataSet.AsQueryable()
                                        join dar in db.DataSetMapping.AsQueryable().Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE)
                                        on ds.DataSetId equals dar.DataSetId
                                        join dra in db.DataSetMapping.AsQueryable().Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA)
                                        on ds.DataSetId equals dra.DataSetId
                                        join du in db.DataSetMapping.AsQueryable().Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_UNIT)
                                        on ds.DataSetId equals du.DataSetId
                                        join at in db.Attactment.Where(e => e.TargetType == (int)Const.TypeAttachment.FILE_DATASET)
                                        on ds.DataSetId equals at.TargetId
                                        where ds.Status != (int)Const.Status.DELETED
                                        && dar.Status != (int)Const.Status.DELETED
                                        && dra.Status != (int)Const.Status.DELETED
                                        && at.Status != (int)Const.Status.DELETED
                                        && (listU.Contains(du.TargetId.Value.ToString()) || ds.UserCreatedId == userId)
                                        && ds.CreatedAt >= dateStart && ds.CreatedAt <= dateEnd
                                        && ((dar.TargetId == paging.ApplicationRangeId || paging.ApplicationRangeId == -1))
                                        && ((dra.TargetId == paging.ResearchAreaId || paging.ResearchAreaId == -1))
                                        && ((at.Extension == paging.Extention || paging.Extention == -1))
                                        group ds by new
                                        {
                                            ds.DataSetId,
                                            ds.UserCreatedId,
                                            ds.Title,
                                            ds.AuthorName,
                                            ds.Type,
                                            ds.Status,
                                        } into e
                                        select new DataSetDTO
                                        {
                                            DataSetId = e.Key.DataSetId,
                                            UserCreatedId = e.Key.UserCreatedId,
                                            Title = e.Key.Title,
                                            AuthorName = e.Key.AuthorName,
                                            Type = e.Key.Type,
                                            Status = e.Key.Status,
                                        }).AsQueryable();
                            }
                        }

                        if (paging.query != null)
                        {
                            paging.query = HttpUtility.UrlDecode(paging.query);
                        }
                        data = data.Where(paging.query);

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
                                itemD.LicenseId = itemData.LicenseId;
                                //applicationRange = db.DataSetMapping.Where(c => c.DataSetId == e.DataSetId
                                //&& c.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE
                                //&& c.Status != (int)Const.Status.DELETED).Select(c => new CategoryDTL
                                //{
                                //    CategoryId = (int)c.TargetId,
                                //}).ToList(),
                                //researchArea = db.DataSetMapping.Where(c => c.DataSetId == e.DataSetId
                                //&& c.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA
                                //&& c.Status != (int)Const.Status.DELETED).Select(c => new CategoryDTL
                                //{
                                //    CategoryId = (int)c.TargetId,
                                //}).ToList(),
                                //unit = db.Unit.Where(c => c.UnitId == e.UnitId).Select(c => new UnitDT
                                //{
                                //    UnitId = c.UnitId,
                                //    Name = c.Name
                                //}).ToList(),
                                itemD.userCreated = await db.Customer.Where(c => c.CustomerId == itemData.UserCreatedId).Select(c => new CustomerDT
                                {
                                    UserId = c.CustomerId,
                                    FullName = c.FullName,
                                    Email = c.Email,
                                    UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
                                }).FirstOrDefaultAsync();
                                itemD.licecses = await db.News.Where(c => c.NewsId == itemData.LicenseId).Select(c => new NewsDTO
                                {
                                    NewsId = c.NewsId,
                                    Title = c.Title,
                                    Url = c.Url,
                                }).FirstOrDefaultAsync();
                                //userApproved = db.Customer.Where(c => c.CustomerId == e.UserApprovedId).Select(c => new CustomerDT
                                //{
                                //    UserId = c.CustomerId,
                                //    FullName = c.FullName,
                                //    UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
                                //}).FirstOrDefault(),
                                //userPublished = db.Customer.Where(c => c.CustomerId == e.UserPublishedId).Select(c => new CustomerDT
                                //{
                                //    UserId = c.CustomerId,
                                //    FullName = c.FullName,
                                //    UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
                                //}).FirstOrDefault(),
                                //listFiles = db.Attactment.Where(c => c.TargetId == e.DataSetId
                                //&& c.TargetType == (int)Const.TypeAttachment.FILE_DATASET).Select(c => new AttactmentDTO
                                //{
                                //    AttactmentId = c.AttactmentId,
                                //    Name = c.Name,
                                //    TargetId = c.TargetId,
                                //    TargetType = c.TargetType,
                                //    Url = c.Url,
                                //    Thumb = c.Thumb,
                                //    Note = c.Note,
                                //    Extension = c.Extension,
                                //    Storage = c.Storage,
                                //    CreatedId = c.CreatedId,
                                //    UpdatedId = c.UpdatedId,
                                //    CreatedAt = c.CreatedAt,
                                //    UpdatedAt = c.UpdatedAt,
                                //    Status = c.Status,
                                //}).ToList()
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
                                                            Storage = c.Storage,
                                                            CreatedId = c.CreatedId,
                                                            UpdatedId = c.UpdatedId,
                                                            CreatedAt = c.CreatedAt,
                                                            UpdatedAt = c.UpdatedAt,
                                                            Status = c.Status,
                                                        }).ToListAsync();

                                //if (itemD.applicationRange != null)
                                //{
                                //    var listAR = await db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE).ToListAsync();
                                //    foreach (var item in itemD.applicationRange)
                                //    {
                                //        var ar = listAR.Where(e => e.CategoryId == item.CategoryId).FirstOrDefault();
                                //        if (ar != null)
                                //        {
                                //            item.Name = ar.Name;
                                //            item.Code = ar.Code;
                                //            item.Url = ar.Url;
                                //        }
                                //    }
                                //}
                                //if (itemD.researchArea != null)
                                //{
                                //    var listRA = await db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA).ToListAsync();
                                //    foreach (var item in itemD.researchArea)
                                //    {
                                //        var ra = listRA.Where(e => e.CategoryId == item.CategoryId).FirstOrDefault();
                                //        if (ra != null)
                                //        {
                                //            item.Name = ra.Name;
                                //            item.Code = ra.Code;
                                //            item.Url = ra.Url;
                                //        }
                                //    }
                                //}
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
                def.meta = new Meta(222, "Bạn không có quyền xem bộ dữ liệu!");
                return Ok(def);
            }
            try
            {
                using (var db = new IOITDataContext())
                {
                    var data = await db.DataSet.Where(e => e.DataSetId == id
                    && e.Status != (int)Const.Status.DELETED).Select(e => new DataSetDTO
                    {
                        DataSetId = e.DataSetId,
                        Title = e.Title,
                        Description = e.Description,
                        Contents = e.Contents,
                        Image = e.Image,
                        Url = e.Url,
                        LinkVideo = e.LinkVideo,
                        AuthorName = e.AuthorName,
                        AuthorEmail = e.AuthorEmail,
                        AuthorPhone = e.AuthorPhone,
                        Version = e.Version,
                        Note = e.Note,
                        DateStartActive = e.DateStartActive,
                        DateStartOn = e.DateStartOn,
                        DateEndOn = e.DateEndOn,
                        DownNumber = e.DownNumber,
                        ViewNumber = e.ViewNumber,
                        Location = e.Location,
                        IsHot = e.IsHot,
                        Type = e.Type,
                        ApplicationRangeId = e.ApplicationRangeId,
                        ResearchAreaId = e.ResearchAreaId,
                        IsPublish = e.IsPublish,
                        ConfirmsPrivate = e.ConfirmsPrivate,
                        ConfirmsPublish = e.ConfirmsPublish,
                        MetaTitle = e.MetaTitle,
                        MetaKeyword = e.MetaKeyword,
                        MetaDescription = e.MetaDescription,
                        LanguageId = e.LanguageId,
                        WebsiteId = e.WebsiteId,
                        CompanyId = e.CompanyId,
                        UserCreatedId = e.UserCreatedId,
                        CreatedAt = e.CreatedAt,
                        UserEditedId = e.UserEditedId,
                        EditedAt = e.EditedAt,
                        UserApprovedId = e.UserApprovedId,
                        ApprovingAt = e.ApprovingAt,
                        ApprovedAt = e.ApprovedAt,
                        UserPublishedId = e.UserPublishedId,
                        PublishingAt = e.PublishingAt,
                        PublishedAt = e.PublishedAt,
                        UserId = e.UserId,
                        UpdatedAt = e.UpdatedAt,
                        Status = e.Status,
                        LicenseId = e.LicenseId,
                        applicationRange = db.DataSetMapping.Where(c => c.DataSetId == e.DataSetId
                        && c.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE
                        && c.Status != (int)Const.Status.DELETED).Select(c => new CategoryDTL
                        {
                            CategoryId = (int)c.TargetId,
                            Location = (int)c.Location,
                        }).OrderBy(e => e.Location).ToList(),
                        researchArea = db.DataSetMapping.Where(c => c.DataSetId == e.DataSetId
                        && c.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA
                        && c.Status != (int)Const.Status.DELETED).Select(c => new CategoryDTL
                        {
                            CategoryId = (int)c.TargetId,
                            Location = (int)c.Location,
                        }).OrderBy(e => e.Location).ToList(),
                        unit = db.Unit.Where(c => c.UnitId == e.UnitId).Select(c => new UnitDT
                        {
                            UnitId = c.UnitId,
                            Name = c.Name
                        }).ToList(),
                        userCreated = db.Customer.Where(c => c.CustomerId == e.UserCreatedId).Select(c => new CustomerDT
                        {
                            UserId = c.CustomerId,
                            FullName = c.FullName,
                            UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
                        }).FirstOrDefault(),
                        licecses = db.News.Where(c => c.NewsId == e.LicenseId).Select(c => new NewsDTO
                        {
                            NewsId = c.NewsId,
                            Title = c.Title,
                            Url = c.Url,
                        }).FirstOrDefault(),
                        userApproved = db.Customer.Where(c => c.CustomerId == e.UserApprovedId).Select(c => new CustomerDT
                        {
                            UserId = c.CustomerId,
                            FullName = c.FullName,
                            UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
                        }).FirstOrDefault(),
                        userPublished = db.Customer.Where(c => c.CustomerId == e.UserPublishedId).Select(c => new CustomerDT
                        {
                            UserId = c.CustomerId,
                            FullName = c.FullName,
                            UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
                        }).FirstOrDefault(),
                        listFiles = db.Attactment.Where(c => c.TargetId == e.DataSetId
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
                            ExtensionName = c.ExtensionName,
                            Storage = c.Storage,
                            CreatedId = c.CreatedId,
                            UpdatedId = c.UpdatedId,
                            CreatedAt = c.CreatedAt,
                            UpdatedAt = c.UpdatedAt,
                            Status = c.Status,
                        }).ToList(),
                    }).FirstOrDefaultAsync();

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    if (data.applicationRange != null)
                    {
                        var listAR = await db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE).ToListAsync();
                        foreach (var item in data.applicationRange)
                        {
                            var ar = listAR.Where(e => e.CategoryId == item.CategoryId).FirstOrDefault();
                            if (ar != null)
                            {
                                item.Name = ar.Name;
                                item.Code = ar.Code;
                                item.Url = ar.Url;
                            }
                        }
                    }
                    if (data.researchArea != null)
                    {
                        var listRA = await db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA).ToListAsync();
                        foreach (var item in data.researchArea)
                        {
                            var ra = listRA.Where(e => e.CategoryId == item.CategoryId).FirstOrDefault();
                            if (ra != null)
                            {
                                item.Name = ra.Name;
                                item.Code = ra.Code;
                                item.Url = ra.Url;
                            }
                        }
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataSet(long id, DataSetDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            //int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền sửa bộ dữ liệu!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu");
                    return Ok(def);
                }
                if (data.Title == null || data.Title == "")
                {
                    def.meta = new Meta(211, "Chưa nhập tiêu đề!");
                    return Ok(def);
                }
                if (data.applicationRange == null)
                {
                    def.meta = new Meta(2112, "Chưa chọn phạm vi ứng dụng!");
                    return Ok(def);
                }
                else
                {
                    if (data.applicationRange.Count <= 0)
                    {
                        def.meta = new Meta(2112, "Chưa chọn phạm vi ứng dụng!");
                        return Ok(def);
                    }
                }
                if (data.researchArea == null)
                {
                    def.meta = new Meta(2112, "Chưa chọn lĩnh vực nghiên cứu!");
                    return Ok(def);
                }
                else
                {
                    if (data.researchArea.Count <= 0)
                    {
                        def.meta = new Meta(2112, "Chưa chọn lĩnh vực nghiên cứu!");
                        return Ok(def);
                    }
                }
                if (data.Contents == null || data.Contents == "")
                {
                    def.meta = new Meta(2111, "Chưa nhập mô tả nội dung");
                    return Ok(def);
                }
                if (data.AuthorName == null || data.AuthorName == "")
                {
                    def.meta = new Meta(2111, "Chưa nhập tác giả!");
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
                        dataSet.Contents = data.Contents;
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(data.Contents);
                        dataSet.Description = doc.DocumentNode.InnerText != null ? HttpUtility.HtmlDecode(doc.DocumentNode.InnerText) : "";
                        dataSet.Image = data.Image;
                        dataSet.Image = data.Image;
                        dataSet.Url = url;
                        dataSet.LinkVideo = data.LinkVideo;
                        dataSet.AuthorName = data.AuthorName;
                        dataSet.AuthorEmail = data.AuthorEmail;
                        dataSet.AuthorPhone = data.AuthorPhone;
                        dataSet.Version = data.Version;
                        dataSet.LicenseId = data.LicenseId;
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

                        if (dataSet.Status == (int)Const.DataSetStatus.TEMP)
                        {
                            dataSet.UserEditedId = userId;
                            dataSet.EditedAt = DateTime.Now;
                        }
                        else if (dataSet.Status == (int)Const.DataSetStatus.PENDING)
                        {
                            dataSet.UserEditedId = userId;
                            dataSet.EditedAt = DateTime.Now;
                            dataSet.ApprovingAt = DateTime.Now;

                            //Gửi Email vả thông báo
                            try
                            {
                                Config config = db.Config.Where(c => c.ConpanyId == Const.WEBSITEID).FirstOrDefault();
                                if (config == null)
                                {
                                    def.meta = new Meta(404, "Không tìm thấy cấu hình để gửi Email xác nhận đăng ký!");
                                    return Ok(def);
                                }

                                if (config.EmailSender == null || config.EmailSender == "" || config.EmailHost == null || config.EmailHost == "" || config.EmailUserName == null || config.EmailUserName == "" || config.EmailPasswordHash == null || config.EmailPasswordHash == "" || config.EmailPort == null)
                                {
                                    def.meta = new Meta(404, "Thông tin cấu hình để gửi Email xác nhận đăng ký không chính xác!");
                                    return Ok(def);
                                }
                                //Lấy thông tin người đăng
                                var userCreate = await db.Customer.Where(e => e.CustomerId == userId).FirstOrDefaultAsync();
                                if (userCreate == null)
                                {
                                    def.meta = new Meta(404, "Thông tin người tạo bộ dữ liệu không đúng!");
                                    return Ok(def);
                                }
                                //Gửi Email vả thông báo
                                string subject = config.EmailSender + " - Duyệt bộ dữ liệu";
                                //Tạo thông báo
                                string linkConfirm = "xem-chi-tiet-du-lieu-" + dataSet.DataSetId;
                                //Nếu tài khoản cá nhân link vào cms, nếu tài khoản tổ chức link vào tổ chức
                                if (userCreate.Type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL)
                                {
                                    linkConfirm = "cms/data/dataset-list";
                                }
                                //Lấy ds người nhận, nếu là cá nhân thì các tk có quyền ql ng dùng,
                                //nếu tổ chức là các tk quản lý tổ chức đó
                                linkConfirm = config.Website + linkConfirm;
                                string linkConfirmUrl = "'" + linkConfirm + "'";
                                if (userCreate.Type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL)
                                {
                                    var listUser = await (from u in db.User
                                                          join c in db.Customer on u.UserMapId equals c.CustomerId
                                                          join ur in db.UserRole on u.UserId equals ur.UserId
                                                          join fr in db.FunctionRole on ur.RoleId equals fr.TargetId
                                                          join f in db.Function on fr.FunctionId equals f.FunctionId
                                                          where u.Status == (int)Const.Status.NORMAL
                                                          && c.Status == (int)Const.Status.NORMAL
                                                          && ur.Status != (int)Const.Status.DELETED
                                                          && fr.Status != (int)Const.Status.DELETED
                                                          && f.Status != (int)Const.Status.DELETED
                                                          && fr.ActiveKey.Substring(0, 1) == "1" && fr.Type == (int)Const.TypeFunction.FUNCTION_ROLE
                                                          && f.Code == "DCK"
                                                          select c).GroupBy(e => new
                                                          {
                                                              e.CustomerId,
                                                              e.Email,
                                                              e.FullName,
                                                              e.IsNotificationMail,
                                                              e.IsNotificationWeb
                                                          }).Select(e => new
                                                          {
                                                              CustomerId = e.Key.CustomerId,
                                                              Email = e.Key.Email,
                                                              FullName = e.Key.FullName,
                                                              IsNotificationMail = e.Key.IsNotificationMail,
                                                              IsNotificationWeb = e.Key.IsNotificationWeb,
                                                          }).ToListAsync();
                                    foreach (var item in listUser)
                                    {
                                        String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/notification-dataset-add.html");
                                        sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                   config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                   config.Website, item.FullName, userCreate.Email, linkConfirmUrl, linkConfirm, userCreate.FullName);
                                        //Tạo thông báo và gửi mail
                                        Notification notification = new Notification();
                                        if (item.Email != null && item.Email != "" && item.IsNotificationMail == true)
                                            notification.IsSentEmail = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, item.Email, subject, sBody);
                                        if (item.IsNotificationWeb == true)
                                        {
                                            notification.NotificationId = Guid.NewGuid();
                                            notification.Title = subject;
                                            notification.Contents = sBody;
                                            notification.UserPushId = userCreate.CustomerId;
                                            notification.UserReadId = item.CustomerId;
                                            notification.UrlLink = linkConfirm;
                                            notification.TargetId = dataSet.DataSetId + "";
                                            notification.TargetType = (int)Const.NotificationTargetType.DATASET;
                                            notification.CreatedAt = DateTime.Now;
                                            notification.UpdatedAt = DateTime.Now;
                                            notification.Status = (int)Const.Status.TEMP;
                                            await db.Notification.AddAsync(notification);
                                        }

                                    }
                                }
                                else
                                {
                                    //Lấy unit cha
                                    List<int> outPut = new List<int>();
                                    if (userCreate.UnitId != null)
                                    {
                                        await GetListUnit(outPut, (int)userCreate.UnitId, db);
                                    }
                                    var listUser = await (from cm in db.CustomerMapping
                                                          join c in db.Customer on cm.CustomerId equals c.CustomerId
                                                          where
                                                          //cm.TargetId == userCreate.UnitId
                                                          outPut.Contains((int)cm.TargetId)
                                    && cm.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                                    && cm.Status != (int)Const.Status.DELETED
                                    && c.Status == (int)Const.Status.NORMAL
                                                          select c).ToListAsync();
                                    foreach (var item in listUser)
                                    {
                                        String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/notification-dataset-add.html");
                                        sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                        config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                        config.Website, item.FullName, userCreate.Email, linkConfirmUrl, linkConfirm, userCreate.FullName);
                                        //Tạo thông báo và gửi mail
                                        Notification notification = new Notification();
                                        if (item.Email != null && item.Email != "" && item.IsNotificationMail == true)
                                            notification.IsSentEmail = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, item.Email, subject, sBody);
                                        if (item.IsNotificationWeb == true)
                                        {
                                            notification.NotificationId = Guid.NewGuid();
                                            notification.Title = subject;
                                            notification.Contents = sBody;
                                            notification.UserPushId = userCreate.CustomerId;
                                            notification.UserReadId = item.CustomerId;
                                            notification.UrlLink = linkConfirm;
                                            notification.TargetId = dataSet.DataSetId + "";
                                            notification.TargetType = (int)Const.NotificationTargetType.DATASET;
                                            notification.CreatedAt = DateTime.Now;
                                            notification.UpdatedAt = DateTime.Now;
                                            notification.Status = (int)Const.Status.TEMP;
                                            await db.Notification.AddAsync(notification);
                                        }
                                    }
                                }

                                await db.SaveChangesAsync();
                            }
                            catch { }
                        }

                        db.Entry(dataSet).State = EntityState.Modified;

                        //Sửa pv ứng dụng
                        List<DataSetMapping> dataApplications = db.DataSetMapping.Where(e => e.DataSetId == dataSet.DataSetId
                                && e.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE
                                && e.Status != (int)Const.Status.DELETED).ToList();
                        if (dataApplications != null)
                        {
                            int k = 1;
                            foreach (var item in data.applicationRange)
                            {
                                DataSetMapping dataApplication = dataApplications.Where(cf => cf.TargetId == item.CategoryId).FirstOrDefault();
                                if (dataApplication != null)
                                {
                                    dataApplication.TargetId = item.CategoryId;
                                    dataApplication.Location = k;
                                    dataApplication.UserId = userId;
                                    //dataApplication.UpdatedAt = DateTime.Now;
                                    db.DataSetMapping.Update(dataApplication);

                                    dataApplications.Remove(dataApplication);
                                }
                                else
                                {
                                    DataSetMapping dataSetMapping = new DataSetMapping();
                                    dataSetMapping.DataSetMappingId = Guid.NewGuid();
                                    dataSetMapping.DataSetId = dataSet.DataSetId;
                                    dataSetMapping.TargetId = item.CategoryId;
                                    dataSetMapping.TargetType = (int)Const.DataSetMapping.DATA_APPLICATION_RANGE;
                                    dataSetMapping.Location = k;
                                    dataSetMapping.CreatedAt = DateTime.Now;
                                    dataSetMapping.UserId = userId;
                                    dataSetMapping.Status = (int)Const.Status.NORMAL;
                                    await db.DataSetMapping.AddAsync(dataSetMapping);
                                }
                                k++;
                            }
                            dataApplications.ForEach(x => x.Status = (int)Const.Status.DELETED);
                        }

                        //Sửa lv nghiên cứu
                        List<DataSetMapping> dataResearchs = db.DataSetMapping.Where(e => e.DataSetId == dataSet.DataSetId
                                && e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA
                                && e.Status != (int)Const.Status.DELETED).ToList();
                        if (dataResearchs != null)
                        {
                            int k = 1;
                            foreach (var item in data.researchArea)
                            {
                                DataSetMapping dataResearch = dataResearchs.Where(cf => cf.TargetId == item.CategoryId).FirstOrDefault();
                                if (dataResearch != null)
                                {
                                    dataResearch.TargetId = item.CategoryId;
                                    dataResearch.UserId = userId;
                                    dataResearch.Location = k;
                                    db.DataSetMapping.Update(dataResearch);

                                    dataResearchs.Remove(dataResearch);
                                }
                                else
                                {
                                    DataSetMapping dataSetMapping = new DataSetMapping();
                                    dataSetMapping.DataSetMappingId = Guid.NewGuid();
                                    dataSetMapping.DataSetId = dataSet.DataSetId;
                                    dataSetMapping.TargetId = item.CategoryId;
                                    dataSetMapping.TargetType = (int)Const.DataSetMapping.DATA_RESEARCH_AREA;
                                    dataSetMapping.Location = k;
                                    dataSetMapping.CreatedAt = DateTime.Now;
                                    dataSetMapping.UserId = userId;
                                    dataSetMapping.Status = (int)Const.Status.NORMAL;
                                    await db.DataSetMapping.AddAsync(dataSetMapping);
                                }
                                k++;
                            }
                            dataResearchs.ForEach(x => x.Status = (int)Const.Status.DELETED);
                        }

                        if (data.listFiles != null)
                        {
                            foreach (var item in data.listFiles)
                            {
                                var attachmentExist = await db.Attactment.FindAsync(item.AttactmentId);
                                if (attachmentExist != null)
                                {
                                    if (item.Status == (int)Const.Status.DELETED)
                                    {
                                        attachmentExist.Status = (int)Const.Status.DELETED;
                                    }
                                    else
                                    {
                                        attachmentExist.Name = item.Name;
                                        attachmentExist.Url = item.Url;
                                        attachmentExist.Note = item.Note;
                                        attachmentExist.Extension = item.Extension;
                                        attachmentExist.ExtensionName = item.ExtensionName;
                                        attachmentExist.Storage = item.Storage;
                                    }
                                    db.Entry(attachmentExist).State = EntityState.Modified;
                                }
                                else
                                {
                                    Attactment attactment = new Attactment();
                                    attactment.AttactmentId = item.AttactmentId != null ? item.AttactmentId : Guid.NewGuid();
                                    attactment.Name = item.Name;
                                    attactment.TargetId = dataSet.DataSetId;
                                    attactment.IsImageMain = item.IsImageMain;
                                    attactment.TargetType = (int)Const.TypeAttachment.FILE_DATASET;
                                    attactment.Url = item.Url;
                                    attactment.Note = item.Note;
                                    attactment.Thumb = item.Thumb;
                                    attactment.Extension = item.Extension;
                                    attactment.ExtensionName = item.ExtensionName;
                                    attactment.Storage = item.Storage;
                                    attactment.CreatedAt = DateTime.Now;
                                    attactment.CreatedId = data.UserId;
                                    attactment.UpdatedId = data.UserId;
                                    attactment.Status = (int)Const.Status.NORMAL;
                                    await db.Attactment.AddAsync(attactment);
                                }
                            }
                        }

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DataSetId > 0)
                            {
                                transaction.Commit();
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Biên tập, chỉnh sửa bộ dataset “" + data.Title + "”";
                                action.ActionType = (int)Const.ActionType.UPDATE;
                                action.TargetId = data.DataSetId.ToString();
                                action.TargetName = data.Title;
                                action.CompanyId = 1;
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

        [HttpPost]
        public async Task<IActionResult> PostDataSet(DataSetDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            int languageId = 1;
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
            //int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền thêm mới bộ dữ liệu!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu");
                    return Ok(def);
                }
                if (data.Title == null || data.Title == "")
                {
                    def.meta = new Meta(211, "Chưa nhập tiêu đề!");
                    return Ok(def);
                }
                if (data.applicationRange == null)
                {
                    def.meta = new Meta(2112, "Chưa chọn phạm vi ứng dụng!");
                    return Ok(def);
                }
                else
                {
                    if (data.applicationRange.Count <= 0)
                    {
                        def.meta = new Meta(2112, "Chưa chọn phạm vi ứng dụng!");
                        return Ok(def);
                    }
                }
                if (data.researchArea == null)
                {
                    def.meta = new Meta(2112, "Chưa chọn lĩnh vực nghiên cứu!");
                    return Ok(def);
                }
                else
                {
                    if (data.researchArea.Count <= 0)
                    {
                        def.meta = new Meta(2112, "Chưa chọn lĩnh vực nghiên cứu!");
                        return Ok(def);
                    }
                }
                if (data.Contents == null || data.Contents == "")
                {
                    def.meta = new Meta(2111, "Chưa nhập mô tả nội dung");
                    return Ok(def);
                }
                if (data.AuthorName == null || data.AuthorName == "")
                {
                    def.meta = new Meta(2111, "Chưa nhập tác giả!");
                    return Ok(def);
                }

                using (var db = new IOITDataContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //check tài khoản
                        var checkUser = await db.Customer.Where(e => e.CustomerId == userId).FirstOrDefaultAsync();
                        if (checkUser == null)
                        {
                            def.meta = new Meta(2111, "Tài khoản đăng nhập không đúng, vui lòng đăng nhập lại!");
                            return Ok(def);
                        }

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
                        dataSet.Contents = data.Contents;
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(data.Contents);
                        dataSet.Description = doc.DocumentNode.InnerText != null ? HttpUtility.HtmlDecode(doc.DocumentNode.InnerText) : "";
                        dataSet.Image = data.Image;
                        dataSet.Url = url;
                        dataSet.LinkVideo = data.LinkVideo;
                        dataSet.AuthorName = data.AuthorName;
                        dataSet.AuthorEmail = data.AuthorEmail;
                        dataSet.AuthorPhone = data.AuthorPhone;
                        dataSet.Version = data.Version;
                        dataSet.LicenseId = data.LicenseId;
                        dataSet.Note = data.Note;
                        dataSet.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        dataSet.DateStartOn = data.DateStartOn == null ? DateTime.Now : data.DateStartOn;
                        dataSet.DateEndOn = data.DateEndOn == null ? DateTime.Now.AddYears(100) : data.DateEndOn;
                        dataSet.DownNumber = data.DownNumber != null ? data.DownNumber : 0;
                        dataSet.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 0;
                        dataSet.Location = data.Location != null ? data.Location : 1;
                        dataSet.IsHot = data.IsHot != null ? data.IsHot : false;
                        dataSet.Type = checkUser.Type != null ? checkUser.Type : (int)Const.DataSetType.DATA_PERSONAL;
                        dataSet.ApplicationRangeId = data.ApplicationRangeId;
                        dataSet.ResearchAreaId = data.ResearchAreaId;
                        dataSet.UnitId = checkUser.UnitId;
                        dataSet.IsPublish = data.IsPublish;
                        dataSet.MetaTitle = data.MetaTitle != null ? data.MetaTitle : data.Title;
                        dataSet.MetaKeyword = data.MetaKeyword != null ? data.MetaKeyword : data.Title;
                        dataSet.MetaDescription = data.Description != null ? (data.Description.Length >= 500 ? data.Description.Substring(0, 499) : data.Description) : data.Title;
                        dataSet.LanguageId = data.LanguageId != null ? data.LanguageId : languageId;
                        dataSet.WebsiteId = data.WebsiteId != null ? data.WebsiteId : 1;
                        dataSet.CompanyId = data.CompanyId != null ? data.CompanyId : 1;
                        dataSet.CreatedAt = DateTime.Now;
                        dataSet.UpdatedAt = DateTime.Now;
                        dataSet.UserCreatedId = userId;
                        dataSet.UserId = userId;
                        dataSet.Status = data.Status != null ? data.Status : (int)Const.DataSetStatus.TEMP;
                        if (dataSet.Status == (int)Const.DataSetStatus.TEMP)
                        {
                            dataSet.UserEditedId = userId;
                            dataSet.EditedAt = DateTime.Now;
                        }
                        else if (dataSet.Status == (int)Const.DataSetStatus.PENDING)
                        {
                            dataSet.UserEditedId = userId;
                            dataSet.EditedAt = DateTime.Now;
                            dataSet.ApprovingAt = DateTime.Now;
                        }
                        await db.DataSet.AddAsync(dataSet);
                        await db.SaveChangesAsync();

                        data.DataSetId = dataSet.DataSetId;
                        if (data.DataSetRootId == null) data.DataSetRootId = dataSet.DataSetId;

                        //add list phạm vi ứng dụng
                        if (data.applicationRange != null)
                        {
                            int kk = 1;
                            foreach (var item in data.applicationRange)
                            {
                                DataSetMapping dataSetMapping = new DataSetMapping();
                                dataSetMapping.DataSetMappingId = Guid.NewGuid();
                                dataSetMapping.DataSetId = dataSet.DataSetId;
                                dataSetMapping.TargetId = item.CategoryId;
                                dataSetMapping.TargetType = (int)Const.DataSetMapping.DATA_APPLICATION_RANGE;
                                dataSetMapping.Location = kk;
                                dataSetMapping.CreatedAt = DateTime.Now;
                                dataSetMapping.UserId = userId;
                                dataSetMapping.Status = (int)Const.Status.NORMAL;
                                await db.DataSetMapping.AddAsync(dataSetMapping);
                                kk++;
                            }
                        }

                        //add list lĩnh vực nghiên cứu
                        if (data.researchArea != null)
                        {
                            int kk = 1;
                            foreach (var item in data.researchArea)
                            {
                                DataSetMapping dataSetMapping = new DataSetMapping();
                                dataSetMapping.DataSetMappingId = Guid.NewGuid();
                                dataSetMapping.DataSetId = dataSet.DataSetId;
                                dataSetMapping.TargetId = item.CategoryId;
                                dataSetMapping.TargetType = (int)Const.DataSetMapping.DATA_RESEARCH_AREA;
                                dataSetMapping.Location = kk;
                                dataSetMapping.CreatedAt = DateTime.Now;
                                dataSetMapping.UserId = userId;
                                dataSetMapping.Status = (int)Const.Status.NORMAL;
                                await db.DataSetMapping.AddAsync(dataSetMapping);
                                kk++;
                            }
                        }

                        //add list files
                        if (data.listFiles != null)
                        {
                            foreach (var item in data.listFiles)
                            {
                                if (item.Status != 99)
                                {
                                    Attactment attactment = new Attactment();
                                    attactment.AttactmentId = item.AttactmentId != Guid.Empty ? item.AttactmentId : Guid.NewGuid();
                                    attactment.Name = item.Name;
                                    attactment.TargetId = dataSet.DataSetId;
                                    attactment.IsImageMain = item.IsImageMain;
                                    attactment.TargetType = (int)Const.TypeAttachment.FILE_DATASET;
                                    attactment.Url = item.Url;
                                    attactment.Note = item.Note;
                                    attactment.Thumb = item.Thumb;
                                    attactment.Extension = item.Extension;
                                    attactment.ExtensionName = item.ExtensionName;
                                    attactment.Storage = item.Storage;
                                    attactment.CreatedAt = DateTime.Now;
                                    attactment.CreatedId = userId;
                                    attactment.UpdatedId = userId;
                                    attactment.Status = (int)Const.Status.NORMAL;
                                    await db.Attactment.AddAsync(attactment);
                                }
                            }
                        }

                        //tính toán add unit
                        //Lấy unit cha
                        List<int> outPut = new List<int>();
                        if (checkUser.UnitId != null)
                        {
                            await GetListUnit(outPut, (int)checkUser.UnitId, db);
                            int kk = 1;
                            foreach (var item in outPut)
                            {
                                DataSetMapping dataSetMapping = new DataSetMapping();
                                dataSetMapping.DataSetMappingId = Guid.NewGuid();
                                dataSetMapping.DataSetId = dataSet.DataSetId;
                                dataSetMapping.TargetId = item;
                                dataSetMapping.TargetType = (int)Const.DataSetMapping.DATA_UNIT;
                                dataSetMapping.Location = kk;
                                dataSetMapping.CreatedAt = DateTime.Now;
                                dataSetMapping.UserId = userId;
                                dataSetMapping.Status = (int)Const.Status.NORMAL;
                                await db.DataSetMapping.AddAsync(dataSetMapping);
                                kk++;
                            }
                        }

                        //Gửi Email vả thông báo
                        if (dataSet.Status == (int)Const.DataSetStatus.PENDING)
                        {
                            try
                            {
                                Config config = db.Config.Where(c => c.ConpanyId == Const.WEBSITEID).FirstOrDefault();
                                if (config == null)
                                {
                                    def.meta = new Meta(404, "Không tìm thấy cấu hình để gửi Email xác nhận đăng ký!");
                                    return Ok(def);
                                }

                                if (config.EmailSender == null || config.EmailSender == "" || config.EmailHost == null || config.EmailHost == "" || config.EmailUserName == null || config.EmailUserName == "" || config.EmailPasswordHash == null || config.EmailPasswordHash == "" || config.EmailPort == null)
                                {
                                    def.meta = new Meta(404, "Thông tin cấu hình để gửi Email xác nhận đăng ký không chính xác!");
                                    return Ok(def);
                                }
                                //Lấy thông tin người đăng
                                var userCreate = await db.Customer.Where(e => e.CustomerId == userId).FirstOrDefaultAsync();
                                if (userCreate == null)
                                {
                                    def.meta = new Meta(404, "Thông tin người tạo bộ dữ liệu không đúng!");
                                    return Ok(def);
                                }
                                //Gửi Email vả thông báo
                                string subject = config.EmailSender + " - Duyệt bộ dữ liệu";
                                //Tạo thông báo

                                string linkConfirm = "xem-chi-tiet-du-lieu-" + dataSet.DataSetId;
                                //Nếu tài khoản cá nhân link vào cms, nếu tài khoản tổ chức link vào tổ chức
                                if (userCreate.Type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL)
                                {
                                    linkConfirm = "cms/data/dataset-list";
                                }
                                //Lấy ds người nhận, nếu là cá nhân thì các tk có quyền ql ng dùng,
                                //nếu tổ chức là các tk quản lý tổ chức đó
                                linkConfirm = config.Website + linkConfirm;
                                string linkConfirmUrl = "'" + linkConfirm + "'";
                                if (userCreate.Type == (int)Const.TypeCustomer.CUSTOMER_PERSONAL)
                                {
                                    var listUser = await (from u in db.User
                                                          join c in db.Customer on u.UserMapId equals c.CustomerId
                                                          join ur in db.UserRole on u.UserId equals ur.UserId
                                                          join fr in db.FunctionRole on ur.RoleId equals fr.TargetId
                                                          join f in db.Function on fr.FunctionId equals f.FunctionId
                                                          where u.Status == (int)Const.Status.NORMAL
                                                          && c.Status == (int)Const.Status.NORMAL
                                                          && ur.Status != (int)Const.Status.DELETED
                                                          && fr.Status != (int)Const.Status.DELETED
                                                          && f.Status != (int)Const.Status.DELETED
                                                          && fr.ActiveKey.Substring(0, 1) == "1" && fr.Type == (int)Const.TypeFunction.FUNCTION_ROLE
                                                          && f.Code == "DCK"
                                                          select c).GroupBy(e => new
                                                          {
                                                              e.CustomerId,
                                                              e.Email,
                                                              e.FullName,
                                                              e.IsNotificationMail,
                                                              e.IsNotificationWeb
                                                          }).Select(e => new
                                                          {
                                                              CustomerId = e.Key.CustomerId,
                                                              Email = e.Key.Email,
                                                              FullName = e.Key.FullName,
                                                              IsNotificationMail = e.Key.IsNotificationMail,
                                                              IsNotificationWeb = e.Key.IsNotificationWeb,
                                                          }).ToListAsync();
                                    foreach (var item in listUser)
                                    {
                                        String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/notification-dataset-add.html");
                                        sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                           config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                           config.Website, item.FullName, userCreate.Email, linkConfirmUrl, linkConfirm, userCreate.FullName);
                                        //Tạo thông báo và gửi mail
                                        Notification notification = new Notification();
                                        if (item.Email != null && item.Email != "" && item.IsNotificationMail == true)
                                            notification.IsSentEmail = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, item.Email, subject, sBody);
                                        if (item.IsNotificationWeb == true)
                                        {
                                            notification.NotificationId = Guid.NewGuid();
                                            notification.Title = subject;
                                            notification.Contents = sBody;
                                            notification.UserPushId = userCreate.CustomerId;
                                            notification.UserReadId = item.CustomerId;
                                            notification.UrlLink = linkConfirm;
                                            notification.TargetId = dataSet.DataSetId + "";
                                            notification.TargetType = (int)Const.NotificationTargetType.DATASET;
                                            notification.CreatedAt = DateTime.Now;
                                            notification.UpdatedAt = DateTime.Now;
                                            notification.Status = (int)Const.Status.TEMP;
                                            await db.Notification.AddAsync(notification);
                                        }

                                    }
                                }
                                else
                                {
                                    ////Lấy unit cha
                                    //List<int> outPut = new List<int>();
                                    //await GetListUnit(outPut, (int)checkUser.UnitId, db);

                                    var listUser = await (from cm in db.CustomerMapping
                                                          join c in db.Customer on cm.CustomerId equals c.CustomerId
                                                          where
                                                          //cm.TargetId == userCreate.UnitId
                                                          outPut.Contains((int)cm.TargetId)
                                                            && cm.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_UNIT
                                                            && cm.Status != (int)Const.Status.DELETED
                                                            && c.Status == (int)Const.Status.NORMAL
                                                          select c).ToListAsync();
                                    foreach (var item in listUser)
                                    {
                                        String sBody = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + "/template/email/notification-dataset-add.html");
                                        sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                                    config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                                    config.Website, item.FullName, userCreate.Email, linkConfirmUrl, linkConfirm, userCreate.FullName);
                                        //Tạo thông báo và gửi mail
                                        Notification notification = new Notification();
                                        if (item.Email != null && item.Email != "" && item.IsNotificationMail == true)
                                            notification.IsSentEmail = EmailService.Send(config.EmailUserName, config.EmailPasswordHash, config.EmailHost, (int)config.EmailPort, item.Email, subject, sBody);
                                        if (item.IsNotificationWeb == true)
                                        {
                                            notification.NotificationId = Guid.NewGuid();
                                            notification.Title = subject;
                                            notification.Contents = sBody;
                                            notification.UserPushId = userCreate.CustomerId;
                                            notification.UserReadId = item.CustomerId;
                                            notification.UrlLink = linkConfirm;
                                            notification.TargetId = dataSet.DataSetId + "";
                                            notification.TargetType = (int)Const.NotificationTargetType.DATASET;
                                            notification.CreatedAt = DateTime.Now;
                                            notification.UpdatedAt = DateTime.Now;
                                            notification.Status = (int)Const.Status.TEMP;
                                            await db.Notification.AddAsync(notification);
                                        }
                                    }
                                }

                                await db.SaveChangesAsync();
                            }
                            catch { }

                        }

                        //đánh dấu nêu tạo từ ceph
                        if (data.folderFileCeph != null)
                        {
                            FolderCeph folderCeph = new FolderCeph();
                            folderCeph.DataSetId = dataSet.DataSetId;
                            folderCeph.Name = data.folderFileCeph.Name;
                            folderCeph.Link = data.folderFileCeph.Link;
                            folderCeph.CreatedAt = DateTime.Now;
                            folderCeph.UpdatedAt = DateTime.Now;
                            folderCeph.UserId = userId;
                            folderCeph.Status = (int)Const.Status.NORMAL;
                            await db.FolderCeph.AddAsync(folderCeph);
                            await db.SaveChangesAsync();
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
                                action.CompanyId = 1;
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
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "CustomerId").Select(c => c.Value).SingleOrDefault());
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

        private async Task GetListUnit(List<int> outPut, int unitId, IOITDataContext db)
        {
            var unit = await db.Unit.Where(x => x.UnitId == unitId && x.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
            if (unit != null)
            {
                outPut.Add(unit.UnitId);
                if (unit.UnitParentId > 0)
                {
                    if (unit.UnitId != unit.UnitParentId)
                    {
                        await GetListUnit(outPut, unit.UnitParentId, db);
                    }
                }
            }
        }

        [HttpGet("getDataChartHomeLine")]
        public async Task<IActionResult> GetDataChartHomeLine()
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                using (var db = new IOITDataContext())
                {
                    ChartLine chartLine = new ChartLine();
                    int day = int.Parse(_configuration["AppSettings:numberDayReport"]);
                    var dateStart = DateTime.Now.AddMonths(-day);
                    var dateEnd = DateTime.Now;
                    List<string> labels = new List<string>();
                    List<string> dataP = new List<string>();
                    List<string> dataU = new List<string>();
                    long max = 0;
                    var dataS = await db.DataSet.Where(e => e.PublishedAt >= dateStart && e.PublishedAt <= dateEnd
                        && e.Status == (int)Const.Status.NORMAL).ToListAsync();
                    for (int i = 0; i <= day; i++)
                    {
                        var dateStartNN = dateStart.AddMonths(i);
                        var dateStartN = new DateTime(dateStartNN.Year, dateStartNN.Month, 1, 0, 0, 0);
                        var dateEndN = dateStartN.AddMonths(1).AddDays(-1);
                        dateEndN = new DateTime(dateEndN.Year, dateEndN.Month, dateEndN.Day, 23, 59, 59);
                        string label = dateStartN.Month + "/" + dateStartN.Year;
                        labels.Add(label);
                        long pp = dataS.Where(e => (e.Type == (int)Const.DataSetType.DATA_PERSONAL || e.Type == (int)Const.DataSetType.DATA_UNIT)
                        && e.PublishedAt.Value >= dateStartN
                        && e.PublishedAt.Value <= dateEndN).Count();
                        dataP.Add(pp + "");
                        long pu = dataS.Where(e => e.Type == (int)Const.DataSetType.DATA_UNIT
                        && e.PublishedAt.Value >= dateStartN
                        && e.PublishedAt.Value <= dateEndN).Count();
                        dataU.Add(pu + "");
                        if (pp > max) max = pp;
                        if (pu > max) max = pu;
                    }
                    chartLine.labels = labels;
                    chartLine.dataP = dataP;
                    chartLine.dataU = dataU;
                    chartLine.max = max;
                    chartLine.day = day;
                    def.meta = new Meta(200, "Success");
                    def.data = chartLine;
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

        [HttpGet("GetDataChartHomeCircle")]
        public async Task<IActionResult> GetDataChartHomeCircle()
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                using (var db = new IOITDataContext())
                {
                    int day = int.Parse(_configuration["AppSettings:numberDayReport"]);
                    var dateStart = DateTime.Now.AddMonths(-day);
                    var dateEnd = DateTime.Now;
                    ChartCircle chartCircle = new ChartCircle();
                    var dataAR = await (from c in db.Category
                                        join dsm in db.DataSetMapping on c.CategoryId equals dsm.TargetId
                                        join ds in db.DataSet on dsm.DataSetId equals ds.DataSetId
                                        where c.Status == (int)Const.Status.NORMAL
                                        && ds.PublishedAt >= dateStart && ds.PublishedAt <= dateEnd
                                        && ds.Status == (int)Const.Status.NORMAL
                                        && c.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE
                                        && dsm.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE
                                        && dsm.Status != (int)Const.Status.DELETED
                                        group c by new
                                        {
                                            c.CategoryId,
                                            c.Name,
                                            c.Url,
                                            c.Image,
                                        } into g
                                        select new CategoryAR
                                        {
                                            CategoryId = g.Key.CategoryId,
                                            Name = g.Key.Name,
                                            Url = g.Key.Url,
                                            Image = g.Key.Image,
                                            DataSetNumber = g.Count()
                                        }).ToListAsync();
                    List<string> labels = new List<string>();
                    List<string> datas = new List<string>();
                    List<string> backgroundColors = new List<string>();
                    foreach (var item in dataAR)
                    {
                        labels.Add(item.Name);
                        datas.Add(item.DataSetNumber + "");
                        backgroundColors.Add(item.Image);
                    }
                    chartCircle.labels = labels;
                    chartCircle.datas = datas;
                    chartCircle.backgroundColors = backgroundColors;
                    chartCircle.day = day;
                    def.meta = new Meta(200, "Success");
                    def.data = chartCircle;
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

        [HttpGet("getDataChartDataArLine")]
        public async Task<IActionResult> getDataChartDataArLine(int id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                using (var db = new IOITDataContext())
                {
                    ChartLine chartLine = new ChartLine();
                    int day = int.Parse(_configuration["AppSettings:numberDayReport"]);
                    var dateStart = DateTime.Now.AddMonths(-day);
                    var dateEnd = DateTime.Now;
                    List<string> labels = new List<string>();
                    List<string> dataP = new List<string>();
                    List<string> dataU = new List<string>();
                    long max = 0;
                    var dataS = await (from dsm in db.DataSetMapping
                                       join ds in db.DataSet on dsm.DataSetId equals ds.DataSetId
                                       where dsm.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE
                                       && (dsm.TargetId == id || id == 0)
                                       && dsm.Status != (int)Const.Status.DELETED
                                       && ds.Status == (int)Const.Status.NORMAL
                                       && ds.PublishedAt >= dateStart && ds.PublishedAt <= dateEnd
                                       group ds by new
                                       {
                                           ds.DataSetId,
                                           ds.PublishedAt,
                                           ds.Type
                                       } into e
                                       select new DataSetDTO
                                       {
                                           DataSetId = e.Key.DataSetId,
                                           PublishedAt = e.Key.PublishedAt,
                                           Type = e.Key.Type,
                                       }).ToListAsync();
                    for (int i = 0; i <= day; i++)
                    {
                        var dateStartNN = dateStart.AddMonths(i);
                        var dateStartN = new DateTime(dateStartNN.Year, dateStartNN.Month, 1, 0, 0, 0);
                        var dateEndN = dateStartN.AddMonths(1).AddDays(-1);
                        dateEndN = new DateTime(dateEndN.Year, dateEndN.Month, dateEndN.Day, 23, 59, 59);
                        string label = dateStartN.Month + "/" + dateStartN.Year;
                        labels.Add(label);
                        long pp = dataS.Where(e => (e.Type == (int)Const.DataSetType.DATA_PERSONAL || e.Type == (int)Const.DataSetType.DATA_UNIT)
                        && e.PublishedAt.Value >= dateStartN
                        && e.PublishedAt.Value <= dateEndN).Count();
                        dataP.Add(pp + "");
                        long pu = dataS.Where(e => e.Type == (int)Const.DataSetType.DATA_UNIT
                        && e.PublishedAt.Value >= dateStartN
                        && e.PublishedAt.Value <= dateEndN).Count();
                        dataU.Add(pu + "");
                        if (pp > max) max = pp;
                        if (pu > max) max = pu;
                    }
                    chartLine.labels = labels;
                    chartLine.dataP = dataP;
                    chartLine.dataU = dataU;
                    chartLine.max = max;
                    chartLine.day = day;
                    def.meta = new Meta(200, "Success");
                    def.data = chartLine;
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

        [HttpGet("GetDataChartDataArCircle")]
        public async Task<IActionResult> GetDataChartDataArCircle(int id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                using (var db = new IOITDataContext())
                {
                    int day = int.Parse(_configuration["AppSettings:numberDayReport"]);
                    var dateStart = DateTime.Now.AddMonths(-day);
                    var dateEnd = DateTime.Now;
                    ChartCircle chartCircle = new ChartCircle();
                    var dataAR = await (from ds in db.DataSet
                                        join at in db.Attactment on ds.DataSetId equals at.TargetId
                                        join dsm in db.DataSetMapping on ds.DataSetId equals dsm.DataSetId
                                        where ds.Status == (int)Const.Status.NORMAL
                                        && ds.PublishedAt >= dateStart && ds.PublishedAt <= dateEnd
                                        && at.Status != (int)Const.Status.DELETED
                                        && at.TargetType == (int)Const.TypeAttachment.FILE_DATASET
                                        && (dsm.TargetId == id || id == 0)
                                        && dsm.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE
                                        && dsm.Status != (int)Const.Status.DELETED
                                        group at by new
                                        {
                                            at.Extension,
                                            at.ExtensionName,
                                        } into g
                                        select new ExtensionAR
                                        {
                                            Extension = g.Key.Extension,
                                            ExtensionName = g.Key.ExtensionName,
                                            DataSetNumber = g.Count()
                                        }).ToListAsync();
                    List<string> labels = new List<string>();
                    List<string> datas = new List<string>();
                    List<string> backgroundColors = new List<string>();
                    foreach (var item in dataAR)
                    {
                        if (item.ExtensionName != null && item.ExtensionName != "")
                        {
                            labels.Add(item.ExtensionName.Substring(1, item.ExtensionName.Length - 1).ToUpper());
                            datas.Add(item.DataSetNumber + "");
                        }
                        //backgroundColors.Add(item.Image);
                    }
                    chartCircle.labels = labels;
                    chartCircle.datas = datas;
                    chartCircle.backgroundColors = backgroundColors;
                    chartCircle.day = day;
                    def.meta = new Meta(200, "Success");
                    def.data = chartCircle;
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
