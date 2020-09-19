using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        private readonly IHostEnvironment host;

        private readonly UserManager _UserManager;
        private readonly TestCategoryManager _TestCategoryManager;
        private readonly ReadingPartOneManager _ReadingPartOneManager;
        private readonly ReadingPartTwoManager _ReadingPartTwoManager;
        private readonly ListeningBaseQuestionManager _ListeningBaseQuestionManager;
        private readonly WritingPartOneManager _WritingPartOneManager;

        private readonly PieceOfTestManager _PieceOfTestManager;

        public TestController(
          IHostEnvironment _host,
          IDataRepository<User> _UserManager,
          IDataRepository<TestCategory> _TestCategoryManager,
          IDataRepository<ReadingPartOne> _ReadingPartOneManager,
          IDataRepository<ReadingPartTwo> _ReadingPartTwoManager,
          IDataRepository<ListeningBaseQuestion> _ListeningBaseQuestionManager,
          IDataRepository<WritingPartOne> _WritingPartOneManager,
          IDataRepository<PieceOfTest> _PieceOfTestManager)
        {
            host = _host;
            this._UserManager = (UserManager)_UserManager;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._ReadingPartOneManager = (ReadingPartOneManager)_ReadingPartOneManager;
            this._ReadingPartTwoManager = (ReadingPartTwoManager)_ReadingPartTwoManager;
            this._ListeningBaseQuestionManager = (ListeningBaseQuestionManager)_ListeningBaseQuestionManager;
            this._WritingPartOneManager = (WritingPartOneManager)_WritingPartOneManager;
            this._PieceOfTestManager = (PieceOfTestManager)_PieceOfTestManager;
        }

        public IActionResult Index(string type = "ALL", int page = 1, string searchKey = "")
        {
            // Truyền gửi tên bảng
            ViewBag.TableName = type.ToUpper();
            ViewBag.SearchKey = searchKey;
            // Lấy các đếm chuẩn
            ViewBag.UserTestCountOfAll = _PieceOfTestManager.UserTestCountOfType(User.Id(), "ALL");
            ViewBag.UserTestCountOfListening = _PieceOfTestManager.UserTestCountOfType(User.Id(), TestCategory.LISTENING);
            ViewBag.UserTestCountOfReading = _PieceOfTestManager.UserTestCountOfType(User.Id(), TestCategory.READING);
            ViewBag.UserTestCountOfSpeaking = _PieceOfTestManager.UserTestCountOfType(User.Id(), TestCategory.SPEAKING);
            ViewBag.UserTestCountOfWriting = _PieceOfTestManager.UserTestCountOfType(User.Id(), TestCategory.WRITING);
            ViewBag.UserTestCountOfCrash = _PieceOfTestManager.UserTestCountOfType(User.Id(), "CRASH");

            // Tiến hành cấu hình phân trang
            int limit = 20;
            int start = (page - 1) * limit;
            // Lấy danh sách
            IEnumerable<PieceOfTest> PieceOfTests = _PieceOfTestManager.GetByPagination(User.Id(), type, start, limit, searchKey);
            // Tạo đối tượng phân trang
            ViewBag.Pagination = new Pagination(nameof(Index), NameUtils.ControllerName<TestController>())
            {
                PageCurrent = page,
                Type = type,
                NumberPage = PaginationUtils.TotalPageCount(
                    _PieceOfTestManager.UserTestCountOfType(
                        User.Id(),
                        type.ToUpper().Trim(),
                        searchKey
                    ).ToInt(),
                    limit
                    ),
                Offset = limit
            };
            return View(PieceOfTests);
        }

        public IActionResult NewTest()
        {
            ViewBag.ListeningQuestionCount = _TestCategoryManager.ListeningQuestionCount();
            ViewBag.ReadingQuestionCount = _TestCategoryManager.ReadingQuestionCount();
            ViewBag.SpeakingQuestionCount = _TestCategoryManager.SpeakingQuestionCount();
            ViewBag.WritingQuestionCount = _TestCategoryManager.WritingQuestionCount();

            // Lấy danh sách GV Hướng dẫn
            ViewBag.Instructors = _UserManager.GetAllInstructors();

            return View();
        }
    }
}
