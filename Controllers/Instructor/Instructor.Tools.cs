using Microsoft.AspNetCore.Mvc;
using TCU.English.Models;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    public partial class InstructorController
    {
        [HttpPost]
        public IActionResult SubmitToolResult(PieceOfTest pot)
        {
            // Định nghĩa đích đến
            var dest = RedirectToAction(nameof(TestPaperController.ReviewHandler), NameUtils.ControllerName<TestPaperController>(), new { id = pot.Id });

            // Nếu không xác định được bài thi
            if (pot == null || pot.Id <= 0)
            {
                this.NotifyError("Cannot determine the test");
                return dest;
            }

            // Nếu nội dung đánh giá rỗng
            if (string.IsNullOrEmpty(pot.InstructorComments))
            {
                this.NotifyError("Please leave your rating for this test.");
                return dest;
            }

            // Nếu chưa cung cấp điểm cho bài nói của sinh viên
            if (pot.Scores < 0)
            {
                this.NotifyError("Please give the student a score.");
                return dest;
            }

            // Nếu điểm quá lớn, thông báo
            if (pot.Scores > Config.SCORES_FULL_SPEAKING)
            {
                this.NotifyError("Cannot cheat student scores.");
                return dest;
            }

            // Nếu tất cả đã hợp lệ, tiến hành lấy bản ghi
            var potRaw = _PieceOfTestManager.Get(pot.Id);

            // Cập nhật dữ liệu mới
            potRaw.InstructorComments = pot.InstructorComments;

            // Nếu là bài nói, Cập nhật điểm cho HV
            if (potRaw.TypeCode == TestCategory.SPEAKING)
                potRaw.Scores = pot.Scores;

            // Cập nhật vào CSDL
            _PieceOfTestManager.Update(potRaw);

            // Gửi thông báo thành công
            this.NotifySuccess("Update your evaluate success!");

            // Về điểm đích đã khai báo
            return dest;
        }
    }
}