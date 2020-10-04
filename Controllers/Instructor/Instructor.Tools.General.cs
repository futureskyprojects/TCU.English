using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.PiceOfTest;
using TCU.English.Utils;
using static TCU.English.Models.PiceOfTest.WritingTestPaper;

namespace TCU.English.Controllers
{
    public partial class InstructorController
    {
        [HttpPost]
        public IActionResult SubmitToolResultForGeneral(GeneralTestPaper gtp, string InstructorComments = "")
        {
            // Định nghĩa đích đến
            var dest = RedirectToAction(nameof(TestPaperController.ReviewHandler), NameUtils.ControllerName<TestPaperController>(), new { id = gtp.PieceOfTestId });

            // Nếu không xác định được bài thi
            if (gtp == null || gtp.PieceOfTestId <= 0)
            {
                this.NotifyError("Cannot determine the test");
                return dest;
            }

            // Nếu nội dung đánh giá cho writing rỗng
            if (gtp.WritingTestPaper == null || gtp.WritingTestPaper.WritingPartTwos == null || string.IsNullOrEmpty(gtp.WritingTestPaper.WritingPartTwos.TeacherReviewParagraph))
            {
                this.NotifyError("You must correct paragraph for your student");
                return dest;
            }

            // Nếu chưa cho điểm bài thi viết
            if (gtp.WritingTestPaper.WritingPartTwos.Scores < 0 || gtp.WritingTestPaper.WritingPartTwos.Scores > Config.SCORES_FULL_WRITING_PART_2)
            {
                this.NotifyError("Writing part 2 Score invalid");
                return dest;
            }

            // Nếu chưa cho điểm bài thi nói
            if (gtp.SpeakingTestPaper.SpeakingPart.Scores < 0 || gtp.SpeakingTestPaper.SpeakingPart.Scores > Config.SCORES_FULL_SPEAKING)
            {
                this.NotifyError("Speaking Score invalid");
                return dest;
            }

            // Nếu tất cả đã hợp lệ, tiến hành lấy bản ghi dữ liệu bài thi
            var pot = _PieceOfTestManager.Get(gtp.PieceOfTestId);

            // Lấy dữ liệu gốc của bài thi
            GeneralTestPaper _gtp = JsonConvert.DeserializeObject<GeneralTestPaper>(pot.ResultOfUserJson);

            // Cập nhật điểm và review part 2 của writing
            _gtp.WritingTestPaper.WritingPartTwos.TeacherReviewParagraph = gtp.WritingTestPaper.WritingPartTwos.TeacherReviewParagraph;
            _gtp.WritingTestPaper.WritingPartTwos.Scores = gtp.WritingTestPaper.WritingPartTwos.Scores;

            // Cập nhật điểm cho speaking
            _gtp.SpeakingTestPaper.SpeakingPart.Scores = gtp.SpeakingTestPaper.SpeakingPart.Scores;

            // Lưu lại json
            pot.ResultOfUserJson = JsonConvert.SerializeObject(_gtp);

            // Cập nhật lại comment nếu có
            if (!string.IsNullOrEmpty(InstructorComments))
                pot.InstructorComments = InstructorComments;

            // Tổng điểm lại
            pot.Scores = 0;
            if (_gtp.ListeningTestPaper.Part1Scores >= 0)
                pot.Scores += _gtp.ListeningTestPaper.Part1Scores;

            if (_gtp.ListeningTestPaper.Part2Scores >= 0)
                pot.Scores += _gtp.ListeningTestPaper.Part2Scores;

            if (_gtp.ReadingTestPaper.ReadingPartOnes.Scores >= 0)
                pot.Scores += _gtp.ReadingTestPaper.ReadingPartOnes.Scores;

            if (_gtp.ReadingTestPaper.ReadingPartTwos.Scores >= 0)
                pot.Scores += _gtp.ReadingTestPaper.ReadingPartTwos.Scores;

            if (_gtp.ReadingTestPaper.ReadingPartThrees.Scores >= 0)
                pot.Scores += _gtp.ReadingTestPaper.ReadingPartThrees.Scores;

            if (_gtp.ReadingTestPaper.ReadingPartFours.Scores >= 0)
                pot.Scores += _gtp.ReadingTestPaper.ReadingPartFours.Scores;

            if (_gtp.WritingTestPaper.WritingPartOnes.Scores >= 0)
                pot.Scores += _gtp.WritingTestPaper.WritingPartOnes.Scores;

            if (_gtp.WritingTestPaper.WritingPartTwos.Scores >= 0)
                pot.Scores += _gtp.WritingTestPaper.WritingPartTwos.Scores;

            if (_gtp.SpeakingTestPaper.SpeakingPart.Scores >= 0)
                pot.Scores += _gtp.SpeakingTestPaper.SpeakingPart.Scores;

            // Cập nhật vào CSDL
            _PieceOfTestManager.Update(pot);

            // Gửi thông báo thành công
            this.NotifySuccess("Update your review to student success!");

            // Về điểm đích đã khai báo
            return dest;
        }
    }
}