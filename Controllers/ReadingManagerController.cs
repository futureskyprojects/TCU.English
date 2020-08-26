using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
        private readonly UserManager _UserManager;
        private readonly UserTypeManager _UserTypeManager;
        private readonly TestCategoryManager _TestCategoryManager;
        private readonly ReadingPartOneManager _ReadingPartOneManager;
        public ReadingManagerController(IDataRepository<User> _UserManager,
            IDataRepository<UserType> _UserTypeManager,
            IDataRepository<TestCategory> _TestCategoryManager,
            IDataRepository<ReadingPartOne> _ReadingPartOneManager)
        {
            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._ReadingPartOneManager = (ReadingPartOneManager)_ReadingPartOneManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Part1(long category,
            int categoryPage = 1,
            int questionPage = 1,
            string categorySearchKey = "",
            string questionSearchKey = "")
        {
            int limit = 20;
            int start = (page - 1) * limit;

            if (searchKey != null && searchKey.Length > 0)
            {
                type = "all"; // Nếu có tìm, thì sẽ là tìm tất cả
            }

            ViewBag.currentPageIndex = page;
            ViewBag.Type = type;

            ViewBag.TestCategories = _TestCategoryManager.GetAll(TestCategory.READING, 1);
            ViewBag.ReadingPartOneQuestions = _ReadingPartOneManager.GetAll();
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
        public IActionResult Delete(long id)
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
    }
}
