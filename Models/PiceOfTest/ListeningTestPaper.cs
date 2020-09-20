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
    public class ListeningTestPaper
    {
        [JsonIgnore]
        const int MAX_QUESTION_LISTENING_PART_1 = 7;
        [JsonIgnore]
        const int MAX_QUESTION_LISTENING_PART_2 = 1;

        [JsonIgnore]
        public int PiceOfTestId { get; set; }
        public ListeningBaseCombined ListeningPartOnes { get; set; }
        public ListeningBaseCombined ListeningPartTwos { get; set; }

        public ListeningTestPaper RemoveCorrectAnswers()
        {
            ListeningPartOnes = RemoveCorrectAnswers(ListeningPartOnes);
            ListeningPartTwos = RemoveCorrectAnswers(ListeningPartTwos);
            return this;
        }
        public ListeningBaseCombined RemoveCorrectAnswers(ListeningBaseCombined listening)
        {
            if (listening.TestCategory != null &&
                listening.ListeningMedia != null &&
                listening.ListeningBaseQuestions.Count > 0)
            {
                for (int i = 0; i < listening.ListeningBaseQuestions.Count; i++)
                {
                    if (listening.ListeningBaseQuestions[i] != null &&
                        listening.ListeningBaseQuestions[i].AnswerList != null &&
                        listening.ListeningBaseQuestions[i].AnswerList.Count > 0)
                    {
                        for (int j = 0; j < listening.ListeningBaseQuestions[i].AnswerList.Count; j++)
                        {
                            listening.ListeningBaseQuestions[i].AnswerList[j].IsCorrect = false;
                        }
                    }
                }
            }
            return listening;
        }

        /// <summary>
        /// Tạo danh sách câu hỏi theo phần
        /// </summary>
        public static ListeningBaseCombined Generate(
            int partId,
            TestCategoryManager _TestCategoryManager,
            ListeningMediaManager _ListeningMediaManager,
            ListeningBaseQuestionManager _ListeningBaseQuestionManager)
        {
            // Lấy danh mục
            var category = _TestCategoryManager
                .GetForGenerateTest(TestCategory.LISTENING, partId)
                .ToList()
                .Shuffle()
                .First();
            // Lấy media
            var media = _ListeningMediaManager.GetByCategory(category.Id);
            // Lấy danh sách câu hỏi
            var questions = _ListeningBaseQuestionManager
                .GetAll(category.Id)
                .ToList()
                //.Shuffle() // Trộn câu hỏi
                .Take(MAX_QUESTION_LISTENING_PART_1)
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
            return new ListeningBaseCombined
            {
                TestCategory = category,
                ListeningMedia = media,
                ListeningBaseQuestions = questions
            };
        }

        #region CALCULATE TRUE
        /// <summary>
        /// Tính số câu đúng theo phần
        /// </summary>
        public int CalculateTrueOfPart(int partId, ListeningTestPaper paper)
        {
            // Lấy danh sách câu hỏi hiện tại
            List<ListeningBaseQuestion> current;
            List<ListeningBaseQuestion> dest;
            if (partId == 1)
            {
                current = ListeningPartOnes.ListeningBaseQuestions;
                dest = paper.ListeningPartOnes.ListeningBaseQuestions;
            }
            else if (partId == 2)
            {
                current = ListeningPartTwos.ListeningBaseQuestions;
                dest = paper.ListeningPartTwos.ListeningBaseQuestions;
            }
            else
                throw new Exception("Wrong index");

            // Nếu mục hiện tại không có câu hỏi hoặc mục đích có số lượng câu hỏi khác mục hiện tại thì thoát
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

        /// <summary>
        /// Tính toán số câu đúng
        /// </summary>
        public int CalculateTrue(ListeningTestPaper paper)
        {
            int count = 0;
            for (int i = 0; i < 2; i++)
            {
                int res = CalculateTrueOfPart(i + 1, paper);
                if (res >= 0)
                    count += res;
            }
            return count;
        }

        /// <summary>
        /// Tính toán số câu đúng
        /// </summary>
        public int CalculateTrue(string readingTestPaperJson)
        {
            ListeningTestPaper paper = JsonConvert.DeserializeObject<ListeningTestPaper>(readingTestPaperJson);
            return CalculateTrue(paper);
        }
        #endregion

        /// <summary>
        /// Học viên đã khoanh tất cả câu trong giấy hay chưa
        /// </summary>
        public bool IsPaperFullSelection()
        {
            foreach (var item in new[] { ListeningPartOnes.ListeningBaseQuestions, ListeningPartTwos.ListeningBaseQuestions })
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

        /// <summary>
        /// Copy những đáp án đã chọn vào trang hiện tại
        /// </summary>
        public ListeningTestPaper CopySelectedAnswers(ListeningTestPaper paper)
        {
            if (ListeningPartTwos.ListeningBaseQuestions == null ||
                ListeningPartTwos.ListeningBaseQuestions.Count <= 0 ||
                paper.ListeningPartTwos.ListeningBaseQuestions == null ||
                paper.ListeningPartTwos.ListeningBaseQuestions != ListeningPartTwos.ListeningBaseQuestions ||
                ListeningPartOnes.ListeningBaseQuestions == null ||
                ListeningPartOnes.ListeningBaseQuestions.Count <= 0 ||
                paper.ListeningPartOnes.ListeningBaseQuestions == null ||
                paper.ListeningPartOnes.ListeningBaseQuestions != ListeningPartOnes.ListeningBaseQuestions)
                return this;

            // Cập nhật cho part 1
            for (int i = 0; i < ListeningPartOnes.ListeningBaseQuestions.Count; i++)
            {
                var correctIndex = paper.ListeningPartOnes.ListeningBaseQuestions[i].AnswerList.FindIndex(0,
                    paper.ListeningPartOnes.ListeningBaseQuestions[i].AnswerList.Count, x => x.IsCorrect);
                if (correctIndex >= 0)
                {
                    ListeningPartOnes.ListeningBaseQuestions[i].AnswerList[correctIndex].IsCorrect = true;
                }
            }
            // Cập nhật cho part 2
            for (int i = 0; i < ListeningPartTwos.ListeningBaseQuestions.Count; i++)
            {
                var correctIndex = paper.ListeningPartTwos.ListeningBaseQuestions[i].AnswerList.FindIndex(0,
                    paper.ListeningPartTwos.ListeningBaseQuestions[i].AnswerList.Count, x => x.IsCorrect);
                if (correctIndex >= 0)
                {
                    ListeningPartTwos.ListeningBaseQuestions[i].AnswerList[correctIndex].IsCorrect = true;
                }
            }
            return this;
        }

        /// <summary>
        /// Đếm tổng số câu hỏi
        /// </summary>
        public int TotalQuestions()
        {
            int total = 0;
            if (ListeningPartOnes.ListeningBaseQuestions != null)
                total += ListeningPartOnes.ListeningBaseQuestions.Count;
            if (ListeningPartTwos.ListeningBaseQuestions != null)
                total += ListeningPartTwos.ListeningBaseQuestions.Count;

            return total;
        }
    }
}
