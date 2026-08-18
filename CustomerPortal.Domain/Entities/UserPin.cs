using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerPortal.Domain.Entities
{
    [Table("UserPin")]
    public class UserPin
    {
        [Key]
        public Guid RecId { get; set; }

        public Guid FkUser { get; set; }

        [Required]
        [StringLength(256)]
        public string PinHash { get; set; } = null!;

        [Required]
        [StringLength(64)]
        public string Salt { get; set; } = null!;

        public int FailedAttemptCount { get; set; }

        public DateTime? LockedUntil { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
