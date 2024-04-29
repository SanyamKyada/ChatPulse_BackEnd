using CP.Services.Interfaces;
using CP.SignalR.DataService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CP.SignalR.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub(SharedDb shared, IUserService userService) : Hub
    {

        private readonly IUserService _userService = userService;

        public async Task SendMessage(string receiverUserId, string message, int conversationId)
        {
            //string senderUserId = Context.User?.Identity?.Name;
            string senderUserId = Context.User?.Claims.FirstOrDefault(x => x.Type ==    ClaimTypes.NameIdentifier)?.Value;

            await _messageService.InsertMessage(conversationId, message, senderUserId);

            await Clients.User(receiverUserId).SendAsync("ReceiveMessage", senderUserId, message);
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

        private async Task NotifyContactsOfStatusChange(string userId, bool isOnline)
        {
            var onlineContacts = await GetOnlineContacts(userId);
            foreach (var contactId in onlineContacts)
            {
                await Clients.User(contactId).SendAsync("UserStatusChanged", userId, isOnline);
            }
        }

        private async Task<IEnumerable<string>> GetOnlineContacts(string userId) => 
            await _userService.GetOnlineContacts(userId);
    }
}
