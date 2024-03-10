using CP.Models.Models;
using CP.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Net;
using System.Security.Claims;
using System.Text;
using CP.Data.Domain;
using CP.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CP.BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IAccountService accountService, IConfiguration config, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _accountService =  accountService;
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            if (string.IsNullOrEmpty(model.Role))
            {
                model.Role = "User";
            }

            var result = await _accountService.RegisterAsync(model);

            /*return Ok(result.Message);*/
            if (result.StatusCode == 1)
            {
                return Ok(new { from = "register", HttpStatusCode = 200 });
            }
            else
            {
                return BadRequest(new { Message = result.Message ?? "Registration failed", HttpStatusCode = HttpStatusCode.BadRequest });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (model.Email is null || model.Password is null)
            {
                return BadRequest("Email or password is missing.");
            }

            try
            {
                var result = await _accountService.LoginAsync(model);
                if (result.StatusCode == 1)
                {
                    // Get the user
                    var user = await _userManager.FindByNameAsync(model.Email);

                    //Get the role
                    var roles = await _userManager.GetRolesAsync(user);

                    // Create the claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim("securityStamp", user.SecurityStamp)
                    };
                    //if (roles.Contains("Admin"))
                    //{
                    //    claims.Add(new Claim("IsAdmin", "true"));
                    //}

                    // Create the JWT token
                    var jwtSettings = _config.GetSection("Jwt").Get<JwtSettings>();
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var expires = DateTime.Now.AddMinutes(jwtSettings.ExpiresInMinutes);

                    var token = new JwtSecurityToken(
                        jwtSettings.Issuer,
                        jwtSettings.Audience,
                        claims,
                    expires: expires,
                        signingCredentials: credentials
                    );

                    // JWT token as a response
                    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
                }
                else
                {
                    return Unauthorized(new { Message = result.Message ?? "Could not log in" });
                }

            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
