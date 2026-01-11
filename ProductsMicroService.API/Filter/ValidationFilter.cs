using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Collections.Generic;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace ProductsMicroService.API.Filter
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                dynamic? validator = _serviceProvider.GetService(validatorType);
                if (validator == null) continue;

               ValidationResult validationResult = await validator.ValidateAsync((dynamic)argument);

                if (!validationResult.IsValid)
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        Success = false,
                        Errors = validationResult.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).ToList()
                            )
                    });
                    return;
                }
            }

            await next();
        }
    }
}
