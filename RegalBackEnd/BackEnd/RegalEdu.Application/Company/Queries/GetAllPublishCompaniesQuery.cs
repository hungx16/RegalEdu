using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Company.Queries
{
    public class GetAllPublishCompaniesQuery : IRequest<Result<List<CompanyModel>>> { }

    public class Handler_GetPublishAll : IRequestHandler<GetAllPublishCompaniesQuery, Result<List<CompanyModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public Handler_GetPublishAll(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result<List<CompanyModel>>> Handle(GetAllPublishCompaniesQuery request, CancellationToken cancellationToken)
        {
            var lang = _localizer.GetCurrentLanguage ( );
            var list = await _context.Companies.Include (x => x.CompanyLearningRoadMaps).Include (t => t.CompanyImages).Where (t => t.IsPublish == true && t.Status == Domain.Enums.StatusType.Active)
                .OrderBy (x => x.CreatedAt)
                .AsNoTracking ( )
                .ToListAsync (cancellationToken);
            var mapped = _mapper.Map<List<CompanyModel>> (list);
            if (lang.Equals ("en", StringComparison.OrdinalIgnoreCase))
            {
                // chỉ lấy những bản ghi hỗ trợ song ngữ
                mapped = mapped
                    .Where (item => item.IsMultilingual)
                    .Select (item =>
                    {
                        item.CompanyName = !string.IsNullOrWhiteSpace (item.EnCompanyName) ? item.EnCompanyName : item.CompanyName;
                        item.CompanyAddress = !string.IsNullOrWhiteSpace (item.EnCompanyAddress) ? item.EnCompanyAddress : item.CompanyAddress;
                        item.Convenience = !string.IsNullOrWhiteSpace (item.EnConvenience) ? item.EnConvenience : item.Convenience;
                        item.LeaningRoadMapTags = !string.IsNullOrWhiteSpace (item.EnLeaningRoadMapTags) ? item.EnLeaningRoadMapTags : item.LeaningRoadMapTags;
                        item.Description = !string.IsNullOrWhiteSpace (item.EnDescription) ? item.EnDescription : item.Description;
                        return item;
                    })
                    .ToList ( );
            }
            return Result<List<CompanyModel>>.Success (mapped);
        }
    }
}