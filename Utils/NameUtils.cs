using System;
using System.Collections.Generic;
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
    }
}
