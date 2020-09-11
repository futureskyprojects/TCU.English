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

            ViewBag.ListeningGPA = _PieceOfTestManager.CalculateGPA(userId, TestCategory.LISTENING).ToString("0.0");
            ViewBag.ReadingGPA = _PieceOfTestManager.CalculateGPA(userId, TestCategory.READING).ToString("0.0");
            ViewBag.WritingGPA = _PieceOfTestManager.CalculateGPA(userId, TestCategory.WRITING).ToString("0.0");

            ViewBag.YourGPA = _PieceOfTestManager.CalculateGPA(userId).ToString("0.0");

            return View();
        }
    }
}
