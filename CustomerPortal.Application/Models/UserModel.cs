using System.ComponentModel.DataAnnotations;
using CustomerPortal.Application.Helper;

namespace CustomerPortal.Application.Models
{
    public class UserModel
    {
        public class UserProfileResponse
        {
            public Guid RecId { get; set; }

            public string FirstName { get; set; } = string.Empty;

            public string LastName { get; set; } = string.Empty;

            public string MaskedEmail { get; set; } = string.Empty;

            public string MaskedMobile { get; set; } = string.Empty;

            public bool IsEmailVerified { get; set; }

            public bool IsMobileVerified { get; set; }

            public bool IsMigrated { get; set; }

            public string Status { get; set; } = string.Empty;

            public DateTime CreatedAt { get; set; }
        }

        public class UserUpdateRequest
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
        }
    }
}
