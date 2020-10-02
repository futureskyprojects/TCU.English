using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using TCU.English.Controllers;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Components
{
    [AuthorizeRoles(UserType.ROLE_ALL, UserType.ROLE_MANAGER_LIBRARY)]
    public class VideoLibraryTable : ViewComponent
    {
        // Danh sách các đuôi video
        public static List<string> videoExtensions = new List<string> {
            ".webm",
            ".mkv",
            ".flv",
            ".vob",
            ".ogv",
            ".ogg",
            ".drc",
            ".gif",
            ".gifv",
            ".mng",
            ".avi",
            ".MTS",
            ".M2TS",
            ".TS",
            ".mov",
            ".qt",
            ".wmv",
            ".yuv",
            ".rm",
            ".rmvb",
            ".viv",
            ".asf",
            ".amv",
            ".mp4",
            ".m4p ",
            ".m4v",
            ".mpg",
            ".mp2",
            ".mpeg",
            ".mpe",
            ".mpv",
            ".mpg",
            ".mpeg",
            ".m2v",
            ".m4v",
            ".svi",
            ".3gp",
            ".3g2",
            ".mxf",
            ".roq",
            ".nsv",
            ".flv",
            ".f4v",
            ".f4p",
            ".f4a",
            ".f4b"};

        // Khởi tạo đối tượng video được lưu trữ trong thư viện
        public class LibraryVideo
        {
            public int Id { get; set; }
            public string FileName { get; set; }
            public string Name { get; set; }
            public string DownloadPath { get; set; }
        }

        private readonly IHostEnvironment host;

        public VideoLibraryTable(IHostEnvironment _host)
        {
            host = _host;
        }

        // Hàm tạo component
        public IViewComponentResult Invoke()
        {
            string root = host.GetContentPathRootForUploadUtils();

            // Tạo thư mục mặc định cho speaking_embed
            var speakingEmbedFolder = Path.Combine(root, UploadsController.MANAGER_ROOT_DIRECTORY, UploadsController.VIDEOS_DIRECTORY);

            // Nếu thư mục chưa tồn tại, thì trả về trống
            if (!Directory.Exists(speakingEmbedFolder))
                return View(new List<LibraryVideo>());

            return View(DirSearch(speakingEmbedFolder));
        }

        // Hàm tìm kiếm danh sách video khả dụng
        private List<LibraryVideo> DirSearch(string sDir)
        {
            int i = 1;
            // Khai báo biến lưu trữ video sẽ lấy được
            List<LibraryVideo> files = new List<LibraryVideo>();
            try
            {
                // Lặp để tìm trong các  tệp tin của thư mục
                foreach (string f in Directory.GetFiles(sDir))
                {
                    // Nếu tệp có phần mở rộng là đuôi của một video nằm trên danh sách trên
                    if (!string.IsNullOrEmpty(videoExtensions.Find(x => x.Trim().ToLower().Equals(Path.GetExtension(f).ToLower()))))
                        files.Add(new LibraryVideo
                        {
                            Id = i++,
                            FileName = Path.GetFileName(f),
                            Name = Path.GetFileNameWithoutExtension(f),
                            DownloadPath = $"/video-downloader/{Path.GetFileName(f)}"
                        });
                }

                // Không cần đệ quy

                //foreach (string d in Directory.GetDirectories(sDir))
                //{
                //    files.AddRange(DirSearch(d));
                //}
            }
            catch (Exception)
            {
            }

            // Trả về danh sách video
            return files;
        }
    }
}
