using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Common.Queries
{
    public class GetDocumentTypesQuery : IRequest<Result<List<DocumentType>>> { }


    public class GetDocumentTypesQueryHandler : IRequestHandler<GetDocumentTypesQuery, Result<List<DocumentType>>>
    {
        private readonly ILocalizationService _localizer;
        private readonly ILogger<GetDocumentTypesQueryHandler> _logger;
        public GetDocumentTypesQueryHandler(ILogger<GetDocumentTypesQueryHandler> logger, ILocalizationService localizer)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger)); ;
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer)); ;
        }
        public async Task<Result<List<DocumentType>>> Handle(GetDocumentTypesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var DocumentTypes = await DocumentTypeFileHelper.LoadDocumentTypesAsync ( );
                return Result<List<DocumentType>>.Success (DocumentTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError (ex, "An unexpected error occurred while getting document type data.");
                return Result<List<DocumentType>>.Failure (_localizer.Format (LocalizationKey.UnexpectedError), ex);
            }

        }
    }

}
