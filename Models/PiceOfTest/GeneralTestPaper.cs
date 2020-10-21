using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Models.PiceOfTest
{
    public class GeneralTestPaper
    {
        public int PieceOfTestId { get; set; }

        public ListeningTestPaper ListeningTestPaper { get; set; }
        public ReadingTestPaper ReadingTestPaper { get; set; }
        public WritingTestPaper WritingTestPaper { get; set; }
        public SpeakingTestPaper SpeakingTestPaper { get; set; }

        /// <summary>
        /// Phương thức tạo bài thi tổng quát
        /// </summary>
        public static GeneralTestPaper Generate(
            TestCategoryManager _TestCategoryManager,
            ListeningBaseQuestionManager _ListeningBaseQuestionManager,
            ListeningMediaManager _ListeningMediaManager,
            WritingPartTwoManager _WritingPartTwoManager,
            SpeakingEmbedManager _SpeakingEmbedManager,
            PieceOfTestManager _PieceOfTestManager,
            int UserId,
            int? InstructorId)
        {
            GeneralTestPaper generateTestPaper = new GeneralTestPaper();

            // Tạo bài thi Listening
            generateTestPaper.ListeningTestPaper = new ListeningTestPaper
            {
                ListeningPartOnes = ListeningTestPaper.Generate(1, _TestCategoryManager, _ListeningMediaManager, _ListeningBaseQuestionManager),
                ListeningPartTwos = ListeningTestPaper.Generate(2, _TestCategoryManager, _ListeningMediaManager, _ListeningBaseQuestionManager),
            };

            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            generateTestPaper.ReadingTestPaper = _TestCategoryManager.GenerateReadingTestPaper();

            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            generateTestPaper.WritingTestPaper = _TestCategoryManager.GenerateWritingTestPaper(_WritingPartTwoManager);

            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            generateTestPaper.SpeakingTestPaper = _TestCategoryManager.GenerateSpeakingTestPaper(_SpeakingEmbedManager);

            // Khởi tạo đối tượng lưu trữ bài kiểm tra này và lưu paper mặc định có đáp án đúng vào
            var piceOfTest = new PieceOfTest
            {
                UserId = UserId,
                InstructorId = InstructorId,
                TypeCode = TestCategory.TEST_ALL,
                PartId = -1,
                ResultOfTestJson = JsonConvert.SerializeObject(generateTestPaper),
                Scores = -1
            };

            // Lưu trữ bài thi vào database trước khi bắt đầu
            _PieceOfTestManager.Add(piceOfTest);

            // Lưu mã bài thi vào
            generateTestPaper.PieceOfTestId = piceOfTest.Id;

            return generateTestPaper;
        }

        /// <summary>
        /// Phương thức kiểm tra xem dã đầy đủ câu trả lời chưa, nếu chưa trả về lỗi, còn không bỏ trống
        /// </summary>
        public string IsFullAnswers()
        {
            if (!Config.IsCheckFullTestPaper)
                return string.Empty;

            if (!ListeningTestPaper.IsPaperFullSelection())
            {
                return "You have not yet answered all the questions of the Listening test";
            }

            if (!ReadingTestPaper.IsPaperFullSelection())
            {
                return "You have not yet answered all the questions of the Reading test";
            }

            if (!WritingTestPaper.IsPaperFullSelection())
            {
                return "You have not written the Writing Part 2 section";
            }

            if (string.IsNullOrEmpty(WritingTestPaper.WritingPartTwos.UserParagraph))
            {
                return "You have not yet answered all the questions of the Writing test";
            }

            // Đã đầy đủ thì trả về rỗng
            return string.Empty;
        }

        public void ScoreCalculate(string resultPaperJson)
        {
            GeneralTestPaper resultPaper = JsonConvert.DeserializeObject<GeneralTestPaper>(resultPaperJson);
            ScoreCalculate(resultPaper);
        }

        public void ScoreCalculate(GeneralTestPaper resultPaper)
        {
            // Tính điểm cho Listening
            ListeningTestPaper.ScoreCalculate(resultPaper.ListeningTestPaper);

            // Tính điểm cho READING
            ReadingTestPaper.ScoresCalculate(resultPaper.ReadingTestPaper);

            // Tính điểm cho Writin Part 1
            WritingTestPaper.ScoreCalculate(resultPaper.WritingTestPaper);

            // Điểm tổng phải là -1 kèm WRITING PART 2 và SPEAKING
            WritingTestPaper.WritingPartTwos.Scores = -1;
            SpeakingTestPaper.SpeakingPart.Scores = -1;
        }


        public void ClearTrueAnswers()
        {
            ReadingTestPaper.RemoveCorrectAnswers();
            ListeningTestPaper.RemoveCorrectAnswers();
            // Xóa answers writing
            for (int i = 0; i < WritingTestPaper.WritingPartOnes.WritingPart.Count; i++)
                WritingTestPaper.WritingPartOnes.WritingPart[i].Answers = string.Empty;
        }

        public float TotalReadingScores()
        {
            float scores = 0;
            if (ReadingTestPaper.ReadingPartOnes.Scores >= 0)
                scores += ReadingTestPaper.ReadingPartOnes.Scores;

            if (ReadingTestPaper.ReadingPartTwos.Scores >= 0)
                scores += ReadingTestPaper.ReadingPartTwos.Scores;

            if (ReadingTestPaper.ReadingPartThrees.Scores >= 0)
                scores += ReadingTestPaper.ReadingPartThrees.Scores;

            if (ReadingTestPaper.ReadingPartFours.Scores >= 0)
                scores += ReadingTestPaper.ReadingPartFours.Scores;

            return scores;
        }

        public float TotalListeningScores()
        {
            float scores = 0;
            if (ListeningTestPaper.Part1Scores >= 0)
                scores += ListeningTestPaper.Part1Scores;

            if (ListeningTestPaper.Part2Scores >= 0)
                scores += ListeningTestPaper.Part2Scores;

            return scores;
        }
        public float TotalWritingScores()
        {
            float scores = 0;
            if (WritingTestPaper.WritingPartOnes.Scores >= 0)
                scores += WritingTestPaper.WritingPartOnes.Scores;

            if (WritingTestPaper.WritingPartTwos.Scores >= 0)
                scores += WritingTestPaper.WritingPartTwos.Scores;

            return scores;
        }

        public bool IsPassed()
        {
            double l = ScoresUtils.GetMaxScores(TestCategory.LISTENING) * 0.3;
            double r = ScoresUtils.GetMaxScores(TestCategory.READING) * 0.3;
            double s = ScoresUtils.GetMaxScores(TestCategory.SPEAKING) * 0.3;
            double w = ScoresUtils.GetMaxScores(TestCategory.WRITING) * 0.3;

            if (ListeningTestPaper.Part1Scores + ListeningTestPaper.Part1Scores < l)
                return false;

            if (ReadingTestPaper.ReadingPartOnes.Scores +
                ReadingTestPaper.ReadingPartTwos.Scores +
                ReadingTestPaper.ReadingPartThrees.Scores +
                ReadingTestPaper.ReadingPartFours.Scores < r)
                return false;

            if (SpeakingTestPaper.SpeakingPart.Scores < s)
                return false;

            if (WritingTestPaper.WritingPartOnes.Scores + WritingTestPaper.WritingPartTwos.Scores < w)
                return false;

            return true;
        }
    }


}
