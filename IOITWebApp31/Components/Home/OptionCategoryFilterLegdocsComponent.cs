using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "OptionCategoryFilterLegdocs")]
    public class OptionCategoryFilterLegdocsComponent : ViewComponent
    {
        public OptionCategoryFilterLegdocsComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<Category> data = (from c in db.Category
                                              where c.Status == 1
                                              && c.TypeCategoryId == 12
                                              select c).OrderByDescending(e => e.Name).ToList();
                return await Task.FromResult((IViewComponentResult)View("OptionCategoryFilterLegdocs", data));
            }
        }

    }
}
