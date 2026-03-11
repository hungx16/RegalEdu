using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PayrollTeacher.Queries
{
    public class PayrollTeacherQuery
    {
        public Guid? TeacherId { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public bool? IsPaid { get; set; }
        public decimal? MinSalaryAmount { get; set; }
        public decimal? MaxSalaryAmount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedPayrollTeachersQuery : IRequest<Result<PagedResult<PayrollTeacherModel>>>
    {
        public PayrollTeacherQuery? PayrollTeacherQuery { get; set; }

        public class GetPagedPayrollTeachersQueryHandler : IRequestHandler<GetPagedPayrollTeachersQuery, Result<PagedResult<PayrollTeacherModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly PagingOptions _pagingOptions;
            private readonly IMapper _mapper;

            public GetPagedPayrollTeachersQueryHandler(
                IRegalEducationDbContext context,
                PagingOptions pagingOptions,
                IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException(nameof(pagingOptions));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Result<PagedResult<PayrollTeacherModel>>> Handle(GetPagedPayrollTeachersQuery request, CancellationToken cancellationToken)
            {
                if (request.PayrollTeacherQuery == null)
                    throw new ArgumentNullException(nameof(request.PayrollTeacherQuery));

                var query = _context.PayrollTeachers
                    .Include(pt => pt.Teacher)
                    .AsNoTracking();

                if (request.PayrollTeacherQuery.TeacherId.HasValue)
                    query = query.Where(pt => pt.TeacherId == request.PayrollTeacherQuery.TeacherId.Value);

                if (request.PayrollTeacherQuery.Year.HasValue)
                    query = query.Where(pt => pt.SalaryMonth.Year == request.PayrollTeacherQuery.Year.Value);

                if (request.PayrollTeacherQuery.Month.HasValue)
                    query = query.Where(pt => pt.SalaryMonth.Month == request.PayrollTeacherQuery.Month.Value);

                if (request.PayrollTeacherQuery.IsPaid.HasValue)
                    query = query.Where(pt => pt.IsPaid == request.PayrollTeacherQuery.IsPaid.Value);

                if (request.PayrollTeacherQuery.MinSalaryAmount.HasValue)
                    query = query.Where(pt => pt.SalaryAmount >= request.PayrollTeacherQuery.MinSalaryAmount.Value);

                if (request.PayrollTeacherQuery.MaxSalaryAmount.HasValue)
                    query = query.Where(pt => pt.SalaryAmount <= request.PayrollTeacherQuery.MaxSalaryAmount.Value);

                int totalRecords = await query.CountAsync(cancellationToken);
                request.PayrollTeacherQuery.PageSize = _pagingOptions.DefaultPageSize;
                var paged = await query
                    .OrderByDescending(x => x.SalaryMonth)
                    .ThenByDescending(x => x.CreatedAt)
                    .Skip((request.PayrollTeacherQuery.Page - 1) * request.PayrollTeacherQuery.PageSize)
                    .Take(request.PayrollTeacherQuery.PageSize)
                    .ToListAsync(cancellationToken);

                var result = paged.Select(pt => _mapper.Map<PayrollTeacherModel>(pt)).ToList();

                var pagedResult = new PagedResult<PayrollTeacherModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<PayrollTeacherModel>>.Success(pagedResult);
            }
        }
    }
}