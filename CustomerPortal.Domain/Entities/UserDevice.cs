using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerPortal.Domain.Entities
{
    [Table("UserDevice")]
    public class UserDevice
    {
        [Key]
        public Guid RecId { get; set; }

        public Guid FkUser { get; set; }

        [Required]
        [StringLength(128)]
        public string DeviceId { get; set; } = null!;

        [Required]
        [StringLength(256)]
        public string BiometricSecretHash { get; set; } = null!;

        [Required]
        [StringLength(64)]
        public string BiometricSecretSalt { get; set; } = null!;

        [StringLength(100)]
        public string? DeviceName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
