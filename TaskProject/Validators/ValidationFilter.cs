using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace TaskProject.Validators
{
    public class ValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Получаем все аргументы метода контроллера
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                var argumentType = argument.GetType();

                // Ищем валидатор для этого типа
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

                if (validator == null) continue;

                // Запускаем валидацию
                var validationContext = new ValidationContext<object>(argument);
                var validationResult = validator.Validate(validationContext);

                if (!validationResult.IsValid)
                {
                    // Если есть ошибки — возвращаем 400 и НЕ вызываем контроллер
                    context.Result = new BadRequestObjectResult(
                        validationResult.Errors.Select(e => new
                        {
                            Field = e.PropertyName,
                            Error = e.ErrorMessage
                        })
                    );
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}