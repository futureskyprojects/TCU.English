using System;

namespace TCU.English
{
    public static class Config
    {
        public static string ProjectName = "TCU English";
        public static string CompanyName = "Vistark Inc.";
        public static string FromYear = "2016";
        public static string ToYear = DateTime.Now.Year.ToString();
        public static string ProjectAuthor = "services@vistark@gmail.com";
        public static string ProjectPortfolioAddress = "https://fb.com/tx.trongnghia98";

        public static long MAX_IMAGE_SIZE = 5 * 1024 * 1024;

        public static int MAX_READING_PART_1_QUESTION = 4;
        public static int MAX_READING_PART_2_QUESTION = 3;
    }
}
