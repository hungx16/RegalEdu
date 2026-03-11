using AutoMapper;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Shared;

namespace RegalEdu.Application.RecruitmentApply.Commands
{
    public class AddRecruitmentApplyCommand : IRequest<Result>
    {
        public required RecruitmentApplyModel RecruitmentApplyModel { get; set; }

        public class Handler : IRequestHandler<AddRecruitmentApplyCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;
            private readonly IFileService _fileService;

            public Handler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer, IFileService fileService)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
                _fileService = fileService ?? throw new ArgumentNullException (nameof (fileService));
            }

            public async Task<Result> Handle(AddRecruitmentApplyCommand request, CancellationToken cancellationToken)
            {
                var model = request.RecruitmentApplyModel;
                try
                {
                    if (model.Attachment != null && !string.IsNullOrEmpty (model.Attachment.Path) && model.Attachment.Path.StartsWith ("temp/", StringComparison.OrdinalIgnoreCase))
                    {
                        model.Attachment.Path = await _fileService.MoveFileAsync (model.Attachment.Path, "recruitment-applies");
                    }

                }
                catch (Exception ex)
                {
                    var errorMessage = _localizer.Format (LocalizationKey.ERR_FILE_UPLOAD_FAILED, model?.Attachment?.Path, Functions.GetFullExceptionMessage (ex));
                    return Result.Failure (errorMessage);
                }
                var entity = _mapper.Map<RegalEdu.Domain.Entities.RecruitmentApply> (request.RecruitmentApplyModel);
                await _context.RecruitmentApplies.AddAsync (entity, cancellationToken);

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                return success
                    ? Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer["RecruitmentApply"]))
                    : Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["RecruitmentApply"]));
            }
        }
    }
}
