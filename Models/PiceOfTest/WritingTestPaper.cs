using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TCU.English.Models.DataManager;
using TCU.English.Utils;

namespace TCU.English.Models.PiceOfTest
{
    public class WritingTestPaper
    {
        [JsonIgnore]
        const int MAX_QUESTION_WRITING_PART_1 = 5;

        public class WritingPartOneDTO
        {
            public TestCategory TestCategory { get; set; }
            public List<WritingPartOne> WritingPart { get; set; }
            public float Scores { get; set; } = -1;
        }
        public class WritingPartTwoDTO
        {
            public WritingCombined WritingPart2 { get; set; }
            [Required(ErrorMessage = "You must enter the text")]
            public string UserParagraph { get; set; }
            public float Scores { get; set; } = -1;
        }
        public int PiceOfTestId { get; set; }
        public WritingPartOneDTO WritingPartOnes { get; set; }
        public WritingPartTwoDTO WritingPartTwos { get; set; }

        #region GENRATE QUESTION
        public static WritingPartOneDTO GeneratePart1(TestCategoryManager _TestCategoryManager)
        {
            // Lấy danh mục
            var category = _TestCategoryManager
                .GetForGenerateTest(TestCategory.WRITING, 1)
                .ToList()
                .Shuffle() // Trộn
                .First();
            // Lấy danh sách câu hỏi
            var questions = category
                .WritingPartOnes
                .ToList()
                .Shuffle() // Trộn câu hỏi
                .Take(MAX_QUESTION_WRITING_PART_1)
                .ToList();

            // Kiến tạo danh sách câu trả lời
            for (int i = 0; i < questions.Count; i++)
                questions[i].BaseAnswers = JsonConvert.DeserializeObject<List<BaseAnswer>>(questions[i].Answers);

            return new WritingPartOneDTO
            {
                TestCategory = category,
                WritingPart = questions
            };
        }
        public static WritingPartTwoDTO GeneratePart2(TestCategoryManager _TestCategoryManager, WritingPartTwoManager _WritingPartTwoManager)
        {
            var category = _TestCategoryManager
                .GetForGenerateTest(TestCategory.WRITING, 2)
                .ToList()
                .Shuffle() // Trộn
                .First();
            var questions = _WritingPartTwoManager.GetByCategoryId(category.Id);

            return new WritingPartTwoDTO
            {
                WritingPart2 = new WritingCombined
                {
                    TestCategory = category,
                    WritingPartTwo = questions
                },
                UserParagraph = string.Empty
            };
        }

        #endregion

        #region CALCULATE TRUE
        private int CalculateTrue(WritingTestPaper resultPaper)
        {
            if (WritingPartOnes.WritingPart != null &&
                WritingPartOnes.WritingPart.Count > 0 &&
                resultPaper.WritingPartOnes.WritingPart != null &&
                WritingPartOnes.WritingPart.Count == resultPaper.WritingPartOnes.WritingPart.Count)
            {
                int count = 0;
                for (int i = 0; i < WritingPartOnes.WritingPart.Count; i++)
                {
                    try
                    {
                        // Lấy câu trả lời mà học viên đã nhập vào
                        string answerInputed = WritingPartOnes.WritingPart[i].Answers;

                        // Nếu câu trả lời khớp với bất kỳ đáp án nào, tiến hành cho điể
                        if (resultPaper.WritingPartOnes.WritingPart[i].BaseAnswers.Any(x => x.AnswerContent.ToLower().Trim().Equals(answerInputed.ToLower().Trim())))
                            count++;
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

        public int CalculateTrue(string readingTestPaperJson)
        {
            WritingTestPaper paper = JsonConvert.DeserializeObject<WritingTestPaper>(readingTestPaperJson);
            return CalculateTrue(paper);
        }
        #endregion

        public bool IsPaperFullSelection()
        {
            if (WritingPartOnes.WritingPart != null)
                return string.IsNullOrEmpty(WritingPartTwos.UserParagraph);

            return !WritingPartOnes.WritingPart.Any(x => string.IsNullOrEmpty(x.Answers));
        }

        public int TotalQuestions()
        {
            return MAX_QUESTION_WRITING_PART_1;
        }
    }
}
