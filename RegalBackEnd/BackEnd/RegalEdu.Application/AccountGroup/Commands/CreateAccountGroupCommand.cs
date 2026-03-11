
using AutoMapper;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
namespace RegalEdu.Application.AccountGroup.Commands
{
    public class CreateAccountGroupCommand : IRequest<Result>
    {
        public required AccountGroupModel AccountGroup { get; set; }
    }

    public class CreateAccountGroupCommandHandler : IRequestHandler<CreateAccountGroupCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IRegalEducationDbContext _context;

        public CreateAccountGroupCommandHandler(IMapper mapper, IRegalEducationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Result> Handle(CreateAccountGroupCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.AccountGroup? existData = _context.AccountGroups.FirstOrDefault(t => t.Name == request.AccountGroup.Name);
            if (existData != null)
            {
                return Result.Failure("User group name is exist.");
            }
            if (request.AccountGroup.UseDefault == true)
            {
                existData = _context.AccountGroups.FirstOrDefault(t => t.UseDefault == true);
                if (existData != null)
                {
                    return Result.Failure("Only one group was used for default.");
                }
            }
            RegalEdu.Domain.Entities.AccountGroup entity = _mapper.Map<Domain.Entities.AccountGroup>(request.AccountGroup);
            await _context.AccountGroups.AddAsync(entity);

            return await _context.SaveChangesAsync() > 0 ? Result.Success() : Result.Failure("Failed to create new account group");
        }
    }
}
