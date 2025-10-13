using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockTitleBar")]
    public class BlockTitleBarComponent : ViewComponent
    {
        public BlockTitleBarComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int CategoryId, Boolean IsBlue, Boolean IsCarouse, Boolean HideLine)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<Category> data = (from c in db.Category
                                              where c.CategoryParentId == CategoryId
                                              && c.Status != (int)Const.Status.DELETED
                                              select c).OrderByDescending(e => e.CreatedAt).ToList();
                var dataParent = db.Category.Where(a => a.CategoryId == CategoryId && a.Status != (int)Const.Status.DELETED).FirstOrDefault();
                ViewBag.TitleParent = dataParent.Name.Trim();
                if (IsBlue == true)
                {
                    ViewBag.Class = "blue-color";
                }
                if (IsCarouse == true)
                {
                    ViewBag.Carouse = "carouseTitleBar";
                }
                if (HideLine == true)
                {
                    ViewBag.HideLine = "hide-line";
                }
                return await Task.FromResult((IViewComponentResult)View("BlockTitleBar", data));
            }
        }

    }
}
