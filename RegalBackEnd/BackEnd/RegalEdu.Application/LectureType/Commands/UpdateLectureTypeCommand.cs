using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Shared;

namespace RegalEdu.Application.LectureType.Commands
{
    public class UpdateLectureTypeCommand : IRequest<Result>
    {
        public required LectureTypeModel LectureTypeModel { get; set; }

        public class UpdateLectureTypeCommandHandler : IRequestHandler<UpdateLectureTypeCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;
            private readonly IFileService _fileService;

            public UpdateLectureTypeCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer,
                IFileService fileService)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
                _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            }

            public async Task<Result> Handle(UpdateLectureTypeCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.LectureTypes
                                           .FirstOrDefaultAsync(x => x.Id == request.LectureTypeModel.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, "LectureType"));

                var model = request.LectureTypeModel;

                try
                {
                    if (model.Attachment != null)
                    {
                        var incoming = model.Attachment.Path?.Trim();

                        if (string.IsNullOrEmpty(incoming))
                        {
                            // User xoá ảnh
                            if (!string.IsNullOrWhiteSpace(entity.Attachment?.Path))
                            {
                                try { await _fileService.DeleteFileAsync(entity.Attachment.Path); } catch { /* ignore */ }
                            }
                            if (entity.Attachment != null)
                                _context.Remove(entity.Attachment);
                        }
                        else if (incoming.StartsWith("temp/", StringComparison.OrdinalIgnoreCase))
                        {
                            // Có ảnh mới từ temp -> move
                            var moved = await _fileService.MoveFileAsync(incoming, "lecture-types");
                            var attacthment = await _context.Attachments.FirstOrDefaultAsync(t => t.SupportingDocumentId == model.Id);

                            // Xoá ảnh cũ nếu có
                            if (attacthment != null && !string.IsNullOrWhiteSpace(attacthment.Path) &&
                                !string.Equals(attacthment.Path, moved, StringComparison.OrdinalIgnoreCase))
                            {
                                try { await _fileService.DeleteFileAsync(attacthment.Path); } catch { /* ignore */ }
                            }

                            if (attacthment != null)
                            {
                                _context.Remove(attacthment);
                                await _context.SaveChangesAsync();
                            }

                            var attachmentNew = new Attachment
                            {
                                Path = moved,
                                FileName = model.Attachment.FileName,
                                LectureTypeId = model.Id
                            };
                            _context.Attachments.Add(attachmentNew);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var err = _localizer.Format(
                        LocalizationKey.ERR_FILE_UPLOAD_FAILED,
                        model.Attachment?.Path ?? string.Empty,
                        Functions.GetFullExceptionMessage(ex)
                    );
                    return Result.Failure(err);
                }
                entity.LectureName = model.LectureName;
                entity.Description = model.Description;
                entity.Status = model.Status;
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (success)
                    return Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["LectureType"]));

                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["LectureType"]));
            }
        }
    }
}
