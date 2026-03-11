using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PayrollTeacher.Queries
{
    public class GetAllPayrollTeachersQuery : IRequest<Result<List<PayrollTeacherModel>>>
    {
        public class GetAllPayrollTeachersQueryHandler : IRequestHandler<GetAllPayrollTeachersQuery, Result<List<PayrollTeacherModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetAllPayrollTeachersQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Result<List<PayrollTeacherModel>>> Handle(GetAllPayrollTeachersQuery request, CancellationToken cancellationToken)
            {
                var payrollTeachers = await _context.PayrollTeachers
                        .AsNoTracking().AsSplitQuery()
                        .Include(pt => pt.Teacher)
                    .ToListAsync(cancellationToken);

                var result = _mapper.Map<List<PayrollTeacherModel>>(payrollTeachers);
                return Result<List<PayrollTeacherModel>>.Success(result);
            }
        }
    }
}