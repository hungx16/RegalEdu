using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Student.Queries
{
    public class GetStudentByIdQuery : IRequest<Result<StudentModel>>
    {
        public required string Id { get; set; }
    }

    public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentModel>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetStudentByIdQueryHandler(IRegalEducationDbContext db, IMapper mapper, ILocalizationService localizer)
        {
            _db = db; _mapper = mapper; _localizer = localizer;
        }

        public async Task<Result<StudentModel>> Handle(GetStudentByIdQuery request, CancellationToken ct)
        {
            var e = await _db.Students.AsNoTracking()
                .Include(s => s.Category)
                .Include(s => s.Employee)
                .Include(s => s.Company)
                .Include(c => c.Region)
                .Include(s => s.RegisterStudys)
                .Include(s => s.Coupons)
                .Include(s => s.Contacts)
                .Include(s => s.StudentActivity)
                .Include(s => s.StudentNote)
                .Include(s => s.StudentCourse)
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(x => x.Id.ToString() == request.Id && !x.IsDeleted, ct);

            if (e == null)
                return Result<StudentModel>.Failure(_localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.Student, request.Id));

            return Result<StudentModel>.Success(_mapper.Map<StudentModel>(e));
        }
    }
}
