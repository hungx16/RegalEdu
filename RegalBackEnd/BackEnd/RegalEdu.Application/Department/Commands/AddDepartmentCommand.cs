using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Department.Commands
{
    public class AddDepartmentCommand : IRequest<Result>
    {
        public required DepartmentModel DepartmentModel { get; set; }
    }

    public class AddDepartmentCommandHandler : IRequestHandler<AddDepartmentCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<AddDepartmentCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public AddDepartmentCommandHandler(IRegalEducationDbContext context, ILogger<AddDepartmentCommandHandler> logger, AutoMapper.IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(AddDepartmentCommand request, CancellationToken cancellationToken)
        {
            var info = AutoCodeConfig.Get (AutoCodeType.Department);
            if (_context is not DbContext dbContext)
            {
                throw new InvalidOperationException (_localizer[LocalizationKey.InvalidDbContextInstance]);
            }
            var result = await AutoCodeHelper.CreateWithAutoCodeRetryAsync (
                info,
                async (code) =>
                {
                    // Fix: Ensure the correct namespace/type is used for the Department entity
                    var department = _mapper.Map<Domain.Entities.Department> (request.DepartmentModel);
                    department.DepartmentCode = code;

                    await _context.Departments.AddAsync (department, cancellationToken);
                    var success = await _context.SaveChangesAsync (cancellationToken) > 0;

                    if (success)
                    {
                        return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Department));
                    }
                    else
                    {
                        return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Department));
                    }
                },
                dbContext
            );
            return result;
        }
    }
}
