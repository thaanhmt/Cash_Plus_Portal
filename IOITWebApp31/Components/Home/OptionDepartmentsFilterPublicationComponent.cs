using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "OptionDepartmentsFilterPublication")]
    public class OptionDepartmentsFilterPublicationComponent : ViewComponent
    {
        public OptionDepartmentsFilterPublicationComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<TypeAttributeItem> data = (from c in db.TypeAttributeItem
                                                       where c.Status == 1
                                                       && c.TypeAttributeId == 26
                                                       select c).OrderByDescending(e => e.Name).ToList();
                return await Task.FromResult((IViewComponentResult)View("OptionDepartmentsFilterPublication", data));
            }
        }

    }
}
