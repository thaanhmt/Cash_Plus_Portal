using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Detail
{
    [ViewComponent(Name = "BlockNameAgencyDetail")]
    public class BlockNameAgencyDetailComponent : ViewComponent
    {
        public BlockNameAgencyDetailComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int AgencyIssued)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<TypeAttributeItem> data = (from at in db.TypeAttributeItem
                                                       where at.Status != (int)Const.Status.DELETED
                                                       && at.TypeAttributeItemId == AgencyIssued
                                                       select at).ToList();

                return await Task.FromResult((IViewComponentResult)View("BlockNameAgencyDetail", data));
            }
        }

    }
}
