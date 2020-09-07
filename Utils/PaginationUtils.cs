using System;
namespace TCU.English.Utils
{
    public static class PaginationUtils
    {
        public static int TotalPageCount(int total, int limit)
        {
            if (limit == 0) limit = 1;
            float numberpage = (float)total / limit;
            int pageCount = (int)Math.Ceiling(numberpage);
            return pageCount > 0 ? pageCount : 1;
        }
    }
}
