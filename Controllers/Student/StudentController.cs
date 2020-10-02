using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IHostEnvironment host;
        private readonly UserManager _UserManager;
        private readonly PieceOfTestManager _PieceOfTestManager;

        public StudentController(
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
        /// Danh sách giáo viên hướng dẫn đã chọn
        /// </summary>
        [HttpGet]
        public IActionResult YourOwnInstructor(int page = 1, string searchKey = "")
        {
            
            int start = (page - 1) * Config.PAGE_PAGINATION_LIMIT;

            long total = _PieceOfTestManager.CountAllInstructorOfStudent(User.Id());
            IEnumerable<User> users = _UserManager.GetAllInstructorsOfStudent(User.Id(), start, Config.PAGE_PAGINATION_LIMIT);
            // Tạo đối tượng phân trang
            ViewBag.Pagination = new Pagination(nameof(Index), NameUtils.ControllerName<UserManagementController>())
            {
                PageCurrent = page,
                NumberPage = PaginationUtils.TotalPageCount(total.ToInt(), Config.PAGE_PAGINATION_LIMIT),
                Offset = Config.PAGE_PAGINATION_LIMIT
            };
            // Get data
            return View(users);
        }

    }
}