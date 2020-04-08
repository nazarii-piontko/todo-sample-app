using FluentValidation;
using ToDo.Backend.DTO.ToDo;

namespace ToDo.Backend.DTO.Validators.ToDo
{
    public sealed class CreateToDoListRequestValidator : AbstractValidator<CreateToDoListRequest>
    {
        public CreateToDoListRequestValidator()
        {
            RuleFor(o => o.Name)
                .NotEmpty();
        }
    }
}