using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Hubs
{
    // Refs https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-3.1
    public class ChatHub : Hub
    {
        private readonly UserManager _UserManager;
        private readonly DiscussionManager _DiscussionManager;
        private readonly DiscussionUserManager _DiscussionUserManager;
        private readonly DiscussionUserMessageManager _DiscussionUserMessageManager;

        public ChatHub(
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

        #region HubMethods

        public Task SendMessage(string discussId, string message, string senderId)
        {
            // Bỏ qua khi không biết ai gửi
            if (senderId.ToInt() <= 0)
                return Clients.All.SendAsync("Drop");

            // Bỏ qua khi không biết cuộc thảo luận nào
            if (discussId.ToInt() <= 0)
                return Clients.All.SendAsync("Drop");

            // Nếu người dùng không thuộc nhóm cũng bỏ qua
            if (_DiscussionManager.IsIn(discussId.ToLong(), senderId.ToLong()))
                return Clients.All.SendAsync("Drop");

            // Lấy mã Du
            DiscussionUser du = _DiscussionUserManager.GetBy(discussId.ToInt(), senderId.ToInt());

            // Lưu tín nhắn
            DiscussionUserMessage dum = new DiscussionUserMessage
            {
                SenderId = du.Id,
                Message = message
            };
            _DiscussionUserMessageManager.Add(dum);

            return Clients.All.SendAsync("ReceiveMessage", discussId, new
            {
                message,
                senderId,
                date = dum.CreatedTime?.ToString("hh:mm:ss dd/MM/yyyy")
            });
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public Task SendMessageToGroup(string message)
        {
            return Clients.Group("SignalR Users").SendAsync("ReceiveMessage", message);
        }

        public Task SendMessageToMultipleUser(string message, params string[] userIds)
        {
            return Clients.Users(userIds).SendAsync("ReceiveMessage", message);
        }
        #endregion

        #region HubMethodName
        [HubMethodName("SendMessageToUser")]
        public Task DirectMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveMessage", message);
        }

        #endregion

        #region ThrowHubException
        public Task ThrowException()
        {
            throw new HubException("This error will be sent to the client!");
        }
        #endregion

        #region OnConnectedAsync
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }
        #endregion

        #region OnDisconnectedAsync
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }
        #endregion}
    }
}
