using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly UserNoteManager _UserNoteManager;

        public NotesController(IDataRepository<UserNote> _UserNoteManager)
        {
            this._UserNoteManager = (UserNoteManager)_UserNoteManager;
        }

        public IActionResult Index(
            int notePage = 1,
            int noteId = -1)
        {
            // Lấy danh sách các ghi chú của User
            IEnumerable<UserNote> userNotes = _UserNoteManager.GetAll(User.Id());

            // Tạo đối tượng phân trang cho Grammars
            ViewBag.NotePagination = new Pagination(nameof(Index), NameUtils.ControllerName<DictionaryController>())
            {
                PageKey = nameof(notePage),
                PageCurrent = notePage,
                NumberPage = PaginationUtils.TotalPageCount(
                    _UserNoteManager.CountFor(User.Id()),
                    Math.Min(10, Config.PAGE_PAGINATION_LIMIT)),
                Offset = Math.Min(10, Config.PAGE_PAGINATION_LIMIT)
            };

            return View(userNotes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult Create(UserNote note)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "Please input your note content" });

            note.UserId = User.Id();

            _UserNoteManager.Add(note);
            return Json(new { status = true, message = "Successfully created, the list will refresh again in 1 second." });
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            ViewBag.IsShowImmediately = true;
            return PartialView(_UserNoteManager.Get(id));
        }

        [HttpPost]
        public IActionResult Update(UserNote note)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "Note invalid" });

            UserNote _un = _UserNoteManager.Get(note.Id);

            _un.WYSIWYGContent = note.WYSIWYGContent;

            _UserNoteManager.Update(_un);
            return Json(new { status = true, message = "Successfully updated, the list will refresh again in 1 second." });
        }

        public IActionResult LoadNote(int id)
        {
            return Content(_UserNoteManager.Get(id)?.WYSIWYGContent ?? string.Empty);
        }
    }
}
