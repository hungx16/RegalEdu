using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentInfo.Commands
{
    public class UpdateRecruitmentInfoCommand : IRequest<Result>
    {
        public required RecruitmentInfoModel RecruitmentInfoModel { get; set; }

        public class Handler : IRequestHandler<UpdateRecruitmentInfoCommand, Result>
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

            public async Task<Result> Handle(UpdateRecruitmentInfoCommand request, CancellationToken cancellationToken)
            {
                var id = request.RecruitmentInfoModel.Id;
                var entity = await _context.RecruitmentInfos.FirstOrDefaultAsync (x => x.Id == id, cancellationToken);

                if (entity == null)
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, _localizer["RecruitmentInfo"]));

                _mapper.Map (request.RecruitmentInfoModel, entity);

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                return success
                    ? Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["RecruitmentInfo"]))
                    : Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["RecruitmentInfo"]));
            }
        }
    }
}
