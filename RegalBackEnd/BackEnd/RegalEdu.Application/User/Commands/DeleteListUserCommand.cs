using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Shared;

namespace RegalEdu.Application.User.Commands
{
    public class DeleteListUserCommand : IRequest<Result>
    {
        public List<string> ListIds { get; set; }

    }

    public class DeleteListUserCommandHandler : IRequestHandler<DeleteListUserCommand, Result>
    {
        private readonly ILogger<DeleteListUserCommandHandler> _logger;
        private readonly IRegalEducationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILocalizationService _localizer;

        public DeleteListUserCommandHandler(
             ILogger<DeleteListUserCommandHandler> logger,
             IRegalEducationDbContext context,
             ICurrentUserService currentUserService,
             ILocalizationService localizer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(logger));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(DeleteListUserCommand request, CancellationToken cancellationToken)
        {
            string modelName = _localizer["User"];
            string action = _localizer["Delete"];
            if (request.ListIds?.Count > 0)
            {
                try
                {
                    foreach (var id in request.ListIds)
                    {
                        var entityExist = _context.ApplicationUsers.FirstOrDefault(t => t.Id.ToString() == id);
                        if (entityExist == null)
                        {
                            return Result.Failure(_localizer.Format("ModelNotFound", modelName));
                        }
                        entityExist.IsDeleted = true;
                        _context.ApplicationUsers.Update(entityExist);
                    }
                    return await _context.SaveChangesAsync() > 0
                        ? Result.Success()
                        : Result.Failure(_localizer.Format("ActionFailed", action, modelName));
                }
                catch (Exception ex)
                {
                    var errorMessage = _localizer.Format("ActionException", action, modelName, ex.Message);
                    if (ex.InnerException != null)
                    {
                        errorMessage += " | " + ex.InnerException.Message;
                    }
                    return Result.Failure(errorMessage);
                }
            }
            else
            {
                return Result.Failure(_localizer.Format("NoModelToAction", modelName, action));
            }
        }

    }
}
