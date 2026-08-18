using CustomerPortal.Application.Enums;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;

namespace CustomerPortal.Application.Services.Registration
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUserRepository _userRepository;

        public RegistrationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<RegistrationModel.AvailabilityCheckResponse>> CheckAvailabilityAsync(
            RegistrationModel.AvailabilityCheckRequest request)
        {
            var isEmailTaken = !string.IsNullOrWhiteSpace(request.Email)
                && await _userRepository.IsEmailTakenAsync(NormaliseEmail(request.Email));

            var isMobileTaken = !string.IsNullOrWhiteSpace(request.Mobile)
                && await _userRepository.IsMobileTakenAsync(NormaliseMobile(request.Mobile));

            var result = new RegistrationModel.AvailabilityCheckResponse
            {
                IsEmailTaken = isEmailTaken,
                IsMobileTaken = isMobileTaken,
                IsAvailable = !isEmailTaken && !isMobileTaken
            };

            return ApiResponse<RegistrationModel.AvailabilityCheckResponse>.SuccessResponse(
                result,
                result.IsAvailable
                    ? ApplicationConstant.ResponseMessages.DetailsAvailable
                    : ApplicationConstant.ResponseMessages.AccountAlreadyExists);
        }

        public async Task<ApiResponse<RegistrationModel.RegistrationStartResponse>> StartAsync(
            RegistrationModel.RegistrationStartRequest request)
        {
            var email = NormaliseEmail(request.Email);
            var mobile = NormaliseMobile(request.Mobile);

            if (await _userRepository.IsEmailTakenAsync(email))
            {
                return ApiResponse<RegistrationModel.RegistrationStartResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.EmailAlreadyRegistered,
                    ApplicationConstant.StatusCodes.Conflict);
            }

            if (await _userRepository.IsMobileTakenAsync(mobile))
            {
                return ApiResponse<RegistrationModel.RegistrationStartResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.MobileAlreadyRegistered,
                    ApplicationConstant.StatusCodes.Conflict);
            }

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                Mobile = mobile,
                Status = (int)EnUserStatus.PendingVerification,
                IsMigrated = false
            };

            var registrationId = await _userRepository.AddAsync(user);

            return ApiResponse<RegistrationModel.RegistrationStartResponse>.SuccessResponse(
                MapToStartResponse(user, registrationId),
                ApplicationConstant.ResponseMessages.RegistrationStarted,
                ApplicationConstant.StatusCodes.Created);
        }

        private static string NormaliseEmail(string email) => email.Trim().ToLowerInvariant();

        private static string NormaliseMobile(string mobile) => mobile.Trim();

        private RegistrationModel.RegistrationStartResponse MapToStartResponse(
            User user, Guid registrationId) => new()
            {
                RegistrationId = registrationId,
                MaskedMobile = MaskingHelper.MaskMobile(user.Mobile),
                MaskedEmail = MaskingHelper.MaskEmail(user.Email),
                Status = ((EnUserStatus)user.Status).ToString(),
                NextStep = OnboardingStepHelper.Resolve(
                    user.IsMobileVerified,
                    user.IsEmailVerified,
                    isConsentAccepted: false,
                    isPinSet: false,
                    isBiometricEnrolled: false).ToString()
            };
    }
}
