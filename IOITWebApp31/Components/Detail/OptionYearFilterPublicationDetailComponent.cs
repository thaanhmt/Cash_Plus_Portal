using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Detail
{
    [ViewComponent(Name = "OptionYearFilterPublicationDetail")]
    public class OptionYearFilterPublicationDetailComponent : ViewComponent
    {
        public OptionYearFilterPublicationDetailComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<Publication> data = (from c in db.Publication
                                                 where c.Status == 1
                                                 select c).GroupBy(e => e.PublishingYear).Select(e => new PublicationDTO
                                                 {
                                                     PublishingYear = e.Key,
                                                 }).OrderByDescending(p => p.PublishingYear).ToList();
                return await Task.FromResult((IViewComponentResult)View("OptionYearFilterPublicationDetail", data));
            }
        }

    }
}
