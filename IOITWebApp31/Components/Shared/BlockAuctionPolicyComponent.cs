using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "BlockAuctionPolicy")]
    public class BlockAuctionPolicyComponent : ViewComponent
    {
        public BlockAuctionPolicyComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("BlockAuctionPolicy"));
        }
    }
}
