using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerPortal.Domain.Entities
{
    [Table("ConsentDocument")]
    public class ConsentDocument
    {
        [Key]
        public Guid RecId { get; set; }

        public int DocumentType { get; set; }

        [Required]
        [StringLength(20)]
        public string Version { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        public DateTime EffectiveFrom { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
