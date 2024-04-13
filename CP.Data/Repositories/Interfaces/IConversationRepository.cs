﻿using CP.Models.Models;

namespace CP.Data.Repositories.Interfaces
{
    public interface IConversationRepository
    {
        Task<List<ConversationSummaryDto>> GetRecentChatsAsync(string userId);
    }
}