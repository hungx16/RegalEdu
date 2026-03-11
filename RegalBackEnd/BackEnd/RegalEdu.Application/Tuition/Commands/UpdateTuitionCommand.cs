using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Tuition.Commands
{
    public class UpdateTuitionCommand : IRequest<Result>
    {
        public required TuitionModel TuitionModel { get; set; }
    }

    public class UpdateTuitionCommandHandler : IRequestHandler<UpdateTuitionCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateTuitionCommandHandler(IRegalEducationDbContext context, AutoMapper.IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(UpdateTuitionCommand request, CancellationToken cancellationToken)
        {
            if (request?.TuitionModel == null)
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer[EntityName.Tuition]));

            var id = request.TuitionModel.Id;

            // 1) Nếu local đang track entity cùng Id thì detach để tránh double-tracking
            var local = _context.Tuition.Local.FirstOrDefault(x => x.Id == id);
            if (local != null)
            {
                _context.EntryEntity(local).State = EntityState.Detached;
            }

            // 2) Lấy entity từ DB ở trạng thái tracking
            var tuition = await _context.Tuition
                // KHÔNG Include navigation ở đây, chỉ update scalar/FK
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (tuition == null)
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, EntityName.Tuition));

            var m = request.TuitionModel;

            if (m.CourseId != Guid.Empty && m.ClassTypeId != Guid.Empty)
            {
                bool duplicateCombination = await _context.Tuition
                    .AnyAsync(x =>
                        x.Id != id &&
                        x.CourseId == m.CourseId &&
                        x.ClassTypeId == m.ClassTypeId,
                        cancellationToken);

                if (duplicateCombination)
                {
                    var duplicateInfo = $"{_localizer[EntityName.Course]}: {m.CourseId}, {_localizer[EntityName.ClassType]}: {m.ClassTypeId}";
                    return Result.Failure(
                        _localizer.Format(
                            LocalizationKey.ERR_DUPLICATE_VALUE,
                            _localizer[EntityName.Tuition],
                            duplicateInfo
                        )
                    );
                }
            }

            // 3) Gán trực tiếp scalar & FK (KHÔNG gán navigation)
            tuition.TuitionCode = (m.TuitionCode ?? string.Empty).Trim();
            tuition.TuitionName = (m.TuitionName ?? string.Empty).Trim();
            tuition.CourseId = m.CourseId;        // FK
            tuition.ClassTypeId = m.ClassTypeId;     // FK
            tuition.DurationHours = m.DurationHours;
            tuition.MinHours = m.MinHours;
            tuition.TotalMonths = m.TotalMonths;
            tuition.Unit = m.Unit;            // enum/int
            tuition.TuitionFee = m.TuitionFee;
            tuition.Status = m.Status;          // 0/1
            tuition.StartDate = m.StartDate;
            tuition.EndDate = m.EndDate;


            if (tuition.MinHours > tuition.DurationHours)
                return Result.Failure(_localizer["validation.minHoursNotExceed"]);



            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            return success
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, _localizer[EntityName.Tuition]))
                : Result.Success(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer[EntityName.Tuition]));
        }

    }
}
