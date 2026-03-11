using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Shared;

namespace RegalEdu.Application.SupportingDocument.Commands
{
    public class UpdateSupportingDocumentCommand : IRequest<Result>
    {
        public required SupportingDocumentModel SupportingDocumentModel { get; set; }

        public class UpdateSupportingDocumentCommandHandler : IRequestHandler<UpdateSupportingDocumentCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;
            private readonly IFileService _fileService;

            public UpdateSupportingDocumentCommandHandler(
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

            public async Task<Result> Handle(UpdateSupportingDocumentCommand request, CancellationToken cancellationToken)
            {
                var model = request.SupportingDocumentModel;

                var entity = await _context.SupportingDocuments
                    .FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken);

                if (entity == null)
                    return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, "SupportingDocument"));

                try
                {
                    if (model.Image != null)
                    {
                        var incoming = model.Image.Path?.Trim();

                        if (string.IsNullOrEmpty(incoming))
                        {
                            // User xoá ảnh
                            if (!string.IsNullOrWhiteSpace(entity.Image?.Path))
                            {
                                try { await _fileService.DeleteFileAsync(entity.Image.Path); } catch { /* ignore */ }
                            }
                            if (entity.Image != null)
                                _context.Remove(entity.Image);
                        }
                        else if (incoming.StartsWith("temp/", StringComparison.OrdinalIgnoreCase))
                        {
                            // Có ảnh mới từ temp -> move
                            var moved = await _fileService.MoveFileAsync(incoming, "images");
                            var image = await _context.Images.FirstOrDefaultAsync(t => t.SupportingDocumentId == model.Id);

                            // Xoá ảnh cũ nếu có
                            if (image != null && !string.IsNullOrWhiteSpace(image.Path) &&
                                !string.Equals(image.Path, moved, StringComparison.OrdinalIgnoreCase))
                            {
                                try { await _fileService.DeleteFileAsync(image.Path); } catch { /* ignore */ }
                            }

                            if (image != null)
                            {
                                _context.Remove(image);
                                await _context.SaveChangesAsync();
                            }

                            var imageNew = new Image
                            {
                                Path = moved,
                                FileName = model.Image.FileName,
                                SupportingDocumentId = model.Id
                            };
                            _context.Images.Add(imageNew);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var err = _localizer.Format(
                        LocalizationKey.ERR_FILE_UPLOAD_FAILED,
                        model.Image?.Path ?? string.Empty,
                        Functions.GetFullExceptionMessage(ex)
                    );
                    return Result.Failure(err);
                }
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
                            var moved = await _fileService.MoveFileAsync(incoming, "supporting-documents");
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
                                SupportingDocumentId = model.Id
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
                entity.DocumentName = model.DocumentName;
                entity.EnDocumentName = model.EnDocumentName;
                entity.Description = model.Description;
                entity.EnDescription = model.EnDescription;
                entity.DocumentTypeId = model.DocumentTypeId;
                entity.WebsiteKeys = model.WebsiteKeys;
                entity.EnWebsiteKeys = model.EnWebsiteKeys;
                entity.StartDate = model.StartDate;
                entity.EndDate = model.EndDate;
                entity.AuthorName = model.AuthorName;
                entity.EnAuthorName = model.EnAuthorName;
                entity.IsPublish = model.IsPublish;
                entity.Format = model.Format;
                entity.Topic = model.Topic;
                entity.YearRelease = model.YearRelease;
                entity.IsMultilingual = model.IsMultilingual;
                entity.Level = model.Level;
                entity.Status = model.Status;
                entity.Link = model.Link;
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;

                // Lưu WebsiteKeys (nếu có)
                await WebsiteKeyHelper.SaveWebsiteKeysAsync(model.ListWebsiteKeys);
                await WebsiteKeyHelper.SaveEnWebsiteKeysAsync(model.ListEnWebsiteKeys);
                if (success)
                    return Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["SupportingDocument"]));
                else
                    return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["SupportingDocument"]));
            }
        }
    }
}
