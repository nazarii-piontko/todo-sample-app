using FluentValidation;
using ToDo.Backend.DTO.Account;

namespace ToDo.Backend.DTO.Validators.Account
{
    public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(o => o.Email)
                .NotEmpty()
                .EmailAddress();
            
            RuleFor(o => o.Password)
                .NotEmpty();
        }
    }
}