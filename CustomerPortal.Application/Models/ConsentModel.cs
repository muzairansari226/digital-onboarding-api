using System.ComponentModel.DataAnnotations;
using CustomerPortal.Application.Helper;

namespace CustomerPortal.Application.Models
{
    public class ConsentModel
    {
        public class ConsentDocumentResponse
        {
            public Guid DocumentId { get; set; }

            public string DocumentType { get; set; } = string.Empty;

            public string Version { get; set; } = string.Empty;

            public string Title { get; set; } = string.Empty;

            public string Content { get; set; } = string.Empty;

            public DateTime EffectiveFrom { get; set; }
        }

        public class ConsentAcceptRequest
        {
            [NotEmptyGuid(ErrorMessage = ApplicationConstant.ValidationMessages.RegistrationIdRequired)]
            public Guid RegistrationId { get; set; }

            [NotEmptyGuid(ErrorMessage = ApplicationConstant.ValidationMessages.DocumentIdRequired)]
            public Guid DocumentId { get; set; }

            [Range(typeof(bool), "true", "true",
                ErrorMessage = ApplicationConstant.ValidationMessages.AcceptanceRequired)]
            public bool IsAccepted { get; set; }
        }

        public class ConsentAcceptResponse
        {
            public Guid ConsentId { get; set; }

            public string DocumentVersion { get; set; } = string.Empty;

            public DateTime AcceptedAt { get; set; }

            public string NextStep { get; set; } = string.Empty;
        }
    }
}
