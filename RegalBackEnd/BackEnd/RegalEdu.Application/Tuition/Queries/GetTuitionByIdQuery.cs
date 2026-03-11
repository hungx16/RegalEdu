using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Tuition.Queries
{
    public class GetTuitionByIdQuery : IRequest<Result<TuitionModel>>
    {
        public required string Id { get; set; }
    }

    public class GetTuitionByIdQueryHandler : IRequestHandler<GetTuitionByIdQuery, Result<TuitionModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetTuitionByIdQueryHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result<TuitionModel>> Handle(GetTuitionByIdQuery request, CancellationToken cancellationToken)
        {
            var tuition = await _context.Tuition
                .Include (x => x.Course)
                .Include (x => x.ClassType)
                .AsNoTracking ( )
                .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

            if (tuition == null)
            {
                var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.Tuition], request.Id);
                return Result<TuitionModel>.Failure (msg);
            }

            var result = _mapper.Map<TuitionModel> (tuition);
            return Result<TuitionModel>.Success (result);
        }
    }
}
