using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models;
using System.IO;

namespace RegalEdu.Application.AllocationEvent.Commands
{
    public class UpdateCompanyEventReportCommand : IRequest<Result>
    {
        public required CompanyEventReportModel CompanyEventReportModel { get; set; }
    }

    public class UpdateCompanyEventReportCommandHandler
        : IRequestHandler<UpdateCompanyEventReportCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localizer;
        private readonly IFileService _fileService;

        public UpdateCompanyEventReportCommandHandler(
            IRegalEducationDbContext context,
            ILocalizationService localizer,
            IFileService fileService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public async Task<Result> Handle(UpdateCompanyEventReportCommand request, CancellationToken cancellationToken)
        {
            if (request.CompanyEventReportModel.Id == null)
            {
                return Result.Failure(_localizer.Format(LocalizationKey.InvalidIdFormat, EntityName.CompanyEventReport));
            }

            if (request.CompanyEventReportModel.CompanyEventId == Guid.Empty)
            {
                return Result.Failure(_localizer.Format(LocalizationKey.InvalidIdFormat, EntityName.CompanyEvent));
            }

            var entity = await _context.CompanyEventReports
                .FirstOrDefaultAsync(r => r.Id == request.CompanyEventReportModel.Id, cancellationToken);

            if (entity == null)
            {
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, EntityName.CompanyEventReport));
            }

            using var tx = await _context.BeginTransactionAsync(cancellationToken);

            try
            {
                entity.CompanyEventId = request.CompanyEventReportModel.CompanyEventId;
                entity.EventDate = request.CompanyEventReportModel.EventDate;
                entity.NumberStudents = request.CompanyEventReportModel.NumberStudents;
                entity.CompanyEventReportCode = request.CompanyEventReportModel.CompanyEventReportCode;
                entity.TotalAmount = request.CompanyEventReportModel.TotalAmount;
                entity.CompanyEventStatus = request.CompanyEventReportModel.CompanyEventStatus;
                entity.LinkContent = request.CompanyEventReportModel.LinkContent;
                entity.LinkFanpage = request.CompanyEventReportModel.LinkFanpage;

                var reportId = entity.Id;

                // Publications - delete missing + upsert
                var existingPubs = await _context.EventPublications
                    .Where(p => p.CompanyEventReportId == reportId)
                    .ToListAsync(cancellationToken);
                var existingPubsById = existingPubs.ToDictionary(p => p.Id);
                var incomingPubs = request.CompanyEventReportModel.EventPublications ?? new List<EventPublicationModel>();
                var incomingPubIds = incomingPubs
                    .Where(p => p.Id.HasValue && p.Id != Guid.Empty)
                    .Select(p => p.Id!.Value)
                    .ToHashSet();
                var pubsToRemove = existingPubs.Where(p => !incomingPubIds.Contains(p.Id)).ToList();
                if (pubsToRemove.Count > 0)
                {
                    _context.EventPublications.RemoveRange(pubsToRemove);
                }

                foreach (var pub in incomingPubs)
                {
                    if (pub.Id.HasValue && existingPubsById.TryGetValue(pub.Id.Value, out var exist))
                    {
                        exist.CompanyEventReportId = reportId;
                        exist.CompanyEventId = null;
                        exist.ItemId = pub.ItemId;
                        exist.Quantity = pub.Quantity;
                        exist.PublicationAmount = pub.PublicationAmount;
                        exist.TotalAmount = pub.TotalAmount;
                        _context.EventPublications.Update(exist);
                    }
                    else
                    {
                        await _context.EventPublications.AddAsync(new EventPublication
                        {
                            Id = pub.Id ?? Guid.NewGuid(),
                            CompanyEventId = null,
                            CompanyEventReportId = reportId,
                            ItemId = pub.ItemId,
                            Quantity = pub.Quantity,
                            PublicationAmount = pub.PublicationAmount,
                            TotalAmount = pub.TotalAmount
                        }, cancellationToken);
                    }
                }

                // Cash costs - delete missing + upsert
                var existingCash = await _context.EventCashes
                    .Where(c => c.CompanyEventReportId == reportId)
                    .ToListAsync(cancellationToken);
                var existingCashById = existingCash.ToDictionary(c => c.Id);
                var incomingCash = request.CompanyEventReportModel.EventCashes ?? new List<EventCashModel>();
                var incomingCashIds = incomingCash
                    .Where(c => c.Id.HasValue && c.Id != Guid.Empty)
                    .Select(c => c.Id!.Value)
                    .ToHashSet();
                var cashToRemove = existingCash.Where(c => !incomingCashIds.Contains(c.Id)).ToList();
                if (cashToRemove.Count > 0)
                {
                    _context.EventCashes.RemoveRange(cashToRemove);
                }

                foreach (var cash in incomingCash)
                {
                    if (cash.Id.HasValue && existingCashById.TryGetValue(cash.Id.Value, out var exist))
                    {
                        exist.CompanyEventReportId = reportId;
                        exist.CompanyEventId = null;
                        exist.CashName = cash.CashName;
                        exist.Quantity = cash.Quantity;
                        exist.Amount = cash.Amount;
                        exist.TotalAmount = cash.TotalAmount;
                        _context.EventCashes.Update(exist);
                    }
                    else
                    {
                        await _context.EventCashes.AddAsync(new EventCash
                        {
                            Id = cash.Id ?? Guid.NewGuid(),
                            CompanyEventId = null,
                            CompanyEventReportId = reportId,
                            CashName = cash.CashName,
                            Quantity = cash.Quantity,
                            Amount = cash.Amount,
                            TotalAmount = cash.TotalAmount
                        }, cancellationToken);
                    }
                }

                // Participants - delete missing + upsert
                var existingParticipants = await _context.EventParticipants
                    .Where(p => p.CompanyEventReportId == reportId)
                    .ToListAsync(cancellationToken);
                var existingParticipantsById = existingParticipants.ToDictionary(p => p.Id);
                var incomingParticipants = request.CompanyEventReportModel.EventParticipants ?? new List<EventParticipantModel>();
                var incomingParticipantIds = incomingParticipants
                    .Where(p => p.Id.HasValue && p.Id != Guid.Empty)
                    .Select(p => p.Id!.Value)
                    .ToHashSet();
                var participantsToRemove = existingParticipants.Where(p => !incomingParticipantIds.Contains(p.Id)).ToList();
                if (participantsToRemove.Count > 0)
                {
                    _context.EventParticipants.RemoveRange(participantsToRemove);
                }

                foreach (var p in incomingParticipants)
                {
                    if (p.Id.HasValue && existingParticipantsById.TryGetValue(p.Id.Value, out var exist))
                    {
                        exist.CompanyEventReportId = reportId;
                        exist.CompanyEventId = null;
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
                        await _context.EventParticipants.AddAsync(new EventParticipant
                        {
                            Id = p.Id ?? Guid.NewGuid(),
                            CompanyEventId = null,
                            CompanyEventReportId = reportId,
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
                        }, cancellationToken);
                    }
                }

                // Attachments - delete missing + upsert
                var existingAttachments = await _context.Attachments
                    .Where(a => a.CompanyEventReportId == reportId)
                    .ToListAsync(cancellationToken);
                var existingAttachmentsById = existingAttachments.ToDictionary(a => a.Id);
                var incomingAttachments = request.CompanyEventReportModel.Attachments ?? new List<AttachmentModel>();
                var incomingAttachmentIds = incomingAttachments
                    .Where(a => a.Id.HasValue && a.Id != Guid.Empty)
                    .Select(a => a.Id!.Value)
                    .ToHashSet();
                var attachmentsToRemove = existingAttachments.Where(a => !incomingAttachmentIds.Contains(a.Id)).ToList();
                if (attachmentsToRemove.Count > 0)
                {
                    foreach (var att in attachmentsToRemove)
                    {
                        if (!string.IsNullOrWhiteSpace(att.Path))
                        {
                            try { await _fileService.DeleteFileAsync(att.Path); } catch { }
                        }
                    }
                    _context.Attachments.RemoveRange(attachmentsToRemove);
                }

                foreach (var a in incomingAttachments)
                {
                    var finalPath = a.Path;
                    if (!string.IsNullOrWhiteSpace(finalPath) &&
                        finalPath.StartsWith("temp/", StringComparison.OrdinalIgnoreCase))
                    {
                        finalPath = await _fileService.MoveFileAsync(finalPath, "company-event-reports");
                    }

                    if (a.Id.HasValue && existingAttachmentsById.TryGetValue(a.Id.Value, out var exist))
                    {
                        if (!string.IsNullOrWhiteSpace(finalPath))
                        {
                            exist.Path = finalPath;
                            exist.FileName = a.FileName ?? Path.GetFileName(finalPath);
                        }
                        exist.CompanyEventReportId = reportId;
                        exist.CompanyEventId = null;
                        _context.Attachments.Update(exist);
                    }
                    else if (!string.IsNullOrWhiteSpace(finalPath))
                    {
                        await _context.Attachments.AddAsync(new Attachment
                        {
                            Id = a.Id ?? Guid.NewGuid(),
                            CompanyEventId = null,
                            CompanyEventReportId = reportId,
                            Path = finalPath,
                            FileName = a.FileName ?? Path.GetFileName(finalPath)
                        }, cancellationToken);
                    }
                }

                var success = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (success)
                {
                    await tx.CommitAsync(cancellationToken);
                    return Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.CompanyEventReport));
                }

                await tx.RollbackAsync(cancellationToken);
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.CompanyEventReport));
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(cancellationToken);
                return Result.Failure($"Update failed: {ex.Message}");
            }
        }
    }
}
