using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AllocationEvent.Queries
{
    public class GetAllCompanyEventProposalQuery
        : IRequest<Result<List<CompanyEventModel>>>
    { }

    public class GetAllCompanyEventProposalQueryHandler
        : IRequestHandler<GetAllCompanyEventProposalQuery, Result<List<CompanyEventModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllCompanyEventProposalQueryHandler(
            IRegalEducationDbContext context,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<List<CompanyEventModel>>> Handle(
            GetAllCompanyEventProposalQuery request,
            CancellationToken cancellationToken)
        {
            var events = await _context.CompanyEvents
          .Include(e => e.AllocationDetailEvent)
              .ThenInclude(ad => ad.AllocationEvent)

          .Include(e => e.AllocationDetailEvent)
              .ThenInclude(ad => ad.Company)
          .Include(e => e.AllocationDetailEvent)
              .ThenInclude(ad => ad.Region)
          .Include(e => e.AffiliatePartner)
          .Include(t => t.EventPublications)
          .Include(t => t.EventCashes)
          .Include(t => t.EventParticipants)
          .Include(t => t.Attachments)
          .Include(t => t.ApproveCompanyEvents)
          .ToListAsync(cancellationToken);
            // 🔹 Map sang DTO
            var result = _mapper.Map<List<CompanyEventModel>>(events);

            return Result<List<CompanyEventModel>>.Success(result);
        }
    }
}
