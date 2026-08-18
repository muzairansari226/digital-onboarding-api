using Asp.Versioning;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Application.Services.Otp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CustomerPortal.Api.Controllers
{
    [ApiVersion(ApplicationConstant.ApiVersions.V1)]
    [Route(ApplicationConstant.ApiRoutes.Otp)]
    public class OtpController : ApiControllerBase
    {
        private readonly IOtpService _otpService;

        public OtpController(IOtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("send")]
        [EnableRateLimiting(ApplicationConstant.RateLimitPolicies.OtpSend)]
        public async Task<IActionResult> Send(
            [FromBody] OtpModel.OtpSendRequest request, CancellationToken cancellationToken)
        {
            return Result(await _otpService.SendAsync(request, cancellationToken));
        }

        [HttpPost("verify")]
        [EnableRateLimiting(ApplicationConstant.RateLimitPolicies.OtpVerify)]
        public async Task<IActionResult> Verify([FromBody] OtpModel.OtpVerifyRequest request)
        {
            return Result(await _otpService.VerifyAsync(request));
        }
    }
}
