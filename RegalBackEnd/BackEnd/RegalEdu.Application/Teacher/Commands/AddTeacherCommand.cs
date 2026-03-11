using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Request;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Shared;

namespace RegalEdu.Application.Teacher.Commands
{
    public class AddTeacherCommand : IRequest<Result>
    {
        public required TeacherModel TeacherModel { get; set; }

        public class AddTeacherCommandHandler : IRequestHandler<AddTeacherCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;
            private readonly IIdentityService _identityService;
            private readonly IEmailService _emailService;
            private readonly IEmailTemplateService _templateService;
            public AddTeacherCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer, IIdentityService identityService,
                IEmailService emailService,
                IEmailTemplateService templateService)
            {
                _context = context;
                _mapper = mapper;
                _localizer = localizer;
                _identityService = identityService ?? throw new ArgumentNullException (nameof (identityService));
                _emailService = emailService ?? throw new ArgumentNullException (nameof (emailService));
                _templateService = templateService ?? throw new ArgumentNullException (nameof (templateService));
            }

            public async Task<Result> Handle(AddTeacherCommand request, CancellationToken cancellationToken)
            {
                if (request.TeacherModel?.ApplicationUser == null)
                {
                    return Result.Failure (_localizer.Format (LocalizationKey.ActionException, _localizer["Create"], _localizer["Teacher"], "ApplicationUser is null."));
                }
                var newPassword = Functions.GenerateSecurePassword ( );
                var info = AutoCodeConfig.Get (AutoCodeType.Teacher);

                // 1. Map và thêm ApplicationUser
                var appUserEntity = _mapper.Map<ApplicationUser> (request.TeacherModel.ApplicationUser);

                // 2. Sử dụng AutoCodeHelper để sinh UserCode và đảm bảo không trùng
                Result userResult = await AutoCodeHelper.CreateWithAutoCodeRetryAsync<Result> (
                    info,
                    async (code) =>
                    {
                        appUserEntity.UserCode = code;

                        // Sinh mới Guid cho ApplicationUser.Id, đảm bảo không trùng (hiếm gặp)
                        do
                        {
                            appUserEntity.Id = Guid.NewGuid ( );
                        }
                        while (await _context.ApplicationUsers.AnyAsync (x => x.Id == appUserEntity.Id, cancellationToken));


                        // Dùng IdentityService để tạo user (đúng chuẩn Identity)
                        Result result = await _identityService.CreateUserAsync (appUserEntity, newPassword);
                        return result;
                    },
                    (DbContext)_context // Ép kiểu sang DbContext nếu cần cho AutoCodeHelper
                );

                if (!userResult.Succeeded)
                {
                    return Result.Failure (_localizer.Format (LocalizationKey.ActionException, _localizer["Create"], _localizer["Teacher"], userResult.Errors));
                }

                appUserEntity = await _context.ApplicationUsers.Where (t => t.Email == appUserEntity.Email).FirstOrDefaultAsync ( );
                if (appUserEntity == null)
                {
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Teacher"]));
                }
                var teacherEntity = _mapper.Map<Domain.Entities.Teacher> (request.TeacherModel);
                teacherEntity.ApplicationUserId = appUserEntity.Id;
                teacherEntity.ApplicationUser = appUserEntity;
                await _context.Teachers.AddAsync (teacherEntity, cancellationToken);

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                if (success)
                {
                    var emailModel = new UserModelRequest
                    {
                        FullName = teacherEntity.ApplicationUser.FullName,
                        Email = teacherEntity.ApplicationUser.Email,
                        Password = newPassword // Mật khẩu mới được sinh ngẫu nhiên
                    };
                    try
                    {
                        var body = await _templateService.RenderTemplateAsync ("CreateNewTeacher", emailModel);

                        await _emailService.SendEmailAsync (teacherEntity.ApplicationUser.Email, _localizer[LocalizationKey.CreateNewTeacher], body);
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi gửi email nếu cần
                        Console.WriteLine ($"Error sending email: {Functions.GetFullExceptionMessage (ex)}");
                        throw new ApplicationException (_localizer.Format (LocalizationKey.ERR_SEND_EMAIL, _localizer["Teacher"], Functions.GetFullExceptionMessage (ex)));
                    }
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer["Teacher"]));
                }
                else
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Teacher"]));
            }
        }
    }
}