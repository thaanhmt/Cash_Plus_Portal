using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "BlockProductSuggestions")]
    public class BlockProductSuggestionsComponent : ViewComponent
    {
        public BlockProductSuggestionsComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int CategoryId, int ProductId, int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<Product> data = (from cm in db.CategoryMapping
                                             join p in db.Product on cm.TargetId equals p.ProductId
                                             where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                                             && cm.TargetId != ProductId
                                             && p.CompanyId == Const.COMPANYID && cm.CategoryId == CategoryId
                                             && p.WebsiteId == Const.WEBSITEID
                                             && p.Status == (int)Const.Status.NORMAL
                                             && cm.Status != (int)Const.Status.DELETED
                                             select p).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();

                int customerId = HttpContext.Session.GetInt32("CustomerId") != null ? (int)HttpContext.Session.GetInt32("CustomerId") : -1;
                ViewBag.CustomerId = customerId;
                string token = HttpContext.Session.GetString("access_token");
                ViewBag.Token = "'" + token + "'";

                return await Task.FromResult((IViewComponentResult)View("BlockProductSuggestions", data));
            }
        }
    }
}
