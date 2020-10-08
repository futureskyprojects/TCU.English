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
        public IActionResult WritingNewTest(int? id)
        {
            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            int PiceOfTestId = _TestCategoryManager.GenerateWritingTestPaper(_PieceOfTestManager, _WritingPartTwoManager, User.Id(), id);
            if (PiceOfTestId > 0)
            {
                // Nếu lưu trữ thành công, thì tiến hành cho thí sinh làm
                return RedirectToAction(nameof(Writing), new { id = PiceOfTestId });
            }
            else
            {
                this.NotifyError("Generate Writing test fail");
                // Không thì trả về trang Index
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Writing(int id)
        {
            ViewBag.Title = "WRITING TESTING";
            if (id <= 0)
            {
                return NotFoundTest();
            }


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
                return RedirectToAction(nameof(WritingReview), new { id });
            }

            // Tránh timer bị reset
            if (piece.CreatedTime != null)
            {
                ViewBag.Timer = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds;
            }

            WritingTestPaper paper = JsonConvert.DeserializeObject<WritingTestPaper>(piece.ResultOfTestJson);

            // Xóa answers
            if (!Config.IsShowCorrectAnswer)
                for (int i = 0; i < paper.WritingPartOnes.WritingPart.Count; i++)
                    paper.WritingPartOnes.WritingPart[i].Answers = string.Empty;

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
                this.NotifyError("Please complete all questions");

                // Lưu trữ các câu trã lời trước đó của HV
                var tempPart1Answered = paper.WritingPartOnes.WritingPart;

                // Lưu trữ đoạn văn đã nhập của HV
                var paragraph = paper.WritingPartTwos.UserParagraph;

                // Load lại trang giấy thi
                paper = JsonConvert.DeserializeObject<WritingTestPaper>(piece.ResultOfTestJson);

                // Gắn câu trả lời trước đã nhập của HV vào
                for (int i = 0; i < paper.WritingPartOnes.WritingPart.Count; i++)
                {
                    paper.WritingPartOnes.WritingPart[i].Answers = tempPart1Answered[i].Answers;
                }

                // Gắn lại đoạn văn
                paper.WritingPartTwos.UserParagraph = paragraph;

                return View(paper);
            }
            int total = paper.TotalQuestions(); // Tổng số câu hỏi
            if (total <= 0)
            {
                this.NotifyError("The test does not have any questions");

                paper = JsonConvert.DeserializeObject<WritingTestPaper>(piece.ResultOfTestJson);
                return View(paper);
            }

            // Tổng số câu đúng cho phần 1
            float scoresPart1 = paper.ScoreCalculate(piece.ResultOfTestJson);

            // Thời gian hoàn tất
            float timeToFinished = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds.ToFloat();

            // Cập nhật điểm vào cho part 1
            paper.WritingPartOnes.Scores = scoresPart1;

            // Chưa có điểm part 2
            paper.WritingPartTwos.Scores = -1;

            // Cập nhật dữ liệu
            piece.ResultOfUserJson = JsonConvert.SerializeObject(paper);

            // Chưa có điểm tổng
            piece.Scores = -1;

            // Cập nhật thời gian kết thúc
            piece.TimeToFinished = timeToFinished;

            // Cập nhật vào CSDL
            _PieceOfTestManager.Update(piece);

            // Chuyển đến trang kết quả
            return RedirectToAction(nameof(Result), new { id = piece.Id, scoresPart = new float[] { scoresPart1 } });
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
            ViewBag.Timer = piece.TimeToFinished;

            // Điểm số của bài thi
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
