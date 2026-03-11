using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentInfo.Queries
{
    public class GetRecruitmentInfoByIdQuery : IRequest<Result>
    {
        public required string Id { get; set; }
    }

    public class GetRecruitmentInfoByIdQueryHandler : IRequestHandler<GetRecruitmentInfoByIdQuery, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetRecruitmentInfoByIdQueryHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(GetRecruitmentInfoByIdQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse (request.Id, out var guid))
                return Result.Failure (_localizer.Format (LocalizationKey.InvalidIdFormat, _localizer["RecruitmentInfo"], request.Id));

            var entity = await _context.RecruitmentInfos
                .Include (x => x.Department)
                .AsNoTracking ( )
                .FirstOrDefaultAsync (x => x.Id == guid && !x.IsDeleted, cancellationToken);

            if (entity == null)
                return Result.Failure (_localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["RecruitmentInfo"], request.Id));

            var model = _mapper.Map<RecruitmentInfoModel> (entity);
            return Result.Success (model);
        }
    }
}