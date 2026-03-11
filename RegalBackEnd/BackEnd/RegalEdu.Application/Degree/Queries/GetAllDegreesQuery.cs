using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Degree.Queries
{
    public class GetAllDegreesQuery : IRequest<Result<List<DegreeModel>>>
    {
        public class GetAllDegreesQueryHandler : IRequestHandler<GetAllDegreesQuery, Result<List<DegreeModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetAllDegreesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<DegreeModel>>> Handle(GetAllDegreesQuery request, CancellationToken cancellationToken)
            {
                var degrees = await _context.Degrees.AsNoTracking ( ).ToListAsync (cancellationToken);
                var result = _mapper.Map<List<DegreeModel>> (degrees);
                return Result<List<DegreeModel>>.Success (result);
            }
        }
    }
}
