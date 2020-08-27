using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Controllers;
using TCU.English.Models;

namespace TCU.English.Utils
{
    public static class FileUploadControlUtils
    {
        public static string GetContentPathRootForUploadUtils(this IHostEnvironment _host)
        {
            return Path.GetDirectoryName(_host.ContentRootPath);
        }
        public static async Task<string> UploadForUserMedia(this IHostEnvironment _host, IFormFile file, User user)
        {
            string res = await Upload(_host, file, user.Username.ToLower());
            if (res != null && res.Length > 0)
            {
                // Xóa tệp ảnh cũ nếu có
                if (user.Avatar != null && user.Avatar.Length > 0)
                {
                    var oldFile = Path.Combine(_host.GetContentPathRootForUploadUtils(), user.Avatar);
                    if (File.Exists(oldFile))
                    {
                        File.Delete(oldFile);
                    }
                }
                return res;
            }
            else
            {
                return "";
            }
        }
        public static async Task<string> UploadForTestMedia(this IHostEnvironment _host, IFormFile file, string testType, int partId)
        {
            return await Upload(_host, file, testType.ToLower(), $"PART_{partId}".ToLower());
        }
        public static async Task<string> Upload(this IHostEnvironment _host, IFormFile file, params string[] subFolders)
        {
            if (file != null && file.Length > 0 && file.Length <= Config.MAX_IMAGE_SIZE)
            {
                if (MimeTypeUtils.Image.CheckContentType(file.ContentType) && MimeTypeUtils.Image.CheckFileExtension(file.FileName))
                {
                    // Upload avatar
                    try
                    {
                        var uniqueFileName = NameUtils.GetUniqueFileName(file.FileName);
                        var uploads = Path.Combine(_host.GetContentPathRootForUploadUtils(), NameUtils.ControllerName<UploadsController>().ToLower());
                        foreach (string subFolder in subFolders)
                        {
                            uploads = Path.Combine(uploads, subFolder);
                        }
                        // Kiểm tra xem folder có tồn tại không? Nếu không thì tạo mới
                        if (!Directory.Exists(uploads))
                            Directory.CreateDirectory(uploads);
                        var filePath = Path.Combine(uploads, uniqueFileName);

                        using (var stream = File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }
                        return filePath.Replace(_host.GetContentPathRootForUploadUtils(), "");
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return "";
        }
    }
}
