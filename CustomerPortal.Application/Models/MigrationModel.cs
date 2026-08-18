using System.ComponentModel.DataAnnotations;
using CustomerPortal.Application.Helper;

namespace CustomerPortal.Application.Models
{
    public class MigrationModel
    {
        public class MigrationStartRequest
        {
            [Required(ErrorMessage = ApplicationConstant.ValidationMessages.MobileRequired)]
            [RegularExpression(ApplicationConstant.RegularExpressions.Mobile,
                ErrorMessage = ApplicationConstant.ValidationMessages.MobileInvalid)]
            [StringLength(ApplicationConstant.FieldLengths.Mobile)]
            public string Mobile { get; set; } = null!;
        }

        public class MigrationStartResponse
        {
            public Guid RegistrationId { get; set; }

            public string MaskedMobile { get; set; } = string.Empty;

            public string MaskedEmail { get; set; } = string.Empty;

            public string Status { get; set; } = string.Empty;

            public string NextStep { get; set; } = string.Empty;
        }
    }
}
