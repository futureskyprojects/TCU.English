using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TCU.English.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult NewTest()
        {
            return View();
        }
    }
}
