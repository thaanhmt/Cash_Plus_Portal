using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "BlockLeftCommon")]
    public class BlockLeftCommonComponent : ViewComponent
    {
        public BlockLeftCommonComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("BlockLeftCommon"));
        }
    }
}
