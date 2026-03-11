using AutoMapper;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PayrollTeacher.Commands
{
    public class AddPayrollTeacherCommand : IRequest<Result>
    {
        public required PayrollTeacherModel PayrollTeacherModel { get; set; }

        public class AddPayrollTeacherCommandHandler : IRequestHandler<AddPayrollTeacherCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public AddPayrollTeacherCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            }

            public async Task<Result> Handle(AddPayrollTeacherCommand request, CancellationToken cancellationToken)
            {
                var payrollTeacher = _mapper.Map<Domain.Entities.PayrollTeacher>(request.PayrollTeacherModel);

                await _context.PayrollTeachers.AddAsync(payrollTeacher, cancellationToken);
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (success)
                {
                    return Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.PayrollTeacher));
                }
                else
                {
                    return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.PayrollTeacher));
                }
            }
        }
    }
}