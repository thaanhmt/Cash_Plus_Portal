using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp31.Controllers
{
    public class AppsettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
