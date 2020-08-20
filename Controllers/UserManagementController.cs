using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IHostEnvironment host;
        private readonly UserManager _UserManager;
        private readonly UserTypeManager _UserTypeManager;
        private readonly UserTypeUserManager _UserTypeUserManager;
        public UserManagementController(IHostEnvironment _host, IDataRepository<User> _UserManager, IDataRepository<UserType> _UserTypeManager, IDataRepository<UserTypeUser> _UserTypeUserManager)
        {
            this.host = _host;
            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
            this._UserTypeUserManager = (UserTypeUserManager)_UserTypeUserManager;
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

        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        [ActionName(nameof(CreateUser))]
        public ActionResult CreateUser(User user, string[] userTypes, IFormFile userAvatar)
        {
            if (ModelState.IsValid)
            {
                if (_UserManager.IsEmailAlreadyInUse(user.Email))
                {
                    ModelState.AddModelError(nameof(Models.User.Username), "That email is already in use");
                }
                else if (_UserManager.IsUsernameAlreadyInUse(user.Username))
                {
                    ModelState.AddModelError(nameof(Models.User.Username), "That username is already in use");
                }
                else
                {
                    if (userAvatar != null && userAvatar.Length > 0 && userAvatar.Length <= Config.MAX_IMAGE_SIZE)
                    {
                        if (MimeTypeUtils.Image.CheckContentType(userAvatar.ContentType) && MimeTypeUtils.Image.CheckFileExtension(userAvatar.FileName))
                        {
                            // Upload avatar
                            try
                            {
                                var uniqueFileName = NameUtils.GetUniqueFileName(userAvatar.FileName);
                                var uploads = Path.Combine(host.ContentRootPath, "uploads", user.Username.ToLower());
                                // Kiểm tra xem folder có tồn tại không? Nếu không thì tạo mới
                                if (!Directory.Exists(uploads))
                                    Directory.CreateDirectory(uploads);
                                var filePath = Path.Combine(uploads, uniqueFileName);
                                // Nếu file đã tồn tại thì xóa
                                if (new FileInfo(filePath).Exists)
                                {
                                    new FileInfo(filePath).Delete();
                                }
                                userAvatar.CopyTo(new FileStream(filePath, FileMode.Create));
                                user.Avatar = filePath.Replace(host.ContentRootPath, "");
                            }
                            catch (Exception e)
                            {
                                ModelState.AddModelError(nameof(Models.User.Avatar), e.Message);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(nameof(Models.User.Avatar), "Invalid avatar image");
                        }
                    }
                }
                // Save user
                _UserManager.Add(user);

                if (user.Id > 0)
                {
                    // Remove all role of added user if have
                    foreach (var role in _UserTypeUserManager.GetAll(user.Id))
                    {
                        _UserTypeUserManager.Delete(role);
                    }
                    // Get selected new roles of user
                    foreach (string typeCode in userTypes)
                    {
                        var userType = _UserTypeManager.Get(typeCode);
                        if (userType != null)
                        {
                            _UserTypeUserManager.Add(new UserTypeUser
                            {
                                // Update role of added user
                                User = user,
                                UserType = userType
                            });
                        }
                    }
                }
            }
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