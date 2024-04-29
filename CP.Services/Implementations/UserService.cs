using CP.Data.Repositories.Interfaces;
using CP.Models.Entities;
using CP.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CP.Services.Implementations
{
    public class UserService(UserManager<ApplicationUser> userManager, IConversationRepository conversationRepository) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IConversationRepository _conversationRepository = conversationRepository;
        private static TimeSpan India_Standard_Time_Offset = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);
        public async Task SetUserStatusAsync(string userId, bool isOnline)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsOnline = isOnline;
                if (!isOnline)
                {
                    DateTime indianTime = DateTime.UtcNow + India_Standard_Time_Offset;
                    user.LastSeenTimestamp = indianTime;
                }
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<List<string>> GetOnlineContacts(string userId) => 
            await _conversationRepository.GetOnlineContactsAsync(userId);
    }
}
