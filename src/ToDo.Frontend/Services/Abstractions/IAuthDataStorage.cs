using System.Threading.Tasks;
using ToDo.Backend.DTO.Account;

namespace ToDo.Frontend.Services.Abstractions
{
    public interface IAuthDataStorage
    {
        Task<bool> HasValidTokenAsync();
        
        Task<string> GetTokenAsync();

        Task<string> GetUserIdAsync();

        Task SetTokenAsync(LoginResponse token);
        
        Task RemoveTokenAsync();
    }
}