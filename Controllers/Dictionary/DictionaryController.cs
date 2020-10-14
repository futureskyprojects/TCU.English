using Microsoft.AspNetCore.Mvc;
using System;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    public class DictionaryController : Controller
    {
        private readonly TopicManager _TopicManager;
        private readonly VocabularyManager _VocabularyManager;
        private readonly TestCategoryManager _TestCategoryManager;
        public DictionaryController(
            IDataRepository<Topic> _TopicManager,
            IDataRepository<Vocabulary> _VocabularyManager,
            IDataRepository<TestCategory> _TestCategoryManager)
        {
            this._TopicManager = _TopicManager as TopicManager;
            this._VocabularyManager = _VocabularyManager as VocabularyManager;
            this._TestCategoryManager = _TestCategoryManager as TestCategoryManager;
        }
        public IActionResult Index(
            int grammarPage = 1,
            string grammarSearchKey = "",
            string lookup = "")
        {
            #region For grammar
            int topicStart = (grammarPage - 1) * Math.Min(10, Config.PAGE_PAGINATION_LIMIT);

            var grammars = _TestCategoryManager.GetByPagination(TestCategory.READING, 1, topicStart, Math.Min(10, Config.PAGE_PAGINATION_LIMIT), grammarSearchKey);
            ViewBag.Grammars = grammars;
            ViewBag.GrammarSearchKey = grammarSearchKey;

            // Tạo đối tượng phân trang cho Grammars
            ViewBag.GrammarPagination = new Pagination(nameof(Index), NameUtils.ControllerName<DictionaryController>())
            {
                PageKey = nameof(grammarPage),
                PageCurrent = grammarPage,
                NumberPage = PaginationUtils.TotalPageCount(
                    _TestCategoryManager.CountFor(TestCategory.READING, 1, grammarSearchKey),
                    Math.Min(10, Config.PAGE_PAGINATION_LIMIT)),
                Offset = Math.Min(10, Config.PAGE_PAGINATION_LIMIT)
            };
            #endregion

            #region For search vocabulary
            ViewBag.Vocabularies = _VocabularyManager.LookUp(lookup ?? string.Empty);
            ViewBag.LookUp = lookup;
            #endregion
            return View();
        }
    }
}
