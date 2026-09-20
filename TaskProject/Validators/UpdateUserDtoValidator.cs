using FluentValidation;
using TaskProject.DTOs;

namespace TaskProject.Validators
{
    public class UpdateUserDtoValidator: AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя пользователя обязательно")
                .Length(2, 100).WithMessage("Имя пользователя от 2 до 100 символов");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Невалидный Email");
        }
    }
}
