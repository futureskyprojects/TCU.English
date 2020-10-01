using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models;

namespace TCU.English.Controllers
{
    public partial class ReadingManagerController
    {

        #region PART 4
        public IActionResult Part4(
           int categoryPage = 1,
           string categorySearchKey = "")
        {
            var testCategories = CategoryRender(nameof(Part4), TestCategory.READING, 4, categoryPage, categorySearchKey);

            return View($"{nameof(Part4)}/Index", testCategories);
        }

        [HttpGet]
        public IActionResult Part4Create()
        {
            return View($"{nameof(Part4)}/{nameof(Part4Create)}",
                new ReadingCombined
                {
                    TestCategory = TestCategory.ReadingCategory(4),
                    ReadingPartTwos = ReadingPartTwo.Generate(Config.MAX_READING_PART_4_QUESTION)
                });
        }

        [HttpPost]
        public IActionResult Part4Create(ReadingCombined readingCombined)
        {
            return Part3Or4Processing(nameof(Part4), nameof(Part4Create), readingCombined, false);
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
                    readingPartTwos = ReadingPartTwo.Generate(Config.MAX_READING_PART_4_QUESTION);
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
            return Part3Or4Processing(nameof(Part4), nameof(Part4Update), readingCombined, false);
        }


        [HttpDelete]
        public IActionResult Part4DeleteAjax(long id) // CategoryId
        {
            return Part3Or4Delele(4, id);
        }

        #endregion

    }
}
