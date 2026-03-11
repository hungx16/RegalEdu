using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Gift.Queries
{
    public class GetAllGiftsQuery : IRequest<Result<List<GiftModel>>> { }

    public class GetAllGiftsQueryHandler : IRequestHandler<GetAllGiftsQuery, Result<List<GiftModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllGiftsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<GiftModel>>> Handle(GetAllGiftsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _context.Gift
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result<List<GiftModel>>.Success(
                _mapper.Map<List<GiftModel>>(entities)
            );
        }
    }
}
