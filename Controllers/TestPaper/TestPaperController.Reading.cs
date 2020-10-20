using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.PiceOfTest;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    public partial class TestPaperController
    {
        public IActionResult ReadingNewTest(int? id)
        {
            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            int PiceOfTestId = _TestCategoryManager.GenerateReadingTestPaper(_PieceOfTestManager, User.Id(), id);
            if (PiceOfTestId > 0)
            {
                // Nếu lưu trữ thành công, thì tiến hành cho thí sinh làm
                return RedirectToAction(nameof(Reading), new { id = PiceOfTestId });
            }
            else
            {
                this.NotifyError("Generate Reading test fail");
                // Không thì trả về trang Index
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Reading(int id)
        {
            ViewBag.Title = "READING TESTING";
            if (id <= 0)
                return NotFoundTest();

            // Sau khi hoàn tất lọc các lỗi, tiến hành xử lý, đếm số câu đúng
            PieceOfTest piece = _PieceOfTestManager.Get(id);

            // Nếu tìm không thấy bài Test
            if (piece == null)
                return NotFoundTest();

            if (piece.InstructorId != User.Id() && piece.UserId != User.Id())
            {
                this.NotifyError("You are not authorized to view or manipulate this test");
                return RedirectToAction(nameof(HomeController.Index), NameUtils.ControllerName<HomeController>());
            }

            // Lấy chủ sở hữu của bài kiểm tra
            User owner = _UserManager.Get(piece.UserId);
            ViewData["Owner"] = new User
            {
                Avatar = owner.Avatar,
                FirstName = owner.FirstName,
                LastName = owner.LastName
            };

            // Nếu bài thi đã hoàn thành, thì chuyển sang màn hình review
            if (piece.ResultOfUserJson != null && piece.ResultOfUserJson.Length > 0 && piece.UpdatedTime != null)
            {
                return RedirectToAction(nameof(ReadingReview), new { id = id });
            }

            // Tránh timer bị reset
            if (piece.CreatedTime != null)
            {
                ViewBag.Timer = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds;
            }

            ReadingTestPaper paper = JsonConvert.DeserializeObject<ReadingTestPaper>(piece.ResultOfTestJson)
                .RemoveCorrectAnswers();
            paper.PiceOfTestId = piece.Id;

            this.NotifySuccess("Try your best, Good Luck!");

            return View(paper);
        }

        [HttpPost]
        public IActionResult Reading(ReadingTestPaper paper)
        {
            if (paper == null)
            {
                this.NotifyError("Not found yor test");
                return Json(new { status = false, message = string.Empty, location = "/" });
            }

            if (paper.PiceOfTestId <= 0)
            {
                this.NotifyError("Not found yor test");
                return Json(new { status = false, message = string.Empty, location = "/" });
            }

            // Sau khi hoàn tất lọc các lỗi, tiến hành xử lý, đếm số câu đúng
            PieceOfTest piece = _PieceOfTestManager.Get(paper.PiceOfTestId);

            if (piece == null)
            {
                this.NotifyError("Not found yor test");
                return Json(new { status = false, message = string.Empty, location = "/" });
            }

            if (piece.InstructorId != User.Id() && piece.UserId != User.Id())
            {
                this.NotifyError("You are not authorized to view or manipulate this test");
                return Json(new { status = false, message = string.Empty, location = "/" });
            }

            int total = paper.TotalQuestions(); // Tổng số câu hỏi
            if (total <= 0)
                return Json(new { status = false, message = "The test does not have any questions", location = string.Empty });

            // Tính điểm
            float scores = paper.ScoresCalculate(piece.ResultOfTestJson);
            float timeToFinished = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds.ToFloat();

            // Cập nhật dữ liệu
            piece.ResultOfUserJson = JsonConvert.SerializeObject(paper);
            piece.Scores = scores;
            piece.TimeToFinished = timeToFinished;
            _PieceOfTestManager.Update(piece);
            // Chuyển đến trang kết quả
            return Json(new { status = true, message = "Successful submission of exams", location = $"{Url.Action(nameof(Result), NameUtils.ControllerName<TestPaperController>())}/{piece.Id}" });
        }

        [HttpGet]
        public IActionResult ReadingReview(int id)
        {
            if (id <= 0)
                return NotFoundTest();

            ViewBag.Title = "READING TESTING";

            ViewBag.IsReviewMode = true;
            // Sau khi hoàn tất lọc các lỗi, tiến hành lấy
            PieceOfTest piece = _PieceOfTestManager.Get(id);

            if (piece == null)
                return NotFoundTest();

            if (piece.InstructorId != User.Id() && piece.UserId != User.Id())
            {
                this.NotifyError("You are not authorized to view or manipulate this test");
                return RedirectToAction(nameof(HomeController.Index), NameUtils.ControllerName<HomeController>());
            }

            // Lấy chủ sở hữu của bài kiểm tra
            User owner = _UserManager.Get(piece.UserId);
            ViewData["Owner"] = new User
            {
                Avatar = owner.Avatar,
                FirstName = owner.FirstName,
                LastName = owner.LastName
            };

            // Thời gian làm bài
            ViewBag.Timer = piece.TimeToFinished;

            // Điểm của bài thi
            ViewBag.Scores = piece.Scores;

            // Bài thi của học viên
            ReadingTestPaper userPaper = JsonConvert.DeserializeObject<ReadingTestPaper>(piece.ResultOfUserJson ?? "");
            // Bài thi mẫu (Được tạo khi thi, của học viên)
            ReadingTestPaper resultPaper = JsonConvert.DeserializeObject<ReadingTestPaper>(piece.ResultOfTestJson ?? "");

            bool isReviewOfFailTest = piece.ResultOfUserJson == null || piece.ResultOfUserJson.Length <= 0 || piece.UpdatedTime == null;
            ViewBag.IsReviewOfFailTest = isReviewOfFailTest;
            ViewBag.ResultPaper = resultPaper;

            if (isReviewOfFailTest)
            {
                userPaper = resultPaper;
            }

            return View(nameof(Reading), userPaper);
        }
    }
}
