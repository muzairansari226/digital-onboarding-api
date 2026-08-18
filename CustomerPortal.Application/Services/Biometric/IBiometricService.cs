using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;

namespace CustomerPortal.Application.Services.Biometric
{
    public interface IBiometricService
    {
        Task<ApiResponse<BiometricModel.BiometricEnrollResponse>> EnrollAsync(
            BiometricModel.BiometricEnrollRequest request);

        Task<ApiResponse<List<BiometricModel.BiometricDeviceResponse>>> GetDevicesAsync(Guid userId);
    }
}
