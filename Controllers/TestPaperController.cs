using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.PiceOfTest;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [Authorize]
    public class TestPaperController : Controller
    {
        private readonly IHostEnvironment host;

        private readonly TestCategoryManager _TestCategoryManager;
        private readonly ReadingPartOneManager _ReadingPartOneManager;
        private readonly ReadingPartTwoManager _ReadingPartTwoManager;
        private readonly ListeningMediaManager _ListeningMediaManager;
        private readonly ListeningBaseQuestionManager _ListeningBaseQuestionManager;
        private readonly WritingPartOneManager _WritingPartOneManager;
        private readonly PieceOfTestManager _PieceOfTestManager;

        public TestPaperController(
          IHostEnvironment _host,
          IDataRepository<TestCategory> _TestCategoryManager,
          IDataRepository<ReadingPartOne> _ReadingPartOneManager,
          IDataRepository<ReadingPartTwo> _ReadingPartTwoManager,
          IDataRepository<ListeningMedia> _ListeningMediaManager,
          IDataRepository<ListeningBaseQuestion> _ListeningBaseQuestionManager,
          IDataRepository<WritingPartOne> _WritingPartOneManager,
          IDataRepository<PieceOfTest> _PieceOfTestManager)
        {
            host = _host;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._ReadingPartOneManager = (ReadingPartOneManager)_ReadingPartOneManager;
            this._ReadingPartTwoManager = (ReadingPartTwoManager)_ReadingPartTwoManager;
            this._ListeningMediaManager = (ListeningMediaManager)_ListeningMediaManager;
            this._ListeningBaseQuestionManager = (ListeningBaseQuestionManager)_ListeningBaseQuestionManager;
            this._WritingPartOneManager = (WritingPartOneManager)_WritingPartOneManager;
            this._PieceOfTestManager = (PieceOfTestManager)_PieceOfTestManager;
        }
        public IActionResult Index()
        {
            return RedirectToAction(nameof(TestController.NewTest), NameUtils.ControllerName<TestController>());
        }

        public IActionResult Result(int id)
        {
            if (id <= 0)
                return BadRequest();
            var piece = _PieceOfTestManager.Get(id);
            if (piece == null)
                return NotFound();
            ViewBag.Title = $"{piece.TypeCode.ToUpper()} TESTING RESULT";
            ViewBag.Scores = piece.Scores;
            return View(piece);
        }

        public IActionResult ReviewHandler(int id)
        {
            var type = _PieceOfTestManager.GetTestType(id);
            if (type == TestCategory.READING)
                return RedirectToAction(nameof(ReadingReview), new { id });
            if (type == TestCategory.LISTENING)
                return RedirectToAction(nameof(ListeningReview), new { id });
            return NotFound();
        }


        #region READING TEST
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
                // Không thì trả về trang Index
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Reading(int id)
        {
            ViewBag.Title = "READING TESTING";
            if (id <= 0)
                return NotFound();

            // Sau khi hoàn tất lọc các lỗi, tiến hành xử lý, đếm số câu đúng
            PieceOfTest piece = _PieceOfTestManager.Get(id);

            // Nếu tìm không thấy bài Test
            if (piece == null)
                return NotFound();

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
            return View(paper);
        }

        [HttpPost]
        public IActionResult Reading(ReadingTestPaper paper)
        {
            if (paper == null)
                return NotFound();
            if (paper.PiceOfTestId <= 0)
                return NotFound();

            ViewBag.Title = "READING TESTING";

            // Sau khi hoàn tất lọc các lỗi, tiến hành xử lý, đếm số câu đúng
            PieceOfTest piece = _PieceOfTestManager.Get(paper.PiceOfTestId);

            // Tránh timer bị reset
            if (piece.CreatedTime != null)
            {
                ViewBag.Timer = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds;
            }

            // Kiểm tra check full
            if (!paper.IsPaperFullSelection())
            {
                ViewBag.Error = "Please complete all questions.";

                paper = JsonConvert.DeserializeObject<ReadingTestPaper>(piece.ResultOfTestJson).RemoveCorrectAnswers()
                    .CopySelectedAnswers(paper);

                return View(paper);
            }
            int total = paper.TotalQuestions(); // Tổng số câu hỏi
            if (total <= 0)
            {
                ViewBag.Error = "The test does not have any questions.";

                paper = JsonConvert.DeserializeObject<ReadingTestPaper>(piece.ResultOfTestJson).RemoveCorrectAnswers()
                    .CopySelectedAnswers(paper);
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
        public IActionResult ReadingReview(int id)
        {
            if (id <= 0)
                return NotFound();

            ViewBag.Title = "READING TESTING";

            ViewBag.IsReviewMode = true;
            // Sau khi hoàn tất lọc các lỗi, tiến hành lấy
            PieceOfTest piece = _PieceOfTestManager.Get(id);

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
        #endregion

        #region LISTENING

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
                return NotFound();

            // Sau khi hoàn tất lọc các lỗi, tiến hành xử lý, đếm số câu đúng
            PieceOfTest piece = _PieceOfTestManager.Get(id);

            // Nếu tìm không thấy bài Test
            if (piece == null)
                return NotFound();

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
                return NotFound();
            if (paper.PiceOfTestId <= 0)
                return NotFound();

            ViewBag.Title = "READING TESTING";

            // Sau khi hoàn tất lọc các lỗi, tiến hành xử lý, đếm số câu đúng
            PieceOfTest piece = _PieceOfTestManager.Get(paper.PiceOfTestId);

            // Tránh timer bị reset
            if (piece.CreatedTime != null)
            {
                ViewBag.Timer = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds;
            }

            // Kiểm tra check full
            if (!paper.IsPaperFullSelection())
            {
                ViewBag.Error = "Please complete all questions.";

                paper = JsonConvert.DeserializeObject<ListeningTestPaper>(piece.ResultOfTestJson).RemoveCorrectAnswers()
                    .CopySelectedAnswers(paper);

                return View(paper);
            }
            int total = paper.TotalQuestions(); // Tổng số câu hỏi
            if (total <= 0)
            {
                ViewBag.Error = "The test does not have any questions.";

                paper = JsonConvert.DeserializeObject<ListeningTestPaper>(piece.ResultOfTestJson).RemoveCorrectAnswers()
                    .CopySelectedAnswers(paper);
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
        public IActionResult ListeningReview(int id)
        {
            if (id <= 0)
                return NotFound();

            ViewBag.Title = "LISTENING TESTING";

            ViewBag.IsReviewMode = true;
            // Sau khi hoàn tất lọc các lỗi, tiến hành lấy
            PieceOfTest piece = _PieceOfTestManager.Get(id);

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
        #endregion
    }
}
