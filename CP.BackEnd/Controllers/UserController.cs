using CP.Models.Models;
using CP.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CP.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpPost("set-availability-status")]
        public async Task<ActionResult<Status>> SetAvailabilityStatus(AvailabilityStatusModel availabilityStatus)
        {
            var response = await _userService.SetAvailabilityStatus(availabilityStatus);

            if(response.StatusCode == 500)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return Ok(response);
        }

        [HttpPost("{userId}/image-upload")]
        public async Task<ActionResult> UploadProfileImage(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            userId = userId ?? User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if(string.IsNullOrEmpty(userId))
                return BadRequest("UserId can't be null.");

            var response = await _userService.SaveFileAsync(file, userId);
            return Ok(new {statusCode = response.StatusCode, message = response.Message, filePath = response.Data});
        }
    }
}
