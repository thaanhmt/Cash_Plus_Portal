using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "HoTroFooterMenu")]
    public class HoTroFooterMenuComponent : ViewComponent
    {
        public HoTroFooterMenuComponent()
        {
        }


        public async Task<IViewComponentResult> InvokeAsync(int MenuId, int MenuParentId, List<MenuItems> menuItems)
        {
            using (var db = new IOITDataContext())
            {
                ViewBag.MenuParentId = MenuParentId;
                if (menuItems != null)
                {
                    List<MenuItems> data = (from mi in db.MenuItem
                                            join c in db.Category on mi.CategoryId equals c.CategoryId
                                            where mi.MenuId == MenuId
                                            && c.CompanyId == Const.COMPANYID
                                            && c.WebsiteId == Const.WEBSITEID
                                            && c.Status == (int)Const.Status.NORMAL
                                            && mi.Status != (int)Const.Status.DELETED
                                            select new MenuItems
                                            {
                                                CategoryId = c.CategoryId,
                                                MenuItemId = mi.MenuItemId,
                                                MenuId = (int)mi.MenuId,
                                                MenuParentId = mi.MenuParentId,
                                                Location = mi.Location,
                                                CategoryName = c.Name,
                                                Url = c.Url,
                                                Icon = c.Icon,
                                                TypeCategoryId = c.TypeCategoryId
                                            }).OrderByDescending(e => e.Location).ToList();

                    return await Task.FromResult((IViewComponentResult)View("HoTroFooterMenu", data));
                }
                else
                {
                    return await Task.FromResult((IViewComponentResult)View("HoTroFooterMenu", menuItems));
                }
            }
        }
    }
}
