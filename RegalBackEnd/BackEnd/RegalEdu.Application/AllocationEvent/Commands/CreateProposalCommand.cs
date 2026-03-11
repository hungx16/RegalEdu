using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.Request;

namespace RegalEdu.Application.AllocationEvent.Commands
{
    // Command: Thêm AllocationEvent cùng AllocationDetailEvent
    public class CreateProposalCommand : IRequest<Result>
    {
        public required CompanyEventProposalRequest CompanyEventProposalRequest { get; set; }
    }

    // Handler: Xử lý logic thêm AllocationEvent + AllocationDetailEvent
    public class CreateProposalCommandHandler
        : IRequestHandler<CreateProposalCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly IFileService _fileService;

        public CreateProposalCommandHandler(
            IRegalEducationDbContext context,
            AutoMapper.IMapper mapper,
            ILocalizationService localizer,
            IFileService fileService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public async Task<Result> Handle(
            CreateProposalCommand request,
            CancellationToken cancellationToken)
        {
            using var tx = await _context.BeginTransactionAsync();

            try
            {

                var ce = new CompanyEvent
                {
                    Id = Guid.NewGuid(),
                    AllocationDetailEventId = request.CompanyEventProposalRequest.CompanyEvent.AllocationDetailEventId,
                    CompanyEventCode = request.CompanyEventProposalRequest.CompanyEvent.CompanyEventCode,
                    CompanyEventName = request.CompanyEventProposalRequest.CompanyEvent.CompanyEventName,
                    EventDate = request.CompanyEventProposalRequest.CompanyEvent.EventDate,
                    AffiliatePartnerId = request.CompanyEventProposalRequest.CompanyEvent.AffiliatePartnerId,
                    NumberStudents = request.CompanyEventProposalRequest.CompanyEvent.NumberStudents,
                    Propose = request.CompanyEventProposalRequest.CompanyEvent.Propose,
                    TotalAmount = request.CompanyEventProposalRequest.CompanyEvent.TotalAmount ?? 0,
                    EventSize = request.CompanyEventProposalRequest.CompanyEvent.EventSize,
                    CompanyEventStatus = request.CompanyEventProposalRequest.CompanyEvent.CompanyEventStatus
                };

                await _context.CompanyEvents.AddAsync(ce);

                // Publications
                if (request.CompanyEventProposalRequest.Publications != null)
                {
                    foreach (var pub in request.CompanyEventProposalRequest.Publications)
                    {
                        await _context.EventPublications.AddAsync(new EventPublication
                        {
                            Id = Guid.NewGuid(),
                            CompanyEventId = ce.Id,
                            CompanyEventReportId = null,
                            ItemId = pub.ItemId,
                            Quantity = pub.Quantity,
                            PublicationAmount = pub.PublicationAmount,
                            TotalAmount = pub.TotalAmount
                        });
                    }
                }

                // Cash Costs
                if (request.CompanyEventProposalRequest.CashCosts != null)
                {
                    foreach (var cash in request.CompanyEventProposalRequest.CashCosts)
                    {
                        await _context.EventCashes.AddAsync(new EventCash
                        {
                            Id = Guid.NewGuid(),
                            CompanyEventId = ce.Id,
                            CompanyEventReportId = null,
                            CashName = cash.CashName,
                            Quantity = cash.Quantity,
                            Amount = cash.Amount,
                            TotalAmount = cash.TotalAmount
                        });
                    }
                }

                // Participants
                if (request.CompanyEventProposalRequest.Participants != null)
                {
                    foreach (var p in request.CompanyEventProposalRequest.Participants)
                    {
                        await _context.EventParticipants.AddAsync(new EventParticipant
                        {
                            Id = Guid.NewGuid(),
                            CompanyEventId = ce.Id,
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
                        });
                    }
                }
                // Attachments
                if (request.CompanyEventProposalRequest.Attachments != null)
                {
                    var attactments = new List<Domain.Entities.Attachment>();
                    var attactmentModels = request.CompanyEventProposalRequest.Attachments ?? new List<AttachmentModel>();

                    foreach (var m in attactmentModels)
                    {
                        string finalUrl = m.Path ?? string.Empty;

                        if (!string.IsNullOrWhiteSpace(finalUrl) && finalUrl.StartsWith("temp/", StringComparison.OrdinalIgnoreCase))
                        {
                            finalUrl = await _fileService.MoveFileAsync(finalUrl, "company-events");
                        }

                        if (!string.IsNullOrWhiteSpace(finalUrl))
                        {
                            await _context.Attachments.AddAsync(new Attachment
                            {
                                //Id = Guid.NewGuid ( ),
                                CompanyEventId = ce.Id,
                                Path = finalUrl,
                                FileName = Path.GetFileName(finalUrl) // <-- Fix: set required FileName property
                            });
                        }
                    }

                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Result.Success(
                    _localizer.Format(
                        LocalizationKey.MSG_CREATE_SUCCESS,
                        $"{EntityName.ProposedAllocation}"
                    )
                );
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(cancellationToken);
                return Result.Failure($"Error: {ex.Message}");
            }
        }
    }
}
