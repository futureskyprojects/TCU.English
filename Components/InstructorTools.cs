using Microsoft.AspNetCore.Mvc;
using TCU.English.Models;
using TCU.English.Models.DataManager;
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
            // Nếu không tìm thấy bài thi
            if (piceOfTestId <= 0)
                return View();

            // Tiến hành lấy bài thi
            var pot = _PieceOfTestManager.GetForInstructorTool(piceOfTestId);

            return View(pot);
        }
    }
}
