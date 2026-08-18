using System.ComponentModel.DataAnnotations;
using CustomerPortal.Application.Helper;

namespace CustomerPortal.Application.Models
{
    public class RegistrationModel
    {
        public class AvailabilityCheckRequest : IValidatableObject
        {
            [EmailAddress(ErrorMessage = ApplicationConstant.ValidationMessages.EmailInvalid)]
            [StringLength(ApplicationConstant.FieldLengths.Email)]
            public string? Email { get; set; }

            [RegularExpression(ApplicationConstant.RegularExpressions.Mobile,
                ErrorMessage = ApplicationConstant.ValidationMessages.MobileInvalid)]
            [StringLength(ApplicationConstant.FieldLengths.Mobile)]
            public string? Mobile { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Mobile))
                {
                    yield return new ValidationResult(
                        ApplicationConstant.ValidationMessages.ContactDetailRequired,
                        new[] { nameof(Email), nameof(Mobile) });
                }
            }
        }

        public class AvailabilityCheckResponse
        {
            public bool IsAvailable { get; set; }

            public bool IsEmailTaken { get; set; }

            public bool IsMobileTaken { get; set; }
        }

        public class RegistrationStartRequest
        {
            [Required(ErrorMessage = ApplicationConstant.ValidationMessages.FirstNameRequired)]
            [StringLength(ApplicationConstant.FieldLengths.Name)]
            [RegularExpression(ApplicationConstant.RegularExpressions.PersonName,
                ErrorMessage = ApplicationConstant.ValidationMessages.FirstNameInvalid)]
            public string FirstName { get; set; } = null!;

            [Required(ErrorMessage = ApplicationConstant.ValidationMessages.LastNameRequired)]
            [StringLength(ApplicationConstant.FieldLengths.Name)]
            [RegularExpression(ApplicationConstant.RegularExpressions.PersonName,
                ErrorMessage = ApplicationConstant.ValidationMessages.LastNameInvalid)]
            public string LastName { get; set; } = null!;

            [Required(ErrorMessage = ApplicationConstant.ValidationMessages.EmailRequired)]
            [EmailAddress(ErrorMessage = ApplicationConstant.ValidationMessages.EmailInvalid)]
            [StringLength(ApplicationConstant.FieldLengths.Email)]
            public string Email { get; set; } = null!;

            [Required(ErrorMessage = ApplicationConstant.ValidationMessages.MobileRequired)]
            [RegularExpression(ApplicationConstant.RegularExpressions.Mobile,
                ErrorMessage = ApplicationConstant.ValidationMessages.MobileInvalid)]
            [StringLength(ApplicationConstant.FieldLengths.Mobile)]
            public string Mobile { get; set; } = null!;
        }

        public class RegistrationStartResponse
        {
            public Guid RegistrationId { get; set; }

            public string MaskedMobile { get; set; } = string.Empty;

            public string MaskedEmail { get; set; } = string.Empty;

            public string Status { get; set; } = string.Empty;

            public string NextStep { get; set; } = string.Empty;
        }
    }
}
