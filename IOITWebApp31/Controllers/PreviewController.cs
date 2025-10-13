using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp31.Controllers
{
    public class PreviewController : Controller
    {
        private static readonly ILog log = LogMaster.GetLogger("preview", "preview");
        private readonly IConfiguration _configuration;

        public PreviewController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ActionResult> News(int id, int idc, string seoName)
        {
            //ViewBag.Class = "header-style-1 menu-has-cart menu-has-search header-sticky";

            try
            {
                var UserId = HttpContext.Session.GetInt32("UserId");
                if (UserId == null)
                {
                    return Redirect("/");
                }

                using (var db = new IOITDataContext())
                {

                    //get url
                    string urlLang = Request.Cookies["ReturnUrl"] != null ? Request.Cookies["ReturnUrl"] : "/";
                    string url = Request.Path;
                    //get lang
                    int lang = Request.Cookies["LanguageId"] != null ? int.Parse(Request.Cookies["LanguageId"]) : Const.LANGUAGEID;
                    int langOld = Request.Cookies["LanguageOldId"] != null ? int.Parse(Request.Cookies["LanguageOldId"]) : Const.LANGUAGEID;
                    //change lang
                    if (lang != langOld && url.Trim().Equals(urlLang.Trim()))
                    {
                        //Set lang
                        Response.Cookies.Append(
                            "LanguageOldId", lang + "",
                            new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddYears(1),
                                IsEssential = true,
                                Path = "/",
                                HttpOnly = false,
                            }
                        );
                        //get id category by lang new
                        var langMap = db.LanguageMapping.Where(e => (e.TargetId1 == id || e.TargetId2 == id)
                         && ((e.LanguageId1 == lang && e.LanguageId2 == langOld) || (e.LanguageId2 == lang && e.LanguageId1 == langOld))
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                         && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        int idCate = 0;
                        if (langMap != null)
                        {
                            if (langMap.TargetId1 == id)
                                idCate = (int)langMap.TargetId2;
                            else
                                idCate = (int)langMap.TargetId1;
                        }

                        if (idCate == 0)
                            return Redirect("/");
                        else if (idCate != id)
                        {
                            var seoLang = db.News.Where(e => e.NewsId == idCate
                               && e.Status == (int)Const.Status.NORMAL
                               && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("DetailNews", new { id = idCate, seoName = seoLang.Url, idc = idc });
                        }
                    }

                    var data = db.News.Where(e => e.NewsId == id
                    //&& e.Status == (int)Const.Status.NORMAL
                    && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    if (data == null)
                        return Redirect("/Home/Error");

                    string strSEO = data.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("DetailNews", new { id = id, seoName = strSEO });
                    var DayNumberWeek = (int)data.DateStartActive.Value.DayOfWeek;
                    if (DayNumberWeek == 0)
                    {
                        ViewBag.DayOfWeek = "Chủ nhật";
                    }
                    else if (DayNumberWeek == 1)
                    {
                        ViewBag.DayOfWeek = "Thứ 2";
                    }
                    else if (DayNumberWeek == 2)
                    {
                        ViewBag.DayOfWeek = "Thứ 3";
                    }
                    else if (DayNumberWeek == 3)
                    {
                        ViewBag.DayOfWeek = "Thứ 4";
                    }
                    else if (DayNumberWeek == 4)
                    {
                        ViewBag.DayOfWeek = "Thứ 5";
                    }
                    else if (DayNumberWeek == 5)
                    {
                        ViewBag.DayOfWeek = "Thứ 6";
                    }
                    else if (DayNumberWeek == 6)
                    {
                        ViewBag.DayOfWeek = "Thứ 7";
                    }
                    ViewBag.Title = data.Title;
                    ViewBag.NewsTypeId = data.TypeNewsId;
                    if (ViewBag.NewsTypeId == 4)
                    {
                        ViewBag.LinkVideo = data.LinkVideo;
                        var uri = new Uri(ViewBag.LinkVideo);
                        var query = HttpUtility.ParseQueryString(uri.Query);
                        var videoId = query["v"];
                        ViewBag.VideoId = videoId;
                    }
                    ViewBag.SeoTitle = data.MetaTitle;
                    ViewBag.SeoDescription = data.MetaDescription;
                    ViewBag.SeoKeywords = data.MetaKeyword;
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    var WebsiteLink = db.Website.Where(c => c.WebsiteId == 1 && c.Status != (int)Const.Status.DELETED).FirstOrDefault().Url;
                    ViewBag.Logo = db.Website.Where(c => c.WebsiteId == 1 && c.Status != (int)Const.Status.DELETED).FirstOrDefault().LogoHeader;
                    ViewBag.Url = WebsiteLink + "/" + seoName;
                    if (data.Image != "")
                        ViewBag.UrlImg = WebsiteLink + "/" + "uploads/thumbs/" + data.Image;
                    else
                        ViewBag.UrlImg = WebsiteLink + "/" + "/uploads/home/no-image.png";

                    //category

                    var cat = db.Category.Where(e => e.CategoryId == idc && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (cat != null)
                    {
                        ViewBag.CategoryName = cat.Name;
                        ViewBag.CategoryId = cat.CategoryId;
                        ViewBag.CategoryUrl = cat.Url;
                        if (cat.CategoryParentId != 0)
                        {
                            var catP = db.Category.Where(e => e.CategoryId == cat.CategoryParentId).FirstOrDefault();
                            if (catP != null)
                            {
                                ViewBag.CategoryNameP = catP.Name;
                                ViewBag.CategoryIdP = catP.CategoryId;
                                ViewBag.CategoryUrlP = catP.Url;
                            }
                        }
                    }
                    else
                    {
                        var cate = (from cm in db.CategoryMapping
                                    join c in db.Category on cm.CategoryId equals c.CategoryId
                                    where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                    && c.CompanyId == Const.COMPANYID && cm.TargetId == id
                                    && c.WebsiteId == Const.WEBSITEID
                                    && c.Status == (int)Const.Status.NORMAL
                                    && cm.Status != (int)Const.Status.DELETED
                                    select c).OrderByDescending(e => e.CreatedAt).FirstOrDefault();

                        if (cate != null)
                        {
                            ViewBag.CategoryName = cate.Name;
                            ViewBag.CategoryId = cate.CategoryId;
                            ViewBag.CategoryUrl = cate.Url;
                            if (cate.CategoryParentId != 0)
                            {
                                var catP = db.Category.Where(e => e.CategoryId == cate.CategoryParentId).FirstOrDefault();
                                if (catP != null)
                                {
                                    ViewBag.CategoryNameP = catP.Name;
                                    ViewBag.CategoryIdP = catP.CategoryId;
                                    ViewBag.CategoryUrlP = catP.Url;
                                }
                            }
                        }
                    }
                    //


                    ViewBag.listPro = (from rt in db.Related
                                       join pr in db.Product on rt.TargetRelatedId equals pr.ProductId
                                       where rt.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT
                                        && rt.TargetId == id
                                        && pr.Status == (int)Const.Status.NORMAL
                                       && rt.Status != (int)Const.Status.DELETED
                                       select pr).ToList();

                    ViewBag.listPost = (from rt in db.Related
                                        join pr in db.News on rt.TargetRelatedId equals pr.NewsId
                                        where rt.TargetType == (int)Const.TypeRelated.NEWS_NEWS
                                         && rt.TargetId == id
                                         && pr.Status == (int)Const.Status.NORMAL
                                        && rt.Status != (int)Const.Status.DELETED
                                        select pr).ToList();


                    var tag = db.Tag.Where(e => e.TargetId == id
                        && e.Status != (int)Const.Status.DELETED
                        && e.TargetType == (int)Const.TypeTag.TAG_NEWS
                        && e.CompanyId == Const.COMPANYID
                        && e.WebsiteId == Const.WEBSITEID).Select(e => e.Name).ToList();
                    if (tag != null)
                    {
                        ViewBag.Tag = tag;
                    }

                    ////Cap nhat so luot xem
                    //data.ViewNumber = data.ViewNumber + 1;
                    //db.Entry(data).State = EntityState.Modified;
                    //db.SaveChanges();

                    return View(data);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Image(int id, int idc, string seoName)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("UserId");
                if (UserId == null)
                {
                    return Redirect("/");
                }

                using (var db = new IOITDataContext())
                {

                    //get url
                    string urlLang = Request.Cookies["ReturnUrl"] != null ? Request.Cookies["ReturnUrl"] : "/";
                    string url = Request.Path;
                    //get lang
                    int lang = Request.Cookies["LanguageId"] != null ? int.Parse(Request.Cookies["LanguageId"]) : Const.LANGUAGEID;
                    int langOld = Request.Cookies["LanguageOldId"] != null ? int.Parse(Request.Cookies["LanguageOldId"]) : Const.LANGUAGEID;
                    //change lang
                    if (lang != langOld && url.Trim().Equals(urlLang.Trim()))
                    {
                        //Set lang
                        Response.Cookies.Append(
                            "LanguageOldId", lang + "",
                            new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddYears(1),
                                IsEssential = true,
                                Path = "/",
                                HttpOnly = false,
                            }
                        );
                        //get id category by lang new
                        var langMap = db.LanguageMapping.Where(e => (e.TargetId1 == id || e.TargetId2 == id)
                         && ((e.LanguageId1 == lang && e.LanguageId2 == langOld) || (e.LanguageId2 == lang && e.LanguageId1 == langOld))
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                         && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        int idCate = 0;
                        if (langMap != null)
                        {
                            if (langMap.TargetId1 == id)
                                idCate = (int)langMap.TargetId2;
                            else
                                idCate = (int)langMap.TargetId1;
                        }

                        if (idCate == 0)
                            return Redirect("/");
                        else if (idCate != id)
                        {
                            var seoLang = db.News.Where(e => e.NewsId == idCate
                               && e.Status == (int)Const.Status.NORMAL
                               && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("DetailNews", new { id = idCate, seoName = seoLang.Url, idc = idc });
                        }
                    }

                    var data = db.News.Where(e => e.NewsId == id
                    //&& e.Status == (int)Const.Status.NORMAL
                    && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    if (data == null)
                        return Redirect("/Home/Error");

                    string strSEO = data.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("DetailNews", new { id = id, seoName = strSEO });
                    var DayNumberWeek = (int)data.DateStartActive.Value.DayOfWeek;
                    if (DayNumberWeek == 0)
                    {
                        ViewBag.DayOfWeek = "Chủ nhật";
                    }
                    else if (DayNumberWeek == 1)
                    {
                        ViewBag.DayOfWeek = "Thứ 2";
                    }
                    else if (DayNumberWeek == 2)
                    {
                        ViewBag.DayOfWeek = "Thứ 3";
                    }
                    else if (DayNumberWeek == 3)
                    {
                        ViewBag.DayOfWeek = "Thứ 4";
                    }
                    else if (DayNumberWeek == 4)
                    {
                        ViewBag.DayOfWeek = "Thứ 5";
                    }
                    else if (DayNumberWeek == 5)
                    {
                        ViewBag.DayOfWeek = "Thứ 6";
                    }
                    else if (DayNumberWeek == 6)
                    {
                        ViewBag.DayOfWeek = "Thứ 7";
                    }
                    ViewBag.Title = data.Title;
                    ViewBag.NewsTypeId = data.TypeNewsId;
                    if (ViewBag.NewsTypeId == 4)
                    {
                        ViewBag.LinkVideo = data.LinkVideo;
                        var uri = new Uri(ViewBag.LinkVideo);
                        var query = HttpUtility.ParseQueryString(uri.Query);
                        var videoId = query["v"];
                        ViewBag.VideoId = videoId;
                    }
                    ViewBag.SeoTitle = data.MetaTitle;
                    ViewBag.SeoDescription = data.MetaDescription;
                    ViewBag.SeoKeywords = data.MetaKeyword;
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    var WebsiteLink = db.Website.Where(c => c.WebsiteId == 1 && c.Status != (int)Const.Status.DELETED).FirstOrDefault().Url;
                    ViewBag.Logo = db.Website.Where(c => c.WebsiteId == 1 && c.Status != (int)Const.Status.DELETED).FirstOrDefault().LogoHeader;
                    ViewBag.Url = WebsiteLink + "/" + seoName;
                    if (data.Image != "")
                        ViewBag.UrlImg = WebsiteLink + "/" + "uploads/thumbs/" + data.Image;
                    else
                        ViewBag.UrlImg = WebsiteLink + "/" + "/uploads/home/no-image.png";

                    //category

                    var cat = db.Category.Where(e => e.CategoryId == idc && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (cat != null)
                    {
                        ViewBag.CategoryName = cat.Name;
                        ViewBag.CategoryId = cat.CategoryId;
                        ViewBag.CategoryUrl = cat.Url;
                        if (cat.CategoryParentId != 0)
                        {
                            var catP = db.Category.Where(e => e.CategoryId == cat.CategoryParentId).FirstOrDefault();
                            if (catP != null)
                            {
                                ViewBag.CategoryNameP = catP.Name;
                                ViewBag.CategoryIdP = catP.CategoryId;
                                ViewBag.CategoryUrlP = catP.Url;
                            }
                        }
                    }
                    else
                    {
                        var cate = (from cm in db.CategoryMapping
                                    join c in db.Category on cm.CategoryId equals c.CategoryId
                                    where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                    && c.CompanyId == Const.COMPANYID && cm.TargetId == id
                                    && c.WebsiteId == Const.WEBSITEID
                                    && c.Status == (int)Const.Status.NORMAL
                                    && cm.Status != (int)Const.Status.DELETED
                                    select c).OrderByDescending(e => e.CreatedAt).FirstOrDefault();

                        if (cate != null)
                        {
                            ViewBag.CategoryName = cate.Name;
                            ViewBag.CategoryId = cate.CategoryId;
                            ViewBag.CategoryUrl = cate.Url;
                            if (cate.CategoryParentId != 0)
                            {
                                var catP = db.Category.Where(e => e.CategoryId == cate.CategoryParentId).FirstOrDefault();
                                if (catP != null)
                                {
                                    ViewBag.CategoryNameP = catP.Name;
                                    ViewBag.CategoryIdP = catP.CategoryId;
                                    ViewBag.CategoryUrlP = catP.Url;
                                }
                            }
                        }
                    }
                    //


                    ViewBag.listPro = (from rt in db.Related
                                       join pr in db.Product on rt.TargetRelatedId equals pr.ProductId
                                       where rt.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT
                                        && rt.TargetId == id
                                        && pr.Status == (int)Const.Status.NORMAL
                                       && rt.Status != (int)Const.Status.DELETED
                                       select pr).ToList();

                    ViewBag.listPost = (from rt in db.Related
                                        join pr in db.News on rt.TargetRelatedId equals pr.NewsId
                                        where rt.TargetType == (int)Const.TypeRelated.NEWS_NEWS
                                         && rt.TargetId == id
                                         && pr.Status == (int)Const.Status.NORMAL
                                        && rt.Status != (int)Const.Status.DELETED
                                        select pr).ToList();


                    var tag = db.Tag.Where(e => e.TargetId == id
                        && e.Status != (int)Const.Status.DELETED
                        && e.TargetType == (int)Const.TypeTag.TAG_NEWS
                        && e.CompanyId == Const.COMPANYID
                        && e.WebsiteId == Const.WEBSITEID).Select(e => e.Name).ToList();
                    if (tag != null)
                    {
                        ViewBag.Tag = tag;
                    }

                    ////Cap nhat so luot xem
                    //data.ViewNumber = data.ViewNumber + 1;
                    //db.Entry(data).State = EntityState.Modified;
                    //db.SaveChanges();

                    return View(data);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Video(int id, int idc, string seoName)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("UserId");
                if (UserId == null)
                {
                    return Redirect("/");
                }

                using (var db = new IOITDataContext())
                {

                    //get url
                    string urlLang = Request.Cookies["ReturnUrl"] != null ? Request.Cookies["ReturnUrl"] : "/";
                    string url = Request.Path;
                    //get lang
                    int lang = Request.Cookies["LanguageId"] != null ? int.Parse(Request.Cookies["LanguageId"]) : Const.LANGUAGEID;
                    int langOld = Request.Cookies["LanguageOldId"] != null ? int.Parse(Request.Cookies["LanguageOldId"]) : Const.LANGUAGEID;
                    //change lang
                    if (lang != langOld && url.Trim().Equals(urlLang.Trim()))
                    {
                        //Set lang
                        Response.Cookies.Append(
                            "LanguageOldId", lang + "",
                            new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddYears(1),
                                IsEssential = true,
                                Path = "/",
                                HttpOnly = false,
                            }
                        );
                        //get id category by lang new
                        var langMap = db.LanguageMapping.Where(e => (e.TargetId1 == id || e.TargetId2 == id)
                         && ((e.LanguageId1 == lang && e.LanguageId2 == langOld) || (e.LanguageId2 == lang && e.LanguageId1 == langOld))
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                         && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        int idCate = 0;
                        if (langMap != null)
                        {
                            if (langMap.TargetId1 == id)
                                idCate = (int)langMap.TargetId2;
                            else
                                idCate = (int)langMap.TargetId1;
                        }

                        if (idCate == 0)
                            return Redirect("/");
                        else if (idCate != id)
                        {
                            var seoLang = db.News.Where(e => e.NewsId == idCate
                               && e.Status == (int)Const.Status.NORMAL
                               && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("DetailNews", new { id = idCate, seoName = seoLang.Url, idc = idc });
                        }
                    }

                    var data = db.News.Where(e => e.NewsId == id
                    //&& e.Status == (int)Const.Status.NORMAL
                    && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    if (data == null)
                        return Redirect("/Home/Error");

                    string strSEO = data.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("DetailNews", new { id = id, seoName = strSEO });
                    var DayNumberWeek = (int)data.DateStartActive.Value.DayOfWeek;
                    if (DayNumberWeek == 0)
                    {
                        ViewBag.DayOfWeek = "Chủ nhật";
                    }
                    else if (DayNumberWeek == 1)
                    {
                        ViewBag.DayOfWeek = "Thứ 2";
                    }
                    else if (DayNumberWeek == 2)
                    {
                        ViewBag.DayOfWeek = "Thứ 3";
                    }
                    else if (DayNumberWeek == 3)
                    {
                        ViewBag.DayOfWeek = "Thứ 4";
                    }
                    else if (DayNumberWeek == 4)
                    {
                        ViewBag.DayOfWeek = "Thứ 5";
                    }
                    else if (DayNumberWeek == 5)
                    {
                        ViewBag.DayOfWeek = "Thứ 6";
                    }
                    else if (DayNumberWeek == 6)
                    {
                        ViewBag.DayOfWeek = "Thứ 7";
                    }
                    ViewBag.Title = data.Title;
                    ViewBag.NewsTypeId = data.TypeNewsId;
                    if (ViewBag.NewsTypeId == 4)
                    {
                        if (data.LinkVideo != null)
                        {
                            ViewBag.LinkVideo = data.LinkVideo;
                            //var uri = new Uri(ViewBag.LinkVideo);
                            //var query = HttpUtility.ParseQueryString(uri.Query);
                            //var videoId = query["v"];
                            //ViewBag.VideoId = videoId;
                        }
                    }
                    ViewBag.SeoTitle = data.MetaTitle;
                    ViewBag.SeoDescription = data.MetaDescription;
                    ViewBag.SeoKeywords = data.MetaKeyword;
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    var WebsiteLink = db.Website.Where(c => c.WebsiteId == 1 && c.Status != (int)Const.Status.DELETED).FirstOrDefault().Url;
                    ViewBag.Logo = db.Website.Where(c => c.WebsiteId == 1 && c.Status != (int)Const.Status.DELETED).FirstOrDefault().LogoHeader;
                    ViewBag.Url = WebsiteLink + "/" + seoName;
                    if (data.Image != "")
                        ViewBag.UrlImg = WebsiteLink + "/" + "uploads/thumbs/" + data.Image;
                    else
                        ViewBag.UrlImg = WebsiteLink + "/" + "/uploads/home/no-image.png";

                    //category

                    var cat = db.Category.Where(e => e.CategoryId == idc && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (cat != null)
                    {
                        ViewBag.CategoryName = cat.Name;
                        ViewBag.CategoryId = cat.CategoryId;
                        ViewBag.CategoryUrl = cat.Url;
                        if (cat.CategoryParentId != 0)
                        {
                            var catP = db.Category.Where(e => e.CategoryId == cat.CategoryParentId).FirstOrDefault();
                            if (catP != null)
                            {
                                ViewBag.CategoryNameP = catP.Name;
                                ViewBag.CategoryIdP = catP.CategoryId;
                                ViewBag.CategoryUrlP = catP.Url;
                            }
                        }
                    }
                    else
                    {
                        var cate = (from cm in db.CategoryMapping
                                    join c in db.Category on cm.CategoryId equals c.CategoryId
                                    where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                    && c.CompanyId == Const.COMPANYID && cm.TargetId == id
                                    && c.WebsiteId == Const.WEBSITEID
                                    && c.Status == (int)Const.Status.NORMAL
                                    && cm.Status != (int)Const.Status.DELETED
                                    select c).OrderByDescending(e => e.CreatedAt).FirstOrDefault();

                        if (cate != null)
                        {
                            ViewBag.CategoryName = cate.Name;
                            ViewBag.CategoryId = cate.CategoryId;
                            ViewBag.CategoryUrl = cate.Url;
                            if (cate.CategoryParentId != 0)
                            {
                                var catP = db.Category.Where(e => e.CategoryId == cate.CategoryParentId).FirstOrDefault();
                                if (catP != null)
                                {
                                    ViewBag.CategoryNameP = catP.Name;
                                    ViewBag.CategoryIdP = catP.CategoryId;
                                    ViewBag.CategoryUrlP = catP.Url;
                                }
                            }
                        }
                    }
                    //


                    ViewBag.listPro = (from rt in db.Related
                                       join pr in db.Product on rt.TargetRelatedId equals pr.ProductId
                                       where rt.TargetType == (int)Const.TypeRelated.NEWS_PRODUCT
                                        && rt.TargetId == id
                                        && pr.Status == (int)Const.Status.NORMAL
                                       && rt.Status != (int)Const.Status.DELETED
                                       select pr).ToList();

                    ViewBag.listPost = (from rt in db.Related
                                        join pr in db.News on rt.TargetRelatedId equals pr.NewsId
                                        where rt.TargetType == (int)Const.TypeRelated.NEWS_NEWS
                                         && rt.TargetId == id
                                         && pr.Status == (int)Const.Status.NORMAL
                                        && rt.Status != (int)Const.Status.DELETED
                                        select pr).ToList();


                    var tag = db.Tag.Where(e => e.TargetId == id
                        && e.Status != (int)Const.Status.DELETED
                        && e.TargetType == (int)Const.TypeTag.TAG_NEWS
                        && e.CompanyId == Const.COMPANYID
                        && e.WebsiteId == Const.WEBSITEID).Select(e => e.Name).ToList();
                    if (tag != null)
                    {
                        ViewBag.Tag = tag;
                    }

                    ////Cap nhat so luot xem
                    //data.ViewNumber = data.ViewNumber + 1;
                    //db.Entry(data).State = EntityState.Modified;
                    //db.SaveChanges();

                    return View(data);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Event(int id, string seoName)
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    //get url
                    string urlLang = Request.Cookies["ReturnUrl"] != null ? Request.Cookies["ReturnUrl"] : "/";
                    string url = Request.Path;
                    //get lang
                    int lang = Request.Cookies["LanguageId"] != null ? int.Parse(Request.Cookies["LanguageId"]) : Const.LANGUAGEID;
                    int langOld = Request.Cookies["LanguageOldId"] != null ? int.Parse(Request.Cookies["LanguageOldId"]) : Const.LANGUAGEID;
                    //change lang
                    if (lang != langOld && url.Trim().Equals(urlLang.Trim()))
                    {
                        //Set lang
                        Response.Cookies.Append(
                            "LanguageOldId", lang + "",
                            new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddYears(1),
                                IsEssential = true,
                                Path = "/",
                                HttpOnly = false,
                            }
                        );
                        //get id category by lang new
                        var langMap = db.LanguageMapping.Where(e => (e.TargetId1 == id || e.TargetId2 == id)
                         && ((e.LanguageId1 == lang && e.LanguageId2 == langOld) || (e.LanguageId2 == lang && e.LanguageId1 == langOld))
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_NEWS
                         && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        int idCate = 0;
                        if (langMap != null)
                        {
                            if (langMap.TargetId1 == id)
                                idCate = (int)langMap.TargetId2;
                            else
                                idCate = (int)langMap.TargetId1;
                        }

                        if (idCate == 0)
                            return Redirect("/");
                        else if (idCate != id)
                        {
                            var seoLang = db.News.Where(e => e.NewsId == idCate
                               && e.TypeNewsId == (int)Const.TypeNews.NEWS_ATTACTMENT
                               && e.Status == (int)Const.Status.NORMAL
                               && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("DetailAttactment", new { id = idCate, seoName = seoLang.Url });
                        }
                    }

                    var data = db.News.Where(e => e.NewsId == id
                    && e.TypeNewsId == (int)Const.TypeNews.NEWS_ATTACTMENT
                    && e.Status == (int)Const.Status.NORMAL
                    && e.CompanyId == Const.COMPANYID
                    && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();

                    if (data == null)
                        return Redirect("/Home/Error");

                    string strSEO = data.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("DetailAttactment", new { id = id, seoName = strSEO });

                    ViewBag.SeoTitle = data.MetaTitle;
                    ViewBag.SeoDescription = data.MetaDescription;
                    ViewBag.SeoKeywords = data.MetaKeyword;
                    ViewBag.Url = _configuration["Settings:Domain"] + "/" + Const.DETAIL_ATTACTMENT + "/" + seoName + "-" + id + ".html";
                    if (data.Image != "")
                        ViewBag.UrlImg = _configuration["Settings:Domain"] + "/uploads/" + data.Image;
                    else
                        ViewBag.UrlImg = _configuration["Settings:Domain"] + "/Content/images/logo.svg";

                    //category
                    var cate = (from cm in db.CategoryMapping
                                join c in db.Category on cm.CategoryId equals c.CategoryId
                                where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                && c.CompanyId == Const.COMPANYID && cm.TargetId == id
                                && c.WebsiteId == Const.WEBSITEID
                                && c.Status == (int)Const.Status.NORMAL
                                && cm.Status != (int)Const.Status.DELETED
                                select c).OrderByDescending(e => e.CreatedAt).FirstOrDefault();

                    ViewBag.CategoryId = -1;
                    if (cate != null)
                    {
                        ViewBag.CategoryParentId = cate.CategoryParentId;
                        ViewBag.CategoryName = cate.Name;
                        ViewBag.CategoryId = cate.CategoryId;
                    }

                    var tag = db.Tag.Where(e => e.TargetId == id
                        && e.Status != (int)Const.Status.DELETED
                        && e.TargetType == (int)Const.TypeTag.TAG_NEWS
                        && e.CompanyId == Const.COMPANYID
                        && e.WebsiteId == Const.WEBSITEID).Select(e => e.Name).ToList();
                    if (tag != null)
                    {
                        ViewBag.Tag = tag;
                    }

                    //Cap nhat so luot xem
                    data.ViewNumber = data.ViewNumber + 1;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();

                    return View(data);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

    }
}
