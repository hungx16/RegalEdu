using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkingTimeConfiguration.Commands
{
    public class UpdateWorkingTimeConfigurationCommand : IRequest<Result>
    {
        public required WorkingTimeConfigurationModel WorkingTimeConfigurationModel { get; set; }

        public class UpdateWorkingTimeConfigurationCommandHandler : IRequestHandler<UpdateWorkingTimeConfigurationCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public UpdateWorkingTimeConfigurationCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(UpdateWorkingTimeConfigurationCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.WorkingTimeConfigurations
                    .Include (t => t.WorkingTimeConfigurationCompanies)
                    .FirstOrDefaultAsync (x => x.Id == request.WorkingTimeConfigurationModel.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, "WorkingTimeConfiguration"));

                // Check trường hợp FE truyền mảng rỗng
                if (request.WorkingTimeConfigurationModel.WorkingTimeConfigurationCompanies != null &&
                    !request.WorkingTimeConfigurationModel.WorkingTimeConfigurationCompanies.Any ( ) && request.WorkingTimeConfigurationModel.ApplyToSystem == false)
                {
                    // Sử dụng localizer cho message lỗi business
                    return Result.Failure (_localizer[LocalizationKey.MustHasAtLeastOneCompany]);
                }
                entity.Status = request.WorkingTimeConfigurationModel.Status;
                entity.IsDefault = request.WorkingTimeConfigurationModel.IsDefault;
                entity.NameConfiguration = request.WorkingTimeConfigurationModel.NameConfiguration;
                entity.Description = request.WorkingTimeConfigurationModel.Description;
                if (entity.WorkingTimeConfigurationCompanies != null && entity.WorkingTimeConfigurationCompanies.Any ( ))
                {
                    _context.WorkingTimeConfigurationCompanies.RemoveRange (entity.WorkingTimeConfigurationCompanies);
                    entity.WorkingTimeConfigurationCompanies.Clear ( );
                }
                if (request.WorkingTimeConfigurationModel.WorkingTimeConfigurationCompanies != null)
                {
                    // Tạo mới liên kết nếu có (nếu rỗng => sẽ không tạo mới gì)
                    var workingTimeConfigurationCompanies = request.WorkingTimeConfigurationModel.WorkingTimeConfigurationCompanies
                        .Select (dp => new WorkingTimeConfigurationCompany
                        {
                            Id = Guid.NewGuid ( ),
                            CompanyId = dp.CompanyId,
                            WorkingTimeConfigurationId = entity.Id
                        }).ToList ( );
                    _context.WorkingTimeConfigurationCompanies.AddRange (workingTimeConfigurationCompanies);
                }
                entity.ApplyToSystem = request.WorkingTimeConfigurationModel.ApplyToSystem;
                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                if (success)
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["WorkingTimeConfiguration"]));
                else
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["WorkingTimeConfiguration"]));
            }
        }
    }
}
