using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;


namespace RegalEdu.Application.AccountGroup.Commands
{
    public class DeleteAccountGroupCommand : IRequest<Result>
    {
        public List<string> ListId { get; set; }
    }
    public class DeleteAccountGroupCommandHandler : IRequestHandler<DeleteAccountGroupCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;

        public DeleteAccountGroupCommandHandler(IRegalEducationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Result> Handle(DeleteAccountGroupCommand request, CancellationToken cancellationToken)
        {
            var listDataRemove = await _context.AccountGroups.Where(t => request.ListId.Contains(t.Id.ToString())).ToListAsync();
            _context.AccountGroups.RemoveRange(listDataRemove);

            return await _context.SaveChangesAsync() > 0 ? Result.Success() : Result.Failure("Faild to delete account group");
        }
    }
}
