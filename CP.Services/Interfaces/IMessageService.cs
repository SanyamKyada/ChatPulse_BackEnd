using CP.Models.Models;

namespace CP.Services.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDto>> GetMessagesInChunkAsync(int conversationId, string userId, int skip, int take);

        Task InsertMessage(int conversationId, string content, string senderId);
        Task SetMessageSeen(int conversationId, string seenByUserId);
    }
}
