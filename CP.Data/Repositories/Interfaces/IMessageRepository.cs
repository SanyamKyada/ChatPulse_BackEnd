using CP.Models.Entities;
using CP.Models.Models;

namespace CP.Data.Repositories.Interfaces
{
    public interface IMessageRepository: IGenericRepository<Message>
    {
        Task<IEnumerable<MessageDto>> GetByConversationAsync(int conversationId, string userId, int skip, int take);

        Task InsertMessage(int conversationId, string content, string senderId);
        Task SetMessageSeen(int conversationId, string seenByUserId);
    }
}
