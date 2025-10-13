using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Category
{

    [ViewComponent(Name = "BlockDS_WidgetCate")]
    public class BlockDS_WidgetCateComponent : ViewComponent
    {
        public BlockDS_WidgetCateComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int Id)
        {
            using (var db = new IOITDataContext())
            {
                IEnumerable<Block> data = (from b in db.Block
                                           where b.BlockId == Id
                                                        && b.Status != (int)Const.Status.DELETED
                                           select b).OrderBy(e => e.CreatedAt).ToList();
                return await Task.FromResult((IViewComponentResult)View("BlockDS_WidgetCate", data));
            }
        }

    }
}
