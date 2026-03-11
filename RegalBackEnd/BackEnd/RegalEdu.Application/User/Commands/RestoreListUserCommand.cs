using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Shared;

namespace RegalEdu.Application.User.Commands
{
    public class RestoreListUserCommand : IRequest<Result>
    {
        public List<string> ListIds { get; set; }
       
    }

    public class RestoreListUserCommandHandler : IRequestHandler<RestoreListUserCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<RestoreListUserCommandHandler> _logger;
        private readonly IRegalEducationDbContext _context;
        private readonly ICurrentUserService _currentUserService;


        public RestoreListUserCommandHandler(
             ILogger<RestoreListUserCommandHandler> logger,
             IIdentityService identityService,
             IRegalEducationDbContext context,
             ICurrentUserService currentUserService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(RestoreListUserCommand request, CancellationToken cancellationToken)
        {
            string modelName = "người dùng";
            string action = "khôi phục";
            if (request.ListIds?.Count > 0)
            {
                try
                {
                    foreach (var id in request.ListIds)
                    {
                        var entityExist = _context.ApplicationUsers.Where(t => t.Id.ToString() == id).FirstOrDefault();
                        if (entityExist == null)
                        {
                            return Result.Failure($"Không tồn tại {modelName}");
                        }
                        entityExist.IsDeleted = false;
                        _context.ApplicationUsers.Update(entityExist);
                    }
                    return await _context.SaveChangesAsync() > 0 ? Result.Success() : Result.Failure($"Thất bại {action} {modelName}");
                }
                catch (Exception ex)
                {
                    // Kiểm tra và hiển thị InnerException nếu có
                    var errorMessage = $"Thất bại {action} {modelName}: " + ex.Message;
                    if (ex.InnerException != null)
                    {
                        errorMessage += " | Inner Exception: " + ex.InnerException.Message;
                    }
                    return Result.Failure(errorMessage);
                }
                finally
                {
                    _logger.LogInformation($"{Functions.CapitalizeFirstLetter(action)} {modelName} [{string.Join(", ", request.ListIds)}] bởi {_currentUserService.UserName ?? "Unknown"}");
                }
            }
            else
            {
                return Result.Failure($"Không có {modelName} nào được chọn để {action}");
            }
        }
    }
}
