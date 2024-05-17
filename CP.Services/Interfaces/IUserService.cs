using CP.Models.Models;

namespace CP.Services.Interfaces
{
    public interface IUserService
    {
        Task SetUserStatusAsync(string userId, bool isOnline);
        Task<List<string>> GetOnlineContacts(string userId);
        Task<List<ContactSearchDto>> SearchPeople(string userId, string query, CancellationToken cancellationToken);
        Task SendFriendRequest(FriendRequestDto request);
    }
}
