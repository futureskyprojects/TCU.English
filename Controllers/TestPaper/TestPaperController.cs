using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            this._UserManager = (UserManager)_UserManager;
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
                return NotFoundTest();

            if (piece.InstructorId != User.Id() && piece.UserId != User.Id())
            {
                this.NotifyError("You are not authorized to view or manipulate this test");
                return RedirectToAction(nameof(HomeController.Index), NameUtils.ControllerName<HomeController>());
            }

            ViewBag.Title = $"{piece.TypeCode.ToUpper()} TESTING RESULT";
            ViewBag.Scores = piece.Scores;

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
            return NotFoundTest();
        }

        private IActionResult NotFoundTest()
        {
            this.NotifyError("Can't find this test");
            return RedirectToAction(nameof(HomeController.Index), NameUtils.ControllerName<HomeController>());
        }
    }
}
