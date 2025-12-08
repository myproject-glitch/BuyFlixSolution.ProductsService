using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductsMicroService.API.Exceptions;
using System.Linq;
using System.Collections.Generic;
using FluentValidation.Results;

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

                // Validate argument
                var validationResult = await validator.ValidateAsync((dynamic)argument);

                if (!validationResult.IsValid)
                {
                    // Cast Errors to IEnumerable<ValidationFailure> to allow LINQ
                    IEnumerable<ValidationFailure> errorsList = (IEnumerable<ValidationFailure>)validationResult.Errors;
                    var errors = errorsList.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();

                    throw new CustomValidationException(errors);
                }
            }

            await next();
        }
    }
}
