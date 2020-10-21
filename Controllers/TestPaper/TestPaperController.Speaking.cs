﻿using System;
using System.IO;
using System.Net.Http;
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
        public IActionResult LoadSpeakingTranscript(int id)
        {
            if (id <= 0)
                return Content(string.Empty);

            string wysiwygContent = _TestCategoryManager.GetWYSIWYGContent(id);

            return Content(wysiwygContent ?? string.Empty);
        }

        public IActionResult SpeakingNewTest(int? id)
        {
            // Kiến tạo danh sách câu hỏi và câu trả lời, đồng thời xáo trộn câu trả lời
            int PiceOfTestId = _TestCategoryManager.GenerateSpeakingTestPaper(_PieceOfTestManager, _SpeakingEmbedManager, User.Id(), id);
            if (PiceOfTestId > 0)
            {
                // Nếu lưu trữ thành công, thì tiến hành cho thí sinh làm
                return RedirectToAction(nameof(Speaking), new { id = PiceOfTestId });
            }
            else
            {
                this.NotifyError("Generate Speaking test fail");
                // Không thì trả về trang Index
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Speaking(int id)
        {
            ViewBag.Title = "SPEAKING TESTING";
            if (id <= 0)
            {
                return NotFoundTest();
            }


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
                return RedirectToAction(nameof(SpeakingReview), new { id });
            }

            // Tránh timer bị reset
            if (piece.CreatedTime != null)
            {
                ViewBag.Timer = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds;
            }

            SpeakingTestPaper paper = JsonConvert.DeserializeObject<SpeakingTestPaper>(piece.ResultOfTestJson);


            paper.PiceOfTestId = piece.Id;

            this.NotifySuccess("Try your best, Good Luck!");

            return View(paper);
        }

        [HttpPost]
        public async Task<IActionResult> Speaking(SpeakingTestPaper paper, string audioBase64)
        {
            if (paper == null)
            {
                this.NotifyError("Not found yor test");
                return Json(new { status = false, message = string.Empty, location = "/" });
            }

            if (paper.PiceOfTestId <= 0)
            {
                this.NotifyError("Not found yor test");
                return Json(new { status = false, message = string.Empty, location = "/" });
            }

            // Lấy người dùng hiện tại
            var owner = _UserManager.Get(User.Id());

            // Nếu không tìm thấy người này là ai
            if (owner == null)
            {
                this.NotifyError("You are not authorized to view or manipulate this test");
                return Json(new { status = false, message = string.Empty, location = "/" });
            }

            // Sau khi hoàn tất lọc các lỗi, tiến hành xử lý, đếm số câu đúng
            PieceOfTest piece = _PieceOfTestManager.Get(paper.PiceOfTestId);

            if (piece == null)
            {
                this.NotifyError("Not found yor test");
                return Json(new { status = false, message = string.Empty, location = "/" });
            }

            if (piece.InstructorId != User.Id() && piece.UserId != User.Id())
            {
                this.NotifyError("You are not authorized to view or manipulate this test");
                return Json(new { status = false, message = string.Empty, location = "/" });
            }

            string fileName = $"{owner.Username.ToLower()}_{piece.TypeCode}_{piece.Id}";

            // Thời gian hoàn tất
            float timeToFinished = DateTime.UtcNow.Subtract((DateTime)piece.CreatedTime).TotalSeconds.ToFloat();

            if (!string.IsNullOrEmpty(audioBase64))
            {
                // Chuyển base64 thành stream
                var bytes = Convert.FromBase64String(audioBase64.Replace("data:audio/mpeg;base64,", ""));
                var memoryStream = new MemoryStream(bytes);

                // Tạo tệp tin
                IFormFile file = new FormFile(memoryStream, 0, bytes.Length, null, $"{fileName}.mp3")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "audio/mpeg"
                };

                // Nếu file null
                if (file == null)
                    return Json(new { status = false, message = "Can not upload your audio, please try again!", location = "/" });

                // Tiến hành lưu tệp tin cho người dùng
                string path = await host.UploadForUserAudio(file, owner);

                // Nếu path không đúng
                if (string.IsNullOrEmpty(path))
                    return Json(new { status = false, message = "Can not upload your speaking, please try again!", location = "/" });

                // Cập nhật đường dẫn bài nói của HV cho bài thi
                paper.SpeakingPart.UserAudioPath = path;
            }

            // Cập nhật dữ liệu
            piece.ResultOfUserJson = JsonConvert.SerializeObject(paper);

            // Chưa có điểm tổng
            piece.Scores = -1;

            // Cập nhật thời gian kết thúc
            piece.TimeToFinished = timeToFinished;

            // Cập nhật vào CSDL
            _PieceOfTestManager.Update(piece);

            // Chuyển đến trang kết quả
            return Json(new { status = true, message = "Successful submission of exams", location = $"{Url.Action(nameof(Result), NameUtils.ControllerName<TestPaperController>())}/{piece.Id}" });
        }

        [HttpGet]
        public IActionResult SpeakingReview(int id)
        {
            if (id <= 0)
                return NotFoundTest();

            ViewBag.Title = "SPEAKING TESTING";

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

            // Điểm số của bài thi
            ViewBag.Scores = piece.Scores;

            // Bài thi của học viên
            SpeakingTestPaper userPaper = JsonConvert.DeserializeObject<SpeakingTestPaper>(piece.ResultOfUserJson ?? "");
            // Bài thi mẫu (Được tạo khi thi, của học viên)
            SpeakingTestPaper resultPaper = JsonConvert.DeserializeObject<SpeakingTestPaper>(piece.ResultOfTestJson ?? "");

            bool isReviewOfFailTest = piece.ResultOfUserJson == null || piece.ResultOfUserJson.Length <= 0 || piece.UpdatedTime == null;
            ViewBag.IsReviewOfFailTest = isReviewOfFailTest;
            ViewBag.ResultPaper = resultPaper;

            if (isReviewOfFailTest)
            {
                userPaper = resultPaper;
            }

            return View(nameof(Speaking), userPaper);
        }
    }
}
