using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Common.Queries
{
    public class GetWardsQuery : IRequest<Result<List<Ward>>>
    {
        public string? ProvinceCode { get; set; }   // optional filter
    }

    public class GetWardsQueryHandler : IRequestHandler<GetWardsQuery, Result<List<Ward>>>
    {
        private readonly ILocalizationService _localizer;
        private readonly ILogger<GetWardsQueryHandler> _logger;

        public GetWardsQueryHandler(ILogger<GetWardsQueryHandler> logger, ILocalizationService localizer)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result<List<Ward>>> Handle(GetWardsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var wards = await WardFileHelper.LoadWardsAsync ( );

                if (!string.IsNullOrEmpty (request.ProvinceCode))
                {
                    wards = wards
                        .Where (w => w.ProvinceCode == request.ProvinceCode)
                        .ToList ( );
                }

                return Result<List<Ward>>.Success (wards);
            }
            catch (Exception ex)
            {
                _logger.LogError (ex, "An unexpected error occurred while getting ward data.");
                return Result<List<Ward>>.Failure (_localizer.Format (LocalizationKey.UnexpectedError), ex);
            }
        }
    }
}
