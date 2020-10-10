using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [AuthorizeRoles(UserType.ROLE_ALL, UserType.ROLE_MANAGER_LIBRARY)]
    public class DictionaryManagerController : Controller
    {
        private readonly TopicManager _TopicManager;
        private readonly VocabularyManager _VocabularyManager;

        public DictionaryManagerController(IDataRepository<Topic> _TopicManager, IDataRepository<Vocabulary> _VocabularyManager)
        {
            this._TopicManager = _TopicManager as TopicManager;
            this._VocabularyManager = _VocabularyManager as VocabularyManager;
        }

        public IActionResult Index(
            long topic = -1,
            int topicPage = 1,
            int vocabularyPage = 1,
            string topicSearchKey = "",
            string vocabularySearchKey = "")
        {

            #region For topic
            int topicStart = (topicPage - 1) * Config.PAGE_PAGINATION_LIMIT;

            var topics = _TopicManager.GetByPagination(topicStart, Config.PAGE_PAGINATION_LIMIT);
            ViewBag.Topics = topics;

            // Tạo đối tượng phân trang cho Category
            ViewBag.TopicPagination = new Pagination(nameof(Index), NameUtils.ControllerName<DictionaryManagerController>())
            {
                PageKey = nameof(topicPage),
                PageCurrent = topicPage,
                NumberPage = PaginationUtils.TotalPageCount(
                    _TopicManager.Count().ToInt(),
                    Config.PAGE_PAGINATION_LIMIT),
                Offset = Config.PAGE_PAGINATION_LIMIT
            };
            #endregion

            #region For vocabulary
            int vocabularyStart = (vocabularyPage - 1) * Config.PAGE_PAGINATION_LIMIT;

            int total = _VocabularyManager.CountFor(topic).ToInt();

            // Tạo đối tượng phân trang cho Câu hỏi
            ViewBag.VocabularyPagination = new Pagination(nameof(Index), NameUtils.ControllerName<ReadingManagerController>())
            {
                PageKey = nameof(vocabularyPage),
                PageCurrent = vocabularyPage,
                TypeKey = nameof(topic),
                Type = "0",
                NumberPage = PaginationUtils.TotalPageCount(total, Config.PAGE_PAGINATION_LIMIT),
                Offset = Config.PAGE_PAGINATION_LIMIT
            };

            ViewBag.TopicName = _TopicManager.Get(topic)?.Name ?? "";

            if (topic <= 0)
                ViewBag.TopicName = "ALL";

            ViewBag.Vocabularies = _VocabularyManager.GetByPagination(topic, vocabularyStart, Config.PAGE_PAGINATION_LIMIT);
            #endregion

            return View();
        }

        [HttpGet]
        public IActionResult TopicCreate()
        {
            return PartialView();
        }

        [HttpGet]
        public IActionResult TopicUpdate(int id)
        {
            if (id <= 0)
                return NotFound();

            Topic topic = _TopicManager.Get(id);

            if (topic == null)
                return NotFound();

            ViewBag.IsShowImmediately = true;

            return PartialView(topic);
        }

        public IActionResult TopicCreateAjax(Topic topic)
        {
            if (_TopicManager.Exists(topic.Name))
                return Json(new { status = false, message = "Topic is exist" });

            _TopicManager.Add(topic);
            return Json(new { status = true, message = "Add new topic success" });
        }

        public IActionResult TopicUpdateAjax(Topic topic)
        {

            _TopicManager.Update(topic);
            return Json(new { status = true, message = "Update topic success" });
        }

        public IActionResult VocabularyCreate()
        {
            ViewBag.Topics = _TopicManager.GetAll();
            return PartialView();
        }

        public IActionResult VocabularyUpdate(int id)
        {

            if (id <= 0)
                return NotFound();

            Vocabulary vocabulary = _VocabularyManager.Get(id);

            if (vocabulary == null)
                return NotFound();

            ViewBag.IsShowImmediately = true;
            ViewBag.Topics = _TopicManager.GetAll();
            return PartialView(vocabulary);
        }

        public IActionResult VocabularyCreateAjax(Vocabulary vocabulary)
        {
            if (_VocabularyManager.Exists(vocabulary.TopicId, vocabulary.Word))
                return Json(new { status = false, message = "Vocabulary is exist" });

            _VocabularyManager.Add(vocabulary);

            return Json(new { status = true, message = "Add new vocabulary success" });
        }

        public IActionResult VocabularyUpdateAjax(Vocabulary vocabulary)
        {
            _VocabularyManager.Add(vocabulary);

            return Json(new { status = true, message = "Add new vocabulary success" });
        }

        public IActionResult TopicDeleteAjax(int id)
        {
            if (id <= 0)
                return Json(new { success = false, responseText = "Topic is invalid" });

            Topic topic = _TopicManager.Get(id);

            if (topic == null)
                return Json(new { success = true, category = "", responseText = "Deleted" });

            _TopicManager.Delete(topic);
            return Json(new { success = true, category = "", responseText = "Deleted" });
        }

        public IActionResult VocabularyDeleteAjax(int id)
        {
            if (id <= 0)
                return Json(new { success = false, responseText = "Vocabulary is invalid" });

            Vocabulary vocabulary = _VocabularyManager.Get(id);

            if (vocabulary == null)
                return Json(new { success = true, category = "", responseText = "Deleted" });

            _VocabularyManager.Delete(vocabulary);
            return Json(new { success = true, category = "", responseText = "Deleted" });
        }

    }
}
