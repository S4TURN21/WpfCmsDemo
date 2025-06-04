using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Remake.WebApi.Entities.DataTransferObjects;
using Remake.WebApi.Entities.Models;
using Remake.WebApi.Services;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Remake.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AccountsController(UserManager<User> userManager, IMapper mapper, ITokenService tokenService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = _mapper.Map<User>(userForRegistration);

            var result = await _userManager.CreateAsync(user, userForRegistration.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return BadRequest(new RegistrationResponseDto { Errors = errors });
            }

            return StatusCode(201);
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServerMessage), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login(UserForAuthenticationDto loginModel)
        {
            if (loginModel is null || !ModelState.IsValid)
            {
                return BadRequest("Invalid client request");
            }

            var user = await _userManager.FindByNameAsync(loginModel.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                return Unauthorized(new ServerMessage("Неверное имя пользователя и/или пароль"));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            claims.AddRange((await _userManager.GetRolesAsync(user)).Select(role => new Claim(ClaimTypes.Role, role)));

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponseDto { Token = accessToken, RefreshToken = refreshToken });
        }

        [Authorize]
        [HttpGet("UserInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<User?>> GetUserInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
    }
}
