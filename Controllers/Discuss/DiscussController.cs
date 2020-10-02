using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;

namespace TCU.English.Controllers
{
    [Authorize]
    public class DiscussController : Controller
    {
        private readonly UserManager _UserManager;
        private readonly DiscussionManager _DiscussionManager;

        public DiscussController(
            IDataRepository<User> _UserManager,
            IDataRepository<Discussion> _DiscussionManager)
        {
            this._UserManager = (UserManager)_UserManager;
            this._DiscussionManager = (DiscussionManager)_DiscussionManager;
        }


        public IActionResult Index(int page = 1, string searchKey = "")
        {
            // Lấy danh sách các cuộc thảo luận của người dùng hiện tại
            
            int start = (page - 1) * Config.PAGE_PAGINATION_LIMIT;
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
