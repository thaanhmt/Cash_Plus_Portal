using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Web;

namespace IOITWebApp31.Controllers
{
    public class CategoryController : Controller
    {
        private static readonly ILog log = LogMaster.GetLogger("category", "category");
        private readonly IConfiguration _configuration;

        public CategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ActionResult Page(int id, int idw, string seoName)
        {
            ViewBag.Class = "body-detail";
            try
            {
                using (var db = new IOITDataContext())
                {
                    if (id == 4102 || id == 4103 || id == 4105 || id == 4106 || id == 4107)
                    {
                        ViewBag.Class = "body-detail";
                    }
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
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY
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
                            var seoLang = db.Category.Where(e => e.CategoryId == idCate
                            && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PAGE_NORMAL
                            && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                            && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("Page", new { id = idCate, idw, seoName = seoLang.Url });
                        }

                    }

                    var seo = db.Category.Where(e => e.CategoryId == id
                        && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PAGE_NORMAL
                        && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                        && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                    if (seo == null)
                        return RedirectToAction("Error", "Home");

                    string strSEO = seo.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("Page", new { id, idw, seoName = strSEO });

                    ViewBag.CategoryParentId = seo.CategoryParentId;
                    ViewBag.Name = seo.Name;
                    ViewBag.CategoryId = seo.CategoryId;
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    //ViewBag.Url = _configuration["Settings:Domain"] + Const.PAGE_NOMAL + seoName + "-" + id + ".html";
                    ViewBag.Url = _configuration["Settings:Domain"] + seoName;
                    if (seo.Image != "")
                        ViewBag.UrlImg = _configuration["Settings:Domain"] + "/uploads/" + seo.Image;
                    else
                        ViewBag.UrlImg = _configuration["Settings:Domain"] + "/Content/images/logo.svg";

                    if (seo.CategoryParentId != 0)
                    {
                        var cate = db.Category.Find(seo.CategoryParentId);
                        ViewBag.CategoryParentName = cate.Name;
                        ViewBag.CategoryParentUrl = cate.Url;
                    }

                    ViewBag.SeoTitle = seo.MetaTitle;
                    ViewBag.SeoDescription = seo.MetaDescription;
                    ViewBag.SeoKeywords = seo.MetaKeyword;
                    return View("Page", seo);
                }
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult News(int id, string seoName, int idw, int p = 1)
        {
            var CustomerId = HttpContext.Session.GetInt32("CustomerId");
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    ViewBag.Logo = website.LogoHeader;
                    var CatPar = db.Category.Where(e => e.CategoryParentId == id && e.Status != (int)Const.Status.DELETED).OrderByDescending(e => e.CreatedAt).ToList();
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    ViewBag.CatChild = CatPar;
                    ViewBag.CatChildCount = CatPar.Count();

                    //get url
                    string urlLang = Request.Cookies["ReturnUrl"] != null ? Request.Cookies["ReturnUrl"] : "/";
                    string url = Request.Path;
                    ViewBag.DomainName = url;
                    //ViewBag.IsComment = db.Category.Where(c => c.CategoryId == id && c.Status != (int)Const.Status.DELETED).OrderByDescending(e => e.CreatedAt).FirstOrDefault().IsComment;
                    ViewBag.CategoryId = db.Category.Where(c => c.CategoryId == id && c.TypeCategoryId != 14 && c.TypeCategoryId != 15 && c.Status != (int)Const.Status.DELETED).OrderByDescending(e => e.CreatedAt).FirstOrDefault().CategoryId;
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
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY
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
                            var seoLang = db.Category.Where(e => e.CategoryId == idCate
                            && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_TEXT
                            && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                            && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("CategoryNews", new { id = idCate, seoName = seoLang.Url, idw, p });
                        }

                    }
                    ViewBag.LanguageId = Request.Cookies["LanguageId"] != null ? Request.Cookies["LanguageId"] : Const.LANGUAGEID + "";
                    if (p < 1 || idw != Const.WEBSITEID)
                    {
                        return Redirect("/Home/Error");
                    }

                    var seo = db.Category.Where(e => e.CategoryId == id
                    && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_TEXT
                    && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                    && e.Status == (int)Const.Status.NORMAL).First();
                    if (seo == null)
                        return Redirect("/Home/Error");

                    string strSEO = seo.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("News", new { id, seoName = strSEO, idw, p });

                    var data = (from cm in db.CategoryMapping
                                join n in db.News on cm.TargetId equals n.NewsId
                                where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                && n.CompanyId == Const.COMPANYID
                                && cm.CategoryId == id
                                && n.WebsiteId == Const.WEBSITEID
                                && n.Status == (int)Const.Status.NORMAL
                                && cm.Status != (int)Const.Status.DELETED
                                select n).OrderByDescending(e => e.DateStartActive).ToList();
                    //if (id == 4628 || id == 4629)
                    //{
                    //    int np = 12;
                    //    ViewBag.np = np;
                    //    if (((data.Count() - 1) / np) + 1 < p)
                    //    {
                    //        return Redirect("/Home/Error");
                    //    }

                    //    if (p == 1)
                    //    {
                    //        ViewBag.SeoTitle = seo.MetaTitle;
                    //        ViewBag.SeoDescription = seo.MetaDescription;
                    //        ViewBag.SeoKeywords = seo.MetaKeyword;
                    //    }
                    //    else
                    //    {
                    //        ViewBag.SeoTitle = seo.MetaTitle;
                    //        ViewBag.SeoDescription = seo.MetaDescription + " " + p;
                    //        ViewBag.SeoKeywords = seo.MetaKeyword;
                    //    }
                    //}
                    //else
                    //{
                    //int np = 12;
                    //ViewBag.np = np;
                    //if (((data.Count() - 1) / np) + 1 < p)
                    //{
                    //    return Redirect("/Home/Error");
                    //}

                    if (p == 1)
                    {
                        ViewBag.SeoTitle = seo.MetaTitle;
                        ViewBag.SeoDescription = seo.MetaDescription;
                        ViewBag.SeoKeywords = seo.MetaKeyword;
                    }
                    else
                    {
                        ViewBag.SeoTitle = seo.MetaTitle;
                        ViewBag.SeoDescription = seo.MetaDescription + " " + p;
                        ViewBag.SeoKeywords = seo.MetaKeyword;
                    }
                    //}


                    ViewBag.Url = _configuration["Settings:Domain"] + Const.CATEGORY_NEWS + seoName + "-" + idw + "-" + id + ".html";
                    if (seo.Image != "")
                        ViewBag.UrlImg = _configuration["Settings:Domain"] + "/uploads/" + seo.Image;
                    else
                        ViewBag.UrlImg = _configuration["Settings:Domain"] + "/Content/images/logo.svg";

                    ViewBag.CategoryParentId = seo.CategoryParentId;
                    ViewBag.CategoryId = seo.CategoryId;
                    ViewBag.Image = seo.Image;
                    if (seo.CategoryParentId != 0)
                    {
                        var cate = db.Category.Find(seo.CategoryParentId);
                        ViewBag.CategoryParentName = cate.Name;
                        ViewBag.CategoryParentUrl = cate.Url;
                    }

                    //ViewBag.Id = seo.Id;
                    ViewBag.CategoryName = seo.Name;
                    ViewBag.CategoryUrl = seo.Url;
                    ViewBag.Description = seo.Description;
                    //ViewBag.Page = p;

                    //ViewBag.CountAll = (_data2.Count - 1).ToString().Trim();
                    //ViewBag.total = (data.Count());
                    //ViewBag.page = p;
                    //if (id == 4628 || id == 4629)
                    //{
                    //    var np = 12;
                    //    if ((ViewBag.total - 1) % 12 == 0)
                    //    {
                    //        ViewBag.totalPage = (ViewBag.total - 1) / np;

                    //    }
                    //    else
                    //    {
                    //        ViewBag.totalPage = ((ViewBag.total - 1) / np) + 1;
                    //    }
                    //    // hien thi so page

                    //    ViewBag.Pre = p - 1;
                    //    ViewBag.Next = p + 1;
                    //    var list = data.Skip((np * (p - 1)) + 1).Take(np).ToList();
                    //    return View("News", list);
                    //}
                    //else
                    //{
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

                    return View("News", list);
                    //}
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Image(int id, string seoName, int p = 1)
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
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY
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
                            var seoLang = db.Category.Where(e => e.CategoryId == idCate
                            && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_IMAGE
                            && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                            && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("Image", new { id = idCate, seoName = seoLang.Url, p });
                        }
                    }

                    int np = 8;

                    if (p < 1)
                    {
                        return Redirect("/Home/Error");
                    }

                    var seo = db.Category.Where(e => e.CategoryId == id
                    && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_IMAGE
                    && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                    && e.Status == (int)Const.Status.NORMAL).First();
                    if (seo == null)
                        return Redirect("/Home/Error");

                    string strSEO = seo.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("Image", new { id = id, seoName = strSEO, p = p });

                    var data = (from cm in db.CategoryMapping
                                join n in db.News on cm.TargetId equals n.NewsId
                                where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                && n.CompanyId == Const.COMPANYID && cm.CategoryId == id
                                && n.WebsiteId == Const.WEBSITEID
                                && n.Status == (int)Const.Status.NORMAL
                                && cm.Status != (int)Const.Status.DELETED
                                select n).OrderByDescending(e => e.CreatedAt).ToList();

                    if (((data.Count() - 1) / np) + 1 < p)
                    {
                        return Redirect("/Home/Error");
                    }

                    if (p == 1)
                    {
                        ViewBag.SeoTitle = seo.MetaTitle;
                        ViewBag.SeoDescription = seo.MetaDescription;
                        ViewBag.SeoKeywords = seo.MetaKeyword;
                    }
                    else
                    {
                        ViewBag.SeoTitle = seo.MetaTitle + " " + p;
                        ViewBag.SeoDescription = seo.MetaDescription + " " + p;
                        ViewBag.SeoKeywords = seo.MetaKeyword;
                    }

                    ViewBag.Url = _configuration["Settings:Domain"] + Const.CATEGORY_NEWS + seoName + "-" + id + ".html";
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
                    ViewBag.Name = seo.Name;
                    //ViewBag.Url = seo.Url;
                    ViewBag.Description = seo.Description;
                    //ViewBag.Page = p;

                    //ViewBag.CountAll = (_data2.Count - 1).ToString().Trim();
                    ViewBag.total = (data.Count() - 1);
                    ViewBag.page = p;
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;
                    var list = data.Skip(np * (p - 1)).Take(np).ToList();
                    return View("News", list);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Video(int id, string seoName, int p = 1)
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
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY
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
                            var seoLang = db.Category.Where(e => e.CategoryId == idCate
                            && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_VIDEO
                            && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                            && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("Video", new { id = idCate, seoName = seoLang.Url, p });
                        }
                    }

                    int np = 8;

                    if (p < 1)
                    {
                        return Redirect("/Home/Error");
                    }

                    var seo = db.Category.Where(e => e.CategoryId == id
                    && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_VIDEO
                    && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                    && e.Status == (int)Const.Status.NORMAL).First();

                    if (seo == null)
                        return Redirect("/Home/Error");

                    string strSEO = seo.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("Video", new { id = id, seoName = strSEO, p = p });

                    var data = (from cm in db.CategoryMapping
                                join n in db.News on cm.TargetId equals n.NewsId
                                where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                && n.CompanyId == Const.COMPANYID && cm.CategoryId == id
                                && n.WebsiteId == Const.WEBSITEID
                                && n.Status == (int)Const.Status.NORMAL
                                && cm.Status != (int)Const.Status.DELETED
                                select n).OrderByDescending(e => e.CreatedAt).ToList();

                    if (((data.Count() - 1) / np) + 1 < p)
                    {
                        return Redirect("/Home/Error");
                    }

                    if (p == 1)
                    {
                        ViewBag.SeoTitle = seo.MetaTitle;
                        ViewBag.SeoDescription = seo.MetaDescription;
                        ViewBag.SeoKeywords = seo.MetaKeyword;
                    }
                    else
                    {
                        ViewBag.SeoTitle = seo.MetaTitle + " " + p;
                        ViewBag.SeoDescription = seo.MetaDescription + " " + p;
                        ViewBag.SeoKeywords = seo.MetaKeyword;
                    }

                    ViewBag.Url = _configuration["Settings:Domain"] + Const.CATEGORY_NEWS + seoName + "-" + id + ".html";
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
                    ViewBag.Name = seo.Name;
                    //ViewBag.Url = seo.Url;
                    ViewBag.Description = seo.Description;
                    //ViewBag.Page = p;

                    //ViewBag.CountAll = (_data2.Count - 1).ToString().Trim();
                    ViewBag.total = (data.Count() - 1);
                    ViewBag.page = p;
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;
                    var list = data.Skip(np * (p - 1)).Take(np).ToList();
                    return View("News", list);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Attactment(int id, string seoName, int p = 1)
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
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY
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
                            var seoLang = db.Category.Where(e => e.CategoryId == idCate
                            && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_ATTACTMENT
                            && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                            && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("Attactment", new { id = idCate, seoName = seoLang.Url, p });
                        }
                    }

                    int np = 8;

                    if (p < 1)
                    {
                        return Redirect("/Home/Error");
                    }

                    var seo = db.Category.Where(e => e.CategoryId == id
                    && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_ATTACTMENT
                    && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                    && e.Status == (int)Const.Status.NORMAL).First();

                    if (seo == null)
                        return Redirect("/Home/Error");

                    string strSEO = seo.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("Attactment", new { id = id, seoName = strSEO, p = p });

                    var data = (from cm in db.CategoryMapping
                                join n in db.News on cm.TargetId equals n.NewsId
                                where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                && n.CompanyId == Const.COMPANYID && cm.CategoryId == id
                                && n.WebsiteId == Const.WEBSITEID
                                && n.Status == (int)Const.Status.NORMAL
                                && cm.Status != (int)Const.Status.DELETED
                                select n).OrderByDescending(e => e.CreatedAt).ToList();

                    if (((data.Count() - 1) / np) + 1 < p)
                    {
                        return Redirect("/Home/Error");
                    }

                    if (p == 1)
                    {
                        ViewBag.SeoTitle = seo.MetaTitle;
                        ViewBag.SeoDescription = seo.MetaDescription;
                        ViewBag.SeoKeywords = seo.MetaKeyword;
                    }
                    else
                    {
                        ViewBag.SeoTitle = seo.MetaTitle + " " + p;
                        ViewBag.SeoDescription = seo.MetaDescription + " " + p;
                        ViewBag.SeoKeywords = seo.MetaKeyword;
                    }

                    ViewBag.Url = _configuration["Settings:Domain"] + Const.CATEGORY_NEWS + seoName + "-" + id + ".html";
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
                    ViewBag.Name = seo.Name;
                    //ViewBag.Url = seo.Url;
                    ViewBag.Description = seo.Description;
                    //ViewBag.Page = p;

                    //ViewBag.CountAll = (_data2.Count - 1).ToString().Trim();
                    ViewBag.total = (data.Count() - 1);
                    ViewBag.page = p;
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;
                    var list = data.Skip(np * (p - 1)).Take(np).ToList();
                    return View("News", list);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Notification(int id, string seoName, int p = 1)
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
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY
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
                            var seoLang = db.Category.Where(e => e.CategoryId == idCate
                            && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_NOTIFICATION
                            && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                            && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("Notification", new { id = idCate, seoName = seoLang.Url, p });
                        }
                    }

                    int np = 8;

                    if (p < 1)
                    {
                        return Redirect("/Home/Error");
                    }

                    var seo = db.Category.Where(e => e.CategoryId == id
                    && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_NEWS_NOTIFICATION
                    && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                    && e.Status == (int)Const.Status.NORMAL).First();

                    if (seo == null)
                        return Redirect("/Home/Error");

                    string strSEO = seo.Url;
                    if (seoName != strSEO)
                        return RedirectToRoute("Notification", new { id = id, seoName = strSEO, p = p });

                    var data = (from cm in db.CategoryMapping
                                join n in db.News on cm.TargetId equals n.NewsId
                                where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                && n.CompanyId == Const.COMPANYID && cm.CategoryId == id
                                && n.WebsiteId == Const.WEBSITEID
                                && n.Status == (int)Const.Status.NORMAL
                                && cm.Status != (int)Const.Status.DELETED
                                select n).OrderByDescending(e => e.CreatedAt).ToList();

                    if (((data.Count() - 1) / np) + 1 < p)
                    {
                        return Redirect("/Home/Error");
                    }

                    if (p == 1)
                    {
                        ViewBag.SeoTitle = seo.MetaTitle;
                        ViewBag.SeoDescription = seo.MetaDescription;
                        ViewBag.SeoKeywords = seo.MetaKeyword;
                    }
                    else
                    {
                        ViewBag.SeoTitle = seo.MetaTitle + " " + p;
                        ViewBag.SeoDescription = seo.MetaDescription + " " + p;
                        ViewBag.SeoKeywords = seo.MetaKeyword;
                    }

                    ViewBag.Url = _configuration["Settings:Domain"] + Const.CATEGORY_NEWS + seoName + "-" + id + ".html";
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
                    ViewBag.Name = seo.Name;
                    //ViewBag.Url = seo.Url;
                    ViewBag.Description = seo.Description;
                    //ViewBag.Page = p;

                    //ViewBag.CountAll = (_data2.Count - 1).ToString().Trim();
                    ViewBag.total = (data.Count() - 1);
                    ViewBag.page = p;
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;
                    var list = data.Skip(np * (p - 1)).Take(np).ToList();
                    return View("News", list);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Product(int id, string seoName, int idw, int p = 1)
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
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY
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
                            var seoLang = db.Category.Where(e => e.CategoryId == idCate
                            && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PRODUCT
                            && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                            && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("CategoryProduct", new { id = idCate, seoName = seoLang.Url });
                        }
                    }

                    if (id != 0)
                    {
                        var seo = db.Category.Where(e => e.CategoryId == id
                        && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PRODUCT
                        && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                        && e.Status == (int)Const.Status.NORMAL).First();

                        if (seo == null)
                            return Redirect("/Home/Error");
                        ViewBag.CategoryName = seo.Name;
                        ViewBag.SeoTitle = seo.MetaTitle;
                        ViewBag.SeoDescription = seo.Contents;
                        ViewBag.SeoKeywords = seo.MetaDescription;
                        ViewBag.Image = seo.Image;
                    }
                    else
                    {
                        string MetaTitle = "Danh mục sản phẩm";
                        ViewBag.SeoTitle = MetaTitle;
                        ViewBag.SeoDescription = MetaTitle;
                        ViewBag.SeoKeywords = MetaTitle;
                        ViewBag.Image = "/images/slideprd.jpg";
                    }

                    //Lấy ds danh mục sản phẩm, ds thương hiệu, ds nhà sx, ds khoảng giá, ds sắp xếp và các giá trị lọc hiện tại
                    ViewBag.cs = db.Category.Where(ca => ca.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PRODUCT && ca.CategoryParentId == 0 && ca.Status != (int)Const.Status.DELETED).ToList();
                    ViewBag.c = id;

                    ViewBag.CatChild = db.Category.Where(e => e.CategoryParentId == id && e.Status != (int)Const.Status.DELETED).OrderBy(e => e.CreatedAt).ToList();


                    ViewBag.ts = db.Manufacturer.Where(ma => ma.TypeOriginId == 2 && ma.Status != (int)Const.Status.DELETED).ToList();

                    ViewBag.ms = db.Manufacturer.Where(ma => ma.TypeOriginId == 1 && ma.Status != (int)Const.Status.DELETED).ToList();

                    ViewBag.rs = db.CategoryRank.Where(cr => cr.TypeRankId == (int)Const.TypeRank.K_G && cr.Status != (int)Const.Status.DELETED).OrderBy(cr => cr.CategoryRankId).ToList();

                    int LoaiHinh_SapXep_DanhmucSanPham = int.Parse(_configuration["KeyId:LoaiHinh_SapXep_DanhmucSanPham"]);
                    ViewBag.os = db.TypeAttributeItem.Where(ty => ty.TypeAttributeId == LoaiHinh_SapXep_DanhmucSanPham && ty.Status != (int)Const.Status.DELETED).OrderBy(ty => ty.Location).ToList();

                    ViewBag.listpPro1 = (from cm in db.CategoryMapping
                                         join pro in db.Product on cm.TargetId equals pro.ProductId
                                         where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                                         && cm.CategoryId == id
                                         && pro.Status == (int)Const.Status.NORMAL
                                         && cm.Status != (int)Const.Status.DELETED
                                         select pro
                                           ).ToList();

                    return View("Product");
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult ProductChild(int id, string seoName, int p = 1)
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
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY
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
                            var seoLang = db.Category.Where(e => e.CategoryId == idCate
                            && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PRODUCT
                            && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                            && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return RedirectToRoute("CategoryProductChild", new { id = idCate, seoName = seoLang.Url, p = p });
                        }
                    }

                    if (id != 0)
                    {
                        var seo = db.Category.Where(e => e.CategoryId == id
                        && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PRODUCT
                        && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                        && e.Status == (int)Const.Status.NORMAL).First();

                        if (seo == null)
                            return Redirect("/Home/Error");
                        ViewBag.CategoryName = seo.Name;
                        ViewBag.CategoryId = seo.CategoryId;
                        ViewBag.MoTa = seo.Description;
                        ViewBag.SeoTitle = seo.MetaTitle;
                        ViewBag.SeoDescription = seo.MetaKeyword;
                        ViewBag.SeoKeywords = seo.MetaDescription;
                        ViewBag.listCatChild = db.Category.Where(c => c.CategoryParentId == seo.CategoryParentId && c.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PRODUCT && c.Status == (int)Const.Status.NORMAL).ToList();


                    }
                    else
                    {
                        string MetaTitle = "Danh mục sản phẩm";
                        ViewBag.SeoTitle = MetaTitle;
                        ViewBag.SeoDescription = MetaTitle;
                        ViewBag.SeoKeywords = MetaTitle;
                    }

                    //Lấy ds danh mục sản phẩm, ds thương hiệu, ds nhà sx, ds khoảng giá, ds sắp xếp và các giá trị lọc hiện tại
                    ViewBag.cs = db.Category.Where(ca => ca.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PRODUCT && ca.CategoryParentId == 0 && ca.Status != (int)Const.Status.DELETED).ToList();
                    ViewBag.c = id;

                    ViewBag.CatChild = db.Category.Where(e => e.CategoryParentId == id && e.Status != (int)Const.Status.DELETED).OrderByDescending(e => e.CreatedAt).ToList();

                    ViewBag.Solution = db.News.Where(e => e.TypeNewsId == 6 && e.LanguageId == lang && e.Status == (int)Const.Status.NORMAL).ToList();



                    ViewBag.ts = db.Manufacturer.Where(ma => ma.TypeOriginId == 2 && ma.Status != (int)Const.Status.DELETED).ToList();

                    var listpPro = (from cm in db.CategoryMapping
                                    join pro in db.Product on cm.TargetId equals pro.ProductId
                                    where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                                    && cm.CategoryId == id
                                    && pro.Status == (int)Const.Status.NORMAL
                                    && cm.Status != (int)Const.Status.DELETED
                                    select pro
                                           ).OrderBy(pro => pro.CreatedAt).ToList();

                    ViewBag.ms = db.Manufacturer.Where(ma => ma.TypeOriginId == 1 && ma.Status != (int)Const.Status.DELETED).ToList();

                    ViewBag.rs = db.CategoryRank.Where(cr => cr.TypeRankId == (int)Const.TypeRank.K_G && cr.Status != (int)Const.Status.DELETED).OrderBy(cr => cr.CategoryRankId).ToList();

                    int LoaiHinh_SapXep_DanhmucSanPham = int.Parse(_configuration["KeyId:LoaiHinh_SapXep_DanhmucSanPham"]);
                    ViewBag.os = db.TypeAttributeItem.Where(ty => ty.TypeAttributeId == LoaiHinh_SapXep_DanhmucSanPham && ty.Status != (int)Const.Status.DELETED).OrderBy(ty => ty.Location).ToList();


                    return View("ProductChild", listpPro);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult GroupProduct()
        {
            try
            {
                ViewBag.CategoryName = "Sản phẩm";
                ViewBag.SeoTitle = ViewBag.CategoryName;

                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    var data = (from c in db.Category
                                where c.CategoryParentId == 0
                                && c.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PRODUCT
                                && c.Status == (int)Const.Status.NORMAL
                                select c).OrderBy(e => e.Location).ToList();

                    return View("GroupProduct", data);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult GroupNews()
        {
            try
            {
                ViewBag.CategoryName = "Tin tức - sự kiện";

                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    int CategoryId = 3220;
                    var cate = db.Category.Find(CategoryId);
                    if (cate != null)
                    {
                        ViewBag.CategoryId = CategoryId;
                        ViewBag.Name = cate.Name;
                        ViewBag.CategoryUrl = cate.Url;
                    }
                    IEnumerable<News> data = (from n in db.News
                                              join cn in db.CategoryMapping on n.NewsId equals cn.TargetId
                                              where cn.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                              && n.Status == (int)Const.Status.NORMAL
                                              && cn.Status == (int)Const.Status.NORMAL
                                              && cn.CategoryId == 3220
                                              select n).OrderByDescending(e => e.CreatedAt).Take(6).ToList();

                    ViewBag.SeoTitle = ViewBag.CategoryName;
                    ViewBag.SeoDescription = ViewBag.CategoryName;
                    ViewBag.SeoKeywords = ViewBag.CategoryName;

                    return View("GroupNews", data);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult ListProduct(int c = 0, string t = "-1", string m = "-1", string r = "-1", int o = -1, int p = 1)
        {
            try
            {
                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    //if (c != -1)
                    //{
                    //    var seo = db.Category.Where(e => e.CategoryId == c
                    //    && e.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PRODUCT
                    //    && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                    //    && e.Status == (int)Const.Status.NORMAL).First();

                    //    if (seo == null)
                    //        return Redirect("/Home/Error");
                    //    ViewBag.CategoryName = seo.Name;
                    //}

                    //Lấy ds danh mục sản phẩm, ds thương hiệu, ds nhà sx, ds khoảng giá, ds sắp xếp và các giá trị lọc hiện tại
                    //ViewBag.cs = db.Category.Where(ca => ca.TypeCategoryId == (int)Const.TypeCategory.CATEGORY_PRODUCT && ca.CategoryParentId == 0 && ca.Status != (int)Const.Status.DELETED).ToList();
                    //ViewBag.c = c;

                    //ViewBag.ts = db.Manufacturer.Where(ma => ma.TypeOriginId == 2 && ma.Status != (int)Const.Status.DELETED).ToList();

                    //ViewBag.ms = db.Manufacturer.Where(ma => ma.TypeOriginId == 1 && ma.Status != (int)Const.Status.DELETED).ToList();

                    //ViewBag.rs = db.CategoryRank.Where(cr => cr.TypeRankId == (int)Const.TypeRank.K_G && cr.Status != (int)Const.Status.DELETED).OrderBy(cr => cr.CategoryRankId).ToList();

                    int LoaiHinh_SapXep_DanhmucSanPham = int.Parse(_configuration["KeyId:LoaiHinh_SapXep_DanhmucSanPham"]);
                    ViewBag.os = db.TypeAttributeItem.Where(ty => ty.TypeAttributeId == LoaiHinh_SapXep_DanhmucSanPham && ty.Status != (int)Const.Status.DELETED).OrderBy(ty => ty.Location).ToList();
                    ViewBag.o = o;

                    ViewBag.p = p;
                    int np = 12;
                    ViewBag.np = np;

                    //Lấy ra dữ liệu
                    IQueryable<Product> data;

                    if (c == 0) data = db.Product.Where(pr => pr.Status != (int)Const.Status.DELETED);
                    else data = (from cm in db.CategoryMapping
                                 join pr in db.Product on cm.TargetId equals pr.ProductId
                                 where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                                 && cm.CategoryId == c
                                 && pr.Status == (int)Const.Status.NORMAL
                                 && cm.Status != (int)Const.Status.DELETED
                                 select pr).OrderByDescending(e => e.CreatedAt);

                    if (t != "-1")
                    {
                        List<int> ts = t.Split(",").Select(int.Parse).ToList();
                        ViewBag.t = ts;
                        data = data.Where(pr => ts.Contains(pr.TrademarkId != null ? (int)pr.TrademarkId : -1));
                    }
                    if (m != "-1")
                    {
                        List<int> ms = m.Split(",").Select(int.Parse).ToList();
                        ViewBag.m = ms;
                        data = data.Where(pr => ms.Contains(pr.ManufacturerId != null ? (int)pr.ManufacturerId : -1));
                    }
                    if (r != "-1")
                    {
                        List<int> rs = r.Split(",").Select(int.Parse).ToList();
                        ViewBag.r = rs;

                        string query = "";
                        foreach (var item in rs)
                        {
                            CategoryRank categoryRank = db.CategoryRank.Where(cr => cr.CategoryRankId == item && cr.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (categoryRank != null)
                            {
                                if (categoryRank.RankStart != null && categoryRank.RankEnd != null)
                                {
                                    if (query == "")
                                    {
                                        query = "(PriceSpecial > " + categoryRank.RankStart + " AND PriceSpecial <= " + categoryRank.RankEnd + ")";
                                    }
                                    else
                                    {
                                        query += " OR " + "(PriceSpecial > " + categoryRank.RankStart + " AND PriceSpecial <= " + categoryRank.RankEnd + ")";
                                    }
                                }
                            }
                        }

                        if (query != "")
                        {
                            query = HttpUtility.UrlDecode(query);
                            data = data.Where(query);
                        }
                    }
                    if (o != (int)Const.TypeOrderByCategoryProduct.DEFAULT)
                    {
                        TypeAttributeItem typeAttributeItem = db.TypeAttributeItem.Where(ty => ty.TypeAttributeItemId == o).FirstOrDefault();
                        if (typeAttributeItem != null)
                        {
                            if (typeAttributeItem.Code != null)
                            {
                                data = data.OrderBy(typeAttributeItem.Code);
                            }
                        }
                    }

                    if (((data.Count() - 1) / np) + 1 < p)
                    {
                        return Redirect("/Home/Error");
                    }

                    //string MetaTitle = "Danh mục sản phẩm";
                    //ViewBag.SeoTitle = MetaTitle;
                    //ViewBag.SeoDescription = MetaTitle;
                    //ViewBag.SeoKeywords = MetaTitle;

                    ViewBag.total = (data.Count() - 1);
                    ViewBag.page = p;
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;

                    var list = data.Count() > 0 ? data.Skip(np * (p - 1)).Take(np).ToList() : data.ToList();

                    int customerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
                    ViewBag.CustomerId = customerId;

                    if (customerId != -1)
                    {
                        foreach (var item in list)
                        {
                            ProductCustomer productCustomer = db.ProductCustomer.Where(pc => pc.TargetId == item.ProductId && pc.CustomerId == customerId && pc.TargetType == (int)Const.TypeProductCustomer.LOVE && pc.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (productCustomer != null) item.Status = 10;
                        }
                    }

                    return PartialView("_ListProduct", list);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult ListGallery(int p = 1)
        {
            ViewBag.Class = "body-detail";
            try
            {
                ViewBag.CategoryName = "Thư viện hình ảnh";

                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;

                    IEnumerable<News> data = db.News.Where(e => e.TypeNewsId == 3 && e.Status != (int)Const.Status.DELETED).ToList();

                    ViewBag.SeoTitle = ViewBag.CategoryName;
                    ViewBag.SeoDescription = ViewBag.CategoryName;
                    ViewBag.SeoKeywords = ViewBag.CategoryName;

                    int np = 9;
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
        public ActionResult ListVideo(int p = 1)
        {
            ViewBag.Class = "body-detail";
            try
            {
                ViewBag.CategoryName = "Thư viện video";

                using (var db = new IOITDataContext())
                {
                    var website = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                    ViewBag.LinkSite = website.Url;
                    IEnumerable<News> data = db.News.Where(e => e.TypeNewsId == 4 && e.Status != (int)Const.Status.DELETED).ToList();

                    ViewBag.SeoTitle = ViewBag.CategoryName;
                    ViewBag.SeoDescription = ViewBag.CategoryName;
                    ViewBag.SeoKeywords = ViewBag.CategoryName;

                    int np = 9;
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

        public ActionResult SolutionsServices(int p = 1)
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

                    if (lang != Const.LANGUAGEID)
                    {
                        ViewBag.CategoryName = "List all solution";
                    }
                    else
                    {
                        ViewBag.CategoryName = "Tất cả giải pháp";
                    }

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

                        if (lang == 1)
                        {
                            return Redirect("/" + Const.PAGE_SOLUTION_VN);
                        }
                        else if (lang == 1007)
                        {
                            return Redirect("/" + Const.PAGE_SOLUTION);
                        }

                    }

                    IEnumerable<News> data = db.News.Where(e => e.TypeNewsId == 6 && e.LanguageId == lang && e.Status != (int)Const.Status.DELETED).ToList();

                    ViewBag.SeoTitle = ViewBag.CategoryName;
                    ViewBag.SeoDescription = ViewBag.CategoryName;
                    ViewBag.SeoKeywords = ViewBag.CategoryName;

                    int np = 120;
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

        public ActionResult PageParent(int id)
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
                         && e.TargetType == (int)Const.TypeLanguageMapping.LANGUAGE_CATEGORY
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
                            var seoLang = db.Category.Where(e => e.CategoryId == idCate
                            && e.CompanyId == Const.COMPANYID && e.WebsiteId == Const.WEBSITEID
                            && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                            if (seoLang == null)
                                return Redirect("/");
                            return Redirect("/" + seoLang.Url);
                        }

                    }

                    var InfoCatParent = db.Category.Where(e => e.CategoryId == id && e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                    var data = db.Category.Where(e => e.CategoryParentId == id && e.Status == (int)Const.Status.NORMAL).ToList();
                    ViewBag.Des = InfoCatParent.Description;
                    return View(data);
                }

            }
            catch
            {
                return Redirect("/Home/Error");
            }

        }

        public ActionResult PageHTML(int id)
        {
            try
            {
                //get url
                string urlLang = Request.Cookies["ReturnUrl"] != null ? Request.Cookies["ReturnUrl"] : "/";
                string url = Request.Path;
                //get lang
                int lang = Request.Cookies["LanguageId"] != null ? int.Parse(Request.Cookies["LanguageId"]) : Const.LANGUAGEID;
                int langOld = Request.Cookies["LanguageOldId"] != null ? int.Parse(Request.Cookies["LanguageOldId"]) : Const.LANGUAGEID;

                if (lang == 1)
                {
                    ViewBag.SeoTitle = "Cơ cấu tổ chức";
                }
                else if (lang == 1007)
                {
                    ViewBag.SeoTitle = "Organizational structure";
                }
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

                    if (lang == 1)
                    {
                        return Redirect("/" + Const.PAGE_ABOUT_HTML_VN);
                    }
                    else if (lang == 1007)
                    {
                        return Redirect("/" + Const.PAGE_ABOUT_HTML);
                    }

                }

                using (var db = new IOITDataContext())
                {

                    return View("CoCauToChuc");

                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }

        }
        public ActionResult ListAllProduct(int p)
        {
            try
            {
                ViewBag.CategoryName = "Tất cả sản phẩm";

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

                    //int langId = Request.Cookies["LanguageId"] != null ? int.Parse(Request.Cookies["LanguageId"]) : Const.LANGUAGEID;
                    if (lang != 1)
                    {
                        ViewBag.CategoryName = "List all product";
                    }
                    else
                    {
                        ViewBag.CategoryName = "Tất cả sản phẩm";
                    }

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

                        if (lang == 1)
                        {
                            return Redirect("/" + Const.PAGE_ALL_PRODUCT_VN);
                        }
                        else if (lang == 1007)
                        {
                            return Redirect("/" + Const.PAGE_ALL_PRODUCT);
                        }

                    }

                    var data = db.Product.Where(e => e.LanguageId == lang && e.Status != (int)Const.Status.DELETED).ToList();

                    ViewBag.SeoTitle = ViewBag.CategoryName;
                    ViewBag.SeoDescription = ViewBag.CategoryName;
                    ViewBag.SeoKeywords = ViewBag.CategoryName;

                    int np = 12;
                    ViewBag.np = np;
                    ViewBag.total = (data.Count() - 1);
                    ViewBag.page = p;
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;
                    var list = data.Skip(np * (p - 1)).Take(np).ToList();

                    return View("ListAllProduct", list);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }
        public ActionResult YearTimeline()

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

                if (lang == 1)
                {
                    return Redirect("/" + Const.PAGE_NOMAL_TIMELINE_VN);
                }
                else if (lang == 1007)
                {
                    return Redirect("/" + Const.PAGE_NOMAL_TIMELINE);
                }

            }

            if (lang == 1)
            {
                ViewBag.CategoryName = "Hành trình phát triển";
                ViewBag.SeoTitle = ViewBag.CategoryName;
                ViewBag.SeoDescription = ViewBag.CategoryName;
                ViewBag.SeoKeywords = ViewBag.CategoryName;
            }
            else
            {
                ViewBag.CategoryName = "TECHPRO DAY";
                ViewBag.SeoTitle = ViewBag.CategoryName;
                ViewBag.SeoDescription = ViewBag.CategoryName;
                ViewBag.SeoKeywords = ViewBag.CategoryName;
            }

            using (var db = new IOITDataContext())
            {
                var data = db.News.Where(y => y.Status != 99 && y.LanguageId == lang && y.YearTimeline != null).OrderBy(y => y.YearTimeline).ToList();
                if (data.Count > 0)
                {
                    ViewBag.YearMin = data[0].YearTimeline;
                    ViewBag.ListYear = data;
                }
                ViewBag.imgBaner = db.Category.Where(c => c.CategoryId == 4367).Select(c => c.Image).SingleOrDefault();
                //var idx = (data.Count() - 1);
                //ViewBag.YearMax = data[idx].YearTimeline;
                //ViewBag.ListPostYear = db.News.Where(y => y.Status != (int)Const.Status.DELETED).OrderByDescending(y => y.YearTimeline).Select(y => y.YearTimeline).ToList();
                return View("Timeline");
            }



        }


    }
}