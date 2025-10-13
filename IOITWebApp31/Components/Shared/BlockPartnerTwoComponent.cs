using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "BlockPartnerTwo")]
    public class BlockPartnerTwoComponent : ViewComponent
    {
        public BlockPartnerTwoComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int PartnerId, int Number)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<Slide> data = data = (from sl in db.Slide
                                                  where sl.TypeSlideId == PartnerId
                                                  && sl.CompanyId == Const.COMPANYID
                                                  && sl.WebsiteId == Const.WEBSITEID
                                                  && sl.Status == (int)Const.Status.NORMAL
                                                  select sl).OrderByDescending(e => e.Location).Take(Number).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockPartnerTwo", data));
            }
        }
    }
}
