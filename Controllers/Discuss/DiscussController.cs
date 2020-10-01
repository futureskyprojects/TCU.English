using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCU.English.Models.DataManager;

namespace TCU.English.Controllers
{
    [Authorize]
    public class DiscussController : Controller
    {
        private readonly UserManager _UserManager;

        public DiscussController(UserManager _UserManager)
        {
            this._UserManager = _UserManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateGroup(int friendId)
        {
            // Nếu không có mã định danh
            if (friendId <= 0)
            {
                this.NotifyError("Cannot identify who you want to discuss");
                return RedirectToAction(nameof(Index));
            }

            // Lấy dữ liệu người dùng xem có hay không, nếu không
            if (!_UserManager.IsExists(friendId))
            {
                this.NotifyError("Cannot identify who you want to discuss");
                return RedirectToAction(nameof(Index));
            }

            // Tiến hành tạo nhóm


            return Redirect("/");
        }
    }
}
