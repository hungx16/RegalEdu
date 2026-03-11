using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.LuckyDraw.Queries
{
    public class GetAllActiveLuckyDrawsQuery : IRequest<Result<List<LuckyDrawDto>>>
    {
    }

    public class GetAllActiveLuckyDrawsQueryHandler : IRequestHandler<GetAllActiveLuckyDrawsQuery, Result<List<LuckyDrawDto>>> 
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllActiveLuckyDrawsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<List<LuckyDrawDto>>> Handle(GetAllActiveLuckyDrawsQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.SetEntity<RegalEdu.Domain.Entities.LuckyDraw>()
                .AsNoTracking()
                .Where(x => x.Status == 1)
                .OrderByDescending(x => x.StartDate)
                .ToListAsync(cancellationToken);

            var dtos = list.Select(x => _mapper.Map<LuckyDrawDto>(x)).ToList();
            return Result<List<LuckyDrawDto>>.Success(dtos);
        }
    }
}
