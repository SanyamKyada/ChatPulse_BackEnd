using CP.Data.Domain;
using CP.Data.Repositories.Interfaces;
using CP.Models.Entities;
using CP.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace CP.Data.Repositories.Implementations
{
    public class MessageRepository: GenericRepository<Message>, IMessageRepository
    {
        private readonly CPDatabaseContext _dbContext;
        public MessageRepository(CPDatabaseContext dbContext): base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<MessageDto>> GetByConversationAsync(int conversationId, string userId, int skip, int take)
        {
            var messages = await _dbContext.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.Timestamp)
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

        public async Task InsertMessage(int conversationId, string content, string senderId)
        {
            var message = new Message() {
                Content = content,
                SenderId = senderId,
                ConversationId = conversationId,
                Timestamp = DateTime.Now
            };

            await _dbContext.Messages.AddAsync(message);
            _dbContext.SaveChanges();
        }

        public async Task SetMessageSeen(int conversationId, string seenByUserId)
        {
            var conversation = await _dbContext.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId);

            if(conversation.User1Id == seenByUserId)
            {
                conversation.User1LastSeen = DateTime.Now;
            }
            else
            {
                conversation.User2LastSeen = DateTime.Now;
            }
            _dbContext.Update(conversation);

            var messages = await _dbContext.Messages.Where(x => x.ConversationId == conversationId && x.SeenByUserId == null).ToListAsync();

            messages.ForEach(x =>
            {
                x.SeenByUserId = seenByUserId;
                x.SeenAt = DateTime.Now;
            });

            _dbContext.UpdateRange(messages);

            await _dbContext.SaveChangesAsync();
        }
    }
}
