using CP.Models.Entities;
using CP.Models.Models;
using CP.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CP.API.Controllers
{
    [Route("api/friend-request")]
    [ApiController]
    public class FriendRequestController : ControllerBase
    {
        private readonly IFriendRequestService _friendRequestService;

        public FriendRequestController(IFriendRequestService friendRequestService)
        {
            _friendRequestService = friendRequestService;
        }

        [HttpGet("{friendRequestId}/get-messages")]
        public async Task<ActionResult<FriendRequestWithMessagesDto>> GetFriendRequestMessages(int friendRequestId)
        {
            return Ok(await _friendRequestService.GetFriendRequestWithMessages(friendRequestId));
        }
    }
}
