using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Company.Commands
{
    public class AddCompanyCommand : IRequest<Result>
    {
        public required CompanyModel CompanyModel { get; set; }

        public class AddCompanyCommandHandler : IRequestHandler<AddCompanyCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;
            private readonly IFileService _fileService;
            private readonly ILogger<AddCompanyCommandHandler> _logger;

            public AddCompanyCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer,
                IFileService fileService,
                ILogger<AddCompanyCommandHandler> logger)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
                _fileService = fileService ?? throw new ArgumentNullException (nameof (fileService));
                _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            }

            public async Task<Result> Handle(AddCompanyCommand request, CancellationToken cancellationToken)
            {
                var info = AutoCodeConfig.Get (AutoCodeType.Company);
                if (_context is not DbContext dbContext)
                    throw new InvalidOperationException (_localizer[LocalizationKey.InvalidDbContextInstance]);

                var result = await AutoCodeHelper.CreateWithAutoCodeRetryAsync (
                    info,
                    async (code) =>
                    {
                        var company = _mapper.Map<Domain.Entities.Company> (request.CompanyModel);
                        company.CompanyCode = code;
                        //if (company.Id == Guid.Empty) company.Id = Guid.NewGuid ( );

                        // Xử lý ảnh: chỉ dựa vào ImageUrl (đã upload tạm từ FE)
                        var images = new List<Domain.Entities.Image> ( );
                        var imgModels = request.CompanyModel.CompanyImages ?? new List<ImageDto> ( );

                        foreach (var m in imgModels.OrderBy (x => x.SortOrder))
                        {
                            string finalUrl = m.Path ?? string.Empty;

                            if (!string.IsNullOrWhiteSpace (finalUrl) && finalUrl.StartsWith ("temp/", StringComparison.OrdinalIgnoreCase))
                            {
                                finalUrl = await _fileService.MoveFileAsync (finalUrl, "images");
                            }

                            if (!string.IsNullOrWhiteSpace (finalUrl))
                            {
                                images.Add (new Domain.Entities.Image
                                {
                                    //Id = Guid.NewGuid ( ),
                                    CompanyId = company.Id,
                                    Path = finalUrl,
                                    Caption = m.Caption,
                                    SortOrder = m.SortOrder,
                                    IsCover = m.IsCover,
                                    Status = request.CompanyModel.Status
                                });
                            }
                        }

                        if (images.Count > 0)
                        {
                            if (!images.Any (i => i.IsCover)) images[0].IsCover = true;
                            company.CompanyImages = images;
                        }

                        await _context.Companies.AddAsync (company, cancellationToken);
                        var success = await _context.SaveChangesAsync (cancellationToken) > 0;

                        return success
                            ? Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Company))
                            : Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Company));
                    },
                    dbContext
                );
                return result;
            }

        }
    }
}
