using AutoMapper;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentInfo.Commands
{
    public class AddRecruitmentInfoCommand : IRequest<Result>
    {
        public required RecruitmentInfoModel RecruitmentInfoModel { get; set; }

        public class Handler : IRequestHandler<AddRecruitmentInfoCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public Handler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
            {
                _context = context;
                _mapper = mapper;
                _localizer = localizer;
            }

            public async Task<Result> Handle(AddRecruitmentInfoCommand request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<Domain.Entities.RecruitmentInfo> (request.RecruitmentInfoModel);
                await _context.RecruitmentInfos.AddAsync (entity, cancellationToken);
                var success = await _context.SaveChangesAsync (cancellationToken) > 0;

                return success
                    ? Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer["RecruitmentInfo"]))
                    : Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["RecruitmentInfo"]));
            }
        }
    }
}
