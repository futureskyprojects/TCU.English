using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [AuthorizeRoles(UserType.ROLE_ALL, UserType.ROLE_MANAGER_USER)]
    public class UserManagementController : Controller
    {
        private readonly UserManager _UserManager;
        private readonly UserTypeManager _UserTypeManager;
        public UserManagementController(IDataRepository<User> _UserManager, IDataRepository<UserType> _UserTypeManager) {
            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateUser()
        {
            return View();
        }
        public ActionResult UpdateUser()
        {
            return View();
        }
        public ActionResult Delete()
        {
            return View();
        }
    }
}