
namespace RegalEdu.Domain.Entities;

public class StudentCourse : BaseEntity
{
    public Guid? StudentId { get; set; }
    public virtual Student? Student { get; set; }
    public Guid? CourseId { get; set; }
    public virtual Course? Course { get; set; }
    public string? CourseName { get; set; }
    public string?  InterestLevel{ get; set; } //Mức dộ quan tâm thấp| cao| trung bình
    public string? Reason { get; set; } //lý do học
    

}
