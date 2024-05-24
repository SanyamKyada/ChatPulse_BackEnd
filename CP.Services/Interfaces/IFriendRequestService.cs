using CP.Models.Entities;
using CP.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.Services.Interfaces
{
    public interface IFriendRequestService
    {
        Task<int> SendFriendRequest(FriendRequestDto request);
        Task InsertFriendRequestMessage(int friendRequestId, string message);
        Task<FriendRequestWithMessagesDto> GetFriendRequestWithMessages(int id);
    }
}
