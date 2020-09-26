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
    [AuthorizeRoles(UserType.ROLE_ALL, UserType.ROLE_INSTRUCTOR_USER)]
    public partial class InstructorController : Controller
    {
        private readonly IHostEnvironment host;
        private readonly UserManager _UserManager;
        private readonly PieceOfTestManager _PieceOfTestManager;

        public InstructorController(
            IHostEnvironment _host,
            IDataRepository<User> _UserManager,
            IDataRepository<PieceOfTest> _PieceOfTestManager)
        {
            this.host = _host;
            this._UserManager = (UserManager)_UserManager;
            this._PieceOfTestManager = (PieceOfTestManager)_PieceOfTestManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return NotFound();
        }

        /// <summary>
        /// Danh sách học viên đã chọn GVHD là tài khoản hiện tại
        /// </summary>
        [HttpGet]
        public IActionResult YourOwnStudents(int page = 1, string searchKey = "")
        {
            int limit = 20;
            int start = (page - 1) * limit;

            ViewBag.SearchKey = searchKey;

            long total = _PieceOfTestManager.CountAllStudentOfInstructor(User.Id());
            IEnumerable<User> users = _UserManager.GetAllStudentsOfInstructor(User.Id(), start, limit, searchKey);
            // Tạo đối tượng phân trang
            ViewBag.Pagination = new Pagination(nameof(Index), NameUtils.ControllerName<UserManagementController>())
            {
                PageCurrent = page,
                NumberPage = PaginationUtils.TotalPageCount(total.ToInt(), limit),
                Offset = limit
            };
            return View(users);
        }

        /// <summary>
        /// Danh sách các bài chấm điểm
        /// </summary>
        [HttpGet]
        public IActionResult Marks(int page = 1, string searchKey = "")
        {
            return View();
        }

        /// <summary>
        /// Danh sách các bài sinh viên chia sẻ đến tài khoản hiện tại
        /// </summary>
        [HttpGet]
        public IActionResult StudentTest(int studentId = -1, string type = "ALL", int page = 1, string searchKey = "")
        {
            // Truyền gửi tên bảng
            ViewBag.TableName = type.ToUpper();
            ViewBag.SearchKey = searchKey;
            // Lấy các đếm chuẩn
            ViewBag.UserTestCountOfAll = _PieceOfTestManager.StudentTestCountOfType(User.Id(), "ALL", string.Empty, studentId);
            ViewBag.UserTestCountOfListening = _PieceOfTestManager.StudentTestCountOfType(User.Id(), TestCategory.LISTENING, string.Empty, studentId);
            ViewBag.UserTestCountOfReading = _PieceOfTestManager.StudentTestCountOfType(User.Id(), TestCategory.READING, string.Empty, studentId);
            ViewBag.UserTestCountOfSpeaking = _PieceOfTestManager.StudentTestCountOfType(User.Id(), TestCategory.SPEAKING, string.Empty, studentId);
            ViewBag.UserTestCountOfWriting = _PieceOfTestManager.StudentTestCountOfType(User.Id(), TestCategory.WRITING, string.Empty, studentId);
            ViewBag.UserTestCountOfCrash = _PieceOfTestManager.StudentTestCountOfType(User.Id(), "CRASH", string.Empty, studentId);

            // Tiến hành cấu hình phân trang
            int limit = 20;
            int start = (page - 1) * limit;
            // Lấy danh sách
            List<PieceOfTest> PieceOfTests = _PieceOfTestManager.GetByPaginationSimpleForInstructor(User.Id(), type, start, limit, searchKey, studentId).ToList();

            // Lấy một số thông tin cần thiết của User
            for (int i = 0; i < PieceOfTests.Count(); i++)
            {
                var tempUser = _UserManager.Get(PieceOfTests[i].UserId);
                PieceOfTests[i].User = new User
                {
                    Avatar = tempUser.Avatar,
                    FirstName = tempUser.FirstName,
                    LastName = tempUser.LastName
                };
            }

            // Tạo đối tượng phân trang
            ViewBag.Pagination = new Pagination(nameof(Index), NameUtils.ControllerName<TestController>())
            {
                PageCurrent = page,
                Type = type,
                NumberPage = PaginationUtils.TotalPageCount(
                    _PieceOfTestManager.StudentTestCountOfType(
                        User.Id(),
                        type.ToUpper().Trim(),
                        searchKey,
                        studentId
                    ).ToInt(),
                    limit
                    ),
                Offset = limit
            };

            // Lấy học viên nếu được chọn cụ thể
            if (studentId > 0)
                ViewBag.Student = _UserManager.Get(studentId);


            return View(PieceOfTests);
        }

        /// <summary>
        /// Danh sách các cuộc thảo luận
        /// </summary>
        [HttpGet]
        public IActionResult Discuss(int page = 1, string searchKey = "")
        {
            return View();
        }

    }
}