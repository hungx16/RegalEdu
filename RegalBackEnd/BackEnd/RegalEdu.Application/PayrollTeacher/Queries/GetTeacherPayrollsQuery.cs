using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PayrollTeacher.Queries
{
    public class GetTeacherPayrollsQuery : IRequest<Result<List<PayrollTeacherModel>>>
    {
        public required Guid TeacherId { get; set; }
        public int? Year { get; set; }

        public class GetTeacherPayrollsQueryHandler : IRequestHandler<GetTeacherPayrollsQuery, Result<List<PayrollTeacherModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetTeacherPayrollsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Result<List<PayrollTeacherModel>>> Handle(GetTeacherPayrollsQuery request, CancellationToken cancellationToken)
            {
                var query = _context.PayrollTeachers
                    .Where(pt => pt.TeacherId == request.TeacherId && !pt.IsDeleted);

                if (request.Year.HasValue)
                {
                    query = query.Where(pt => pt.SalaryMonth.Year == request.Year.Value);
                }

                var payrolls = await query
                    .OrderByDescending(pt => pt.SalaryMonth)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                var result = _mapper.Map<List<PayrollTeacherModel>>(payrolls);
                return Result<List<PayrollTeacherModel>>.Success(result);
            }
        }
    }
}