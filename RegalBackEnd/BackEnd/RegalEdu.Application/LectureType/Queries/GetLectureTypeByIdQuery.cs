using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LectureType.Queries
{
    public class GetLectureTypeByIdQuery : IRequest<Result<LectureTypeModel>>
    {
        public required string Id { get; set; }

        public class GetLectureTypeByIdQueryHandler : IRequestHandler<GetLectureTypeByIdQuery, Result<LectureTypeModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public GetLectureTypeByIdQueryHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result<LectureTypeModel>> Handle(GetLectureTypeByIdQuery request, CancellationToken cancellationToken)
            {
                var entity = await _context.LectureTypes
                    .AsNoTracking ( )
                    .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

                if (entity == null)
                {
                    var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["LectureType"], request.Id);
                    return Result<LectureTypeModel>.Failure (msg);
                }

                var result = _mapper.Map<LectureTypeModel> (entity);
                return Result<LectureTypeModel>.Success (result);
            }
        }
    }
}
