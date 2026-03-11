using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AllocationEvent.Queries
{
    public class GetCompanyEventReportsByCompanyEventIdQuery
        : IRequest<Result<List<CompanyEventReportModel>>>
    {
        public required string CompanyEventId { get; set; }
    }

    public class GetCompanyEventReportsByCompanyEventIdQueryHandler
        : IRequestHandler<GetCompanyEventReportsByCompanyEventIdQuery, Result<List<CompanyEventReportModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetCompanyEventReportsByCompanyEventIdQueryHandler(
            IRegalEducationDbContext context,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<List<CompanyEventReportModel>>> Handle(
            GetCompanyEventReportsByCompanyEventIdQuery request,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.CompanyEventId, out var companyEventId))
            {
                return Result<List<CompanyEventReportModel>>.Failure("Invalid ID format.");
            }

            var reports = await _context.CompanyEventReports
                .Where(r => r.CompanyEventId == companyEventId)
                .Include(r => r.CompanyEvent).ThenInclude(t => t.AllocationDetailEvent).ThenInclude(t => t.AllocationEvent)
                .Include(r => r.CompanyEvent).ThenInclude(t => t.AllocationDetailEvent).ThenInclude(t => t.Company)
                .Include(r => r.CompanyEvent).ThenInclude(t => t.AllocationDetailEvent).ThenInclude(t => t.Region)
                .Include(r => r.ApproveCompanyEvents)
                .Include(t => t.EventPublications)
                .Include(t => t.EventCashes)
                .Include(t => t.EventParticipants)
                .Include(t => t.Attachments)
                .ToListAsync(cancellationToken);

            var result = _mapper.Map<List<CompanyEventReportModel>>(reports);
            return Result<List<CompanyEventReportModel>>.Success(result);
        }
    }
}
