using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Controllers
{
    public class DetailController : Controller
    {
        private static readonly ILog log = LogMaster.GetLogger("detail", "detail");
        private readonly IConfiguration _configuration;

        public DetailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ActionResult News(int id, int idc, string seoName)
        {
            ViewBag.Class = "body-detail single-" + id;
            //var CustomerId = HttpContext.Session.GetInt32("CustomerId");

            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    ViewBag.Logo = website.LogoHeader;
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
                    && e.Status == (int)Const.Status.NORMAL
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
                        //var uri = new Uri(ViewBag.LinkVideo);
                        //var query = HttpUtility.ParseQueryString(uri.Query);
                        //var videoId = query["v"];
                        //ViewBag.VideoId = videoId;
                    }
                    ViewBag.SeoTitle = data.MetaTitle;
                    ViewBag.SeoDescription = data.MetaDescription;
                    ViewBag.SeoKeywords = data.MetaKeyword;
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    var WebsiteLink = db.Website.Where(c => c.WebsiteId == 1 && c.Status != (int)Const.Status.DELETED).FirstOrDefault().Url;
                    ViewBag.Logo = db.Website.Where(c => c.WebsiteId == 1 && c.Status != (int)Const.Status.DELETED).FirstOrDefault().LogoHeader;
                    ViewBag.Url = WebsiteLink + "/" + seoName;
                    if (data.Image != "")
                        ViewBag.UrlImg = WebsiteLink + "/" + "uploads/" + data.Image;
                    else
                        ViewBag.UrlImg = WebsiteLink + "/" + "/uploads/home/no-image.png";

                    if (!string.IsNullOrEmpty(data.Image))
                        ViewBag.OgImage = WebsiteLink + "/uploads/" + data.Image;
                    else
                        ViewBag.OgImage = WebsiteLink + "/images/homecashplus/CashPlus-feature.jpg";

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

                    if (ViewBag.listPost == null || ((List<News>)ViewBag.listPost).Count == 0)
                    {
                        var cateId = -1;
                        if (ViewBag.CategoryId != null && int.TryParse(ViewBag.CategoryId.ToString(), out cateId) && cateId > 0)
                        {
                            ViewBag.listPost = (from n in db.News
                                join cm in db.CategoryMapping on n.NewsId equals cm.TargetId
                                where n.NewsId != id
                                    && n.Status == (int)Const.Status.NORMAL
                                    && cm.CategoryId == cateId
                                    && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                    && cm.Status != (int)Const.Status.DELETED
                                    && n.DateStartActive < data.DateStartActive // chỉ lấy tin trước ngày hiện tại
                                orderby n.DateStartActive descending
                                select n).Distinct().Take(4).ToList();
                        }
                        else
                        {
                            ViewBag.listPost = db.News
                                .Where(n => n.NewsId != id && n.Status == (int)Const.Status.NORMAL && n.DateStartActive < data.DateStartActive)
                                .OrderByDescending(n => n.DateStartActive)
                                .Take(4)
                                .ToList();
                        }
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

        public ActionResult Image(int id, string seoName)
        {
            ViewBag.Class = "body-detail";
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    var data = db.News.Where(e => e.NewsId == id
                     && e.Status == (int)Const.Status.NORMAL
                     && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    if (data == null)
                        return Redirect("/Home/Error");

                    string strSEO = data.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("DetailNews", new { id = id, seoName = strSEO });

                    ViewBag.SeoTitle = data.MetaTitle;
                    ViewBag.SeoDescription = data.MetaDescription;
                    ViewBag.SeoKeywords = data.MetaKeyword;
                    var listAttachment = db.Attactment.Where(a => a.TargetId == id && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList();

                    return View(listAttachment);

                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Video(int id, string seoName)
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
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
                               && e.TypeNewsId == (int)Const.TypeNews.NEWS_VIDEO
                               && e.Status == (int)Const.Status.NORMAL
                               && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("DetailVideo", new { id = idCate, seoName = seoLang.Url });
                        }
                    }

                    var data = db.News.Where(e => e.NewsId == id
                    && e.TypeNewsId == (int)Const.TypeNews.NEWS_VIDEO
                    && e.Status == (int)Const.Status.NORMAL
                    && e.CompanyId == Const.COMPANYID
                    && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();

                    if (data == null)
                        return Redirect("/Home/Error");

                    string strSEO = data.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("DetailVideo", new { id = id, seoName = strSEO });

                    ViewBag.SeoTitle = data.MetaTitle;
                    ViewBag.SeoDescription = data.MetaDescription;
                    ViewBag.SeoKeywords = data.MetaKeyword;
                    ViewBag.Url = _configuration["Settings:Domain"] + "/" + Const.DETAIL_VIDEO + "/" + seoName + "-" + id + ".html";
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

        public ActionResult Attactment(int id, string seoName)
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
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

        public ActionResult Notificaton(int id, string seoName)
        {
            //Session["current_url"] = Request.Url.AbsoluteUri;
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
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
                               && e.TypeNewsId == (int)Const.TypeNews.NEWS_NOTIFICATION
                               && e.Status == (int)Const.Status.NORMAL
                               && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("DetailNotificaton", new { id = idCate, seoName = seoLang.Url });
                        }
                    }

                    var data = db.News.Where(e => e.NewsId == id
                    && e.TypeNewsId == (int)Const.TypeNews.NEWS_NOTIFICATION
                    && e.Status == (int)Const.Status.NORMAL
                    && e.CompanyId == Const.COMPANYID
                    && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();

                    if (data == null)
                        return Redirect("/Home/Error");

                    string strSEO = data.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("DetailNotificaton", new { id = id, seoName = strSEO });

                    ViewBag.SeoTitle = data.MetaTitle;
                    ViewBag.SeoDescription = data.MetaDescription;
                    ViewBag.SeoKeywords = data.MetaKeyword;
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    ViewBag.Url = _configuration["Settings:Domain"] + "/" + Const.DETAIL_NOTIFICATION + "/" + seoName + "-" + id + ".html";
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

        public ActionResult Partner(int id, string seoName)
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    var data = db.News.Where(e => e.NewsId == id
                    && e.TypeNewsId == (int)Const.TypeNews.NEWS_TEXT
                    && e.Status == (int)Const.Status.NORMAL
                    && e.CompanyId == Const.COMPANYID
                    && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    if (data == null)
                        return Redirect("/Home/Error");

                    string strSEO = data.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("DetailNews", new { id = id, seoName = strSEO });

                    ViewBag.SeoTitle = data.MetaTitle;
                    ViewBag.SeoDescription = data.MetaDescription;
                    ViewBag.SeoKeywords = data.MetaKeyword;
                    ViewBag.Url = _configuration["Settings:Domain"] + "/" + Const.DETAIL_NEWS + "/" + seoName + "-" + id + ".html";
                    if (!string.IsNullOrEmpty(data.Image))
                        ViewBag.OgImage = _configuration["Settings:Domain"] + "/uploads/" + data.Image;
                    else
                        ViewBag.OgImage = _configuration["Settings:Domain"] + "/images/homecashplus/CashPlus-feature.jpg";

                    //category
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
                        ViewBag.CategoryParentId = cate.CategoryParentId;
                        ViewBag.CategoryName = cate.Name;
                        ViewBag.CategoryId = cate.CategoryId;
                        ViewBag.CategoryUrl = cate.Url;
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

        // van ban
        public ActionResult DetailLegalDoc(int LegalDocId)
        {
            ViewBag.SeoTitle = "Chi tiết văn bản";
            ViewBag.Class = "body-detail";
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    List<TypeAttributeItem> typeAttributeItems = db.TypeAttributeItem.Where(t => (t.TypeAttributeId == 1 || t.TypeAttributeId == 2 || t.TypeAttributeId == 3) && t.Status != (int)Const.Status.DELETED).ToList();
                    ViewBag.typeTexts = typeAttributeItems.Where(t => t.TypeAttributeId == 3).ToList();

                    ViewBag.agencyIssues = typeAttributeItems.Where(t => t.TypeAttributeId == 1).ToList();

                    List<int> yearIssues = new List<int>();
                    int Year = DateTime.Now.Year;
                    for (var i = Year; i >= Year - 30; i--)
                    {
                        yearIssues.Add(i);
                    }
                    ViewBag.yearIssues = yearIssues;

                    var legalDoc = db.LegalDoc.Where(e => e.LegalDocId == LegalDocId
                    && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();

                    if (legalDoc == null)
                        return Redirect("/Home/Error");

                    LegalDocDT data = new LegalDocDT();
                    data.LegalDocId = legalDoc.LegalDocId;

                    data.Code = legalDoc.Code;
                    data.Name = legalDoc.Name;
                    data.DateIssue = legalDoc.DateIssue;
                    data.DateEffect = legalDoc.DateEffect;
                    data.Signer = legalDoc.Signer;
                    data.AgencyIssue = legalDoc.AgencyIssue;
                    data.YearIssue = legalDoc.YearIssue;
                    data.TypeText = legalDoc.TypeText;
                    data.Field = legalDoc.Field;
                    data.Note = legalDoc.Note;
                    data.Attactment = legalDoc.Attactment;
                    data.Contents = legalDoc.Contents;
                    data.CreatedAt = legalDoc.CreatedAt;
                    data.UpdatedAt = legalDoc.UpdatedAt;
                    data.TichYeu = legalDoc.TichYeu;

                    TypeAttributeItem AgencyIssueName = db.TypeAttributeItem.Where(t => t.TypeAttributeItemId == data.AgencyIssue).FirstOrDefault();
                    TypeAttributeItem TypeTextName = db.TypeAttributeItem.Where(t => t.TypeAttributeItemId == data.TypeText).FirstOrDefault();

                    data.AgencyIssueName = AgencyIssueName != null ? AgencyIssueName.Name : "";
                    data.TypeTextName = TypeTextName != null ? TypeTextName.Name : "";

                    return View(data);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }
        //end
        // Chi tiết ấn phẩm
        public ActionResult Publications(int Id, string seoName)
        {
            ViewBag.SeoTitle = "Chi tiết ấn phẩm";
            ViewBag.Class = "body-detail";
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    var Pubication = db.Publication.Where(e => e.PublicationId == Id
                    && e.Url == seoName && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                    if (Pubication.Author != null)
                    {
                        var NameAuthor = db.Author.Where(at => at.AuthorId == Pubication.Author && at.Status == 1).FirstOrDefault().Name;
                        ViewBag.NameAuthor = NameAuthor;
                    }
                    else
                    {
                        ViewBag.NameAuthor = "Chưa cập nhật";

                    }
                    if (Pubication.Department != null)
                    {
                        var NameDepartment = db.TypeAttributeItem.Where(nd => nd.TypeAttributeItemId == Pubication.Department && nd.TypeAttributeId == 26 && nd.Status == 1).FirstOrDefault().Name;
                        ViewBag.NameDepartment = NameDepartment;
                    }
                    else
                    {
                        ViewBag.NameDepartment = "Chưa cập nhật";
                    }

                    ViewBag.YearPublic = Pubication.DateStartActive.Value.ToString("dd/MM/yyyy");
                    ViewBag.Slug = Pubication.Url;
                    ViewBag.Id = Pubication.PublicationId;
                    if (Pubication == null)
                        return Redirect("/Home/Error");

                    PublicationDT data = new PublicationDT();
                    data.PublicationId = Pubication.PublicationId;
                    data.Title = Pubication.Title;
                    data.Description = Pubication.Description;
                    data.Contents = Pubication.Contents;
                    data.Image = Pubication.Image;
                    data.Url = Pubication.Url;
                    data.NumberOfTopic = Pubication.NumberOfTopic;
                    data.TitleEn = Pubication.TitleEn;
                    data.IsLanguage = Pubication.IsLanguage;
                    data.DescriptionEn = Pubication.DescriptionEn;
                    data.ContentsEn = Pubication.ContentsEn;
                    data.DatePublic = Pubication.DatePublic;
                    return View(data);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }
        //end
        public async Task<ActionResult> ListComment(int id)
        {
            using (var db = new IOITDataContext())
            {
                var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.LinkSite = website.Url;
                var data = await (from cm in db.Comment
                                  join cus in db.Customer on cm.CustomerId equals cus.CustomerId
                                  where cm.TargetId == id
                                    && cm.TargetType == (int)Const.TypeComment.COMMENT_NEWS
                                    && cm.Status == (int)Const.Status.OK
                                  select new CommentDT
                                  {
                                      CommentId = cm.CommentId,
                                      CustomerId = cm.CustomerId,
                                      CustomerName = cus.FullName,
                                      TargetId = cm.TargetId,
                                      TargetType = cm.TargetType,
                                      Contents = cm.Contents,
                                      CommentParentId = cm.CommentParentId,
                                      SumLike = cm.NumberLike,
                                      CreatedAt = cm.CreatedAt,
                                      UpdateAt = cm.UpdateAt,
                                      Status = cm.Status,
                                  }).OrderByDescending(e => e.CreatedAt).ToListAsync();

                ViewBag.NewsId = id;
                return PartialView("_ListComment", data);
            }
        }
        public static List<CommentDT> getListComment(List<CommentDT> input, int parent)
        {
            List<CommentDT> listComments = new List<CommentDT>();
            var data = input.Where(e => e.CommentParentId == parent).ToList();
            foreach (var item in data)
            {
                item.commentChild = getListComment(input, (int)item.CommentParentId);
                listComments.Add(item);
            }
            return listComments;
        }

        // Chi tiết Văn bản
        public ActionResult LegalDocs(int Id, string seoName)
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    ViewBag.SeoTitle = "Chi tiết văn bản";
                    ViewBag.Class = "body-detail";
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    var LegalDoc = db.LegalDoc.Where(e => e.LegalDocId == Id
                && e.Url == seoName && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                    ViewBag.Slug = LegalDoc.Url;
                    ViewBag.Id = LegalDoc.LegalDocId;
                    string exAttach = LegalDoc.Attactment;
                    ViewBag.ext = Path.GetExtension(exAttach);
                    if (LegalDoc == null)
                        return Redirect("/Home/Error");

                    LegalDocDT data = new LegalDocDT();
                    data.LegalDocId = LegalDoc.LegalDocId;
                    data.Name = LegalDoc.Name;
                    data.Code = LegalDoc.Code;
                    data.DateIssue = LegalDoc.DateIssue;
                    data.DateEffect = LegalDoc.DateEffect;
                    data.Url = LegalDoc.Url;
                    data.Signer = LegalDoc.Signer;
                    data.AgencyIssue = LegalDoc.AgencyIssue;
                    data.YearIssue = LegalDoc.YearIssue;
                    data.YearIssue = LegalDoc.YearIssue;
                    data.TichYeu = LegalDoc.TichYeu;
                    data.Attactment = LegalDoc.Attactment;
                    data.Note = LegalDoc.Note;
                    data.Contents = LegalDoc.Contents;
                    data.AgencyIssued = LegalDoc.AgencyIssued;
                    return View(data);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public async Task<ActionResult> DetailData(long id, string seoName)
        {
            try
            {
                ViewBag.access_token = "'" + HttpContext.Session.GetString("access_token") + "'";
                ViewBag.CustomerId = HttpContext.Session.GetInt32("CustomerId");
                ViewBag.DatasetId = id;
                CustomerLogin customerLogin = new CustomerLogin();
                customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
                customerLogin.access_token = HttpContext.Session.GetString("access_token");
                ViewBag.CustomerLogin = customerLogin;
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;

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
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_DATASET
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
                            var seoLang = db.DataSet.Where(e => e.DataSetId == idCate
                               && e.Status == (int)Const.Status.NORMAL
                               && e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("DetailData", new { id = idCate, seoName = seoLang.Url });
                        }
                    }

                    var data = await db.DataSet.Where(e => e.DataSetId == id
                     && e.Status == (int)Const.Status.NORMAL).Select(e => new DataSetDTO
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
                             Location = c.Location,
                         }).OrderBy(e => e.Location).ToList(),
                         researchArea = db.DataSetMapping.Where(c => c.DataSetId == e.DataSetId
                         && c.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA
                         && c.Status != (int)Const.Status.DELETED).Select(c => new CategoryDTL
                         {
                             CategoryId = (int)c.TargetId,
                             Location = c.Location,
                         }).OrderBy(e => e.Location).ToList(),
                         unit = db.Unit.Where(c => c.UnitId == e.UnitId).Select(c => new UnitDT
                         {
                             UnitId = c.UnitId,
                             Name = c.Name
                         }).ToList(),
                         licecses = db.News.Where(c => c.NewsId == e.LicenseId).Select(c => new NewsDTO
                         {
                             NewsId = c.NewsId,
                             Title = c.Title,
                             Url = c.Url,
                         }).FirstOrDefault(),
                         userCreated = db.Customer.Where(c => c.CustomerId == e.UserCreatedId).Select(c => new CustomerDT
                         {
                             UserId = c.CustomerId,
                             FullName = c.FullName,
                             UnitName = db.Unit.Where(u => u.UnitId == c.UnitId).Select(u => u.Name).FirstOrDefault(),
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
                         && c.TargetType == (int)Const.TypeAttachment.FILE_DATASET).Select(c => new AttactmentDTO
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
                         }).ToList(),
                     }).FirstOrDefaultAsync();

                    if (data == null)
                        return Redirect("/Home/Error");

                    string strSEO = data.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("DetailData", new { id = id, seoName = strSEO });

                    ViewBag.SeoTitle = data.MetaTitle;
                    ViewBag.SeoDescription = data.MetaDescription;
                    ViewBag.SeoKeywords = data.MetaKeyword;
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    //var WebsiteLink = db.Website.Where(c => c.WebsiteId == 1 && c.Status != (int)Const.Status.DELETED).FirstOrDefault().Url;
                    ViewBag.Logo = db.Website.Where(c => c.WebsiteId == 1 && c.Status != (int)Const.Status.DELETED).FirstOrDefault().LogoHeader;
                    ViewBag.Url = website.Url + "/" + seoName + "-" + id;
                    if (data.Image != "")
                        ViewBag.UrlImg = website.Url + "/" + "uploads/" + data.Image;
                    else
                        ViewBag.UrlImg = website.Url + "/" + "/uploads/home/no-image.png";

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

                    int time = 60;
                    //check xem có cập nhật lượt xem ko
                    //Lưu lại lượt xem
                    var ids = Request.Cookies["ViewDs"];
                    if (ids == null)
                    {
                        Response.Cookies.Append(
                                "ViewDs", id + "",
                                new CookieOptions
                                {
                                    Expires = DateTimeOffset.UtcNow.AddSeconds(time),
                                    IsEssential = true,
                                    Path = "/",
                                    HttpOnly = false,
                                }
                            );


                        data.ViewNumber = data.ViewNumber + 1;
                        //Tính lại điểm sao
                        var cs = await db.ConfigStar.Where(e =>
                        e.FromView <= data.ViewNumber && e.ToView >= data.ViewNumber
                        && e.FromDownload <= data.DownNumber && e.ToDownload >= data.DownNumber
                        && e.Operator == (int)Const.OperatorType.AND).OrderByDescending(e => e.Star).FirstOrDefaultAsync();
                        if (cs != null)
                            data.RateStar = cs.Star;
                        else
                        {
                            cs = await db.ConfigStar.Where(e =>
                            ((e.FromView <= data.ViewNumber && e.ToView >= data.ViewNumber)
                            || (e.FromDownload <= data.DownNumber && e.ToDownload >= data.DownNumber))
                            && e.Operator == (int)Const.OperatorType.OR).OrderByDescending(e => e.Star).FirstOrDefaultAsync();
                            if (cs != null)
                                data.RateStar = cs.Star;
                        }

                        //list pvud
                        var listAr = await db.DataSetMapping.Where(e => e.DataSetId == id
                        && e.TargetType == (int)Const.DataSetMapping.DATA_APPLICATION_RANGE).ToListAsync();
                        //list lvnc
                        var listRa = await db.DataSetMapping.Where(e => e.DataSetId == id
                        && e.TargetType == (int)Const.DataSetMapping.DATA_RESEARCH_AREA).ToListAsync();

                        //list unit
                        var listUnit = await db.DataSetMapping.Where(e => e.DataSetId == id
                        && e.TargetType == (int)Const.DataSetMapping.DATA_UNIT).ToListAsync();

                        int year = DateTime.Now.Year;
                        int month = DateTime.Now.Month;
                        int day = DateTime.Now.Day;
                        List<DataSetView> listDataSetViews = new List<DataSetView>();
                        List<DataSetView> listUpdateViews = new List<DataSetView>();
                        if (listAr.Count > 0)
                        {
                            foreach (var itemA in listAr)
                            {
                                if (listRa.Count > 0)
                                {
                                    foreach (var itemB in listRa)
                                    {
                                        if (listUnit.Count > 0)
                                        {
                                            foreach (var itemC in listUnit)
                                            {
                                                //check  xem có chưa để công view
                                                var checkView = await db.DataSetView.Where(e => e.DataSetId == id
                                                && e.ApplicationRangeId == itemA.TargetId
                                                && e.ResearchAreaId == itemB.TargetId
                                                && e.UnitId == itemC.TargetId
                                                && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                                                if (checkView != null)
                                                {
                                                    if (checkView.IpAddress == IpAddress() && checkView.UpdatedAt.Value.AddSeconds(time) < DateTime.Now)
                                                    {
                                                        checkView.ViewNumber = checkView.ViewNumber != null ? (checkView.ViewNumber + 1) : 1;
                                                        checkView.IpAddress = IpAddress();
                                                        checkView.UpdatedAt = DateTime.Now;
                                                        listUpdateViews.Add(checkView);
                                                    }
                                                }
                                                else
                                                {
                                                    DataSetView dataSetView = new DataSetView();
                                                    dataSetView.DataSetViewId = Guid.NewGuid();
                                                    dataSetView.DataSetId = id;
                                                    dataSetView.ViewNumber = 1;
                                                    dataSetView.ApplicationRangeId = itemA.TargetId;
                                                    dataSetView.ResearchAreaId = itemB.TargetId;
                                                    dataSetView.UnitId = itemC.TargetId;
                                                    dataSetView.IpAddress = IpAddress();
                                                    dataSetView.UpdatedId = 1;
                                                    dataSetView.CreatedId = 1;
                                                    dataSetView.CreatedAt = DateTime.Now;
                                                    dataSetView.UpdatedAt = DateTime.Now;
                                                    dataSetView.Status = (int)Const.Status.NORMAL;
                                                    listDataSetViews.Add(dataSetView);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //check  xem có chưa đẻ công view
                                            var checkView = await db.DataSetView.Where(e => e.DataSetId == id
                                            && e.ApplicationRangeId == itemA.TargetId
                                            && e.ResearchAreaId == itemB.TargetId
                                            && e.CreatedAt.Value.Year == year && e.CreatedAt.Value.Month == month && e.CreatedAt.Value.Day == day
                                            && e.UnitId == -1 && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                                            if (checkView != null)
                                            {
                                                if (checkView.IpAddress == IpAddress() && checkView.UpdatedAt.Value.AddSeconds(time) < DateTime.Now)
                                                {
                                                    checkView.ViewNumber = checkView.ViewNumber != null ? (checkView.ViewNumber + 1) : 1;
                                                    checkView.IpAddress = IpAddress();
                                                    checkView.UpdatedAt = DateTime.Now;
                                                    listUpdateViews.Add(checkView);
                                                }
                                            }
                                            else
                                            {
                                                DataSetView dataSetView = new DataSetView();
                                                dataSetView.DataSetViewId = Guid.NewGuid();
                                                dataSetView.DataSetId = id;
                                                dataSetView.ViewNumber = 1;
                                                dataSetView.ApplicationRangeId = itemA.TargetId;
                                                dataSetView.ResearchAreaId = itemB.TargetId;
                                                dataSetView.UnitId = -1;
                                                dataSetView.IpAddress = IpAddress();
                                                dataSetView.UpdatedId = 1;
                                                dataSetView.CreatedId = 1;
                                                dataSetView.CreatedAt = DateTime.Now;
                                                dataSetView.UpdatedAt = DateTime.Now;
                                                dataSetView.Status = (int)Const.Status.NORMAL;
                                                listDataSetViews.Add(dataSetView);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (listUnit.Count > 0)
                                    {
                                        foreach (var itemC in listUnit)
                                        {
                                            //check  xem có chưa đẻ công view
                                            var checkView = await db.DataSetView.Where(e => e.DataSetId == id
                                            && e.ApplicationRangeId == itemA.TargetId
                                            && e.ResearchAreaId == -1
                                            && e.UnitId == itemC.TargetId && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                                            if (checkView != null)
                                            {
                                                checkView.ViewNumber = checkView.ViewNumber != null ? (checkView.ViewNumber + 1) : 1;
                                                checkView.UpdatedAt = DateTime.Now;
                                                listUpdateViews.Add(checkView);
                                            }
                                            else
                                            {
                                                DataSetView dataSetView = new DataSetView();
                                                dataSetView.DataSetViewId = Guid.NewGuid();
                                                dataSetView.DataSetId = id;
                                                dataSetView.ViewNumber = 1;
                                                dataSetView.ApplicationRangeId = itemA.TargetId;
                                                dataSetView.ResearchAreaId = -1;
                                                dataSetView.UnitId = itemC.TargetId;
                                                dataSetView.IpAddress = IpAddress();
                                                dataSetView.UpdatedId = 1;
                                                dataSetView.CreatedId = 1;
                                                dataSetView.CreatedAt = DateTime.Now;
                                                dataSetView.UpdatedAt = DateTime.Now;
                                                dataSetView.Status = (int)Const.Status.NORMAL;
                                                listDataSetViews.Add(dataSetView);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //check  xem có chưa đẻ công view
                                        var checkView = await db.DataSetView.Where(e => e.DataSetId == id
                                        && e.ApplicationRangeId == itemA.TargetId
                                        && e.ResearchAreaId == -1
                                        && e.UnitId == -1 && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                                        if (checkView != null)
                                        {
                                            checkView.ViewNumber = checkView.ViewNumber != null ? (checkView.ViewNumber + 1) : 1;
                                            checkView.UpdatedAt = DateTime.Now;
                                            listUpdateViews.Add(checkView);
                                        }
                                        else
                                        {
                                            DataSetView dataSetView = new DataSetView();
                                            dataSetView.DataSetViewId = Guid.NewGuid();
                                            dataSetView.DataSetId = id;
                                            dataSetView.ViewNumber = 1;
                                            dataSetView.ApplicationRangeId = itemA.TargetId;
                                            dataSetView.ResearchAreaId = -1;
                                            dataSetView.UnitId = -1;
                                            dataSetView.IpAddress = IpAddress();
                                            dataSetView.UpdatedId = 1;
                                            dataSetView.CreatedId = 1;
                                            dataSetView.CreatedAt = DateTime.Now;
                                            dataSetView.UpdatedAt = DateTime.Now;
                                            dataSetView.Status = (int)Const.Status.NORMAL;
                                            listDataSetViews.Add(dataSetView);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (listRa.Count > 0)
                            {
                                foreach (var itemB in listRa)
                                {
                                    if (listUnit.Count > 0)
                                    {
                                        foreach (var itemC in listUnit)
                                        {
                                            //check  xem có chưa đẻ công view
                                            var checkView = await db.DataSetView.Where(e => e.DataSetId == id
                                            && e.ApplicationRangeId == -1
                                            && e.ResearchAreaId == itemB.TargetId
                                            && e.UnitId == itemC.TargetId && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                                            if (checkView != null)
                                            {
                                                checkView.ViewNumber = checkView.ViewNumber != null ? (checkView.ViewNumber + 1) : 1;
                                                checkView.UpdatedAt = DateTime.Now;
                                                listUpdateViews.Add(checkView);
                                            }
                                            else
                                            {
                                                DataSetView dataSetView = new DataSetView();
                                                dataSetView.DataSetViewId = Guid.NewGuid();
                                                dataSetView.DataSetId = id;
                                                dataSetView.ViewNumber = 1;
                                                dataSetView.ApplicationRangeId = -1;
                                                dataSetView.ResearchAreaId = itemB.TargetId;
                                                dataSetView.UnitId = itemC.TargetId;
                                                dataSetView.IpAddress = IpAddress();
                                                dataSetView.UpdatedId = 1;
                                                dataSetView.CreatedId = 1;
                                                dataSetView.CreatedAt = DateTime.Now;
                                                dataSetView.UpdatedAt = DateTime.Now;
                                                dataSetView.Status = (int)Const.Status.NORMAL;
                                                listDataSetViews.Add(dataSetView);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //check  xem có chưa đẻ công view
                                        var checkView = await db.DataSetView.Where(e => e.DataSetId == id
                                        && e.ApplicationRangeId == -1
                                        && e.ResearchAreaId == itemB.TargetId
                                        && e.UnitId == -1 && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                                        if (checkView != null)
                                        {
                                            checkView.ViewNumber = checkView.ViewNumber != null ? (checkView.ViewNumber + 1) : 1;
                                            checkView.UpdatedAt = DateTime.Now;
                                            listUpdateViews.Add(checkView);
                                        }
                                        else
                                        {
                                            DataSetView dataSetView = new DataSetView();
                                            dataSetView.DataSetViewId = Guid.NewGuid();
                                            dataSetView.DataSetId = id;
                                            dataSetView.ViewNumber = 1;
                                            dataSetView.ApplicationRangeId = -1;
                                            dataSetView.ResearchAreaId = itemB.TargetId;
                                            dataSetView.UnitId = -1;
                                            dataSetView.IpAddress = IpAddress();
                                            dataSetView.UpdatedId = 1;
                                            dataSetView.CreatedId = 1;
                                            dataSetView.CreatedAt = DateTime.Now;
                                            dataSetView.UpdatedAt = DateTime.Now;
                                            dataSetView.Status = (int)Const.Status.NORMAL;
                                            listDataSetViews.Add(dataSetView);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (listUnit.Count > 0)
                                {
                                    foreach (var itemC in listUnit)
                                    {
                                        //check  xem có chưa đẻ công view
                                        var checkView = await db.DataSetView.Where(e => e.DataSetId == id
                                        && e.ApplicationRangeId == -1
                                        && e.ResearchAreaId == -1
                                        && e.UnitId == itemC.TargetId && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                                        if (checkView != null)
                                        {
                                            checkView.ViewNumber = checkView.ViewNumber != null ? (checkView.ViewNumber + 1) : 1;
                                            checkView.UpdatedAt = DateTime.Now;
                                            listUpdateViews.Add(checkView);
                                        }
                                        else
                                        {
                                            DataSetView dataSetView = new DataSetView();
                                            dataSetView.DataSetViewId = Guid.NewGuid();
                                            dataSetView.DataSetId = id;
                                            dataSetView.ViewNumber = 1;
                                            dataSetView.ApplicationRangeId = -1;
                                            dataSetView.ResearchAreaId = -1;
                                            dataSetView.UnitId = itemC.TargetId;
                                            dataSetView.IpAddress = IpAddress();
                                            dataSetView.UpdatedId = 1;
                                            dataSetView.CreatedId = 1;
                                            dataSetView.CreatedAt = DateTime.Now;
                                            dataSetView.UpdatedAt = DateTime.Now;
                                            dataSetView.Status = (int)Const.Status.NORMAL;
                                            listDataSetViews.Add(dataSetView);
                                        }
                                    }
                                }
                                else
                                {
                                    //check  xem có chưa đẻ công view
                                    var checkView = await db.DataSetView.Where(e => e.DataSetId == id
                                    && e.ApplicationRangeId == -1
                                    && e.ResearchAreaId == -1
                                    && e.UnitId == -1 && e.Status != (int)Const.Status.DELETED).FirstOrDefaultAsync();
                                    if (checkView != null)
                                    {
                                        checkView.ViewNumber = checkView.ViewNumber != null ? (checkView.ViewNumber + 1) : 1;
                                        checkView.UpdatedAt = DateTime.Now;
                                        listUpdateViews.Add(checkView);
                                    }
                                    else
                                    {
                                        DataSetView dataSetView = new DataSetView();
                                        dataSetView.DataSetViewId = Guid.NewGuid();
                                        dataSetView.DataSetId = id;
                                        dataSetView.ViewNumber = 1;
                                        dataSetView.ApplicationRangeId = -1;
                                        dataSetView.ResearchAreaId = -1;
                                        dataSetView.UnitId = -1;
                                        dataSetView.IpAddress = IpAddress();
                                        dataSetView.UpdatedId = 1;
                                        dataSetView.CreatedId = 1;
                                        dataSetView.CreatedAt = DateTime.Now;
                                        dataSetView.UpdatedAt = DateTime.Now;
                                        dataSetView.Status = (int)Const.Status.NORMAL;
                                        listDataSetViews.Add(dataSetView);
                                    }
                                }
                            }
                        }
                        await db.DataSetView.AddRangeAsync(listDataSetViews);
                        db.DataSetView.UpdateRange(listUpdateViews);

                        db.Entry(data).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    return View(data);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult DetailFaq(int Id)
        {
            ViewBag.SeoTitle = "Chi tiết FAQ";
            ViewBag.SeoDescription = "Chi tiết FAQ";
            ViewBag.SeoKeywords = "Chi tiết FAQ";
            ViewBag.Class = "body-detail";
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;

                    var legalDoc = db.LegalDoc.Where(e => e.LegalDocId == Id
                    && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();

                    if (legalDoc == null)
                        return Redirect("/Home/Error");

                    return View(legalDoc);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
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