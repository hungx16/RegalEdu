using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.TeacherWorkLogs.Commands
{
    public class AddTeacherWorkLogCommand : IRequest<Result>
    {
        public required TeacherWorkLogModel TeacherWorkLogModel { get; set; }
    }

    public class AddTeacherWorkLogCommandHandler : IRequestHandler<AddTeacherWorkLogCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localizer;
        private readonly IMapper _mapper;

        public AddTeacherWorkLogCommandHandler(
            IRegalEducationDbContext context,
            ILocalizationService localizer,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> Handle(AddTeacherWorkLogCommand request, CancellationToken cancellationToken)
        {
            if (request.TeacherWorkLogModel == null)
            {
                return Result.Failure(
                    _localizer.Format(LocalizationKey.ERR_INVALID_VALUE, "TeacherWorkLogModel"));
            }

            var model = request.TeacherWorkLogModel;

            if (!model.WorkingTimeId.HasValue && model.StartTime >= model.EndTime)
            {
                return Result.Failure(
                    _localizer.Format(LocalizationKey.ERR_INVALID_VALUE, "TimeRange"));
            }

            var teacherExists = await _context.Teachers
                .AnyAsync(t => t.Id == model.TeacherId && !t.IsDeleted, cancellationToken);

            if (!teacherExists)
            {
                return Result.Failure(
                    _localizer.Format(LocalizationKey.EntityWithIdNotFound, _localizer["Teacher"], model.TeacherId));
            }

            RegalEdu.Domain.Entities.WorkingTime? workingTime = null;
            if (model.WorkingTimeId.HasValue)
            {
                workingTime = await _context.WorkingTimes
                    .FirstOrDefaultAsync(wt => wt.Id == model.WorkingTimeId.Value && !wt.IsDeleted, cancellationToken);

                if (workingTime == null)
                {
                    return Result.Failure(
                        _localizer.Format(LocalizationKey.EntityWithIdNotFound, _localizer["WorkingTime"], model.WorkingTimeId.Value));
                }

                if ((byte)model.Date.DayOfWeek != workingTime.DayOfWeek)
                {
                    return Result.Failure(
                        _localizer.Format(LocalizationKey.ERR_INVALID_VALUE, "WorkingTimeDayOfWeek"));
                }
            }

            var workLog = _mapper.Map<TeacherWorkLog>(model);
            workLog.Date = model.Date.Date;

            if (workingTime != null)
            {
                workLog.StartTime = workingTime.StartTime;
                workLog.EndTime = workingTime.EndTime;
            }

            await _context.TeacherWorkLogs.AddAsync(workLog, cancellationToken);

            var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (saved)
            {
                return Result.Success(
                    _localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, "TeacherWorkLog"));
            }

            return Result.Failure(
                _localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, "TeacherWorkLog"));
        }
    }
}
