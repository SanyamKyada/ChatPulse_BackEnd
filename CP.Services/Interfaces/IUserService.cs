﻿using CP.Models.Models;
using Microsoft.AspNetCore.Http;

namespace CP.Services.Interfaces
{
    public interface IUserService
    {
        Task SetUserStatusAsync(string userId, bool isOnline);
        Task<List<string>> GetOnlineContacts(string userId);
        Task<List<ContactSearchDto>> SearchPeople(string userId, string query, CancellationToken cancellationToken);
        Task<ContactSearchDto> GetFriendRequestSenderUser(string userId);
        Task<Status> SetAvailabilityStatus(AvailabilityStatusModel availabilityStatus);

        Task<ServiceResponse<string>> SaveFileAsync(IFormFile file, string userId);
    }
}
