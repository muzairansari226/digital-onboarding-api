using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;

namespace CustomerPortal.Application.Services.Pin
{
    public interface IPinService
    {
        Task<ApiResponse<PinModel.PinSetResponse>> SetAsync(PinModel.PinSetRequest request);

        Task<ApiResponse<PinModel.PinVerifyResponse>> VerifyAsync(PinModel.PinVerifyRequest request);
    }
}
