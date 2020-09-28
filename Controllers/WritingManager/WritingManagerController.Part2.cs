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
    public partial class WritingManagerController
    {
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

    }
}
