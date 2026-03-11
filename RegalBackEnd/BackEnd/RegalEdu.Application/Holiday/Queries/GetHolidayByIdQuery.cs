using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.Holiday.Queries
{
    public class GetHolidayByIdQuery : IRequest<Result<HolidayModel>>
    {
        public required string Id { get; set; }

        public class GetHolidayByIdQueryHandler : IRequestHandler<GetHolidayByIdQuery, Result<HolidayModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public GetHolidayByIdQueryHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result<HolidayModel>> Handle(GetHolidayByIdQuery request, CancellationToken cancellationToken)
            {
                var holiday = await _context.Holidays
                    .Include (h => h.Category)
                    .AsNoTracking ( )
                    .FirstOrDefaultAsync (h => h.Id.ToString ( ) == request.Id && !h.IsDeleted, cancellationToken);

                if (holiday == null)
                {
                    var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["Holiday"], request.Id);
                    return Result<HolidayModel>.Failure (msg);
                }

                var result = _mapper.Map<HolidayModel> (holiday);
                return Result<HolidayModel>.Success (result);
            }
        }
    }
}
