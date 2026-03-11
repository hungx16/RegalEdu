using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.SupportingDocument.Queries
{
    public class GetSupportingDocumentByIdQuery : IRequest<Result<SupportingDocumentModel>>
    {
        public required string Id { get; set; }

        public class Handler : IRequestHandler<GetSupportingDocumentByIdQuery, Result<SupportingDocumentModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public Handler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result<SupportingDocumentModel>> Handle(GetSupportingDocumentByIdQuery request, CancellationToken cancellationToken)
            {
                var entity = await _context.SupportingDocuments
                    .AsNoTracking ( )
                    .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

                if (entity == null)
                {
                    var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["SupportingDocument"], request.Id);
                    return Result<SupportingDocumentModel>.Failure (msg);
                }

                var result = _mapper.Map<SupportingDocumentModel> (entity);
                return Result<SupportingDocumentModel>.Success (result);
            }
        }
    }
}
