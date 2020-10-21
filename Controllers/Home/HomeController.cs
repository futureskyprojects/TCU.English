using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private PieceOfTestManager _PieceOfTestManager;
        public HomeController(IDataRepository<PieceOfTest> _PieceOfTestManager)
        {
            this._PieceOfTestManager = (PieceOfTestManager)_PieceOfTestManager;
        }
        public IActionResult Index()
        {
            long userId = User.Id();
            if (userId < 0)
            {
                return NotFound();
            }
            ViewBag.CompletedTests = _PieceOfTestManager.CompletedTestsCount(userId);
            ViewBag.PassedTests = _PieceOfTestManager.PassedTestsCount(userId);
            ViewBag.FaildTests = _PieceOfTestManager.FaildTestsCount(userId);

            float ListeningGPA = _PieceOfTestManager.CalculateGPA(userId, TestCategory.LISTENING);
            float ReadingGPA = _PieceOfTestManager.CalculateGPA(userId, TestCategory.READING);
            float WritingGPA = _PieceOfTestManager.CalculateGPA(userId, TestCategory.WRITING);
            float SpeakingGPA = _PieceOfTestManager.CalculateGPA(userId, TestCategory.SPEAKING);

            ViewBag.ListeningGPA = ListeningGPA.ToScores();
            ViewBag.ReadingGPA = ReadingGPA.ToScores();
            ViewBag.WritingGPA = WritingGPA.ToScores();
            ViewBag.SpeakingGPA = SpeakingGPA.ToScores();

            ViewBag.GFailed = _PieceOfTestManager.FaildTestsCount(userId, TestCategory.TEST_ALL);
            ViewBag.GSuccess = _PieceOfTestManager.PassedTestsCount(userId, TestCategory.TEST_ALL);
            ViewBag.GHighest = _PieceOfTestManager.HightestScore(userId, TestCategory.TEST_ALL);

            ViewBag.YourGPA = (ListeningGPA + ReadingGPA + WritingGPA + SpeakingGPA).ToScores();

            return View();
        }
    }
}
