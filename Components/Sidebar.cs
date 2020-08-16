using Microsoft.AspNetCore.Mvc;

namespace TCU.English.Components
{
    public class Sidebar : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
