using CP.Models.Models;
using CP.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using CP.Models.Entities;

namespace CP.BackEnd.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IRefereshTokenService _refereshTokenService;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IAccountService accountService,IRefereshTokenService refereshTokenService, IConfiguration config, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _accountService = accountService;
            _refereshTokenService = refereshTokenService;
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
        public async Task<IActionResult> Login(LoginModel userCred)
        {
            if (userCred.Email is null || userCred.Password is null)
            {
                return BadRequest("Email or password is missing.");
            }

            try
            {
                var result = await _accountService.LoginAsync(userCred);
                if (result.StatusCode == 1)
                {
                    // Get the user
                    var user = await _userManager.FindByNameAsync(userCred.Email);

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
                    return Ok(new { 
                        jwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                        refreshToken = await _refereshTokenService.GenerateToken(user.Id),
                        userId = user.Id ,
                        userN = user.Name
                    });
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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefToken([FromBody] TokenResponse tokenResponse)
        {

            /// Generate Token
            var tokenhandler = new JwtSecurityTokenHandler();
            var jwtSettings = _config.GetSection("Jwt").Get<JwtSettings>();
            var tokenkey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
            SecurityToken securityToken;
            var principal = tokenhandler.ValidateToken(tokenResponse.jwttoken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenkey),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = false 

            }, out securityToken);

            var token = securityToken as JwtSecurityToken;
            if (token != null && !token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                return Unauthorized();
            }
            var username = principal.Identity?.Name;
            var userId = principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            var exists = _refereshTokenService.TokenExists(userId, tokenResponse.refreshtoken);
            if (!exists == null)
                return Unauthorized();

            var response = TokenAuthenticate(username, principal.Claims.ToArray()).Result;

            return Ok(response);
        }

        [NonAction]
        public async Task<TokenResponse> TokenAuthenticate(string user, Claim[] claims)
        {
            var jwtSettings = _config.GetSection("Jwt").Get<JwtSettings>();

            var token = new JwtSecurityToken(
              claims: claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)), SecurityAlgorithms.HmacSha256)
            );

            var jwttoken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponse() { 
                jwttoken = jwttoken,
                refreshtoken = await _refereshTokenService.GenerateToken(user) 
            };

        }
    }
}
