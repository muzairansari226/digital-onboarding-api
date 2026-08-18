using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerPortal.Domain.Entities
{
    [Table("HomeContentCard")]
    public class HomeContentCard
    {
        [Key]
        public Guid RecId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(1000)]
        public string? Body { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [StringLength(500)]
        public string? ActionUrl { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
