using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Logging;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Department.Commands
{
    public class UpdateDepartmentCommand : IRequest<Result>
    {
        public required DepartmentModel DepartmentModel { get; set; }
    }

    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<UpdateDepartmentCommandHandler> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateDepartmentCommandHandler(IRegalEducationDbContext context, ILogger<UpdateDepartmentCommandHandler> logger, AutoMapper.IMapper mapper, ILocalizationService localizer)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var department = await _context.Departments.FirstOrDefaultAsync (x => x.Id == request.DepartmentModel.Id);
                if (department == null)
                {
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, EntityName.Department));
                }
                _mapper.Map (request.DepartmentModel, department);
                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                if (success)
                {
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer[EntityName.Department]));
                }
                else
                {
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer[EntityName.Department]));
                }
            }
            catch (Exception ex)
            {
                return _logger.LogErrorAndFail (_localizer, ex, LocalizationKey.UnexpectedError.ToString ( ));
            }
        }
    }
}
