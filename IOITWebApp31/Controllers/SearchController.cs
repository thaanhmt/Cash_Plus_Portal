using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wangkanai.Detection;

namespace IOITWebApp31.Controllers
{
    public class SearchController : Controller
    {
        public IConfiguration _configuration { get; }
        private readonly IDevice _device;
        private readonly IHtmlLocalizer<HomeController> _localizer;
        private readonly IHttpContextAccessor _httpContext;

        public SearchController(IConfiguration configuration, IDeviceResolver deviceResolver,
            IHtmlLocalizer<HomeController> localizer, IHttpContextAccessor httpContext)
        {
            this._configuration = configuration;
            this._device = deviceResolver.Device;
            this._localizer = localizer;
            _httpContext = httpContext;
        }
        //public ActionResult Index()
        //{
        //    using (var db = new IOITDataContext())
        //    {
        //        var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
        //        ViewBag.LinkSite = website.Url;
        //        var data = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
        //        ViewBag.SeoTitle = data.MetaTitle;
        //        ViewBag.SeoDescription = data.MetaDescription;
        //        ViewBag.SeoKeywords = data.MetaKeyword;
        //        ViewBag.Logo = data.LogoHeader;
        //        ViewBag.LogoFooter = data.LogoFooter;
        //        ViewBag.Fanpage = data.Link1;
        //        ViewBag.LinkTwitter = data.Link2;
        //        ViewBag.LinkYoutube = data.Link3;
        //        ViewBag.LinkSite = data.Url;
        //        return View();
        //    }
        //}

        //public IActionResult ResulftSearch(int p)
        //{
        //    //var sCategory = HttpContext.Session.GetInt32("sCategory");
        //    var sName = HttpContext.Session.GetString("sName");
        //    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
        //    if (ViewBag.LanguageId == "1007")
        //    {
        //        //Tiếng anh
        //        ViewBag.TypeSearch = "Search results for keywords \"" + sName + "\"";
        //        ViewBag.SeoTitle = "Search results for keywords \"" + sName + "\"";
        //    }
        //    else
        //    {
        //        //Tiếng việt
        //        ViewBag.TypeSearch = "Kết quả tìm kiếm cho từ khoá \"" + sName + "\"";
        //        ViewBag.SeoTitle = "Kết quả tìm kiếm cho từ khoá \"" + sName + "\"";
        //    }
        //    ViewBag.p = p;
        //    return View();
        //}


        //[HttpGet]
        public async Task<IActionResult> ResulftSearch(Search search, int p = 1)
        {
            ViewBag.Class = "body-search";

            using (var db = new IOITDataContext())
            {
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                if (ViewBag.LanguageId == "1007")
                {
                    ViewBag.TypeSearch = "Search results for keywords \"" + search.sName + "\"";
                    ViewBag.SeoTitle = "Search results for keywords \"" + search.sName + "\"";
                }
                else
                {
                    ViewBag.TypeSearch = "Kết quả tìm kiếm cho từ khoá \"" + search.sName + "\"";
                    ViewBag.SeoTitle = "Kết quả tìm kiếm cho từ khoá \"" + search.sName + "\"";
                }
                ViewBag.sType = search.sType;
                ViewBag.typeS = search.typeS;

                search.sName = search.sName != null ? search.sName : "";
                if (search.sName == "")
                    return View(null);
                HttpContext.Session.SetInt32("sType", search.sType);
                HttpContext.Session.SetInt32("typeS", search.typeS);
                HttpContext.Session.SetString("sName", search.sName);

                // Nếu typeS không truyền lên hoặc = 0 thì mặc định là 1 (chỉ lấy tin tức)
                if (search == null || search.typeS == 0)
                {
                    search.typeS = 1;
                }

                string[] str = search.sName.Split(' ');
                string keySearch = str[0];
                for (int i = 1; i < str.Length; i++)
                {
                    keySearch += " and " + str[i];
                }
                List<DataSearch> data = new List<DataSearch>();
                if (search.sType == 1 || search.typeS == 1)
                {
                    //data = await db.News.Where(e => EF.Functions.Contains(e.Title, keySearch)
                    //|| EF.Functions.Contains(e.Contents, keySearch)).Select(e=> new DataSearch
                    //{
                    //    Id = e.NewsId,
                    //    Title = e.Title,
                    //    Contents = e.Contents,
                    //    Description = e.Description,
                    //    Image = e.Image,
                    //    Url = e.Url,
                    //    DateStartActive = e.DateStartActive,
                    //    LinkVideo = e.LinkVideo,
                    //    Type = e.TypeNewsId
                    //}).OrderByDescending(e => e.DateStartActive).Take(50).ToListAsync();

                    data = await db.News.Where(e => e.Title.Contains(search.sName)).Select(e => new DataSearch
                    {
                        Id = e.NewsId,
                        Title = e.Title,
                        Contents = e.Contents,
                        Description = e.Description,
                        Image = e.Image,
                        Url = e.Url,
                        DateStartActive = e.DateStartActive,
                        LinkVideo = e.LinkVideo,
                        Type = e.TypeNewsId
                    }).OrderByDescending(e => e.DateStartActive).Take(10).ToListAsync();
                }
                else if (search.sType == 2 || search.typeS == 2)
                {
                    //data = await db.LegalDoc.FromSqlRaw("SELECT * FROM dbo.LegalDoc WHERE CONTAINS([AttactmentBit],'" + keySearch + "') OR CONTAINS(Name,'" + keySearch + "')").Select(e => new DataSearch
                    //{
                    //    Id = e.LegalDocId,
                    //    Title = e.Name,
                    //    Contents = e.Contents,
                    //    Description = e.Description,
                    //    Image = e.Image,
                    //    Url = e.Url,
                    //    DateStartActive = e.DateEffect,
                    //    Type = 100,
                    //    Attactment = e.Attactment,
                    //    TichYeu = e.TichYeu,
                    //    DateIssue = e.DateIssue,
                    //    DateEffect = e.DateEffect,
                    //}).OrderByDescending(e => e.DateStartActive).Take(50).ToListAsync();
                    //data = await (from n in db.LegalDoc
                    //              where n.Status == (int)Const.Status.NORMAL
                    //              && (n.Name.Contains(search.sName) || search.sName == "")
                    //              select n).OrderByDescending(e => e.CreatedAt).Select(e => new DataSearch
                    //{
                    //    Id = e.LegalDocId,
                    //    Title = e.Name,
                    //    Contents = e.Contents,
                    //    Description = e.Description,
                    //    Image = e.Image,
                    //    Url = e.Url,
                    //    DateStartActive = e.DateEffect,
                    //    Type = 100,
                    //    Attactment = e.Attactment,
                    //    TichYeu = e.TichYeu,
                    //    DateIssue = e.DateIssue,
                    //    DateEffect = e.DateEffect,
                    //}).OrderByDescending(e => e.DateStartActive).Take(10).ToListAsync();
                    ViewBag.sNameF = search.sName;
                }

                ViewBag.p = p;
                int np = 10;
                ViewBag.np = np;
                ViewBag.totalAll = data.Count();
                ViewBag.total = (data.Count() - 1);
                ViewBag.page = p;
                // hien thi so page
                ViewBag.Pre = p - 1;
                ViewBag.Next = p + 1;
                ViewBag.NumPage = np;
                ViewBag.pageS = p - 5 > 1 ? (p - 5) : 1;
                ViewBag.pageE = (p + 5 < ((data.Count() - 1) / np) + 1) ? (p + 5) : (((data.Count() - 1) / np) + 1);

                var list = data.Count() > 0 ? data.Skip(np * (p - 1)).Take(np).ToList() : data.ToList();

                return View(list);
            }
        }

        public async Task<IActionResult> ResulftSearchData(Search search)
        {
            using (var db = new IOITDataContext())
            {
                //ViewBag.TypeSearch = 0;
                ViewBag.ArId = -1;
                Response.Cookies.Append(
                           "ArId", ViewBag.ArId + "",
                           new CookieOptions
                           {
                               Expires = DateTimeOffset.UtcNow.AddYears(1),
                               IsEssential = true,
                               Path = "/",
                               HttpOnly = false,
                           }
                       );
                ViewBag.DsEx = -1;
                Response.Cookies.Append(
                           "DsEx", ViewBag.DsEx + "",
                           new CookieOptions
                           {
                               Expires = DateTimeOffset.UtcNow.AddYears(1),
                               IsEssential = true,
                               Path = "/",
                               HttpOnly = false,
                           }
                       );
                ViewBag.DsUId = -1;
                Response.Cookies.Append(
                           "DsUId", ViewBag.DsUId + "",
                           new CookieOptions
                           {
                               Expires = DateTimeOffset.UtcNow.AddYears(1),
                               IsEssential = true,
                               Path = "/",
                               HttpOnly = false,
                           }
                       );
                ViewBag.RaId = -1;
                Response.Cookies.Append(
                           "RaId", ViewBag.RaId + "",
                           new CookieOptions
                           {
                               Expires = DateTimeOffset.UtcNow.AddYears(1),
                               IsEssential = true,
                               Path = "/",
                               HttpOnly = false,
                           }
                       );
                //var sCategory = HttpContext.Session.GetInt32("sCategory");
                var sName = search.sName != null ? search.sName : "";
                ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                if (ViewBag.LanguageId == "1007")
                {
                    //Tiếng anh
                    ViewBag.TypeSearch = "Search results for keywords \"" + sName + "\"";
                    ViewBag.SeoTitle = "Search results for keywords \"" + sName + "\"";
                }
                else
                {
                    //Tiếng việt
                    ViewBag.TypeSearch = "Kết quả tìm kiếm cho từ khoá \"" + sName + "\"";
                    ViewBag.SeoTitle = "Kết quả tìm kiếm cho từ khoá \"" + sName + "\"";
                }
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                Response.Cookies.Append(
                              "NameS", sName,
                              new CookieOptions
                              {
                                  Expires = DateTimeOffset.UtcNow.AddYears(1),
                                  IsEssential = true,
                                  Path = "/",
                                  HttpOnly = false,
                              }
                          );
                //ViewBag.ArId = Request.Cookies["ArId"] != null ? int.Parse(Request.Cookies["ArId"]) : -1;
                ViewBag.NameS = search.sName;
                ViewBag.NameS2 = "'" + ViewBag.NameS + "'";
                ViewBag.Class = "body-data-page";
                //Lấy ds phạm vi ứng dụng
                ViewBag.listAr = await db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE
                && e.Status == (int)Const.Status.NORMAL).OrderBy(e => e.Location).ToListAsync();
                //Lấy ds lĩnh vực nghiên cứu
                ViewBag.listRa = await db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA
                && e.Status == (int)Const.Status.NORMAL).OrderBy(e => e.Location).ToListAsync();
                //Lấy ds tổ chức
                ViewBag.listUnit = await db.Unit.Where(e => e.UnitParentId == 0
                && e.Status == (int)Const.Status.NORMAL).OrderBy(e => e.Location).ToListAsync();
                //List định dạng
                string extensionFileName = _configuration["AppSettings:extensionFileName"];
                string extensionFileId = _configuration["AppSettings:extensionFileId"];
                IList<string> ListExtensionsId = extensionFileId.Split(',', StringSplitOptions.RemoveEmptyEntries)
               .Select(s => s.Trim()).ToArray();
                IList<string> ListExtensions = extensionFileName.Split(',', StringSplitOptions.RemoveEmptyEntries)
               .Select(s => s.Trim()).ToArray();
                List<AttactmentDTO> listExtensions = new List<AttactmentDTO>();
                for (int i = 0; i < ListExtensions.Count(); i++)
                {
                    AttactmentDTO attactment = new AttactmentDTO();
                    attactment.ExtensionName = ListExtensions[i].Substring(1, ListExtensions[i].Length - 1).ToUpper();
                    attactment.Extension = byte.Parse(ListExtensionsId[i]);
                    listExtensions.Add(attactment);
                }
                ViewBag.listExtensions = listExtensions;
                return View();
            }
        }

        public async Task<ActionResult> ListResulftSearchData(int id, int p = 1, int ex = -1, int uid = -1, int aid = -1, int od = 1, string textS = "")
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    if (id == -2)
                    {
                        id = Request.Cookies["ArId"] != null ? int.Parse(Request.Cookies["ArId"]) : -1;
                    }
                    else
                    {
                        Response.Cookies.Append(
                            "ArId", id + "",
                            new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddYears(1),
                                IsEssential = true,
                                Path = "/",
                                HttpOnly = false,
                            }
                        );
                    }
                    if (ex == -2)
                    {
                        ex = Request.Cookies["DsEx"] != null ? int.Parse(Request.Cookies["DsEx"]) : -1;
                    }
                    else
                    {
                        Response.Cookies.Append(
                           "DsEx", ex + "",
                           new CookieOptions
                           {
                               Expires = DateTimeOffset.UtcNow.AddYears(1),
                               IsEssential = true,
                               Path = "/",
                               HttpOnly = false,
                           }
                       );
                    }
                    if (uid == -2)
                    {
                        uid = Request.Cookies["DsUId"] != null ? int.Parse(Request.Cookies["DsUId"]) : -1;
                    }
                    else
                    {
                        Response.Cookies.Append(
                           "DsUId", uid + "",
                           new CookieOptions
                           {
                               Expires = DateTimeOffset.UtcNow.AddYears(1),
                               IsEssential = true,
                               Path = "/",
                               HttpOnly = false,
                           }
                       );
                    }
                    if (aid == -2)
                    {
                        aid = Request.Cookies["RaId"] != null ? int.Parse(Request.Cookies["RaId"]) : -1;
                    }
                    else
                    {
                        Response.Cookies.Append(
                           "RaId", aid + "",
                           new CookieOptions
                           {
                               Expires = DateTimeOffset.UtcNow.AddYears(1),
                               IsEssential = true,
                               Path = "/",
                               HttpOnly = false,
                           }
                       );
                    }
                    if (textS == "" || textS == null)
                    {
                        textS = Request.Cookies["NameS"] != null ? Request.Cookies["NameS"] : "";
                    }

                    Response.Cookies.Append(
                       "NameS", textS,
                       new CookieOptions
                       {
                           Expires = DateTimeOffset.UtcNow.AddYears(1),
                           IsEssential = true,
                           Path = "/",
                           HttpOnly = false,
                       }
                   );

                    ViewBag.CustomerId = HttpContext.Session.GetInt32("CustomerId");
                    ViewBag.access_token = "'" + HttpContext.Session.GetString("access_token") + "'";
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    ViewBag.p = p;
                    int np = 10;
                    ViewBag.np = np;
                    //textS = textS != null ? textS : "";
                    var data = await (from ds in db.DataSet
                                      join dsm in db.DataSetMapping on ds.DataSetId equals dsm.DataSetId
                                      where ds.Status == (int)Const.Status.NORMAL
                                      && dsm.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE
                                      && (dsm.TargetId == id || id == -1)
                                      && (ds.Title.Contains(textS) || textS == "")
                                      group ds by new
                                      {
                                          ds.DataSetId,
                                          ds.PublishedAt,
                                          ds.ViewNumber,
                                          ds.DownNumber,
                                      } into e
                                      select new DataSetDTO
                                      {
                                          DataSetId = e.Key.DataSetId,
                                          PublishedAt = e.Key.PublishedAt,
                                          ViewNumber = e.Key.ViewNumber,
                                          DownNumber = e.Key.DownNumber,
                                      }).ToListAsync();
                    if (ex != -1 || uid != -1 || aid != -1)
                    {
                        //data = (from ds in data
                        //        join du in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_UNIT)
                        //        on ds.DataSetId equals du.DataSetId
                        //        join dra in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA)
                        //        on ds.DataSetId equals dra.DataSetId
                        //        join at in db.Attactment.Where(e => e.TargetType == (int)Const.TypeAttachment.FILE_DATASET)
                        //        on ds.DataSetId equals at.TargetId
                        //        where ds.Status != (int)Const.Status.DELETED
                        //        && du.Status != (int)Const.Status.DELETED
                        //        && dra.Status != (int)Const.Status.DELETED
                        //        && at.Status != (int)Const.Status.DELETED
                        //        && ((du.TargetId == uid || uid == -1))
                        //        && ((dra.TargetId == aid || aid == -1))
                        //        && ((at.Extension == ex || ex == -1))
                        //        group ds by new
                        //        {
                        //            ds.DataSetId,
                        //            ds.PublishedAt,
                        //            ds.ViewNumber,
                        //            ds.DownNumber,
                        //        } into e
                        //        select new DataSetDTO
                        //        {
                        //            DataSetId = e.Key.DataSetId,
                        //            PublishedAt = e.Key.PublishedAt,
                        //            ViewNumber = e.Key.ViewNumber,
                        //            DownNumber = e.Key.DownNumber,
                        //        }).ToList();
                        data = (from ds in data
                                join du in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_UNIT)
                                on ds.DataSetId equals du.DataSetId
                                join dra in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA)
                                on ds.DataSetId equals dra.DataSetId
                                join at in db.Attactment.Where(e => e.TargetType == (int)Const.TypeAttachment.FILE_DATASET)
                                on ds.DataSetId equals at.TargetId into atj
                                from at in atj.DefaultIfEmpty()
                                where ds.Status != (int)Const.Status.DELETED
                                && du.Status != (int)Const.Status.DELETED
                                && dra.Status != (int)Const.Status.DELETED
                                && (at == null || at.Status != (int)Const.Status.DELETED)
                                && ((du.TargetId == uid || uid == -1))
                                && ((dra.TargetId == aid || aid == -1))
                                && ((at != null ? at.Extension == ex : ex == -1) || ex == -1)
                                group ds by new
                                {
                                    ds.DataSetId,
                                    ds.PublishedAt,
                                    ds.ViewNumber,
                                    ds.DownNumber,
                                } into e
                                select new DataSetDTO
                                {
                                    DataSetId = e.Key.DataSetId,
                                    PublishedAt = e.Key.PublishedAt,
                                    ViewNumber = e.Key.ViewNumber,
                                    DownNumber = e.Key.DownNumber,
                                }).ToList();
                    }

                    if (od == 2)
                    {
                        data = data.OrderByDescending(e => e.ViewNumber).ToList();
                    }
                    else if (od == 3)
                    {
                        data = data.OrderByDescending(e => e.DownNumber).ToList();
                    }
                    else
                    {
                        data = data.OrderByDescending(e => e.PublishedAt).ToList();
                    }

                    //ViewBag.total = (data.Count());
                    //ViewBag.page = p;
                    //if ((ViewBag.total - 10) % np == 0)
                    //{
                    //    ViewBag.totalPage = (ViewBag.total) / np;

                    //}
                    //else
                    //{
                    //    ViewBag.totalPage = ((ViewBag.total) / np) + 1;
                    //}
                    // hien thi so page
                    ViewBag.totalAll = data.Count();
                    ViewBag.total = (data.Count() - 1);
                    ViewBag.page = p;
                    //if ((ViewBag.total - np) % np == 0)
                    //{
                    //    ViewBag.totalPage = (ViewBag.total) / np;
                    //}
                    //else
                    //{
                    //    ViewBag.totalPage = ((ViewBag.total) / np) + 1;
                    //}
                    // hien thi so page
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;
                    ViewBag.NumPage = np;
                    ViewBag.pageS = p - 5 > 1 ? (p - 5) : 1;
                    ViewBag.pageE = (p + 5 < ((data.Count() - 1) / np) + 1) ? (p + 5) : (((data.Count() - 1) / np) + 1);


                    var list = data.Count() > 0 ? data.Skip(np * (p - 1)).Take(np).ToList() : data.ToList();

                    foreach (var itemD in list)
                    {
                        var itemData = await db.DataSet.Where(e => e.DataSetId == itemD.DataSetId).FirstOrDefaultAsync();
                        itemD.DataSetId = itemData.DataSetId;
                        itemD.Title = itemData.Title;
                        itemD.Description = itemData.Description;
                        //itemD.Contents = itemData.Contents;
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
                        itemD.RateStar = itemData.RateStar;
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
                        itemD.userCreated = db.Customer.Where(c => c.CustomerId == itemD.UserCreatedId).Select(c => new CustomerDT
                        {
                            UserId = c.CustomerId,
                            FullName = c.FullName,
                            UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
                        }).FirstOrDefault();

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

                    return PartialView("_ResulftSearchData", list);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResulftSearchAr(Search search)
        {
            //HttpContext.Session.SetString("sName", search.sName);
            search.sNameA = search.sNameA != null ? search.sNameA : "";
            Response.Cookies.Append(
                          "NameAr", search.sNameA,
                          new CookieOptions
                          {
                              Expires = DateTimeOffset.UtcNow.AddYears(1),
                              IsEssential = true,
                              Path = "/",
                              HttpOnly = false,
                          }
                      );
            return Redirect("/toan-bo-du-lieu");
        }

        public async Task<IActionResult> LinkAllAr(int id)
        {
            Response.Cookies.Append(
                          "ArId", id + "",
                          new CookieOptions
                          {
                              Expires = DateTimeOffset.UtcNow.AddYears(1),
                              IsEssential = true,
                              Path = "/",
                              HttpOnly = false,
                          }
                      );
            return Redirect("/toan-bo-du-lieu");
        }

        public ActionResult ListResultf(int p = 1)
        {
            using (var db = new IOITDataContext())
            {
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                int languageId = Request.Cookies["LanguageId"] != null ? int.Parse(Request.Cookies["LanguageId"]) : Const.LANGUAGEID;

                ViewBag.p = p;
                int np = 10;
                ViewBag.np = np;

                var sName = HttpContext.Session.GetString("sName");
                var sType = HttpContext.Session.GetInt32("sType");
                ViewBag.sType = sType;
                if (sType == 1)
                {
                    ViewBag.News = (from pr in db.News
                                    where pr.Title.ToLower().Contains(sName.ToLower())
                                    && pr.LanguageId == languageId
                                    && pr.Status == (int)Const.Status.NORMAL
                                    select pr).OrderByDescending(e => e.CreatedAt).ToList();
                }
                if (sType == 2)
                {
                    ViewBag.NewsGT = (from pr in db.News
                                      where pr.Title.ToLower().Contains(sName.ToLower())
                                      && pr.Status == (int)Const.Status.NORMAL
                                      && pr.LanguageId == languageId
                                      && pr.TypeNewsId == 6
                                      select pr).OrderByDescending(e => e.CreatedAt).ToList();
                }
                if (sType == 3)
                {
                    ViewBag.Pro = (from pr in db.Product
                                   where pr.Name.ToLower().Contains(sName.ToLower())
                                   && pr.LanguageId == languageId
                                   && pr.Status == (int)Const.Status.NORMAL
                                   select pr).OrderByDescending(e => e.CreatedAt).ToList();
                }

                return PartialView("_ListResultf");

            }
        }

    }
}