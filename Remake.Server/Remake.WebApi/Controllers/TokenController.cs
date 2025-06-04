using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Remake.WebApi.Entities.DataTransferObjects;
using Remake.WebApi.Entities.Models;
using Remake.WebApi.Services;

namespace Remake.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public TokenController(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult<AuthResponseDto>> Refresh(TokenRefreshDto tokenApiModel)
        {
            if (tokenApiModel is null || !ModelState.IsValid)
            {
                return BadRequest("Invalid client request");
            }

            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

            var username = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if(user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid client request");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponseDto { Token = newAccessToken, RefreshToken = newRefreshToken });
        }
    }
}
