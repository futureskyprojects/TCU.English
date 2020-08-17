using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        public UserManagementController(IDataRepository<User> _UserManager, IDataRepository<UserType> _UserTypeManager)
        {
            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
        }
        public ActionResult Index()
        {
            IEnumerable<User> users = _UserManager.GetAll();
            ViewBag.AllUserCount = _UserManager.Count();
            ViewBag.AllLearnersCount = _UserManager.Count(UserType.ROLE_NORMAL_USER);
            ViewBag.AllManagersCount = _UserManager.Count(UserType.ROLE_MANAGER_USER) + _UserManager.Count(UserType.ROLE_MANAGER_LIBRARY) + _UserManager.Count(UserType.ROLE_ALL);
            ViewBag.AllBlockedCount = _UserManager.Count(null);
            return View(users);
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