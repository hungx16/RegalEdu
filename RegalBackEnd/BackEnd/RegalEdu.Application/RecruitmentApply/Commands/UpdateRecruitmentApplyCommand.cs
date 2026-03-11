using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.RecruitmentApply.Commands
{
    public class UpdateRecruitmentApplyCommand : IRequest<Result>
    {
        public required RecruitmentApplyModel RecruitmentApplyModel { get; set; }

        public class Handler : IRequestHandler<UpdateRecruitmentApplyCommand, Result>
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

            public async Task<Result> Handle(UpdateRecruitmentApplyCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.RecruitmentApplies
                    .FirstOrDefaultAsync (x => x.Id == request.RecruitmentApplyModel.Id, cancellationToken);

                if (entity == null)
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, _localizer["RecruitmentApply"]));

                _mapper.Map (request.RecruitmentApplyModel, entity);

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                return success
                    ? Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["RecruitmentApply"]))
                    : Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["RecruitmentApply"]));
            }
        }
    }
}
