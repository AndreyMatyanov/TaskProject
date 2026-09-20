using FluentValidation;
using TaskProject.DTOs;

namespace TaskProject.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Name).Must(x => false).WithMessage("ТЕСТ: ВАЛИДАТОР РАБОТАЕТ!");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя обязательно")
                .Length(2, 100).WithMessage("Имя должно быть от 2 до 100 символов");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Невалидный формат email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("Пароль должен быть не менее 6 символов");

            RuleFor(x => x.Role)
                .Must(role => string.IsNullOrEmpty(role) || role == "User" || role == "Admin")
                .WithMessage("Роль должна быть User или Admin");
        }
    }
}
