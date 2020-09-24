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
    public class WritingManagerController : Controller
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

        #region PART 1
        public IActionResult Part1(
           long category,
           int categoryPage = 1,
           int questionPage = 1,
           string categorySearchKey = "",
           string questionSearchKey = "")
        {
            int limit = 20;
            int categoryStart = (categoryPage - 1) * limit;
            int questionStart = (questionPage - 1) * limit;

            var testCategories = _TestCategoryManager.GetByPagination(TestCategory.WRITING, 1, categoryStart, limit);
            ViewBag.TestCategories = testCategories;

            var quesitons = new List<WritingPartOne>();

            if (category > 0)
            {
                var testCategory = _TestCategoryManager.Get(category);
                if (testCategory == null)
                {
                    return NotFound();
                }
                else
                {
                    ViewBag.QuestionType = testCategory.Name ?? "";
                    quesitons = _WritingPartOneManager.GetByPagination(category, questionStart, limit).ToList();
                }
            }
            else
            {
                ViewBag.QuestionType = "ALL";
                quesitons = _WritingPartOneManager.GetByPagination(questionStart, limit).ToList();
            }

            ViewBag.Questions = quesitons;
            // Tạo đối tượng phân trang cho Category
            ViewBag.CategoryPagination = new Pagination(nameof(Part1), NameUtils.ControllerName<WritingManagerController>())
            {
                PageKey = nameof(categoryPage),
                PageCurrent = categoryPage,
                NumberPage = PaginationUtils.TotalPageCount(testCategories.Count(), limit),
                Offset = limit
            };

            // Tạo đối tượng phân trang cho Reading Part 1
            ViewBag.QuestionPagination = new Pagination(nameof(Part1), NameUtils.ControllerName<WritingManagerController>())
            {
                PageKey = nameof(questionPage),
                PageCurrent = questionPage,
                TypeKey = nameof(category),
                Type = "0",
                NumberPage = PaginationUtils.TotalPageCount(quesitons.Count(), limit),
                Offset = limit
            };

            return View($"{nameof(Part1)}/Index");
        }

        [HttpGet]
        public IActionResult Part1Create(WritingPartOne writingPartOne)
        {
            if (writingPartOne == null)
                writingPartOne = new WritingPartOne();
            ViewBag.TestCategories = _TestCategoryManager.GetAll(TestCategory.WRITING, 1) ?? new List<TestCategory>();
            return PartialView($"{nameof(Part1)}/{nameof(Part1Create)}", writingPartOne);
        }

        [HttpPost]
        public IActionResult Part1CreateAjax(WritingPartOne writingPartOne)
        {
            if (writingPartOne.BaseAnswers != null && writingPartOne.BaseAnswers.Count > 0 && writingPartOne.BaseAnswers.Where(x => x.AnswerContent == "").Count() <= 0)
            {
                writingPartOne.Answers = JsonConvert.SerializeObject(writingPartOne.BaseAnswers);
            }
            else
            {
                return Json(new { status = false, message = "Please provide atleast one of answers" });
            }
            if (ModelState.IsValid)
            {
                if (writingPartOne.TestCategoryId <= 0)
                {
                    return Json(new { status = false, message = "Please choose a category for this question" });
                }
                if (writingPartOne.Answers != null && writingPartOne.Answers.Length > 3)
                {
                    List<BaseAnswer> answers = BaseAnswer.GetAnswers(writingPartOne.Answers);
                    if (answers != null && answers.Count > 0)
                    {
                        writingPartOne.CreatorId = User.Id();
                        _WritingPartOneManager.Add(writingPartOne);
                        return Json(new { status = true, message = "Successfully created, the list will refresh again in 1 second." });
                    }
                }
                return Json(new { status = false, message = "Unable to determine the answer to this question" });
            }
            else
            {
                string errors = "";
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        if (errors.Length > 0)
                            errors += "\r\n";
                        errors += error.ErrorMessage;
                    }
                }
                return Json(new { status = false, message = errors });
            }
        }

        [HttpGet]
        public IActionResult Part1Update(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            else
            {
                var writingPartOneQuestion = _WritingPartOneManager.Get(id);
                if (writingPartOneQuestion == null)
                    return NotFound();
                writingPartOneQuestion.BaseAnswers = JsonConvert.DeserializeObject<List<BaseAnswer>>(writingPartOneQuestion.Answers);
                ViewBag.TestCategories = _TestCategoryManager.GetAll(TestCategory.WRITING, 1) ?? new List<TestCategory>();
                ViewBag.IsShowImmediately = true;
                return PartialView($"{nameof(Part1)}/{nameof(Part1Update)}", writingPartOneQuestion);
            }
        }

        [HttpPost]
        public IActionResult Part1UpdateAjax(WritingPartOne writingPartOne)
        {
            if (writingPartOne.BaseAnswers != null && writingPartOne.BaseAnswers.Count > 0)
            {
                writingPartOne.Answers = JsonConvert.SerializeObject(writingPartOne.BaseAnswers);
            }
            else
            {
                return Json(new { status = false, message = "Please provide atleast one of answers" });
            }
            if (ModelState.IsValid)
            {
                if (writingPartOne.TestCategoryId <= 0)
                {
                    return Json(new { status = false, message = "Please choose a category for this question" });
                }
                if (writingPartOne.Answers != null && writingPartOne.Answers.Length > 3)
                {
                    List<BaseAnswer> answers = BaseAnswer.GetAnswers(writingPartOne.Answers);
                    if (answers != null && answers.Count == Config.MAX_WRITING_PART_1_QUESTION)
                    {
                        _WritingPartOneManager.Update(writingPartOne);
                        return Json(new { status = true, message = "Successfully updated, the list will refresh again in 1 second." });
                    }
                }
                return Json(new { status = false, message = "Unable to determine the answer to this question" });
            }
            else
            {
                string errors = "";
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        if (errors.Length > 0)
                            errors += "\r\n";
                        errors += error.ErrorMessage;
                    }
                }
                return Json(new { status = false, message = errors });
            }
        }

        [HttpDelete]
        public IActionResult Part1DeleteAjax(long id)
        {
            var writingPartOneQuestion = _WritingPartOneManager.Get(id);
            if (writingPartOneQuestion == null)
            {
                return Json(new { success = false, responseText = "This question was not found." });
            }
            else
            {
                _WritingPartOneManager.Delete(writingPartOneQuestion);
                return Json(new { success = true, user = JsonConvert.SerializeObject(writingPartOneQuestion), responseText = "Deleted" });
            }
        }

        #endregion

        #region PART 2
        public IActionResult Part2(
           int categoryPage = 1,
           string categorySearchKey = "")
        {
            var testCategories = CategoryRender(nameof(Part2), TestCategory.WRITING, 2, categoryPage, categorySearchKey);

            return View($"{nameof(Part2)}/Index", testCategories);
        }

        [HttpGet]
        public IActionResult Part2Create()
        {
            return View($"{nameof(Part2)}/{nameof(Part2Create)}",
                new WritingCombined
                {
                    TestCategory = TestCategory.WritingCategory(2),
                });
        }

        [HttpPost]
        public IActionResult Part2Create(WritingCombined WritingCombined)
        {
            return Part2Processing(nameof(Part2), nameof(Part2Create), WritingCombined, false);
        }

        [HttpGet]
        public IActionResult Part2Update(long id)
        {
            if (id <= 0)
                return BadRequest();

            var testCategory = _TestCategoryManager.Get(id);
            if (testCategory == null)
                return NotFound();

            var WritingPartTwo = _WritingPartTwoManager.GetByCategoryId(testCategory.Id);
            if (WritingPartTwo == null)
                return NotFound();

            return View($"{nameof(Part2)}/{nameof(Part2Update)}",
                new WritingCombined
                {
                    TestCategory = testCategory,
                    WritingPartTwo = WritingPartTwo
                });
        }

        [HttpPost]
        public IActionResult Part2Update(WritingCombined WritingCombined)
        {
            return Part2Processing(nameof(Part2), nameof(Part2Update), WritingCombined);
        }


        [HttpDelete]
        public IActionResult Part2DeleteAjax(long id) // CategoryId
        {
            return Part2Delele(2, id);
        }

        #endregion    

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
            int limit = 20;
            int categoryStart = (categoryPage - 1) * limit;

            var testCategories = _TestCategoryManager.GetByPagination(typeCode, partId, categoryStart, limit);

            // Tạo đối tượng phân trang cho Category
            ViewBag.CategoryPagination = new Pagination(actionName, NameUtils.ControllerName<WritingManagerController>())
            {
                PageKey = nameof(categoryPage),
                PageCurrent = categoryPage,
                NumberPage = PaginationUtils.TotalPageCount(
                    _TestCategoryManager.GetAll(typeCode, partId).Count(),
                    limit),
                Offset = limit
            };
            return testCategories;
        }
    }
}
