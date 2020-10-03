using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.PiceOfTest;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    public partial class TestPaperController
    {

        [HttpGet]
        public IActionResult General(int id)
        {
            ViewBag.Title = "GENERAL TESTING";
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
                return RedirectToAction(nameof(GeneralReview), new { id });
            }

            // Tránh timer bị reset
            if (piece.CreatedTime != null)
            {
                ViewBag.Timer = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds;
            }

            // Giải mã thành giữ liệu bài thi
            GeneralTestPaper paper = JsonConvert.DeserializeObject<GeneralTestPaper>(piece.ResultOfTestJson);

            // Gắn mã dữ liệu
            paper.PieceOfTestId = piece.Id;

            // Xóa đáp án của bài thi
            paper.ClearTrueAnswers();

            this.NotifySuccess("Try your best, Good Luck!");

            return View(paper);
        }
        public IActionResult GeneralNewTest(int? id)
        {
            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            int PiceOfTestId = GeneralTestPaper.Generate(
                _TestCategoryManager,
                _ListeningBaseQuestionManager,
                _ListeningMediaManager,
                _WritingPartTwoManager,
                _SpeakingEmbedManager,
                _PieceOfTestManager,
                User.Id(),
                id).PieceOfTestId;

            if (PiceOfTestId > 0)
            {
                // Nếu lưu trữ thành công, thì tiến hành cho thí sinh làm
                return RedirectToAction(nameof(General), new { id = PiceOfTestId });
            }
            else
            {
                // Không thì trả về trang Index
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public IActionResult General(GeneralTestPaper paper, string audioBase64)
        {
            if (paper == null)
                return NotFoundTest();

            if (paper.PieceOfTestId <= 0)
                return NotFoundTest();

            ViewBag.Title = "GENERAL TESTING";

            // Lấy bài thi từ mã số
            PieceOfTest piece = _PieceOfTestManager.Get(paper.PieceOfTestId);

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

            if (string.IsNullOrEmpty(audioBase64))
            {
                // Xóa đáp án của bài thi
                this.NotifyError("You have not recorded the reading yet");
                return View(paper);
            }

            // Kiểm tra check full
            string validateString = paper.IsFullAnswers();

            if (!string.IsNullOrEmpty(validateString))
            {
                this.NotifyError("Please complete all questions");

                //paper = JsonConvert.DeserializeObject<GeneralTestPaper>(piece.ResultOfTestJson);
                // Xóa đáp án của bài thi
                return View(paper);
            }

            // Tính toán số điểm
            paper.ScoreCalculate(piece.ResultOfTestJson);

            // Thời gian kết thúc bài thi
            float timeToFinished = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds.ToFloat();

            // Cập nhật dữ liệu
            piece.ResultOfUserJson = JsonConvert.SerializeObject(paper);
            piece.TimeToFinished = timeToFinished;
            _PieceOfTestManager.Update(piece);

            // Chuyển đến trang kết quả
            return RedirectToAction(nameof(Result), new { id = piece.Id });
        }
        [HttpGet]
        public IActionResult GeneralReview(int id)
        {
            if (id <= 0)
                return NotFoundTest();

            ViewBag.Title = "GENERAL TESTING";

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
            GeneralTestPaper userPaper = JsonConvert.DeserializeObject<GeneralTestPaper>(piece.ResultOfUserJson ?? "");
            // Bài thi mẫu (Được tạo khi thi, của học viên)
            GeneralTestPaper resultPaper = JsonConvert.DeserializeObject<GeneralTestPaper>(piece.ResultOfTestJson ?? "");

            bool isReviewOfFailTest = piece.ResultOfUserJson == null || piece.ResultOfUserJson.Length <= 0 || piece.UpdatedTime == null;
            ViewBag.IsReviewOfFailTest = isReviewOfFailTest;
            ViewBag.ResultPaper = resultPaper;

            if (isReviewOfFailTest)
            {
                userPaper = resultPaper;
            }

            return View(nameof(General), userPaper);
        }
    }
}
