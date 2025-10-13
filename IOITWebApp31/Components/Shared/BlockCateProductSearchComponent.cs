using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "BlockCateProductSearch")]
    public class BlockCateProductSearchComponent : ViewComponent
    {
        public BlockCateProductSearchComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int TypeCategoryId, int CategoryParentId, List<IOITWebApp31.Models.EF.Category> cates, int Level)
        {
            using (var db = new IOITDataContext())
            {
                ViewBag.CategoryParentId = CategoryParentId;
                ViewBag.TypeCategoryId = TypeCategoryId;
                ViewBag.Level = Level;

                if (CategoryParentId == 0)
                {
                    List<IOITWebApp31.Models.EF.Category> data = db.Category.Where(c => c.TypeCategoryId == TypeCategoryId && c.Status != (int)Const.Status.DELETED).ToList();

                    return await Task.FromResult((IViewComponentResult)View("BlockCateProductSearch", data));
                }
                else
                {
                    return await Task.FromResult((IViewComponentResult)View("BlockCateProductSearch", cates));
                }
            }
        }
    }
}
