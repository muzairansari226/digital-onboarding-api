using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;

namespace CustomerPortal.Application.Services.Registration
{
    public interface IRegistrationService
    {
        Task<ApiResponse<RegistrationModel.AvailabilityCheckResponse>> CheckAvailabilityAsync(
            RegistrationModel.AvailabilityCheckRequest request);

        Task<ApiResponse<RegistrationModel.RegistrationStartResponse>> StartAsync(
            RegistrationModel.RegistrationStartRequest request);
    }
}
