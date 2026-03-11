
using RegalEdu.Domain.Entities;

namespace RegalEdu.Domain.Models;

public class StudentCourseModel : BaseEntityModel
{
    public Guid? StudentId { get; set; }
    public virtual StudentModel? Student { get; set; }
    public Guid? CourseId { get; set; }
    public virtual CourseModel? Course { get; set; }
    public string? CourseName { get; set; }
    public string? InterestLevel { get; set; } //Mức dộ quan tâm thấp| cao| trung bình
    public string? Reason { get; set; }
}
