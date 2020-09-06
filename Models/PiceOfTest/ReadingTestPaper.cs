using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models.DataManager;
using TCU.English.Utils;

namespace TCU.English.Models.PiceOfTest
{
    public class ReadingTestPaper
    {
        [JsonIgnore]
        const int MAX_QUESTION_READING_PART_1 = 10;
        [JsonIgnore]
        const int MAX_QUESTION_READING_PART_2 = 5;
        [JsonIgnore]
        const int MAX_QUESTION_READING_PART_3 = 5;
        [JsonIgnore]
        const int MAX_QUESTION_READING_PART_4 = 10;
        [JsonIgnore]
        public int PiceOfTestId { get; set; }
        public (TestCategory, List<ReadingPartOne>) ReadingPartOnes { get; set; }
        public (TestCategory, List<ReadingPartTwo>) ReadingPartTwos { get; set; }
        public (TestCategory, List<ReadingPartTwo>) ReadingPartThrees { get; set; }
        public (TestCategory, List<ReadingPartTwo>) ReadingPartFours { get; set; }

        public ReadingTestPaper RemoveCorrectAnswers()
        {
            #region Remove for part 1
            if (ReadingPartOnes.Item1 != null &&
                ReadingPartOnes.Item2 != null &&
                ReadingPartOnes.Item2.Count > 0)
            {
                for (int i = 0; i < ReadingPartOnes.Item2.Count; i++)
                {
                    if (ReadingPartOnes.Item2[i] != null &&
                        ReadingPartOnes.Item2[i].AnswerList != null &&
                        ReadingPartOnes.Item2[i].AnswerList.Count > 0)
                    {
                        for (int j = 0; j < ReadingPartOnes.Item2[i].AnswerList.Count; j++)
                        {
                            ReadingPartOnes.Item2[i].AnswerList[j].IsCorrect = false;
                        }
                    }
                }
            }
            #endregion

            #region Remove for part 2
            if (ReadingPartTwos.Item1 != null &&
                ReadingPartTwos.Item2 != null &&
                ReadingPartTwos.Item2.Count > 0)
            {
                for (int i = 0; i < ReadingPartTwos.Item2.Count; i++)
                {
                    if (ReadingPartTwos.Item2[i] != null &&
                        ReadingPartTwos.Item2[i].AnswerList != null &&
                        ReadingPartTwos.Item2[i].AnswerList.Count > 0)
                    {
                        for (int j = 0; j < ReadingPartTwos.Item2[i].AnswerList.Count; j++)
                        {
                            ReadingPartTwos.Item2[i].AnswerList[j].IsCorrect = false;
                        }
                    }
                }
            }
            #endregion

            #region Remove for part 3
            if (ReadingPartThrees.Item1 != null &&
                ReadingPartThrees.Item2 != null &&
                ReadingPartThrees.Item2.Count > 0)
            {
                for (int i = 0; i < ReadingPartThrees.Item2.Count; i++)
                {
                    if (ReadingPartThrees.Item2[i] != null &&
                        ReadingPartThrees.Item2[i].AnswerList != null &&
                        ReadingPartThrees.Item2[i].AnswerList.Count > 0)
                    {
                        for (int j = 0; j < ReadingPartThrees.Item2[i].AnswerList.Count; j++)
                        {
                            ReadingPartThrees.Item2[i].AnswerList[j].IsCorrect = false;
                        }
                    }
                }
            }
            #endregion

            #region Remove for part 4
            if (ReadingPartFours.Item1 != null &&
                ReadingPartFours.Item2 != null &&
                ReadingPartFours.Item2.Count > 0)
            {
                for (int i = 0; i < ReadingPartFours.Item2.Count; i++)
                {
                    if (ReadingPartFours.Item2[i] != null &&
                        ReadingPartFours.Item2[i].AnswerList != null &&
                        ReadingPartFours.Item2[i].AnswerList.Count > 0)
                    {
                        for (int j = 0; j < ReadingPartFours.Item2[i].AnswerList.Count; j++)
                        {
                            ReadingPartFours.Item2[i].AnswerList[j].IsCorrect = false;
                        }
                    }
                }
            }
            #endregion

            return this;
        }

        public static (TestCategory, List<ReadingPartOne>) GeneratePart1(TestCategoryManager _TestCategoryManager)
        {
            // Lấy danh mục
            var category = _TestCategoryManager
                .GetAll(TestCategory.READING, 1)
                .ToList()
                .Shuffle()
                .First();
            // Lấy danh sách câu hỏi
            var questions = category
                .ReadingPartOnes
                .ToList()
                .Shuffle()
                .Take(MAX_QUESTION_READING_PART_1)
                .ToList();
            // Kiến tạo, trộn đáp án
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i].AnswerList = JsonConvert.DeserializeObject<List<BaseAnswer>>(questions[i].Answers);
                if (questions[i].AnswerList != null && questions[i].AnswerList.Count > 0)
                {
                    questions[i].AnswerList.Shuffle();
                }
            }
            return (category, questions);
        }
        public static (TestCategory, List<ReadingPartTwo>) GeneratePart2(TestCategoryManager _TestCategoryManager)
        {
            var category = _TestCategoryManager
                .GetAll(TestCategory.READING, 2)
                .ToList()
                .Shuffle()
                .First();
            var questions = category
                .ReadingPartTwos
                .ToList()
                .Shuffle()
                .Take(MAX_QUESTION_READING_PART_2)
                .ToList();
            // Kiến tạo, trộn đáp án
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i].AnswerList = JsonConvert.DeserializeObject<List<BaseAnswer>>(questions[i].Answers);
                if (questions[i].AnswerList != null && questions[i].AnswerList.Count > 0)
                {
                    questions[i].AnswerList.Shuffle();
                }
            }
            return (category, questions);
        }
        public static (TestCategory, List<ReadingPartTwo>) GeneratePart3(TestCategoryManager _TestCategoryManager)
        {
            var category = _TestCategoryManager
                .GetAll(TestCategory.READING, 3)
                .ToList()
                .Shuffle()
                .First();
            var questions = category
                .ReadingPartTwos
                .ToList()
                .Shuffle()
                .Take(MAX_QUESTION_READING_PART_3)
                .ToList();
            // Kiến tạo, trộn đáp án
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i].AnswerList = JsonConvert.DeserializeObject<List<BaseAnswer>>(questions[i].Answers);
                if (questions[i].AnswerList != null && questions[i].AnswerList.Count > 0)
                {
                    questions[i].AnswerList.Shuffle();
                }
            }
            return (category, questions);
        }
        public static (TestCategory, List<ReadingPartTwo>) GeneratePart4(TestCategoryManager _TestCategoryManager)
        {
            var category = _TestCategoryManager
                .GetAll(TestCategory.READING, 4)
                .ToList()
                .Shuffle()
                .First();
            var questions = category
                .ReadingPartTwos
                .ToList()
                .Shuffle()
                .Take(MAX_QUESTION_READING_PART_4)
                .ToList();
            // Kiến tạo, trộn đáp án
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i].AnswerList = JsonConvert.DeserializeObject<List<BaseAnswer>>(questions[i].Answers);
                if (questions[i].AnswerList != null && questions[i].AnswerList.Count > 0)
                {
                    questions[i].AnswerList.Shuffle();
                }
            }
            return (category, questions);
        }

    }
}
