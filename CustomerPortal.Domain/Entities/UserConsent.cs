using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerPortal.Domain.Entities
{
    [Table("UserConsent")]
    public class UserConsent
    {
        [Key]
        public Guid RecId { get; set; }

        public Guid FkUser { get; set; }

        public Guid FkConsentDocument { get; set; }

        public int DocumentType { get; set; }

        [Required]
        [StringLength(20)]
        public string DocumentVersion { get; set; } = null!;

        public DateTime AcceptedAt { get; set; }

        [StringLength(45)]
        public string? AcceptedFromIp { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
