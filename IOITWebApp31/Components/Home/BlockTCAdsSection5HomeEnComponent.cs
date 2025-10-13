using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Home
{

    [ViewComponent(Name = "BlockTCAdsSection5HomeEn")]
    public class BlockTCAdsSection5HomeEnComponent : ViewComponent
    {
        public BlockTCAdsSection5HomeEnComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Id, int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<Slide> data = (from cm in db.Slide
                                           where cm.TypeSlideId == Id
                                                        && cm.Status != (int)Const.Status.DELETED
                                                        && cm.LanguageId == 1008
                                           select cm).OrderBy(e => e.Location).Take(Number).ToList();
                return await Task.FromResult((IViewComponentResult)View("BlockTCAdsSection5HomeEn", data));
            }
        }

    }
}
