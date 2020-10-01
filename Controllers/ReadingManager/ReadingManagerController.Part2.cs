using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    public partial class ReadingManagerController
    {

        #region PART 2
        public IActionResult Part2(
           long category,
           int categoryPage = 1,
           int questionPage = 1,
           string categorySearchKey = "",
           string questionSearchKey = "")
        {
            ViewBag.TestCategories = CategoryRender(nameof(Part2), TestCategory.READING, 2, categoryPage, categorySearchKey);

            ViewBag.Questions = QuestionRender(nameof(Part2), TestCategory.READING, 2, questionPage, category.ToInt(), questionSearchKey);

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
            return await Part2Processing(readingPartTwo, questionImage);
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
            return await Part2Processing(readingPartTwo, questionImage);
        }

        [HttpDelete]
        public IActionResult Part2DeleteAjax(long id)
        {
            return Part1Or2Delete(2, id);
        }

        #endregion

    }
}
