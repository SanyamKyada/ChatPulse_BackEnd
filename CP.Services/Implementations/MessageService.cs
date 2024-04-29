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

        public async Task InsertMessage(int conversationId, string content, string senderId)
            => await _messageRepository.InsertMessage(conversationId, content, senderId);

        public async Task SetMessageSeen(int conversationId, string seenByUserId)
            => await _messageRepository.SetMessageSeen(conversationId, seenByUserId);
    }
}
