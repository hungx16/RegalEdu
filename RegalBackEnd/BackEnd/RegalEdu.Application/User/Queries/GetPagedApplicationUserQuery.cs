using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Request;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models;


namespace RegalEdu.Application.User.Queries
{
    public class GetPagedApplicationUserQuery : IRequest<Result<PagedResult<ApplicationUserModel>>>
    {
        public ApplicationUserQuery? ApplicationUserQuery { get; set; }
    }

    public class GetPagedApplicationUserQueryHandler : IRequestHandler<GetPagedApplicationUserQuery, Result<PagedResult<ApplicationUserModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        public GetPagedApplicationUserQueryHandler(
            IRegalEducationDbContext dbContext)
        {
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Result<PagedResult<ApplicationUserModel>>> Handle(GetPagedApplicationUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Ensure ApplicationUserQuery is not null before accessing its properties
                //request.ApplicationUserQuery = null;
                if (request.ApplicationUserQuery == null)
                {
                    throw new ArgumentNullException(nameof(request.ApplicationUserQuery));
                }
                // Start querying
                IQueryable<ApplicationUser> users = _context.ApplicationUsers.AsNoTracking();

                // Filter by each field
                if (!string.IsNullOrWhiteSpace(request.ApplicationUserQuery.FullName))
                {
                    users = users.Where(u => u.FullName!.Contains(request.ApplicationUserQuery.FullName));
                }

                if (!string.IsNullOrWhiteSpace(request.ApplicationUserQuery.UserName))
                {
                    users = users.Where(u => u.UserName!.Contains(request.ApplicationUserQuery.UserName));
                }

                if (!string.IsNullOrWhiteSpace(request.ApplicationUserQuery.Email))
                {
                    users = users.Where(u => u.Email!.Contains(request.ApplicationUserQuery.Email));
                }

                if (!string.IsNullOrWhiteSpace(request.ApplicationUserQuery.PhoneNumber))
                {
                    users = users.Where(u => u.PhoneNumber!.Contains(request.ApplicationUserQuery.PhoneNumber));
                }

                if (request.ApplicationUserQuery.IsDeleted.HasValue)
                {
                    users = users.Where(u => u.IsDeleted == request.ApplicationUserQuery.IsDeleted.Value);
                }
                if (request.ApplicationUserQuery.CreatedAt.HasValue)
                    if (request.ApplicationUserQuery.CreatedAt.HasValue && request.ApplicationUserQuery.CreatedAt.Value.Date != default)
                    {
                        users = users.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date == request.ApplicationUserQuery.CreatedAt.Value.Date);
                    }
                int totalRecords = await users.CountAsync(cancellationToken);

                // Pagination
                var pagedUsers = await users
                    .OrderByDescending(u => u.CreatedAt) // hoặc tùy ý OrderBy khác
                    .Skip((request.ApplicationUserQuery.Page - 1) * request.ApplicationUserQuery.PageSize)
                    .Take(request.ApplicationUserQuery.PageSize)
                    .ToListAsync(cancellationToken);
                // Mapping
                var result = pagedUsers.Select(user => new ApplicationUserModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserCode = user.UserCode,
                    UserName = user.UserName,
                    Email = user.Email,
                    Avatar = user?.Avatar,
                    Gender = user.Gender,
                    GenderText = user.Gender ? "Nam" : "Nữ",
                    IsDeleted = user.IsDeleted,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt
                }).ToList();

                var pagedResult = new PagedResult<ApplicationUserModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<ApplicationUserModel>>.Success(pagedResult);
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
