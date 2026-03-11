using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkBoardTeacher.Queries
{
    public class GetWorkBoardTeacherByIdQuery : IRequest<Result<WorkBoardTeacherModel>>
    {
        public required Guid Id { get; set; }

        public class GetWorkBoardTeacherByIdQueryHandler : IRequestHandler<GetWorkBoardTeacherByIdQuery, Result<WorkBoardTeacherModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public GetWorkBoardTeacherByIdQueryHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            }

            public async Task<Result<WorkBoardTeacherModel>> Handle(GetWorkBoardTeacherByIdQuery request, CancellationToken cancellationToken)
            {
                var workBoardTeacher = await _context.WorkBoardTeachers
                    .Include(wbt => wbt.Teacher)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

                if (workBoardTeacher == null)
                {
                    var msg = _localizer.Format(LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.WorkBoardTeacher], request.Id);
                    return Result<WorkBoardTeacherModel>.Failure(msg);
                }

                var result = _mapper.Map<WorkBoardTeacherModel>(workBoardTeacher);
                return Result<WorkBoardTeacherModel>.Success(result);
            }
        }
    }
}