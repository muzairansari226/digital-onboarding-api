using System.Text.Json;
using CustomerPortal.Application.Helper;

namespace CustomerPortal.Api.Middlewares
{
    /// <summary>
    /// Catches everything a service could not describe for itself. Registered first in the
    /// pipeline so it wraps routing, model binding and controllers alike, which is what lets
    /// services drop their own try/catch entirely.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        private static readonly JsonSerializerOptions JsonOptions =
            new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public ExceptionHandlingMiddleware(
            RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception exception)
            {
                var correlationId = context.TraceIdentifier;

                _logger.LogError(
                    exception,
                    ApplicationConstant.LogMessages.UnhandledException,
                    correlationId,
                    context.Request.Method,
                    context.Request.Path);

                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode = ApplicationConstant.StatusCodes.InternalServerError;
                context.Response.ContentType = JsonContentType;

                var payload = ApiResponse<string>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.UnexpectedError,
                    ApplicationConstant.StatusCodes.InternalServerError,
                    new List<string>
                    {
                        string.Format(
                            ApplicationConstant.ResponseMessages.ReferenceFormat, correlationId)
                    });

                await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
            }
        }

        private const string JsonContentType = "application/json";
    }
}
