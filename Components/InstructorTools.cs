using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.PiceOfTest;
using TCU.English.Models.Repository;

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
            ViewBag.WTP = wtp;
        }
    }
}
