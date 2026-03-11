using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Domain.Entities;

// Bảng điểm chi tiết của từng nội dung kiểm tra của từng học viên theo lớp
[Table("ClassScoreBoard")]
public class ClassScoreBoard : BaseEntity
{ 
    public Guid ClassId { get; set; }
    [ForeignKey(nameof(ClassId))]
    public virtual Class? Class { get; set; }
    public Guid StudentId { get; set; }
    [ForeignKey(nameof(StudentId))]
    public virtual Student? Student { get; set; }
    public ScoreType ScoreType { get; set; }// MidTerm = 1 Giữa kỳ, FinalTerm = 2 Cuối kỳ        
    public Guid? CategoryId { get; set; }//Danh mục nội dung kiểm tra
    [ForeignKey(nameof(CategoryId))]
    public virtual Category? Category { get; set; }// Chỉ áp dụng khi CategoryType = 3
    public double? Score { get; set; }
}
