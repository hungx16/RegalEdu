using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Employee.Commands
{
    public class UpdateProfileCommand : IRequest<Result>
    {
        public required EmployeeModel ProfileModel { get; set; }
    }
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateProfileCommandHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy Employee từ DB
            var employeeEntity = await _context.Employees
                .FirstOrDefaultAsync (x => x.Id == request.ProfileModel.Id, cancellationToken);

            if (employeeEntity == null)
            {
                return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, _localizer["Profile"]));
            }
            // 2. Lấy ApplicationUser từ DB qua ApplicationUserId (đã có trong Employee)
            var applicationUserEntity = await _context.ApplicationUsers
                .FirstOrDefaultAsync (u => u.Id == employeeEntity.ApplicationUserId, cancellationToken);

            if (applicationUserEntity == null)
            {
                return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, _localizer["ApplicationUser"]));
            }

            // 3. Map thông tin update vào entity hiện tại
            applicationUserEntity.FullName = request.ProfileModel.ApplicationUser?.FullName;
            applicationUserEntity.PhoneNumber = request.ProfileModel.ApplicationUser?.PhoneNumber;
            applicationUserEntity.Address = request.ProfileModel.ApplicationUser?.Address;
            applicationUserEntity.Gender = request.ProfileModel.ApplicationUser.Gender;
            applicationUserEntity.DateOfBirth = request.ProfileModel.ApplicationUser?.DateOfBirth;
            applicationUserEntity.Avatar = request.ProfileModel.ApplicationUser?.Avatar;
            applicationUserEntity.Email = request.ProfileModel.ApplicationUser?.Email;
            applicationUserEntity.UserName = request.ProfileModel.ApplicationUser?.Email.ToUpper ( ); // Cập nhật UserName nếu cần

            applicationUserEntity.NormalizedEmail = applicationUserEntity.UserName;
            applicationUserEntity.NormalizedUserName = applicationUserEntity.NormalizedEmail;
            employeeEntity.PersonalEmail = request.ProfileModel.PersonalEmail;

            var success = await _context.SaveChangesAsync (cancellationToken) > 0;
            if (success)
                return Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["Profile"]));
            else
                return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Profile"]));
        }
    }
}
