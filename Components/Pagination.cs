using Microsoft.AspNetCore.Mvc;

namespace TCU.English.Components
{
    public class Pagination : ViewComponent
    {
        public IViewComponentResult Invoke(Models.Pagination pagination)
        {
            return View(pagination);
        }
    }
}
