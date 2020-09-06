using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.PiceOfTest;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    public class TestPaperController : Controller
    {
        private readonly IHostEnvironment host;

        private readonly TestCategoryManager _TestCategoryManager;
        private readonly ReadingPartOneManager _ReadingPartOneManager;
        private readonly ReadingPartTwoManager _ReadingPartTwoManager;
        private readonly ListeningBaseQuestionManager _ListeningBaseQuestionManager;
        private readonly WritingPartOneManager _WritingPartOneManager;
        private readonly PieceOfTestManager _PieceOfTestManager;

        public TestPaperController(
          IHostEnvironment _host,
          IDataRepository<TestCategory> _TestCategoryManager,
          IDataRepository<ReadingPartOne> _ReadingPartOneManager,
          IDataRepository<ReadingPartTwo> _ReadingPartTwoManager,
          IDataRepository<ListeningBaseQuestion> _ListeningBaseQuestionManager,
          IDataRepository<WritingPartOne> _WritingPartOneManager,
          IDataRepository<PieceOfTest> _PieceOfTestManager)
        {
            host = _host;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
            this._ReadingPartOneManager = (ReadingPartOneManager)_ReadingPartOneManager;
            this._ReadingPartTwoManager = (ReadingPartTwoManager)_ReadingPartTwoManager;
            this._ListeningBaseQuestionManager = (ListeningBaseQuestionManager)_ListeningBaseQuestionManager;
            this._WritingPartOneManager = (WritingPartOneManager)_WritingPartOneManager;
            this._PieceOfTestManager = (PieceOfTestManager)_PieceOfTestManager;
        }
        public IActionResult Index()
        {
            return RedirectToAction(nameof(TestController.NewTest), NameUtils.ControllerName<TestController>());
        }

        public IActionResult Reading()
        {
            ViewBag.Title = "READING TESTING";
            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            ReadingTestPaper paper = _TestCategoryManager.GenerateReadingTestPaper();
            // Khởi tạo đối tượng lưu trữ bài kiểm tra này và lưu paper mặc định có đáp án đúng vào
            var piceOfTest = new PieceOfTest
            {
                UserId = User.Id(),
                TypeCode = TestCategory.READING,
                PartId = -1,
                ResultOfTestJson = JsonConvert.SerializeObject(paper)
            };
            // Lưu trữ bài thi vào database trước khi bắt đầu
            _PieceOfTestManager.Add(piceOfTest);
            // Xóa đáp án đúng trong paper
            paper.RemoveCorrectAnswers();
            // Lưu ID của lưu trữ vào paper trước khi khởi đầu bài thi
            paper.PiceOfTestId = piceOfTest.Id;

            if (paper.PiceOfTestId > 0)
            {
                // Nếu lưu trữ thành công, thì tiến hành cho thí sinh làm
                return View(paper);
            }
            else
            {
                // Không thì trả về trang Index
                return RedirectToAction(nameof(Index));
            }

        }
    }
}
