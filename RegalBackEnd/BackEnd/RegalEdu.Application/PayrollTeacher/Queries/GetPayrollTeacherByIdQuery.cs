using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PayrollTeacher.Queries
{
    public class GetPayrollTeacherByIdQuery : IRequest<Result<PayrollTeacherModel>>
    {
        public required Guid Id { get; set; }

        public class GetPayrollTeacherByIdQueryHandler : IRequestHandler<GetPayrollTeacherByIdQuery, Result<PayrollTeacherModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public GetPayrollTeacherByIdQueryHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            }

            public async Task<Result<PayrollTeacherModel>> Handle(GetPayrollTeacherByIdQuery request, CancellationToken cancellationToken)
            {
                var payrollTeacher = await _context.PayrollTeachers
                    .Include(pt => pt.Teacher)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

                if (payrollTeacher == null)
                {
                    var msg = _localizer.Format(LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.PayrollTeacher], request.Id);
                    return Result<PayrollTeacherModel>.Failure(msg);
                }

                var result = _mapper.Map<PayrollTeacherModel>(payrollTeacher);
                return Result<PayrollTeacherModel>.Success(result);
            }
        }
    }
}