using CustomerPortal.Application.Contracts;
using CustomerPortal.Application.Enums;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Application.Settings;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;
using Microsoft.Extensions.Options;

namespace CustomerPortal.Application.Services.Biometric
{
    public class BiometricService : IBiometricService
    {
        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserPinRepository _userPinRepository;
        private readonly ISecretHasher _secretHasher;
        private readonly ISecureCodeGenerator _secureCodeGenerator;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly OnboardingSettings _onboardingSettings;

        public BiometricService(
            IUserDeviceRepository userDeviceRepository,
            IUserRepository userRepository,
            IUserPinRepository userPinRepository,
            ISecretHasher secretHasher,
            ISecureCodeGenerator secureCodeGenerator,
            IDateTimeProvider dateTimeProvider,
            IOptions<OnboardingSettings> onboardingSettings)
        {
            _userDeviceRepository = userDeviceRepository;
            _userRepository = userRepository;
            _userPinRepository = userPinRepository;
            _secretHasher = secretHasher;
            _secureCodeGenerator = secureCodeGenerator;
            _dateTimeProvider = dateTimeProvider;
            _onboardingSettings = onboardingSettings.Value;
        }

        public async Task<ApiResponse<BiometricModel.BiometricEnrollResponse>> EnrollAsync(
            BiometricModel.BiometricEnrollRequest request)
        {
            var user = await _userRepository.GetByIdAsync(request.RegistrationId);
            if (user is null)
            {
                return ApiResponse<BiometricModel.BiometricEnrollResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.UserNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            // Biometric login is an alternative to the PIN, not a replacement for setting one -
            // without a PIN there is nothing to fall back to when the sensor fails.
            var userPin = await _userPinRepository.GetByUserAsync(user.RecId);
            if (userPin is null)
            {
                return ApiResponse<BiometricModel.BiometricEnrollResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.PinNotSet,
                    ApplicationConstant.StatusCodes.BadRequest);
            }

            var now = _dateTimeProvider.UtcNow;
            var deviceId = request.DeviceId.Trim();
            var secret = _secureCodeGenerator.GenerateSecret(
                _onboardingSettings.BiometricSecretByteLength);
            var salt = _secretHasher.GenerateSalt();

            var device = await _userDeviceRepository.GetByUserAndDeviceAsync(user.RecId, deviceId);
            if (device is null)
            {
                device = new UserDevice
                {
                    FkUser = user.RecId,
                    DeviceId = deviceId,
                    DeviceName = request.DeviceName?.Trim(),
                    BiometricSecretHash = _secretHasher.Hash(secret, salt),
                    BiometricSecretSalt = salt
                };

                device.RecId = await _userDeviceRepository.AddAsync(device);
            }
            else
            {
                device.DeviceName = request.DeviceName?.Trim() ?? device.DeviceName;
                device.BiometricSecretHash = _secretHasher.Hash(secret, salt);
                device.BiometricSecretSalt = salt;
                await _userDeviceRepository.UpdateAsync(device);
            }

            return ApiResponse<BiometricModel.BiometricEnrollResponse>.SuccessResponse(
                new BiometricModel.BiometricEnrollResponse
                {
                    DeviceRecId = device.RecId,
                    DeviceSecret = secret,
                    EnrolledAt = now,
                    NextStep = EnOnboardingStep.Completed.ToString()
                },
                ApplicationConstant.ResponseMessages.BiometricEnrolled,
                ApplicationConstant.StatusCodes.Created);
        }

        public async Task<ApiResponse<List<BiometricModel.BiometricDeviceResponse>>> GetDevicesAsync(
            Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                return ApiResponse<List<BiometricModel.BiometricDeviceResponse>>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.UserNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            var devices = await _userDeviceRepository.GetByUserAsync(userId);

            return ApiResponse<List<BiometricModel.BiometricDeviceResponse>>.SuccessResponse(
                devices.Select(MapToDeviceResponse).ToList(),
                ApplicationConstant.ResponseMessages.DevicesRetrieved);
        }

        private static BiometricModel.BiometricDeviceResponse MapToDeviceResponse(
            UserDevice device) => new()
            {
                DeviceRecId = device.RecId,
                DeviceId = device.DeviceId,
                DeviceName = device.DeviceName,
                EnrolledAt = device.CreatedAt,
                UpdatedAt = device.UpdatedAt
            };
    }
}
