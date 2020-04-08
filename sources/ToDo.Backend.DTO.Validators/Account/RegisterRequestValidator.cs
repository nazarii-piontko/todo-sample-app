using FluentValidation;
using ToDo.Backend.DTO.Account;

namespace ToDo.Backend.DTO.Validators.Account
{
    public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(o => o.Email)
                .NotEmpty()
                .EmailAddress();
            
            RuleFor(o => o.Password)
                .NotEmpty();
        }
    }
}