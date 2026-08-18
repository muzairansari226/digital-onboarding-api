namespace CustomerPortal.Application.Models
{
    public class OnboardingModel
    {
        public class OnboardingRequirementResponse
        {
            public Guid RecId { get; set; }

            public string Code { get; set; } = string.Empty;

            public string Title { get; set; } = string.Empty;

            public string? Description { get; set; }

            public int DisplayOrder { get; set; }

            public bool IsMandatory { get; set; }
        }

        public class OnboardingStatusResponse
        {
            public Guid RegistrationId { get; set; }

            public bool IsMobileVerified { get; set; }

            public bool IsEmailVerified { get; set; }

            public bool IsConsentAccepted { get; set; }

            public bool IsPinSet { get; set; }

            public bool IsBiometricEnrolled { get; set; }

            public bool IsMigrated { get; set; }

            public string Status { get; set; } = string.Empty;

            public string NextStep { get; set; } = string.Empty;
        }
    }
}
