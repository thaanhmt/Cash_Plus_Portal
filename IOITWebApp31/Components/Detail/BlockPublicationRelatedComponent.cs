using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockPublicationRelated")]
    public class BlockPublicationRelatedComponent : ViewComponent
    {
        public BlockPublicationRelatedComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number, int Id, string Language)
        {
            using (var db = new IOITDataContext())
            {
                ViewBag.Language = Language;
                IEnumerable<Publication> data = (from n in db.Publication
                                                 where n.Status == (int)Const.Status.NORMAL
                                                 && n.PublicationId != Id
                                                 select n).OrderByDescending(e => e.DateStartActive).ThenByDescending(e => e.CreatedAt).Take(Number).ToList();
                return await Task.FromResult((IViewComponentResult)View("BlockPublicationRelated", data));
            }
        }

    }
}
