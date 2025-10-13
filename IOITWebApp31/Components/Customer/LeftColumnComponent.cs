using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Customer
{
    [ViewComponent(Name = "LeftColumn")]
    public class LeftColumnComponent : ViewComponent
    {
        public LeftColumnComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            using (var db = new IOITDataContext())
            {
                //check xem menu có hiện ko
                string access_key = HttpContext.Session.GetString("access_key");
                if (access_key != null && access_key != "")
                {
                    ViewBag.CQTC = CheckRole.CheckRoleByCode(access_key, "CQTC", (int)Const.Action.MENU);
                    ViewBag.QLND = CheckRole.CheckRoleByCode(access_key, "QLND", (int)Const.Action.MENU);
                }
                else
                {
                    ViewBag.CQTC = false;
                    ViewBag.QLND = false;
                }
                return await Task.FromResult((IViewComponentResult)View("LeftColumn"));
            }
        }
    }
}
