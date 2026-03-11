using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    [Table ("DepartmentPosition")]
    public class DepartmentPositionModel : BaseEntityModel
    {
        /// <summary>
        /// FK đến bảng Department
        /// </summary>
        public Guid DepartmentId { get; set; }

        [ForeignKey ("DepartmentId")]
        public DepartmentDto? Department { get; set; } = null!;

        /// <summary>
        /// FK đến bảng Position
        /// </summary>
        public Guid PositionId { get; set; }

        [ForeignKey ("PositionId")]
        public PositionDto? Position { get; set; } = null!;
    }
}
