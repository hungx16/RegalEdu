
namespace RegalEdu.Domain.Entities;

public class StudentNote : BaseEntity
{
    public Guid? StudentId { get; set; }
    public virtual Student? Student { get; set; }
    public Guid? EmployeeId { get; set; } //Người thực hiện
    public virtual Employee? Employee { get; set; }
    public DateTime? NoteDate{ get; set; } //Ngày taoj
    public string? NoteContext { get; set; }
    
}
