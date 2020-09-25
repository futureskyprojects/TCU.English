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

            // Nếu tất cả đã hợp lệ, tiến hành lấy bản ghi
            var potRaw = _PieceOfTestManager.Get(pot.Id);

            // Cập nhật dữ liệu mới
            potRaw.InstructorComments = pot.InstructorComments;

            // Cập nhật vào CSDL
            _PieceOfTestManager.Update(potRaw);

            // Gửi thông báo thành công
            this.NotifySuccess("Update your comment success!");

            // Về điểm đích đã khai báo
            return dest;
        }
    }
}