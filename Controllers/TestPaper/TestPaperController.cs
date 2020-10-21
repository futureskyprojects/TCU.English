﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.PiceOfTest;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [Authorize]
    public partial class TestPaperController : Controller
    {
        private readonly IHostEnvironment host;

        private readonly TestCategoryManager _TestCategoryManager;
        private readonly ReadingPartOneManager _ReadingPartOneManager;
        private readonly ReadingPartTwoManager _ReadingPartTwoManager;
        private readonly ListeningMediaManager _ListeningMediaManager;
        private readonly ListeningBaseQuestionManager _ListeningBaseQuestionManager;
        private readonly WritingPartOneManager _WritingPartOneManager;
        private readonly WritingPartTwoManager _WritingPartTwoManager;
        private readonly SpeakingEmbedManager _SpeakingEmbedManager;
        private readonly PieceOfTestManager _PieceOfTestManager;
        private readonly UserManager _UserManager;

        public TestPaperController(
          IHostEnvironment _host,
          IDataRepository<TestCategory> _TestCategoryManager,
          IDataRepository<ReadingPartOne> _ReadingPartOneManager,
          IDataRepository<ReadingPartTwo> _ReadingPartTwoManager,
          IDataRepository<ListeningMedia> _ListeningMediaManager,
          IDataRepository<ListeningBaseQuestion> _ListeningBaseQuestionManager,
          IDataRepository<WritingPartOne> _WritingPartOneManager,
          IDataRepository<WritingPartTwo> _WritingPartTwoManager,
          IDataRepository<PieceOfTest> _PieceOfTestManager,
          IDataRepository<SpeakingEmbed> _SpeakingEmbedManager,
          IDataRepository<User> _UserManager)
        {
            host = _host;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._ReadingPartOneManager = (ReadingPartOneManager)_ReadingPartOneManager;
            this._ReadingPartTwoManager = (ReadingPartTwoManager)_ReadingPartTwoManager;
            this._ListeningMediaManager = (ListeningMediaManager)_ListeningMediaManager;
            this._ListeningBaseQuestionManager = (ListeningBaseQuestionManager)_ListeningBaseQuestionManager;
            this._WritingPartOneManager = (WritingPartOneManager)_WritingPartOneManager;
            this._WritingPartTwoManager = (WritingPartTwoManager)_WritingPartTwoManager;
            this._PieceOfTestManager = (PieceOfTestManager)_PieceOfTestManager;
            this._SpeakingEmbedManager = (SpeakingEmbedManager)_SpeakingEmbedManager;
            this._UserManager = (UserManager)_UserManager;
        }
        public IActionResult Index()
        {
            return RedirectToAction(nameof(TestController.NewTest), NameUtils.ControllerName<TestController>());
        }

        public IActionResult Result(int id, params float[] scoresPart)
        {
            if (id <= 0)
                return BadRequest();

            var piece = _PieceOfTestManager.Get(id);
            if (piece == null)
                return NotFoundTest();

            if (piece.InstructorId != User.Id() && piece.UserId != User.Id())
            {
                this.NotifyError("You are not authorized to view or manipulate this test");
                return RedirectToAction(nameof(HomeController.Index), NameUtils.ControllerName<HomeController>());
            }

            ViewBag.Title = $"{piece.TypeCode.ToUpper()} TESTING RESULT";
            if (piece.Scores >= 0)
                ViewBag.Scores = piece.Scores;
            else
                ViewBag.Scores = 0;

            ViewBag.MaxScores = ScoresUtils.GetMaxScores(piece.TypeCode);

            // Nếu đây là bài thi viết
            if (piece.TypeCode == TestCategory.WRITING)
            {
                // Cập nhật điểm mới của phần cho đúng
                ViewBag.Scores = scoresPart[0];

                // Cập nhật điểm gới hạn cho đúng
                ViewBag.MaxScores = Config.SCORES_FULL_WRITING_PART_1;

                // Gắn cờ xác minh
                ViewBag.IsWriting = true;

                // Đổi tin nhắn thông báo thành công
                ViewBag.Msg = "Congratulations, you finished the test, with <span class=\"font-weight-bold text-danger\">PART 1</span> score is";
            }

            if (piece.TypeCode == TestCategory.SPEAKING)
            {
                ViewBag.IsSpeaking = true;
                // Đổi tin nhắn thông báo thành công
                ViewBag.Msg = "<i class=\"\">Congratulations, you finished the test, your teacher will review your test and mark for you later.</i>";
            }

            if (piece.TypeCode == TestCategory.TEST_ALL)
            {
                //ViewBag.Title = "GENERAL TESTING RESULT";
                //ViewBag.IsGeneral = true;
                //// Đổi tin nhắn thông báo thành công
                //ViewBag.Msg = "<i class=\"\">Congratulations, you finished the test, your teacher will review your test and mark for you later.</i>";
                this.NotifySuccess("Here are the results of your test");
                return RedirectToAction(nameof(GeneralReview), new { piece.Id });
            }


            this.NotifySuccess("Your test is completed!");

            return View(piece);
        }

        public IActionResult ReviewHandler(int id)
        {
            var type = _PieceOfTestManager.GetTestType(id);
            if (type == TestCategory.READING)
                return RedirectToAction(nameof(ReadingReview), new { id });
            if (type == TestCategory.LISTENING)
                return RedirectToAction(nameof(ListeningReview), new { id });
            if (type == TestCategory.WRITING)
                return RedirectToAction(nameof(WritingReview), new { id });
            if (type == TestCategory.SPEAKING)
                return RedirectToAction(nameof(SpeakingReview), new { id });
            if (type == TestCategory.TEST_ALL)
                return RedirectToAction(nameof(GeneralReview), new { id });
            return NotFoundTest();
        }

        private IActionResult NotFoundTest()
        {
            this.NotifyError("Can't find this test");
            return RedirectToAction(nameof(HomeController.Index), NameUtils.ControllerName<HomeController>());
        }

        #region Kiểm tra xem trang đã chọn đủ chưa AJAX
        [HttpPost]
        public IActionResult CheckFullListening(ListeningTestPaper paper)
        {
            string message = string.Empty;

            bool status = paper.IsPaperFullSelection();

            return Json(new { status, message });
        }

        [HttpPost]
        public IActionResult CheckFullReading(ReadingTestPaper paper)
        {
            string message = string.Empty;

            bool status = paper.IsPaperFullSelection();

            return Json(new { status, message });
        }

        [HttpPost]
        public IActionResult CheckFullSpeaking(string audioBase64)
        {
            string message = string.Empty;

            bool status = true;

            if (string.IsNullOrEmpty(audioBase64))
            {
                message = "You have not recorded the audio yet. Please Record atleast 1 second";
                status = false;
            }

            return Json(new { status, message });
        }

        [HttpPost]
        public IActionResult CheckFullWriting(WritingTestPaper paper)
        {
            string message = string.Empty;

            bool status = paper.IsPaperFullSelection();

            return Json(new { status, message });
        }

        [HttpPost]
        public IActionResult CheckFullGeneral(GeneralTestPaper paper, string audioBase64)
        {
            string message = paper.IsFullAnswers();

            bool status = string.IsNullOrEmpty(message);

            if (string.IsNullOrEmpty(audioBase64))
            {
                message = "\r\nYou have not recorded the audio yet";
                status = false;
            }

            return Json(new { status, message });
        }
        #endregion

    }
}
