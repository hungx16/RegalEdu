using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.SupportingDocument.Queries
{
    public class GetAllPublishedSupportingDocumentsQuery : IRequest<Result<List<SupportingDocumentModel>>>
    {
        public class Handler : IRequestHandler<GetAllPublishedSupportingDocumentsQuery, Result<List<SupportingDocumentModel>>>
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

            public async Task<Result<List<SupportingDocumentModel>>> Handle(GetAllPublishedSupportingDocumentsQuery request, CancellationToken cancellationToken)
            {
                var lang = _localizer.GetCurrentLanguage ( );

                var data = await _context.SupportingDocuments
                    .Include (t => t.Image)
                    .Include (t => t.Attachment)
                    .Where (t => t.IsPublish == true
                                 && t.Status == RegalEdu.Domain.Enums.StatusType.Active)
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<SupportingDocumentModel>> (data);

                if (lang.Equals ("en", StringComparison.OrdinalIgnoreCase))
                {

                    // Chỉ lấy các bản ghi hỗ trợ song ngữ
                    result = result
                        .Where (item => item.IsMultilingual)
                        .Select (item =>
                        {
                            // Cập nhật tiếng Anh
                            item.DocumentName = !string.IsNullOrWhiteSpace (item.EnDocumentName) ? item.EnDocumentName : item.DocumentName;
                            item.Description = !string.IsNullOrWhiteSpace (item.EnDescription) ? item.EnDescription : item.Description;
                            item.WebsiteKeys = !string.IsNullOrWhiteSpace (item.EnWebsiteKeys) ? item.EnWebsiteKeys : item.WebsiteKeys;
                            item.AuthorName = !string.IsNullOrWhiteSpace (item.EnAuthorName) ? item.EnAuthorName : item.AuthorName;

                            return item;
                        })
                        .ToList ( );
                }

                return Result<List<SupportingDocumentModel>>.Success (result);
            }

        }
    }
}
