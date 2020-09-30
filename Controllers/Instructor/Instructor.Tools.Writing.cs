using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.PiceOfTest;
using TCU.English.Utils;
using static TCU.English.Models.PiceOfTest.WritingTestPaper;

namespace TCU.English.Controllers
{
    public partial class InstructorController
    {
        [HttpPost]
        public IActionResult SubmitToolResultForWriting(WritingTestPaper wtp, WritingPartTwoDTO wp2DTO)
        {
            // Định nghĩa đích đến
            var dest = RedirectToAction(nameof(TestPaperController.ReviewHandler), NameUtils.ControllerName<TestPaperController>(), new { id = wtp.PiceOfTestId });

            // Nếu không xác định được bài thi
            if (wtp == null || wtp.PiceOfTestId <= 0)
            {
                this.NotifyError("Cannot determine the test");
                return dest;
            }

            // Nếu nội dung đánh giá rỗng
            if (wp2DTO == null || string.IsNullOrEmpty(wp2DTO.TeacherReviewParagraph))
            {
                this.NotifyError("You must correct paragraph for your student");
                return dest;
            }

            // Điểm cho bài thi
            if (wp2DTO.Scores < 0 || wp2DTO.Scores > Config.SCORES_FULL_WRITING_PART_2)
            {
                this.NotifyError("Score invalid");
                return dest;
            }

            // Nếu tất cả đã hợp lệ, tiến hành lấy bản ghi
            var pot = _PieceOfTestManager.Get(wtp.PiceOfTestId);

            // Lấy dữ liệu gốc
            var constWtp = JsonConvert.DeserializeObject<WritingTestPaper>(pot.ResultOfUserJson) as WritingTestPaper;

            // Cập nhật điểm số mới
            pot.Scores = constWtp.WritingPartOnes.Scores + wp2DTO.Scores;

            // Cập nhật điểm số
            constWtp.WritingPartTwos.Scores = wp2DTO.Scores;
            constWtp.WritingPartTwos.TeacherReviewParagraph = wp2DTO.TeacherReviewParagraph;

            // Lưu lại json
            pot.ResultOfUserJson = JsonConvert.SerializeObject(constWtp);

            // Cập nhật vào CSDL
            _PieceOfTestManager.Update(pot);

            // Gửi thông báo thành công
            this.NotifySuccess("Update your review to student success!");

            // Về điểm đích đã khai báo
            return dest;
        }
    }
}