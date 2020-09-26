using Microsoft.AspNetCore.Mvc;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;

namespace TCU.English.Components
{
    public class InstructorTestUnMarksCount : ViewComponent
    {
        private readonly PieceOfTestManager _PieceOfTestManager;
        private readonly UserManager _UserManager;
        public InstructorTestUnMarksCount(
            IDataRepository<PieceOfTest> _PieceOfTestManager,
            IDataRepository<User> _UserManager
            )
        {
            this._PieceOfTestManager = (PieceOfTestManager)_PieceOfTestManager;
            this._UserManager = (UserManager)_UserManager;
        }
        public IViewComponentResult Invoke()
        {

            return View();
        }
    }
}
