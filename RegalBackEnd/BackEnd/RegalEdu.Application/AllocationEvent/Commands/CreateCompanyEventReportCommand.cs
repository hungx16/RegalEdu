using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;


namespace RegalEdu.Application.AllocationEvent.Commands
{
    public class CreateCompanyEventReportCommand : IRequest<Result>
    {
        public required CompanyEventReportModel CompanyEventReportModel { get; set; }
    }

    public class CreateCompanyEventReportCommandHandler
        : IRequestHandler<CreateCompanyEventReportCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly IFileService _fileService;

        public CreateCompanyEventReportCommandHandler(
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
            CreateCompanyEventReportCommand request,
            CancellationToken cancellationToken)
        {
            if (request.CompanyEventReportModel.CompanyEventId == Guid.Empty)
            {
                return Result.Failure(_localizer.Format(LocalizationKey.InvalidIdFormat, EntityName.CompanyEvent));
            }

            using var tx = await _context.BeginTransactionAsync(cancellationToken);

            var entity = _mapper.Map<CompanyEventReport>(request.CompanyEventReportModel);

            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            await _context.CompanyEventReports.AddAsync(entity, cancellationToken);

            // Publications
            if (request.CompanyEventReportModel.EventPublications != null)
            {
                foreach (var pub in request.CompanyEventReportModel.EventPublications)
                {
                    await _context.EventPublications.AddAsync(new EventPublication
                    {
                        Id = Guid.NewGuid(),
                        CompanyEventId = null,
                        CompanyEventReportId = entity.Id,
                        ItemId = pub.ItemId,
                        Quantity = pub.Quantity,
                        PublicationAmount = pub.PublicationAmount,
                        TotalAmount = pub.TotalAmount
                    }, cancellationToken);
                }
            }

            // Cash Costs
            if (request.CompanyEventReportModel.EventCashes != null)
            {
                foreach (var cash in request.CompanyEventReportModel.EventCashes)
                {
                    await _context.EventCashes.AddAsync(new EventCash
                    {
                        Id = Guid.NewGuid(),
                        CompanyEventId = null,
                        CompanyEventReportId = entity.Id,
                        CashName = cash.CashName,
                        Quantity = cash.Quantity,
                        Amount = cash.Amount,
                        TotalAmount = cash.TotalAmount
                    }, cancellationToken);
                }
            }

            // Participants
            if (request.CompanyEventReportModel.EventParticipants != null)
            {
                foreach (var p in request.CompanyEventReportModel.EventParticipants)
                {
                    await _context.EventParticipants.AddAsync(new EventParticipant
                    {
                        Id = Guid.NewGuid(),
                        CompanyEventId = null,
                        CompanyEventReportId = entity.Id,
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

            // Attachments
            if (request.CompanyEventReportModel.Attachments != null)
            {
                foreach (var m in request.CompanyEventReportModel.Attachments)
                {
                    var finalPath = m.Path;
                    if (!string.IsNullOrWhiteSpace(finalPath) &&
                        finalPath.StartsWith("temp/", StringComparison.OrdinalIgnoreCase))
                    {
                        finalPath = await _fileService.MoveFileAsync(finalPath, "company-event-reports");
                    }

                    if (!string.IsNullOrWhiteSpace(finalPath))
                    {
                        await _context.Attachments.AddAsync(new Attachment
                        {
                            CompanyEventReportId = entity.Id,
                            Path = finalPath,
                            FileName = m.FileName ?? Path.GetFileName(finalPath)
                        }, cancellationToken);
                    }
                }
            }

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
            {
                await tx.CommitAsync(cancellationToken);
                return Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.CompanyEventReport));
            }

            await tx.RollbackAsync(cancellationToken);
            return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.CompanyEventReport));
        }
    }
}
