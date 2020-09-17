using System.Collections.Generic;
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
    public class ListeningManagerController : Controller
    {
        private readonly IHostEnvironment host;

        private readonly UserManager _UserManager;
        private readonly UserTypeManager _UserTypeManager;
        private readonly TestCategoryManager _TestCategoryManager;
        private readonly ListeningMediaManager _ListeningMediaManager;
        private readonly ListeningBaseQuestionManager _ListeningBaseQuestionManager;

        public ListeningManagerController(
           IHostEnvironment _host,
           IDataRepository<User> _UserManager,
           IDataRepository<UserType> _UserTypeManager,
           IDataRepository<TestCategory> _TestCategoryManager,
           IDataRepository<ListeningMedia> _ListeningMediaManager,
           IDataRepository<ListeningBaseQuestion> _ListeningBaseQuestionManager)
        {
            host = _host;

            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._ListeningMediaManager = (ListeningMediaManager)_ListeningMediaManager;
            this._ListeningBaseQuestionManager = (ListeningBaseQuestionManager)_ListeningBaseQuestionManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region PART 1
        public IActionResult Part1(int categoryPage = 1, string categorySearchKey = "")
        {
            var testCategories = IndexRender(nameof(Part1), TestCategory.LISTENING, 1, categoryPage, categorySearchKey);
            return View($"{nameof(Part1)}/Index", testCategories);
        }

        [HttpGet]
        public IActionResult Part1Create()
        {
            return View($"{nameof(Part1)}/{nameof(Part1Create)}", new ListeningBaseCombined
            {
                TestCategory = TestCategory.ListeningCategory(1),
                ListeningMedia = ListeningMedia.Generate(),
                ListeningBaseQuestions = ListeningBaseQuestion.Generate(1)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Part1Create(ListeningBaseCombined listeningBaseCombined, IFormFile audio, params IFormFile[] images)
        {
            return await Processing(nameof(Part1), nameof(Part1Create), listeningBaseCombined, audio, images, false);
        }

        [HttpGet]
        public IActionResult Part1Update(long id)
        {
            return UpdateRender(nameof(Part1), nameof(Part1Update), id);
        }

        [HttpPost]
        public async Task<IActionResult> Part1Update(ListeningBaseCombined listeningBaseCombined, IFormFile audio, params IFormFile[] images)
        {
            return await Processing(nameof(Part1), nameof(Part1Update), listeningBaseCombined, audio, images);
        }


        [HttpDelete]
        public IActionResult Part1DeleteAjax(long id) // CategoryId
        {
            return Delete(TestCategory.LISTENING, 1, id);
        }

        #endregion

        #region PART 2
        public IActionResult Part2(int categoryPage = 1, string categorySearchKey = "")
        {
            var testCategories = IndexRender(nameof(Part1), TestCategory.LISTENING, 2, categoryPage, categorySearchKey);
            return View($"{nameof(Part2)}/Index", testCategories);
        }

        [HttpGet]
        public IActionResult Part2Create()
        {
            return View($"{nameof(Part2)}/{nameof(Part2Create)}", new ListeningBaseCombined
            {
                TestCategory = TestCategory.ListeningCategory(2),
                ListeningMedia = ListeningMedia.Generate(),
                ListeningBaseQuestions = ListeningBaseQuestion.Generate(6)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Part2Create(ListeningBaseCombined listeningBaseCombined, IFormFile audio)
        {
            return await Processing(nameof(Part1), nameof(Part1Create), listeningBaseCombined, audio);
        }

        [HttpGet]
        public IActionResult Part2Update(long id)
        {
            return UpdateRender(nameof(Part2), nameof(Part2Update), id);
        }

        [HttpPost]
        public async Task<IActionResult> Part2Update(ListeningBaseCombined listeningBaseCombined, IFormFile audio)
        {
            return await Processing(nameof(Part1), nameof(Part1Create), listeningBaseCombined, audio);
        }

        [HttpDelete]
        public IActionResult Part2DeleteAjax(long id) // CategoryId
        {
            return Delete(TestCategory.LISTENING, 2, id);
        }
        #endregion

        //=============================================================================//
        public bool IsValidate(ListeningBaseCombined listeningBaseCombined, IFormFile audio)
        {
            ModelState.Remove(nameof(ListeningBaseQuestion.Answers));
            if ((audio == null || audio.Length <= 0) && listeningBaseCombined.TestCategory.Id <= 0)
            {
                ModelState.AddModelError(string.Empty, $"{nameof(ListeningMedia.Audio)} is required.");
                return false;
            }
            return (listeningBaseCombined != null && listeningBaseCombined.TestCategory != null &&
                listeningBaseCombined.TestCategory != null &&
                listeningBaseCombined.TestCategory.Name != null && listeningBaseCombined.TestCategory.Name.Length > 0 &&
                listeningBaseCombined.TestCategory.TypeCode != null && listeningBaseCombined.TestCategory.TypeCode.Length > 0 &&
                listeningBaseCombined.TestCategory.PartId > 0 &&
                listeningBaseCombined.ListeningMedia != null &&
                listeningBaseCombined.ListeningBaseQuestions != null &&
                listeningBaseCombined.ListeningBaseQuestions.Count > 0);
        }
        public bool ValidateAndCountAnswers(ListeningBaseCombined listeningBaseCombined, bool isCheckQuestionText = true, bool isCheckAnswersText = true, IFormFile[] images = null)
        {
            // Tổng số câu trả lời
            int sumOfAnswer = 0;
            // Lấy mã người tạo
            int userId = User.Id();
            // Cập nhật mã người tạo cho category
            if (listeningBaseCombined.TestCategory.CreatorId <= 0)
                listeningBaseCombined.TestCategory.CreatorId = userId;
            // Tiến hành kiểm tra trong các phần câu hỏi và đặt mã người tạo + convert đối tượng thành Json và gán vào ANSWERS
            for (int i = 0; i < listeningBaseCombined.ListeningBaseQuestions.Count; i++)
            {
                if ((listeningBaseCombined.ListeningBaseQuestions[i].QuestionText == null || listeningBaseCombined.ListeningBaseQuestions[i].QuestionText.Length <= 0) && isCheckQuestionText)
                {
                    ModelState.AddModelError(string.Empty, $"{nameof(ListeningBaseQuestion.QuestionText)} of question {i + 1} is required.");
                    return false;
                }

                //
                string validate = listeningBaseCombined.ListeningBaseQuestions[i].AnswerList.BaseAnswerValidation(isCheckAnswersText);
                if (!string.IsNullOrEmpty(validate))
                {
                    ModelState.AddModelError(string.Empty, $"{nameof(ListeningBaseQuestion.Answers)} of question {i + 1}: {validate}");
                    return false;
                }
                if (listeningBaseCombined.ListeningBaseQuestions[i].CreatorId <= 0)
                    listeningBaseCombined.ListeningBaseQuestions[i].CreatorId = userId;

                sumOfAnswer += listeningBaseCombined.ListeningBaseQuestions[i].AnswerList.Count;
            }

            if (sumOfAnswer < 0)
                return false;

            // Nếu là thêm nhưng số lượng ảnh kèm theo không đủ
            if (images != null && listeningBaseCombined.TestCategory.CreatorId <= 0 && sumOfAnswer != images.Length)
            {
                ModelState.AddModelError(string.Empty, "Please provide full photos for the answers.");
                return false;
            }

            return true;
        }
        private bool UpdateTestCategory(ListeningBaseCombined listeningBaseCombined)
        {
            if (listeningBaseCombined.TestCategory.Id <= 0)
            {
                _TestCategoryManager.Add(listeningBaseCombined.TestCategory);
            }
            else
            {
                _TestCategoryManager.Update(listeningBaseCombined.TestCategory);
            }
            return listeningBaseCombined.TestCategory.Id > 0;
        }
        private async Task<bool> UpdateAudio(ListeningBaseCombined listeningBaseCombined, IFormFile audio)
        {
            if ((audio == null || audio.Length <= 0) && listeningBaseCombined.TestCategory.Id > 0)
                return true;

            // Tiến hành tải audio lên
            string audioUploadPath = await host.UploadForTestAudio(audio, TestCategory.LISTENING, listeningBaseCombined.TestCategory.PartId);

            if (audioUploadPath == null || audioUploadPath.Length <= 0)
            {
                // Nếu gặp sự cố thì tiến hành xóa bỏ mục câu hỏi và trở lại trang thêm để thông báo
                if (listeningBaseCombined.ListeningMedia.Id <= 0)
                    _TestCategoryManager.Delete(listeningBaseCombined.TestCategory);
                ModelState.AddModelError(string.Empty, "Cannot upload audio file.");
                return false;
            }
            else
            {
                // Cập nhật đường dẫn vào
                listeningBaseCombined.ListeningMedia.Audio = audioUploadPath;
                // Cập nhật mục nó thuộc về
                if (listeningBaseCombined.ListeningMedia.TestCategoryId <= 0)
                {
                    listeningBaseCombined.ListeningMedia.TestCategoryId = listeningBaseCombined.TestCategory.Id;
                    listeningBaseCombined.ListeningMedia.Active = true;
                    // Cập nhật vào CSDl
                    _ListeningMediaManager.Add(listeningBaseCombined.ListeningMedia);
                }
                else
                {
                    // Cập nhật vào CSDl
                    _ListeningMediaManager.Update(listeningBaseCombined.ListeningMedia);
                }
            }

            return true;
        }
        private async Task<bool> UpdateImages(ListeningBaseCombined listeningBaseCombined, IFormFile[] images)
        {
            if (listeningBaseCombined.TestCategory.Id > 0 && (images == null || images.Length <= 0))
                return true;

            if (listeningBaseCombined.TestCategory.Id <= 0 && images.Length < listeningBaseCombined.ListeningBaseQuestions.Count)
            {
                ModelState.AddModelError(string.Empty, "Missing photos for the answers.");
                return false;
            }

            // Tiến hành tải các ảnh lên
            List<string> uploadImgePaths = new List<string>();
            for (int i = 0; i < images.Length; i++)
            {
                string uploadResult = await host.UploadForTestImage(images[i], TestCategory.LISTENING, 1);
                if (uploadResult == null || uploadResult.Length <= 0)
                {
                    uploadResult = "";
                }
                uploadImgePaths.Add(uploadResult);
            }
            var imgPathIndex = 0;
            // Cập nhật các đường dẫn được tải lên vào các câu trả lời
            for (int i = 0; i < listeningBaseCombined.ListeningBaseQuestions.Count; i++)
            {
                for (int j = 0; j < listeningBaseCombined.ListeningBaseQuestions[i].AnswerList.Count; j++)
                {
                    if (string.IsNullOrEmpty(listeningBaseCombined.ListeningBaseQuestions[i].AnswerList[j].AnswerContent))
                        listeningBaseCombined.ListeningBaseQuestions[i].AnswerList[j].AnswerContent = uploadImgePaths[imgPathIndex++];
                }
                listeningBaseCombined.ListeningBaseQuestions[i].Answers = JsonConvert.SerializeObject(listeningBaseCombined.ListeningBaseQuestions[i].AnswerList);
            }
            // Cập nhật người tạo, đồng thời thêm vào CSDL
            for (int i = 0; i < listeningBaseCombined.ListeningBaseQuestions.Count; i++)
            {
                if (listeningBaseCombined.ListeningBaseQuestions[i].TestCategoryId <= 0)
                {
                    listeningBaseCombined.ListeningBaseQuestions[i].TestCategoryId = listeningBaseCombined.TestCategory.Id;
                    _ListeningBaseQuestionManager.Add(listeningBaseCombined.ListeningBaseQuestions[i]);
                }
                else
                {
                    _ListeningBaseQuestionManager.Update(listeningBaseCombined.ListeningBaseQuestions[i]);
                }
            }
            return true;
        }
        private async Task<IActionResult> Processing(string partName, string actionName, ListeningBaseCombined listeningBaseCombined, IFormFile audio, bool isCheckQuestionText = true, bool isUploadImages = false, IFormFile[] images = null)
        {
            var view = View($"{partName}/{actionName}", listeningBaseCombined);

            // Nếu mục không hợp lệ
            if (!IsValidate(listeningBaseCombined, audio))
                return view;

            if (isUploadImages)
            {
                // Nếu cho phép cập nhật hình ảnh, thì kiểm tra tính hợp lệ với cả ảnh
                if (!ValidateAndCountAnswers(listeningBaseCombined, isCheckQuestionText, false, images)) return view;
            }
            else
            {
                // Nếu không cho phép cập nhật hình ảnh, chỉ kiểm tra tính hợp lệ của câu hỏi
                if (!ValidateAndCountAnswers(listeningBaseCombined, isCheckQuestionText)) return view;
            }

            // Nếu cập nhật mục không thành công
            if (!UpdateTestCategory(listeningBaseCombined))
            {
                ModelState.AddModelError(string.Empty, "An error occurred during execution.");
                return view;
            }

            // Nếu up âm thanh không được
            if (!await UpdateAudio(listeningBaseCombined, audio))
                return View($"{partName}/{actionName}", listeningBaseCombined);

            // Cho phép upload hình ảnh
            if (isUploadImages && !await UpdateImages(listeningBaseCombined, images))
                return View($"{partName}/{actionName}", listeningBaseCombined);

            // Chuyển hướng đến hiển thị danh sách
            return RedirectToAction(partName);
        }
        private async Task<IActionResult> Processing(string partName, string actionName, ListeningBaseCombined listeningBaseCombined, IFormFile audio, IFormFile[] images, bool isCheckQuestionText = true)
        {
            return await Processing(partName, actionName, listeningBaseCombined, audio, isCheckQuestionText, true, images);
        }
        private IActionResult Delete(string typeCode, int partId, long id)
        {
            var category = _TestCategoryManager.Get(id);
            if (category.TypeCode != typeCode || category.PartId != partId)
            {
                return Json(new { success = false, responseText = "You cannot perform deletion to item other than the current item." });
            }
            if (category == null)
            {
                return Json(new { success = false, responseText = "This test category was not found." });
            }
            else
            {
                _TestCategoryManager.Delete(category);
                return Json(new { success = true, category = JsonConvert.SerializeObject(category), responseText = "Deleted" });
            }
        }
        private IEnumerable<TestCategory> IndexRender(string actionName, string typeCode, int partId, int categoryPage = 1, string categorySearchKey = "")
        {
            int limit = 20;
            int categoryStart = (categoryPage - 1) * limit;

            var testCategories = _TestCategoryManager.GetByPagination(typeCode, partId, categoryStart, limit);

            // Tạo đối tượng phân trang cho Category
            ViewBag.CategoryPagination = new Pagination(actionName, NameUtils.ControllerName<ListeningManagerController>())
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
        private IActionResult UpdateRender(string partName, string actionName, long id)
        {
            if (id <= 0)
                return BadRequest();

            // Lấy mục câu hỏi
            var testCategory = _TestCategoryManager.Get(id);
            if (testCategory == null)
                return NotFound();

            // Lấy danh sách câu trả lời
            var listeningBaseQuestions = _ListeningBaseQuestionManager.GetAll(testCategory.Id).ToList();

            // Tạo câu hỏi nếu chưa có
            if (listeningBaseQuestions.Count <= 0)
                listeningBaseQuestions = ListeningBaseQuestion.Generate(10);

            // Chuyển json câu hỏi thành danh sách để thao tác
            for (int i = 0; i < listeningBaseQuestions.Count(); i++)
                if (listeningBaseQuestions[i].Answers != null && listeningBaseQuestions[i].Answers.Length > 0)
                    listeningBaseQuestions[i].AnswerList = JsonConvert.DeserializeObject<List<BaseAnswer>>(listeningBaseQuestions[i].Answers);

            // Lấy danh sách MEDIA
            var listeningMedia = _ListeningMediaManager.GetByCategory(testCategory.Id);

            return View($"{partName}/{actionName}",
                new ListeningBaseCombined
                {
                    TestCategory = testCategory,
                    ListeningBaseQuestions = listeningBaseQuestions,
                    ListeningMedia = listeningMedia
                });
        }
    }
}
