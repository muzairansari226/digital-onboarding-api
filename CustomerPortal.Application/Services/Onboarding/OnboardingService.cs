using CustomerPortal.Application.Enums;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;

namespace CustomerPortal.Application.Services.Onboarding
{
    public class OnboardingService : IOnboardingService
    {
        private readonly IOnboardingRequirementRepository _onboardingRequirementRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserConsentRepository _userConsentRepository;
        private readonly IUserPinRepository _userPinRepository;
        private readonly IUserDeviceRepository _userDeviceRepository;

        public OnboardingService(
            IOnboardingRequirementRepository onboardingRequirementRepository,
            IUserRepository userRepository,
            IUserConsentRepository userConsentRepository,
            IUserPinRepository userPinRepository,
            IUserDeviceRepository userDeviceRepository)
        {
            _onboardingRequirementRepository = onboardingRequirementRepository;
            _userRepository = userRepository;
            _userConsentRepository = userConsentRepository;
            _userPinRepository = userPinRepository;
            _userDeviceRepository = userDeviceRepository;
        }

        public async Task<ApiResponse<List<OnboardingModel.OnboardingRequirementResponse>>>
            GetRequirementsAsync()
        {
            var requirements = await _onboardingRequirementRepository.GetAllActiveAsync();

            return ApiResponse<List<OnboardingModel.OnboardingRequirementResponse>>.SuccessResponse(
                requirements.Select(MapToRequirementResponse).ToList(),
                ApplicationConstant.ResponseMessages.RequirementsRetrieved);
        }

        public async Task<ApiResponse<OnboardingModel.OnboardingStatusResponse>> GetStatusAsync(
            Guid registrationId)
        {
            var user = await _userRepository.GetByIdAsync(registrationId);
            if (user is null)
            {
                return ApiResponse<OnboardingModel.OnboardingStatusResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.UserNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            // Progress is re-derived from the rows that each step writes; nothing about the
            // journey is stored as state of its own, so there is nothing to fall out of sync.
            var isConsentAccepted = await _userConsentRepository.HasAcceptedAsync(
                user.RecId, (int)EnConsentDocumentType.PrivacyPolicy);
            var isPinSet = await _userPinRepository.GetByUserAsync(user.RecId) is not null;
            var devices = await _userDeviceRepository.GetByUserAsync(user.RecId);

            var result = new OnboardingModel.OnboardingStatusResponse
            {
                RegistrationId = user.RecId,
                IsMobileVerified = user.IsMobileVerified,
                IsEmailVerified = user.IsEmailVerified,
                IsConsentAccepted = isConsentAccepted,
                IsPinSet = isPinSet,
                IsBiometricEnrolled = devices.Count > 0,
                IsMigrated = user.IsMigrated,
                Status = ((EnUserStatus)user.Status).ToString(),
                NextStep = OnboardingStepHelper.Resolve(
                    user.IsMobileVerified,
                    user.IsEmailVerified,
                    isConsentAccepted,
                    isPinSet,
                    devices.Count > 0).ToString()
            };

            return ApiResponse<OnboardingModel.OnboardingStatusResponse>.SuccessResponse(
                result, ApplicationConstant.ResponseMessages.OnboardingStatusRetrieved);
        }

        private static OnboardingModel.OnboardingRequirementResponse MapToRequirementResponse(
            OnboardingRequirement requirement) => new()
            {
                RecId = requirement.RecId,
                Code = requirement.Code,
                Title = requirement.Title,
                Description = requirement.Description,
                DisplayOrder = requirement.DisplayOrder,
                IsMandatory = requirement.IsMandatory
            };
    }
}
