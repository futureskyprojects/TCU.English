using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TCU.English.Controllers
{
    public class ListeningManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
