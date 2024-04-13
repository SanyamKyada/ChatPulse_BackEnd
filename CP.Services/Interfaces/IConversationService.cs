using CP.Models.Models;

namespace CP.Services.Interfaces
{
    public interface IConversationService
    {
        Task<List<ConversationSummaryDto>> GetRecentChatsAsync(string userId);
    }
}
