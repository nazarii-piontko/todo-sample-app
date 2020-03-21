using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ToDo.Frontend.Services;
using ToDo.Frontend.Services.Abstractions;

namespace ToDo.Frontend.ViewModels
{
    public sealed class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly IAuthDataStorage _authDataStorage;

        public RegisterViewModel(IAuthService authService,
            IAuthDataStorage authDataStorage)
        {
            _authService = authService;
            _authDataStorage = authDataStorage;
        }
        
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    
        [Required]
        public string Password { get; set; }
        
        public override async Task InitializeAsync()
        {
            var hasValidToken = await _authDataStorage.HasValidTokenAsync()
                .ConfigureAwait(true);
            
            if (hasValidToken)
                NavigateTo("/");
        }
        
        public async Task RegisterAsync()
        {
            try
            {
                await _authService.RegisterAsync(Email, Password)
                    .ConfigureAwait(true);
                
                NavigateTo("/login");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex)
                    .ConfigureAwait(true);
            }
        }
    }
}