using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductsMicroService.API.Exceptions;

namespace ProductsMicroService.API.Middleware
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

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // 🔥 Use DI logger, not Log.Error
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    CustomValidationException => StatusCodes.Status400BadRequest,
                    BadRequestException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                var problemDetails = new ProblemDetails
                {
                    Title = ex.Message,
                    Status = context.Response.StatusCode,
                    Detail = ex.InnerException?.Message,
                    Type = ex.GetType().Name
                };

                if (ex is CustomValidationException validationEx)
                    problemDetails.Extensions["errors"] = validationEx.Errors;

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
