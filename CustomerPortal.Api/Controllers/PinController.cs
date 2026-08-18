using Asp.Versioning;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Application.Services.Pin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CustomerPortal.Api.Controllers
{
    [ApiVersion(ApplicationConstant.ApiVersions.V1)]
    [Route(ApplicationConstant.ApiRoutes.Pin)]
    public class PinController : ApiControllerBase
    {
        private readonly IPinService _pinService;

        public PinController(IPinService pinService)
        {
            _pinService = pinService;
        }

        [HttpPost("set")]
        public async Task<IActionResult> Set([FromBody] PinModel.PinSetRequest request)
        {
            return Result(await _pinService.SetAsync(request));
        }

        [HttpPost("verify")]
        [EnableRateLimiting(ApplicationConstant.RateLimitPolicies.PinVerify)]
        public async Task<IActionResult> Verify([FromBody] PinModel.PinVerifyRequest request)
        {
            return Result(await _pinService.VerifyAsync(request));
        }
    }
}
