using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Detail
{
    [ViewComponent(Name = "OptionDepartmentsFilterLegdocsDetail")]
    public class OptionDepartmentsFilterLegdocsDetailComponent : ViewComponent
    {
        public OptionDepartmentsFilterLegdocsDetailComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<TypeAttributeItem> data = (from c in db.TypeAttributeItem
                                                       where c.Status == 1
                                                       && c.TypeAttributeId == 4
                                                       select c).OrderByDescending(e => e.Name).ToList();
                return await Task.FromResult((IViewComponentResult)View("OptionDepartmentsFilterLegdocsDetail", data));
            }
        }

    }
}
