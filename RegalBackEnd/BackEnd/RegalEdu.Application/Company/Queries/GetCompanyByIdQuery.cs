using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Company.Queries
{
    public class GetCompanyByIdQuery : IRequest<Result<CompanyModel>>
    {
        public required string Id { get; set; }

        public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public GetCompanyByIdQueryHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result<CompanyModel>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
            {
                var company = await _context.Companies
                    .Include (x => x.Manager).ThenInclude (x => x.ApplicationUser)
                    .Include (x => x.LogRegionComs).ThenInclude (x => x.Region)
                    .AsNoTracking ( )
                    .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

                if (company == null)
                {
                    var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["Company"], request.Id);
                    return Result<CompanyModel>.Failure (msg);
                }

                var result = _mapper.Map<CompanyModel> (company);
                return Result<CompanyModel>.Success (result);
            }
        }
    }
}
