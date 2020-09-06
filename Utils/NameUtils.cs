using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TCU.English.Utils
{
    public static class NameUtils
    {
        public static string ControllerName<T>()
        {
            return typeof(T).Name.Replace("Controller", "");
        }
        public static string ViewComponentName<T>()
        {
            return typeof(T).Name.Replace("ViewComponent", "");
        }

        public static string GetUniqueFileName(string fileName)
        {
            using var sha256 = new SHA256Managed();
            fileName = Path.GetFileName(fileName);
            string fileNameWithoutExtension = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(Path.GetFileNameWithoutExtension(fileName)))).Replace("-", "");
            return fileNameWithoutExtension
              + "_"
              + Guid.NewGuid().ToString().Substring(0, 4)
              + Path.GetExtension(fileName);
        }
    }
}
