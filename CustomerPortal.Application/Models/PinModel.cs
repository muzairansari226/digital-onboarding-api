using System.ComponentModel.DataAnnotations;
using CustomerPortal.Application.Helper;

namespace CustomerPortal.Application.Models
{
    public class PinModel
    {
        public class PinSetRequest
        {
            [NotEmptyGuid(ErrorMessage = ApplicationConstant.ValidationMessages.RegistrationIdRequired)]
            public Guid RegistrationId { get; set; }

            [Required(ErrorMessage = ApplicationConstant.ValidationMessages.PinRequired)]
            [RegularExpression(ApplicationConstant.RegularExpressions.NumericOnly,
                ErrorMessage = ApplicationConstant.ValidationMessages.PinInvalid)]
            public string Pin { get; set; } = null!;
        }

        public class PinSetResponse
        {
            public bool IsSet { get; set; }

            public string Status { get; set; } = string.Empty;

            public string NextStep { get; set; } = string.Empty;
        }

        public class PinVerifyRequest
        {
            [NotEmptyGuid(ErrorMessage = ApplicationConstant.ValidationMessages.RegistrationIdRequired)]
            public Guid RegistrationId { get; set; }

            [Required(ErrorMessage = ApplicationConstant.ValidationMessages.PinRequired)]
            [RegularExpression(ApplicationConstant.RegularExpressions.NumericOnly,
                ErrorMessage = ApplicationConstant.ValidationMessages.PinInvalid)]
            public string Pin { get; set; } = null!;
        }

        public class PinVerifyResponse
        {
            public bool IsVerified { get; set; }

            public int AttemptsRemaining { get; set; }

            public bool IsLocked { get; set; }

            public DateTime? LockedUntil { get; set; }
        }
    }
}
