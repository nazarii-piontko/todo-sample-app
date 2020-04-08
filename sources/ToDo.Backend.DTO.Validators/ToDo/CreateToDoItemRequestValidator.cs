using FluentValidation;
using ToDo.Backend.DTO.ToDo;

namespace ToDo.Backend.DTO.Validators.ToDo
{
    public sealed class CreateToDoItemRequestValidator : AbstractValidator<CreateToDoItemRequest>
    {
        public CreateToDoItemRequestValidator()
        {
            RuleFor(o => o.Text)
                .NotEmpty();
        }
    }
}