using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Home
{

    [ViewComponent(Name = "BlockTCLinks")]
    public class BlockTCLinksComponent : ViewComponent
    {
        public BlockTCLinksComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<BackLink> data = (from cm in db.BackLink
                                              where cm.Status != (int)Const.Status.DELETED
                                              select cm).OrderBy(e => e.CreatedAt).ToList();
                return await Task.FromResult((IViewComponentResult)View("BlockTCLinks", data));
            }
        }

    }
}
