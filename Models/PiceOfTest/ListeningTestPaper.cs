﻿using Microsoft.AspNetCore.Builder;
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
        const int MAX_CATEGORY_LISTENING_PART_1 = 7;
        [JsonIgnore]
        const int MAX_CATEGORY_LISTENING_PART_2 = 1;

        [JsonIgnore]
        public int PiceOfTestId { get; set; }
        public List<ListeningBaseCombined> ListeningPartOnes { get; set; }
        public List<ListeningBaseCombined> ListeningPartTwos { get; set; }

        public ListeningTestPaper RemoveCorrectAnswers()
        {
            ListeningPartOnes = RemoveCorrectAnswers(ListeningPartOnes);
            ListeningPartTwos = RemoveCorrectAnswers(ListeningPartTwos);
            return this;
        }
        public List<ListeningBaseCombined> RemoveCorrectAnswers(List<ListeningBaseCombined> listening)
        {
            if (listening != null &&
                listening.Count > 0)
            {
                for (int i = 0; i < listening.Count; i++)
                {
                    if (listening[i].TestCategory != null &&
                        listening[i].ListeningMedia != null &&
                        listening[i].ListeningBaseQuestions != null &&
                        listening[i].ListeningBaseQuestions.Count > 0)
                    {
                        for (int j = 0; j < listening[i].ListeningBaseQuestions.Count; j++)
                        {
                            if (listening[i].ListeningBaseQuestions[j] != null &&
                                listening[i].ListeningBaseQuestions[j].AnswerList != null &&
                                listening[i].ListeningBaseQuestions[j].AnswerList.Count > 0)
                            {
                                for (int k = 0; k < listening[i].ListeningBaseQuestions[j].AnswerList.Count; k++)
                                {
                                    listening[i].ListeningBaseQuestions[j].AnswerList[k].IsCorrect = false;
                                }
                            }
                        }
                    }
                }
            }
            return listening;
        }

        /// <summary>
        /// Tạo danh sách câu hỏi theo phần
        /// </summary>
        public static List<ListeningBaseCombined> Generate(
            int partId,
            TestCategoryManager _TestCategoryManager,
            ListeningMediaManager _ListeningMediaManager,
            ListeningBaseQuestionManager _ListeningBaseQuestionManager)
        {
            // Khai báo nơi chưa kết quả
            List<ListeningBaseCombined> result = new List<ListeningBaseCombined>();
            // Só phần cần lấy và số câu hỏi từng phần
            int numberOfCategory;
            int numberQuestionOfCategory;
            if (partId == 1)
            {
                numberOfCategory = MAX_CATEGORY_LISTENING_PART_1;
                numberQuestionOfCategory = Config.MAX_LISTENING_PART_1_QUESTION;
            }
            else if (partId == 2)
            {
                numberOfCategory = MAX_CATEGORY_LISTENING_PART_2;
                numberQuestionOfCategory = Config.MAX_LISTENING_PART_2_QUESTION;
            }
            else
                throw new Exception("Wrong part ID");

            // Lấy danh mục
            var categories = _TestCategoryManager
                .GetForGenerateTest(TestCategory.LISTENING, partId)
                .ToList()
                .Shuffle() // Trộn
                .Take(numberOfCategory);

            // Mỗi ListeningBaseCombined tương ứng cho một danh mục
            foreach (TestCategory category in categories)
            {
                // Lấy media
                var media = _ListeningMediaManager.GetByCategory(category.Id);

                // Lấy danh sách câu hỏi
                var questions = _ListeningBaseQuestionManager
                    .GetAll(category.Id)
                    .ToList()
                    //.Shuffle() // Trộn câu hỏi
                    .Take(numberQuestionOfCategory)
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
                // Thêm vào kết quả
                result.Add(new ListeningBaseCombined
                {
                    TestCategory = category,
                    ListeningMedia = media,
                    ListeningBaseQuestions = questions
                });
            }
            // Trả về kết quả
            return result;
        }

        #region CALCULATE TRUE
        /// <summary>
        /// Tính số câu đúng theo phần
        /// </summary>
        public int CalculateTrueOfPart(int partId, ListeningTestPaper paper)
        {
            // Khung chứa câu hỏi và câu trả lời
            List<ListeningBaseCombined> currentCombined;
            List<ListeningBaseCombined> destCombined;

            if (partId == 1)
            {
                if (ListeningPartOnes == null ||
                    ListeningPartOnes.Count <= 0 ||
                    paper.ListeningPartOnes == null ||
                    paper.ListeningPartOnes.Count != ListeningPartOnes.Count)
                    return -1;
                // Gắn kết dữ liệu
                currentCombined = ListeningPartOnes;
                destCombined = paper.ListeningPartOnes;
            }
            else
            {
                if (ListeningPartTwos == null ||
                     ListeningPartTwos.Count <= 0 ||
                     paper.ListeningPartTwos == null ||
                     paper.ListeningPartTwos.Count != ListeningPartTwos.Count)
                    return -1;
                // Gắn kết dữ liệu
                currentCombined = ListeningPartTwos;
                destCombined = paper.ListeningPartTwos;
            }
            // vì chắc chắn 2 cặp dữ liệu trên có size bằng nhau, nên ta tiến hành cho lặp kép
            for (int i = 0; i < currentCombined.Count && i < destCombined.Count; i++)
            {
                // Lấy danh sách câu hỏi hiện tại
                List<ListeningBaseQuestion> current = currentCombined[i].ListeningBaseQuestions;
                List<ListeningBaseQuestion> dest = destCombined[i].ListeningBaseQuestions;

                // Nếu mục hiện tại không có câu hỏi hoặc mục đích có số lượng câu hỏi khác mục hiện tại thì thoát
                if (current != null &&
                    current.Count > 0 &&
                    dest != null &&
                    current.Count == dest.Count)
                {
                    int count = 0;
                    for (int j = 0; j < current.Count; j++)
                    {
                        try
                        {
                            string trueAnswerOfCurrent = current[j].AnswerList.First(x => x.IsCorrect).AnswerContent;
                            string trueAnswerOfDestination = dest[j].AnswerList.First(x => x.IsCorrect).AnswerContent;
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
            }

            // Có lỗi gì đó thì trả về là -1
            return -1;
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
            foreach (var item in new[] { ListeningPartOnes, ListeningPartTwos })
            {
                foreach (var question in item)
                {
                    foreach (var answers in question.ListeningBaseQuestions)
                    {
                        if (answers != null && !answers.AnswerList.Any(x => x.IsCorrect))
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
            if (paper.ListeningPartOnes == null ||
                ListeningPartOnes == null ||
                ListeningPartOnes.Count <= 0 ||
                paper.ListeningPartOnes.Count != ListeningPartOnes.Count ||
                paper.ListeningPartOnes == null ||
                ListeningPartOnes == null ||
                ListeningPartOnes.Count <= 0 ||
                paper.ListeningPartOnes.Count != ListeningPartOnes.Count)
                return this;

            // PART 1
            for (int i = 0; i < ListeningPartOnes.Count && i < paper.ListeningPartOnes.Count; i++)
            {
                for (int j = 0; j < ListeningPartOnes[i].ListeningBaseQuestions.Count; j++)
                {
                    var correctIndex = paper.ListeningPartOnes[i].ListeningBaseQuestions[j].AnswerList.FindIndex(0,
                        paper.ListeningPartOnes[i].ListeningBaseQuestions[j].AnswerList.Count, x => x.IsCorrect);
                    if (correctIndex >= 0)
                    {
                        ListeningPartOnes[i].ListeningBaseQuestions[j].AnswerList[correctIndex].IsCorrect = true;
                    }
                }
            }

            // PART 2
            for (int i = 0; i < ListeningPartTwos.Count && i < paper.ListeningPartTwos.Count; i++)
            {
                for (int j = 0; j < ListeningPartTwos[i].ListeningBaseQuestions.Count; j++)
                {
                    var correctIndex = paper.ListeningPartTwos[i].ListeningBaseQuestions[j].AnswerList.FindIndex(0,
                        paper.ListeningPartTwos[i].ListeningBaseQuestions[j].AnswerList.Count, x => x.IsCorrect);
                    if (correctIndex >= 0)
                    {
                        ListeningPartTwos[i].ListeningBaseQuestions[j].AnswerList[correctIndex].IsCorrect = true;
                    }
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

            // PART 1
            if (ListeningPartOnes != null)
                foreach (var item in ListeningPartOnes)
                    if (item != null)
                        total += item.ListeningBaseQuestions.Count;

            // PART 2
            if (ListeningPartTwos != null)
                foreach (var item in ListeningPartTwos)
                    if (item != null)
                        total += item.ListeningBaseQuestions.Count;

            return total;
        }
    }
}
