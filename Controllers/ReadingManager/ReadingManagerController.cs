﻿using System;
using System.Collections.Generic;
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
    public partial class ReadingManagerController : Controller
    {
        private readonly IHostEnvironment host;

        private readonly UserManager _UserManager;
        private readonly UserTypeManager _UserTypeManager;
        private readonly TestCategoryManager _TestCategoryManager;
        private readonly ReadingPartOneManager _ReadingPartOneManager;
        private readonly ReadingPartTwoManager _ReadingPartTwoManager;
        public ReadingManagerController(
            IHostEnvironment _host,
            IDataRepository<User> _UserManager,
            IDataRepository<UserType> _UserTypeManager,
            IDataRepository<TestCategory> _TestCategoryManager,
            IDataRepository<ReadingPartOne> _ReadingPartOneManager,
            IDataRepository<ReadingPartTwo> _ReadingPartTwoManager)
        {
            this.host = _host;

            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._ReadingPartOneManager = (ReadingPartOneManager)_ReadingPartOneManager;
            this._ReadingPartTwoManager = (ReadingPartTwoManager)_ReadingPartTwoManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        //==========================================================//
        private string ErrorCollect()
        {
            string errors = "";
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    if (errors.Length > 0)
                        errors += "\r\n";
                    errors += error.ErrorMessage;
                }
            }
            return errors;
        }
        private string IsValidate(int testCategoryId, List<BaseAnswer> answers, int maxAnswers)
        {
            ModelState.Remove(nameof(ReadingPartOne.Answers));
            // Dữ liệu gửi lên không hợp lệ
            if (!ModelState.IsValid)
                return ErrorCollect();
            // Chưa chọn loại
            if (testCategoryId <= 0)
                return "Please choose a category for this question";
            // Chưa đủ câu trả lời
            if (answers == null || answers.Count < maxAnswers)
                return "Answers is not enough.";
            // Câu trả lời không đảm bảo tính hợp lệ
            string validation = answers.BaseAnswerValidation();
            if (validation != null && validation.Length > 0)
                return validation;
            return null;
        }
        private bool IsValidate(ReadingCombined readingCombined, bool isCheckQuestionText = true)
        {
            ModelState.Remove(nameof(ReadingPartTwo.Answers));
            if (!(readingCombined != null && readingCombined.TestCategory != null &&
                readingCombined.TestCategory.Name != null && readingCombined.TestCategory.Name.Length > 0 &&
                readingCombined.TestCategory.TypeCode != null && readingCombined.TestCategory.TypeCode.Length > 0 &&
                readingCombined.TestCategory.PartId > 0 &&
                readingCombined.TestCategory.WYSIWYGContent != null && readingCombined.TestCategory.WYSIWYGContent.Length > 0 &&
                readingCombined.ReadingPartTwos.Count > 0))
                return false;
            // Lấy mã người tạo
            int userId = User.Id();
            // Cập nhật mã người tạo cho category
            if (readingCombined.TestCategory.CreatorId <= 0)
                readingCombined.TestCategory.CreatorId = userId;
            // Tiến hành kiểm tra trong các phần câu hỏi và đặt mã người tạo + convert đối tượng thành Json và gán vào ANSWERS
            for (int i = 0; i < readingCombined.ReadingPartTwos.Count; i++)
            {
                if ((readingCombined.ReadingPartTwos[i].QuestionText == null || readingCombined.ReadingPartTwos[i].QuestionText.Length <= 0) && isCheckQuestionText)
                {
                    ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.QuestionText)} of question {i + 1} is required.");
                    return false;
                }

                string validation = readingCombined.ReadingPartTwos[i].AnswerList.BaseAnswerValidation();
                if (validation != null && validation.Length > 0)
                {
                    ModelState.AddModelError(string.Empty, $"{nameof(ReadingPartTwo.Answers)} of question {i + 1} error: {validation}.");
                    return false;
                }

                if (readingCombined.ReadingPartTwos[i].CreatorId <= 0)
                    readingCombined.ReadingPartTwos[i].CreatorId = userId;

                readingCombined.ReadingPartTwos[i].Answers = readingCombined.ReadingPartTwos[i].AnswerList.ToJson();
            }
            return true;
        }
        private IActionResult Part1Processing(ReadingPartOne readingPartOne)
        {
            // Kiểm tra tính hợp lệ
            string validateMsg = IsValidate(readingPartOne.TestCategoryId, readingPartOne.AnswerList, Config.MAX_READING_PART_1_QUESTION);
            if (!string.IsNullOrEmpty(validateMsg))
                return Json(new { status = false, message = validateMsg });
            // Chuyển danh sách câu trả lời thành JSON để lưu trữ
            readingPartOne.Answers = readingPartOne.AnswerList.ToJson();
            // Nếu dữ liệu không hợp lệ
            if (readingPartOne.Answers == null || readingPartOne.Answers.Length <= 0)
                return Json(new { status = false, message = "Unable to determine the answer to this question" });
            // Nếu chưa có người tạo
            if (readingPartOne.CreatorId <= 0)
                readingPartOne.CreatorId = User.Id();
            // Nếu là thêm mới
            if (readingPartOne.Id <= 0)
                _ReadingPartOneManager.Add(readingPartOne);
            else // Còn không là cập nhật
                _ReadingPartOneManager.Update(readingPartOne);
            // Trả về kết quả thành công
            return Json(new { status = true, message = "Successfully created, the list will refresh again in 1 second." });
        }
        private async Task<IActionResult> Part2Processing(ReadingPartTwo readingPartTwo, IFormFile questionImage)
        {
            // Kiểm tra tính hợp lệ
            string validateMsg = IsValidate(readingPartTwo.TestCategoryId, readingPartTwo.AnswerList, Config.MAX_READING_PART_2_QUESTION);
            if (!string.IsNullOrEmpty(validateMsg))
                return Json(new { status = false, message = validateMsg });
            // Chuyển danh sách câu trả lời thành JSON để lưu trữ
            readingPartTwo.Answers = readingPartTwo.AnswerList.ToJson();
            // Nếu dữ liệu không hợp lệ
            if (readingPartTwo.Answers == null || readingPartTwo.Answers.Length <= 0)
                return Json(new { status = false, message = "Unable to determine the answer to this question" });
            // Nếu chưa chọn ảnh cho mục này
            if (questionImage == null && readingPartTwo.Id <= 0)
                return Json(new { status = false, message = "Please select picture for the question." });
            // Cập nhật ảnh lên máy chủ
            string uploadResult = await host.UploadForTestImage(questionImage, TestCategory.READING, 2);
            if ((uploadResult == null || uploadResult.Length <= 0) && questionImage != null && questionImage.Length > 0)
                return Json(new { status = false, message = "Please check the picture of the question again" });
            // Nếu cập nhật ảnh thành công, lưu vào database
            if (uploadResult != null && uploadResult.Length > 0)
            {
                if (readingPartTwo.QuestionImage != null && readingPartTwo.QuestionImage.Length > 0)
                    host.RemoveUploadMeida(readingPartTwo.QuestionImage);
                readingPartTwo.QuestionImage = uploadResult;
            }
            // Nếu chưa có người tạo
            if (readingPartTwo.CreatorId <= 0)
                readingPartTwo.CreatorId = User.Id();
            // Nếu là tạo mới
            if (readingPartTwo.Id <= 0)
                _ReadingPartTwoManager.Add(readingPartTwo);
            else // Hoặc là cập nhật
                _ReadingPartTwoManager.Update(readingPartTwo);
            return Json(new { status = true, message = "Successfully created, the list will refresh again in 1 second." });
        }
        private IActionResult Part3Or4Processing(string partName, string partAction, ReadingCombined readingCombined, bool isCheckQuestionText = true)
        {
            var view = View($"{partName}/{partAction}", readingCombined);
            // Nếu không hợp lệ
            if (!IsValidate(readingCombined, isCheckQuestionText))
                return view;

            // Tiến hành thêm danh mục vào CSDL và lấy ID
            if (readingCombined.TestCategory.Id <= 0)
                _TestCategoryManager.Add(readingCombined.TestCategory);
            else
                _TestCategoryManager.Update(readingCombined.TestCategory);

            if (readingCombined.TestCategory.Id <= 0)
            {
                ModelState.AddModelError(string.Empty, "An error occurred during execution.");
                return view;
            }

            // Cập nhật câu hỏi
            for (int i = 0; i < readingCombined.ReadingPartTwos.Count; i++)
            {
                if (readingCombined.ReadingPartTwos[i].TestCategoryId <= 0)
                    readingCombined.ReadingPartTwos[i].TestCategoryId = readingCombined.TestCategory.Id;
                if (readingCombined.ReadingPartTwos[i].Id <= 0)
                    _ReadingPartTwoManager.Add(readingCombined.ReadingPartTwos[i]);
                else
                    _ReadingPartTwoManager.Update(readingCombined.ReadingPartTwos[i]);
            }

            this.NotifySuccess("Update completed!");

            // Trả về
            return RedirectToAction(partName);
        }
        private IActionResult Part3Or4Delele(int partId, long id)
        {
            var category = _TestCategoryManager.Get(id);
            if (category.TypeCode != TestCategory.READING || category.PartId != partId)
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
        private IActionResult Part1Or2Delete(int partId, long id)
        {
            ReadingPartOne readingPart1Question = null;
            ReadingPartTwo readingPart2Question = null;

            if (partId == 1) readingPart1Question = _ReadingPartOneManager.Get(id);
            if (partId == 2) readingPart2Question = _ReadingPartTwoManager.Get(id);

            bool isFound = (partId == 1 && readingPart1Question != null) || (partId == 2 && readingPart2Question != null);

            if (!isFound)
                return Json(new { success = false, responseText = "This question was not found." });

            if (partId == 1) _ReadingPartOneManager.Delete(readingPart1Question);
            if (partId == 2) _ReadingPartTwoManager.Delete(readingPart2Question);

            return Json(new { success = true, id, responseText = "Deleted" });
        }
        private IEnumerable<TestCategory> CategoryRender(string actionName, string typeCode, int partId, int categoryPage = 1, string categorySearchKey = "")
        {
            
            int categoryStart = (categoryPage - 1) * Config.PAGE_PAGINATION_LIMIT;

            var testCategories = _TestCategoryManager.GetByPagination(typeCode, partId, categoryStart, Config.PAGE_PAGINATION_LIMIT);

            // Tạo đối tượng phân trang cho Category
            ViewBag.CategoryPagination = new Pagination(actionName, NameUtils.ControllerName<ReadingManagerController>())
            {
                PageKey = nameof(categoryPage),
                PageCurrent = categoryPage,
                NumberPage = PaginationUtils.TotalPageCount(
                    _TestCategoryManager.GetAll(typeCode, partId).Count(),
                    Config.PAGE_PAGINATION_LIMIT),
                Offset = Config.PAGE_PAGINATION_LIMIT
            };
            return testCategories;
        }
        private IEnumerable<object> QuestionRender(string actionName, string typeCode, int partId, int questionPage = 1, int category = 0, string questionSearchKey = "")
        {
            
            int questionStart = (questionPage - 1) * Config.PAGE_PAGINATION_LIMIT;

            int total;
            if (partId == 1)
                total = _ReadingPartOneManager.GetAll(category).Count();
            else
                total = _ReadingPartTwoManager.GetAll(typeCode, partId, category).Count();

            // Tạo đối tượng phân trang cho Câu hỏi
            ViewBag.QuestionPagination = new Pagination(actionName, NameUtils.ControllerName<ReadingManagerController>())
            {
                PageKey = nameof(questionPage),
                PageCurrent = questionPage,
                TypeKey = nameof(category),
                Type = "0",
                NumberPage = PaginationUtils.TotalPageCount(total, Config.PAGE_PAGINATION_LIMIT),
                Offset = Config.PAGE_PAGINATION_LIMIT
            };

            if (category > 0)
            {
                var testCategory = _TestCategoryManager.Get(category);
                if (testCategory == null)
                {
                    return new List<object>();
                }
                else
                {
                    ViewBag.QuestionType = testCategory.Name ?? "";
                    if (partId == 1)
                        return _ReadingPartOneManager.GetByPagination(category, questionStart, Config.PAGE_PAGINATION_LIMIT);
                    else
                        return _ReadingPartTwoManager.GetByPagination(category, typeCode, partId, questionStart, Config.PAGE_PAGINATION_LIMIT);
                }
            }
            else
            {
                ViewBag.QuestionType = "ALL";
                if (partId == 1)
                    return _ReadingPartOneManager.GetByPagination(questionStart, Config.PAGE_PAGINATION_LIMIT);
                else
                    return _ReadingPartTwoManager.GetByPagination(typeCode, partId, questionStart, Config.PAGE_PAGINATION_LIMIT);
            }
        }
    }
}
