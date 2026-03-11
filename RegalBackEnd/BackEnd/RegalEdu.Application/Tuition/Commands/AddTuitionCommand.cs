using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Tuition.Commands
{
    public class AddTuitionCommand : IRequest<Result>
    {
        public required TuitionModel TuitionModel { get; set; }
    }

    public class AddTuitionCommandHandler : IRequestHandler<AddTuitionCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public AddTuitionCommandHandler(IRegalEducationDbContext context, AutoMapper.IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(AddTuitionCommand request, CancellationToken cancellationToken)
        {
            if (_context is not DbContext dbContext)
            {
                throw new InvalidOperationException (_localizer[LocalizationKey.InvalidDbContextInstance]);
            }

            var tuition = _mapper.Map<Domain.Entities.Tuition> (request.TuitionModel);
            await _context.Tuition.AddAsync (tuition, cancellationToken);
            var success = await _context.SaveChangesAsync (cancellationToken) > 0;

            if (success)
            {
                return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer[EntityName.Tuition]));
            }
            else
            {
                return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer[EntityName.Tuition]));
            }
        }
    }
}