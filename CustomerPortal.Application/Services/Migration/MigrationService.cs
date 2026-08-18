using CustomerPortal.Application.Enums;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;

namespace CustomerPortal.Application.Services.Migration
{
    public class MigrationService : IMigrationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserPinRepository _userPinRepository;

        public MigrationService(
            IUserRepository userRepository,
            IUserPinRepository userPinRepository)
        {
            _userRepository = userRepository;
            _userPinRepository = userPinRepository;
        }

        public async Task<ApiResponse<MigrationModel.MigrationStartResponse>> StartAsync(
            MigrationModel.MigrationStartRequest request)
        {
            var user = await _userRepository.GetByMobileAsync(request.Mobile.Trim());
            if (user is null)
            {
                return ApiResponse<MigrationModel.MigrationStartResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.MigrationAccountNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            // A PIN is the last thing the journey sets, so its presence is what marks the account
            // as already migrated - a second run would otherwise re-walk a finished journey.
            var existingPin = await _userPinRepository.GetByUserAsync(user.RecId);
            if (existingPin is not null)
            {
                return ApiResponse<MigrationModel.MigrationStartResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.MigrationAlreadyCompleted,
                    ApplicationConstant.StatusCodes.Conflict);
            }

            if (!user.IsMigrated)
            {
                user.IsMigrated = true;
                await _userRepository.UpdateAsync(user);
            }

            return ApiResponse<MigrationModel.MigrationStartResponse>.SuccessResponse(
                MapToStartResponse(user),
                ApplicationConstant.ResponseMessages.MigrationStarted);
        }

        private static MigrationModel.MigrationStartResponse MapToStartResponse(User user) => new()
        {
            RegistrationId = user.RecId,
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
