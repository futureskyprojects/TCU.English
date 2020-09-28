using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    public partial class WritingManagerController
    {
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
            return Part1Processing(writingPartOne);
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
            return Part1Processing(writingPartOne);
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


        private IActionResult Part1Processing(WritingPartOne writingPartOne)
        {
            // Nếu chưa có câu trả lời
            if (writingPartOne.BaseAnswers == null || writingPartOne.BaseAnswers.Count <= 0)
                return Json(new { status = false, message = "Please provide atleast one of answers" });

            // Nếu dữ liệu nhập vào không hợp lệ
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "Data invalid" });

            // Nếu chưa chọn mục cha
            if (writingPartOne.TestCategoryId <= 0)
                return Json(new { status = false, message = "Please choose a category for this question" });

            // Chuyển dữ liệu thành JSON và lưu vào
            writingPartOne.Answers = JsonConvert.SerializeObject(writingPartOne.BaseAnswers);

            // Nếu không có bất kỳ câu trả lời nào
            if (string.IsNullOrEmpty(writingPartOne.Answers))
                return Json(new { status = false, message = "Unable to determine the answer to this question" });

            // Cập nhật mã người tạo nếu chưa có
            if (writingPartOne.CreatorId <= 0)
                writingPartOne.CreatorId = User.Id();

            // Đầy đủ thì thêm vào CSDL
            if (writingPartOne.Id <= 0)
                _WritingPartOneManager.Add(writingPartOne);
            else
                _WritingPartOneManager.Update(writingPartOne);

            // Trả về kết quả
            return Json(new { status = true, message = "Successfully created, the list will refresh again in 1 second." });
        }
    }
}
