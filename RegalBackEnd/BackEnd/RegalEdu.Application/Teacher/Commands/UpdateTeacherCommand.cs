using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Teacher.Commands
{
    public class UpdateTeacherCommand : IRequest<Result>
    {
        public required TeacherModel TeacherModel { get; set; }

        public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public UpdateTeacherCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
            {
                // 1. Lấy Employee từ DB
                var teacherEntity = await _context.Teachers
                    .FirstOrDefaultAsync (x => x.Id == request.TeacherModel.Id, cancellationToken);

                if (teacherEntity == null)
                {
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, "Teacher"));
                }
                // 2. Lấy ApplicationUser từ DB qua ApplicationUserId (đã có trong Employee)
                var applicationUserEntity = await _context.ApplicationUsers
                    .FirstOrDefaultAsync (u => u.Id == teacherEntity.ApplicationUserId, cancellationToken);

                if (applicationUserEntity == null)
                {
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, "ApplicationUser"));
                }

                // 3. Map thông tin update vào entity hiện tại
                _mapper.Map (request.TeacherModel.ApplicationUser, applicationUserEntity);

                // Update thông tin Employee (vị trí, phòng ban, mã số thuế, ...)
                _mapper.Map (request.TeacherModel, teacherEntity);

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                if (success)
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["Teacher"]));
                else
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Teacher"]));
            }
        }
    }
}
