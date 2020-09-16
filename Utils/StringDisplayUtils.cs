using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Utils
{
    public static class StringDisplayUtils
    {
        public static string TrimMax(this string str, int max = 30)
        {
            if (str != null && str.Length > max)
                return $"{str.Substring(max)}...";
            return str;
        }
    }
}
