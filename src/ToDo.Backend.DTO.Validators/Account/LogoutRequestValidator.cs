using FluentValidation;
using ToDo.Backend.DTO.Account;

namespace ToDo.Backend.DTO.Validators.Account
{
    public sealed class LogoutRequestValidator : AbstractValidator<LogoutRequest>
    {
        public LogoutRequestValidator()
        {
            RuleFor(o => o.Token)
                .NotEmpty();
        }
    }
}