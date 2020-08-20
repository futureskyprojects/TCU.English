using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult Index(string type = "all", int page = 1, string searchKey = "")
        {
            int limit = 20;
            int start = (page - 1) * limit;

            long allUserCount = _UserManager.Count();
            long allLearnersCount = _UserManager.Count(UserType.ROLE_NORMAL_USER);
            long allManagersCount = _UserManager.Count(UserType.ROLE_MANAGER_USER) + _UserManager.Count(UserType.ROLE_MANAGER_LIBRARY) + _UserManager.Count(UserType.ROLE_ALL);
            long allBlockedCount = _UserManager.Count(null);

            if (searchKey != null && searchKey.Length > 0)
            {
                type = "all"; // Nếu có tìm, thì sẽ là tìm tất cả
            }

            ViewBag.currentPageIndex = page;
            ViewBag.Type = type;

            ViewBag.AllUserCount = allUserCount;
            ViewBag.AllLearnersCount = allLearnersCount;
            ViewBag.AllManagersCount = allManagersCount;
            ViewBag.AllBlockedCount = allBlockedCount;

            long total;
            IEnumerable<User> users = new List<User>();
            if (type.ToUpper() == "LEARNER".ToUpper())
            {
                total = allLearnersCount;
                users = _UserManager.GetByPagination(start, limit, UserType.ROLE_NORMAL_USER);

                ViewBag.TableName = "LIST ALL LEARNERS";
                ViewBag.TableDescription = "List all learners using your system";
            }
            else if (type.ToUpper() == "MANAGER".ToUpper())
            {
                total = allManagersCount;
                users = users
                    .Concat(_UserManager.GetByPagination(start, limit, UserType.ROLE_ALL))
                    .Concat(_UserManager.GetByPagination(start, limit, UserType.ROLE_MANAGER_LIBRARY))
                    .Concat(_UserManager.GetByPagination(start, limit, UserType.ROLE_MANAGER_USER)).Distinct();

                ViewBag.TableName = "LIST ALL MANAGER";
                ViewBag.TableDescription = "List all manager of your system";
            }
            else if (type.ToUpper() == "BLOCKED".ToUpper())
            {
                total = allBlockedCount;
                users = _UserManager.GetByPagination(start, limit, null);

                ViewBag.TableName = "LIST ALL BLOCKED USERS";
                ViewBag.TableDescription = "List all blocked user using your system";
            }
            else
            {
                // ALL
                total = allUserCount;
                users = _UserManager.GetByPagination(searchKey, start, limit);

                if (searchKey != null && searchKey.Length > 0)
                {
                    ViewBag.TableName = $"RESULT FOR \"{searchKey}\"";
                    ViewBag.TableDescription = $"Match with {users.Count()} results";
                }
                else
                {
                    ViewBag.TableName = "LIST ALL USERS";
                    ViewBag.TableDescription = "List all user using your system";
                }
            }
            ViewBag.totalPageNumber = PaginationUtils.TotalPageCount(total.ToInt(), limit);
            ViewBag.offset = limit;
            // Get data
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