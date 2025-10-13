using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "BlockCart")]
    public class BlockCartComponent : ViewComponent
    {
        public BlockCartComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("BlockCart"));
        }
    }
}
