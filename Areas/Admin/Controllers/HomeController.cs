using Microsoft.AspNetCore.Mvc;
using TCU.English.Models;
using TCU.English.Utils;

namespace TCU.English.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
