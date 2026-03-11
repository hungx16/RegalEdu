
using RegalEdu.Domain.Entities;

namespace RegalEdu.Domain.Models;

public class StudentNoteModel : BaseEntityModel
{
    public Guid? StudentId { get; set; }
    public virtual StudentModel? Student { get; set; }
    public Guid? EmployeeId { get; set; } //Người thực hiện
    public virtual EmployeeModel? Employee { get; set; }
    public DateTime? NoteDate { get; set; } //Ngày taoj
    public string? NoteContext { get; set; }
}
