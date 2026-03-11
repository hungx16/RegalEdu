using AutoMapper;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkBoardTeacher.Commands
{
    public class AddWorkBoardTeacherCommand : IRequest<Result>
    {
        public required WorkBoardTeacherModel WorkBoardTeacherModel { get; set; }

        public class AddWorkBoardTeacherCommandHandler : IRequestHandler<AddWorkBoardTeacherCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public AddWorkBoardTeacherCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            }

            public async Task<Result> Handle(AddWorkBoardTeacherCommand request, CancellationToken cancellationToken)
            {
                var workBoardTeacher = _mapper.Map<Domain.Entities.WorkBoardTeacher>(request.WorkBoardTeacherModel);

                await _context.WorkBoardTeachers.AddAsync(workBoardTeacher, cancellationToken);
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (success)
                {
                    return Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.WorkBoardTeacher));
                }
                else
                {
                    return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.WorkBoardTeacher));
                }
            }
        }
    }
}