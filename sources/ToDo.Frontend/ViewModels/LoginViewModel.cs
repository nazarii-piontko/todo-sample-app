using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ToDo.Frontend.Services.Abstractions;

namespace ToDo.Frontend.ViewModels
{
    public sealed class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly IAuthDataStorage _authDataStorage;

        public LoginViewModel(IAuthService authService,
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
                NavigateTo("/app");
        }

        public async Task LoginAsync()
        {
            try
            {
                await _authService.LoginAsync(Email, Password)
                    .ConfigureAwait(true);
                
                NavigateTo("/app", true);
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex)
                    .ConfigureAwait(true);
            }
        }
    }
}