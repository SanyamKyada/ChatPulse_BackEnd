using CP.Models.Models;
using CP.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CP.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{userId}/search-people")]
        public async Task<ActionResult<IEnumerable<ContactSearchDto>>> SearchPeople(string userId, [FromQuery]string query, CancellationToken cancellationToken)
        {
            userId = userId ?? User.Claims.FirstOrDefault(x => x.Type ==    ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _userService.SearchPeople(userId, query, cancellationToken));
        }

        [HttpPost("send-friend-request")]
        public async Task<ActionResult<Status>> SendFriendRequest([FromBody]FriendRequestDto requestDto)
        {
            try
            {
                await _userService.SendFriendRequest(requestDto);
                return Ok(new Status() { StatusCode = 200, Message = "Friend request sent successfully"});
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while sending friend request: {ex.Message}");
            }
        }
    }
}
