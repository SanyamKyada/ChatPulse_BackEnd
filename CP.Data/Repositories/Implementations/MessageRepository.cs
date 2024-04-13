using CP.Data.Domain;
using CP.Data.Repositories.Interfaces;
using CP.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace CP.Data.Repositories.Implementations
{
    public class MessageRepository(CPDatabaseContext dbContext) : IMessageRepository
    {
        private readonly CPDatabaseContext _dbContext = dbContext;

        public async Task<IEnumerable<MessageDto>> GetByConversationAsync(int conversationId, string userId, int skip, int take)
        {
            var messages = await _dbContext.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.Timestamp)
                .Skip(skip)
                .Take(take)
                .Select(m => new MessageDto
                {
                    MessageId = m.Id,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    IsFromCurrentUser = m.SenderId == userId
                })
                .ToListAsync();

            return messages;
        }
    }
}
