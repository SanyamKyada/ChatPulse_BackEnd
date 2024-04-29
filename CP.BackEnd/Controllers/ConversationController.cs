using CP.Models.Models;
using CP.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CP.API.Controllers
{
    [Route("api/conversation")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;
        private readonly ILogger<ConversationController> _logger;
        public ConversationController(IMessageService messageService, IConversationService conversationService, ILogger<ConversationController> logger)
        {
            _messageService = messageService;
            _conversationService = conversationService;
            _logger = logger;
        }

        [HttpGet("{userId}/recent")]
        public async Task<ActionResult<IEnumerable<ConversationSummaryDto>>> GetRecentConversations(string userId)
        {
            try
            {
                var recentChats = await _conversationService.GetRecentChatsAsync(userId);
                return Ok(recentChats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving recent chats for user: {userId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving messages for this conversation.");
            }
        }

        [HttpGet("{conversationId}/messages")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetConversationMessages(int conversationId, string userId, int skip = 0, int take = 20)
        {
            try
            {
                var messages = await _messageService.GetMessagesInChunkAsync(conversationId, userId, skip, take);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving messages for conversation: {conversationId}", conversationId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving messages for this conversation.");
            }
        }

        [HttpPut("{conversationId}/messages/seen/{seenByUserId}")]
        public async Task UpdateMessageStatus(int conversationId, string seenByUserId)
            => await _messageService.SetMessageSeen(conversationId, seenByUserId);
    }
}
