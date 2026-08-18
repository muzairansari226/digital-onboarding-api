using Asp.Versioning;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Application.Services.Biometric;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPortal.Api.Controllers
{
    [ApiVersion(ApplicationConstant.ApiVersions.V1)]
    [Route(ApplicationConstant.ApiRoutes.Biometric)]
    public class BiometricController : ApiControllerBase
    {
        private readonly IBiometricService _biometricService;

        public BiometricController(IBiometricService biometricService)
        {
            _biometricService = biometricService;
        }

        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll(
            [FromBody] BiometricModel.BiometricEnrollRequest request)
        {
            return Result(await _biometricService.EnrollAsync(request));
        }
    }
}
