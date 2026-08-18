using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerPortal.Domain.Entities
{
    [Table("OnboardingRequirement")]
    public class OnboardingRequirement
    {
        [Key]
        public Guid RecId { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
