using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;


namespace RegalEdu.Application.AccountGroup.Commands
{
    public class UpdateAccountGroupCommand : IRequest<Result>
    {
        public AccountGroupModel Entity { get; set; }
    }
    public class UpdateAccountGroupCommandHandler : IRequestHandler<UpdateAccountGroupCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localizer;

        public UpdateAccountGroupCommandHandler(IMapper mapper, IRegalEducationDbContext context, ILocalizationService localizer)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer)); ;
        }
        public async Task<Result> Handle(UpdateAccountGroupCommand request, CancellationToken cancellationToken)
        {
            var existData = _context.AccountGroups.FirstOrDefault(t => t.Name == request.Entity.Name);
            if (existData != null && existData.Id != request.Entity.Id)
            {
                return Result.Failure(_localizer["AccountGroupNameExists"]);
            }

            var entity = await _context.AccountGroups.FirstOrDefaultAsync(i => i.Id.Equals(request.Entity.Id), cancellationToken);

            if (entity == null)
            {
                return Result.Failure(_localizer["AccountGroupNotExist"]);
            }
            if (request.Entity.UseDefault == true)
            {
                existData = _context.AccountGroups.FirstOrDefault(t => t.Id != request.Entity.Id && t.UseDefault == true);
                if (existData != null)
                {
                    return Result.Failure(_localizer["OnlyOneDefaultGroup"]);
                }
            }

            _mapper.Map(request.Entity, entity);
            return await _context.SaveChangesAsync(cancellationToken) > 0
                ? Result.Success()
                : Result.Failure(_localizer["UpdateAccountGroupFailed"]);
        }
    }
}
