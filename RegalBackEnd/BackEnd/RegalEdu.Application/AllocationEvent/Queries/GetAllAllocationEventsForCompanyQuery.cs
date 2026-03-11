using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

public class GetAllAllocationEventsForCompanyQuery : IRequest<Result<List<AllocationEventModel>>> { }

public class GetAllAllocationEventsForCompanyQueryHandler
    : IRequestHandler<GetAllAllocationEventsForCompanyQuery, Result<List<AllocationEventModel>>>
{
    private readonly IRegalEducationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetAllAllocationEventsForCompanyQueryHandler(
        IRegalEducationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Result<List<AllocationEventModel>>> Handle(
        GetAllAllocationEventsForCompanyQuery request,
        CancellationToken cancellationToken)
    {
        // Lấy employeeId từ user hiện tại
        if (!Guid.TryParse(_currentUser.EmployeeId, out var employeeId))
            return Result<List<AllocationEventModel>>.Success(new List<AllocationEventModel>());

        var allocations = await _context.AllocationEvents
            .Where(a => !a.IsDeleted &&
                        a.AllocationEventStatus != RegalEdu.Domain.Enums.AllocationEventStatus.Draft &&
                        a.AllocationEventStatus != RegalEdu.Domain.Enums.AllocationEventStatus.Cancelled)
            .Where(a => a.AllocationDetails.Any(d =>
                !d.IsDeleted &&
                d.Company != null &&
                d.Company.ManagerId == employeeId))
            .Select(a => new AllocationEventModel
            {
                Id = a.Id,
                AllocationCode = a.AllocationCode,
                AllocationMonth = a.AllocationMonth,
                AllocationYear = a.AllocationYear,
                EventBudget = a.EventBudget,
                AllocationEventStatus = a.AllocationEventStatus,

                AllocationDetails = a.AllocationDetails
                    .Where(d => !d.IsDeleted &&
                                d.Company != null &&
                                d.Company.ManagerId == employeeId)
                    .Select(d => new AllocationDetailEventModel
                    {
                        Id = d.Id,
                        AllocationEventId = d.AllocationEventId,
                        CompanyId = d.CompanyId,
                        RegionId = d.RegionId,
                        EventId = d.EventId,
                        Quantity = d.Quantity,
                        Budget = d.Budget,
                        NoAllocation = d.NoAllocation,
                        Status = d.Status,

                        // ====== LOAD THÊM COMPANY EVENTS CHO MỖI DETAIL ======
                        CompanyEvents = d.CompanyEvents!
                            .Select(ce => new CompanyEventModel
                            {
                                Id = ce.Id,
                                AllocationDetailEventId = ce.AllocationDetailEventId,
                                CompanyEventCode = ce.CompanyEventCode,
                                CompanyEventName = ce.CompanyEventName,
                                EventDate = ce.EventDate,
                                AffiliatePartnerId = ce.AffiliatePartnerId,
                                NumberStudents = ce.NumberStudents,
                                Propose = ce.Propose,
                                TotalAmount = ce.TotalAmount,
                                EventSize = ce.EventSize,
                                CompanyEventStatus = ce.CompanyEventStatus,

                                // Nếu CompanyEventModel có các list con này thì map luôn
                                EventCashes = ce.EventCashes!
                                    .Where(ec => !ec.IsDeleted)
                                    .Select(ec => new EventCashModel
                                    {
                                        Id = ec.Id,
                                        CompanyEventId = ec.CompanyEventId,
                                        CashName = ec.CashName,
                                        Quantity = ec.Quantity,
                                        Amount = ec.Amount,
                                        TotalAmount = ec.TotalAmount
                                    }).ToList(),

                                EventPublications = ce.EventPublications!
                                    .Where(ep => !ep.IsDeleted)
                                    .Select(ep => new EventPublicationModel
                                    {
                                        Id = ep.Id,
                                        CompanyEventId = ep.CompanyEventId,
                                        ItemId = ep.ItemId,
                                        Quantity = ep.Quantity,
                                        PublicationAmount = ep.PublicationAmount,
                                        TotalAmount = ep.TotalAmount
                                    }).ToList(),

                                EventParticipants = ce.EventParticipants!
                                    .Where(ep => !ep.IsDeleted)
                                    .Select(ep => new EventParticipantModel
                                    {
                                        Id = ep.Id,
                                        CompanyEventId = ep.CompanyEventId,
                                        IsStudent = ep.IsStudent,
                                        StudentCode = ep.StudentCode,
                                        ParticipantName = ep.ParticipantName,
                                        ParticipantGender = ep.ParticipantGender,
                                        ParticipantDateOfBirth = ep.ParticipantDateOfBirth,
                                        ParticipantAddress = ep.ParticipantAddress,
                                        ParticipantPhoneNumber = ep.ParticipantPhoneNumber,
                                        ParticipantContact = ep.ParticipantContact,
                                        ParticipantEmail = ep.ParticipantEmail,
                                        ParticipantSchool = ep.ParticipantSchool,
                                        ParticipantSourceKnown = ep.ParticipantSourceKnown,
                                        ParticipantJob = ep.ParticipantJob,
                                        EmployeeId = ep.EmployeeId
                                    }).ToList(),

                                Attachments = ce.Attachments!
                                    .Where(att => !att.IsDeleted)
                                    .Select(att => new AttachmentModel
                                    {
                                        Id = att.Id,
                                        FileName = att.FileName,
                                        Path = att.Path,
                                        SupportingDocumentId = att.SupportingDocumentId,
                                        LectureTypeId = att.LectureTypeId,
                                        CompanyEventId = att.CompanyEventId
                                    }).ToList()
                            }).ToList()
                        // ====== HẾT PHẦN COMPANY EVENTS ======
                    }).ToList(),

                AllocationEventHistories = a.AllocationEventHistories
                    .Where(h => !h.IsDeleted)
                    .Select(h => new AllocationEventHistoryModel
                    {
                        Id = h.Id,
                        AllocationEventId = h.AllocationEventId,
                        TargetName = h.TargetName,
                        ActionName = h.ActionName,
                        Description = h.Description,
                        CreatedAt = h.CreatedAt,
                        UpdatedAt = h.UpdatedAt,
                        CreatedBy = h.CreatedBy,
                        UpdatedBy = h.UpdatedBy,
                        Status = h.Status,
                        IsDeleted = h.IsDeleted
                    }).ToList()
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result<List<AllocationEventModel>>.Success(allocations);
    }
}
