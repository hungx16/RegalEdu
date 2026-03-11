using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Employee.Queries
{
    public class GetEmployeeByIdOrEmailQuery : IRequest<Result<EmployeeModel>>
    {
        public string? Id { get; set; }
        public string? CompanyEmail { get; set; }
    }

    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdOrEmailQuery, Result<EmployeeModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetEmployeeByIdQueryHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result<EmployeeModel>> Handle(GetEmployeeByIdOrEmailQuery request, CancellationToken cancellationToken)
        {
            var employee = await _context.Employees
                .Include(t => t.ApplicationUser)
                .Include(x => x.Position)
                .Include(x => x.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    ((!string.IsNullOrEmpty(request.Id) && x.Id.ToString() == request.Id)
                     || (!string.IsNullOrEmpty(request.CompanyEmail) && x.ApplicationUser != null && x.ApplicationUser.Email == request.CompanyEmail))
                    && !x.IsDeleted, cancellationToken);

            EmployeeModel? result = null;

            if (employee != null)
            {
                result = _mapper.Map<EmployeeModel>(employee);
            }
            else
            {
                // Email can belong to a teacher as well. Try to fetch teacher profile with avatar.
                var teacher = await _context.Teachers
                    .Include(t => t.ApplicationUser)
                    .Include(t => t.Company)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t =>
                        ((!string.IsNullOrEmpty(request.Id) && t.Id.ToString() == request.Id)
                         || (!string.IsNullOrEmpty(request.CompanyEmail) && t.ApplicationUser != null && t.ApplicationUser.Email == request.CompanyEmail))
                        && !t.IsDeleted, cancellationToken);

                if (teacher != null)
                {
                    var applicationUserModel = _mapper.Map<ApplicationUserModel>(teacher.ApplicationUser);
                    result = new EmployeeModel
                    {
                        Id = teacher.Id,
                        ApplicationUserId = teacher.ApplicationUserId,
                        ApplicationUser = applicationUserModel,
                        CompanyId = teacher.CompanyId,
                        PositionId = Guid.Empty,
                        DepartmentId = Guid.Empty

                    };
                }
            }

            if (result == null)
            {
                var identifier = !string.IsNullOrEmpty(request.Id) ? request.Id : request.CompanyEmail;
                var msg = _localizer.Format(LocalizationKey.EntityWithIdNotFound, $"{_localizer["Employee"]}/{_localizer["Teacher"]}", identifier);
                return Result<EmployeeModel>.Failure(msg);
            }

            return Result<EmployeeModel>.Success(result);
        }
    }
}
