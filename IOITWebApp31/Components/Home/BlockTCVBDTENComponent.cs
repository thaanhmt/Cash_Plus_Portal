using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Home.Components
{
    [ViewComponent(Name = "BlockTCVBDTEN")]
    public class BlockTCVBDTENComponent : ViewComponent
    {
        public BlockTCVBDTENComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<LegalDoc> data = (from lg in db.LegalDoc
                                              where lg.Status == 1
                                              && lg.LanguageId == 1008
                                              select lg).OrderByDescending(e => e.CreatedAt).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockTCVBDTEN", data));
            }
        }

    }
}
