using System.IO;
using Microsoft.AspNetCore.Mvc;
using TCU.English.Models;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    public partial class UploadsController
    {
        public static string MANAGER_ROOT_DIRECTORY = "FileManagerByVistark";
        public static string VIDEOS_DIRECTORY = "VideoLibrary";

        // Refs https://docs.gleamtech.com/fileultimate/html/using-fileultimate-in-an-asp-net-core-project.htm
        [HttpGet("file-manager")]
        [AuthorizeRoles(UserType.ROLE_ALL, UserType.ROLE_MANAGER_LIBRARY)]
        public IActionResult FileManager()
        {
            string root = host.GetContentPathRootForUploadUtils();

            // Tránh băm source
            root = Path.Combine(root, MANAGER_ROOT_DIRECTORY);

            // Nếu không thể tạo được thư mục root
            if (!Directory.Exists(root) && !Directory.CreateDirectory(root).Exists)
            {
                this.NotifyError("Can not create or access to root file manager");
                return Redirect("/");
            }

            // Gán giá trị đường dẫn mặc định
            ViewBag.RootPath = root;

            // Tạo thư mục mặc định cho speaking_embed
            var speakingEmbedFolder = Path.Combine(root, VIDEOS_DIRECTORY);

            // Tạo sẵn thư mục cho phần này
            if (!Directory.Exists(speakingEmbedFolder) && Directory.CreateDirectory(speakingEmbedFolder).Exists)
                this.NotifySuccess("Initialize folder for store Speaking Video success!");

            // Nếu được thì trả về view
            return View();
        }
    }
}
