using CP.Models.Entities;
using CP.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CP.Services.Implementations
{
    public class UserService(UserManager<ApplicationUser> userManager) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
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
    }
}
