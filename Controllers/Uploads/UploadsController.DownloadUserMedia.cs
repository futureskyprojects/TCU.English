using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    public partial class UploadsController
    {
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

    }
}
