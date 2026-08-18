using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;

namespace CustomerPortal.Application.Services.Onboarding
{
    public interface IOnboardingService
    {
        Task<ApiResponse<List<OnboardingModel.OnboardingRequirementResponse>>> GetRequirementsAsync();

        Task<ApiResponse<OnboardingModel.OnboardingStatusResponse>> GetStatusAsync(Guid registrationId);
    }
}
