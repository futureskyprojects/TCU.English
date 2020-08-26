using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        private readonly UserManager _UserManager;
        private readonly UserTypeManager _UserTypeManager;
        public AuthenticationController(IDataRepository<User> _UserManager, IDataRepository<UserType> _UserTypeManager)
        {
            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomeController.Index), NameUtils.ControllerName<HomeController>());
            }
            return View();
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(
            scheme: CookieAuthenticationConfig.DefaultSchemeName);

            return RedirectToAction(nameof(LogIn));
        }

        // https://viblo.asia/p/su-dung-cookie-authentication-trong-aspnet-core-djeZ1VG8lWz
        public async Task<IActionResult> LogIn(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                User user = _UserManager.Get(userLogin.Identity);
                if (user != null)
                {
                    var (Verified, NeedsUpgrade) = Utils.PasswordUtils.PasswordHasher.VerifyHashedPassword(user.HashPassword, userLogin.Password);
                    if (Verified)
                    {
                        string RequestPath = HttpContext.Request.Query[CookieAuthenticationConfig.ReturnUrlParameter];
                        // create claims
                        List<Claim> claims = new List<Claim>
                        {
                            new Claim(CustomClaimTypes.Id, user.Id.ToString()??"0"),
                            new Claim(ClaimTypes.NameIdentifier, user.Username??""),
                            new Claim(ClaimTypes.Email, user.Email??""),
                            new Claim(ClaimTypes.DateOfBirth, user.BirthDay.ToString()??""),
                            new Claim(ClaimTypes.Name, $"{user.LastName} {user.FirstName}"??""),
                            new Claim(ClaimTypes.GivenName, user.FirstName??""),
                            new Claim(ClaimTypes.Surname, user.LastName??""),
                            new Claim(ClaimTypes.Gender, user.Gender.ToString()??""),
                            new Claim(CustomClaimTypes.Avatar, user.Avatar??""),
                            new Claim(ClaimTypes.Role, string.Join(",",_UserTypeManager.GetAll(user.Id).Select(role => role.UserTypeName))??"")
                        };

                        // create identity
                        ClaimsIdentity identity = new ClaimsIdentity(claims, "user-info-cookie");

                        // create principal
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        // sign-in
                        await HttpContext.SignInAsync(
                                scheme: CookieAuthenticationConfig.DefaultSchemeName,
                                principal: principal,
                                properties: new AuthenticationProperties
                                {
                                    IsPersistent = userLogin.IsRemember, // for 'remember me' feature
                                    ExpiresUtc = DateTime.UtcNow.AddMinutes(10)
                                });
                        if (string.IsNullOrEmpty(RequestPath))
                            return RedirectToAction(nameof(HomeController.Index), NameUtils.ControllerName<HomeController>());
                        else
                            return Redirect(RequestPath);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Account or password is incorrect");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Account or password is incorrect");
                }
            }
            else
            {
            }
            return View(nameof(Index), userLogin);
        }
    }
}
