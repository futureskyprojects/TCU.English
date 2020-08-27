using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [AuthorizeRoles(UserType.ROLE_ALL, UserType.ROLE_MANAGER_LIBRARY)]
    public class ReadingManagerController : Controller
    {
        private readonly IHostEnvironment host;
        private readonly string PATH_ROOT;

        private readonly UserManager _UserManager;
        private readonly UserTypeManager _UserTypeManager;
        private readonly TestCategoryManager _TestCategoryManager;
        private readonly ReadingPartOneManager _ReadingPartOneManager;
        private readonly ReadingPartTwoManager _ReadingPartTwoManager;
        public ReadingManagerController(
            IHostEnvironment _host,
            IDataRepository<User> _UserManager,
            IDataRepository<UserType> _UserTypeManager,
            IDataRepository<TestCategory> _TestCategoryManager,
            IDataRepository<ReadingPartOne> _ReadingPartOneManager,
            IDataRepository<ReadingPartTwo> _ReadingPartTwoManager)
        {
            this.host = _host;
            this.PATH_ROOT = Path.GetDirectoryName(host.ContentRootPath);

            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._ReadingPartOneManager = (ReadingPartOneManager)_ReadingPartOneManager;
            this._ReadingPartTwoManager = (ReadingPartTwoManager)_ReadingPartTwoManager;
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

            var testCategories = _TestCategoryManager.GetByPagination(TestCategory.READING, 1, categoryStart, limit);
            ViewBag.TestCategories = testCategories;

            var readingPartOneQuestions = new List<ReadingPartOne>();

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
                    readingPartOneQuestions = _ReadingPartOneManager.GetByPagination(category, questionStart, limit).ToList();
                }
            }
            else
            {
                ViewBag.QuestionType = "ALL";
                readingPartOneQuestions = _ReadingPartOneManager.GetByPagination(questionStart, limit).ToList();
            }

            ViewBag.ReadingPartOneQuestions = readingPartOneQuestions;
            // Tạo đối tượng phân trang cho Category
            ViewBag.CategoryPagination = new Pagination(nameof(Part1), NameUtils.ControllerName<ReadingManagerController>())
            {
                PageKey = nameof(categoryPage),
                PageCurrent = categoryPage,
                NumberPage = PaginationUtils.TotalPageCount(testCategories.Count(), limit),
                Offset = limit
            };

            // Tạo đối tượng phân trang cho Reading Part 1
            ViewBag.ReadingPartOnePagination = new Pagination(nameof(Part1), NameUtils.ControllerName<ReadingManagerController>())
            {
                PageKey = nameof(questionPage),
                PageCurrent = questionPage,
                TypeKey = nameof(category),
                Type = "0",
                NumberPage = PaginationUtils.TotalPageCount(readingPartOneQuestions.Count(), limit),
                Offset = limit
            };

            return View($"{nameof(Part1)}/Index");
        }

        [HttpGet]
        public IActionResult Part1Create(ReadingPartOne readingPartOne)
        {
            if (readingPartOne == null)
                readingPartOne = new ReadingPartOne();
            ViewBag.TestCategories = _TestCategoryManager.GetAll(TestCategory.READING, 1) ?? new List<TestCategory>();
            return PartialView($"{nameof(Part1)}/Part1Create", readingPartOne);
        }

        [HttpPost]
        public IActionResult Part1CreateAjax(ReadingPartOne readingPartOne)
        {
            if (ModelState.IsValid)
            {
                if (readingPartOne.TestCategoryId <= 0)
                {
                    return Json(new { status = false, message = "Please choose a category for this question" });
                }
                if (readingPartOne.Answers != null && readingPartOne.Answers.Length > 10)
                {
                    List<BaseAnswer> answers = BaseAnswer.GetAnswers(readingPartOne.Answers);
                    if (answers != null && answers.Count == Config.MAX_READING_PART_1_QUESTION)
                    {
                        bool isFullAnswer = true;
                        bool isHaveCorrectAnswer = false;

                        for (int i = 0; i < answers.Count; i++)
                        {
                            if (answers.ElementAt(i).AnswerContent.Length <= 0)
                            {
                                isFullAnswer = false;
                                break;
                            }
                            if (answers.ElementAt(i).IsCorrect)
                            {
                                if (isHaveCorrectAnswer)
                                {
                                    return Json(new { status = false, message = "There cannot be more than one correct answer" });
                                }
                                isHaveCorrectAnswer = true;
                            }
                        }

                        if (isFullAnswer && isHaveCorrectAnswer)
                        {
                            readingPartOne.CreatorId = User.Id();
                            _ReadingPartOneManager.Add(readingPartOne);
                            return Json(new { status = true, message = "Successfully created, the list will refresh again in 1 second." });
                        }
                        else if (!isFullAnswer)
                        {
                            return Json(new { status = false, message = "Please provide a full range of answers" });
                        }
                        else
                        {
                            return Json(new { status = false, message = "Please determine the correct answer for the answer" });
                        }
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
                var readingPartOneQuestion = _ReadingPartOneManager.Get(id);
                if (readingPartOneQuestion == null)
                    return NotFound();
                ViewBag.TestCategories = _TestCategoryManager.GetAll(TestCategory.READING, 1) ?? new List<TestCategory>();
                ViewBag.IsShowImmediately = true;
                return PartialView($"{nameof(Part1)}/Part1Update", readingPartOneQuestion);
            }
        }

        [HttpPost]
        public IActionResult Part1UpdateAjax(ReadingPartOne readingPartOne)
        {
            if (ModelState.IsValid)
            {
                if (readingPartOne.TestCategoryId <= 0)
                {
                    return Json(new { status = false, message = "Please choose a category for this question" });
                }
                if (readingPartOne.Answers != null && readingPartOne.Answers.Length > 10)
                {
                    List<BaseAnswer> answers = BaseAnswer.GetAnswers(readingPartOne.Answers);
                    if (answers != null && answers.Count == Config.MAX_READING_PART_1_QUESTION)
                    {
                        bool isFullAnswer = true;
                        bool isHaveCorrectAnswer = false;

                        for (int i = 0; i < answers.Count; i++)
                        {
                            if (answers.ElementAt(i).AnswerContent.Length <= 0)
                            {
                                isFullAnswer = false;
                                break;
                            }
                            if (answers.ElementAt(i).IsCorrect)
                            {
                                if (isHaveCorrectAnswer)
                                {
                                    return Json(new { status = false, message = "There cannot be more than one correct answer" });
                                }
                                isHaveCorrectAnswer = true;
                            }
                        }

                        if (isFullAnswer && isHaveCorrectAnswer)
                        {
                            _ReadingPartOneManager.Update(readingPartOne);
                            return Json(new { status = true, message = "Successfully updated, the list will refresh again in 1 second." });
                        }
                        else if (!isFullAnswer)
                        {
                            return Json(new { status = false, message = "Please provide a full range of answers" });
                        }
                        else
                        {
                            return Json(new { status = false, message = "Please determine the correct answer for the answer" });
                        }
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
            var readingPartOneQuestion = _ReadingPartOneManager.Get(id);
            if (readingPartOneQuestion == null)
            {
                return Json(new { success = false, responseText = "This question was not found." });
            }
            else
            {
                _ReadingPartOneManager.Delete(readingPartOneQuestion);
                return Json(new { success = true, user = JsonConvert.SerializeObject(readingPartOneQuestion), responseText = "Deleted" });
            }
        }

        #endregion

        #region PART 2
        public IActionResult Part2(
           long category,
           int categoryPage = 1,
           int questionPage = 1,
           string categorySearchKey = "",
           string questionSearchKey = "")
        {
            int limit = 20;
            int categoryStart = (categoryPage - 1) * limit;
            int questionStart = (questionPage - 1) * limit;

            var testCategories = _TestCategoryManager.GetByPagination(TestCategory.READING, 2, categoryStart, limit);
            ViewBag.TestCategories = testCategories;

            var readingQuestions = new List<ReadingPartTwo>();

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
                    readingQuestions = _ReadingPartTwoManager.GetByPagination(category, questionStart, limit).ToList();
                }
            }
            else
            {
                ViewBag.QuestionType = "ALL";
                readingQuestions = _ReadingPartTwoManager.GetByPagination(questionStart, limit).ToList();
            }

            ViewBag.ReadingPartTwos = readingQuestions;
            // Tạo đối tượng phân trang cho Category
            ViewBag.CategoryPagination = new Pagination(nameof(Part2), NameUtils.ControllerName<ReadingManagerController>())
            {
                PageKey = nameof(categoryPage),
                PageCurrent = categoryPage,
                NumberPage = PaginationUtils.TotalPageCount(testCategories.Count(), limit),
                Offset = limit
            };

            // Tạo đối tượng phân trang cho Reading Part 1
            ViewBag.ReadingPagination = new Pagination(nameof(Part2), NameUtils.ControllerName<ReadingManagerController>())
            {
                PageKey = nameof(questionPage),
                PageCurrent = questionPage,
                TypeKey = nameof(category),
                Type = "0",
                NumberPage = PaginationUtils.TotalPageCount(readingQuestions.Count(), limit),
                Offset = limit
            };

            return View($"{nameof(Part2)}/Index");
        }

        [HttpGet]
        public IActionResult Part2Create(ReadingPartTwo readingPartTwo)
        {
            if (readingPartTwo == null)
                readingPartTwo = new ReadingPartTwo();
            ViewBag.TestCategories = _TestCategoryManager.GetAll(TestCategory.READING, 2) ?? new List<TestCategory>();
            return PartialView($"{nameof(Part2)}/Part2Create", readingPartTwo);
        }

        [HttpPost]
        public async Task<IActionResult> Part2CreateAjax(ReadingPartTwo readingPartTwo, IFormFile questionImage)
        {
            if (ModelState.IsValid)
            {
                if (readingPartTwo.TestCategoryId <= 0)
                {
                    return Json(new { status = false, message = "Please choose a category for this question" });
                }
                if (readingPartTwo.Answers != null && readingPartTwo.Answers.Length > 10)
                {
                    List<BaseAnswer> answers = BaseAnswer.GetAnswers(readingPartTwo.Answers);
                    if (answers != null && answers.Count == Config.MAX_READING_PART_2_QUESTION)
                    {
                        bool isFullAnswer = true;
                        bool isHaveCorrectAnswer = false;

                        for (int i = 0; i < answers.Count; i++)
                        {
                            if (answers.ElementAt(i).AnswerContent.Length <= 0)
                            {
                                isFullAnswer = false;
                                break;
                            }
                            if (answers.ElementAt(i).IsCorrect)
                            {
                                if (isHaveCorrectAnswer)
                                {
                                    return Json(new { status = false, message = "There cannot be more than one correct answer" });
                                }
                                isHaveCorrectAnswer = true;
                            }
                        }

                        if (isFullAnswer && isHaveCorrectAnswer)
                        {
                            string uploadResult = await host.UploadForTestMedia(questionImage, TestCategory.READING, 2);
                            if (uploadResult == null || uploadResult.Length <= 0)
                            {
                                return Json(new { status = false, message = "Please check the picture of the question again" });
                            }
                            else
                            {
                                readingPartTwo.QuestionImage = uploadResult;
                                readingPartTwo.CreatorId = User.Id();
                                _ReadingPartTwoManager.Add(readingPartTwo);
                                return Json(new { status = true, message = "Successfully created, the list will refresh again in 1 second." });
                            }
                        }
                        else if (!isFullAnswer)
                        {
                            return Json(new { status = false, message = "Please provide a full range of answers" });
                        }
                        else
                        {
                            return Json(new { status = false, message = "Please determine the correct answer for the answer" });
                        }
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
        public IActionResult Part2Update(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            else
            {
                var readingQuestion = _ReadingPartTwoManager.Get(id);
                if (readingQuestion == null)
                    return NotFound();
                ViewBag.TestCategories = _TestCategoryManager.GetAll(TestCategory.READING, 2) ?? new List<TestCategory>();
                ViewBag.IsShowImmediately = true;
                return PartialView($"{nameof(Part2)}/Part2Update", readingQuestion);
            }
        }

        [HttpPost]
        public IActionResult Part2UpdateAjax(ReadingPartTwo readingPartTwo)
        {
            if (ModelState.IsValid)
            {
                if (readingPartTwo.TestCategoryId <= 0)
                {
                    return Json(new { status = false, message = "Please choose a category for this question" });
                }
                if (readingPartTwo.Answers != null && readingPartTwo.Answers.Length > 10)
                {
                    List<BaseAnswer> answers = BaseAnswer.GetAnswers(readingPartTwo.Answers);
                    if (answers != null && answers.Count == Config.MAX_READING_PART_2_QUESTION)
                    {
                        bool isFullAnswer = true;
                        bool isHaveCorrectAnswer = false;

                        for (int i = 0; i < answers.Count; i++)
                        {
                            if (answers.ElementAt(i).AnswerContent.Length <= 0)
                            {
                                isFullAnswer = false;
                                break;
                            }
                            if (answers.ElementAt(i).IsCorrect)
                            {
                                if (isHaveCorrectAnswer)
                                {
                                    return Json(new { status = false, message = "There cannot be more than one correct answer" });
                                }
                                isHaveCorrectAnswer = true;
                            }
                        }

                        if (isFullAnswer && isHaveCorrectAnswer)
                        {
                            _ReadingPartTwoManager.Update(readingPartTwo);
                            return Json(new { status = true, message = "Successfully updated, the list will refresh again in 1 second." });
                        }
                        else if (!isFullAnswer)
                        {
                            return Json(new { status = false, message = "Please provide a full range of answers" });
                        }
                        else
                        {
                            return Json(new { status = false, message = "Please determine the correct answer for the answer" });
                        }
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
        public IActionResult Part2DeleteAjax(long id)
        {
            var readingQuestion = _ReadingPartTwoManager.Get(id);
            if (readingQuestion == null)
            {
                return Json(new { success = false, responseText = "This question was not found." });
            }
            else
            {
                _ReadingPartTwoManager.Delete(readingQuestion);
                return Json(new { success = true, user = JsonConvert.SerializeObject(readingQuestion), responseText = "Deleted" });
            }
        }

        #endregion
    }
}
