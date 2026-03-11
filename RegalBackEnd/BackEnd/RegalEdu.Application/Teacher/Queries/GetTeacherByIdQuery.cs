using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Teacher.Queries
{
    public class GetTeacherByIdQuery : IRequest<Result<TeacherModel>>
    {
        public required string Id { get; set; }

        public class GetTeacherByIdQueryHandler : IRequestHandler<GetTeacherByIdQuery, Result<TeacherModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public GetTeacherByIdQueryHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result<TeacherModel>> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
            {
                var teacher = await _context.Teachers
                    .Include (t => t.Company)
                    .AsNoTracking ( )
                    .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

                if (teacher == null)
                {
                    var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["Teacher"], request.Id);
                    return Result<TeacherModel>.Failure (msg);
                }

                var result = new TeacherModel
                {
                    Id = teacher.Id,

                    TeacherNickname = teacher.TeacherNickname,
                    TeacherQualifications = teacher.TeacherQualifications,
                    TeacherSpecialization = teacher.TeacherSpecialization,
                    WorkType = teacher.WorkType,
                    JoinDate = teacher.JoinDate,
                    PreferLevel = teacher.PreferLevel,
                    TeachingOutside = teacher.TeachingOutside,
                    TeacherAssistant = teacher.TeacherAssistant,
                    IsOnline = teacher.IsOnline

                };

                return Result<TeacherModel>.Success (result);
            }
        }
    }
}
