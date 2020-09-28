using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using TCU.English.Models;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    public partial class UploadsController
    {
        [HttpGet("video-downloader/{filename}")]
        [Authorize]
        public IActionResult DownloadVideo(string filename)
        {
            try
            {
                var uploads = Path.Combine(host.GetContentPathRootForUploadUtils(), MANAGER_ROOT_DIRECTORY, VIDEOS_DIRECTORY, filename);

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
