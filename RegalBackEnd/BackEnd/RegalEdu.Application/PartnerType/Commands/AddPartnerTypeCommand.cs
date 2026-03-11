using MediatR;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.PartnerType.Commands
{
    public class AddPartnerTypeCommand : IRequest<Result>
    {
        public required PartnerTypeModel Model { get; set; }

        public class Handler : IRequestHandler<AddPartnerTypeCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public Handler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            }

            public async Task<Result> Handle(AddPartnerTypeCommand request, CancellationToken cancellationToken)
            {
                bool dup = await _context.PartnerTypes.AnyAsync(x => x.PartnerTypeCode == request.Model.PartnerTypeCode && !x.IsDeleted, cancellationToken);
                if (dup)
                    return Result.Failure(_localizer.Format("PartnerTypeCodeAlreadyExists", request.Model.PartnerTypeCode));

                var entity = _mapper.Map<RegalEdu.Domain.Entities.PartnerType>(request.Model);
                await _context.PartnerTypes.AddAsync(entity, cancellationToken);

                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                return success
                    ? Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, _localizer["PartnerType"]))
                    : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["PartnerType"]));
            }
        }
    }
}