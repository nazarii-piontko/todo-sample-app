using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ToDo.Backend.Domain;
using ToDo.Backend.DTO;
using ToDo.Backend.DTO.Account;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ToDo.Backend.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/account")]
    public sealed class AccountController : ApiController
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(IConfiguration configuration, 
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var newUser = new User
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
                return BadRequest(new ErrorResponse(result.Errors.Select(x => x.Description)));

            return NoContent();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Email);
            
            var result = user != null 
                ? await _signInManager.CheckPasswordSignInAsync(user, request.Password, false)
                    .ConfigureAwait(false)
                : SignInResult.Failed;

            if (!result.Succeeded)
            {
                return Unauthorized(result.IsLockedOut 
                    ? new ErrorResponse("User is locked out.") 
                    : new ErrorResponse("Email and/or password is invalid."));
            }

            var claims = new[]
            {
                // ReSharper disable once PossibleNullReferenceException
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth:JwtSecurityKey"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Auth:JwtExpiryInDays"]));

            var token = new JwtSecurityToken(
                _configuration["Auth:JwtIssuer"],
                _configuration["Auth:JwtAudience"],
                claims,
                expires: expires,
                signingCredentials: signingCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResponse
            {
                Token = tokenString,
                Expires = expires
            });
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            // Do nothing, token will be valid until expiry
            return Task.FromResult<IActionResult>(NoContent());
        }
    }
}