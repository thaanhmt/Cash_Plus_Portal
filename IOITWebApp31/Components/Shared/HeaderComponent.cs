using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "Header")]
    public class HeaderComponent : ViewComponent
    {
        public HeaderComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            using (var db = new IOITDataContext())
            {
                //get lang
                ViewBag.LanguageId = Request.Cookies["LanguageId"];

                var danhmucSp = db.Category.Where(c => c.TypeCategoryId == 11 && c.CategoryParentId == 0 && c.Status != (int)Const.Status.DELETED).OrderBy(c => c.CategoryId).ToList();
                ViewBag.danhmucSp = danhmucSp;

                CustomerLogin customerLogin = new CustomerLogin();
                customerLogin.CustomerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
                customerLogin.Email = HttpContext.Session.GetString("CustomerEmail");
                customerLogin.FullName = HttpContext.Session.GetString("CustomerFullName");
                customerLogin.Avata = HttpContext.Session.GetString("CustomerAvata");
                customerLogin.Address = HttpContext.Session.GetString("CustomerAddress");
                customerLogin.Password = HttpContext.Session.GetString("CustomerPassword");
                customerLogin.PhomeNumber = HttpContext.Session.GetString("CustomerPhoneNumber");
                customerLogin.access_token = HttpContext.Session.GetString("access_token");
                customerLogin.Sex = HttpContext.Session.GetInt32("CustomerSex") != null ? (int)HttpContext.Session.GetInt32("CustomerSex") : 1;
                customerLogin.NunberNotification = HttpContext.Session.GetInt32("NunberNotification") != null ? (int)HttpContext.Session.GetInt32("NunberNotification") : 1;

                ViewBag.CustomerId = customerLogin.CustomerId;
                ViewBag.FullName = customerLogin.FullName;

                var data = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.Website = data;
                ViewBag.SeoTitle = data.MetaTitle;
                ViewBag.SeoDescription = data.MetaDescription;
                ViewBag.SeoKeywords = data.MetaKeyword;
                ViewBag.Phone = data.Hotline;
                ViewBag.Email = data.Hotmail;
                ViewBag.Logo = data.LogoHeader;
                ViewBag.BannerHeader = data.Banner;
                ViewBag.BannerFooter = data.LogoFooter;
                ViewBag.Url = data.Url;
                ViewBag.Hotline = data.Hotline;
                ViewBag.Hotmail = data.Hotmail;
                ViewBag.LinkFacebookFanpage = data.Link1;
                //ShoppingCart objCart = Models.Common.SessionExtensions.GetObject<ShoppingCart>(HttpContext.Session, "Cart");
                //if (objCart != null)
                // { ViewBag.TotalItem = objCart.ListItem.Count();
                //}
                //else
                //{
                //    ViewBag.TotalItem = 0;
                //}
                string access_key = HttpContext.Session.GetString("access_key");
                if (access_key != null && access_key != "")
                {
                    ViewBag.IsDatabase = CheckRole.CheckRoleByCode(access_key, "QTDL", (int)Const.Action.VIEW);
                }
                else
                {
                    ViewBag.IsDatabase = false;
                }


                return await Task.FromResult((IViewComponentResult)View("Header", customerLogin));
            }
        }
    }
}
