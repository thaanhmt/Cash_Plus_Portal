using IOITWebApp31.Models;
using IOITWebApp31.Models.Common;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "Footer")]
    public class FooterComponent : ViewComponent
    {
        public FooterComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int CategoryId, int Number)
        {
            using (var db = new IOITDataContext())
            {
                // lay sesiion
                ShoppingCart objCart = Models.Common.SessionExtensions.GetObject<ShoppingCart>(HttpContext.Session, "Cart");
                if (objCart == null)
                {
                    objCart = new ShoppingCart();
                }
                ViewBag.TotalItem = objCart.ListItem.Count();
                //
                var DataConfig = db.Config.Where(c => c.Status != (int)Const.Status.DELETED && c.ConfigId == 1).FirstOrDefault();

                ViewBag.FooterScript = DataConfig.FooterScript;

                ViewBag.LanguageId = Request.Cookies["LanguageId"];
                var data = db.Website.Where(e => e.WebsiteId == Const.WEBSITEID).FirstOrDefault();
                ViewBag.Website = data.Url;
                ViewBag.SeoTitle = data.MetaTitle;
                ViewBag.SeoDescription = data.MetaDescription;
                ViewBag.SeoKeywords = data.MetaKeyword;
                ViewBag.Phone = data.Hotline;
                ViewBag.Email = data.Hotmail;
                ViewBag.Logo = data.LogoHeader;
                ViewBag.BannerHeader = data.Banner;
                ViewBag.BannerFooter = data.LogoFooter;
                ViewBag.Hotline = data.Hotline;
                ViewBag.Hotmail = data.Hotmail;
                ViewBag.Address = data.Address;
                ViewBag.AddressLink = data.LinkOther1;
                ViewBag.UnitName = data.UnitName;
                return await Task.FromResult((IViewComponentResult)View("Footer", data));
            }
        }
    }
}

