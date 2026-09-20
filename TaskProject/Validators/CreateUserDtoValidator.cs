using FluentValidation;
using TaskProject.DTOs;

namespace TaskProject.Validators
{
    public class CreateUserDtoValidator: AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя обязательно")
                .Length(2, 100).WithMessage("Имя пользователя от 2 до 100");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Email невалиден");

        }
    }
}
