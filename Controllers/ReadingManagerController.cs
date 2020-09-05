using System;
using System.Collections.Generic;
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
    public class ReadingManagerController : Controller
    {
        private readonly IHostEnvironment host;

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
            return PartialView($"{nameof(Part1)}/{nameof(Part1Create)}", readingPartOne);
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
                return PartialView($"{nameof(Part1)}/{nameof(Part1Update)}", readingPartOneQuestion);
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
                    readingQuestions = _ReadingPartTwoManager.GetByPagination(category, TestCategory.READING, 2, questionStart, limit).ToList();
                }
            }
            else
            {
                ViewBag.QuestionType = "ALL";
                readingQuestions = _ReadingPartTwoManager.GetByPagination(TestCategory.READING, 2, questionStart, limit).ToList();
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
            return PartialView($"{nameof(Part2)}/{nameof(Part2Create)}", readingPartTwo);
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
                            string uploadResult = await host.UploadForTestImage(questionImage, TestCategory.READING, 2);
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
                return PartialView($"{nameof(Part2)}/{nameof(Part2Update)}", readingQuestion);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Part2UpdateAjax(ReadingPartTwo readingPartTwo, IFormFile questionImage)
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
                            string uploadResult = await host.UploadForTestImage(questionImage, TestCategory.READING, 2);
                            if (uploadResult != null || uploadResult.Length > 0)
                            {
                                readingPartTwo.QuestionImage = uploadResult;
                            }
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

        #region PART 3
        public IActionResult Part3(
           int categoryPage = 1,
           string categorySearchKey = "")
        {
            int limit = 20;
            int categoryStart = (categoryPage - 1) * limit;

            var testCategories = _TestCategoryManager.GetByPagination(TestCategory.READING, 3, categoryStart, limit);

            // Tạo đối tượng phân trang cho Category
            ViewBag.CategoryPagination = new Pagination(nameof(Part3), NameUtils.ControllerName<ReadingManagerController>())
            {
                PageKey = nameof(categoryPage),
                PageCurrent = categoryPage,
                NumberPage = PaginationUtils.TotalPageCount(testCategories.Count(), limit),
                Offset = limit
            };

            return View($"{nameof(Part3)}/Index", testCategories);
        }

        [HttpGet]
        public IActionResult Part3Create()
        {
            return View($"{nameof(Part3)}/{nameof(Part3Create)}", new ReadingCombined { TestCategory = TestCategory.ReadingCategory(3), ReadingPartTwos = ReadingPartTwo.Generate(5) });
        }

        [HttpPost]
        public IActionResult Part3Create(ReadingCombined readingCombined)
        {
            ModelState.Remove(nameof(ReadingPartTwo.Answers));
            if (readingCombined != null && readingCombined.TestCategory != null &&
                readingCombined.TestCategory.Name != null && readingCombined.TestCategory.Name.Length > 0 &&
                readingCombined.TestCategory.TypeCode != null && readingCombined.TestCategory.TypeCode.Length > 0 &&
                readingCombined.TestCategory.PartId > 0 &&
                readingCombined.TestCategory.WYSIWYGContent != null && readingCombined.TestCategory.WYSIWYGContent.Length > 0 &&
                readingCombined.ReadingPartTwos.Count > 0)
            {
                // Lấy mã người tạo
                int userId = User.Id();
                // Cập nhật mã người tạo cho category
                readingCombined.TestCategory.CreatorId = userId;
                // Tiến hành kiểm tra trong các phần câu hỏi và đặt mã người tạo + convert đối tượng thành Json và gán vào ANSWERS
                for (int i = 0; i < readingCombined.ReadingPartTwos.Count; i++)
                {
                    if (readingCombined.ReadingPartTwos[i].QuestionText == null || readingCombined.ReadingPartTwos[i].QuestionText.Length <= 0)
                    {
                        ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.QuestionText)} of question {i + 1} is required.");
                        return View($"{nameof(Part3)}/{nameof(Part3Create)}", readingCombined);
                    }
                    if (readingCombined.ReadingPartTwos[i].AnswerList.Count <= 0 || readingCombined.ReadingPartTwos[i].AnswerList.Any(x => string.IsNullOrEmpty(x.AnswerContent)))
                    {
                        ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.Answers)} of question {i + 1} is required.");
                        return View($"{nameof(Part3)}/{nameof(Part3Create)}", readingCombined);
                    }
                    if (!readingCombined.ReadingPartTwos[i].AnswerList.Any(it => it.IsCorrect))
                    {
                        ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.Answers)} of question {i + 1} must have correct option.");
                        return View($"{nameof(Part3)}/{nameof(Part3Create)}", readingCombined);
                    }
                    readingCombined.ReadingPartTwos[i].CreatorId = userId;
                    readingCombined.ReadingPartTwos[i].Answers = JsonConvert.SerializeObject(readingCombined.ReadingPartTwos[i].AnswerList);
                }
                // Tiến hành thêm danh mục vào CSDL và lấy ID
                _TestCategoryManager.Add(readingCombined.TestCategory);
                if (readingCombined.TestCategory.Id > 0)
                {
                    for (int i = 0; i < readingCombined.ReadingPartTwos.Count; i++)
                    {
                        readingCombined.ReadingPartTwos[i].TestCategoryId = readingCombined.TestCategory.Id;
                        _ReadingPartTwoManager.Add(readingCombined.ReadingPartTwos[i]);
                    }
                    return RedirectToAction(nameof(Part3));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred during execution.");
                }
            }
            return View($"{nameof(Part3)}/{nameof(Part3Create)}", readingCombined);
        }

        [HttpGet]
        public IActionResult Part3Update(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            else
            {
                var testCategory = _TestCategoryManager.Get(id);
                if (testCategory == null)
                {
                    return NotFound();
                }
                var readingPartTwos = _ReadingPartTwoManager.GetAll(testCategory.Id).ToList();
                if (readingPartTwos.Count <= 0)
                {
                    readingPartTwos = ReadingPartTwo.Generate(5);
                }
                for (int i = 0; i < readingPartTwos.Count(); i++)
                {
                    if (readingPartTwos[i].Answers != null && readingPartTwos[i].Answers.Length > 0)
                    {
                        try
                        {
                            readingPartTwos[i].AnswerList = JsonConvert.DeserializeObject<List<BaseAnswer>>(readingPartTwos[i].Answers);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                return View($"{nameof(Part3)}/{nameof(Part3Update)}",
                    new ReadingCombined
                    {
                        TestCategory = testCategory,
                        ReadingPartTwos = readingPartTwos
                    });
            }
        }

        [HttpPost]
        public IActionResult Part3Update(ReadingCombined readingCombined)
        {
            ModelState.Remove(nameof(ReadingPartTwo.Answers));
            if (readingCombined != null && readingCombined.TestCategory != null &&
                readingCombined.TestCategory.Name != null && readingCombined.TestCategory.Name.Length > 0 &&
                readingCombined.TestCategory.TypeCode != null && readingCombined.TestCategory.TypeCode.Length > 0 &&
                readingCombined.TestCategory.PartId > 0 &&
                readingCombined.TestCategory.WYSIWYGContent != null && readingCombined.TestCategory.WYSIWYGContent.Length > 0 &&
                readingCombined.ReadingPartTwos.Count > 0)
            {
                // Lấy mã người tạo
                int userId = User.Id();
                // Cập nhật mã người tạo cho category
                if (readingCombined.TestCategory.CreatorId <= 0)
                    readingCombined.TestCategory.CreatorId = userId;
                // Tiến hành kiểm tra trong các phần câu hỏi và đặt mã người tạo + convert đối tượng thành Json và gán vào ANSWERS
                for (int i = 0; i < readingCombined.ReadingPartTwos.Count; i++)
                {
                    if (readingCombined.ReadingPartTwos[i].QuestionText == null || readingCombined.ReadingPartTwos[i].QuestionText.Length <= 0)
                    {
                        ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.QuestionText)} of question {i + 1} is required.");
                        return View($"{nameof(Part3)}/{nameof(Part3Update)}", readingCombined);
                    }
                    if (readingCombined.ReadingPartTwos[i].AnswerList.Count <= 0 || readingCombined.ReadingPartTwos[i].AnswerList.Any(x => string.IsNullOrEmpty(x.AnswerContent)))
                    {
                        ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.Answers)} of question {i + 1} is required.");
                        return View($"{nameof(Part3)}/{nameof(Part3Update)}", readingCombined);
                    }
                    if (!readingCombined.ReadingPartTwos[i].AnswerList.Any(it => it.IsCorrect))
                    {
                        ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.Answers)} of question {i + 1} must have correct option.");
                        return View($"{nameof(Part3)}/{nameof(Part3Update)}", readingCombined);
                    }

                    if (readingCombined.ReadingPartTwos[i].CreatorId <= 0)
                        readingCombined.ReadingPartTwos[i].CreatorId = userId;
                    string json = JsonConvert.SerializeObject(readingCombined.ReadingPartTwos[i].AnswerList);
                    if (readingCombined.ReadingPartTwos[i].Answers != null && readingCombined.ReadingPartTwos[i].Answers != json)
                        readingCombined.ReadingPartTwos[i].Answers = json;
                }
                // Tiến hành thêm danh mục vào CSDL và lấy ID
                _TestCategoryManager.Add(readingCombined.TestCategory);
                if (readingCombined.TestCategory.Id > 0)
                {
                    for (int i = 0; i < readingCombined.ReadingPartTwos.Count; i++)
                    {
                        readingCombined.ReadingPartTwos[i].TestCategoryId = readingCombined.TestCategory.Id;
                        _ReadingPartTwoManager.Add(readingCombined.ReadingPartTwos[i]);
                    }
                    return RedirectToAction(nameof(Part3));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred during execution.");
                }
            }
            return View($"{nameof(Part3)}/{nameof(Part3Update)}", readingCombined);
        }


        [HttpDelete]
        public IActionResult Part3DeleteAjax(long id) // CategoryId
        {
            var category = _TestCategoryManager.Get(id);
            if (category.TypeCode != TestCategory.READING || category.PartId != 3)
            {
                return Json(new { success = false, responseText = "You cannot perform deletion to item other than the current item." });
            }
            if (category == null)
            {
                return Json(new { success = false, responseText = "This test category was not found." });
            }
            else
            {
                _TestCategoryManager.Delete(category);
                return Json(new { success = true, category = JsonConvert.SerializeObject(category), responseText = "Deleted" });
            }
        }

        #endregion

        #region PART 4
        public IActionResult Part4(
           int categoryPage = 1,
           string categorySearchKey = "")
        {
            int limit = 20;
            int categoryStart = (categoryPage - 1) * limit;

            var testCategories = _TestCategoryManager.GetByPagination(TestCategory.READING, 4, categoryStart, limit);

            // Tạo đối tượng phân trang cho Category
            ViewBag.CategoryPagination = new Pagination(nameof(Part4), NameUtils.ControllerName<ReadingManagerController>())
            {
                PageKey = nameof(categoryPage),
                PageCurrent = categoryPage,
                NumberPage = PaginationUtils.TotalPageCount(testCategories.Count(), limit),
                Offset = limit
            };

            return View($"{nameof(Part4)}/Index", testCategories);
        }

        [HttpGet]
        public IActionResult Part4Create()
        {
            return View($"{nameof(Part4)}/{nameof(Part4Create)}", new ReadingCombined { TestCategory = TestCategory.ReadingCategory(4), ReadingPartTwos = ReadingPartTwo.Generate(10) });
        }

        [HttpPost]
        public IActionResult Part4Create(ReadingCombined readingCombined)
        {
            ModelState.Remove(nameof(ReadingPartTwo.Answers));
            if (readingCombined != null && readingCombined.TestCategory != null &&
                readingCombined.TestCategory.Name != null && readingCombined.TestCategory.Name.Length > 0 &&
                readingCombined.TestCategory.TypeCode != null && readingCombined.TestCategory.TypeCode.Length > 0 &&
                readingCombined.TestCategory.PartId > 0 &&
                readingCombined.TestCategory.WYSIWYGContent != null && readingCombined.TestCategory.WYSIWYGContent.Length > 0 &&
                readingCombined.ReadingPartTwos.Count > 0)
            {
                // Lấy mã người tạo
                int userId = User.Id();
                // Cập nhật mã người tạo cho category
                readingCombined.TestCategory.CreatorId = userId;
                // Tiến hành kiểm tra trong các phần câu hỏi và đặt mã người tạo + convert đối tượng thành Json và gán vào ANSWERS
                for (int i = 0; i < readingCombined.ReadingPartTwos.Count; i++)
                {
                    //if (readingCombined.ReadingPartTwos[i].QuestionText == null || readingCombined.ReadingPartTwos[i].QuestionText.Length <= 0)
                    //{
                    //    ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.QuestionText)} of question {i + 1} is required.");
                    //    return View($"{nameof(Part4)}/{nameof(Part4Create)}", readingCombined);
                    //}
                    if (readingCombined.ReadingPartTwos[i].AnswerList.Count <= 0 || readingCombined.ReadingPartTwos[i].AnswerList.Any(x => string.IsNullOrEmpty(x.AnswerContent)))
                    {
                        ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.Answers)} of question {i + 1} is required.");
                        return View($"{nameof(Part4)}/{nameof(Part4Create)}", readingCombined);
                    }
                    if (!readingCombined.ReadingPartTwos[i].AnswerList.Any(it => it.IsCorrect))
                    {
                        ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.Answers)} of question {i + 1} must have correct option.");
                        return View($"{nameof(Part4)}/{nameof(Part4Create)}", readingCombined);
                    }
                    readingCombined.ReadingPartTwos[i].CreatorId = userId;
                    readingCombined.ReadingPartTwos[i].Answers = JsonConvert.SerializeObject(readingCombined.ReadingPartTwos[i].AnswerList);
                }
                // Tiến hành thêm danh mục vào CSDL và lấy ID
                _TestCategoryManager.Add(readingCombined.TestCategory);
                if (readingCombined.TestCategory.Id > 0)
                {
                    for (int i = 0; i < readingCombined.ReadingPartTwos.Count; i++)
                    {
                        readingCombined.ReadingPartTwos[i].TestCategoryId = readingCombined.TestCategory.Id;
                        _ReadingPartTwoManager.Add(readingCombined.ReadingPartTwos[i]);
                    }
                    return RedirectToAction(nameof(Part4));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred during execution.");
                }
            }
            return View($"{nameof(Part4)}/{nameof(Part4Create)}", readingCombined);
        }

        [HttpGet]
        public IActionResult Part4Update(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            else
            {
                var testCategory = _TestCategoryManager.Get(id);
                if (testCategory == null)
                {
                    return NotFound();
                }
                var readingPartTwos = _ReadingPartTwoManager.GetAll(testCategory.Id).ToList();
                if (readingPartTwos.Count <= 0)
                {
                    readingPartTwos = ReadingPartTwo.Generate(10);
                }
                for (int i = 0; i < readingPartTwos.Count(); i++)
                {
                    if (readingPartTwos[i].Answers != null && readingPartTwos[i].Answers.Length > 0)
                    {
                        try
                        {
                            readingPartTwos[i].AnswerList = JsonConvert.DeserializeObject<List<BaseAnswer>>(readingPartTwos[i].Answers);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                return View($"{nameof(Part4)}/{nameof(Part4Update)}",
                    new ReadingCombined
                    {
                        TestCategory = testCategory,
                        ReadingPartTwos = readingPartTwos
                    });
            }
        }

        [HttpPost]
        public IActionResult Part4Update(ReadingCombined readingCombined)
        {
            ModelState.Remove(nameof(ReadingPartTwo.Answers));
            if (readingCombined != null && readingCombined.TestCategory != null &&
                readingCombined.TestCategory.Name != null && readingCombined.TestCategory.Name.Length > 0 &&
                readingCombined.TestCategory.TypeCode != null && readingCombined.TestCategory.TypeCode.Length > 0 &&
                readingCombined.TestCategory.PartId > 0 &&
                readingCombined.TestCategory.WYSIWYGContent != null && readingCombined.TestCategory.WYSIWYGContent.Length > 0 &&
                readingCombined.ReadingPartTwos.Count > 0)
            {
                // Lấy mã người tạo
                int userId = User.Id();
                // Cập nhật mã người tạo cho category
                if (readingCombined.TestCategory.CreatorId <= 0)
                    readingCombined.TestCategory.CreatorId = userId;
                // Tiến hành kiểm tra trong các phần câu hỏi và đặt mã người tạo + convert đối tượng thành Json và gán vào ANSWERS
                for (int i = 0; i < readingCombined.ReadingPartTwos.Count; i++)
                {
                    //if (readingCombined.ReadingPartTwos[i].QuestionText == null || readingCombined.ReadingPartTwos[i].QuestionText.Length <= 0)
                    //{
                    //    ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.QuestionText)} of question {i + 1} is required.");
                    //    return View($"{nameof(Part4)}/{nameof(Part4Update)}", readingCombined);
                    //}
                    if (readingCombined.ReadingPartTwos[i].AnswerList.Count <= 0 || readingCombined.ReadingPartTwos[i].AnswerList.Any(x => string.IsNullOrEmpty(x.AnswerContent)))
                    {
                        ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.Answers)} of question {i + 1} is required.");
                        return View($"{nameof(Part4)}/{nameof(Part4Update)}", readingCombined);
                    }
                    if (!readingCombined.ReadingPartTwos[i].AnswerList.Any(it => it.IsCorrect))
                    {
                        ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.Answers)} of question {i + 1} must have correct option.");
                        return View($"{nameof(Part4)}/{nameof(Part4Update)}", readingCombined);
                    }

                    if (readingCombined.ReadingPartTwos[i].CreatorId <= 0)
                        readingCombined.ReadingPartTwos[i].CreatorId = userId;
                    string json = JsonConvert.SerializeObject(readingCombined.ReadingPartTwos[i].AnswerList);
                    if (readingCombined.ReadingPartTwos[i].Answers != null && readingCombined.ReadingPartTwos[i].Answers != json)
                        readingCombined.ReadingPartTwos[i].Answers = json;
                }
                // Tiến hành thêm danh mục vào CSDL và lấy ID
                _TestCategoryManager.Add(readingCombined.TestCategory);
                if (readingCombined.TestCategory.Id > 0)
                {
                    for (int i = 0; i < readingCombined.ReadingPartTwos.Count; i++)
                    {
                        readingCombined.ReadingPartTwos[i].TestCategoryId = readingCombined.TestCategory.Id;
                        _ReadingPartTwoManager.Add(readingCombined.ReadingPartTwos[i]);
                    }
                    return RedirectToAction(nameof(Part4));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred during execution.");
                }
            }
            return View($"{nameof(Part4)}/{nameof(Part4Update)}", readingCombined);
        }


        [HttpDelete]
        public IActionResult Part4DeleteAjax(long id) // CategoryId
        {
            var category = _TestCategoryManager.Get(id);
            if (category.TypeCode != TestCategory.READING || category.PartId != 4)
            {
                return Json(new { success = false, responseText = "You cannot perform deletion to item other than the current item." });
            }
            if (category == null)
            {
                return Json(new { success = false, responseText = "This test category was not found." });
            }
            else
            {
                _TestCategoryManager.Delete(category);
                return Json(new { success = true, category = JsonConvert.SerializeObject(category), responseText = "Deleted" });
            }
        }

        #endregion
    }
}
