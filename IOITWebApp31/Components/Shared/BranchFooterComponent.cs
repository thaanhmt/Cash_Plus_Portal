using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "BranchFooter")]
    public class BranchFooterComponent : ViewComponent
    {
        public BranchFooterComponent()
        {
        }
        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                int languageId = Request.Cookies["LanguageId"] != null ? int.Parse(Request.Cookies["LanguageId"]) : 1;
                IEnumerable<Branch> data = db.Branch.Where(a => a.LanguageId == languageId && a.Status == (int)Const.Status.NORMAL).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BranchFooter", data));
            }
        }
    }
}
