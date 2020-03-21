using System.Threading.Tasks;
using ToDo.Frontend.Services;
using ToDo.Frontend.Services.Abstractions;

namespace ToDo.Frontend.ViewModels
{
    public sealed class IndexViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        public IndexViewModel(IAuthService authService)
        {
            _authService = authService;
        }
        
        public async Task LogoutAsync()
        {
            await _authService.LogoutAsync();
            
            NavigateTo("/", true);
        }
    }
}