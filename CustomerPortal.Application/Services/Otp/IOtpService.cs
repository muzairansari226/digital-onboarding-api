using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;

namespace CustomerPortal.Application.Services.Otp
{
    public interface IOtpService
    {
        Task<ApiResponse<OtpModel.OtpSendResponse>> SendAsync(
            OtpModel.OtpSendRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse<OtpModel.OtpVerifyResponse>> VerifyAsync(OtpModel.OtpVerifyRequest request);
    }
}
