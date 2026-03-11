using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;



namespace RegalEdu.Application.Common.Queries
{
    public class GetProvincesQuery : IRequest<Result<List<Province>>> { }


    public class GetProvincesQueryHandler : IRequestHandler<GetProvincesQuery, Result<List<Province>>>
    {
        private readonly ILocalizationService _localizer;
        private readonly ILogger<GetProvincesQueryHandler> _logger;
        public GetProvincesQueryHandler(ILogger<GetProvincesQueryHandler> logger, ILocalizationService localizer)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger)); ;
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer)); ;
        }
        public async Task<Result<List<Province>>> Handle(GetProvincesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var provinces = await ProvinceFileHelper.LoadProvincesAsync ( );
                return Result<List<Province>>.Success (provinces);
            }
            catch (Exception ex)
            {
                _logger.LogError (ex, "An unexpected error occurred while getting province data.");
                return Result<List<Province>>.Failure (_localizer.Format (LocalizationKey.UnexpectedError), ex);
            }

        }
    }

}
