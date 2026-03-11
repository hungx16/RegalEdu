using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Item.Queries
{
    public class GetDeletedItemsQuery : IRequest<Result<List<ItemModel>>> { }

    public class Handler_GetDeleted : IRequestHandler<GetDeletedItemsQuery, Result<List<ItemModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public Handler_GetDeleted(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<ItemModel>>> Handle(GetDeletedItemsQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.Items
                .IgnoreQueryFilters ( )
                .Where (x => x.IsDeleted)
                .AsNoTracking ( )
                .ToListAsync (cancellationToken);

            return Result<List<ItemModel>>.Success (_mapper.Map<List<ItemModel>> (list));
        }
    }
}
