using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.Holiday.Queries
{
    public class GetDeletedHolidaysQuery : IRequest<Result<List<HolidayModel>>>
    {
        public class GetDeletedHolidaysQueryHandler : IRequestHandler<GetDeletedHolidaysQuery, Result<List<HolidayModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetDeletedHolidaysQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<HolidayModel>>> Handle(GetDeletedHolidaysQuery request, CancellationToken cancellationToken)
            {
                var holidays = await _context.Holidays
                    .IgnoreQueryFilters ( )
                    .Include (h => h.Category)
                    .Where (h => h.IsDeleted)
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<HolidayModel>> (holidays);
                return Result<List<HolidayModel>>.Success (result);
            }
        }
    }
}
