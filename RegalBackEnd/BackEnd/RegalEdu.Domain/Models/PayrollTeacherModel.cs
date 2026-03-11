using RegalEdu.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models;

public class PayrollTeacherModel : BaseEntityModel
{
    [Required]
    public Guid TeacherId { get; set; }

    public TeacherModel? Teacher { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime SalaryMonth { get; set; }

    [Required]
    [Range(0, 31)]
    public int StandardWorkDay { get; set; }

    [Required]
    [Range(0, 31)]
    public decimal ActualWorkDay { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal SalaryAmount { get; set; }

    public string? Note { get; set; }

    public bool IsPaid { get; set; } = false;

    [DataType(DataType.Date)]
    public DateTime? PaidDate { get; set; }

    // Các trường tính toán/thống kê có thể thêm nếu cần
    public decimal WorkDayRatio => StandardWorkDay > 0 ? ActualWorkDay / StandardWorkDay : 0;

    public decimal DailyRate => ActualWorkDay > 0 ? SalaryAmount / ActualWorkDay : 0;
}