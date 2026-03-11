using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Request;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;


namespace RegalEdu.Application.Common.Queries
{
    public class GenerateCodeQuery : IRequest<Result<string>>
    {
        public required GenerateCodeRequest GenerateCodeRequest { get; set; }
    }

    public class GenerateCodeQueryHandler : IRequestHandler<GenerateCodeQuery, Result<string>>
    {

        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localizer;
        private readonly ILogger<GenerateCodeQueryHandler> _logger;
        public GenerateCodeQueryHandler(IRegalEducationDbContext dbContext, ILogger<GenerateCodeQueryHandler> logger, ILocalizationService localizer)
        {
            _context = dbContext ?? throw new ArgumentNullException (nameof (dbContext));
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(GenerateCodeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace (request.GenerateCodeRequest.Prefix) ||
                    string.IsNullOrWhiteSpace (request.GenerateCodeRequest.TableName) ||
                    string.IsNullOrWhiteSpace (request.GenerateCodeRequest.ColumnName))
                {
                    return Result<string>.Failure (_localizer[LocalizationKey.PrefixTableColumnRequired]);
                }

                var info = new AutoCodeInfo
                {
                    Prefix = request.GenerateCodeRequest.Prefix,
                    TableName = request.GenerateCodeRequest.TableName,
                    ColumnName = request.GenerateCodeRequest.ColumnName,
                    Length = request.GenerateCodeRequest.Length,
                    Format = request.GenerateCodeRequest.Format,
                    Year = request.GenerateCodeRequest.Year,
                    Month = request.GenerateCodeRequest.Month
                };

                var code = await AutoCodeHelper.GenerateCodeAsync (info, (Microsoft.EntityFrameworkCore.DbContext)_context);
                return Result<string>.Success (code);
            }
            catch (Exception ex)
            {
                _logger.LogError (ex, "An unexpected error occurred while generating code.");
                return Result<string>.Failure (_localizer.Format (LocalizationKey.UnexpectedError), ex);
            }
        }
    }
}
