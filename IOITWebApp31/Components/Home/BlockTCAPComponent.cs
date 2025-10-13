using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockTCAP")]
    public class BlockTCAPComponent : ViewComponent
    {
        public BlockTCAPComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<Publication> data = (from ap in db.Publication
                                                 where
                                                 ap.Status == (int)Const.Status.NORMAL
                                                 select ap).OrderByDescending(e => e.DateStartActive).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockTCAP", data));
            }
        }

    }
}
