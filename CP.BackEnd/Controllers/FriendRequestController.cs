using CP.Models.Models;
using CP.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CP.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/friend-request")]
    [ApiController]
    public class FriendRequestController : ControllerBase
    {
        private readonly IFriendRequestService _friendRequestService;

        public FriendRequestController(IFriendRequestService friendRequestService)
        {
            _friendRequestService = friendRequestService;
        }

        [HttpPost("accept-friend-request")]
        public async Task<ActionResult> AcceptFriendRequest([FromBody]int friendRequestId)
        {
            var result = await _friendRequestService.AcceptFriendRequest(friendRequestId);
            if(result.Item1.StatusCode == 1)
            {
                return Ok(new { ConversationId = result.Item2 });
            }

            return BadRequest(result.Item1);
        }

        [HttpGet("{friendRequestId}/get-messages")]
        public async Task<ActionResult<FriendRequestWithMessagesDto>> GetFriendRequestMessages(int friendRequestId)
        {
            return Ok(await _friendRequestService.GetFriendRequestWithMessages(friendRequestId));
        }
    }
}
