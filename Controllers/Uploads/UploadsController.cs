using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace TCU.English.Controllers
{
    [Authorize]
    public partial class UploadsController : Controller
    {
        private readonly IHostEnvironment host;
        public UploadsController(IHostEnvironment _host)
        {
            host = _host;
        }

        [HttpGet("[controller]/{prefix}")]
        public IActionResult Index()
        {
            return NotFound();
        }
    }
}
