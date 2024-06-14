using CP.Data.Domain;
using CP.Data.Repositories.Interfaces;
using CP.Models.Entities;
using CP.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace CP.Data.Repositories.Implementations
{
    public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
    {
        private readonly CPDatabaseContext _dbContext;
        private readonly IFriendRequestRepository _friendRequestRepository;
        public ConversationRepository(CPDatabaseContext dbContext, IFriendRequestRepository friendRequestRepository) : base(dbContext)
        {
            _dbContext = dbContext;
            _friendRequestRepository = friendRequestRepository;
        }

        public async Task<List<ConversationSummaryDto>> GetRecentChatsAsync(string userId)
        {
            var recentConversations = await _dbContext.Conversations
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .Select(c => new ConversationSummaryDto
                {
                    ConversationId = c.Id,
                    NumberOfUnseenMessages = c.Messages.Count(m => m.SenderId != userId && m.SeenByUserId == null
                        && m.Timestamp > (c.User1Id == userId ? c.User2LastSeen : c.User1LastSeen)),
                    Contact = c.User1Id == userId ? new ContactDto
                    {
                        ContactId = c.User2Id,
                        Name = c.User2.Name,
                        ProfileImage = c.User2.ProfileImage,
                        IsOnline = c.User2.IsOnline,
                        LastSeenTimestamp = c.User2.LastSeenTimestamp,
                        IsFriend = true

                    } : new ContactDto
                    {
                        ContactId = c.User1Id,
                        Name = c.User1.Name,
                        ProfileImage = c.User1.ProfileImage,
                        IsOnline = c.User1.IsOnline,
                        LastSeenTimestamp = c.User1.LastSeenTimestamp,
                        IsFriend = true
                    },
                    //ConversationType = c.ConversationType,
                    LastMessage = c.Messages.Count() > 0 ? new MessageDto
                    {
                        MessageId = c.Messages.OrderByDescending(m => m.Timestamp).First().Id,
                        Content = c.Messages.OrderByDescending(m => m.Timestamp).First().Content,
                        Timestamp = c.Messages.OrderByDescending(m => m.Timestamp).First().Timestamp
                    }
                    : 
                    _friendRequestRepository.GetIQ().Where(x => x.Status == FriendRequestStatus.Accepted && (c.User1Id == x.SenderUserId && c.User2Id == x.ReceiverUserId) ||
                        (c.User1Id == x.ReceiverUserId && c.User2Id == x.SenderUserId))
                        .Select(z => new MessageDto()
                        {
                            IsWave = true,
                            Timestamp = z.RequestTimeStamp
                        }).FirstOrDefault()
                })
                .ToListAsync();

            var friendRequests = await _dbContext.FriendRequests
                .Where(x => (x.SenderUserId == userId || x.ReceiverUserId == userId) && x.Status == FriendRequestStatus.Pending)
                .Select(fr => new ConversationSummaryDto()
                {
                    FriendRequestId = fr.Id,
                    NumberOfUnseenMessages = 1,
                    Contact = fr.SenderUserId == userId ? new ContactDto
                    {
                        ContactId = fr.ReceiverUserId,
                        Name = fr.ReceiverUser.Name,
                        ProfileImage = fr.ReceiverUser.ProfileImage,
                        IsOnline = fr.ReceiverUser.IsOnline,
                        LastSeenTimestamp = fr.ReceiverUser.LastSeenTimestamp,
                        IsFriend = false

                    } : new ContactDto
                    {
                        ContactId = fr.SenderUserId,
                        Name = fr.SenderUser.Name,
                        ProfileImage = fr.SenderUser.ProfileImage,
                        IsOnline = fr.SenderUser.IsOnline,
                        LastSeenTimestamp = fr.SenderUser.LastSeenTimestamp,
                        IsFriend = false
                    },
                    LastMessage = fr.FriendRequestMessages.Count() > 0
                        ? new MessageDto
                    {
                        MessageId = fr.FriendRequestMessages.OrderByDescending(m => m.Timestamp).First().Id,
                        Content = fr.FriendRequestMessages.OrderByDescending(m => m.Timestamp).First().Content,
                        Timestamp = fr.FriendRequestMessages.OrderByDescending(m => m.Timestamp).First().Timestamp
                    }
                        : new MessageDto
                        {
                            IsWave = true,
                            Timestamp = fr.RequestTimeStamp
                        }
                })
                .ToListAsync();

            recentConversations.AddRange(friendRequests);

            return recentConversations.OrderByDescending(conversation => conversation.LastMessage.Timestamp).ToList();
        }

        public async Task<List<string>> GetOnlineContactsAsync(string userId) =>
            await _dbContext.Conversations
                    .Where(c => (c.User1Id == userId || c.User2Id == userId) 
                        && (c.User1Id == userId ? c.User2.IsOnline : c.User1.IsOnline))
                    .Select(x => x.User1Id == userId ? x.User2Id : x.User1Id)
                    .ToListAsync();

        public async Task<List<string>> GetAllContactsAsync(string userId) =>
            await _dbContext.Conversations
                    .Where(c => c.User1Id == userId || c.User2Id == userId)
                    .Select(x => x.User1Id == userId ? x.User2Id : x.User1Id)
                    .ToListAsync();
    }
}
