using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using ToDo.Frontend.Services.Abstractions;

namespace ToDo.Frontend.Services
{
    public sealed class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthDataStorage _authService;

        public AuthStateProvider(IAuthDataStorage authService)
        {
            _authService = authService;
        }
        
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userId = await _authService.GetUserIdAsync();

            if (string.IsNullOrEmpty(userId))
                return new AuthenticationState(new ClaimsPrincipal());

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            }, "JWT Token");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var state = new AuthenticationState(claimsPrincipal);
            
            return state;
        }
    }
}