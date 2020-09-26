using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.PiceOfTest;

namespace TCU.English.Utils
{
    public static partial class TestCategoryUtils
    {
        public static int GenerateListeningTestPaper(
            this TestCategoryManager _TestCategoryManager,
            ListeningMediaManager _ListeningMediaManager,
            ListeningBaseQuestionManager _ListeningBaseQuestionManager,
            PieceOfTestManager _PieceOfTestManager,
            int UserId,
            int? InstructorId)
        {
            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            var paper = new ListeningTestPaper
            {
                ListeningPartOnes = ListeningTestPaper.Generate(1, _TestCategoryManager, _ListeningMediaManager, _ListeningBaseQuestionManager),
                ListeningPartTwos = ListeningTestPaper.Generate(2, _TestCategoryManager, _ListeningMediaManager, _ListeningBaseQuestionManager),
            };
            // Khởi tạo đối tượng lưu trữ bài kiểm tra này và lưu paper mặc định có đáp án đúng vào
            var piceOfTest = new PieceOfTest
            {
                UserId = UserId,
                InstructorId = InstructorId,
                TypeCode = TestCategory.LISTENING,
                PartId = -1,
                ResultOfTestJson = JsonConvert.SerializeObject(paper),
            };
            // Lưu trữ bài thi vào database trước khi bắt đầu
            _PieceOfTestManager.Add(piceOfTest);
            // Xóa đáp án đúng trong paper
            return piceOfTest.Id;
        }
    }
}
