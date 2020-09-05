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

        public WritingManagerController(
           IHostEnvironment _host,
           IDataRepository<User> _UserManager,
           IDataRepository<UserType> _UserTypeManager,
           IDataRepository<TestCategory> _TestCategoryManager,
           IDataRepository<WritingPartOne> _WritingPartOneManager)
        {
            host = _host;
            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._WritingPartOneManager = (WritingPartOneManager)_WritingPartOneManager;
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
                    if (answers != null && answers.Count == Config.MAX_READING_PART_1_QUESTION)
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
    }
}
