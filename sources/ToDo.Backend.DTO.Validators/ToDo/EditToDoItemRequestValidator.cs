using FluentValidation;
using ToDo.Backend.DTO.ToDo;

namespace ToDo.Backend.DTO.Validators.ToDo
{
    public sealed class EditToDoItemRequestValidator : AbstractValidator<EditToDoItemRequest>
    {
        public EditToDoItemRequestValidator()
        {
            RuleFor(o => o.Text)
                .NotEmpty();
        }
    }
}