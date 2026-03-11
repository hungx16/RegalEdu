using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Company.Commands
{
    public class CreateLogRegionComCommand : IRequest<Result>
    {
        public required LogRegionComModel LogRegionComModel { get; set; }

        public class CreateLogRegionComCommandHandler : IRequestHandler<CreateLogRegionComCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public CreateLogRegionComCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(CreateLogRegionComCommand request, CancellationToken cancellationToken)
            {
                var activeLogRegionCom = await _context.LogRegionComs
                    .Where (x => x.CompanyId == request.LogRegionComModel.CompanyId && x.Status == 0 && x.EndDate == null)
                    .FirstOrDefaultAsync (cancellationToken);
                if (activeLogRegionCom?.StartedDate.Date >= request.LogRegionComModel.StartedDate.Date)
                {
                    return Result.Failure (_localizer["NewRegionStartDateMustBeGreaterThanCurrent"]);

                }
                if (activeLogRegionCom?.RegionId >= request.LogRegionComModel.RegionId)
                {
                    return Result.Failure (_localizer["CurrentAndNewRegionMustBeDifferent"]);

                }
                if (activeLogRegionCom != null)
                {
                    activeLogRegionCom.Status = StatusType.InActive;
                    activeLogRegionCom.EndDate = request.LogRegionComModel.StartedDate.AddDays (-1);
                }
                var logRegionCom = _mapper.Map<LogRegionComModel, RegalEdu.Domain.Entities.LogRegionCom> (request.LogRegionComModel);
                await _context.LogRegionComs.AddAsync (logRegionCom, cancellationToken);
                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                if (success)
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer[EntityName.Company]));
                else
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer[EntityName.Company]));
            }
        }
    }
}
