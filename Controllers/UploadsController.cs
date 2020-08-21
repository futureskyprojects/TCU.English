using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [Authorize]
    public class UploadsController : Controller
    {
        private readonly IHostEnvironment host;
        private readonly string PATH_ROOT;
        public UploadsController(IHostEnvironment _host)
        {
            host = _host;
            this.PATH_ROOT = Path.GetDirectoryName(host.ContentRootPath);
        }

        [HttpGet("[controller]/{username}/{filename}")]
        public IActionResult Index(string username, string filename)
        {
            try
            {
                var uploads = Path.Combine(PATH_ROOT, NameUtils.ControllerName<UploadsController>().ToLower(), username, filename);

                if (System.IO.File.Exists(uploads))
                {
                    return File(System.IO.File.ReadAllBytes(uploads), "application/octet-stream");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
