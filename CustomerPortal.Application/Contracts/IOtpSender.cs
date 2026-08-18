using CustomerPortal.Application.Enums;

namespace CustomerPortal.Application.Contracts
{
    public interface IOtpSender
    {
        Task<bool> SendAsync(
            EnOtpChannel channel,
            string destination,
            string code,
            CancellationToken cancellationToken = default);
    }
}
