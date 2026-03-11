using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Item.Queries
{
    public class GetItemByIdQuery : IRequest<Result<ItemModel>>
    {
        public required string Id { get; set; }
    }

    public class Handler_GetById : IRequestHandler<GetItemByIdQuery, Result<ItemModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public Handler_GetById(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result<ItemModel>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Items
                .AsNoTracking ( )
                .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

            if (entity == null)
                return Result<ItemModel>.Failure (_localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["Item"], request.Id));

            return Result<ItemModel>.Success (_mapper.Map<ItemModel> (entity));
        }
    }
}
