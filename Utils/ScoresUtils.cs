using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Utils
{
    public static class ScoresUtils
    {
        public static float ScoresCalculate(int correctNum, int totalNum, float maxScores)
        {
            return Math.Ceiling(((float)correctNum / totalNum) * maxScores).ToFloat();
        }
    }
}
