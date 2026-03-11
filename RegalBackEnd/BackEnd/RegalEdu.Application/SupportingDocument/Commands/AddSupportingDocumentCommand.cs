using AutoMapper;
using MediatR;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Shared;

namespace RegalEdu.Application.SupportingDocument.Commands
{
    public class AddSupportingDocumentCommand : IRequest<Result>
    {
        public required SupportingDocumentModel SupportingDocumentModel { get; set; }


        public class AddSupportingDocumentCommandHandler : IRequestHandler<AddSupportingDocumentCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;
            private readonly IFileService _fileService;

            public AddSupportingDocumentCommandHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer, IFileService fileService)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
                _fileService = fileService ?? throw new ArgumentNullException (nameof (fileService));
            }

            public async Task<Result> Handle(AddSupportingDocumentCommand request, CancellationToken cancellationToken)
            {
                // 2. Kiểm tra và xử lý file đính kèm nếu có
                var model = request.SupportingDocumentModel;
                try
                {
                    if (model.Image != null && !string.IsNullOrEmpty (model.Image.Path) && model.Image.Path.StartsWith ("temp/", StringComparison.OrdinalIgnoreCase))
                    {
                        model.Image.Path = await _fileService.MoveFileAsync (model.Image.Path, "images");
                    }

                }
                catch (Exception ex)
                {
                    var errorMessage = _localizer.Format (LocalizationKey.ERR_FILE_UPLOAD_FAILED, model?.Image?.Path, Functions.GetFullExceptionMessage (ex));
                    return Result.Failure (errorMessage);
                }
                try
                {
                    if (model.Attachment != null && !string.IsNullOrEmpty (model.Attachment.Path) && model.Attachment.Path.StartsWith ("temp/", StringComparison.OrdinalIgnoreCase))
                    {
                        model.Attachment.Path = await _fileService.MoveFileAsync (model.Attachment.Path, "supporting-documents");
                    }

                }
                catch (Exception ex)
                {
                    var errorMessage = _localizer.Format (LocalizationKey.ERR_FILE_UPLOAD_FAILED, model?.Attachment?.Path, Functions.GetFullExceptionMessage (ex));
                    return Result.Failure (errorMessage);
                }
                var entity = _mapper.Map<Domain.Entities.SupportingDocument> (request.SupportingDocumentModel);
                // 4. Gán đường dẫn file đã lưu vào entity

                await _context.SupportingDocuments.AddAsync (entity, cancellationToken);

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                await WebsiteKeyHelper.SaveWebsiteKeysAsync (model.ListWebsiteKeys);

                if (success)
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer["SupportingDocument"]));
                else
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["SupportingDocument"]));
            }
        }
    }
}
