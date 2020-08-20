using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }
    }
}
