using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.PiceOfTest;
using TCU.English.Models.Repository;
using static TCU.English.Models.PiceOfTest.SpeakingTestPaper;

namespace TCU.English.Components
{
    public class InstructorTools : ViewComponent
    {
        private readonly PieceOfTestManager _PieceOfTestManager;
        private readonly UserManager _UserManager;
        public InstructorTools(
            IDataRepository<PieceOfTest> _PieceOfTestManager,
            IDataRepository<User> _UserManager
            )
        {
            this._PieceOfTestManager = (PieceOfTestManager)_PieceOfTestManager;
            this._UserManager = (UserManager)_UserManager;
        }

        public IViewComponentResult InvokeForGeneralTest(int pieceOfTestId)
        {
            // Lấy bài thi theo mã cho trước
            PieceOfTest pot = _PieceOfTestManager.Get(pieceOfTestId);

            // Nếu đây là bài hỏng của học viên
            if (string.IsNullOrEmpty(pot.ResultOfUserJson))
                return View("GeneralTools");

            // Lấy đối tượng bài thi chung
            GeneralTestPaper gtp = JsonConvert.DeserializeObject<GeneralTestPaper>(pot.ResultOfUserJson);

            // Gắn bài thi vô đối tượng biến tạm
            ViewBag.POT = _PieceOfTestManager.GetForInstructorTool(pieceOfTestId); ;

            // Khởi tạo đoạn văn chấm cho GV nếu chưa có
            if (string.IsNullOrEmpty(gtp.WritingTestPaper.WritingPartTwos.TeacherReviewParagraph))
                gtp.WritingTestPaper.WritingPartTwos.TeacherReviewParagraph = gtp.WritingTestPaper.WritingPartTwos.UserParagraph;



            return View("GeneralTools", new GeneralTestPaper
            {
                PieceOfTestId = gtp.PieceOfTestId,
                WritingTestPaper = new WritingTestPaper
                {
                    WritingPartTwos = new WritingTestPaper.WritingPartTwoDTO
                    {
                        TeacherReviewParagraph = gtp.WritingTestPaper.WritingPartTwos.TeacherReviewParagraph,
                        Scores = gtp.WritingTestPaper.WritingPartTwos.Scores
                    }
                },
                SpeakingTestPaper = new SpeakingTestPaper
                {
                    SpeakingPart = new SpeakingDTO
                    {
                        Scores = gtp.SpeakingTestPaper.SpeakingPart.Scores
                    }
                }
            });
        }

        public IViewComponentResult Invoke(int piceOfTestId)
        {
            // Nếu biến số truyền vào không đúng
            if (piceOfTestId <= 0)
                return View();

            // Tiến hành lấy bài thi
            var pot = _PieceOfTestManager.GetForInstructorTool(piceOfTestId);

            // Nếu không tìm thấy bài thi
            if (pot == null)
                return View();

            // Nếu là bài thi tổng, chuyển sang một tool riêng
            if (pot.TypeCode == TestCategory.TEST_ALL)
                return InvokeForGeneralTest(piceOfTestId);

            ViewBag.POT = pot;

            // Nếu bài thi là bài viết
            if (pot.TypeCode == TestCategory.WRITING)
                WritingToolProcess(piceOfTestId);

            return View();
        }

        private void WritingToolProcess(int piceOfTestId)
        {
            var useRes = _PieceOfTestManager.GetUserResult(piceOfTestId);
            if (string.IsNullOrEmpty(useRes))
                return;

            if (!(JsonConvert.DeserializeObject<WritingTestPaper>(useRes) is WritingTestPaper wtp))
                return;

            // Nếu GVHD chưa nhận xét gì, copy bài của học viên sang để dễ chỉnh sửa
            if (string.IsNullOrEmpty(wtp.WritingPartTwos.TeacherReviewParagraph))
                wtp.WritingPartTwos.TeacherReviewParagraph = wtp.WritingPartTwos.UserParagraph;

            // Model truyền cho Writing
            ViewBag.WP2 = wtp.WritingPartTwos;
        }
    }
}
