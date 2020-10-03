using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.PiceOfTest;

namespace TCU.English.Utils
{
    public static partial class TestCategoryUtils
    {
        public static SpeakingTestPaper GenerateSpeakingTestPaper(this TestCategoryManager _TestCategoryManager, SpeakingEmbedManager _SpeakingPartTwoManager)
        {
            return new SpeakingTestPaper
            {
                SpeakingPart = SpeakingTestPaper.Generate(_TestCategoryManager, _SpeakingPartTwoManager)
            };
        }
        public static int GenerateSpeakingTestPaper(this TestCategoryManager _TestCategoryManager, PieceOfTestManager _PieceOfTestManager, SpeakingEmbedManager _SpeakingPartTwoManager, int UserId, int? InstructorId)
        {
            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            SpeakingTestPaper paper = _TestCategoryManager.GenerateSpeakingTestPaper(_SpeakingPartTwoManager);
            // Khởi tạo đối tượng lưu trữ bài kiểm tra này và lưu paper mặc định có đáp án đúng vào
            var piceOfTest = new PieceOfTest
            {
                UserId = UserId,
                InstructorId = InstructorId,
                TypeCode = TestCategory.SPEAKING,
                PartId = -1,
                ResultOfTestJson = JsonConvert.SerializeObject(paper),
            };

            // Lưu trữ bài thi vào database trước khi bắt đầu
            _PieceOfTestManager.Add(piceOfTest);

            return piceOfTest.Id;
        }
    }
}
