using Microsoft.AspNetCore.Mvc;

namespace TCU.English.Components
{
    public class Header : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
