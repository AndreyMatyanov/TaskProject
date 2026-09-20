using FluentValidation;
using TaskProject.DTOs;

namespace TaskProject.Validators
{
    public class UpdateProjectDtoValidator: AbstractValidator<UpdateProjectDto>
    {
        public UpdateProjectDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название проекта обязательно")
                .Length(2, 100).WithMessage("Название от 2 до 100 символов");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Описание не более 500 символов");
        }
    }
}
