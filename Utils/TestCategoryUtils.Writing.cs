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
        public static WritingTestPaper GenerateWritingTestPaper(this TestCategoryManager _TestCategoryManager, WritingPartTwoManager _WritingPartTwoManager)
        {
            return new WritingTestPaper
            {
                WritingPartOnes = WritingTestPaper.GeneratePart1(_TestCategoryManager),
                WritingPartTwos = WritingTestPaper.GeneratePart2(_TestCategoryManager, _WritingPartTwoManager),
            };
        }
        public static int GenerateWritingTestPaper(this TestCategoryManager _TestCategoryManager, PieceOfTestManager _PieceOfTestManager, WritingPartTwoManager _WritingPartTwoManager, int UserId, int? InstructorId)
        {
            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            WritingTestPaper paper = _TestCategoryManager.GenerateWritingTestPaper(_WritingPartTwoManager);
            // Khởi tạo đối tượng lưu trữ bài kiểm tra này và lưu paper mặc định có đáp án đúng vào
            var piceOfTest = new PieceOfTest
            {
                UserId = UserId,
                InstructorId = InstructorId,
                TypeCode = TestCategory.WRITING,
                PartId = -1,
                ResultOfTestJson = JsonConvert.SerializeObject(paper),
            };

            // Lưu trữ bài thi vào database trước khi bắt đầu
            _PieceOfTestManager.Add(piceOfTest);

            return piceOfTest.Id;
        }
    }
}
