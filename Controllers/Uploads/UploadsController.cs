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
        public UploadsController(IHostEnvironment _host)
        {
            host = _host;
        }

        [HttpGet("[controller]/{prefix}")]
        public IActionResult Index()
        {
            var s = Request.Path;
            return NotFound();
        }

        [HttpGet("[controller]/{username}/{type}/{filename}")]
        public IActionResult DownloadUserMedia(string username, string type, string filename)
        {
            try
            {
                var uploads = Path.Combine(host.GetContentPathRootForUploadUtils(), NameUtils.ControllerName<UploadsController>().ToLower(), username, type, filename);

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
        [HttpGet("[controller]/{testType}/{part}/{type}/{filename}")]
        public IActionResult DownloadTestMedia(string testType, string part, string type, string filename)
        {
            try
            {
                var uploads = Path.Combine(host.GetContentPathRootForUploadUtils(), NameUtils.ControllerName<UploadsController>().ToLower(), testType, part, type, filename);

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
