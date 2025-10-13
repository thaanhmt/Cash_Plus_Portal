using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("web/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("shopping-cart", "shopping-cart");

        [HttpGet("clearCart")]
        public async Task<IActionResult> ClearCart()
        {
            DefaultResponse def = new DefaultResponse();
            using (var db = new IOITDataContext())
            {
                HttpContext.Session.Remove("Cart");
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
        }

    }
}