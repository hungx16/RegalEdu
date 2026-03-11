using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Event.Queries
{
    public class GetAllEventsQuery : IRequest<Result<List<EventModel>>>
    {
    }

    public class GetAllEventsQueryHandler : IRequestHandler<GetAllEventsQuery, Result<List<EventModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllEventsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper)); ;
        }

        public async Task<Result<List<EventModel>>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _context.Events.AsNoTracking ( ).ToListAsync (cancellationToken: cancellationToken);
            var result = _mapper.Map<List<EventModel>> (events);

            return Result<List<EventModel>>.Success (result);
        }
    }
}