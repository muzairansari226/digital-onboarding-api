using CustomerPortal.Application.Contracts;
using CustomerPortal.Application.Enums;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Application.Settings;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;
using Microsoft.Extensions.Options;

namespace CustomerPortal.Application.Services.Otp
{
    public class OtpService : IOtpService
    {
        private readonly IOtpChallengeRepository _otpChallengeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOtpSender _otpSender;
        private readonly ISecretHasher _secretHasher;
        private readonly ISecureCodeGenerator _secureCodeGenerator;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly OtpSettings _otpSettings;
        private readonly OnboardingSettings _onboardingSettings;

        public OtpService(
            IOtpChallengeRepository otpChallengeRepository,
            IUserRepository userRepository,
            IOtpSender otpSender,
            ISecretHasher secretHasher,
            ISecureCodeGenerator secureCodeGenerator,
            IDateTimeProvider dateTimeProvider,
            IOptions<OtpSettings> otpSettings,
            IOptions<OnboardingSettings> onboardingSettings)
        {
            _otpChallengeRepository = otpChallengeRepository;
            _userRepository = userRepository;
            _otpSender = otpSender;
            _secretHasher = secretHasher;
            _secureCodeGenerator = secureCodeGenerator;
            _dateTimeProvider = dateTimeProvider;
            _otpSettings = otpSettings.Value;
            _onboardingSettings = onboardingSettings.Value;
        }

        public async Task<ApiResponse<OtpModel.OtpSendResponse>> SendAsync(
            OtpModel.OtpSendRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(request.RegistrationId);
            if (user is null)
            {
                return ApiResponse<OtpModel.OtpSendResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.UserNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            if (IsChannelVerified(user, request.Channel))
            {
                return ApiResponse<OtpModel.OtpSendResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.OtpChannelAlreadyVerified,
                    ApplicationConstant.StatusCodes.Conflict);
            }

            // The screens verify the mobile number before the email address; enforcing that here
            // stops the sequence being skipped by calling the API directly.
            if (request.Channel == EnOtpChannel.Email
                && _onboardingSettings.EnforceChannelOrder
                && !user.IsMobileVerified)
            {
                return ApiResponse<OtpModel.OtpSendResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.MobileVerificationRequired,
                    ApplicationConstant.StatusCodes.BadRequest);
            }

            var now = _dateTimeProvider.UtcNow;
            var channel = (int)request.Channel;

            var latestChallenge = await _otpChallengeRepository.GetLatestAsync(user.RecId, channel);
            if (latestChallenge is not null)
            {
                var elapsedSeconds = (int)(now - latestChallenge.CreatedAt).TotalSeconds;
                var remainingSeconds = _otpSettings.ResendCooldownSeconds - elapsedSeconds;
                if (remainingSeconds > 0)
                {
                    return ApiResponse<OtpModel.OtpSendResponse>.ErrorResponse(
                        string.Format(
                            ApplicationConstant.ResponseMessages.OtpResendCooldownFormat,
                            remainingSeconds),
                        ApplicationConstant.StatusCodes.TooManyRequests);
                }
            }

            var windowStart = now.AddMinutes(-_otpSettings.ResendWindowMinutes);
            var issuedInWindow = await _otpChallengeRepository.CountIssuedSinceAsync(
                user.RecId, channel, windowStart);
            if (issuedInWindow >= _otpSettings.MaxResendsPerWindow)
            {
                return ApiResponse<OtpModel.OtpSendResponse>.ErrorResponse(
                    string.Format(
                        ApplicationConstant.ResponseMessages.OtpResendLimitFormat,
                        _otpSettings.ResendWindowMinutes),
                    ApplicationConstant.StatusCodes.TooManyRequests);
            }

            // Retiring outstanding challenges first means a resend never leaves two live codes.
            await _otpChallengeRepository.ConsumeOutstandingAsync(user.RecId, channel, now);

            var code = _secureCodeGenerator.GenerateNumericCode(_otpSettings.CodeLength);
            var salt = _secretHasher.GenerateSalt();

            await _otpChallengeRepository.AddAsync(new OtpChallenge
            {
                FkUser = user.RecId,
                Channel = channel,
                CodeHash = _secretHasher.Hash(code, salt),
                Salt = salt,
                ExpiresAt = now.AddMinutes(_otpSettings.ExpiryMinutes)
            });

            var destination = GetDestination(user, request.Channel);
            var isSent = await _otpSender.SendAsync(request.Channel, destination, code, cancellationToken);
            if (!isSent)
            {
                return ApiResponse<OtpModel.OtpSendResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.OtpSendFailed,
                    ApplicationConstant.StatusCodes.BadGateway);
            }

            return ApiResponse<OtpModel.OtpSendResponse>.SuccessResponse(
                new OtpModel.OtpSendResponse
                {
                    MaskedDestination = MaskDestination(user, request.Channel),
                    ExpiresInSeconds = _otpSettings.ExpiryMinutes * SecondsPerMinute,
                    ResendAvailableInSeconds = _otpSettings.ResendCooldownSeconds,
                    Code = _otpSettings.ReturnCodeInResponse ? code : null
                },
                ApplicationConstant.ResponseMessages.OtpSent);
        }

        public async Task<ApiResponse<OtpModel.OtpVerifyResponse>> VerifyAsync(
            OtpModel.OtpVerifyRequest request)
        {
            var user = await _userRepository.GetByIdAsync(request.RegistrationId);
            if (user is null)
            {
                return ApiResponse<OtpModel.OtpVerifyResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.UserNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            if (IsChannelVerified(user, request.Channel))
            {
                return ApiResponse<OtpModel.OtpVerifyResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.OtpChannelAlreadyVerified,
                    ApplicationConstant.StatusCodes.Conflict);
            }

            var now = _dateTimeProvider.UtcNow;
            var channel = (int)request.Channel;

            var challenge = await _otpChallengeRepository.GetLatestUnconsumedAsync(user.RecId, channel);
            if (challenge is null)
            {
                return ApiResponse<OtpModel.OtpVerifyResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.OtpNotFound,
                    ApplicationConstant.StatusCodes.BadRequest);
            }

            if (challenge.ExpiresAt <= now)
            {
                await ConsumeAsync(challenge, now);
                return ApiResponse<OtpModel.OtpVerifyResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.OtpExpired,
                    ApplicationConstant.StatusCodes.BadRequest);
            }

            challenge.AttemptCount++;

            if (_secretHasher.Verify(request.Code, challenge.Salt, challenge.CodeHash))
            {
                await ConsumeAsync(challenge, now);
                await MarkChannelVerifiedAsync(user, request.Channel);

                return ApiResponse<OtpModel.OtpVerifyResponse>.SuccessResponse(
                    MapToVerifyResponse(user, isVerified: true, attemptsRemaining: 0),
                    ApplicationConstant.ResponseMessages.OtpVerified);
            }

            var attemptsRemaining = Math.Max(
                0, _otpSettings.MaxVerificationAttempts - challenge.AttemptCount);

            if (attemptsRemaining == 0)
            {
                // The challenge is burned rather than left to be guessed at within its window.
                await ConsumeAsync(challenge, now);
                return ApiResponse<OtpModel.OtpVerifyResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.OtpAttemptsExhausted,
                    ApplicationConstant.StatusCodes.TooManyRequests,
                    MapToVerifyResponse(user, isVerified: false, attemptsRemaining));
            }

            await _otpChallengeRepository.UpdateAsync(challenge);

            return ApiResponse<OtpModel.OtpVerifyResponse>.ErrorResponse(
                string.Format(
                    ApplicationConstant.ResponseMessages.OtpIncorrectFormat, attemptsRemaining),
                ApplicationConstant.StatusCodes.BadRequest,
                MapToVerifyResponse(user, isVerified: false, attemptsRemaining));
        }

        private const int SecondsPerMinute = 60;

        private async Task ConsumeAsync(OtpChallenge challenge, DateTime consumedAt)
        {
            challenge.ConsumedAt = consumedAt;
            await _otpChallengeRepository.UpdateAsync(challenge);
        }

        private async Task MarkChannelVerifiedAsync(User user, EnOtpChannel channel)
        {
            if (channel == EnOtpChannel.Mobile)
            {
                user.IsMobileVerified = true;
            }
            else
            {
                user.IsEmailVerified = true;
            }

            await _userRepository.UpdateAsync(user);
        }

        private static bool IsChannelVerified(User user, EnOtpChannel channel)
            => channel == EnOtpChannel.Mobile ? user.IsMobileVerified : user.IsEmailVerified;

        private static string GetDestination(User user, EnOtpChannel channel)
            => channel == EnOtpChannel.Mobile ? user.Mobile : user.Email;

        private static string MaskDestination(User user, EnOtpChannel channel)
            => channel == EnOtpChannel.Mobile
                ? MaskingHelper.MaskMobile(user.Mobile)
                : MaskingHelper.MaskEmail(user.Email);

        private static OtpModel.OtpVerifyResponse MapToVerifyResponse(
            User user, bool isVerified, int attemptsRemaining) => new()
            {
                IsVerified = isVerified,
                AttemptsRemaining = attemptsRemaining,
                IsMobileVerified = user.IsMobileVerified,
                IsEmailVerified = user.IsEmailVerified,
                NextStep = OnboardingStepHelper.Resolve(
                    user.IsMobileVerified,
                    user.IsEmailVerified,
                    isConsentAccepted: false,
                    isPinSet: false,
                    isBiometricEnrolled: false).ToString()
            };
    }
}
