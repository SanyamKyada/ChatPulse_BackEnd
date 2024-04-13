using CP.Data.Repositories.Interfaces;
using CP.Models.Models;
using CP.Services.Interfaces;

namespace CP.Services.Implementations
{
    public class MessageService(IMessageRepository messageRepository) : IMessageService
    {
        private readonly IMessageRepository _messageRepository = messageRepository;

        public async Task<IEnumerable<MessageDto>> GetMessagesInChunkAsync(int conversationId, string userId, int skip, int take)
          => await _messageRepository.GetByConversationAsync(conversationId, userId, skip, take);
    }
}
