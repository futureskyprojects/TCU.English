﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;
using static TCU.English.Hubs.ChatHub;

namespace TCU.English.Controllers
{
    [Authorize]
    public class DiscussController : Controller
    {
        private readonly UserManager _UserManager;
        private readonly DiscussionManager _DiscussionManager;
        private readonly DiscussionUserManager _DiscussionUserManager;
        private readonly DiscussionUserMessageManager _DiscussionUserMessageManager;

        public DiscussController(
            IDataRepository<User> _UserManager,
            IDataRepository<Discussion> _DiscussionManager,
            IDataRepository<DiscussionUser> _DiscussionUserManager,
            IDataRepository<DiscussionUserMessage> _DiscussionUserMessageManager)
        {
            this._UserManager = (UserManager)_UserManager;
            this._DiscussionManager = (DiscussionManager)_DiscussionManager;
            this._DiscussionUserManager = (DiscussionUserManager)_DiscussionUserManager;
            this._DiscussionUserMessageManager = (DiscussionUserMessageManager)_DiscussionUserMessageManager;
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
                RedirectToAction(nameof(Messages), discussion.Id);
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
            return RedirectToAction(nameof(Messages), discussion.Id);
        }
        [HttpGet]
        public IActionResult Messages(int id)
        {
            if (id <= 0)
            {
                this.NotifyError("Disscuss ID invalid!");
                return RedirectToAction(nameof(Index));
            }

            Discussion discussion = _DiscussionManager.Get(id);

            if (discussion == null)
            {
                this.NotifyError("Discuss not found!");
                return RedirectToAction(nameof(Index));
            }

            if (!_DiscussionManager.IsIn(discussion.Id, User.Id()))
            {
                this.NotifyError("You are not member of this discuss!");
                return RedirectToAction(nameof(Index));
            }

            // Khai báo bản thân
            ViewBag.YourSelft = _UserManager.Get(User.Id());

            // Khai báo bạn trong cuộc trò chuyện
            long yourFriendId = 0;

            // Nếu bạn là người tạo
            if (discussion.CreatorId == User.Id())
                yourFriendId = _DiscussionManager.GetFirstMemberId(discussion.Id); // thì thành viên kia chính là bạn của bạn
            else
                yourFriendId = discussion.CreatorId; // Còn không, người tạo chính là bạn của bạn

            // Nếu không tìm ra ai khác trong cuộc trò chuyện, quay lại
            if (yourFriendId <= 0)
            {
                this.NotifyError("Not found your partner in discuss");
                return RedirectToAction(nameof(Index));
            }

            // Gửi dữ liệu
            ViewBag.Friend = _UserManager.Get(yourFriendId);

            // Lấy danh sách các tin nhắn trước đó
            var dums = _DiscussionUserMessageManager.GetAllForDiscuss(discussion.Id);

            // Tạo danh sách đối tượng mà JS có thể hiểu được
            List<MessageObject> dumsJsonArray = new List<MessageObject>();

            foreach (DiscussionUserMessage dum in dums)
            {
                var messageObj = new MessageObject
                {
                    Message = dum.Message,
                    SenderId = dum.DiscussionUser.UserId,
                    Time = dum.CreatedTime?.ToLocalTime().ToString("hh:mm:ss dd/MM/yyyy")
                };
                dumsJsonArray.Add(messageObj);
            }

            // Chuyển thành JSON cho javascript hiểu
            ViewBag.DumJsonArray = JsonConvert.SerializeObject(dumsJsonArray);

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
