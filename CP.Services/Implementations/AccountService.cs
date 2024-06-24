using CP.Models.Entities;
using CP.Models.Models;
using CP.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CP.Services.Implementations
{
    public class AccountService( UserManager<ApplicationUser> _userManager, 
        RoleManager<IdentityRole> _roleManager, SignInManager<ApplicationUser> _signInManager ) : IAccountService
    {
        public async Task<Status> RegisterAsync(RegistrationModel model)
        {
            var status = new Status();
            var userNameExists = await _userManager.FindByNameAsync(model.Username);
            if (userNameExists != null)
            {
                status.StatusCode = 0;
                status.Message = "Username already exist";
                return status;
            }
            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
            {
                status.StatusCode = 0;
                status.Message = "Email already exist";
                return status;
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                LastName = model.LastName,
                FirstName = model.FirstName,
                Name = model.LastName + ' ' + model.FirstName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";
                return status;
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            if (await _roleManager.RoleExistsAsync(model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            status.StatusCode = 1;
            status.Message = "You have registered successfully";
            return status;
        }

        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid UserName";
                return status;
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password)) 
            {
                status.StatusCode = 0;
                status.Message = "Invalid password";
                return status;
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);
            if (signInResult.Succeeded)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                    //new Claim(ClaimTypes.Name, user.Email)
                    //new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}") 
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                status.StatusCode = 1;
                status.Message = "Logged in successfully";
            }
            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User is locked out";
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Error in logging in";
            }

            return status;

        }
    }
}
