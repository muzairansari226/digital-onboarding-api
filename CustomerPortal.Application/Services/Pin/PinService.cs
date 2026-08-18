using CustomerPortal.Application.Contracts;
using CustomerPortal.Application.Enums;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Application.Settings;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;
using Microsoft.Extensions.Options;

namespace CustomerPortal.Application.Services.Pin
{
    public class PinService : IPinService
    {
        private readonly IUserPinRepository _userPinRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserConsentRepository _userConsentRepository;
        private readonly ISecretHasher _secretHasher;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly PinSettings _pinSettings;

        public PinService(
            IUserPinRepository userPinRepository,
            IUserRepository userRepository,
            IUserConsentRepository userConsentRepository,
            ISecretHasher secretHasher,
            IDateTimeProvider dateTimeProvider,
            IOptions<PinSettings> pinSettings)
        {
            _userPinRepository = userPinRepository;
            _userRepository = userRepository;
            _userConsentRepository = userConsentRepository;
            _secretHasher = secretHasher;
            _dateTimeProvider = dateTimeProvider;
            _pinSettings = pinSettings.Value;
        }

        public async Task<ApiResponse<PinModel.PinSetResponse>> SetAsync(PinModel.PinSetRequest request)
        {
            var user = await _userRepository.GetByIdAsync(request.RegistrationId);
            if (user is null)
            {
                return ApiResponse<PinModel.PinSetResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.UserNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            if (!user.IsMobileVerified)
            {
                return ApiResponse<PinModel.PinSetResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.MobileVerificationRequired,
                    ApplicationConstant.StatusCodes.BadRequest);
            }

            if (!user.IsEmailVerified)
            {
                return ApiResponse<PinModel.PinSetResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.EmailVerificationRequired,
                    ApplicationConstant.StatusCodes.BadRequest);
            }

            var hasConsent = await _userConsentRepository.HasAcceptedAsync(
                user.RecId, (int)EnConsentDocumentType.PrivacyPolicy);
            if (!hasConsent)
            {
                return ApiResponse<PinModel.PinSetResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.ConsentRequired,
                    ApplicationConstant.StatusCodes.BadRequest);
            }

            var existingPin = await _userPinRepository.GetByUserAsync(user.RecId);
            if (existingPin is not null)
            {
                return ApiResponse<PinModel.PinSetResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.PinAlreadySet,
                    ApplicationConstant.StatusCodes.Conflict);
            }

            if (request.Pin.Length != _pinSettings.Length)
            {
                return ApiResponse<PinModel.PinSetResponse>.ErrorResponse(
                    string.Format(
                        ApplicationConstant.ResponseMessages.PinLengthFormat, _pinSettings.Length),
                    ApplicationConstant.StatusCodes.BadRequest);
            }

            if (PinStrengthHelper.IsTrivial(request.Pin))
            {
                return ApiResponse<PinModel.PinSetResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.PinTooWeak,
                    ApplicationConstant.StatusCodes.BadRequest);
            }

            var salt = _secretHasher.GenerateSalt();
            await _userPinRepository.AddAsync(new UserPin
            {
                FkUser = user.RecId,
                PinHash = _secretHasher.Hash(request.Pin, salt),
                Salt = salt
            });

            user.Status = (int)EnUserStatus.Active;
            await _userRepository.UpdateAsync(user);

            return ApiResponse<PinModel.PinSetResponse>.SuccessResponse(
                new PinModel.PinSetResponse
                {
                    IsSet = true,
                    Status = ((EnUserStatus)user.Status).ToString(),
                    NextStep = OnboardingStepHelper.Resolve(
                        user.IsMobileVerified,
                        user.IsEmailVerified,
                        isConsentAccepted: true,
                        isPinSet: true,
                        isBiometricEnrolled: false).ToString()
                },
                ApplicationConstant.ResponseMessages.PinSet,
                ApplicationConstant.StatusCodes.Created);
        }

        public async Task<ApiResponse<PinModel.PinVerifyResponse>> VerifyAsync(
            PinModel.PinVerifyRequest request)
        {
            var user = await _userRepository.GetByIdAsync(request.RegistrationId);
            if (user is null)
            {
                return ApiResponse<PinModel.PinVerifyResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.UserNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            var userPin = await _userPinRepository.GetByUserAsync(user.RecId);
            if (userPin is null)
            {
                return ApiResponse<PinModel.PinVerifyResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.PinNotSet,
                    ApplicationConstant.StatusCodes.BadRequest);
            }

            var now = _dateTimeProvider.UtcNow;

            if (userPin.LockedUntil.HasValue && userPin.LockedUntil.Value > now)
            {
                var remainingMinutes = (int)Math.Ceiling(
                    (userPin.LockedUntil.Value - now).TotalMinutes);

                return ApiResponse<PinModel.PinVerifyResponse>.ErrorResponse(
                    string.Format(
                        ApplicationConstant.ResponseMessages.PinLockedFormat, remainingMinutes),
                    ApplicationConstant.StatusCodes.Locked,
                    MapToVerifyResponse(isVerified: false, attemptsRemaining: 0, userPin));
            }

            if (_secretHasher.Verify(request.Pin, userPin.Salt, userPin.PinHash))
            {
                // Resetting on success is what stops isolated mistakes accumulating into a lockout
                // over weeks of normal use.
                userPin.FailedAttemptCount = 0;
                userPin.LockedUntil = null;
                await _userPinRepository.UpdateAsync(userPin);

                if (user.Status == (int)EnUserStatus.Locked)
                {
                    user.Status = (int)EnUserStatus.Active;
                    await _userRepository.UpdateAsync(user);
                }

                return ApiResponse<PinModel.PinVerifyResponse>.SuccessResponse(
                    MapToVerifyResponse(
                        isVerified: true, _pinSettings.MaxFailedAttempts, userPin),
                    ApplicationConstant.ResponseMessages.PinVerified);
            }

            userPin.FailedAttemptCount++;

            var attemptsRemaining = Math.Max(
                0, _pinSettings.MaxFailedAttempts - userPin.FailedAttemptCount);

            if (attemptsRemaining == 0)
            {
                userPin.LockedUntil = now.AddMinutes(_pinSettings.LockoutMinutes);
                await _userPinRepository.UpdateAsync(userPin);

                user.Status = (int)EnUserStatus.Locked;
                await _userRepository.UpdateAsync(user);

                return ApiResponse<PinModel.PinVerifyResponse>.ErrorResponse(
                    string.Format(
                        ApplicationConstant.ResponseMessages.PinLockedFormat,
                        _pinSettings.LockoutMinutes),
                    ApplicationConstant.StatusCodes.Locked,
                    MapToVerifyResponse(isVerified: false, attemptsRemaining, userPin));
            }

            await _userPinRepository.UpdateAsync(userPin);

            return ApiResponse<PinModel.PinVerifyResponse>.ErrorResponse(
                string.Format(
                    ApplicationConstant.ResponseMessages.PinIncorrectFormat, attemptsRemaining),
                ApplicationConstant.StatusCodes.BadRequest,
                MapToVerifyResponse(isVerified: false, attemptsRemaining, userPin));
        }

        private static PinModel.PinVerifyResponse MapToVerifyResponse(
            bool isVerified, int attemptsRemaining, UserPin userPin) => new()
            {
                IsVerified = isVerified,
                AttemptsRemaining = attemptsRemaining,
                IsLocked = userPin.LockedUntil.HasValue,
                LockedUntil = userPin.LockedUntil
            };
    }
}
