using System.ComponentModel.DataAnnotations;
using CustomerPortal.Application.Enums;
using CustomerPortal.Application.Helper;

namespace CustomerPortal.Application.Models
{
    public class OtpModel
    {
        public class OtpSendRequest
        {
            [NotEmptyGuid(ErrorMessage = ApplicationConstant.ValidationMessages.RegistrationIdRequired)]
            public Guid RegistrationId { get; set; }

            [Required(ErrorMessage = ApplicationConstant.ValidationMessages.ChannelRequired)]
            [EnumDataType(typeof(EnOtpChannel),
                ErrorMessage = ApplicationConstant.ValidationMessages.ChannelInvalid)]
            public EnOtpChannel Channel { get; set; }
        }

        public class OtpSendResponse
        {
            public string MaskedDestination { get; set; } = string.Empty;

            public int ExpiresInSeconds { get; set; }

            public int ResendAvailableInSeconds { get; set; }

            public string? Code { get; set; }
        }

        public class OtpVerifyRequest
        {
            [NotEmptyGuid(ErrorMessage = ApplicationConstant.ValidationMessages.RegistrationIdRequired)]
            public Guid RegistrationId { get; set; }

            [Required(ErrorMessage = ApplicationConstant.ValidationMessages.ChannelRequired)]
            [EnumDataType(typeof(EnOtpChannel),
                ErrorMessage = ApplicationConstant.ValidationMessages.ChannelInvalid)]
            public EnOtpChannel Channel { get; set; }

            [Required(ErrorMessage = ApplicationConstant.ValidationMessages.CodeRequired)]
            [RegularExpression(ApplicationConstant.RegularExpressions.NumericOnly,
                ErrorMessage = ApplicationConstant.ValidationMessages.CodeInvalid)]
            public string Code { get; set; } = null!;
        }

        public class OtpVerifyResponse
        {
            public bool IsVerified { get; set; }

            public int AttemptsRemaining { get; set; }

            public bool IsMobileVerified { get; set; }

            public bool IsEmailVerified { get; set; }

            public string NextStep { get; set; } = string.Empty;
        }
    }
}
