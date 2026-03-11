using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Teacher.Queries
{
    public class TeacherQuery
    {
        public string? TeacherCode { get; set; }
        public string? TeacherName { get; set; }
        public Guid? ManagerId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedTeachersQuery : IRequest<Result<PagedResult<TeacherModel>>>
    {
        public TeacherQuery? TeacherQuery { get; set; }

        public class GetPagedTeachersQueryHandler : IRequestHandler<GetPagedTeachersQuery, Result<PagedResult<TeacherModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly PagingOptions _pagingOptions;
            private readonly IMapper _mapper;

            public GetPagedTeachersQueryHandler(
                IRegalEducationDbContext context,
                PagingOptions pagingOptions,
                IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<PagedResult<TeacherModel>>> Handle(GetPagedTeachersQuery request, CancellationToken cancellationToken)
            {
                if (request.TeacherQuery == null)
                    throw new ArgumentNullException (nameof (request.TeacherQuery));

                var query = _context.Teachers
                    .Include (t => t.Company)
                    .AsNoTracking ( );

                int totalRecords = await query.CountAsync (cancellationToken);
                request.TeacherQuery.PageSize = _pagingOptions.DefaultPageSize;

                var paged = await query
                    .OrderByDescending (x => x.CreatedAt)
                    .Skip ((request.TeacherQuery.Page - 1) * request.TeacherQuery.PageSize)
                    .Take (request.TeacherQuery.PageSize)
                    .ToListAsync (cancellationToken);

                var result = paged.Select (teacher => new TeacherModel
                {
                    Id = teacher.Id,
                    TeacherNickname = teacher.TeacherNickname,
                    TeacherQualifications = teacher.TeacherQualifications,
                    TeacherSpecialization = teacher.TeacherSpecialization,
                    WorkType = teacher.WorkType,
                    JoinDate = teacher.JoinDate,
                    PreferLevel = teacher.PreferLevel,
                    TeachingOutside = teacher.TeachingOutside,
                    TeacherAssistant = teacher.TeacherAssistant,
                    IsOnline = teacher.IsOnline,
                    ApplicationUserId = teacher.ApplicationUserId,
                }).ToList ( );

                var pagedResult = new PagedResult<TeacherModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<TeacherModel>>.Success (pagedResult);
            }
        }
    }
}
