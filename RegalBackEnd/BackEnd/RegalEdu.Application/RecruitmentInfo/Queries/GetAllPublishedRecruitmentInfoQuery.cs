using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentInfo.Queries
{
    public class GetAllPublishedRecruitmentInfoQuery : IRequest<Result<List<RecruitmentInfoModel>>> { }

    public class GetAllPublishedRecruitmentInfoQueryHandler : IRequestHandler<GetAllPublishedRecruitmentInfoQuery, Result<List<RecruitmentInfoModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetAllPublishedRecruitmentInfoQueryHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer)); ;
        }

        public async Task<Result<List<RecruitmentInfoModel>>> Handle(
    GetAllPublishedRecruitmentInfoQuery request,
    CancellationToken cancellationToken)
        {
            var lang = _localizer.GetCurrentLanguage ( );

            var list = await _context.RecruitmentInfos
                .Include (x => x.Department)
                .AsNoTracking ( )
                .Where (x => x.IsPublish == true
                            && x.Status == RegalEdu.Domain.Enums.StatusType.Active)
                .ToListAsync (cancellationToken);
            var provinces = await ProvinceFileHelper.LoadProvincesAsync ( );

            var mapped = _mapper.Map<List<RecruitmentInfoModel>> (list);
            foreach (var x in mapped)
            {
                // Set ProvinceCode to the localized province name
                x.ProvinceCode = provinces
                          .FirstOrDefault (p => p.ProvinceCode == x.ProvinceCode)?
                          .ProvinceName ?? x.ProvinceCode;
            }
            if (lang.Equals ("en", StringComparison.OrdinalIgnoreCase))
            {

                mapped = mapped
                    .Where (x => x.IsMultilingual) // only take records that support EN
                    .Select (x =>
                    {

                        x.RecruitmentInfoName = !string.IsNullOrWhiteSpace (x.EnRecruitmentInfoName)
                            ? x.EnRecruitmentInfoName : x.RecruitmentInfoName;

                        x.Description = !string.IsNullOrWhiteSpace (x.EnDescription)
                            ? x.EnDescription : x.Description;

                        x.Position = !string.IsNullOrWhiteSpace (x.EnPosition)
                            ? x.EnPosition : x.Position;

                        x.Experience = !string.IsNullOrWhiteSpace (x.EnExperience)
                            ? x.EnExperience : x.Experience;
                        if (x.Department != null && !string.IsNullOrWhiteSpace (x.Department.EnDepartmentName))
                        {
                            x.Department.DepartmentName = x.Department.EnDepartmentName;
                        }
                        if (provinces == null) return x;
                        // Set ProvinceCode to the English province name
                        x.ProvinceCode = provinces
                            .FirstOrDefault (p => p.ProvinceName == x.ProvinceCode)?
                            .EnProvinceName ?? x.ProvinceCode;
                        return x;
                    })
                    .ToList ( );
            }

            return Result<List<RecruitmentInfoModel>>.Success (mapped);
        }
    }
}
