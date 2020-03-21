using FluentValidation;
using ToDo.Backend.DTO.ToDo;

namespace ToDo.Backend.DTO.Validators.ToDo
{
    public sealed class EditToDoListRequestValidator : AbstractValidator<EditToDoListRequest>
    {
        public EditToDoListRequestValidator()
        {
            RuleFor(o => o.Name)
                .NotEmpty();
        }
    }
}