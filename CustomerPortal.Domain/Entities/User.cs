using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerPortal.Domain.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        public Guid RecId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(256)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Mobile { get; set; } = null!;

        public bool IsEmailVerified { get; set; }

        public bool IsMobileVerified { get; set; }

        public int Status { get; set; }

        public bool IsMigrated { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
