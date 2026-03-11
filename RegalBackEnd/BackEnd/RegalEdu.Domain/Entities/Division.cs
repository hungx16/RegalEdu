using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("Division")]
    public class Division : BaseEntity
    {
        [Required]
        [MaxLength (10)]
        public string DivisionCode { get; set; } = string.Empty;  // DIxxxx

        [Required]
        [MaxLength (200)]
        public string DivisionName { get; set; } = string.Empty;

        public int DivisionLevel { get; set; }

        [MaxLength (1000)]
        public string? Description { get; set; }

        public List<Department>? Departments { get; set; } = new List<Department> ( );
    }
}
