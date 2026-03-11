using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.LuckyDraw.Queries
{
    public class GetLuckyDrawByIdQuery : IRequest<Result<LuckyDrawDto>>
    {
        public required Guid Id { get; set; }
    }

    public class GetLuckyDrawByIdQueryHandler : IRequestHandler<GetLuckyDrawByIdQuery, Result<LuckyDrawDto>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetLuckyDrawByIdQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<LuckyDrawDto>> Handle(GetLuckyDrawByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.SetEntity<RegalEdu.Domain.Entities.LuckyDraw>().FindAsync(new object[] { request.Id });
            if (entity == null) return Result<LuckyDrawDto>.Failure("Not found");
            return Result<LuckyDrawDto>.Success(_mapper.Map<LuckyDrawDto>(entity));
        }
    }
}
