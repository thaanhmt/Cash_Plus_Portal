using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Detail
{
    [ViewComponent(Name = "OptionAuthorFilterPublicationDetail")]
    public class OptionAuthorFilterPublicationDetailComponent : ViewComponent
    {
        public OptionAuthorFilterPublicationDetailComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<Author> data = (from c in db.Author
                                            where c.Status == 1
                                            select c).OrderBy(e => e.Name).ToList();
                return await Task.FromResult((IViewComponentResult)View("OptionAuthorFilterPublicationDetail", data));
            }
        }

    }
}
