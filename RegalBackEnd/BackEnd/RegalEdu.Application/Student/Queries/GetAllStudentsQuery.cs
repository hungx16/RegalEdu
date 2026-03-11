using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Student.Queries
{
    public class GetAllStudentsQuery : IRequest<Result<List<StudentModel>>> { }

    public class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, Result<List<StudentModel>>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;

        public GetAllStudentsQueryHandler(IRegalEducationDbContext db, IMapper mapper)
        {
            _db = db; _mapper = mapper;
        }

        public async Task<Result<List<StudentModel>>> Handle(GetAllStudentsQuery request, CancellationToken ct)
        {
            var data = await _db.Students.AsNoTracking()
                .Include(s => s.Category)
                .Include(s => s.Employee)
                .Include(s => s.Company)
                .Include(s => s.RegisterStudys)
                .Include(s => s.Coupons)
                .Include(s => s.Contacts)
                .Include(s => s.StudentActivity)
                .Include(s => s.StudentNote)
                .Include(s => s.StudentCourse)
                .Include(s => s.Enrollments)
                .Include(s => s.Profile)
                .Include(s=>s.Region)
                .ToListAsync(ct);

            return Result<List<StudentModel>>.Success(_mapper.Map<List<StudentModel>>(data));
        }
    }
}
