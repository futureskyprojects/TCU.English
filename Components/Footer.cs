using Microsoft.AspNetCore.Mvc;

namespace TCU.English.Components
{
    public class Footer : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
