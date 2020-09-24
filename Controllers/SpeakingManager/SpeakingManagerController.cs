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
    [AuthorizeRoles(UserType.ROLE_ALL, UserType.ROLE_MANAGER_LIBRARY)]
    public class SpeakingManagerController : Controller
    {
        private readonly IHostEnvironment host;

        private readonly UserManager _UserManager;
        private readonly UserTypeManager _UserTypeManager;
        private readonly TestCategoryManager _TestCategoryManager;
        private readonly SpeakingEmbedManager _SpeakingEmbedManager;

        public SpeakingManagerController(
           IHostEnvironment _host,
           IDataRepository<User> _UserManager,
           IDataRepository<UserType> _UserTypeManager,
           IDataRepository<TestCategory> _TestCategoryManager,
           IDataRepository<SpeakingEmbed> _SpeakingEmbedManager)
        {
            host = _host;
            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._SpeakingEmbedManager = (SpeakingEmbedManager)_SpeakingEmbedManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region EMBED
        public IActionResult Embed(
           int categoryPage = 1,
           string categorySearchKey = "")
        {
            var testCategories = CategoryRender(nameof(Embed), TestCategory.SPEAKING, Config.SPEAKING_EMBED_PART_ID, categoryPage, categorySearchKey);

            return View($"{nameof(Embed)}/Index", testCategories);
        }

        [HttpGet]
        public IActionResult EmbedCreate()
        {
            return View($"{nameof(Embed)}/{nameof(EmbedCreate)}",
                new SpeakingEmbedCombined
                {
                    TestCategory = TestCategory.SpeakingCategory(Config.SPEAKING_EMBED_PART_ID),
                });
        }

        [HttpPost]
        public IActionResult EmbedCreate(SpeakingEmbedCombined SpeakingEmbedCombined)
        {
            return EmbedProcessing(nameof(Embed), nameof(EmbedCreate), SpeakingEmbedCombined, false);
        }

        [HttpGet]
        public IActionResult EmbedUpdate(long id)
        {
            if (id <= 0)
                return BadRequest();

            var testCategory = _TestCategoryManager.Get(id);
            if (testCategory == null)
                return NotFound();

            var SpeakingEmbed = _SpeakingEmbedManager.GetByCategoryId(testCategory.Id);
            if (SpeakingEmbed == null)
                return NotFound();

            return View($"{nameof(Embed)}/{nameof(EmbedUpdate)}",
                new SpeakingEmbedCombined
                {
                    TestCategory = testCategory,
                    SpeakingEmbed = SpeakingEmbed
                });
        }

        [HttpPost]
        public IActionResult EmbedUpdate(SpeakingEmbedCombined SpeakingEmbedCombined)
        {
            return EmbedProcessing(nameof(Embed), nameof(EmbedUpdate), SpeakingEmbedCombined);
        }


        [HttpDelete]
        public IActionResult EmbedDeleteAjax(long id) // CategoryId
        {
            return EmbedDelele(Config.SPEAKING_EMBED_PART_ID, id);
        }

        #endregion    

        //==========================================================//
        private bool IsValidate(SpeakingEmbedCombined SpeakingEmbedCombined)
        {
            if (!(SpeakingEmbedCombined != null && SpeakingEmbedCombined.TestCategory != null &&
                SpeakingEmbedCombined.TestCategory.TypeCode != null && SpeakingEmbedCombined.TestCategory.TypeCode.Length > 0 &&
                SpeakingEmbedCombined.TestCategory.PartId > 0 &&
                SpeakingEmbedCombined.TestCategory.WYSIWYGContent != null && SpeakingEmbedCombined.TestCategory.WYSIWYGContent.Length > 0 &&
                SpeakingEmbedCombined.SpeakingEmbed.YoutubeVideo != null &&
                SpeakingEmbedCombined.SpeakingEmbed.YoutubeVideo.Length > 0))
                return false;

            // Lấy mã người tạo
            int userId = User.Id();

            // Cập nhật mã người tạo cho category
            if (SpeakingEmbedCombined.TestCategory.CreatorId <= 0)
                SpeakingEmbedCombined.TestCategory.CreatorId = userId;

            if (SpeakingEmbedCombined.SpeakingEmbed.CreatorId <= 0)
                SpeakingEmbedCombined.SpeakingEmbed.CreatorId = userId;

            return true;
        }
        private IActionResult EmbedProcessing(string partName, string partAction, SpeakingEmbedCombined SpeakingEmbedCombined, bool isCheckQuestionText = true)
        {
            var view = View($"{partName}/{partAction}", SpeakingEmbedCombined);
            // Nếu không hợp lệ
            if (!IsValidate(SpeakingEmbedCombined))
            {
                ModelState.AddModelError(string.Empty, "Please enter the full information of the required item");
                return view;
            }

            // Tiến hành thêm danh mục vào CSDL và lấy ID
            if (SpeakingEmbedCombined.TestCategory.Id <= 0)
                _TestCategoryManager.Add(SpeakingEmbedCombined.TestCategory);
            else
                _TestCategoryManager.Update(SpeakingEmbedCombined.TestCategory);

            if (SpeakingEmbedCombined.TestCategory.Id <= 0)
            {
                ModelState.AddModelError(string.Empty, "An error occurred during execution.");
                return view;
            }

            // Cập nhật ID danh mục cho bài viết 2
            if (SpeakingEmbedCombined.SpeakingEmbed.TestCategoryId <= 0)
                SpeakingEmbedCombined.SpeakingEmbed.TestCategoryId = SpeakingEmbedCombined.TestCategory.Id;

            // Kiểm tra và cập nhật vào CSDL
            if (SpeakingEmbedCombined.SpeakingEmbed.Id <= 0)
                _SpeakingEmbedManager.Add(SpeakingEmbedCombined.SpeakingEmbed);
            else
                _SpeakingEmbedManager.Update(SpeakingEmbedCombined.SpeakingEmbed);

            this.NotifySuccess("Update completed!");

            // Trả về
            return RedirectToAction(partName);
        }
        private IActionResult EmbedDelele(int partId, long id)
        {
            var category = _TestCategoryManager.Get(id);
            if (category.TypeCode != TestCategory.SPEAKING || category.PartId != partId)
            {
                return Json(new { success = false, responseText = "You cannot perform deletion to item other than the current item." });
            }
            if (category == null)
            {
                return Json(new { success = false, responseText = "This category was not found." });
            }
            else
            {
                _TestCategoryManager.Delete(category);
                return Json(new { success = true, category = JsonConvert.SerializeObject(category), responseText = "Deleted" });
            }
        }

        private IEnumerable<TestCategory> CategoryRender(string actionName, string typeCode, int partId, int categoryPage = 1, string categorySearchKey = "")
        {
            int limit = 20;
            int categoryStart = (categoryPage - 1) * limit;

            var testCategories = _TestCategoryManager.GetByPagination(typeCode, partId, categoryStart, limit);

            // Tạo đối tượng phân trang cho Category
            ViewBag.CategoryPagination = new Pagination(actionName, NameUtils.ControllerName<SpeakingManagerController>())
            {
                PageKey = nameof(categoryPage),
                PageCurrent = categoryPage,
                NumberPage = PaginationUtils.TotalPageCount(
                    _TestCategoryManager.GetAll(typeCode, partId).Count(),
                    limit),
                Offset = limit
            };
            return testCategories;
        }
    }
}
