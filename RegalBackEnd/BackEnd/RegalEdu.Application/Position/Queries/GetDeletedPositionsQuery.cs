using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Position.Queries
{
    public class GetDeletedPositionsQuery : IRequest<Result<List<PositionModel>>> { }

    public class GetDeletedPositionsQueryHandler : IRequestHandler<GetDeletedPositionsQuery, Result<List<PositionModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetDeletedPositionsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<PositionModel>>> Handle(GetDeletedPositionsQuery request, CancellationToken cancellationToken)
        {
            var positions = await _context.Positions
                .IgnoreQueryFilters ( )
                .Where (d => d.IsDeleted)
                .AsNoTracking ( )
                .ToListAsync (cancellationToken);
            var result = _mapper.Map<List<PositionModel>> (positions);
            return Result<List<PositionModel>>.Success (result);
        }
    }
}
