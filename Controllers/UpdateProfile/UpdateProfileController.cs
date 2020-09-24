using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [Authorize]
    public class UpdateProfileController : Controller
    {
        private readonly IHostEnvironment host;
        private readonly UserManager _UserManager;
        private readonly UserTypeManager _UserTypeManager;
        private readonly UserTypeUserManager _UserTypeUserManager;

        public UpdateProfileController(
            IHostEnvironment _host,
            IDataRepository<User> _UserManager,
            IDataRepository<UserType> _UserTypeManager,
            IDataRepository<UserTypeUser> _UserTypeUserManager)
        {
            this.host = _host;
            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
            this._UserTypeUserManager = (UserTypeUserManager)_UserTypeUserManager;
        }
        public IActionResult Index()
        {
            long userId = User.Id();
            if (userId > 0)
            {
                var user = _UserManager.Get(userId);
                if (user != null)
                {
                    return View(user);
                }
            }
            return NotFoundUser();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(User user, IFormFile userAvatar)
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
                            return View(nameof(Index), user);
                        }
                    }
                    // Save user
                    _UserManager.Update(user);

                    this.NotifySuccess("Update success");
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private IActionResult NotFoundUser()
        {
            this.NotifyError("Can't find this user");
            return RedirectToAction(nameof(HomeController.Index), NameUtils.ControllerName<HomeController>());
        }
    }
}
