using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Logging;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponIssue.Commands
{
    public class UpdateCouponIssueCommand : IRequest<Result>
    {
        public required CouponIssueModel CouponIssueModel { get; set; }
    }

    public class UpdateCouponIssueCommandHandler : IRequestHandler<UpdateCouponIssueCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<UpdateCouponIssueCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateCouponIssueCommandHandler(
            IRegalEducationDbContext context,
            ILogger<UpdateCouponIssueCommandHandler> logger,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result> Handle(UpdateCouponIssueCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.CouponIssues
                .Include(x => x.Coupons)
                .Include(x => x.CouponIssueStudent)
                .FirstOrDefaultAsync(x => x.Id == request.CouponIssueModel.Id, cancellationToken);

            if (entity == null)
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, EntityName.CouponIssue));

            _mapper.Map(request.CouponIssueModel, entity);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            return success
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.CouponIssue))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.CouponIssue));
        }
    }
}
