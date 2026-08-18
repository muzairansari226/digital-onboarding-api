using Asp.Versioning;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Application.Services.Registration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CustomerPortal.Api.Controllers
{
    [ApiVersion(ApplicationConstant.ApiVersions.V1)]
    [Route(ApplicationConstant.ApiRoutes.Registration)]
    public class RegistrationController : ApiControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost("availability")]
        [EnableRateLimiting(ApplicationConstant.RateLimitPolicies.RegistrationStart)]
        public async Task<IActionResult> CheckAvailability(
            [FromBody] RegistrationModel.AvailabilityCheckRequest request)
        {
            return Result(await _registrationService.CheckAvailabilityAsync(request));
        }

        [HttpPost("start")]
        [EnableRateLimiting(ApplicationConstant.RateLimitPolicies.RegistrationStart)]
        public async Task<IActionResult> Start(
                    [FromBody] RegistrationModel.RegistrationStartRequest request)
        {
            return Result(await _registrationService.StartAsync(request));
        }
    }
}
