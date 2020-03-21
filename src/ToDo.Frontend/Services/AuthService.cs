using System.Threading.Tasks;
using ToDo.Backend.DTO;
using ToDo.Backend.DTO.Account;
using ToDo.Frontend.Services.Abstractions;

namespace ToDo.Frontend.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly RestClient _restClient;
        private readonly IAuthDataStorage _authDataStorage;

        public AuthService(RestClient restClient, 
            IAuthDataStorage authDataStorage)
        {
            _restClient = restClient;
            _authDataStorage = authDataStorage;
        }
        
        public Task RegisterAsync(string email, string password)
        {
            return _restClient.PostAsync<RegisterRequest, EmptyResponse>("api/v1.0/account/register",
                request: new RegisterRequest
                {
                    Email = email,
                    Password = password
                });
        }

        public async Task LoginAsync(string email, string password)
        {
            var loginResponse = await _restClient.PostAsync<LoginRequest, LoginResponse>("api/v1.0/account/login",
                request: new LoginRequest
                {
                    Email = email,
                    Password = password
                }).ConfigureAwait(false);
            
            await _authDataStorage.SetTokenAsync(loginResponse)
                .ConfigureAwait(false);
        }

        public async Task LogoutAsync()
        {
            var token = await _authDataStorage.GetTokenAsync()
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(token))
                return;
            
            await _restClient.PostAsync<LogoutRequest, EmptyResponse>("api/v1.0/account/logout",
                request: new LogoutRequest
                {
                    Token = token
                }).ConfigureAwait(false);
            
            await _authDataStorage.RemoveTokenAsync()
                .ConfigureAwait(false);
        }
    }
}