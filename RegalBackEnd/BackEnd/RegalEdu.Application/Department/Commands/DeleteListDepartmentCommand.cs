using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Department.Commands
{
    public class DeleteListDepartmentCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class DeleteListDepartmentCommandHandler : IRequestHandler<DeleteListDepartmentCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<DeleteListDepartmentCommandHandler> _logger;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDeleteService;

        public DeleteListDepartmentCommandHandler(IRegalEducationDbContext context, ILogger<DeleteListDepartmentCommandHandler> logger, ILocalizationService localizer, ISoftDeleteService softDeleteService)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            _softDeleteService = softDeleteService ?? throw new ArgumentNullException (nameof (softDeleteService));
        }

        public async Task<Result> Handle(DeleteListDepartmentCommand request, CancellationToken cancellationToken)
        {
            if (request.ListIds == null || !request.ListIds.Any ( ))
                return Result.Failure (_localizer.Format (LocalizationKey.NoModelToDelete, EntityName.Department));

            int successCount = 0;
            int failCount = 0;
            var failMessages = new List<string> ( );

            foreach (var id in request.ListIds)
            {
                var department = _context.Departments.FirstOrDefault (x => x.Id.ToString ( ) == id);
                if (department != null)
                {
                    var result = await _softDeleteService.RecursiveSoftDelete (department.Id, typeof (Domain.Entities.Department));
                    if (result.Succeeded)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        var deleteFailMsg = _localizer.Format (
                            LocalizationKey.EntityDeleteFailed,
                            EntityName.Department, id, result.Errors
                        );
                        failMessages.Add (deleteFailMsg);
                        _logger.LogWarning (deleteFailMsg);
                    }
                }
                else
                {
                    failCount++;
                    var notFoundMsg = _localizer.Format (
                        LocalizationKey.EntityWithIdNotFound,
                        EntityName.Department, id
                    );
                    failMessages.Add (notFoundMsg);
                    _logger.LogWarning (notFoundMsg);
                }
            }

            var msg = _localizer.Format (
                LocalizationKey.MSG_DELETE_RESULT,
                EntityName.Department, successCount, failCount
            );
            if (failMessages.Any ( ))
                msg += " " + string.Join (" ", failMessages);

            if (successCount > 0)
                return Result.Success (msg);
            else
                return Result.Failure (msg);
        }
    }
}
