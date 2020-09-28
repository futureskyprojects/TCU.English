using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using TCU.English.Models;
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

        // Refs https://docs.gleamtech.com/fileultimate/html/using-fileultimate-in-an-asp-net-core-project.htm
        [HttpGet("file-manager")]
        [AuthorizeRoles(UserType.ROLE_ALL, UserType.ROLE_MANAGER_LIBRARY)]
        public IActionResult FileManager()
        {
            string root = host.GetContentPathRootForUploadUtils();

            // Tránh băm source
            root = Path.Combine(root,"FileManagerByVistark");

            // Nếu không thể tạo được thư mục root
            if (!Directory.Exists(root) && !Directory.CreateDirectory(root).Exists)
            {
                this.NotifyError("Can not create or access to root file manager");
                return Redirect("/");
            }

            // Gán giá trị đường dẫn mặc định
            ViewBag.RootPath = root;

            // Nếu được thì trả về view
            return View();
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
