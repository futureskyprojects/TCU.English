using System;

namespace TCU.English.Utils
{
    public static class NumberUtils
    {
        public static int ToInt(this long number)
        {
            return Convert.ToInt32(number);
        }

    }
}
