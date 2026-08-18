using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Asp.Versioning;
using CustomerPortal.Api.Filters;
using CustomerPortal.Api.Middlewares;
using CustomerPortal.Application;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Settings;
using CustomerPortal.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddPolicy(
    ApplicationConstant.CorsPolicies.Default,
    policy => policy
        .WithOrigins(builder.Configuration
            .GetSection(ApplicationConstant.ConfigurationSections.AllowedOrigins)
            .Get<string[]>() ?? Array.Empty<string>())
        .AllowAnyMethod()
        .AllowAnyHeader()));

builder.Services
    .AddControllers(options => options.Filters.Add<ValidateModelStateFilter>())
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplicationDI()
                .AddInfrastructureDI(builder.Configuration);

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = false;
    options.DefaultApiVersion = new ApiVersion(1, 0);
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.Configure<ApiBehaviorOptions>(
    options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddRateLimiter(BuildRateLimiter(builder.Configuration));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        ApplicationConstant.Swagger.DocumentName,
        new OpenApiInfo
        {
            Title = ApplicationConstant.Swagger.Title,
            Version = ApplicationConstant.Swagger.Version
        });

    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
    options.EnableAnnotations();

    IncludeXmlComments(options, Assembly.GetExecutingAssembly());
    IncludeXmlComments(options, typeof(ApplicationConstant).Assembly);
});

var app = builder.Build();

// First in the pipeline, so it wraps routing, model binding and controllers alike.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint(
        ApplicationConstant.Swagger.EndpointPath, ApplicationConstant.Swagger.Title));
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers[ApplicationConstant.ResponseHeaders.ContentTypeOptions] =
        ApplicationConstant.ResponseHeaders.ContentTypeOptionsValue;
    context.Response.Headers[ApplicationConstant.ResponseHeaders.CacheControl] =
        ApplicationConstant.ResponseHeaders.CacheControlValue;
    await next();
});

app.UseRouting();
app.UseCors(ApplicationConstant.CorsPolicies.Default);

// After UseRouting, so the per-endpoint policies on the OTP and PIN actions are resolved.
app.UseRateLimiter();

app.MapControllers();

app.Run();

static void IncludeXmlComments(
    Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options, Assembly assembly)
{
    var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml");
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
}

static Action<RateLimiterOptions> BuildRateLimiter(IConfiguration configuration)
{
    var settings = configuration
        .GetSection(ApplicationConstant.ConfigurationSections.RateLimiting)
        .Get<RateLimitSettings>()
        ?? throw new InvalidOperationException(
            $"The '{ApplicationConstant.ConfigurationSections.RateLimiting}' configuration section is missing.");

    return options =>
    {
        options.RejectionStatusCode = ApplicationConstant.StatusCodes.TooManyRequests;

        // Partitioned by caller IP. The per-account limits - the resend cooldown, the resend cap
        // and the PIN lockout - live in the services, because those need the account, which the
        // limiter cannot see without reading the body.
        AddFixedWindowPolicy(
            options, ApplicationConstant.RateLimitPolicies.OtpSend, settings.OtpSend);
        AddFixedWindowPolicy(
            options, ApplicationConstant.RateLimitPolicies.OtpVerify, settings.OtpVerify);
        AddFixedWindowPolicy(
            options, ApplicationConstant.RateLimitPolicies.PinVerify, settings.PinVerify);
        AddFixedWindowPolicy(
            options,
            ApplicationConstant.RateLimitPolicies.RegistrationStart,
            settings.RegistrationStart);

        options.OnRejected = async (context, cancellationToken) =>
        {
            context.HttpContext.Response.ContentType = "application/json";

            var payload = ApiResponse<string>.ErrorResponse(
                ApplicationConstant.ResponseMessages.TooManyRequests,
                ApplicationConstant.StatusCodes.TooManyRequests);

            await context.HttpContext.Response.WriteAsync(
                JsonSerializer.Serialize(
                    payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                cancellationToken);
        };
    };
}

static void AddFixedWindowPolicy(
    RateLimiterOptions options, string policyName, RateLimitPolicySettings policy)
    => options.AddPolicy(policyName, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? UnknownPartitionKey,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = policy.PermitLimit,
                Window = TimeSpan.FromSeconds(policy.WindowSeconds)
            }));

/// <summary>Exposed so the API can be hosted by WebApplicationFactory in integration tests.</summary>
public partial class Program
{
    internal const string UnknownPartitionKey = "unknown";
}
