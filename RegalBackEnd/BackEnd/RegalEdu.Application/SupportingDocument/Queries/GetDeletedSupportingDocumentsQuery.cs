using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.SupportingDocument.Queries
{
    public class GetDeletedSupportingDocumentsQuery : IRequest<Result<List<SupportingDocumentModel>>>
    {
        public class Handler : IRequestHandler<GetDeletedSupportingDocumentsQuery, Result<List<SupportingDocumentModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public Handler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<SupportingDocumentModel>>> Handle(GetDeletedSupportingDocumentsQuery request, CancellationToken cancellationToken)
            {
                var data = await _context.SupportingDocuments
                    .IgnoreQueryFilters ( )
                    .Where (x => x.IsDeleted)
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<SupportingDocumentModel>> (data);
                return Result<List<SupportingDocumentModel>>.Success (result);
            }
        }
    }
}
