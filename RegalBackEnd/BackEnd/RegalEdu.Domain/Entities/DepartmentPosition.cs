using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("DepartmentPosition")]
    public class DepartmentPosition : BaseEntity
    {
        /// <summary>
        /// FK đến bảng Department
        /// </summary>
        public Guid DepartmentId { get; set; }

        [ForeignKey ("DepartmentId")]
        public virtual Department? Department { get; set; } = null!;

        /// <summary>
        /// FK đến bảng Position
        /// </summary>
        public Guid PositionId { get; set; }

        [ForeignKey ("PositionId")]
        public virtual Position? Position { get; set; } = null!;


    }
}
