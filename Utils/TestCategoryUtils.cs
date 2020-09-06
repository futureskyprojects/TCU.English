using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models.DataManager;
using TCU.English.Models.PiceOfTest;

namespace TCU.English.Utils
{
    public static class TestCategoryUtils
    {
        public static ReadingTestPaper GenerateReadingTestPaper(this TestCategoryManager _TestCategoryManager)
        {
            return new ReadingTestPaper
            {
                ReadingPartOnes = ReadingTestPaper.GeneratePart1(_TestCategoryManager),
                ReadingPartTwos = ReadingTestPaper.GeneratePart2(_TestCategoryManager),
                ReadingPartThrees = ReadingTestPaper.GeneratePart3(_TestCategoryManager),
                ReadingPartFours = ReadingTestPaper.GeneratePart4(_TestCategoryManager)
            };
        }
    }
}
