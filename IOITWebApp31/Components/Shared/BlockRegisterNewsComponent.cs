using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "BlockRegisterNews")]
    public class BlockRegisterNewsComponent : ViewComponent
    {
        public BlockRegisterNewsComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("BlockRegisterNews"));
        }
    }
}
