using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZiraLink.IDS.Framework;
using ZiraLink.IDS.Models;
using static Duende.IdentityServer.IdentityServerConstants;

namespace ZiraLink.IDS.Pages.Account.Register
{
    [ApiController]
    //[SecurityHeaders]
    [Authorize(LocalApi.PolicyName)]
    [Route("{controller}")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ApiResponse<string>> CreateUserAsync([FromBody] CreateUserInputModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(model.Username))
                throw new Exception($"{nameof(model.Username)} is required");

            if (string.IsNullOrEmpty(model.Password))
                throw new Exception($"{nameof(model.Password)} is required");

            if (string.IsNullOrEmpty(model.Email))
                throw new Exception($"{nameof(model.Email)} is required");

            if (string.IsNullOrEmpty(model.Name))
                throw new Exception($"{nameof(model.Name)} is required");

            if (string.IsNullOrEmpty(model.Family))
                throw new Exception($"{nameof(model.Family)} is required");

            var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) throw new Exception("User creation failed");

            // You might want to sign the user in automatically after registration
            // and redirect them to a dashboard or other appropriate page.
            // This might involve using SignInManager.
            result = await _userManager.AddClaimsAsync(user, new List<Claim>
                {
                    new Claim(JwtClaimTypes.GivenName, model.Name),
                    new Claim(JwtClaimTypes.FamilyName, model.Family),
                    new Claim(JwtClaimTypes.Name, $"{model.Name} {model.Family}"),
                    new Claim(JwtClaimTypes.Role, "customer")
                });


            if (!result.Succeeded) throw new Exception("Adding user claims failed");

            return ApiResponse<string>.CreateSuccessResponse(user.Id);
        }

        [HttpPatch("ChangePassword")]
        public async Task<ApiDefaultResponse> ChangePasswordAsync([FromBody] ChangePasswordInputModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(model.UserId))
                throw new Exception($"{nameof(model.UserId)} is required");

            if (string.IsNullOrEmpty(model.CurrentPassword))
                throw new Exception($"{nameof(model.CurrentPassword)} is required");

            if (string.IsNullOrEmpty(model.NewPassword))
                throw new Exception($"{nameof(model.NewPassword)} is required");

            var user = _userManager.Users.Where(x => x.Id == model.UserId).SingleOrDefault();
            if (user == null) throw new Exception("User not found");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded) throw new Exception("Changing password failed");

            return ApiDefaultResponse.CreateSuccessResponse();
        }

        [HttpPatch]
        public async Task<ApiDefaultResponse> UpdateProfileAsync([FromBody] UpdateProfileInputModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(model.UserId))
                throw new Exception($"{nameof(model.UserId)} is required");

            if (string.IsNullOrEmpty(model.Name))
                throw new Exception($"{nameof(model.Name)} is required");

            if (string.IsNullOrEmpty(model.Family))
                throw new Exception($"{nameof(model.Family)} is required");

            var user = _userManager.Users.Where(x => x.Id == model.UserId).SingleOrDefault();
            if (user == null) throw new Exception("User not found");

            var claims = await _userManager.GetClaimsAsync(user);

            await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.GivenName), new Claim(JwtClaimTypes.GivenName, model.Name));
            await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.FamilyName), new Claim(JwtClaimTypes.FamilyName, model.Family));
            await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.Name), new Claim(JwtClaimTypes.Name, $"{model.Name} {model.Family}"));
            
            return ApiDefaultResponse.CreateSuccessResponse();
        }
    }
}
