using System.Net;
using System.Text.Json;

namespace TaskProject.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Произошла ошибка при обработке запроса: {Method} {Path} от {UserAgent}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.Headers.UserAgent.ToString()
                    );

                context.Response.ContentType = "application/json";
                var response = new { Message = ex.Message };

                context.Response.StatusCode = ex switch
                {
                    InvalidOperationException => (int)HttpStatusCode.Conflict,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    ArgumentException => (int)HttpStatusCode.BadRequest,
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                if (context.Response.StatusCode == 500)
                    response = new { Message = "Внутренняя ошибка сервера" };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
