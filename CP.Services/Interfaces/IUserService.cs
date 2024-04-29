﻿namespace CP.Services.Interfaces
{
    public interface IUserService
    {
        Task SetUserStatusAsync(string userId, bool isOnline);
        Task<List<string>> GetOnlineContacts(string userId);
    }
}
