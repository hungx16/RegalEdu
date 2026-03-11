using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AllocationEvent.Queries
{
    public class GetAllCompanyEventReportsQuery : IRequest<Result<List<CompanyEventReportModel>>>
    {
    }

    public class GetAllCompanyEventReportsQueryHandler
        : IRequestHandler<GetAllCompanyEventReportsQuery, Result<List<CompanyEventReportModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserPermissionInfoService _userPermissionInfoService;

        private readonly ICurrentUserService _currentUserService;
        public GetAllCompanyEventReportsQueryHandler(IRegalEducationDbContext context, IMapper mapper, IUserPermissionInfoService userPermissionInfoService, ICurrentUserService currentUserService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userPermissionInfoService = userPermissionInfoService ?? throw new ArgumentNullException(nameof(userPermissionInfoService));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task<Result<List<CompanyEventReportModel>>> Handle(
            GetAllCompanyEventReportsQuery request,
            CancellationToken cancellationToken)
        {
            var employeeIdStr = _currentUserService.EmployeeId;

            var reports = await _context.CompanyEventReports
                .Include(r => r.CompanyEvent).ThenInclude(t => t.AllocationDetailEvent).ThenInclude(t => t.AllocationEvent)
                .Include(r => r.CompanyEvent).ThenInclude(t => t.AllocationDetailEvent).ThenInclude(t => t.Company)
                .Include(r => r.CompanyEvent).ThenInclude(t => t.AllocationDetailEvent).ThenInclude(t => t.Region)
                .Include(r => r.ApproveCompanyEvents)
                .Include(t => t.EventPublications)
                .Include(t => t.EventCashes)
                .Include(t => t.EventParticipants)
                .Include(t => t.Attachments)
                .Include(t => t.ApproveCompanyEvents)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);

            var result = _mapper.Map<List<CompanyEventReportModel>>(reports);
            return Result<List<CompanyEventReportModel>>.Success(result);
        }
    }
}
