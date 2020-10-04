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

        public static long MAX_IMAGE_SIZE = 10 * 1024 * 1024;

        public static int MAX_READING_PART_1_QUESTION = 4;
        public static int MAX_READING_PART_2_QUESTION = 3;
        public static int MAX_READING_PART_3_QUESTION = 5;
        public static int MAX_READING_PART_4_QUESTION = 10;

        public static int MAX_LISTENING_PART_1_QUESTION = 1;
        public static int MAX_LISTENING_PART_2_QUESTION = 6;

        public static int SPEAKING_EMBED_PART_ID = 999;

        public static int MAX_WRITING_PART_1_QUESTION = 1;

        // Cấu hình phân trang
        public static int PAGE_PAGINATION_LIMIT = 20;

        // Phần cấu hình số điểm tổng cho mỗi phần
        public static float SCORES_FULL_LISTENING_PART_1 = 14;
        public static float SCORES_FULL_LISTENING_PART_2 = 6;

        public static float SCORES_FULL_READING_PART_1 = 10;
        public static float SCORES_FULL_READING_PART_2 = 5;
        public static float SCORES_FULL_READING_PART_3 = 5;
        public static float SCORES_FULL_READING_PART_4 = 10;

        public static float SCORES_FULL_WRITING_PART_1 = 10;
        public static float SCORES_FULL_WRITING_PART_2 = 20;

        public static float SCORES_FULL_SPEAKING = 20;
        // kết thúc phần cấu hình số điểm tổng cho mỗi phần

        // Cấu hình thời gian cho mỗi phần (Phút)
        public static float TIME_FOR_LISTENING = 20;

        public static float TIME_FOR_READING = 45;

        public static float TIME_FOR_WRITING = 45;

        public static float TIME_FOR_SPEAKING = 10;
        // End Cấu hình thời gian cho mỗi phần

        public static string ToScores(this object scores)
        {
            float theScores = scores.ToString().ToFloat();

            if (theScores == -1)
                return "Not yet";

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
