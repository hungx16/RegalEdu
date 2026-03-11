using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.Request;

namespace RegalEdu.Application.AllocationEvent.Commands
{
    public class UpdateProposalCommand : IRequest<Result>
    {
        public CompanyEventProposalRequest CompanyEventProposalRequest { get; set; } = null!;
    }

    public class UpdateProposalCommandHandler : IRequestHandler<UpdateProposalCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localizer;
        private readonly IFileService _fileService;

        public UpdateProposalCommandHandler(
            IRegalEducationDbContext context,
            ILocalizationService localizer,
            IFileService fileService)
        {
            _context = context;
            _localizer = localizer;
            _fileService = fileService;
        }

        public async Task<Result> Handle(UpdateProposalCommand request, CancellationToken cancellationToken)
        {
            var model = request.CompanyEventProposalRequest.CompanyEvent;

            if (model.Id == null)
                return Result.Failure("Missing proposal ID");

            var entity = await _context.CompanyEvents
                .Include(e => e.EventPublications)
                .Include(e => e.EventCashes)
                .Include(e => e.EventParticipants)
                .Include(e => e.Attachments)
                .FirstOrDefaultAsync(e => e.Id == model.Id, cancellationToken);

            if (entity == null)
                return Result.Failure("Proposal not found");

            using var tx = await _context.BeginTransactionAsync();

            try
            {
                // --------- 1) Update main entity ----------
                entity.AllocationDetailEventId = model.AllocationDetailEventId;
                entity.CompanyEventCode = model.CompanyEventCode;
                entity.CompanyEventName = model.CompanyEventName;
                entity.EventDate = model.EventDate;
                entity.AffiliatePartnerId = model.AffiliatePartnerId;
                entity.NumberStudents = model.NumberStudents;
                entity.Propose = model.Propose;
                entity.TotalAmount = model.TotalAmount ?? 0;
                entity.EventSize = model.EventSize;
                entity.CompanyEventStatus = model.CompanyEventStatus;

                _context.CompanyEvents.Update(entity);

                var req = request.CompanyEventProposalRequest;

                // =========================================================
                // 2) Publications (EventPublications) – delete + upsert
                // =========================================================
                var deletedPublicationIds = (req.DeletedPublicationIds ?? new List<string>())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToHashSet();

                // 2.1 Xoá các publication trong danh sách deletedPublicationIds
                if (deletedPublicationIds.Count > 0 && entity.EventPublications?.Any() == true)
                {
                    var toRemove = entity.EventPublications
                        .Where(p => deletedPublicationIds.Contains(p.Id.ToString()))
                        .ToList();

                    if (toRemove.Count > 0)
                    {
                        _context.EventPublications.RemoveRange(toRemove);
                        // đồng bộ lại collection trên entity để tránh upsert nhầm
                        foreach (var r in toRemove)
                            entity.EventPublications.Remove(r);
                    }
                }

                // 2.2 Upsert các publication còn lại
                var incomingPubs = req.Publications ?? new List<EventPublicationModel>();
                var existingPubsById = entity.EventPublications?
                    .ToDictionary(x => x.Id.ToString())
                    ?? new Dictionary<string, EventPublication>();

                foreach (var pub in incomingPubs)
                {
                    var hasId = pub.Id != null && pub.Id != Guid.Empty;
                    if (!hasId)
                    {
                        // Thêm mới
                        var newPub = new EventPublication
                        {
                            Id = Guid.NewGuid(),
                            CompanyEventId = entity.Id,
                            CompanyEventReportId = null,
                            ItemId = pub.ItemId,
                            Quantity = pub.Quantity,
                            PublicationAmount = pub.PublicationAmount,
                            TotalAmount = pub.TotalAmount
                        };
                        await _context.EventPublications.AddAsync(newPub, cancellationToken);
                    }
                    else
                    {
                        var key = pub.Id!.Value.ToString();
                        if (existingPubsById.TryGetValue(key, out var exist))
                        {
                            // Cập nhật bản ghi cũ
                            exist.ItemId = pub.ItemId;
                            exist.Quantity = pub.Quantity;
                            exist.PublicationAmount = pub.PublicationAmount;
                            exist.TotalAmount = pub.TotalAmount;
                            _context.EventPublications.Update(exist);
                        }
                        else
                        {
                            // Phòng trường hợp FE gửi Id mà DB không có -> thêm mới
                            var newPub = new EventPublication
                            {
                                Id = pub.Id!.Value,
                                CompanyEventId = entity.Id,
                                CompanyEventReportId = null,
                                ItemId = pub.ItemId,
                                Quantity = pub.Quantity,
                                PublicationAmount = pub.PublicationAmount,
                                TotalAmount = pub.TotalAmount
                            };
                            await _context.EventPublications.AddAsync(newPub, cancellationToken);
                        }
                    }
                }

                // =========================================================
                // 3) CashCosts (EventCashes) – delete + upsert
                // =========================================================
                var deletedCashIds = (req.DeletedCashIds ?? new List<string>())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToHashSet();

                if (deletedCashIds.Count > 0 && entity.EventCashes?.Any() == true)
                {
                    var toRemove = entity.EventCashes
                        .Where(c => deletedCashIds.Contains(c.Id.ToString()))
                        .ToList();

                    if (toRemove.Count > 0)
                    {
                        _context.EventCashes.RemoveRange(toRemove);
                        foreach (var r in toRemove)
                            entity.EventCashes.Remove(r);
                    }
                }

                var incomingCash = req.CashCosts ?? new List<EventCashModel>();
                var existingCashById = entity.EventCashes?
                    .ToDictionary(x => x.Id.ToString())
                    ?? new Dictionary<string, EventCash>();

                foreach (var cash in incomingCash)
                {
                    var hasId = cash.Id != null && cash.Id != Guid.Empty;
                    if (!hasId)
                    {
                        var newCash = new EventCash
                        {
                            Id = Guid.NewGuid(),
                            CompanyEventId = entity.Id,
                            CompanyEventReportId = null,
                            CashName = cash.CashName,
                            Quantity = cash.Quantity,
                            Amount = cash.Amount,
                            TotalAmount = cash.TotalAmount
                        };
                        await _context.EventCashes.AddAsync(newCash, cancellationToken);
                    }
                    else
                    {
                        var key = cash.Id!.Value.ToString();
                        if (existingCashById.TryGetValue(key, out var exist))
                        {
                            exist.CashName = cash.CashName;
                            exist.Quantity = cash.Quantity;
                            exist.Amount = cash.Amount;
                            exist.TotalAmount = cash.TotalAmount;
                            _context.EventCashes.Update(exist);
                        }
                        else
                        {
                            var newCash = new EventCash
                            {
                                Id = cash.Id!.Value,
                                CompanyEventId = entity.Id,
                                CompanyEventReportId = null,
                                CashName = cash.CashName,
                                Quantity = cash.Quantity,
                                Amount = cash.Amount,
                                TotalAmount = cash.TotalAmount
                            };
                            await _context.EventCashes.AddAsync(newCash, cancellationToken);
                        }
                    }
                }

                // =========================================================
                // 4) Participants – delete + upsert
                // =========================================================
                var deletedParticipantIds = (req.DeletedParticipantIds ?? new List<string>())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToHashSet();

                if (deletedParticipantIds.Count > 0 && entity.EventParticipants?.Any() == true)
                {
                    var toRemove = entity.EventParticipants
                        .Where(p => deletedParticipantIds.Contains(p.Id.ToString()))
                        .ToList();

                    if (toRemove.Count > 0)
                    {
                        _context.EventParticipants.RemoveRange(toRemove);
                        foreach (var r in toRemove)
                            entity.EventParticipants.Remove(r);
                    }
                }

                var incomingParticipants = req.Participants ?? new List<EventParticipantModel>();
                var existingParticipantsById = entity.EventParticipants?
                    .ToDictionary(x => x.Id.ToString())
                    ?? new Dictionary<string, EventParticipant>();

                foreach (var p in incomingParticipants)
                {
                    var hasId = p.Id != null && p.Id != Guid.Empty;
                    if (!hasId)
                    {
                        var newP = new EventParticipant
                        {
                            Id = Guid.NewGuid(),
                            CompanyEventId = entity.Id,
                            CompanyEventReportId = null,
                            IsStudent = p.IsStudent,
                            StudentCode = p.StudentCode,
                            ParticipantName = p.ParticipantName,
                            ParticipantGender = p.ParticipantGender,
                            ParticipantDateOfBirth = p.ParticipantDateOfBirth,
                            ParticipantAddress = p.ParticipantAddress,
                            ParticipantPhoneNumber = p.ParticipantPhoneNumber,
                            ParticipantContact = p.ParticipantContact,
                            ParticipantEmail = p.ParticipantEmail,
                            ParticipantSchool = p.ParticipantSchool,
                            ParticipantSourceKnown = p.ParticipantSourceKnown,
                            ParticipantJob = p.ParticipantJob,
                            EmployeeId = p.EmployeeId
                        };
                        await _context.EventParticipants.AddAsync(newP, cancellationToken);
                    }
                    else
                    {
                        var key = p.Id!.Value.ToString();
                        if (existingParticipantsById.TryGetValue(key, out var exist))
                        {
                            exist.IsStudent = p.IsStudent;
                            exist.StudentCode = p.StudentCode;
                            exist.ParticipantName = p.ParticipantName;
                            exist.ParticipantGender = p.ParticipantGender;
                            exist.ParticipantDateOfBirth = p.ParticipantDateOfBirth;
                            exist.ParticipantAddress = p.ParticipantAddress;
                            exist.ParticipantPhoneNumber = p.ParticipantPhoneNumber;
                            exist.ParticipantContact = p.ParticipantContact;
                            exist.ParticipantEmail = p.ParticipantEmail;
                            exist.ParticipantSchool = p.ParticipantSchool;
                            exist.ParticipantSourceKnown = p.ParticipantSourceKnown;
                            exist.ParticipantJob = p.ParticipantJob;
                            exist.EmployeeId = p.EmployeeId;
                            _context.EventParticipants.Update(exist);
                        }
                        else
                        {
                            var newP = new EventParticipant
                            {
                                Id = p.Id!.Value,
                                CompanyEventId = entity.Id,
                                CompanyEventReportId = null,
                                IsStudent = p.IsStudent,
                                StudentCode = p.StudentCode,
                                ParticipantName = p.ParticipantName,
                                ParticipantGender = p.ParticipantGender,
                                ParticipantDateOfBirth = p.ParticipantDateOfBirth,
                                ParticipantAddress = p.ParticipantAddress,
                                ParticipantPhoneNumber = p.ParticipantPhoneNumber,
                                ParticipantContact = p.ParticipantContact,
                                ParticipantEmail = p.ParticipantEmail,
                                ParticipantSchool = p.ParticipantSchool,
                                ParticipantSourceKnown = p.ParticipantSourceKnown,
                                ParticipantJob = p.ParticipantJob,
                                EmployeeId = p.EmployeeId
                            };
                            await _context.EventParticipants.AddAsync(newP, cancellationToken);
                        }
                    }
                }

                // =========================================================
                // 5) Attachments – giữ logic xoá + upsert như cũ
                // =========================================================
                var deletedAttachmentIds = (req.DeletedAttachmentIds ?? new List<string>())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToHashSet();

                if (deletedAttachmentIds.Count > 0 && entity.Attachments?.Any() == true)
                {
                    var toRemove = entity.Attachments
                        .Where(a => deletedAttachmentIds.Contains(a.Id.ToString()))
                        .ToList();

                    if (toRemove.Count > 0)
                    {
                        foreach (var att in toRemove)
                        {
                            if (!string.IsNullOrWhiteSpace(att.Path))
                            {
                                try { await _fileService.DeleteFileAsync(att.Path); } catch { /* ignore */ }
                            }
                        }
                        _context.Attachments.RemoveRange(toRemove);
                        foreach (var r in toRemove)
                            entity.Attachments.Remove(r);
                    }
                }

                var incomingAttachments = req.Attachments ?? new List<AttachmentModel>();
                var existingAttById = entity.Attachments?
                    .ToDictionary(x => x.Id.ToString())
                    ?? new Dictionary<string, Domain.Entities.Attachment>();

                foreach (var a in incomingAttachments)
                {
                    var isNew = !a.Id.HasValue || a.Id == Guid.Empty;
                    string? finalPath = a.Path;

                    if (!string.IsNullOrWhiteSpace(finalPath) &&
                        finalPath.StartsWith("temp/", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            finalPath = await _fileService.MoveFileAsync(finalPath, "company-events");
                        }
                        catch
                        {
                            return Result.Failure(_localizer["ERR_FILE_MOVE_FAILED"]);
                        }
                    }

                    if (isNew)
                    {
                        await _context.Attachments.AddAsync(new Domain.Entities.Attachment
                        {
                            CompanyEventId = entity.Id,
                            Path = finalPath ?? string.Empty,
                            FileName = a.FileName ?? System.IO.Path.GetFileName(finalPath ?? string.Empty)
                        }, cancellationToken);
                    }
                    else
                    {
                        var key = a.Id!.Value.ToString();
                        if (existingAttById.TryGetValue(key, out var exist))
                        {
                            exist.Path = finalPath ?? exist.Path;
                            exist.FileName = a.FileName ?? exist.FileName;
                            _context.Attachments.Update(exist);
                        }
                        else
                        {
                            await _context.Attachments.AddAsync(new Domain.Entities.Attachment
                            {
                                Id = a.Id!.Value,
                                CompanyEventId = entity.Id,
                                Path = finalPath ?? string.Empty,
                                FileName = a.FileName ?? System.IO.Path.GetFileName(finalPath ?? string.Empty)
                            }, cancellationToken);
                        }
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);

                return Result.Success(_localizer.Format("MSG_UPDATE_SUCCESS", "Đề xuất sự kiện"));
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(cancellationToken);
                return Result.Failure($"Update failed: {ex.Message}");
            }
        }
    }
}
