using Asp.Versioning;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Services.Onboarding;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPortal.Api.Controllers
{
    [ApiVersion(ApplicationConstant.ApiVersions.V1)]
    [Route(ApplicationConstant.ApiRoutes.Onboarding)]
    public class OnboardingController : ApiControllerBase
    {
        private readonly IOnboardingService _onboardingService;

        public OnboardingController(IOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        [HttpGet("requirements")]
        public async Task<IActionResult> GetRequirements()
        {
            return Result(await _onboardingService.GetRequirementsAsync());
        }


        [HttpGet("registrations/{registrationId:guid}")]
        public async Task<IActionResult> GetStatus(Guid registrationId)
        {
            return Result(await _onboardingService.GetStatusAsync(registrationId));
        }
    }
}
