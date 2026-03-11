using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Tuition.Queries
{
    public class GetDeletedTuitionQuery : IRequest<Result<List<TuitionModel>>> { }

    public class GetDeletedTuitionQueryHandler : IRequestHandler<GetDeletedTuitionQuery, Result<List<TuitionModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetDeletedTuitionQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<TuitionModel>>> Handle(GetDeletedTuitionQuery request, CancellationToken cancellationToken)
        {
            var tuitions = await _context.Tuition
                .IgnoreQueryFilters ( )
                .Include (t => t.Course)
                .Include (t => t.ClassType)
                .Where (d => d.IsDeleted)
                .AsNoTracking ( )
                .ToListAsync (cancellationToken);

            var result = _mapper.Map<List<TuitionModel>> (tuitions);
            return Result<List<TuitionModel>>.Success (result);
        }
    }
}
