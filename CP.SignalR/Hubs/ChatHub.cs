using CP.Services.Interfaces;
using CP.SignalR.DataService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CP.SignalR.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub(SharedDb shared, IUserService userService) : Hub
    {

        private readonly IUserService _userService = userService;

        public async Task SendMessage(string receiverUserId, string message)
        {
            string senderUserId = Context.User?.Identity?.Name;

            await Clients.User(receiverUserId).SendAsync("ReceiveMessage", senderUserId, message);
        }

        public async override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            await _userService.SetUserStatusAsync(userId, true);
            base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            await _userService.SetUserStatusAsync(userId, false);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
