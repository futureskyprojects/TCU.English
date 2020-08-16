using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Components
{
    [ViewComponent]
    public class HeaderMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            //return View($"/Views/Shared/{Utils.NameUtils.ViewComponentName<HeaderMenuViewComponent>()}/Default.cshtml");
            //return View($"~/Views/Shared/{Utils.NameUtils.ViewComponentName<HeaderMenuViewComponent>()}/Default.cshtml");
            return View();
        }
    }
}
