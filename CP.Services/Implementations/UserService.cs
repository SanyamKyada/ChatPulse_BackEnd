using CP.Data.Repositories.Interfaces;
using CP.Models.Entities;
using CP.Models.Models;
using CP.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CP.Services.Implementations
{
    public class UserService(UserManager<ApplicationUser> _userManager, IConversationRepository _conversationRepository) : IUserService
    {
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

        public async Task<List<ContactSearchDto>> SearchPeople(string userId, string query, CancellationToken cancellationToken)
        {
            var contactIds = await _conversationRepository.GetAllContactsAsync(userId);
            return await _userManager.Users
                .Where(x =>
                    x.Id != userId
                    && !contactIds.Contains(x.Id)
                    //&& (x.FirstName.ToLower().StartsWith(query.ToLower()) || x.LastName.ToLower().StartsWith(query.ToLower())))
                    && x.Name.ToLower().Contains(query.ToLower()))
                .Select(z => new ContactSearchDto()
                {
                    UserId = z.Id,
                    Name = z.Name,
                    IsOnline = z.IsOnline,
                    LastSeenTimestamp = z.LastSeenTimestamp,
                    ProfileImage = z.ProfileImage,
                    IsRequestAlreadySent = z.FriendRequestReceiver.Any(x => x.SenderUserId == userId && x.Status == FriendRequestStatus.Pending)
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<ContactSearchDto> GetFriendRequestSenderUser(string userId)
        {
            return await _userManager.Users
                .Where(x => x.Id == userId)
                .Select(z => new ContactSearchDto()
                {
                    UserId = z.Id,
                    Name = z.Name,
                    IsOnline = z.IsOnline,
                    LastSeenTimestamp = z.LastSeenTimestamp,
                    ProfileImage = z.ProfileImage,
                    IsRequestAlreadySent = z.FriendRequestReceiver.Any(x => x.SenderUserId == userId && x.Status == FriendRequestStatus.Pending)
                })
                .FirstOrDefaultAsync() ?? new ContactSearchDto();
        }

        public async Task<Status> SetAvailabilityStatus(AvailabilityStatusModel availabilityStatus)
        {
            var status = new Status();
            var user = await _userManager.FindByIdAsync(availabilityStatus.UserId);

            if (user == null)
            {
                status.StatusCode = 500;
                status.Message = "User not found while updating availability status";
                return status;
            }
            user.AvailabilityStatus = availabilityStatus.Status;
            await _userManager.UpdateAsync(user);

            status.StatusCode = 200;
            status.Message = "Status updated successfuly";
            return status;
        }
    }
}
