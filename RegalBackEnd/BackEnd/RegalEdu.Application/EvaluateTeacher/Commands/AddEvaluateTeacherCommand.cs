using AutoMapper;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.EvaluateTeacher.Commands
{
    public class AddEvaluateTeacherCommand : IRequest<Result>
    {
        public required EvaluateTeacherModel EvaluateTeacherModel { get; set; }

        public class AddEvaluateTeacherCommandHandler : IRequestHandler<AddEvaluateTeacherCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public AddEvaluateTeacherCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            }

            public async Task<Result> Handle(AddEvaluateTeacherCommand request, CancellationToken cancellationToken)
            {
                var evaluateTeacher = _mapper.Map<Domain.Entities.EvaluateTeacher>(request.EvaluateTeacherModel);

                await _context.EvaluateTeachers.AddAsync(evaluateTeacher, cancellationToken);
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (success)
                {
                    return Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.EvaluateTeacher));
                }
                else
                {
                    return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.EvaluateTeacher));
                }
            }
        }
    }
}