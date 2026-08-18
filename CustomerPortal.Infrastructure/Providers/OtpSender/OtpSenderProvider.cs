using CustomerPortal.Application.Contracts;
using CustomerPortal.Application.Enums;
using CustomerPortal.Application.Helper;
using Microsoft.Extensions.Logging;

namespace CustomerPortal.Infrastructure.Providers.OtpSender
{
    public class OtpSenderProvider : IOtpSender
    {
        private readonly ILogger<OtpSenderProvider> _logger;

        public OtpSenderProvider(ILogger<OtpSenderProvider> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendAsync(
            EnOtpChannel channel,
            string destination,
            string code,
            CancellationToken cancellationToken = default)
        {
            var isDelivered = channel switch
            {
                EnOtpChannel.Mobile => await SendSmsAsync(destination, code, cancellationToken),
                EnOtpChannel.Email => await SendEmailAsync(destination, code, cancellationToken),
                _ => false
            };

            // The destination is masked and the code is never included - a log is one of the
            // easiest places for a live passcode to leak from.
            var maskedDestination = channel == EnOtpChannel.Mobile
                ? MaskingHelper.MaskMobile(destination)
                : MaskingHelper.MaskEmail(destination);

            _logger.LogInformation(
                ApplicationConstant.LogMessages.OtpDispatchPending, channel, maskedDestination);

            return isDelivered;
        }

        private Task<bool> SendSmsAsync(
            string destination, string code, CancellationToken cancellationToken)
            => Task.FromResult(true);

        private Task<bool> SendEmailAsync(
            string destination, string code, CancellationToken cancellationToken)
            => Task.FromResult(true);
    }
}
