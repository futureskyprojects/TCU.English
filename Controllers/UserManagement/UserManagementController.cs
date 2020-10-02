﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        public IActionResult Index(string type = "all", int page = 1, string searchKey = "")
        {
            
            int start = (page - 1) * Config.PAGE_PAGINATION_LIMIT;

            long allUserCount = _UserManager.Count();
            long allLearnersCount = _UserManager.Count(UserType.ROLE_NORMAL_USER);
            long allManagersCount = _UserManager.Count(UserType.ROLE_MANAGER_USER) + _UserManager.Count(UserType.ROLE_MANAGER_LIBRARY) + _UserManager.Count(UserType.ROLE_ALL);
            long allBlockedCount = _UserManager.Count(null);

            if (searchKey != null && searchKey.Length > 0)
            {
                type = "all"; // Nếu có tìm, thì sẽ là tìm tất cả
            }

            ViewBag.AllUserCount = allUserCount;
            ViewBag.AllLearnersCount = allLearnersCount;
            ViewBag.AllManagersCount = allManagersCount;
            ViewBag.AllBlockedCount = allBlockedCount;

            long total;
            IEnumerable<User> users = new List<User>();
            if (type.ToUpper() == "LEARNER".ToUpper())
            {
                total = allLearnersCount;
                users = _UserManager.GetByPagination(start, Config.PAGE_PAGINATION_LIMIT, UserType.ROLE_NORMAL_USER);

                ViewBag.TableName = "LIST ALL LEARNERS";
                ViewBag.TableDescription = "List all learners using your system";
            }
            else if (type.ToUpper() == "MANAGER".ToUpper())
            {
                total = allManagersCount;
                users = users
                    .Concat(_UserManager.GetByPagination(start, Config.PAGE_PAGINATION_LIMIT, UserType.ROLE_ALL))
                    .Concat(_UserManager.GetByPagination(start, Config.PAGE_PAGINATION_LIMIT, UserType.ROLE_MANAGER_LIBRARY))
                    .Concat(_UserManager.GetByPagination(start, Config.PAGE_PAGINATION_LIMIT, UserType.ROLE_MANAGER_USER)).Distinct();

                ViewBag.TableName = "LIST ALL MANAGER";
                ViewBag.TableDescription = "List all manager of your system";
            }
            else if (type.ToUpper() == "BLOCKED".ToUpper())
            {
                total = allBlockedCount;
                users = _UserManager.GetByPagination(start, Config.PAGE_PAGINATION_LIMIT, null);

                ViewBag.TableName = "LIST ALL BLOCKED USERS";
                ViewBag.TableDescription = "List all blocked user using your system";
            }
            else
            {
                // ALL
                total = allUserCount;
                users = _UserManager.GetByPagination(searchKey, start, Config.PAGE_PAGINATION_LIMIT);

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

            // Tạo đối tượng phân trang
            ViewBag.Pagination = new Pagination(nameof(Index), NameUtils.ControllerName<UserManagementController>())
            {
                PageCurrent = page,
                Type = type,
                NumberPage = PaginationUtils.TotalPageCount(total.ToInt(), Config.PAGE_PAGINATION_LIMIT),
                Offset = Config.PAGE_PAGINATION_LIMIT
            };

            // Get data
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user, string[] userTypes, IFormFile userAvatar)
        {
            if (ModelState.IsValid)
            {
                if (_UserManager.IsEmailAlreadyInUse(user.Email))
                {
                    ModelState.AddModelError(nameof(Models.User.Email), "That email is already in use");
                    return View(user);
                }
                else if (_UserManager.IsUsernameAlreadyInUse(user.Username))
                {
                    ModelState.AddModelError(nameof(Models.User.Username), "That username is already in use");
                    return View(user);
                }
                else
                {
                    if (userAvatar != null && userAvatar.Length > 0)
                    {
                        string uploadResult = await host.UploadForUserImage(userAvatar, user);
                        if (uploadResult != null && uploadResult.Length > 0)
                        {
                            user.Avatar = uploadResult;
                        }
                        else
                        {
                            ModelState.AddModelError(nameof(Models.User.Avatar), "Invalid avatar image");
                            return View(user);
                        }
                    }
                }
                // Save user
                _UserManager.Add(user);

                if (user.Id > 0 && userTypes != null && userTypes.Length > 0)
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
            else
            {
                return View(user);
            }
            this.NotifySuccess($"Account {user.Username} created");
            return RedirectToAction(nameof(UpdateUser), new { id = user.Id });
        }

        [HttpGet]
        public IActionResult UpdateUser(long id)
        {
            if (id <= 0)
                return BadRequest();
            var user = _UserManager.Get(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return View(user);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(User user, string[] userTypes, IFormFile userAvatar)
        {
            if (ModelState.IsValid)
            {
                if (_UserManager.IsEmailAlreadyInUse(user.Username, user.Email))
                {
                    ModelState.AddModelError(nameof(Models.User.Email), "That email is already in use");
                }
                else
                {
                    if (userAvatar != null && userAvatar.Length > 0)
                    {
                        string uploadResult = await host.UploadForUserImage(userAvatar, user);
                        if (uploadResult != null && uploadResult.Length > 0)
                        {
                            user.Avatar = uploadResult;
                        }
                        else
                        {
                            ModelState.AddModelError(nameof(Models.User.Avatar), "Invalid avatar image");
                            return View(user);
                        }
                    }
                    // Save user
                    _UserManager.Update(user);

                    if (user.Id > 0 && userTypes != null && userTypes.Length > 0)
                    {
                        _UserTypeUserManager.Delete(user.Id);
                        // Get selected new roles of user
                        foreach (string typeCode in userTypes)
                        {
                            var userType = _UserTypeManager.Get(typeCode);
                            if (userType != null)
                            {
                                _UserTypeUserManager.Add(new UserTypeUser
                                {
                                    // Update role of added user
                                    UserId = user.Id,
                                    UserTypeId = userType.Id
                                });
                            }
                        }
                    }
                }
            }
            this.NotifySuccess($"Account {user.Username} updated");
            return RedirectToAction(nameof(UpdateUser), new { id = user.Id });
        }

        [HttpDelete]
        public IActionResult Delete(long id)
        {
            var user = _UserManager.Get(id);
            var userRole = user.UserTypeUser.OrderBy(it => it.UserType.Priority).Last();
            UserType maxCurrentUserType = UserType.GetMaxUserType((User.FindFirstValue(ClaimTypes.Role) ?? "").Split(","));
            if (user != null)
            {
                if (user.Username == User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return Json(new { success = false, responseText = "You cannot remove yourself!" });
                }
                else if (userRole != null && maxCurrentUserType != null && UserType.CompareRole(maxCurrentUserType.UserTypeName, userRole.UserType.UserTypeName) < 0)
                {
                    return Json(new { success = false, responseText = "You do not have sufficient authority to delete this account!" });
                }
                else
                {
                    _UserManager.Delete(user);
                    user.HashPassword = "";
                    var uploads = Path.Combine(host.GetContentPathRootForUploadUtils(), NameUtils.ControllerName<UploadsController>().ToLower(), user.Username.ToLower());
                    // Xóa thư mục tệp tin của người dùng này nếu có tồn tại
                    if (Directory.Exists(uploads))
                        Directory.Delete(uploads, true);
                    return Json(new { success = true, user = JsonConvert.SerializeObject(user), responseText = "Deleted" });
                }
            }
            else
            {
                return Json(new { success = false, responseText = "Can not find this user!" });
            }
        }
    }
}