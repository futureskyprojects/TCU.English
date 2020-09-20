using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [AuthorizeRoles(UserType.ROLE_ALL, UserType.ROLE_INSTRUCTOR_USER)]
    public class InstructorController : Controller
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

        [HttpGet]
        public IActionResult YourOwnStudents(int page = 1, string searchKey = "")
        {
            int limit = 20;
            int start = (page - 1) * limit;

            long total = _PieceOfTestManager.CountOfInstructor(User.Id());
            IEnumerable<User> users = _UserManager.GetAllStudentsOfInstructor(User.Id(), start, limit);
            // Tạo đối tượng phân trang
            ViewBag.Pagination = new Pagination(nameof(Index), NameUtils.ControllerName<UserManagementController>())
            {
                PageCurrent = page,
                NumberPage = PaginationUtils.TotalPageCount(total.ToInt(), limit),
                Offset = limit
            };
            // Get data
            return View(users);
        }

        [HttpGet]
        public IActionResult Marks(int page = 1, string searchKey = "")
        {
            return View();
        }

    }
}