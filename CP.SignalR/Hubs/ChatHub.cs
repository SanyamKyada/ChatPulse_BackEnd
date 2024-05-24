using CP.Models.Models;
using CP.Services.Interfaces;
using CP.SignalR.Constants;
using CP.SignalR.DataService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CP.SignalR.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub(SharedDb shared, IUserService userService, IMessageService messageService, IFriendRequestService _friendRequestService) : Hub
    {

        private readonly IUserService _userService = userService;
        private readonly IMessageService _messageService = messageService;

        public async Task SendMessage(string receiverUserId, string message, int conversationId)
        {
            string senderUserId = Context.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            await _messageService.InsertMessage(conversationId, message, senderUserId);

            await Clients.User(receiverUserId).SendAsync(SignalRClient.ReceiveMessage, senderUserId, message);
        }

        public async Task NotifyTyping()
        {
            string userId = Context.User?.Claims.FirstOrDefault(x => x.Type ==    ClaimTypes.NameIdentifier)?.Value;
            var onlineContacts = await GetOnlineContacts(userId);
            foreach (var contactId in onlineContacts)
            {
                await Clients.User(contactId).SendAsync(SignalRClient.ReceiveTypingNotification, userId);
            }
        }

        public async Task<int> SendFriendRequest(FriendRequestDto requestDto)
        {
            string senderUserId = Context.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            requestDto.UserId = senderUserId;

            var friendRequestId = await _friendRequestService.SendFriendRequest(requestDto);
            if(!requestDto.HasWaved && requestDto.Message != null)
            {
                await _friendRequestService.InsertFriendRequestMessage(friendRequestId, requestDto.Message);
            }
            var senderUser = await userService.GetFriendRequestSenderUser(senderUserId);

            await Clients.User(requestDto.ReceiverUserId).SendAsync(SignalRClient.ReceiveFriendRequest, friendRequestId, senderUser );
            return friendRequestId;
        }

        public async Task SendFriendRequestMessage(int friendRequestId, string receiverUserId, string message)
        {
            string senderUserId = Context.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if(friendRequestId != null && message != null)
                await _friendRequestService.InsertFriendRequestMessage(friendRequestId, message);

            await Clients.User(receiverUserId).SendAsync(SignalRClient.ReceiveFriendRequestMessage, senderUserId, friendRequestId, message );
        }

        public async override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            await _userService.SetUserStatusAsync(userId, true);
            await NotifyContactsOfStatusChange(userId, true);
            base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            await _userService.SetUserStatusAsync(userId, false);
            await NotifyContactsOfStatusChange(userId, false);
            await base.OnDisconnectedAsync(exception);
        }

        #region Private Methods

        private async Task NotifyContactsOfStatusChange(string userId, bool isOnline)
        {
            var onlineContacts = await GetOnlineContacts(userId);
            foreach (var contactId in onlineContacts)
            {
                await Clients.User(contactId).SendAsync(SignalRClient.UserStatusChanged, userId, isOnline);
            }
        }

        private async Task<IEnumerable<string>> GetOnlineContacts(string userId) => 
            await _userService.GetOnlineContacts(userId);

        #endregion

    }
}
