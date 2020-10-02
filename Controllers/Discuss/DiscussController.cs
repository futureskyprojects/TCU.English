using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [Authorize]
    public class DiscussController : Controller
    {
        private readonly UserManager _UserManager;
        private readonly DiscussionManager _DiscussionManager;
        private readonly DiscussionUserManager _DiscussionUserManager;

        public DiscussController(
            IDataRepository<User> _UserManager,
            IDataRepository<Discussion> _DiscussionManager,
            IDataRepository<DiscussionUser> _DiscussionUserManager)
        {
            this._UserManager = (UserManager)_UserManager;
            this._DiscussionManager = (DiscussionManager)_DiscussionManager;
            this._DiscussionUserManager = (DiscussionUserManager)_DiscussionUserManager;
        }


        public IActionResult Index(int page = 1, string searchKey = "")
        {
            // Lấy danh sách các cuộc thảo luận của người dùng hiện tại            
            int start = (page - 1) * Config.PAGE_PAGINATION_LIMIT;
            var discussions = _DiscussionManager.GetByPaginationFor(User.Id(), start, Config.PAGE_PAGINATION_LIMIT);

            // Tạo đối tượng phân trang
            ViewBag.Pagination = new Pagination(nameof(Index), NameUtils.ControllerName<DiscussController>())
            {
                PageCurrent = page,
                NumberPage = PaginationUtils.TotalPageCount(
                    _DiscussionManager.CountAllFor(User.Id()).ToInt(),
                    Config.PAGE_PAGINATION_LIMIT),
                Offset = Config.PAGE_PAGINATION_LIMIT
            };

            return View(discussions);
        }

        public IActionResult CreateDiscuss(int friendId)
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

            // Kiểm tra xem nhóm đã có hay chưa
            Discussion discussion = _DiscussionManager.GetExistP2PDiscuss(User.Id(), friendId);

            // Nếu đã có, chuyển về cuộc hội thoại
            if (discussion != null && discussion.Id > 0)
            {
                this.NotifySuccess("Entered the discussion");
                RedirectToAction(nameof(Messages), discussion);
            }

            // Nếu nhóm chưa có, tạo nhóm
            discussion = new Discussion
            {
                CreatorId = User.Id()
            };

            // Lưu nhóm
            _DiscussionManager.Add(discussion);

            // Tạo thành viên
            DiscussionUser discussionUser = new DiscussionUser
            {
                UserId = friendId,
                DiscussionId = discussion.Id
            };

            // Lưu thành viên
            _DiscussionUserManager.Add(discussionUser);

            this.NotifySuccess("Entered the discussion");
            // Chuyển đến màn hình hội thoại
            return RedirectToAction(nameof(Messages), discussion);
        }

        public IActionResult Messages(Discussion discussion)
        {
            ViewBag.YourSelft = _UserManager.Get(User.Id());

            // Nếu bạn là người tạo
            if (discussion.CreatorId == User.Id())
                ViewBag.Friend = _DiscussionManager.GetFirstMember(discussion.Id); // thì thành viên kia chính là bạn của bạn
            else
                ViewBag.Friend = _UserManager.Get(discussion.CreatorId); // Còn không, người tạo chính là bạn của bạn

            return View(discussion);
        }


        public IActionResult CountUnRead(int id)
        {
            // Nếu id không hợp lệ
            if (id <= 0) return Json(new { success = true, count = 0 });

            // Đếm
            long count = _DiscussionManager.CountUnReadFor(id);

            // Trả về kết quả
            return Json(new { success = true, count });
        }

        public IActionResult DeleteAjax(int id)
        {
            // Nếu id không hợp lệ
            if (id <= 0)
                return Json(new { success = false, responseText = "This question was not found." });

            // Lấy cuộc trao đổi
            var discuss = _DiscussionManager.Get(id);

            // Nếu không tìm thấy, cho là thành công
            if (discuss == null)
                return Json(new { success = true, id, responseText = "Deleted" });

            // Nếu không phải củ sở hữu
            if (discuss.CreatorId != User.Id())
                return Json(new { success = false, responseText = "You not owner of this discuss." });

            // Xóa
            _DiscussionManager.Delete(discuss);

            // Trả về
            return Json(new { success = true, id, responseText = "Deleted" });
        }
    }
}
