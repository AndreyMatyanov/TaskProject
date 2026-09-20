using FluentValidation;
using TaskProject.DTOs;

namespace TaskProject.Validators
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Название задачи обязательно")
                .Length(2, 200).WithMessage("Название от 2 до 200 символов");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Описание не более 1000 символов");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Статус обязателен")
                .Must(s => s == "Todo" || s == "InProgress" || s == "Done")
                .WithMessage("Статус должен быть Todo, InProgress или Done");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Приоритет обязателен")
                .Must(p => p == "Low" || p == "Medium" || p == "High")
                .WithMessage("Приоритет должен быть Low, Medium или High");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId должен быть больше 0");

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("ProjectId должен быть больше 0");

            RuleFor(x => x.DueDate)
                .Must(date => !date.HasValue || date.Value > DateTime.UtcNow)
                .When(x => x.DueDate.HasValue)
                .WithMessage("Дата выполнения не должна быть в прошлом");
        }
    }
}
