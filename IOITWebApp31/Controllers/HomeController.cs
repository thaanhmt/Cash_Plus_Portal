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
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Wangkanai.Detection;

namespace IOITWebApp31.Controllers
{
    public class HomeController : Controller
    {
        public IConfiguration _configuration { get; }
        private readonly IDevice _device;
        private readonly IHtmlLocalizer<HomeController> _localizer;
        private readonly IHttpContextAccessor _httpContext;

        public HomeController(IConfiguration configuration, IDeviceResolver deviceResolver,
            IHtmlLocalizer<HomeController> localizer, IHttpContextAccessor httpContext)
        {
            this._configuration = configuration;
            this._device = deviceResolver.Device;
            this._localizer = localizer;
            _httpContext = httpContext;
        }

        public ActionResult Index()
        {
            ViewBag.PageHome = 88;
            using (var db = new IOITDataContext())
            {
                ViewBag.TypeSearchData = 1;
                if (_device.Type == DeviceType.Desktop)
                {
                    ViewBag.TypeDevice = 0;
                }
                else
                {
                    ViewBag.TypeDevice = 2;
                }
                var baseUri = $"{Request.Scheme}://{Request.Host}:{Request.Host.Port ?? 80}";
                ViewBag.LanguageId = _httpContext.HttpContext.Request.Cookies["LanguageId"] != null ? _httpContext.HttpContext.Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";

                var DataConfig = db.Config.Where(c => c.Status != (int)Const.Status.DELETED && c.ConfigId == 1).FirstOrDefault();
                ViewBag.IsDebug = DataConfig.ModeSite;
                ViewBag.HeaderScript = DataConfig.HeaderScript;
                ViewBag.BodyScript = DataConfig.BodyScript;
                ViewBag.FooterScript = DataConfig.FooterScript;
                ViewBag.CustomCss = DataConfig.CustomCss;

                var data = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                List<int> danhmucSp = db.Category.Where(c => c.TypeCategoryId == 11 && c.CategoryParentId == 0 && c.Status != (int)Const.Status.DELETED).OrderBy(c => c.CategoryId).Select(e => e.CategoryId).ToList();
                ViewBag.SeoTitle = data.MetaTitle;
                ViewBag.SeoDescription = data.MetaDescription;
                ViewBag.SeoKeywords = data.MetaKeyword;
                ViewBag.Logo = data.LogoHeader;
                ViewBag.LogoFooter = data.LogoFooter;
                ViewBag.Fanpage = data.Link1;
                ViewBag.LinkTwitter = data.Link2;
                ViewBag.LinkYoutube = data.Link3;
                ViewBag.LinkSite = data.Url;
                // Lấy danh sách tin tức mới nhất (không lọc theo LanguageId)
                ViewBag.NewsList = db.News
                    .Where(n =>
                        (n.TypeNewsId == (int)Const.TypeNews.NEWS_TEXT || n.TypeNewsId == (int)Const.TypeNews.NEWS_NEWS)
                        && n.CompanyId == Const.COMPANYID
                        && n.WebsiteId == Const.WEBSITEID
                        && n.Status == (int)Const.Status.NORMAL
                        && n.DateStartActive <= DateTime.Now
                    )
                    .OrderByDescending(n => n.DateStartActive)
                    .Take(9)
                    .ToList();
                return View(danhmucSp);
            }
        }

        public ActionResult About()
        {
            ViewBag.SeoTitle = "Về chúng tôi";
            ViewBag.Message = "Về chúng tôi";
            int AboutId = 4096;

            using (var db = new IOITDataContext())
            {
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                var CatPar = db.Category.Where(e => e.CategoryParentId == AboutId && e.Status != (int)Const.Status.DELETED).OrderBy(e => e.CreatedAt).ToList();
                ViewBag.Class = "body-detail";
                //Category category = db.Category.Where(c => c.CategoryId == AboutId && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                if (CatPar.Count > 0)
                {

                    return View(CatPar);
                }
                else
                {
                    return Redirect("/Home/Error");
                }
            }
        }

        //public ActionResult Contact()
        //{
        //    ViewBag.Class = "body-detail";
        //    //get url
        //    string urlLang = Request.Cookies["ReturnUrl"] != null ? Request.Cookies["ReturnUrl"] : "/";
        //    string url = Request.Path;
        //    //get lang
        //    int lang = Request.Cookies["LanguageId"] != null ? int.Parse(Request.Cookies["LanguageId"]) : Const.LANGUAGEID;
        //    int langOld = Request.Cookies["LanguageOldId"] != null ? int.Parse(Request.Cookies["LanguageOldId"]) : Const.LANGUAGEID;

        //    if (lang == 1)
        //    {
        //        ViewBag.SeoTitle = "Liên hệ";
        //        ViewBag.Message = "Liên hệ với chúng tôi";
        //    }
        //    else if(lang == 1007)
        //    {
        //        ViewBag.SeoTitle = "Contact us";
        //        ViewBag.Message = "Contact us";

        //    }
        //    //change lang
        //    if (lang != langOld && url.Trim().Equals(urlLang.Trim()))
        //    {
        //        //Set lang
        //        Response.Cookies.Append(
        //            "LanguageOldId", lang + "",
        //            new CookieOptions
        //            {
        //                Expires = DateTimeOffset.UtcNow.AddYears(1),
        //                IsEssential = true,
        //                Path = "/",
        //                HttpOnly = false,
        //            }
        //        );

        //        if (lang == 1)
        //        {
        //            return Redirect("/" + Const.PAGE_NOMAL_CONTACT_VN);
        //        }
        //        else if (lang == 1007)
        //        {
        //            return Redirect("/"+ Const.PAGE_NOMAL_CONTACT);
        //        }

        //    }

        //    //Lấy ra danh sách chi nhánh
        //    using (var db = new IOITDataContext())
        //    {
        //        var branch = db.Branch.Where(e => e.Status == (int)Const.Status.NORMAL).ToList();

        //        return View(branch);
        //    }
        //}

        public ActionResult CMS()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult ListMerchant()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Partner()
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    //int np = 6;
                    int id = 1;
                    //if (p < 1 || idw != Const.WEBSITEID)
                    //{
                    //    return Redirect("/Home/Error");
                    //}

                    var seo = db.Category.Where(e => e.CategoryId >= id
                    && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_TEXT
                    && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                    && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                    if (seo == null)
                        return Redirect("/Home/Error");

                    //string strSEO = seo.Url;
                    //if (seoName != strSEO)
                    //    return RedirectToRoute("News", new { id, seoName = strSEO, idw, p });

                    var data = (from cm in db.CategoryMapping
                                join n in db.News on cm.TargetId equals n.NewsId
                                where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                && n.CompanyId == Const.COMPANYID
                                && cm.CategoryId == id
                                && n.WebsiteId == Const.WEBSITEID
                                && n.Status == (int)Const.Status.NORMAL
                                && cm.Status != (int)Const.Status.DELETED
                                select n).OrderByDescending(e => e.CreatedAt).ToList();

                    //if (((data.Count() - 1) / np) + 1 < p)
                    //{
                    //    return Redirect("/Home/Error");
                    //}

                    //if (p == 1)
                    //{
                    ViewBag.SeoTitle = seo.MetaTitle;
                    ViewBag.SeoDescription = seo.MetaDescription;
                    ViewBag.SeoKeywords = seo.MetaKeyword;
                    //}
                    //else
                    //{
                    //    ViewBag.SeoTitle = seo.MetaTitle + " " + p;
                    //    ViewBag.SeoDescription = seo.MetaDescription + " " + p;
                    //    ViewBag.SeoKeywords = seo.MetaKeyword;
                    //}

                    ViewBag.Url = _configuration["Settings:Domain"] + "doi-tac.html";
                    if (seo.Image != "")
                        ViewBag.UrlImg = _configuration["Settings:Domain"] + "/uploads/" + seo.Image;
                    else
                        ViewBag.UrlImg = _configuration["Settings:Domain"] + "/Content/images/logo.svg";

                    ViewBag.CategoryParentId = seo.CategoryParentId;
                    ViewBag.CategoryId = seo.CategoryId;
                    if (seo.CategoryParentId != 0)
                    {
                        var cate = db.Category.Find(seo.CategoryParentId);
                        ViewBag.CategoryParentName = cate.Name;
                        ViewBag.CategoryParentUrl = cate.Url;
                    }

                    //ViewBag.Id = seo.Id;
                    ViewBag.CategoryName = seo.Name;
                    //ViewBag.Url = seo.Url;
                    ViewBag.Description = seo.Description;
                    //ViewBag.Page = p;

                    //ViewBag.CountAll = (_data2.Count - 1).ToString().Trim();
                    //ViewBag.total = (data.Count() - 1);
                    //ViewBag.page = p;
                    //ViewBag.Pre = p - 1;
                    //ViewBag.Next = p + 1;
                    //var list = data.Skip(np * (p - 1)).Take(np).ToList();
                    return View("Partner", data);
                }
            }
            catch (Exception e)
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult News()
        {
            string title = "Tin tức mới nhất từ CashPlus";
            ViewBag.SeoTitle = title;
            ViewBag.SeoDescription = title;
            ViewBag.SeoKeywords = title;
            using (var db = new IOITDataContext())
            {
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                ViewBag.Logo = website.LogoHeader;

                // BỔ SUNG: Lấy danh sách tin mới nhất cho view
                var newsList = db.News
                    .Where(n =>
                        (n.TypeNewsId == (int)Const.TypeNews.NEWS_TEXT || n.TypeNewsId == (int)Const.TypeNews.NEWS_NEWS)
                        && n.CompanyId == Const.COMPANYID
                        && n.WebsiteId == Const.WEBSITEID
                        && n.Status == (int)Const.Status.NORMAL
                        && n.DateStartActive <= DateTime.Now
                    )
                    .OrderByDescending(n => n.DateStartActive)
                    .Take(30)
                    .ToList();
                ViewBag.NewsList = newsList;
                return View(newsList);
            }
        }
        public async Task<ActionResult> DataPage()
        {

            ViewBag.SeoTitle = "Trang Dữ Liệu";
            ViewBag.SeoDescription = "Trang Dữ Liệu";
            ViewBag.SeoKeywords = "Trang Dữ Liệu";
            ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
            using (var db = new IOITDataContext())
            {
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                ViewBag.Logo = website.LogoHeader;
                //Lấy PVUD đầu tiên
                //var ar = await db.Category.Where(e => e.Status == (int)Const.Status.NORMAL
                //&& e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE).OrderBy(e => e.Location).FirstOrDefaultAsync();
                //ViewBag.ArId = ar != null ? ar.CategoryId : 0;
                ViewBag.ArId = 0;
                //HttpContext.Session.SetString("ArId", ViewBag.ArId);
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
                Response.Cookies.Append(
                         "NameAr", "",
                         new CookieOptions
                         {
                             Expires = DateTimeOffset.UtcNow.AddYears(1),
                             IsEssential = true,
                             Path = "/",
                             HttpOnly = false,
                         }
                     );
            }
            ViewBag.Class = "body-data-page";
            return View();
        }
        public async Task<ActionResult> ListDataTq(int id, int p = 1)
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    ViewBag.ArId = id;
                    //Response.Cookies.Append(
                    //        "ArId", id + "",
                    //        new CookieOptions
                    //        {
                    //            Expires = DateTimeOffset.UtcNow.AddYears(1),
                    //            IsEssential = true,
                    //            Path = "/",
                    //            HttpOnly = false,
                    //        }
                    //    );
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    ViewBag.CustomerId = HttpContext.Session.GetInt32("CustomerId");
                    ViewBag.access_token = "'" + HttpContext.Session.GetString("access_token") + "'";
                    ViewBag.p = p;
                    int np = 10;
                    ViewBag.np = np;
                    IEnumerable<DataSetDTO> data = (from ds in db.DataSet
                                                    join dsm in db.DataSetMapping on ds.DataSetId equals dsm.DataSetId
                                                    where ds.Status == (int)Const.Status.NORMAL
                                                    && (dsm.TargetId == id || id == 0)
                                                    && dsm.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE
                                                    && dsm.Status != (int)Const.Status.DELETED
                                                    group ds by new
                                                    {
                                                        ds.DataSetId,
                                                        ds.CreatedAt
                                                    } into e
                                                    select new DataSetDTO
                                                    {
                                                        DataSetId = e.Key.DataSetId,
                                                        CreatedAt = e.Key.CreatedAt,
                                                    }).OrderByDescending(e => e.CreatedAt).ToList();

                    ViewBag.total = (data.Count() - 1);
                    ViewBag.page = p;

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

                    return PartialView("_ListDataTq", list);
                }
            }
            catch
            {
                //return Redirect("/Home/Error");
                return null;
            }
        }

        public async Task<ActionResult> ListTopDown(int id, int n = 5)
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    var data = await (from e in db.DataSet
                                      join dsm in db.DataSetMapping on e.DataSetId equals dsm.DataSetId
                                      where e.Status == (int)Const.Status.NORMAL
                                      && (dsm.TargetId == id || id == 0)
                                      && dsm.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE
                                      && dsm.Status != (int)Const.Status.DELETED
                                      select new DataSetDTO
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

                                      }).OrderByDescending(c => c.DownNumber).Take(n).ToListAsync();

                    return PartialView("_ListTopDown", data);
                }
            }
            catch
            {
                //return Redirect("/Home/Error");
                return null;
            }
        }

        public async Task<ActionResult> ListTopView(int id, int n = 5)
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    var data = await (from e in db.DataSet
                                      join dsm in db.DataSetMapping on e.DataSetId equals dsm.DataSetId
                                      where e.Status == (int)Const.Status.NORMAL
                                      && (dsm.TargetId == id || id == 0)
                                      && dsm.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE
                                      && dsm.Status != (int)Const.Status.DELETED
                                      select new DataSetDTO
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

                                      }).OrderByDescending(c => c.ViewNumber).Take(n).ToListAsync();

                    return PartialView("_ListTopView", data);
                }
            }
            catch
            {
                //return Redirect("/Home/Error");
                return null;
            }
        }

        public async Task<ActionResult> DataPageAll()
        {
            ViewBag.SeoTitle = "Toàn bộ dữ liệu";
            ViewBag.SeoDescription = "Toàn bộ dữ liệu";
            ViewBag.SeoKeywords = "Toàn bộ dữ liệu";
            ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
            using (var db = new IOITDataContext())
            {
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
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                ViewBag.Logo = website.LogoHeader;
                //ViewBag.ArId = Request.Cookies["ArId"] != null ? int.Parse(Request.Cookies["ArId"]) : 0;
                ViewBag.ArId = Request.Cookies["ArId"] != null ? int.Parse(Request.Cookies["ArId"]) : 0;
                //ViewBag.ArId = 0;
                ViewBag.NameAr = Request.Cookies["NameAr"] != null ? Request.Cookies["NameAr"] : "";
                ViewBag.NameAr2 = "'" + ViewBag.NameAr + "'";
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
        public async Task<ActionResult> ListDataAll(int id, int p = 1, int ex = -1, int uid = -1, int aid = -1, int od = 1, string textS = "")
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    if (id == -2)
                    {
                        id = Request.Cookies["ArId"] != null ? int.Parse(Request.Cookies["ArId"]) : 0;
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
                        textS = "";//Request.Cookies["NameAr"] != null ? Request.Cookies["NameAr"] : "";
                    }

                    Response.Cookies.Append(
                       "NameAr", textS,
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
                                      && (dsm.TargetId == id || id == 0)
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

                    return PartialView("_ListDataAll", list);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<ActionResult> DataForUser(int id)
        {
            using (var db = new IOITDataContext())
            {
                var customer = await db.Customer.Where(e => e.CustomerId == id).FirstOrDefaultAsync();
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                ViewBag.Logo = website.LogoHeader;
                ViewBag.Id = id;
                ViewBag.SeoTitle = "Dữ liệu cá nhân " + customer.FullName;
                ViewBag.SeoDescription = "Dữ liệu cá nhân " + customer.FullName;
                ViewBag.SeoKeywords = "Dữ liệu cá nhân " + customer.FullName;

                //Tổng số bộ dữ liệu
                ViewBag.DataNumber = await (from ds in db.DataSet
                                            where ds.Status == (int)Const.Status.NORMAL
                                            && ds.UserCreatedId == id
                                            select ds).CountAsync();
                //

                ViewBag.PositionName = await db.TypeAttributeItem.Where(e => e.TypeAttributeItemId == customer.PositionId).Select(e => e.Name).FirstOrDefaultAsync();
                ViewBag.AcademicRank = await db.TypeAttributeItem.Where(e => e.TypeAttributeItemId == customer.AcademicRankId).Select(e => e.Name).FirstOrDefaultAsync();
                ViewBag.DegreeName = await db.TypeAttributeItem.Where(e => e.TypeAttributeItemId == customer.DegreeId).Select(e => e.Name).FirstOrDefaultAsync();
                ViewBag.CountryName = await db.Country.Where(e => e.CountryId == customer.CountryId).Select(e => e.Name).FirstOrDefaultAsync();
                ViewBag.listLvnc = await (from cm in db.CustomerMapping
                                          join c in db.Category on cm.TargetId equals c.CategoryId
                                          where cm.Status != (int)Const.Status.DELETED
                                          && cm.CustomerId == id
                                          && cm.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION
                                          select new CategoryDTL
                                          {
                                              CategoryId = c.CategoryId,
                                              Code = c.Code,
                                              Name = c.Name,
                                              Url = c.Url,
                                              Location = c.Location,
                                          }).OrderByDescending(e => e.Location).ToListAsync();
                //Lấy ds phạm vi ứng dụng
                ViewBag.listAr = await db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE
                && e.Status == (int)Const.Status.NORMAL).OrderBy(e => e.Location).ToListAsync();
                //Lấy ds lĩnh vực nghiên cứu
                ViewBag.listRa = await db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA
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
                return View(customer);
            }
        }

        public async Task<ActionResult> ListDataUser(int id, int p = 1, int ex = -1, int aid = -1, int idc = 0, string textS = "")
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    if (id == -2)
                    {
                        id = Request.Cookies["UArId"] != null ? int.Parse(Request.Cookies["UArId"]) : -1;
                    }
                    else
                    {
                        Response.Cookies.Append(
                            "UArId", id + "",
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
                        ex = Request.Cookies["UDsEx"] != null ? int.Parse(Request.Cookies["UDsEx"]) : -1;
                    }
                    else
                    {
                        Response.Cookies.Append(
                           "UDsEx", ex + "",
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
                        aid = Request.Cookies["URaId"] != null ? int.Parse(Request.Cookies["URaId"]) : -1;
                    }
                    else
                    {
                        Response.Cookies.Append(
                           "URaId", aid + "",
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
                        textS = "";//Request.Cookies["NameAr"] != null ? Request.Cookies["NameAr"] : "";
                    }

                    Response.Cookies.Append(
                       "NameAr", textS,
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
                    var data = await (from ds in db.DataSet
                                      where ds.Status == (int)Const.Status.NORMAL
                                      && ds.UserCreatedId == idc
                                      && (ds.Title.Contains(textS) || textS == "")
                                      select new DataSetDTO
                                      {
                                          DataSetId = ds.DataSetId,
                                          PublishedAt = ds.PublishedAt,
                                      }).ToListAsync();
                    if (ex != -1 || id != -1 || aid != -1)
                    {
                        data = (from ds in db.DataSet
                                join dar in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE)
                                on ds.DataSetId equals dar.DataSetId
                                join dra in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA)
                                on ds.DataSetId equals dra.DataSetId
                                join at in db.Attactment.Where(e => e.TargetType == (int)Const.TypeAttachment.FILE_DATASET)
                                on ds.DataSetId equals at.TargetId into atj
                                from at in atj.DefaultIfEmpty()
                                where ds.Status == (int)Const.Status.NORMAL
                                && ds.UserCreatedId == idc
                                && (ds.Title.Contains(textS) || textS == "")
                                && dar.Status != (int)Const.Status.DELETED
                                && dra.Status != (int)Const.Status.DELETED
                                && (at == null || at.Status != (int)Const.Status.DELETED) // Filter null values
                                && (dar.TargetId == id || id == -1)
                                && (dra.TargetId == aid || aid == -1)
                                && ((at != null ? at.Extension == ex : ex == -1) || ex == -1) // Optional chaining operator for null values
                                group ds by new
                                {
                                    ds.DataSetId,
                                    ds.PublishedAt
                                } into e
                                select new DataSetDTO
                                {
                                    DataSetId = e.Key.DataSetId,
                                    PublishedAt = e.Key.PublishedAt,
                                }).ToList();

                    }

                    data = data.OrderByDescending(e => e.PublishedAt).ToList();

                    //ViewBag.total = (data.Count());
                    //ViewBag.page = p;
                    //if ((ViewBag.total - np) % np == 0)
                    //{
                    //    ViewBag.totalPage = (ViewBag.total) / np;
                    //}
                    //else
                    //{
                    //    ViewBag.totalPage = ((ViewBag.total) / np) + 1;
                    //}
                    // hien thi so page
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

                    return PartialView("_ListDataUser", list);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<ActionResult> DataForUnit(int id)
        {
            using (var db = new IOITDataContext())
            {
                var unit = await db.Unit.Where(e => e.UnitId == id).FirstOrDefaultAsync();
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                ViewBag.Logo = website.LogoHeader;
                ViewBag.Id = id;
                ViewBag.SeoTitle = "Dữ liệu cơ quan tổ chức " + unit.Name;
                ViewBag.SeoDescription = "Dữ liệu cơ quan tổ chức " + unit.Name;
                ViewBag.SeoKeywords = "Dữ liệu cơ quan tổ chức " + unit.Name;

                //Tổng số bộ dữ liệu
                ViewBag.DataNumber = await (from ds in db.DataSet
                                            join dsm in db.DataSetMapping on ds.DataSetId equals dsm.DataSetId
                                            where ds.Status == (int)Const.Status.NORMAL
                                            && ds.Type == (int)Const.DataSetType.DATA_UNIT
                                            && dsm.Status != (int)Const.Status.DELETED
                                            && dsm.TargetId == id
                                            && dsm.TargetType == (int)Const.DataSetMapping.DATA_UNIT
                                            select ds).CountAsync();
                //

                ViewBag.ProvinceName = await db.Province.Where(e => e.ProvinceId == unit.ProvinceId).Select(e => e.Name).FirstOrDefaultAsync();
                ViewBag.DistrictName = await db.District.Where(e => e.DistrictId == unit.DistrictId).Select(e => e.Name).FirstOrDefaultAsync();
                ViewBag.WardsName = await db.Wards.Where(e => e.WardId == unit.WardId).Select(e => e.Name).FirstOrDefaultAsync();
                string adress = unit.Address;
                if (ViewBag.WardsName != null) adress += "," + ViewBag.WardsName;
                if (ViewBag.DistrictName != null) adress += "," + ViewBag.DistrictName;
                if (ViewBag.ProvinceName != null) adress += "," + ViewBag.ProvinceName;

                //ViewBag.CountryName = await db.Country.Where(e => e.CountryId == customer.CountryId).Select(e => e.Name).FirstOrDefaultAsync();
                ViewBag.listLvnc = await (from cm in db.CustomerMapping
                                          join c in db.Category on cm.TargetId equals c.CategoryId
                                          where cm.Status != (int)Const.Status.DELETED
                                          && cm.CustomerId == id
                                          && cm.TargetType == (int)Const.TargetCustomerMapping.CUSTOMER_APPLICATION
                                          select new CategoryDTL
                                          {
                                              CategoryId = c.CategoryId,
                                              Code = c.Code,
                                              Name = c.Name,
                                              Url = c.Url,
                                              Location = c.Location,
                                          }).OrderByDescending(e => e.Location).ToListAsync();
                //Lấy ds phạm vi ứng dụng
                ViewBag.listAr = await db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_APPLICATION_RANGE
                && e.Status == (int)Const.Status.NORMAL).OrderBy(e => e.Location).ToListAsync();
                //Lấy ds lĩnh vực nghiên cứu
                ViewBag.listRa = await db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_RESEARCH_AREA
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
                return View(unit);
            }
        }

        public async Task<ActionResult> ListDataUnit(int id, int p = 1, int ex = -1, int aid = -1, int idu = 0, string textS = "")
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    if (id == -2)
                    {
                        id = Request.Cookies["UtArId"] != null ? int.Parse(Request.Cookies["UtArId"]) : -1;
                    }
                    else
                    {
                        Response.Cookies.Append(
                            "UtArId", id + "",
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
                        ex = Request.Cookies["UtDsEx"] != null ? int.Parse(Request.Cookies["UtDsEx"]) : -1;
                    }
                    else
                    {
                        Response.Cookies.Append(
                           "UtDsEx", ex + "",
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
                        aid = Request.Cookies["UtRaId"] != null ? int.Parse(Request.Cookies["UtRaId"]) : -1;
                    }
                    else
                    {
                        Response.Cookies.Append(
                           "UtRaId", aid + "",
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
                        textS = "";//Request.Cookies["NameAr"] != null ? Request.Cookies["NameAr"] : "";
                    }

                    Response.Cookies.Append(
                       "NameAr", textS,
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
                    var data = await (from ds in db.DataSet
                                      join dsm in db.DataSetMapping on ds.DataSetId equals dsm.DataSetId
                                      where ds.Status == (int)Const.Status.NORMAL
                                      && dsm.Status != (int)Const.Status.DELETED
                                      && dsm.TargetId == idu
                                      && dsm.TargetType == (int)Const.DataSetMapping.DATA_UNIT
                                      && (ds.Title.Contains(textS) || textS == "")
                                      select new DataSetDTO
                                      {
                                          DataSetId = ds.DataSetId,
                                          PublishedAt = ds.PublishedAt,
                                      }).ToListAsync();
                    if (ex != -1 || id != -1 || aid != -1)
                    {
                        data = (from ds in db.DataSet
                                join dar in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE)
                                on ds.DataSetId equals dar.DataSetId
                                join dra in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA)
                                on ds.DataSetId equals dra.DataSetId
                                join dsm in db.DataSetMapping.Where(e => e.TargetType == (int)Const.DataSetMapping.DATA_UNIT)
                                on ds.DataSetId equals dsm.DataSetId
                                join at in db.Attactment.Where(e => e.TargetType == (int)Const.TypeAttachment.FILE_DATASET)
                                on ds.DataSetId equals at.TargetId into atj
                                from at in atj.DefaultIfEmpty()
                                where ds.Status == (int)Const.Status.NORMAL
                                && dsm.Status != (int)Const.Status.DELETED
                                && dsm.TargetId == idu
                                && (ds.Title.Contains(textS) || textS == "")
                                && dar.Status != (int)Const.Status.DELETED
                                && dra.Status != (int)Const.Status.DELETED
                                && (at == null || at.Status != (int)Const.Status.DELETED) // Filter null values
                                && (dar.TargetId == id || id == -1)
                                && (dra.TargetId == aid || aid == -1)
                                && ((at != null ? at.Extension == ex : ex == -1) || ex == -1) // Optional chaining operator for null values
                                group ds by new
                                {
                                    ds.DataSetId,
                                    ds.PublishedAt
                                } into e
                                select new DataSetDTO
                                {
                                    DataSetId = e.Key.DataSetId,
                                    PublishedAt = e.Key.PublishedAt,
                                }).ToList();

                    }

                    data = data.OrderByDescending(e => e.PublishedAt).ToList();

                    // hien thi so page
                    ViewBag.total = (data.Count() - 1);
                    ViewBag.page = p;

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

                    return PartialView("_ListDataUnit", list);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public ActionResult ViewInfoUser()
        {
            ViewBag.SeoTitle = "Thông tin cá nhân";
            ViewBag.SeoDescription = "Thông tin cá nhân";
            ViewBag.SeoKeywords = "Thông tin cá nhân";
            return View();
        }

        public ActionResult ContactFaQ(int p = 1)
        {
            ViewBag.SeoTitle = "Trang FAQ";
            ViewBag.SeoDescription = "Trang FAQ";
            ViewBag.SeoKeywords = "Trang FAQ";
            ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
            using (var db = new IOITDataContext())
            {
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                ViewBag.Logo = website.LogoHeader;
                int np = 10;
                ViewBag.np = np;

                if (p == 1)
                {
                    ViewBag.SeoTitle = "Trang FAQ";
                    ViewBag.SeoDescription = "Trang FAQ";
                    ViewBag.SeoKeywords = "Trang FAQ";
                }
                else
                {
                    ViewBag.SeoTitle = "Trang FAQ";
                    ViewBag.SeoDescription = "Trang FAQ" + " " + p;
                    ViewBag.SeoKeywords = "lien-he-faq";
                }
                ViewBag.Class = "body-faq-page";
                return View();
            }
        }
        public ActionResult ListFaqs(int p = 1, string textS = "")
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    ViewBag.p = p;
                    int np = 10;
                    ViewBag.np = np;
                    if (textS == null) textS = "";
                    IEnumerable<LegalDoc> data = (from n in db.LegalDoc
                                                  where n.Status == (int)Const.Status.NORMAL
                                                  && (n.Name.Contains(textS) || textS == "")
                                                  select n).OrderByDescending(e => e.CreatedAt).ToList();

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

                    return PartialView("_ListFaqs", list);
                }
            }
            catch
            {
                //return Redirect("/Home/Error");
                return null;
            }
        }
        public ActionResult ContactPage()
        {
            ViewBag.SeoTitle = "Liên hệ";
            ViewBag.SeoDescription = "Liên hệ";
            ViewBag.SeoKeywords = "Liên hệ";
            using (var db = new IOITDataContext())
            {
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                ViewBag.Logo = website.LogoHeader;
                return View(website);
            }

        }
        public ActionResult RouterDev()
        {
            ViewBag.SeoTitle = "Liên kết template";
            ViewBag.SeoDescription = "Liên kết template";
            ViewBag.SeoKeywords = "Liên kết template";
            return View();
        }

        public ActionResult Service()
        {
            ViewBag.SeoTitle = "Dịch vụ";
            ViewBag.Message = "Dịch vụ";
            using (var db = new IOITDataContext())
            {
                IEnumerable<News> data = (from n in db.News
                                          where n.Status != (int)Const.Status.DELETED
                                          select n).OrderBy(e => e.Location).Take(3).ToList();

                return View(data);
            }

        }

        public ActionResult LibraryImage(int p = 1)
        {
            ViewBag.SeoTitle = "Thư viện ảnh";
            ViewBag.SeoDescription = "Thư viện ảnh";
            ViewBag.SeoKeywords = "Thư viện ảnh";

            try
            {
                using (var db = new IOITDataContext())
                {
                    int np = 12;

                    var data = (from n in db.News
                                where n.Status == (int)Const.Status.NORMAL
                                && n.TypeNewsId == (int)Const.TypeNews.NEWS_IMAGE
                                select n).OrderByDescending(e => e.CreatedAt).ToList();

                    if (((data.Count() - 1) / np) + 1 < p)
                    {
                        return Redirect("/Home/Error");
                    }

                    ViewBag.total = (data.Count() - 1);
                    ViewBag.page = p;
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;
                    var list = data.Skip(np * (p - 1)).Take(np).Select(e => new NewsDTO
                    {
                        NewsId = e.NewsId,
                        Title = e.Title,
                        Description = e.Description,
                        Image = e.Image,
                        listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).Select(a => new AttactmentDTO
                        {
                            Url = a.Url
                        }).ToList()
                    }).ToList();
                    return View(list);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult LibraryVideo(int p = 1)
        {
            ViewBag.SeoTitle = "Thư viện video";
            ViewBag.SeoDescription = "Thư viện video";
            ViewBag.SeoKeywords = "Thư viện video";

            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    int np = 9;

                    var data = (from n in db.News
                                where n.Status == (int)Const.Status.NORMAL
                                && n.TypeNewsId == (int)Const.TypeNews.NEWS_VIDEO
                                select n).OrderByDescending(e => e.CreatedAt).ToList();

                    if (((data.Count() - 1) / np) + 1 < p)
                    {
                        return Redirect("/Home/Error");
                    }

                    ViewBag.total = (data.Count() - 1);
                    ViewBag.page = p;
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;
                    var list = data.Skip(np * (p - 1)).Take(np).Select(e => new NewsDTO
                    {
                        NewsId = e.NewsId,
                        Title = e.Title,
                        Description = e.Description,
                        LinkVideo = e.LinkVideo
                    }).ToList();
                    return View(list);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Profile()
        {
            var CustomerId = HttpContext.Session.GetInt32("CustomerId");
            if (CustomerId == null)
            {
                return Redirect("/Home/Error");
            }
            ViewBag.SeoTitle = "Thông tin cá nhân";
            ViewBag.Message = "Thông tin cá nhân";

            return View();

        }

        // Van ban
        public ActionResult LegalDoc(string textSearch = "", int typeText = -1, int agencyIssue = -1, int yearIssue = -1, int p = 1)
        {
            try
            {
                ViewBag.SeoTitle = "Văn bản";
                ViewBag.SeoDescription = ViewBag.SeoTitle;
                ViewBag.SeoKeywords = ViewBag.SeoTitle;
                ViewBag.Class = "body-detail";
                ViewBag.textSearch = textSearch != "" ? textSearch : null;
                if (agencyIssue != -1) ViewBag.agencyIssue = agencyIssue;
                if (typeText != -1) ViewBag.typeText = typeText;
                if (yearIssue != -1) ViewBag.yearIssue = yearIssue;

                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    List<TypeAttributeItem> typeAttributeItems = db.TypeAttributeItem.Where(t => (t.TypeAttributeId == 4) && t.Status != (int)Const.Status.DELETED).ToList();
                    ViewBag.typeTexts = typeAttributeItems.Where(t => t.TypeAttributeId == 4).ToList();

                    ViewBag.agencyIssues = typeAttributeItems.Where(t => t.TypeAttributeId == 4).ToList();

                    List<int> yearIssues = new List<int>();
                    int Year = DateTime.Now.Year;
                    for (var i = Year; i >= Year - 30; i--)
                    {
                        yearIssues.Add(i);
                    }
                    ViewBag.yearIssues = yearIssues;

                    int np = 10;
                    ViewBag.np = np;

                    List<LegalDoc> data = db.LegalDoc.Where(c => c.Status != (int)Const.Status.DELETED).ToList();

                    if (textSearch != "")
                    {
                        data = data.Where(l => l.Code.ToLower().Contains(textSearch.ToLower()) || l.Name.ToLower().Contains(textSearch.ToLower())).ToList();
                    }

                    if (typeText != -1)
                    {
                        data = data.Where(l => l.TypeText == typeText).ToList();
                    }

                    if (agencyIssue != -1)
                    {
                        data = data.Where(l => l.AgencyIssue == agencyIssue).ToList();
                    }

                    if (yearIssue != -1)
                    {
                        data = data.Where(l => l.YearIssue == yearIssue).ToList();
                    }


                    //if (((data.Count() - 1) / np) + 1 < p)
                    //{
                    //    return Redirect("/Home/Error");
                    //}

                    ViewBag.total = (data.Count() - 1);
                    ViewBag.page = p;
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;
                    var list = data.Skip(np * (p - 1)).Take(np).ToList();

                    return View(list);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }
        //end

        [HttpPost]
        public IActionResult SetLanguage(string culture, string id, string returnUrl)
        {
            int lang = Request.Cookies["LanguageId"] != null ? int.Parse(_httpContext.HttpContext.Request.Cookies["LanguageId"]) : Const.LANGUAGEID;
            _httpContext.HttpContext.Response.Cookies.Append(
                            "LanguageOldId", lang + "",
                            new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddYears(1),
                                IsEssential = true,
                                Path = "/",
                                HttpOnly = false,
                            }
                        );
            _httpContext.HttpContext.Response.Cookies.Append(
                "Language", culture,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    Path = "/",
                    HttpOnly = false,
                }
            );
            _httpContext.HttpContext.Response.Cookies.Append(
                "LanguageId", id,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    Path = "/",
                    HttpOnly = false,
                }
            );

            _httpContext.HttpContext.Response.Cookies.Append(
                "ReturnUrl", returnUrl,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    Path = "/",
                    HttpOnly = false,
                }
            );

            return LocalRedirect(returnUrl);



        }

        public ActionResult FAQ(int p = 1)
        {
            ViewBag.SeoTitle = "FAQ - Giải đáp câu hỏi thường gặp trong CashPlus";
            ViewBag.Message = "FAQ - Giải đáp câu hỏi thường gặp trong CashPlus";

            using (var db = new IOITDataContext())
            {
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                int np = 8;
                ViewBag.np = np;
                ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";                

                if (p == 1)
                {
                    ViewBag.SeoTitle = "FAQ - Giải đáp câu hỏi thường gặp trong CashPlus";
                    ViewBag.SeoDescription = "FAQ - Giải đáp câu hỏi thường gặp trong CashPlus";
                    ViewBag.SeoKeywords = "cau hoi, faq, cashplus";
                }
                else
                {
                    ViewBag.SeoTitle = "FAQ - Giải đáp câu hỏi thường gặp trong CashPlus";
                    ViewBag.SeoDescription = "FAQ - Giải đáp câu hỏi thường gặp trong CashPlus" + " " + p;
                    ViewBag.SeoKeywords = "cauhoi, faq, cashplus";
                }
                ViewBag.Class = "body-detail";                
                return View();
            }
        }

        public ActionResult ListPublication(string textS = "", int year = -1, int author = -1, int department = -1, int p = 1)
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    ViewBag.p = p;
                    int np = 8;
                    ViewBag.np = np;
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    if (textS == null) textS = "";
                    //Lấy ra dữ liệu

                    var data = db.Publication.Where(pr =>
                    (pr.PublishingYear == year || year == -1)
                    //&& (pr.NumberOfTopic == topic || topic == -1)
                    && (pr.Author == author || author == -1)
                    && (pr.Department == department || department == -1)
                    && (pr.Url.Contains(textS) || textS == "")
                    && pr.Status == (int)Const.Status.NORMAL).OrderByDescending(e => e.CreatedAt).ToList();

                    //if (((data.Count() - 1) / np) + 1 < p)
                    //{
                    //    return Redirect("/Home/Error");
                    //}

                    ViewBag.total = (data.Count());
                    ViewBag.page = p;
                    if ((ViewBag.total - 4) % np == 0)
                    {
                        ViewBag.totalPage = (ViewBag.total) / np;

                    }
                    else
                    {
                        ViewBag.totalPage = ((ViewBag.total) / np) + 1;
                    }
                    // hien thi so page
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;

                    var list = data.Count() > 0 ? data.Skip(np * (p - 1)).Take(np).ToList() : data.ToList();

                    return PartialView("_ListPublication", list);
                }
            }
            catch
            {
                //return Redirect("/Home/Error");
                return null;
            }
        }

        public ActionResult LegalDocs(int p = 1)
        {
            ViewBag.SeoTitle = "Quản lý văn bản";
            ViewBag.Message = "Quản lý văn bản";
            ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
            using (var db = new IOITDataContext())
            {
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                int np = 8;
                ViewBag.np = np;
                //IEnumerable<Publication> data = db.Publication.Where(e => e.Status != (int)Const.Status.DELETED).OrderByDescending(e => e.CreatedAt).ToList();
                //if (((data.Count() - 1) / np) + 1 < p)
                //{
                //    return Redirect("/Home/Error");
                //}

                if (p == 1)
                {
                    ViewBag.SeoTitle = "Quản lý văn bản bản";
                    ViewBag.SeoDescription = "Quản lý văn bản";
                    ViewBag.SeoKeywords = "văn bản";
                }
                else
                {
                    ViewBag.SeoTitle = "Quản lý văn bản bản";
                    ViewBag.SeoDescription = "Quản lý văn bản" + " " + p;
                    ViewBag.SeoKeywords = "an-pham";
                }
                ViewBag.Class = "body-detail";
                //ViewBag.total = (data.Count());
                //ViewBag.page = p;
                //if ((ViewBag.total - 4) % 8 == 0)
                //{
                //    ViewBag.totalPage = (ViewBag.total) / 8;

                //}
                //else
                //{
                //    ViewBag.totalPage = ((ViewBag.total) / 8) + 1;
                //}
                //// hien thi so page

                //ViewBag.Pre = p - 1;
                //ViewBag.Next = p + 1;
                //var list = data.Skip((np * (p - 1))).Take(np).ToList();
                return View();
            }
        }

        public ActionResult ListLegalDocs(string textS = "", int cateCq = -1, int departments = -1, string tieuChi = "",
            string dateStart = "", string dateEnd = "", int p = 1)
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    dateStart = dateStart != null ? dateStart : "";
                    dateEnd = dateEnd != null ? dateEnd : "";
                    ViewBag.p = p;
                    int np = 8;
                    ViewBag.np = np;
                    textS = textS != null ? textS : "";
                    //int cateCq = input.cateCq != null ? (int)input.cateCq : -1;
                    //int departments = input.departments != null ? (int)input.departments : -1;
                    ////Lấy ra dữ liệu
                    if (tieuChi == "title")
                    {
                        if (cateCq != -1)
                        {
                            IEnumerable<LegalDoc> data = (from cm in db.CategoryMapping
                                                          join n in db.LegalDoc on cm.TargetId equals n.LegalDocId
                                                          where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_LEGAL_DOC
                                                          && cm.CategoryId == cateCq
                                                          && n.Status == (int)Const.Status.NORMAL
                                                          && cm.Status != (int)Const.Status.DELETED
                                                          && (n.Name.Contains(textS) || textS == "")
                                                          && (n.AgencyIssued == departments || departments == -1)
                                                          && ((n.DateIssue >= DateTime.Parse(dateStart + " 0:0:0") && n.DateIssue <= DateTime.Parse(dateEnd + " 23:59:59"))
                                                          || dateStart == "" || dateEnd == "")
                                                          select n).OrderByDescending(e => e.DateIssue).ToList();
                            ViewBag.total = (data.Count());
                            ViewBag.page = p;
                            if ((ViewBag.total - 4) % np == 0)
                            {
                                ViewBag.totalPage = (ViewBag.total) / np;

                            }
                            else
                            {
                                ViewBag.totalPage = ((ViewBag.total) / np) + 1;
                            }
                            // hien thi so page
                            ViewBag.Pre = p - 1;
                            ViewBag.Next = p + 1;

                            var list = data.Count() > 0 ? data.Skip(np * (p - 1)).Take(np).ToList() : data.ToList();
                            return PartialView("_ListLegalDocs", list);
                        }
                        else
                        {
                            IEnumerable<LegalDoc> data = (from n in db.LegalDoc
                                                          where n.Status == (int)Const.Status.NORMAL
                                                          && (n.Name.Contains(textS) || textS == "")
                                                          && (n.AgencyIssued == departments || departments == -1)
                                                          && ((n.DateIssue >= DateTime.Parse(dateStart + " 0:0:0") && n.DateIssue <= DateTime.Parse(dateEnd + " 23:59:59"))
                                                          || dateStart == "" || dateEnd == "")
                                                          select n).OrderByDescending(e => e.DateIssue).ToList();
                            ViewBag.total = (data.Count());
                            ViewBag.page = p;
                            if ((ViewBag.total - 4) % np == 0)
                            {
                                ViewBag.totalPage = (ViewBag.total) / np;

                            }
                            else
                            {
                                ViewBag.totalPage = ((ViewBag.total) / np) + 1;
                            }
                            // hien thi so page
                            ViewBag.Pre = p - 1;
                            ViewBag.Next = p + 1;

                            var list = data.Count() > 0 ? data.Skip(np * (p - 1)).Take(np).ToList() : data.ToList();
                            return PartialView("_ListLegalDocs", list);
                        }
                    }
                    else if (tieuChi == "trich_yeu")
                    {
                        if (cateCq != -1)
                        {
                            IEnumerable<LegalDoc> data = (from cm in db.CategoryMapping
                                                          join n in db.LegalDoc on cm.TargetId equals n.LegalDocId
                                                          where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_LEGAL_DOC
                                                          && cm.CategoryId == cateCq
                                                          && n.Status == (int)Const.Status.NORMAL
                                                          && cm.Status != (int)Const.Status.DELETED
                                                          && (n.TichYeu.Contains(textS) || textS == "")
                                                          && (n.AgencyIssued == departments || departments == -1)
                                                          && ((n.DateIssue >= DateTime.Parse(dateStart + " 0:0:0") && n.DateIssue <= DateTime.Parse(dateEnd + " 23:59:59"))
                                                          || dateStart == "" || dateEnd == "")
                                                          select n).OrderByDescending(e => e.DateIssue).ToList();


                            ViewBag.total = (data.Count());
                            ViewBag.page = p;
                            if ((ViewBag.total - 4) % np == 0)
                            {
                                ViewBag.totalPage = (ViewBag.total) / np;

                            }
                            else
                            {
                                ViewBag.totalPage = ((ViewBag.total) / np) + 1;
                            }
                            // hien thi so page
                            ViewBag.Pre = p - 1;
                            ViewBag.Next = p + 1;

                            var list = data.Count() > 0 ? data.Skip(np * (p - 1)).Take(np).ToList() : data.ToList();

                            return PartialView("_ListLegalDocs", list);
                        }
                        else
                        {
                            IEnumerable<LegalDoc> data = (from n in db.LegalDoc
                                                          where n.Status == (int)Const.Status.NORMAL
                                                          && (n.TichYeu.Contains(textS) || textS == "")
                                                          && (n.AgencyIssued == departments || departments == -1)
                                                          && ((n.DateIssue >= DateTime.Parse(dateStart + " 0:0:0") && n.DateIssue <= DateTime.Parse(dateEnd + " 23:59:59"))
                                                          || dateStart == "" || dateEnd == "")
                                                          select n).OrderByDescending(e => e.DateIssue).ToList();


                            ViewBag.total = (data.Count());
                            ViewBag.page = p;
                            if ((ViewBag.total - 4) % np == 0)
                            {
                                ViewBag.totalPage = (ViewBag.total) / np;

                            }
                            else
                            {
                                ViewBag.totalPage = ((ViewBag.total) / np) + 1;
                            }
                            // hien thi so page
                            ViewBag.Pre = p - 1;
                            ViewBag.Next = p + 1;

                            var list = data.Count() > 0 ? data.Skip(np * (p - 1)).Take(np).ToList() : data.ToList();

                            return PartialView("_ListLegalDocs", list);
                        }

                    }
                    else
                    {
                        if (cateCq != -1)
                        {
                            IEnumerable<LegalDoc> data = (from cm in db.CategoryMapping
                                                          join n in db.LegalDoc on cm.TargetId equals n.LegalDocId
                                                          where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_LEGAL_DOC
                                                          && cm.CategoryId == cateCq
                                                          && n.Status == (int)Const.Status.NORMAL
                                                          && cm.Status != (int)Const.Status.DELETED
                                                          && (n.TichYeu.Contains(textS) || n.Name.Contains(textS) || textS == "")
                                                          && (n.AgencyIssued == departments || departments == -1)
                                                          && ((n.DateIssue >= DateTime.Parse(dateStart + " 0:0:0") && n.DateIssue <= DateTime.Parse(dateEnd + " 23:59:59"))
                                                          || dateStart == "" || dateEnd == "")
                                                          select n).OrderByDescending(e => e.DateIssue).ToList();

                            ViewBag.total = (data.Count());
                            ViewBag.page = p;
                            if ((ViewBag.total - 4) % np == 0)
                            {
                                ViewBag.totalPage = (ViewBag.total) / np;

                            }
                            else
                            {
                                ViewBag.totalPage = ((ViewBag.total) / np) + 1;
                            }
                            // hien thi so page
                            ViewBag.Pre = p - 1;
                            ViewBag.Next = p + 1;

                            var list = data.Count() > 0 ? data.Skip(np * (p - 1)).Take(np).ToList() : data.ToList();

                            return PartialView("_ListLegalDocs", list);
                        }
                        else
                        {
                            IEnumerable<LegalDoc> data = (from n in db.LegalDoc
                                                          where n.Status == (int)Const.Status.NORMAL
                                                          && (n.TichYeu.Contains(textS) || n.Name.Contains(textS) || textS == "")
                                                          && (n.AgencyIssued == departments || departments == -1)
                                                          && ((n.DateIssue >= DateTime.Parse(dateStart + " 0:0:0") && n.DateIssue <= DateTime.Parse(dateEnd + " 23:59:59"))
                                                          || dateStart == "" || dateEnd == "")
                                                          select n).OrderByDescending(e => e.DateIssue).ToList();

                            ViewBag.total = (data.Count());
                            ViewBag.page = p;
                            if ((ViewBag.total - 4) % np == 0)
                            {
                                ViewBag.totalPage = (ViewBag.total) / np;

                            }
                            else
                            {
                                ViewBag.totalPage = ((ViewBag.total) / np) + 1;
                            }
                            // hien thi so page
                            ViewBag.Pre = p - 1;
                            ViewBag.Next = p + 1;

                            var list = data.Count() > 0 ? data.Skip(np * (p - 1)).Take(np).ToList() : data.ToList();

                            return PartialView("_ListLegalDocs", list);
                        }
                    }
                }
            }
            catch
            {
                //return Redirect("/Home/Error");
                return null;
            }
        }

        public ActionResult Rss()
        {
            using (var db = new IOITDataContext())
            {
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                var cate = db.Category.Where(e => e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_TEXT
                 && e.Status != (int)Const.Status.DELETED).ToList();
                return View(cate);
            }
        }

        public ActionResult RssItem(string name, int id)
        {
            using (var db = new IOITDataContext())
            {
                var cate = db.Category.Where(e => e.CategoryId == id && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                if (cate == null)
                {
                    return View();
                }
                var feed = new SyndicationFeed(cate.Name, cate.Description, new Uri(_configuration["Settings:Domain"] + cate.Url), "RSSUrl", DateTime.Now);

                feed.Copyright = new TextSyndicationContent($"{DateTime.Now.Year} Tạp chí dân chủ và pháp luật");
                var items = new List<SyndicationItem>();
                var postings = (from cm in db.CategoryMapping
                                join n in db.News on cm.TargetId equals n.NewsId
                                where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                && n.TypeNewsId == (int)Const.TypeNews.NEWS_TEXT
                                && cm.CategoryId == id
                                && n.CompanyId == Const.COMPANYID
                                && n.WebsiteId == Const.WEBSITEID
                                && n.Status == (int)Const.Status.NORMAL
                                && cm.Status != (int)Const.Status.DELETED
                                && n.DateStartActive <= DateTime.Now
                                select n).OrderByDescending(e => e.DateStartActive).ToList();
                foreach (var item in postings)
                {
                    var postUrl = _configuration["Settings:Domain"] + item.Url;
                    var title = item.Title;
                    var description = item.Description;
                    items.Add(new SyndicationItem(title, description, new Uri(postUrl), item.NewsId + "", (DateTime)item.DateStartActive));
                }

                feed.Items = items;
                var settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    NewLineHandling = NewLineHandling.Entitize,
                    NewLineOnAttributes = true,
                    Indent = true
                };
                using (var stream = new MemoryStream())
                {
                    using (var xmlWriter = XmlWriter.Create(stream, settings))
                    {
                        var rssFormatter = new Rss20FeedFormatter(feed, false);
                        rssFormatter.WriteTo(xmlWriter);
                        xmlWriter.Flush();
                    }
                    return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
                }
            }
        }


    }
}