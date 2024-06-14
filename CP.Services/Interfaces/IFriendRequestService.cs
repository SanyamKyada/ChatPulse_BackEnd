using CP.Models.Models;

namespace CP.Services.Interfaces
{
    public interface IFriendRequestService
    {
        Task<int> SendFriendRequest(FriendRequestDto request);
        Task InsertFriendRequestMessage(int friendRequestId, string message);
        Task<FriendRequestWithMessagesDto> GetFriendRequestWithMessages(int id);
        Task<Tuple<Status, int>> AcceptFriendRequest(int friendRequestId);
    }
}
