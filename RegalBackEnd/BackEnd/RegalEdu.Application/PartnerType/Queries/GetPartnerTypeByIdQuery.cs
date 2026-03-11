using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PartnerType.Queries
{
    public class GetPartnerTypeByIdQuery : IRequest<Result<PartnerTypeModel>>
    {
        public required string Id { get; set; }
    }

    public class Handler_GetById : IRequestHandler<GetPartnerTypeByIdQuery, Result<PartnerTypeModel>>
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

        public async Task<Result<PartnerTypeModel>> Handle(GetPartnerTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.PartnerTypes
                .AsNoTracking ( )
                .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

            if (entity == null)
                return Result<PartnerTypeModel>.Failure (_localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["PartnerType"], request.Id));

            return Result<PartnerTypeModel>.Success (_mapper.Map<PartnerTypeModel> (entity));
        }
    }
}