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

        public class ReadingPartOneDTO
        {
            public TestCategory TestCategory { get; set; }
            public List<ReadingPartOne> ReadingPart { get; set; }
            public float Scores { get; set; } = -1;
        }
        public class ReadingPartTwoDTO
        {
            public TestCategory TestCategory { get; set; }
            public List<ReadingPartTwo> ReadingPart { get; set; }
            public float Scores { get; set; } = -1;
        }

        //[JsonIgnore]
        public int PiceOfTestId { get; set; }
        public ReadingPartOneDTO ReadingPartOnes { get; set; }
        public ReadingPartTwoDTO ReadingPartTwos { get; set; }
        public ReadingPartTwoDTO ReadingPartThrees { get; set; }
        public ReadingPartTwoDTO ReadingPartFours { get; set; }

        public ReadingTestPaper RemoveCorrectAnswers()
        {
            #region Remove for part 1
            if (ReadingPartOnes.TestCategory != null &&
                ReadingPartOnes.ReadingPart != null &&
                ReadingPartOnes.ReadingPart.Count > 0)
            {
                for (int i = 0; i < ReadingPartOnes.ReadingPart.Count; i++)
                {
                    if (ReadingPartOnes.ReadingPart[i] != null &&
                        ReadingPartOnes.ReadingPart[i].AnswerList != null &&
                        ReadingPartOnes.ReadingPart[i].AnswerList.Count > 0)
                    {
                        for (int j = 0; j < ReadingPartOnes.ReadingPart[i].AnswerList.Count; j++)
                        {
                            ReadingPartOnes.ReadingPart[i].AnswerList[j].IsCorrect = false;
                        }
                    }
                }
            }
            #endregion

            #region Remove for part 2
            if (ReadingPartTwos.TestCategory != null &&
                ReadingPartTwos.ReadingPart != null &&
                ReadingPartTwos.ReadingPart.Count > 0)
            {
                for (int i = 0; i < ReadingPartTwos.ReadingPart.Count; i++)
                {
                    if (ReadingPartTwos.ReadingPart[i] != null &&
                        ReadingPartTwos.ReadingPart[i].AnswerList != null &&
                        ReadingPartTwos.ReadingPart[i].AnswerList.Count > 0)
                    {
                        for (int j = 0; j < ReadingPartTwos.ReadingPart[i].AnswerList.Count; j++)
                        {
                            ReadingPartTwos.ReadingPart[i].AnswerList[j].IsCorrect = false;
                        }
                    }
                }
            }
            #endregion

            #region Remove for part 3
            if (ReadingPartThrees.TestCategory != null &&
                ReadingPartThrees.ReadingPart != null &&
                ReadingPartThrees.ReadingPart.Count > 0)
            {
                for (int i = 0; i < ReadingPartThrees.ReadingPart.Count; i++)
                {
                    if (ReadingPartThrees.ReadingPart[i] != null &&
                        ReadingPartThrees.ReadingPart[i].AnswerList != null &&
                        ReadingPartThrees.ReadingPart[i].AnswerList.Count > 0)
                    {
                        for (int j = 0; j < ReadingPartThrees.ReadingPart[i].AnswerList.Count; j++)
                        {
                            ReadingPartThrees.ReadingPart[i].AnswerList[j].IsCorrect = false;
                        }
                    }
                }
            }
            #endregion

            #region Remove for part 4
            if (ReadingPartFours.TestCategory != null &&
                ReadingPartFours.ReadingPart != null &&
                ReadingPartFours.ReadingPart.Count > 0)
            {
                for (int i = 0; i < ReadingPartFours.ReadingPart.Count; i++)
                {
                    if (ReadingPartFours.ReadingPart[i] != null &&
                        ReadingPartFours.ReadingPart[i].AnswerList != null &&
                        ReadingPartFours.ReadingPart[i].AnswerList.Count > 0)
                    {
                        for (int j = 0; j < ReadingPartFours.ReadingPart[i].AnswerList.Count; j++)
                        {
                            ReadingPartFours.ReadingPart[i].AnswerList[j].IsCorrect = false;
                        }
                    }
                }
            }
            #endregion

            return this;
        }

        #region GENRATE QUESTION
        public static ReadingPartOneDTO GeneratePart1(TestCategoryManager _TestCategoryManager)
        {
            // Lấy danh mục
            var category = _TestCategoryManager
                .GetForGenerateTest(TestCategory.READING, 1)
                .ToList()
                .Shuffle()
                .First();
            // Lấy danh sách câu hỏi
            var questions = category
                .ReadingPartOnes
                .ToList()
                .Shuffle() // Trộn câu hỏi
                .Take(MAX_QUESTION_READING_PART_1)
                .ToList();
            // Kiến tạo, trộn đáp án
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i].AnswerList = JsonConvert.DeserializeObject<List<BaseAnswer>>(questions[i].Answers);
                if (questions[i].AnswerList != null && questions[i].AnswerList.Count > 0)
                {
                    questions[i].AnswerList.Shuffle(); // Trộn đáp án
                }
            }
            return new ReadingPartOneDTO
            {
                TestCategory = category,
                ReadingPart = questions
            };
        }
        public static ReadingPartTwoDTO GeneratePart2(TestCategoryManager _TestCategoryManager)
        {
            var category = _TestCategoryManager
                .GetForGenerateTest(TestCategory.READING, 2)
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
            return new ReadingPartTwoDTO
            {
                TestCategory = category,
                ReadingPart = questions
            };
        }
        public static ReadingPartTwoDTO GeneratePart3(TestCategoryManager _TestCategoryManager)
        {
            var category = _TestCategoryManager
                .GetForGenerateTest(TestCategory.READING, 3)
                .ToList()
                .Shuffle()
                .First();
            var questions = category
                .ReadingPartTwos
                .ToList()
                //.Shuffle() // Không trộn thứ tự câu hỏi
                .Take(MAX_QUESTION_READING_PART_3)
                .ToList();
            // Kiến tạo, trộn đáp án
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i].AnswerList = JsonConvert.DeserializeObject<List<BaseAnswer>>(questions[i].Answers);
                if (questions[i].AnswerList != null && questions[i].AnswerList.Count > 0)
                {
                    questions[i].AnswerList.Shuffle(); // Chỉ trộn câu trả lời
                }
            }
            return new ReadingPartTwoDTO
            {
                TestCategory = category,
                ReadingPart = questions
            };
        }
        public static ReadingPartTwoDTO GeneratePart4(TestCategoryManager _TestCategoryManager)
        {
            var category = _TestCategoryManager
                .GetForGenerateTest(TestCategory.READING, 4)
                .ToList()
                .Shuffle()
                .First();
            var questions = category
                .ReadingPartTwos
                .ToList()
                //.Shuffle() // Không trộn thứ tự câu hỏi
                .Take(MAX_QUESTION_READING_PART_4)
                .ToList();
            // Kiến tạo, trộn đáp án
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i].AnswerList = JsonConvert.DeserializeObject<List<BaseAnswer>>(questions[i].Answers);
                if (questions[i].AnswerList != null && questions[i].AnswerList.Count > 0)
                {
                    questions[i].AnswerList.Shuffle();// Chỉ trộn thứ tự câu trả lời
                }
            }
            return new ReadingPartTwoDTO
            {
                TestCategory = category,
                ReadingPart = questions
            };
        }
        #endregion

        #region CALCULATE TRUE
        private int CalculateTrueOfPart1(ReadingTestPaper paper)
        {
            if (ReadingPartOnes.ReadingPart != null &&
                ReadingPartOnes.ReadingPart.Count > 0 &&
                paper.ReadingPartOnes.ReadingPart != null &&
                ReadingPartOnes.ReadingPart.Count == paper.ReadingPartOnes.ReadingPart.Count)
            {
                int count = 0;
                for (int i = 0; i < ReadingPartOnes.ReadingPart.Count; i++)
                {
                    try
                    {
                        string trueAnswerOfCurrent = ReadingPartOnes.ReadingPart[i].AnswerList.First(x => x.IsCorrect).AnswerContent;
                        string trueAnswerOfDestination = paper.ReadingPartOnes.ReadingPart[i].AnswerList.First(x => x.IsCorrect).AnswerContent;
                        if (trueAnswerOfCurrent.ToLower().Trim() == trueAnswerOfDestination.ToLower().Trim())
                        {
                            count++;
                        }
                    }
                    catch (Exception)
                    {
                        // Bỏ qua
                    }
                }
                return count;
            }
            else
            {
                return -1;
            }
        }
        public int CalculateTrueOfPart(int partId, ReadingTestPaper paper)
        {
            if (partId == 1)
            {
                return CalculateTrueOfPart1(paper);
            }
            else if (partId > 1 && partId <= 4)
            {
                List<ReadingPartTwo> current = new[] { ReadingPartTwos, ReadingPartThrees, ReadingPartFours }[partId - 2].ReadingPart;
                List<ReadingPartTwo> dest = new[] { paper.ReadingPartTwos, paper.ReadingPartThrees, paper.ReadingPartFours }[partId - 2].ReadingPart;
                if (current != null &&
                    current.Count > 0 &&
                    dest != null &&
                    current.Count == dest.Count)
                {
                    int count = 0;
                    for (int i = 0; i < current.Count; i++)
                    {
                        try
                        {
                            string trueAnswerOfCurrent = current[i].AnswerList.First(x => x.IsCorrect).AnswerContent;
                            string trueAnswerOfDestination = dest[i].AnswerList.First(x => x.IsCorrect).AnswerContent;
                            if (trueAnswerOfCurrent.ToLower().Trim() == trueAnswerOfDestination.ToLower().Trim())
                            {
                                count++;
                            }
                        }
                        catch (Exception)
                        {
                            // Bỏ qua
                        }
                    }
                    return count;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                throw new Exception("Reading không có quá 4 PART!");
            }
        }
        public float ScoresCalculate(ReadingTestPaper paper)
        {
            // Tính điểm cho part 1
            var totalTrue = CalculateTrueOfPart(1, paper);
            if (totalTrue >= 0)
                ReadingPartOnes.Scores = ScoresUtils.ScoresCalculate(totalTrue, ReadingPartOnes.ReadingPart.Count, Config.SCORES_FULL_READING_PART_1);

            // Tính điểm cho part 2
            totalTrue = CalculateTrueOfPart(2, paper);
            if (totalTrue >= 0)
                ReadingPartTwos.Scores = ScoresUtils.ScoresCalculate(totalTrue, ReadingPartTwos.ReadingPart.Count, Config.SCORES_FULL_READING_PART_2);

            // Tính điểm cho part 3
            totalTrue = CalculateTrueOfPart(3, paper);
            if (totalTrue >= 0)
                ReadingPartThrees.Scores = ScoresUtils.ScoresCalculate(totalTrue, ReadingPartThrees.ReadingPart.Count, Config.SCORES_FULL_READING_PART_3);

            // Tính điểm cho part 4
            totalTrue = CalculateTrueOfPart(4, paper);
            if (totalTrue >= 0)
                ReadingPartFours.Scores = ScoresUtils.ScoresCalculate(totalTrue, ReadingPartFours.ReadingPart.Count, Config.SCORES_FULL_READING_PART_4);

            if (ReadingPartOnes.Scores >= 0 && ReadingPartTwos.Scores >= 0 && ReadingPartThrees.Scores >= 0 && ReadingPartFours.Scores >= 0)
                return ReadingPartOnes.Scores + ReadingPartTwos.Scores + ReadingPartThrees.Scores + ReadingPartFours.Scores;
            else
                return -1;
        }

        public float ScoreCalculate(string readingTestPaperJson)
        {
            ReadingTestPaper paper = JsonConvert.DeserializeObject<ReadingTestPaper>(readingTestPaperJson);
            return ScoresCalculate(paper);
        }
        #endregion

        public bool IsPaperFullSelection()
        {
            if (ReadingPartOnes.ReadingPart != null)
                foreach (var answers in ReadingPartOnes.ReadingPart)
                {
                    if (!answers.AnswerList.Any(x => x.IsCorrect))
                    {
                        return false;
                    }
                }

            foreach (var item in new[] { ReadingPartTwos.ReadingPart, ReadingPartThrees.ReadingPart, ReadingPartFours.ReadingPart })
            {
                if (item != null)
                    foreach (var answers in item)
                    {
                        if (!answers.AnswerList.Any(x => x.IsCorrect))
                        {
                            return false;
                        }
                    }
            }
            return true;
        }

        public ReadingTestPaper CopySelectedAnswers(ReadingTestPaper paper)
        {
            if (ReadingPartOnes.ReadingPart != null &&
                paper.ReadingPartOnes.ReadingPart != null &&
                ReadingPartOnes.ReadingPart.Count == paper.ReadingPartOnes.ReadingPart.Count)
            {
                for (int i = 0; i < ReadingPartOnes.ReadingPart.Count; i++)
                {
                    var correctIndex = paper.ReadingPartOnes.ReadingPart[i].AnswerList.FindIndex(0,
                        paper.ReadingPartOnes.ReadingPart[i].AnswerList.Count, x => x.IsCorrect);
                    if (correctIndex >= 0)
                    {
                        ReadingPartOnes.ReadingPart[i].AnswerList[correctIndex].IsCorrect = true;
                    }
                }
            }

            if (ReadingPartTwos.ReadingPart != null &&
                paper.ReadingPartTwos.ReadingPart != null &&
                ReadingPartTwos.ReadingPart.Count == paper.ReadingPartTwos.ReadingPart.Count)
            {
                for (int i = 0; i < ReadingPartTwos.ReadingPart.Count; i++)
                {
                    var correctIndex = paper.ReadingPartTwos.ReadingPart[i].AnswerList.FindIndex(0,
                        paper.ReadingPartTwos.ReadingPart[i].AnswerList.Count, x => x.IsCorrect);
                    if (correctIndex >= 0)
                    {
                        ReadingPartTwos.ReadingPart[i].AnswerList[correctIndex].IsCorrect = true;
                    }
                }
            }

            if (ReadingPartThrees.ReadingPart != null &&
                paper.ReadingPartThrees.ReadingPart != null &&
                ReadingPartThrees.ReadingPart.Count == paper.ReadingPartThrees.ReadingPart.Count)
            {
                for (int i = 0; i < ReadingPartThrees.ReadingPart.Count; i++)
                {
                    var correctIndex = paper.ReadingPartThrees.ReadingPart[i].AnswerList.FindIndex(0,
                        paper.ReadingPartThrees.ReadingPart[i].AnswerList.Count, x => x.IsCorrect);
                    if (correctIndex >= 0)
                    {
                        ReadingPartThrees.ReadingPart[i].AnswerList[correctIndex].IsCorrect = true;
                    }
                }
            }

            if (ReadingPartFours.ReadingPart != null &&
                paper.ReadingPartFours.ReadingPart != null &&
                ReadingPartFours.ReadingPart.Count == paper.ReadingPartFours.ReadingPart.Count)
            {
                for (int i = 0; i < ReadingPartFours.ReadingPart.Count; i++)
                {
                    var correctIndex = paper.ReadingPartFours.ReadingPart[i].AnswerList.FindIndex(0,
                        paper.ReadingPartFours.ReadingPart[i].AnswerList.Count, x => x.IsCorrect);
                    if (correctIndex >= 0)
                    {
                        ReadingPartFours.ReadingPart[i].AnswerList[correctIndex].IsCorrect = true;
                    }
                }
            }
            return this;
        }

        public int TotalQuestions()
        {
            int total = 0;
            if (ReadingPartOnes.ReadingPart != null)
                total += ReadingPartOnes.ReadingPart.Count;
            if (ReadingPartTwos.ReadingPart != null)
                total += ReadingPartTwos.ReadingPart.Count;
            if (ReadingPartThrees.ReadingPart != null)
                total += ReadingPartThrees.ReadingPart.Count;
            if (ReadingPartFours.ReadingPart != null)
                total += ReadingPartFours.ReadingPart.Count;
            return total;
        }
    }
}
