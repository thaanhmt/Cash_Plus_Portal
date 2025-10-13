using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace IOITWebApp31.Controllers
{
    public class TagController : Controller
    {
        public ActionResult News(string seoName, int p = 1)
        {

            //Session["current_url"] = Request.Url.AbsoluteUri;
            try
            {
                using (var db = new IOITDataContext())
                {
                    if (p < 1)
                    {
                        return Redirect("/Home/Error");
                    }

                    int pageSize = 12;

                    //var tag = db.Tags.First(e => e.Url.Trim() == seoName.Trim() 
                    //    && e.TargetType == (int)Const.TypeTag.TAG_NEWS 
                    //    && e.CompanyId == Const.COMPANYID
                    //    && e.WebsiteId == Const.WEBSITEID
                    //    && e.Status!=(int)Const.Status.DELETED);

                    //if (tag == null)
                    //    return Redirect("/Home/Error");

                    var data = (from t in db.Tag
                                join n in db.News on t.TargetId equals n.NewsId
                                where t.TargetType == (int)Const.TypeTag.TAG_NEWS
                                && n.CompanyId == Const.COMPANYID && t.Url.Trim() == seoName.Trim()
                                && n.WebsiteId == Const.WEBSITEID
                                && n.Status == (int)Const.Status.NORMAL
                                && t.Status != (int)Const.Status.DELETED
                                select n).OrderByDescending(e => e.CreatedAt).ToList();

                    if (data == null)
                        return Redirect("/Home/Error");

                    if (((data.Count() - 1) / pageSize) + 1 < p)
                    {
                        return Redirect("/Home/Error");
                    }

                    if (p == 1)
                    {
                        ViewBag.SeoTitle = data.FirstOrDefault().Title;
                        ViewBag.SeoDescription = data.FirstOrDefault().Title;
                        ViewBag.SeoKeywords = data.FirstOrDefault().Title;
                    }
                    else
                    {
                        ViewBag.SeoTitle = data.FirstOrDefault().Title + " " + p;
                        ViewBag.SeoDescription = data.FirstOrDefault().Title + " " + p;
                        ViewBag.SeoKeywords = data.FirstOrDefault().Title;
                    }

                    ViewBag.Name = data.FirstOrDefault().Title;
                    ViewBag.Page = p;

                    ViewBag.CountAll = (data.Count() - 1).ToString().Trim();
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;
                    var list = data.Skip(pageSize * (p - 1)).Take(pageSize).ToList();

                    return View("TagNews", list);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Product(string seoName, int p = 1)
        {

            //Session["current_url"] = Request.Url.AbsoluteUri;
            try
            {
                using (var db = new IOITDataContext())
                {
                    if (p < 1)
                    {
                        return Redirect("/Home/Error");
                    }

                    int pageSize = 12;

                    //var tag = db.Tags.First(e => e.Url.Trim() == seoName.Trim() && e.Status != (int)Const.Status.DELETED);

                    //if (tag == null)
                    //    return Redirect("/Home/Error");

                    var data = (from t in db.Tag
                                join n in db.News on t.TargetId equals n.NewsId
                                where t.TargetType == (int)Const.TypeTag.TAG_NEWS
                                && n.CompanyId == Const.COMPANYID && t.Url.Trim() == seoName.Trim()
                                && n.WebsiteId == Const.WEBSITEID
                                && n.Status == (int)Const.Status.NORMAL
                                && t.Status != (int)Const.Status.DELETED
                                select n).OrderByDescending(e => e.CreatedAt).ToList();

                    if (data == null)
                        return Redirect("/Home/Error");

                    if (((data.Count() - 1) / pageSize) + 1 < p)
                    {
                        return Redirect("/Home/Error");
                    }

                    if (p == 1)
                    {
                        ViewBag.SeoTitle = data.FirstOrDefault().Title;
                        ViewBag.SeoDescription = data.FirstOrDefault().Title;
                        ViewBag.SeoKeywords = data.FirstOrDefault().Title;
                    }
                    else
                    {
                        ViewBag.SeoTitle = data.FirstOrDefault().Title + " " + p;
                        ViewBag.SeoDescription = data.FirstOrDefault().Title + " " + p;
                        ViewBag.SeoKeywords = data.FirstOrDefault().Title;
                    }

                    ViewBag.Name = data.FirstOrDefault().Title;
                    ViewBag.Page = p;

                    ViewBag.CountAll = (data.Count() - 1).ToString().Trim();
                    ViewBag.Pre = p - 1;
                    ViewBag.Next = p + 1;
                    var list = data.Skip(pageSize * (p - 1)).Take(pageSize).ToList();

                    return View("TagProduct", list);
                }
            }
            catch
            {
                return Redirect("/Home/Error");
            }
        }

    }
}