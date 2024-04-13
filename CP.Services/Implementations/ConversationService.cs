using CP.Data.Repositories.Interfaces;
using CP.Models.Models;
using CP.Services.Interfaces;

namespace CP.Services.Implementations
{
    public class ConversationService(IConversationRepository conversationRepository) : IConversationService
    {
        private readonly IConversationRepository _conversationRepository = conversationRepository;

        public async Task<List<ConversationSummaryDto>> GetRecentChatsAsync(string userId)
             => await _conversationRepository.GetRecentChatsAsync(userId);
    }
}
