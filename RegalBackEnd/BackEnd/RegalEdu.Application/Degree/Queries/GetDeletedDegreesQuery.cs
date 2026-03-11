using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Degree.Queries
{
    public class GetDeletedDegreesQuery : IRequest<Result<List<DegreeModel>>>
    {
        public class GetDeletedDegreesQueryHandler : IRequestHandler<GetDeletedDegreesQuery, Result<List<DegreeModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetDeletedDegreesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<DegreeModel>>> Handle(GetDeletedDegreesQuery request, CancellationToken cancellationToken)
            {
                var entities = await _context.Degrees
                    .IgnoreQueryFilters ( )
                    .Where (x => x.IsDeleted)
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<DegreeModel>> (entities);
                return Result<List<DegreeModel>>.Success (result);
            }
        }
    }
}
