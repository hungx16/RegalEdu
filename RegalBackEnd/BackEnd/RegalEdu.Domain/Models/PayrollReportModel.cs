using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegalEdu.Domain.Models
{
    public class CompanyInfoModel
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public bool IsMainCompany { get; set; }
    }

    // Cập nhật PayrollTeacherDetailModel
    public class PayrollTeacherDetailModel
    {
        public Guid Id { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string TeacherCode { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;  // Company chính
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public List<CompanyInfoModel> AllCompanies { get; set; } = new();  // Tất cả companies mà teacher làm việc
        public decimal SalaryAmount { get; set; }
        public int StandardWorkDay { get; set; }
        public int ActualWorkDay { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
        public decimal NetSalary { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public string Status => IsPaid ? "Đã thanh toán" : "Chưa thanh toán";
    }

    // Cập nhật PayrollReportModel
    public class PayrollReportModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
        public decimal TotalSalaryAmount { get; set; }
        public decimal AverageSalary { get; set; }
        public int TotalTeachers { get; set; }
        public int PaidCount { get; set; }
        public int UnpaidCount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal UnpaidAmount { get; set; }
        public List<PayrollTeacherDetailModel> PayrollDetails { get; set; } = new();
        public Dictionary<string, decimal> SalaryByCompany { get; set; } = new();  // Đổi từ SalaryByDepartment
        public Guid? FilteredCompanyId { get; set; }
        public string? FilteredCompanyName { get; set; }
    }
}
