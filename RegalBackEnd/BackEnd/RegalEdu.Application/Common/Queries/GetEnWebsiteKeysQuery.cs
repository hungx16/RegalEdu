using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models.DTO;



namespace RegalEdu.Application.Common.Queries
{
    public class GetEnWebsiteKeysQuery : IRequest<Result<List<WebsiteKey>>> { }


    public class GetEnWebsiteKeysQueryHandler : IRequestHandler<GetEnWebsiteKeysQuery, Result<List<WebsiteKey>>>
    {
        private readonly ILocalizationService _localizer;
        private readonly ILogger<GetWebsiteKeysQueryHandler> _logger;
        public GetEnWebsiteKeysQueryHandler(ILogger<GetWebsiteKeysQueryHandler> logger, ILocalizationService localizer)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger)); ;
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer)); ;
        }
        public async Task<Result<List<WebsiteKey>>> Handle(GetEnWebsiteKeysQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var WebsiteKeys = await WebsiteKeyHelper.LoadEnWebsiteKeysAsync ( );
                return Result<List<WebsiteKey>>.Success (WebsiteKeys);
            }
            catch (Exception ex)
            {
                _logger.LogError (ex, "An unexpected error occurred while getting WebsiteKey data.");
                return Result<List<WebsiteKey>>.Failure (_localizer.Format (LocalizationKey.UnexpectedError), ex);
            }

        }
    }

}
