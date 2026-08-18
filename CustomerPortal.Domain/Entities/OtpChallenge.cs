using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerPortal.Domain.Entities
{
    [Table("OtpChallenge")]
    public class OtpChallenge
    {
        [Key]
        public Guid RecId { get; set; }

        public Guid FkUser { get; set; }

        public int Channel { get; set; }

        [Required]
        [StringLength(256)]
        public string CodeHash { get; set; } = null!;

        [Required]
        [StringLength(64)]
        public string Salt { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public int AttemptCount { get; set; }

        public DateTime? ConsumedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
