using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models;

namespace TCU.English.Utils
{
    public static class ScoresUtils
    {
        public static float ScoresCalculate(int correctNum, int totalNum, float maxScores)
        {
            return Math.Ceiling(((float)correctNum / totalNum) * maxScores).ToFloat();
        }

        public static float GetMaxScores(string typeCode)
        {
            float maxScores;
            if (typeCode.Equals(TestCategory.READING))
            {
                maxScores = Config.SCORES_FULL_READING_PART_1 + Config.SCORES_FULL_READING_PART_2 + Config.SCORES_FULL_READING_PART_3 + Config.SCORES_FULL_READING_PART_4;
            }
            else if (typeCode.Equals(TestCategory.LISTENING))
            {
                maxScores = Config.SCORES_FULL_LISTENING_PART_1 + Config.SCORES_FULL_LISTENING_PART_2;
            }
            else if (typeCode.Equals(TestCategory.WRITING))
            {
                maxScores = Config.SCORES_FULL_WRITING_PART_1 + Config.SCORES_FULL_WRITING_PART_2;
            }
            else if (typeCode.Equals(TestCategory.SPEAKING))
            {
                maxScores = Config.SCORES_FULL_SPEAKING;
            }
            else
            {
                maxScores = -1;
            }

            return maxScores;
        }
    }
}
