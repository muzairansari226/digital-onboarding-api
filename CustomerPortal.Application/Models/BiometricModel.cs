using System.ComponentModel.DataAnnotations;
using CustomerPortal.Application.Helper;

namespace CustomerPortal.Application.Models
{
    public class BiometricModel
    {
        public class BiometricEnrollRequest
        {
            [NotEmptyGuid(ErrorMessage = ApplicationConstant.ValidationMessages.RegistrationIdRequired)]
            public Guid RegistrationId { get; set; }

            [Required(ErrorMessage = ApplicationConstant.ValidationMessages.DeviceIdRequired)]
            [StringLength(ApplicationConstant.FieldLengths.DeviceId)]
            public string DeviceId { get; set; } = null!;

            [StringLength(ApplicationConstant.FieldLengths.DeviceName)]
            public string? DeviceName { get; set; }
        }

        public class BiometricEnrollResponse
        {
            public Guid DeviceRecId { get; set; }

            public string DeviceSecret { get; set; } = string.Empty;

            public DateTime EnrolledAt { get; set; }

            public string NextStep { get; set; } = string.Empty;
        }

        public class BiometricDeviceResponse
        {
            public Guid DeviceRecId { get; set; }

            public string DeviceId { get; set; } = string.Empty;

            public string? DeviceName { get; set; }

            public DateTime EnrolledAt { get; set; }

            public DateTime UpdatedAt { get; set; }
        }
    }
}
