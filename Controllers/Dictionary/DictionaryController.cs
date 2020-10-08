using Microsoft.AspNetCore.Mvc;

namespace TCU.English.Controllers
{
    public class DictionaryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
