using System;

namespace TCU.English.Utils
{
    public static class ConvertUtils
    {
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
