using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using ToDo.Backend.DTO.Account;
using ToDo.Frontend.Services.Abstractions;

namespace ToDo.Frontend.Services
{
    public sealed class AuthDataStorage : IAuthDataStorage
    {
        private const string AuthDataKey = "Auth.Data";
        
        private readonly ILocalStorageService _localStorage;
        
        public AuthDataStorage(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        
        public async Task<bool> HasValidTokenAsync()
        {
            var token = await GetTokenAsync()
                .ConfigureAwait(false);
            return !string.IsNullOrEmpty(token);
        }
        
        public async Task<string> GetTokenAsync()
        {
            var tokenExists = await _localStorage.ContainKeyAsync(AuthDataKey)
                .ConfigureAwait(false);

            if (!tokenExists)
                return string.Empty;
            
            var data = await _localStorage.GetItemAsync<LoginResponse>(AuthDataKey)
                .ConfigureAwait(false);

            if (data.Expires < DateTime.UtcNow)
                return string.Empty;

            return data.Token;
        }

        public async Task<string> GetUserIdAsync()
        {
            var data = await _localStorage.GetItemAsync<LoginResponse>(AuthDataKey)
                .ConfigureAwait(false);
            
            if (data == null || data.Expires < DateTime.UtcNow)
                return string.Empty;
            
            var claims = ParseClaimsFromJwt(data.Token);

            var userId = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            
            return userId;
        }

        public async Task SetTokenAsync(LoginResponse data)
        {
            await _localStorage.SetItemAsync(AuthDataKey, data)
                .ConfigureAwait(false);
        }

        public async Task RemoveTokenAsync()
        {
            await _localStorage.RemoveItemAsync(AuthDataKey)
                .ConfigureAwait(false); 
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}