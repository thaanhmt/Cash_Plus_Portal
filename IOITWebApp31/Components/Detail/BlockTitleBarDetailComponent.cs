using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockTitleBarDetail")]
    public class BlockTitleBarDetailComponent : ViewComponent
    {
        public BlockTitleBarDetailComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int CategoryId, Boolean IsDanger, Boolean IsCarouse)
        {
            using (var db = new IOITDataContext())
            {
                var GetParent = db.Category.Where(cc => cc.CategoryId == CategoryId && cc.Status != (int)Const.Status.DELETED).FirstOrDefault();
                var ParentId = GetParent.CategoryParentId;
                if (ParentId == 0)
                {
                    IEnumerable<Category> data = (from c in db.Category
                                                  where c.CategoryParentId == CategoryId
                                                  && c.Status != (int)Const.Status.DELETED
                                                  select c).OrderByDescending(e => e.CreatedAt).ToList();
                    var dataParent = db.Category.Where(a => a.CategoryId == CategoryId && a.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    ViewBag.TitleParent = dataParent.Name;
                    if (IsDanger == true)
                    {
                        ViewBag.Class = "dangerLabel";
                    }
                    if (IsCarouse == true)
                    {
                        ViewBag.Carouse = "carouseTitleBar";
                    }
                    return await Task.FromResult((IViewComponentResult)View("BlockTitleBarDetail", data));
                }
                else
                {
                    IEnumerable<Category> data = (from c in db.Category
                                                  where c.CategoryParentId == ParentId
                                                  && c.Status != (int)Const.Status.DELETED
                                                  select c).OrderByDescending(e => e.CreatedAt).ToList();
                    var dataParent = db.Category.Where(a => a.CategoryId == ParentId && a.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    ViewBag.TitleParent = dataParent.Name;
                    if (IsDanger == true)
                    {
                        ViewBag.Class = "dangerLabel";
                    }
                    if (IsCarouse == true)
                    {
                        ViewBag.Carouse = "carouseTitleBar";
                    }
                    return await Task.FromResult((IViewComponentResult)View("BlockTitleBarDetail", data));
                }
            }
        }

    }
}
