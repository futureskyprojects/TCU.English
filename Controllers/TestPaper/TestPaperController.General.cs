using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.PiceOfTest;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    public partial class TestPaperController
    {

        [HttpGet]
        public IActionResult General(int id)
        {
            ViewBag.Title = "GENERAL TESTING";
            if (id <= 0)
                return NotFoundTest();

            // Sau khi hoàn tất lọc các lỗi, tiến hành xử lý, đếm số câu đúng
            PieceOfTest piece = _PieceOfTestManager.Get(id);

            // Nếu tìm không thấy bài Test
            if (piece == null)
                return NotFoundTest();

            if (piece.InstructorId != User.Id() && piece.UserId != User.Id())
            {
                this.NotifyError("You are not authorized to view or manipulate this test");
                return RedirectToAction(nameof(HomeController.Index), NameUtils.ControllerName<HomeController>());
            }

            // Lấy chủ sở hữu của bài kiểm tra
            User owner = _UserManager.Get(piece.UserId);
            ViewData["Owner"] = new User
            {
                Avatar = owner.Avatar,
                FirstName = owner.FirstName,
                LastName = owner.LastName
            };

            // Nếu bài thi đã hoàn thành, thì chuyển sang màn hình review
            if (piece.ResultOfUserJson != null && piece.ResultOfUserJson.Length > 0 && piece.UpdatedTime != null)
            {
                return RedirectToAction(nameof(GeneralReview), new { id });
            }

            // Tránh timer bị reset
            if (piece.CreatedTime != null)
            {
                ViewBag.Timer = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds;
            }

            // Giải mã thành giữ liệu bài thi
            GeneralTestPaper paper = JsonConvert.DeserializeObject<GeneralTestPaper>(piece.ResultOfTestJson);

            // Gắn mã dữ liệu
            paper.PieceOfTestId = piece.Id;

            // Xóa đáp án của bài thi
            paper.ClearTrueAnswers();

            this.NotifySuccess("Try your best, Good Luck!");

            return View(paper);
        }
        public IActionResult GeneralNewTest(int? id)
        {
            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            int PiceOfTestId = GeneralTestPaper.Generate(
                _TestCategoryManager,
                _ListeningBaseQuestionManager,
                _ListeningMediaManager,
                _WritingPartTwoManager,
                _SpeakingEmbedManager,
                _PieceOfTestManager,
                User.Id(),
                id).PieceOfTestId;

            if (PiceOfTestId > 0)
            {
                // Nếu lưu trữ thành công, thì tiến hành cho thí sinh làm
                return RedirectToAction(nameof(General), new { id = PiceOfTestId });
            }
            else
            {
                // Không thì trả về trang Index
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> General(GeneralTestPaper paper, string audioBase64)
        {
            if (paper == null)
                return NotFoundTest();

            if (paper.PieceOfTestId <= 0)
                return NotFoundTest();

            ViewBag.Title = "GENERAL TESTING";

            if (string.IsNullOrEmpty(audioBase64))
            {
                // Xóa đáp án của bài thi
                this.NotifyError("You have not recorded the reading yet");
                return View(paper);
            }

            // Chuyển base64 thành stream
            var bytes = Convert.FromBase64String(audioBase64.Replace("data:audio/mpeg;base64,", ""));
            var memoryStream = new MemoryStream(bytes);

            // Lấy bài thi từ mã số
            PieceOfTest piece = _PieceOfTestManager.Get(paper.PieceOfTestId);

            if (piece == null)
                return NotFoundTest();

            if (piece.InstructorId != User.Id() && piece.UserId != User.Id())
            {
                this.NotifyError("You are not authorized to view or manipulate this test");
                return RedirectToAction(nameof(HomeController.Index), NameUtils.ControllerName<HomeController>());
            }

            // Lấy chủ sở hữu của bài kiểm tra
            User owner = _UserManager.Get(piece.UserId);
            ViewData["Owner"] = new User
            {
                Avatar = owner.Avatar,
                FirstName = owner.FirstName,
                LastName = owner.LastName
            };

            string fileName = $"{owner.Username.ToLower()}_{piece.TypeCode}_{piece.Id}";
            // Tạo tệp tin
            IFormFile file = new FormFile(memoryStream, 0, bytes.Length, null, $"{fileName}.mp3")
            {
                Headers = new HeaderDictionary(),
                ContentType = "audio/mpeg"
            };

            // Nếu file null
            if (file == null)
            {
                this.NotifyError("Can not upload your audio, please try again!");
                return View(paper);
            }

            // Tiến hành lưu tệp tin cho người dùng
            string path = await host.UploadForUserAudio(file, owner);

            // Nếu path không đúng
            if (string.IsNullOrEmpty(path))
            {
                this.NotifyError("Can not upload your speaking, please try again!");
                return View(paper);
            }

            // Tránh timer bị reset
            if (piece.CreatedTime != null)
            {
                ViewBag.Timer = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds;
            }

            // Kiểm tra check full
            string validateString = paper.IsFullAnswers();

            if (!string.IsNullOrEmpty(validateString))
            {
                this.NotifyError("Please complete all questions");

                //paper = JsonConvert.DeserializeObject<GeneralTestPaper>(piece.ResultOfTestJson);
                // Xóa đáp án của bài thi
                return View(paper);
            }

            // Tính toán số điểm
            paper.ScoreCalculate(piece.ResultOfTestJson);

            // Thời gian kết thúc bài thi
            float timeToFinished = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds.ToFloat();

            // Cập nhật đường dẫn bài nói của HV cho bài thi
            paper.SpeakingTestPaper.SpeakingPart.UserAudioPath = path;

            // Cập nhật dữ liệu
            piece.ResultOfUserJson = JsonConvert.SerializeObject(paper);
            piece.TimeToFinished = timeToFinished;
            _PieceOfTestManager.Update(piece);

            // Chuyển đến trang kết quả
            return RedirectToAction(nameof(Result), new { id = piece.Id });
        }
        [HttpGet]
        public IActionResult GeneralReview(int id)
        {
            if (id <= 0)
                return NotFoundTest();

            ViewBag.Title = "GENERAL TESTING";

            ViewBag.IsReviewMode = true;

            // Sau khi hoàn tất lọc các lỗi, tiến hành lấy
            PieceOfTest piece = _PieceOfTestManager.Get(id);

            if (piece == null)
                return NotFoundTest();

            if (piece.InstructorId != User.Id() && piece.UserId != User.Id())
            {
                this.NotifyError("You are not authorized to view or manipulate this test");
                return RedirectToAction(nameof(HomeController.Index), NameUtils.ControllerName<HomeController>());
            }

            // Lấy chủ sở hữu của bài kiểm tra
            User owner = _UserManager.Get(piece.UserId);
            ViewData["Owner"] = new User
            {
                Avatar = owner.Avatar,
                FirstName = owner.FirstName,
                LastName = owner.LastName
            };

            // Thời gian làm bài
            ViewBag.Timer = piece.TimeToFinished;

            // Điểm của bài thi
            ViewBag.Scores = piece.Scores;

            // Bài thi của học viên
            GeneralTestPaper userPaper = JsonConvert.DeserializeObject<GeneralTestPaper>(piece.ResultOfUserJson ?? "");
            // Bài thi mẫu (Được tạo khi thi, của học viên)
            GeneralTestPaper resultPaper = JsonConvert.DeserializeObject<GeneralTestPaper>(piece.ResultOfTestJson ?? "");

            bool isReviewOfFailTest = piece.ResultOfUserJson == null || piece.ResultOfUserJson.Length <= 0 || piece.UpdatedTime == null;
            ViewBag.IsReviewOfFailTest = isReviewOfFailTest;
            ViewBag.ResultPaper = resultPaper;

            if (isReviewOfFailTest)
            {
                userPaper = resultPaper;
            }

            return View(nameof(General), userPaper);
        }
    }
}
