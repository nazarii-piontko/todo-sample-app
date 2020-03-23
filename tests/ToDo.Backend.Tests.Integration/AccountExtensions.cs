using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ToDo.Backend.DTO.Account;

namespace ToDo.Backend.Tests.Integration
{
    public static class AccountExtensions
    {
        public static async Task<HttpResponseMessage> RegisterUserAsync(this HttpClient client, string email, string password)
        {
            var response = await client.PostAsync("api/v1.0/account/register",
                    new RegisterRequest
                    {
                        Email = email,
                        Password = password
                    }.AsJsonContent());

            return response;
        }
        
        public static async Task<HttpResponseMessage> LoginUserAsync(this HttpClient client, string email, string password)
        {
            var response = await client.PostAsync("api/v1.0/account/login",
                    new RegisterRequest
                    {
                        Email = email,
                        Password = password
                    }.AsJsonContent());

            return response;
        }
        
        public static async Task<string> GetUserTokenAsync(this HttpClient client, 
            string email = "user-valid@company.com",
            string password = "1234_Qwerty")
        {
            var response = await client.LoginUserAsync(email, password);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response = await client.RegisterUserAsync(email, password);
                response.EnsureSuccessStatusCode();
                
                response = await client.LoginUserAsync(email, password);
                response.EnsureSuccessStatusCode();
            }

            var loginResponse = await response.AsTypeAsync<LoginResponse>();

            return loginResponse.Token;
        }
    }
}