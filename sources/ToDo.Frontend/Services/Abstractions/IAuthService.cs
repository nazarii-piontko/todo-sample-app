using System.Threading.Tasks;

namespace ToDo.Frontend.Services.Abstractions
{
    public interface IAuthService
    {
        Task RegisterAsync(string email, string password);
        
        Task LoginAsync(string email, string password);
        
        Task LogoutAsync();
    }
}