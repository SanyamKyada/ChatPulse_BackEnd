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
    }
}
