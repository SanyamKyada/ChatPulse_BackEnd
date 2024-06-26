﻿using CP.Models.Entities;
using CP.Models.Models;

namespace CP.Data.Repositories.Interfaces
{
    public interface IConversationRepository: IGenericRepository<Conversation>
    {
        Task<List<ConversationSummaryDto>> GetRecentChatsAsync(string userId);
        Task<List<string>> GetOnlineContactsAsync(string userId);
        Task<List<string>> GetAllContactsAsync(string userId);
    }
}
