using System;

namespace TCU.English.Utils
{
    public static class ConvertUtils
    {
        public static string ToCamelCase(this string name)
        {
            return Char.ToLowerInvariant(name[0]) + name.Substring(1).Replace("_", string.Empty);
        }
        public static int ToInt(this object number)
        {
            try
            {
                return Convert.ToInt32(number);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public static long ToLong(this object number)
        {
            try
            {
                return Convert.ToInt64(number);
            }
            catch (Exception)
            {
                return -1L;
            }
        }

        public static bool ToBoolean(this object boolean)
        {
            try
            {
                return Convert.ToBoolean(boolean);
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
