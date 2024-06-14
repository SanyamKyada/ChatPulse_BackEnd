using CP.Data.Repositories.Interfaces;
using CP.Models.Entities;
using CP.Models.Models;
using CP.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CP.Services.Implementations
{
    public class FriendRequestService(IFriendRequestRepository _friendRequestRepository, IFriendRequestMessageRepository _friendRequestMessageRepository, IConversationRepository _conversationRepository, IMessageRepository _messageRepository) : IFriendRequestService
    {
        private static TimeSpan India_Standard_Time_Offset = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);

        public async Task<int> SendFriendRequest(FriendRequestDto request)
        {
            DateTime indianTime = DateTime.UtcNow + India_Standard_Time_Offset;

            var friendReq = new FriendRequest()
            {
                SenderUserId = request.UserId,
                ReceiverUserId = request.ReceiverUserId,
                Status = FriendRequestStatus.Pending,
                RequestTimeStamp = indianTime,
                HasWaved = request.HasWaved,
            };
            _friendRequestRepository.Insert(friendReq);
            await _friendRequestRepository.Save();

            return friendReq.Id;
        }

        public async Task InsertFriendRequestMessage(int friendRequestId, string message)
        {
            DateTime indianTime = DateTime.UtcNow + India_Standard_Time_Offset;

            var friendReqMessage = new FriendRequestMessage()
            {
                Content = message,
                FriendRequestId = friendRequestId,
                Timestamp = indianTime
            };
            _friendRequestMessageRepository.Insert(friendReqMessage);
            await _friendRequestMessageRepository.Save();
        }

        public async Task<FriendRequestWithMessagesDto> GetFriendRequestWithMessages(int id)
        {
            var data = await _friendRequestRepository.GetIQ().Include(x => x.FriendRequestMessages).FirstOrDefaultAsync(x => x.Id == id);
            return new FriendRequestWithMessagesDto()
            {
                Id = data.Id,
                HasWaved = data.HasWaved,
                ReceiverUserId = data.ReceiverUserId,
                SenderUserId = data.SenderUserId,
                Status = data.Status,
                RequestTimeStamp = data.RequestTimeStamp,
                FriendRequestMessages = data.FriendRequestMessages.Select(x => new FriendRequestMessagesDto()
                {
                    Id = x.Id,
                    Content = x.Content,
                    Timestamp = x.Timestamp
                }).ToList()
            };
        }

        public async Task<Tuple<Status, int>> AcceptFriendRequest(int friendRequestId)
        {
            var status = new Status();
            var friendReq = await _friendRequestRepository.GetById(friendRequestId);
            if (friendReq == null)
            {
                status.StatusCode = 0;
                status.Message = "Friend request not found";
                return Tuple.Create(status, 0);
            }

            var newConversation = new Conversation()
            {
                User1Id = friendReq.SenderUserId,
                User2Id = friendReq.ReceiverUserId,
                User1LastSeen = DateTime.Now,
                User2LastSeen = DateTime.Now,
            };
            _conversationRepository.Insert(newConversation);
            await _conversationRepository.Save();

            var friendReqMessages = _friendRequestMessageRepository.Get(filter: x => x.FriendRequestId == friendRequestId);

            if (friendReqMessages != null && friendReqMessages.Count() > 0)
            {
                var conversationMessages = friendReqMessages.Select(x => new Message()
                {
                    Content = x.Content,
                    ConversationId = newConversation.Id,
                    SenderId = friendReq.SenderUserId,
                    Timestamp = x.Timestamp
                });
                _messageRepository.InsertRange(conversationMessages.ToList());
                await _messageRepository.Save();
            }

            friendReq.Status = FriendRequestStatus.Accepted;
            _friendRequestRepository.Update(friendReq);
            await _friendRequestRepository.Save();

            status.StatusCode = 1;
            status.Message = "New conversation established successfuly";
            return Tuple.Create(status, newConversation.Id);

        }
    }
}
