using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;

namespace TCU.English.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        private readonly IHostEnvironment host;

        private readonly TestCategoryManager _TestCategoryManager;
        private readonly ReadingPartOneManager _ReadingPartOneManager;
        private readonly ReadingPartTwoManager _ReadingPartTwoManager;
        private readonly ListeningBaseQuestionManager _ListeningBaseQuestionManager;
        private readonly WritingPartOneManager _WritingPartOneManager;

        public TestController(
          IHostEnvironment _host,
          IDataRepository<TestCategory> _TestCategoryManager,
          IDataRepository<ReadingPartOne> _ReadingPartOneManager,
          IDataRepository<ReadingPartTwo> _ReadingPartTwoManager,
          IDataRepository<ListeningBaseQuestion> _ListeningBaseQuestionManager,
          IDataRepository<WritingPartOne> _WritingPartOneManager)
        {
            host = _host;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._ReadingPartOneManager = (ReadingPartOneManager)_ReadingPartOneManager;
            this._ReadingPartTwoManager = (ReadingPartTwoManager)_ReadingPartTwoManager;
            this._ListeningBaseQuestionManager = (ListeningBaseQuestionManager)_ListeningBaseQuestionManager;
            this._WritingPartOneManager = (WritingPartOneManager)_WritingPartOneManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewTest()
        {
            ViewBag.ListeningQuestionCount = _TestCategoryManager.ListeningQuestionCount();
            ViewBag.ReadingQuestionCount = _TestCategoryManager.ReadingQuestionCount();
            ViewBag.SpeakingQuestionCount = _TestCategoryManager.SpeakingQuestionCount();
            ViewBag.WritingQuestionCount = _TestCategoryManager.WritingQuestionCount();
            return View();
        }
    }
}
