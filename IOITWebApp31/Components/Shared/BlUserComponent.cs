using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOITWebApp31.Components.Shared
{
    [ViewComponent(Name = "BlUser")]
    public class BlUserComponent : ViewComponent
    {
        public BlUserComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int IndexRouter)
        {
            ViewBag.IndexRouter = IndexRouter;

            return await Task.FromResult((IViewComponentResult)View("BlUser"));
        }
    }
}
