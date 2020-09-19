using System;
using TCU.English.Utils;

namespace TCU.English
{
    public static class Config
    {
        public static string ProjectName = "TCU English";
        public static string CompanyName = "Vistark Inc.";
        public static string FromYear = "2020";
        public static string ToYear = DateTime.Now.Year.ToString();
        public static string ProjectAuthor = "services@vistark@gmail.com";
        public static string ProjectPortfolioAddress = "https://fb.com/tx.trongnghia98";

        /// <summary>
        /// Thời gian sống của cookie và phiên đăng nhập
        /// </summary>
        public static long MAX_COOKIE_LIFE_MINUTES = 1 * 365 * 24 * 60;

        public static long MAX_IMAGE_SIZE = 5 * 1024 * 1024;

        public static int MAX_READING_PART_1_QUESTION = 4;
        public static int MAX_READING_PART_2_QUESTION = 3;
        public static int MAX_READING_PART_3_QUESTION = 5;
        public static int MAX_READING_PART_4_QUESTION = 10;

        public static int MAX_LISTENING_PART_1_QUESTION = 1;
        public static int MAX_LISTENING_PART_2_QUESTION = 6;

        public static int SPEAKING_EMBED_PART_ID = 999;

        public static int MAX_WRITING_PART_1_QUESTION = 1;

        public static float THRESHOLD_POINT = 5;
        public static float MAX_SCORE_POINT = 10;

        public static string ToScores(this object scores)
        {
            float theScores = scores.ToString().ToFloat();
            if (theScores.ToInt() == theScores)
            {
                return theScores.ToInt().ToString();
            }
            else
            {
                return theScores.ToString("0.0");
            }
        }
    }
}
