using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Position.Queries
{
    public class GetAllPositionsQuery : IRequest<Result<List<PositionModel>>> { }

    public class GetAllPositionsQueryHandler : IRequestHandler<GetAllPositionsQuery, Result<List<PositionModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllPositionsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<PositionModel>>> Handle(GetAllPositionsQuery request, CancellationToken cancellationToken)
        {
            var positions = await _context.Positions
                                                    .Include (t => t.DepartmentPositions).ThenInclude (t => t.Department)
                                                    .AsNoTracking ( ).ToListAsync ( );
            var result = _mapper.Map<List<PositionModel>> (positions);
            return Result<List<PositionModel>>.Success (result);
        }
    }
}
