using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.EvaluateTeacher.Queries
{
    public class GetEvaluateTeacherByIdQuery : IRequest<Result<EvaluateTeacherModel>>
    {
        public required Guid Id { get; set; }

        public class GetEvaluateTeacherByIdQueryHandler : IRequestHandler<GetEvaluateTeacherByIdQuery, Result<EvaluateTeacherModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public GetEvaluateTeacherByIdQueryHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            }

            public async Task<Result<EvaluateTeacherModel>> Handle(GetEvaluateTeacherByIdQuery request, CancellationToken cancellationToken)
            {
                var evaluateTeacher = await _context.EvaluateTeachers
                    .Include(et => et.Teacher)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

                if (evaluateTeacher == null)
                {
                    var msg = _localizer.Format(LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.EvaluateTeacher], request.Id);
                    return Result<EvaluateTeacherModel>.Failure(msg);
                }

                var result = _mapper.Map<EvaluateTeacherModel>(evaluateTeacher);
                return Result<EvaluateTeacherModel>.Success(result);
            }
        }
    }
}