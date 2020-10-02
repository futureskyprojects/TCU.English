using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [AuthorizeRoles(UserType.ROLE_ALL, UserType.ROLE_MANAGER_LIBRARY)]
    public partial class WritingManagerController : Controller
    {
        private readonly IHostEnvironment host;

        private readonly UserManager _UserManager;
        private readonly UserTypeManager _UserTypeManager;
        private readonly TestCategoryManager _TestCategoryManager;
        private readonly WritingPartOneManager _WritingPartOneManager;
        private readonly WritingPartTwoManager _WritingPartTwoManager;

        public WritingManagerController(
           IHostEnvironment _host,
           IDataRepository<User> _UserManager,
           IDataRepository<UserType> _UserTypeManager,
           IDataRepository<TestCategory> _TestCategoryManager,
           IDataRepository<WritingPartOne> _WritingPartOneManager,
           IDataRepository<WritingPartTwo> _WritingPartTwoManager)
        {
            host = _host;
            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._WritingPartOneManager = (WritingPartOneManager)_WritingPartOneManager;
            this._WritingPartTwoManager = (WritingPartTwoManager)_WritingPartTwoManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        //==========================================================//
        private bool IsValidate(WritingCombined WritingCombined)
        {
            if (!(WritingCombined != null && WritingCombined.TestCategory != null &&
                WritingCombined.TestCategory.TypeCode != null && WritingCombined.TestCategory.TypeCode.Length > 0 &&
                WritingCombined.TestCategory.PartId > 0 &&
                WritingCombined.TestCategory.WYSIWYGContent != null && WritingCombined.TestCategory.WYSIWYGContent.Length > 0 &&
                WritingCombined.WritingPartTwo.Questions != null &&
                WritingCombined.WritingPartTwo.Questions.Length > 0))
                return false;

            // Lấy mã người tạo
            int userId = User.Id();

            // Cập nhật mã người tạo cho category
            if (WritingCombined.TestCategory.CreatorId <= 0)
                WritingCombined.TestCategory.CreatorId = userId;

            if (WritingCombined.WritingPartTwo.CreatorId <= 0)
                WritingCombined.WritingPartTwo.CreatorId = userId;

            return true;
        }
        private IActionResult Part2Processing(string partName, string partAction, WritingCombined WritingCombined, bool isCheckQuestionText = true)
        {
            var view = View($"{partName}/{partAction}", WritingCombined);
            // Nếu không hợp lệ
            if (!IsValidate(WritingCombined))
            {
                ModelState.AddModelError(string.Empty, "Please enter the full information of the required item");
                return view;
            }

            // Tiến hành thêm danh mục vào CSDL và lấy ID
            if (WritingCombined.TestCategory.Id <= 0)
                _TestCategoryManager.Add(WritingCombined.TestCategory);
            else
                _TestCategoryManager.Update(WritingCombined.TestCategory);

            if (WritingCombined.TestCategory.Id <= 0)
            {
                ModelState.AddModelError(string.Empty, "An error occurred during execution.");
                return view;
            }

            // Cập nhật ID danh mục cho bài viết 2
            if (WritingCombined.WritingPartTwo.TestCategoryId <= 0)
                WritingCombined.WritingPartTwo.TestCategoryId = WritingCombined.TestCategory.Id;

            // Kiểm tra và cập nhật vào CSDL
            if (WritingCombined.WritingPartTwo.Id <= 0)
                _WritingPartTwoManager.Add(WritingCombined.WritingPartTwo);
            else
                _WritingPartTwoManager.Update(WritingCombined.WritingPartTwo);

            this.NotifySuccess("Update completed!");
            // Trả về
            return RedirectToAction(partName);
        }
        private IActionResult Part2Delele(int partId, long id)
        {
            var category = _TestCategoryManager.Get(id);
            if (category.TypeCode != TestCategory.WRITING || category.PartId != partId)
            {
                return Json(new { success = false, responseText = "You cannot perform deletion to item other than the current item." });
            }
            if (category == null)
            {
                return Json(new { success = false, responseText = "This category was not found." });
            }
            else
            {
                _TestCategoryManager.Delete(category);
                return Json(new { success = true, category = JsonConvert.SerializeObject(category), responseText = "Deleted" });
            }
        }

        private IEnumerable<TestCategory> CategoryRender(string actionName, string typeCode, int partId, int categoryPage = 1, string categorySearchKey = "")
        {
            
            int categoryStart = (categoryPage - 1) * Config.PAGE_PAGINATION_LIMIT;

            var testCategories = _TestCategoryManager.GetByPagination(typeCode, partId, categoryStart, Config.PAGE_PAGINATION_LIMIT);

            // Tạo đối tượng phân trang cho Category
            ViewBag.CategoryPagination = new Pagination(actionName, NameUtils.ControllerName<WritingManagerController>())
            {
                PageKey = nameof(categoryPage),
                PageCurrent = categoryPage,
                NumberPage = PaginationUtils.TotalPageCount(
                    _TestCategoryManager.GetAll(typeCode, partId).Count(),
                    Config.PAGE_PAGINATION_LIMIT),
                Offset = Config.PAGE_PAGINATION_LIMIT
            };
            return testCategories;
        }
    }
}
