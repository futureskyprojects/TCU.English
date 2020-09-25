using Microsoft.AspNetCore.Mvc;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;

namespace TCU.English.Components
{
    public class InstructorTools : ViewComponent
    {
        private readonly PieceOfTestManager _PieceOfTestManager;
        public InstructorTools(IDataRepository<PieceOfTest> _PieceOfTestManager)
        {
            this._PieceOfTestManager = (PieceOfTestManager)_PieceOfTestManager;
        }
        public IViewComponentResult Invoke(int piceOfTestId)
        {
            // Nếu không tìm thấy bài thi
            if (piceOfTestId <= 0)
                return View();

            // Tiến hành lấy mã bài thi
            var pot = _PieceOfTestManager.GetForInstructorTool(piceOfTestId);

            return View(pot);
        }
    }
}
