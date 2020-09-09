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
        private readonly ListeningBaseQuestionManager _ListeningBaseQuestionManager;
        private readonly WritingPartOneManager _WritingPartOneManager;
        private readonly PieceOfTestManager _PieceOfTestManager;

        public TestPaperController(
          IHostEnvironment _host,
          IDataRepository<TestCategory> _TestCategoryManager,
          IDataRepository<ReadingPartOne> _ReadingPartOneManager,
          IDataRepository<ReadingPartTwo> _ReadingPartTwoManager,
          IDataRepository<ListeningBaseQuestion> _ListeningBaseQuestionManager,
          IDataRepository<WritingPartOne> _WritingPartOneManager,
          IDataRepository<PieceOfTest> _PieceOfTestManager)
        {
            host = _host;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._ReadingPartOneManager = (ReadingPartOneManager)_ReadingPartOneManager;
            this._ReadingPartTwoManager = (ReadingPartTwoManager)_ReadingPartTwoManager;
            this._ListeningBaseQuestionManager = (ListeningBaseQuestionManager)_ListeningBaseQuestionManager;
            this._WritingPartOneManager = (WritingPartOneManager)_WritingPartOneManager;
            this._PieceOfTestManager = (PieceOfTestManager)_PieceOfTestManager;
        }
        public IActionResult Index()
        {
            return RedirectToAction(nameof(TestController.NewTest), NameUtils.ControllerName<TestController>());
        }

        //[HttpPost]
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

        public IActionResult Reading()
        {
            ViewBag.Title = "READING TESTING";
            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            ReadingTestPaper paper = _TestCategoryManager.GenerateReadingTestPaper(_PieceOfTestManager, User.Id());

            if (paper.PiceOfTestId > 0)
            {
                // Nếu lưu trữ thành công, thì tiến hành cho thí sinh làm
                return View(paper);
            }
            else
            {
                // Không thì trả về trang Index
                return RedirectToAction(nameof(Index));
            }

        }

        [HttpPost]
        public IActionResult Reading(ReadingTestPaper paper)
        {
            if (paper == null)
                return NotFound();
            if (paper.PiceOfTestId <= 0)
                return NotFound();

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
    }
}
