using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentApply.Queries
{
    public class GetRecruitmentApplyByIdQuery : IRequest<Result<RecruitmentApplyModel>>
    {
        public required string Id { get; set; }

        public class Handler : IRequestHandler<GetRecruitmentApplyByIdQuery, Result<RecruitmentApplyModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public Handler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result<RecruitmentApplyModel>> Handle(GetRecruitmentApplyByIdQuery request, CancellationToken cancellationToken)
            {
                if (!Guid.TryParse (request.Id, out var guid))
                    return Result<RecruitmentApplyModel>.Failure (_localizer.Format (LocalizationKey.InvalidIdFormat, _localizer["RecruitmentApply"], request.Id));

                var entity = await _context.RecruitmentApplies
                    .Include (x => x.RecruitmentInfo)
                    .Include (x => x.Attachment)
                    .AsNoTracking ( )
                    .FirstOrDefaultAsync (x => x.Id == guid && !x.IsDeleted, cancellationToken);

                if (entity == null)
                    return Result<RecruitmentApplyModel>.Failure (_localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["RecruitmentApply"], request.Id));

                var model = _mapper.Map<RecruitmentApplyModel> (entity);
                return Result<RecruitmentApplyModel>.Success (model);
            }
        }
    }
}
