using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PayrollTeacher.Commands
{
    public class UpdatePayrollTeacherCommand : IRequest<Result>
    {
        public required PayrollTeacherModel PayrollTeacherModel { get; set; }

        public class UpdatePayrollTeacherCommandHandler : IRequestHandler<UpdatePayrollTeacherCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public UpdatePayrollTeacherCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            }

            public async Task<Result> Handle(UpdatePayrollTeacherCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.PayrollTeachers.FirstOrDefaultAsync(x => x.Id == request.PayrollTeacherModel.Id, cancellationToken);
                if (entity == null)
                {
                    return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, _localizer[EntityName.PayrollTeacher]));
                }
                _mapper.Map(request.PayrollTeacherModel, entity);

                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (success)
                    return Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, _localizer[EntityName.PayrollTeacher]));
                else
                    return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer[EntityName.PayrollTeacher]));
            }
        }
    }
}