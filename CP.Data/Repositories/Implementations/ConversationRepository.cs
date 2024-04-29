using CP.Data.Domain;
using CP.Data.Repositories.Interfaces;
using CP.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace CP.Data.Repositories.Implementations
{
    public class ConversationRepository(CPDatabaseContext dbContext) : IConversationRepository
    {
        private readonly CPDatabaseContext _dbContext = dbContext;

        public async Task<List<ConversationSummaryDto>> GetRecentChatsAsync(string userId)
        {
            var recentConversations = await _dbContext.Conversations
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .Select(c => new ConversationSummaryDto
                {
                    ConversationId = c.Id,
                    NumberOfUnseenMessages = c.Messages.Count(m => m.SeenByUserId != userId 
                        && m.Timestamp > (c.User1Id == userId ? c.User2LastSeen : c.User1LastSeen)),
                    Contact = c.User1Id == userId ? new ContactDto
                    {
                        ContactId = c.User2Id,
                        Name = c.User2.Name,
                        ProfileImage = c.User2.ProfileImage,
                        IsOnline = c.User2.IsOnline,
                        LastSeenTimestamp = c.User2.LastSeenTimestamp,
                        
                    } : new ContactDto
                    {
                        ContactId = c.User1Id,
                        Name = c.User1.Name,
                        ProfileImage = c.User1.ProfileImage,
                        IsOnline = c.User1.IsOnline,
                        LastSeenTimestamp = c.User1.LastSeenTimestamp,
                    },
                    //ConversationType = c.ConversationType,
                    LastMessage = new MessageDto
                    {
                        MessageId = c.Messages.OrderByDescending(m => m.Timestamp).First().Id,
                        Content = c.Messages.OrderByDescending(m => m.Timestamp).First().Content,
                        Timestamp = c.Messages.OrderByDescending(m => m.Timestamp).First().Timestamp
                    }
                })
                .ToListAsync();

            return recentConversations;
        }

        public async Task<List<string>> GetOnlineContactsAsync(string userId) =>
            await _dbContext.Conversations
                    .Where(c => (c.User1Id == userId || c.User2Id == userId) 
                        && (c.User1Id == userId ? c.User2.IsOnline : c.User1.IsOnline))
                    .Select(x => x.User1Id == userId ? x.User2Id : x.User1Id)
                    .ToListAsync();
    }
}
