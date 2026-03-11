using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Item.Queries
{
    public class GetAllItemsQuery : IRequest<Result<List<ItemModel>>> { }

    public class Handler_GetAll : IRequestHandler<GetAllItemsQuery, Result<List<ItemModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public Handler_GetAll(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<ItemModel>>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.Items.AsNoTracking ( ).ToListAsync (cancellationToken);
            return Result<List<ItemModel>>.Success (_mapper.Map<List<ItemModel>> (list));
        }
    }
}
