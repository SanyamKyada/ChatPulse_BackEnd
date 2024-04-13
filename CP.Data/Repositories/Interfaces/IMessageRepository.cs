using CP.Models.Models;

namespace CP.Data.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<MessageDto>> GetByConversationAsync(int conversationId, string userId, int skip, int take);
    }
}
