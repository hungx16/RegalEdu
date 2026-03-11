using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;

namespace RegalEdu.Application.PayrollTeacher.Queries
{
    public class PayrollSummaryModel
    {
        public int TotalRecords { get; set; }
        public decimal TotalSalaryAmount { get; set; }
        public decimal AverageSalary { get; set; }
        public int PaidCount { get; set; }
        public int UnpaidCount { get; set; }
    }
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
        public Dictionary<string, decimal> SalaryByDepartment { get; set; } = new();
    }

    public class PayrollTeacherDetailModel
    {
        public Guid Id { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string TeacherCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
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
    /// <summary>
    /// Lấy tổng kết lương
    /// </summary>
    public class GetPayrollSummaryQuery : IRequest<Result<PayrollSummaryModel>>
    {
        public int? Year { get; set; }
        public int? Month { get; set; }

        public class GetPayrollSummaryQueryHandler : IRequestHandler<GetPayrollSummaryQuery, Result<PayrollSummaryModel>>
        {
            private readonly IRegalEducationDbContext _context;

            public GetPayrollSummaryQueryHandler(IRegalEducationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<Result<PayrollSummaryModel>> Handle(GetPayrollSummaryQuery request, CancellationToken cancellationToken)
            {
                var query = _context.PayrollTeachers.Where(pt => !pt.IsDeleted);

                if (request.Year.HasValue)
                    query = query.Where(pt => pt.SalaryMonth.Year == request.Year.Value);

                if (request.Month.HasValue)
                    query = query.Where(pt => pt.SalaryMonth.Month == request.Month.Value);

                var totalRecords = await query.CountAsync(cancellationToken);
                var totalSalary = await query.SumAsync(pt => pt.SalaryAmount, cancellationToken);
                var paidCount = await query.CountAsync(pt => pt.IsPaid, cancellationToken);
                var unpaidCount = totalRecords - paidCount;
                var averageSalary = totalRecords > 0 ? totalSalary / totalRecords : 0;

                var summary = new PayrollSummaryModel
                {
                    TotalRecords = totalRecords,
                    TotalSalaryAmount = totalSalary,
                    AverageSalary = averageSalary,
                    PaidCount = paidCount,
                    UnpaidCount = unpaidCount
                };

                return Result<PayrollSummaryModel>.Success(summary);
            }
        }
    }
}