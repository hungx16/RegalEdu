using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.Holiday.Queries
{
    public class GetAllHolidaysQuery : IRequest<Result<List<HolidayModel>>>
    {
        public class GetAllHolidaysQueryHandler : IRequestHandler<GetAllHolidaysQuery, Result<List<HolidayModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetAllHolidaysQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<HolidayModel>>> Handle(GetAllHolidaysQuery request, CancellationToken cancellationToken)
            {
                var holidays = await _context.Holidays
                    .Include (h => h.Category)
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<HolidayModel>> (holidays);
                return Result<List<HolidayModel>>.Success (result);
            }
        }
    }
}
