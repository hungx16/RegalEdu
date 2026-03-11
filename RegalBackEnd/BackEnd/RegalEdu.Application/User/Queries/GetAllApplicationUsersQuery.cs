using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;


namespace RegalEdu.Application.User.Queries
{
    public class GetAllApplicationUsersQuery : IRequest<Result<List<ApplicationUserModel>>>
    {
    }

    public class GetAllApplicationUsersQueryHandler : IRequestHandler<GetAllApplicationUsersQuery, Result<List<ApplicationUserModel>>>
    {

        private readonly IRegalEducationDbContext _context;
        public GetAllApplicationUsersQueryHandler(IRegalEducationDbContext dbContext)
        {
            _context = dbContext ?? throw new ArgumentNullException (nameof (dbContext));
        }

        public async Task<Result<List<ApplicationUserModel>>> Handle(GetAllApplicationUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _context.ApplicationUsers.Include (t => t.Employee).Include (t => t.Teacher).AsNoTracking ( ).ToListAsync (cancellationToken);

                var result = users.Select (user => new ApplicationUserModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserCode = user.UserCode,
                    UserName = user.UserName,
                    Email = user.Email,
                    Avatar = user?.Avatar,
                    Gender = (bool)user.Gender,
                    GenderText = (bool)user.Gender ? "Nam" : "Nữ",
                    IsDeleted = user.IsDeleted,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    Teacher = user.Teacher == null ? null : new TeacherModel
                    {
                        Id = user.Teacher.Id,
                        TeacherNickname = user.Teacher.TeacherNickname,
                        TeacherQualifications = user.Teacher.TeacherQualifications,
                        TeacherSpecialization = user.Teacher.TeacherSpecialization,
                        WorkType = user.Teacher.WorkType,
                        JoinDate = user.Teacher.JoinDate,
                        PreferLevel = user.Teacher.PreferLevel,
                        TeachingOutside = user.Teacher.TeachingOutside,
                        TeacherAssistant = user.Teacher.TeacherAssistant,
                        IsOnline = user.Teacher.IsOnline,
                        ApplicationUserId = user.Teacher.ApplicationUserId,

                    },
                    Employee = user.Employee == null ? null : new EmployeeModel
                    {
                        Id = user.Employee.Id,
                        ApplicationUserId = user.Employee.ApplicationUserId,
                        CompanyId = user.Employee.CompanyId,
                        PositionId = user.Employee.PositionId,
                        DepartmentId = user.Employee.DepartmentId,
                        EmployeeTax = user.Employee.EmployeeTax,
                        IsSupport = user.Employee.IsSupport,
                        EmployeeStartedDate = user.Employee.EmployeeStartedDate,
                        EmployeeEndDate = user.Employee.EmployeeEndDate,
                        EmployeeNewEndDate = user.Employee.EmployeeNewEndDate,
                        PersonalEmail = user.Employee.PersonalEmail,
                        Status = user.Employee.Status
                    }



                }).ToList ( );

                return Result<List<ApplicationUserModel>>.Success (result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
