using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.SupportingDocument.Queries
{
    public class SupportingDocumentQuery
    {
        public string? DocumentName { get; set; }
        public int? DocumentTypeId { get; set; }
        public string? WebsiteKeyContains { get; set; }
        public string? AuthorName { get; set; }
        public DateTime? PublishDateFrom { get; set; }
        public DateTime? PublishDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedSupportingDocumentsQuery : IRequest<Result<PagedResult<SupportingDocumentModel>>>
    {
        public SupportingDocumentQuery? SupportingDocumentQuery { get; set; }

        public class Handler : IRequestHandler<GetPagedSupportingDocumentsQuery, Result<PagedResult<SupportingDocumentModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly PagingOptions _pagingOptions;
            private readonly IMapper _mapper;

            public Handler(IRegalEducationDbContext context, PagingOptions pagingOptions, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<PagedResult<SupportingDocumentModel>>> Handle(GetPagedSupportingDocumentsQuery request, CancellationToken cancellationToken)
            {
                if (request.SupportingDocumentQuery == null)
                    throw new ArgumentNullException (nameof (request.SupportingDocumentQuery));

                var q = _context.SupportingDocuments.AsNoTracking ( );

                if (!string.IsNullOrWhiteSpace (request.SupportingDocumentQuery.DocumentName))
                    q = q.Where (x => x.DocumentName.Contains (request.SupportingDocumentQuery.DocumentName));

                if (request.SupportingDocumentQuery.DocumentTypeId.HasValue)
                    q = q.Where (x => x.DocumentTypeId == request.SupportingDocumentQuery.DocumentTypeId.Value);

                if (!string.IsNullOrWhiteSpace (request.SupportingDocumentQuery.WebsiteKeyContains))
                    q = q.Where (x => x.WebsiteKeys != null && x.WebsiteKeys.Contains (request.SupportingDocumentQuery.WebsiteKeyContains));

                if (!string.IsNullOrWhiteSpace (request.SupportingDocumentQuery.AuthorName))
                    q = q.Where (x => x.AuthorName != null && x.AuthorName.Contains (request.SupportingDocumentQuery.AuthorName));

                if (request.SupportingDocumentQuery.PublishDateFrom.HasValue)
                    q = q.Where (x => x.StartDate >= request.SupportingDocumentQuery.PublishDateFrom.Value);

                if (request.SupportingDocumentQuery.PublishDateTo.HasValue)
                    q = q.Where (x => x.StartDate <= request.SupportingDocumentQuery.PublishDateTo.Value);

                if (request.SupportingDocumentQuery.EndDateFrom.HasValue)
                    q = q.Where (x => x.EndDate >= request.SupportingDocumentQuery.EndDateFrom.Value);

                if (request.SupportingDocumentQuery.EndDateTo.HasValue)
                    q = q.Where (x => x.EndDate <= request.SupportingDocumentQuery.EndDateTo.Value);

                var total = await q.CountAsync (cancellationToken);
                request.SupportingDocumentQuery.PageSize = _pagingOptions.DefaultPageSize;

                var pageItems = await q
                    .OrderByDescending (x => x.CreatedAt)
                    .Skip ((request.SupportingDocumentQuery.Page - 1) * request.SupportingDocumentQuery.PageSize)
                    .Take (request.SupportingDocumentQuery.PageSize)
                    .ToListAsync (cancellationToken);

                var mapped = pageItems.Select (_mapper.Map<SupportingDocumentModel>).ToList ( );

                var result = new PagedResult<SupportingDocumentModel> { Items = mapped, Total = total };
                return Result<PagedResult<SupportingDocumentModel>>.Success (result);
            }
        }
    }
}
