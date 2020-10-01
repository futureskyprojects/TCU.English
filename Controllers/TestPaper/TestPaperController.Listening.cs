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
        public IActionResult LoadTranscript([FromQuery] int id, [FromQuery] int mediaId)
        {
            if (id <= 0 || mediaId <= 0)
                return Content(string.Empty);

            // Lấy bài thi theo mã
            PieceOfTest piece = _PieceOfTestManager.Get(id);

            // Nếu không có, trả về
            if (piece == null)
                return Content(string.Empty);

            // Lấy trang giấy thi
            ListeningTestPaper paper = JsonConvert.DeserializeObject<ListeningTestPaper>(piece.ResultOfTestJson);

            // Nếu trang giấy rỗng
            if (paper == null)
                return Content(string.Empty);

            // Cố gắng tìm kiếm transcript ở part 1
            if (paper.ListeningPartOnes != null && paper.ListeningPartOnes.Any(x => x.ListeningMedia.Id == mediaId))
                return Content(paper.ListeningPartOnes.Where(x => x.ListeningMedia.Id == mediaId).FirstOrDefault()?.ListeningMedia?.Transcript ?? "");

            // Cố gắng tìm kiếm transcript ở part 2
            if (paper.ListeningPartTwos != null && paper.ListeningPartTwos.Any(x => x.ListeningMedia.Id == mediaId))
                return Content(paper.ListeningPartTwos.Where(x => x.ListeningMedia.Id == mediaId).FirstOrDefault()?.ListeningMedia?.Transcript ?? "");

            // Nếu cũng không có thì trả về rỗng
            return Content(string.Empty);

        }

        /// <summary>
        /// Phương thức tạo dữ liệu thi cho bài thi Listening
        /// </summary>
        /// <param name="id">Đây là mã của đối tượng thuộc kiểu PiecOfTest</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Listening(int id)
        {
            ViewBag.Title = "LISTENING TESTING";
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
                return RedirectToAction(nameof(ListeningReview), new { id });
            }

            // Tránh timer bị reset
            if (piece.CreatedTime != null)
            {
                ViewBag.Timer = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds;
            }

            ListeningTestPaper paper = JsonConvert.DeserializeObject<ListeningTestPaper>(piece.ResultOfTestJson)
                .RemoveCorrectAnswers();
            paper.PiceOfTestId = piece.Id;

            this.NotifySuccess("Try your best, Good Luck!");

            return View(paper);
        }
        public IActionResult ListeningNewTest(int? id)
        {
            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            int PiceOfTestId = _TestCategoryManager
                .GenerateListeningTestPaper(
                _ListeningMediaManager,
                _ListeningBaseQuestionManager,
                _PieceOfTestManager,
                User.Id(),
                id);

            if (PiceOfTestId > 0)
            {
                // Nếu lưu trữ thành công, thì tiến hành cho thí sinh làm
                return RedirectToAction(nameof(Listening), new { id = PiceOfTestId });
            }
            else
            {
                // Không thì trả về trang Index
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public IActionResult Listening(ListeningTestPaper paper)
        {
            if (paper == null)
                return NotFoundTest();

            if (paper.PiceOfTestId <= 0)
                return NotFoundTest();

            ViewBag.Title = "READING TESTING";

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

                paper = JsonConvert.DeserializeObject<ListeningTestPaper>(piece.ResultOfTestJson).RemoveCorrectAnswers()
                    .CopySelectedAnswers(paper);

                return View(paper);
            }
            int total = paper.TotalQuestions(); // Tổng số câu hỏi
            if (total <= 0)
            {
                this.NotifyError("The test does not have any questions");

                paper = JsonConvert.DeserializeObject<ListeningTestPaper>(piece.ResultOfTestJson).RemoveCorrectAnswers()
                    .CopySelectedAnswers(paper);
                return View(paper);
            }
            // Tính toán số điểm
            float scores = paper.ScoreCalculate(piece.ResultOfTestJson);

            // Thời gian kết thúc bài thi
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
        public IActionResult ListeningReview(int id)
        {
            if (id <= 0)
                return NotFoundTest();

            ViewBag.Title = "LISTENING TESTING";

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
            ListeningTestPaper userPaper = JsonConvert.DeserializeObject<ListeningTestPaper>(piece.ResultOfUserJson ?? "");
            // Bài thi mẫu (Được tạo khi thi, của học viên)
            ListeningTestPaper resultPaper = JsonConvert.DeserializeObject<ListeningTestPaper>(piece.ResultOfTestJson ?? "");

            bool isReviewOfFailTest = piece.ResultOfUserJson == null || piece.ResultOfUserJson.Length <= 0 || piece.UpdatedTime == null;
            ViewBag.IsReviewOfFailTest = isReviewOfFailTest;
            ViewBag.ResultPaper = resultPaper;

            if (isReviewOfFailTest)
            {
                userPaper = resultPaper;
            }

            return View(nameof(Listening), userPaper);
        }
    }
}
