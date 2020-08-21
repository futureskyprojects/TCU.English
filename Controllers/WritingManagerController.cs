using Microsoft.AspNetCore.Mvc;

namespace TCU.English.Controllers
{
    public class WritingManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
