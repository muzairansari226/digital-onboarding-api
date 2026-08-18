using CustomerPortal.Application.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CustomerPortal.Api.Filters
{

    // Emits DataAnnotations failures in the same envelope as everything else. The framework's
    // automatic 400 is suppressed in <c>Program</c> precisely so this can replace it - otherwise
    // validation would be the one response shape a client had to special-case.

    public class ValidateModelStateFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            var response = ApiResponse<string>.ErrorResponse(
                ApplicationConstant.ResponseMessages.ValidationFailed,
                ApplicationConstant.StatusCodes.BadRequest,
                BuildErrors(context));

            context.Result = new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        private static List<string> BuildErrors(ActionExecutingContext context)
        {
            var hasUnreadableBody = context.ModelState.Any(entry =>
                entry.Key.StartsWith(JsonPathPrefix, StringComparison.Ordinal)
                || entry.Value?.Errors.Any(error => error.Exception is not null) == true);

            if (hasUnreadableBody)
            {
                return new List<string> { ApplicationConstant.ValidationMessages.RequestBodyInvalid };
            }

            return context.ModelState
                .SelectMany(entry => entry.Value?.Errors ?? new())
                .Select(error => error.ErrorMessage)
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .Distinct()
                .ToList();
        }

        private const string JsonPathPrefix = "$.";
    }
}
