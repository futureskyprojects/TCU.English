using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.PiceOfTest;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    public partial class TestPaperController
    {
        public IActionResult WritingNewTest(int? id)
        {
            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            int PiceOfTestId = _TestCategoryManager.GenerateWritingTestPaper(_WritingPartTwoManager, User.Id(), id);
            if (PiceOfTestId > 0)
            {
                // Nếu lưu trữ thành công, thì tiến hành cho thí sinh làm
                return RedirectToAction(nameof(Writing), new { id = PiceOfTestId });
            }
            else
            {
                // Không thì trả về trang Index
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Writing(int id)
        {
            ViewBag.Title = "WRITING TESTING";
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
                return RedirectToAction(nameof(WritingReview), new { id = id });
            }

            // Tránh timer bị reset
            if (piece.CreatedTime != null)
            {
                ViewBag.Timer = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds;
            }

            WritingTestPaper paper = JsonConvert.DeserializeObject<WritingTestPaper>(piece.ResultOfTestJson);

            paper.PiceOfTestId = piece.Id;

            this.NotifySuccess("Try your best, Good Luck!");

            return View(paper);
        }

        [HttpPost]
        public IActionResult Writing(WritingTestPaper paper)
        {
            if (paper == null)
                return NotFoundTest();

            if (paper.PiceOfTestId <= 0)
                return NotFoundTest();

            ViewBag.Title = "WRITING TESTING";

            // Sau khi hoàn tất lọc các lỗi, tiến hành xử lý, đếm số câu đúng
            PieceOfTest piece = _PieceOfTestManager.Get(paper.PiceOfTestId);

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

            // Tránh timer bị reset
            if (piece.CreatedTime != null)
            {
                ViewBag.Timer = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds;
            }

            // Kiểm tra check full
            if (!paper.IsPaperFullSelection())
            {
                ViewBag.Error = "Please complete all questions.";

                paper = JsonConvert.DeserializeObject<WritingTestPaper>(piece.ResultOfTestJson);

                return View(paper);
            }
            int total = paper.TotalQuestions(); // Tổng số câu hỏi
            if (total <= 0)
            {
                ViewBag.Error = "The test does not have any questions.";

                paper = JsonConvert.DeserializeObject<WritingTestPaper>(piece.ResultOfTestJson);
                return View(paper);
            }
            int correct = paper.CalculateTrue(piece.ResultOfTestJson); // Tổng số câu đúng
            float scores = Math.Ceiling(((float)correct / total) * Config.MAX_SCORE_POINT).ToFloat();
            float timeToFinished = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds.ToFloat();
            // Cập nhật dữ liệu
            piece.ResultOfUserJson = JsonConvert.SerializeObject(paper);
            piece.Scores = scores;
            piece.TimeToFinished = timeToFinished;
            _PieceOfTestManager.Update(piece);
            // Chuyển đến trang kết quả
            return RedirectToAction(nameof(Result), new { id = piece.Id });
        }

        [HttpGet]
        public IActionResult WritingReview(int id)
        {
            if (id <= 0)
                return NotFoundTest();

            ViewBag.Title = "WRITING TESTING";

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
            if (piece.CreatedTime != null)
            {
                if (piece.UpdatedTime == null)
                {
                    ViewBag.Timer = 0;
                    //ViewBag.Timer = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds;
                }
                else
                {
                    ViewBag.Timer = ((DateTime)piece.UpdatedTime).Subtract((DateTime)piece.CreatedTime).TotalSeconds;
                }
            }
            ViewBag.Scores = piece.Scores;

            // Bài thi của học viên
            WritingTestPaper userPaper = JsonConvert.DeserializeObject<WritingTestPaper>(piece.ResultOfUserJson ?? "");
            // Bài thi mẫu (Được tạo khi thi, của học viên)
            WritingTestPaper resultPaper = JsonConvert.DeserializeObject<WritingTestPaper>(piece.ResultOfTestJson ?? "");

            bool isReviewOfFailTest = piece.ResultOfUserJson == null || piece.ResultOfUserJson.Length <= 0 || piece.UpdatedTime == null;
            ViewBag.IsReviewOfFailTest = isReviewOfFailTest;
            ViewBag.ResultPaper = resultPaper;

            if (isReviewOfFailTest)
            {
                userPaper = resultPaper;
            }

            return View(nameof(Writing), userPaper);
        }
    }
}
