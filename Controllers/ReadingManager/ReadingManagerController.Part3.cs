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

        #region PART 3
        public IActionResult Part3(
           int categoryPage = 1,
           string categorySearchKey = "")
        {
            var testCategories = CategoryRender(nameof(Part3), TestCategory.READING, 3, categoryPage, categorySearchKey);
            return View($"{nameof(Part3)}/Index", testCategories);
        }

        [HttpGet]
        public IActionResult Part3Create()
        {
            return View($"{nameof(Part3)}/{nameof(Part3Create)}",
                new ReadingCombined
                {
                    TestCategory = TestCategory.ReadingCategory(3),
                    ReadingPartTwos = ReadingPartTwo.Generate(Config.MAX_READING_PART_3_QUESTION)
                });
        }

        [HttpPost]
        public IActionResult Part3Create(ReadingCombined readingCombined)
        {
            return Part3Or4Processing(nameof(Part3), nameof(Part3Create), readingCombined);
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
                    readingPartTwos = ReadingPartTwo.Generate(Config.MAX_READING_PART_3_QUESTION);
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
            return Part3Or4Processing(nameof(Part3), nameof(Part3Update), readingCombined);
        }


        [HttpDelete]
        public IActionResult Part3DeleteAjax(long id) // CategoryId
        {
            return Part3Or4Delele(3, id);
        }

        #endregion

    }
}
