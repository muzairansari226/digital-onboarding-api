using Asp.Versioning;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Application.Services.Migration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CustomerPortal.Api.Controllers
{
    [ApiVersion(ApplicationConstant.ApiVersions.V1)]
    [Route(ApplicationConstant.ApiRoutes.Migration)]
    public class MigrationController : ApiControllerBase
    {
        private readonly IMigrationService _migrationService;

        public MigrationController(IMigrationService migrationService)
        {
            _migrationService = migrationService;
        }

        [HttpPost("start")]
        [EnableRateLimiting(ApplicationConstant.RateLimitPolicies.RegistrationStart)]
        public async Task<IActionResult> Start([FromBody] MigrationModel.MigrationStartRequest request)
        {
            return Result(await _migrationService.StartAsync(request));
        }
    }
}
