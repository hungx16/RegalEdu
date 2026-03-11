using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Degree.Queries
{
    public class GetDegreeByIdQuery : IRequest<Result<DegreeModel>>
    {
        public required string Id { get; set; }

        public class GetDegreeByIdQueryHandler : IRequestHandler<GetDegreeByIdQuery, Result<DegreeModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public GetDegreeByIdQueryHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result<DegreeModel>> Handle(GetDegreeByIdQuery request, CancellationToken cancellationToken)
            {
                var entity = await _context.Degrees
                    .AsNoTracking ( )
                    .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

                if (entity == null)
                {
                    var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["Degree"], request.Id);
                    return Result<DegreeModel>.Failure (msg);
                }

                var result = _mapper.Map<DegreeModel> (entity);
                return Result<DegreeModel>.Success (result);
            }
        }
    }
}
