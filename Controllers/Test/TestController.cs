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

        public IActionResult Index(string type = "ALL", int page = 1, string searchKey = "", int instructorId = -1)
        {
            // Truyền gửi tên bảng
            ViewBag.TableName = type.ToUpper();
            ViewBag.SearchKey = searchKey;
            // Lấy các đếm chuẩn
            ViewBag.UserTestCountOfAll = _PieceOfTestManager.UserTestCountOfType(User.Id(), "ALL", string.Empty, instructorId);
            ViewBag.UserTestCountOfListening = _PieceOfTestManager.UserTestCountOfType(User.Id(), TestCategory.LISTENING, string.Empty, instructorId);
            ViewBag.UserTestCountOfReading = _PieceOfTestManager.UserTestCountOfType(User.Id(), TestCategory.READING, string.Empty, instructorId);
            ViewBag.UserTestCountOfSpeaking = _PieceOfTestManager.UserTestCountOfType(User.Id(), TestCategory.SPEAKING, string.Empty, instructorId);
            ViewBag.UserTestCountOfWriting = _PieceOfTestManager.UserTestCountOfType(User.Id(), TestCategory.WRITING, string.Empty, instructorId);
            ViewBag.UserTestCountOfCrash = _PieceOfTestManager.UserTestCountOfType(User.Id(), "CRASH", string.Empty, instructorId);

            // Tiến hành cấu hình phân trang
            
            int start = (page - 1) * Config.PAGE_PAGINATION_LIMIT;
            // Lấy danh sách
            IEnumerable<PieceOfTest> PieceOfTests = _PieceOfTestManager.GetByPaginationSimple(User.Id(), type, start, Config.PAGE_PAGINATION_LIMIT, searchKey, instructorId);
            // Tạo đối tượng phân trang
            ViewBag.Pagination = new Pagination(nameof(Index), NameUtils.ControllerName<TestController>())
            {
                PageCurrent = page,
                Type = type,
                NumberPage = PaginationUtils.TotalPageCount(
                    _PieceOfTestManager.UserTestCountOfType(
                        User.Id(),
                        type.ToUpper().Trim(),
                        searchKey,
                        instructorId
                    ).ToInt(),
                    Config.PAGE_PAGINATION_LIMIT
                    ),
                Offset = Config.PAGE_PAGINATION_LIMIT
            };

            // Lấy GVHD nếu có
            if (instructorId > 0)
                ViewBag.Instructor = _UserManager.Get(instructorId);

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
