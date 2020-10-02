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

        #region PART 1

        public IActionResult Part1(
            long category,
            int categoryPage = 1,
            int questionPage = 1,
            string categorySearchKey = "",
            string questionSearchKey = "")
        {
            
            int questionStart = (questionPage - 1) * Config.PAGE_PAGINATION_LIMIT;

            ViewBag.TestCategories = CategoryRender(nameof(Part1), TestCategory.READING, 1, categoryPage, categorySearchKey);

            ViewBag.Questions = QuestionRender(nameof(Part1), TestCategory.READING, 1, questionPage, category.ToInt(), questionSearchKey);

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
            return Part1Processing(readingPartOne);
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
            return Part1Processing(readingPartOne);
        }

        [HttpDelete]
        public IActionResult Part1DeleteAjax(long id)
        {
            return Part1Or2Delete(1, id);
        }

        #endregion

    }
}
