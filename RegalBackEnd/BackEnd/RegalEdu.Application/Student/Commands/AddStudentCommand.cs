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
using System.Threading;

namespace RegalEdu.Application.Student.Commands
{
    public class AddStudentCommand : IRequest<Result>
    {
        public required StudentModel StudentModel { get; set; }
    }

    public class AddStudentCommandHandler : IRequestHandler<AddStudentCommand, Result>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;
        public AddStudentCommandHandler(IRegalEducationDbContext db, IMapper mapper, ILocalizationService localizer, IIdentityService identityService, 
            IEmailService emailService, IEmailTemplateService emailTemplateService)
        {
            _db = db; _mapper = mapper; _localizer = localizer;
            _identityService = identityService;
            _emailService = emailService;
            _templateService = emailTemplateService;
        }

        public async Task<Result> Handle(AddStudentCommand request, CancellationToken ct)
        {
            var m = request.StudentModel;
            //tạo user
            if (m?.ApplicationUser == null)
            {
                return Result.Failure(_localizer.Format(LocalizationKey.ActionException, _localizer["Create"], _localizer["Employee"], "ApplicationUser is null."));
            }
            var newPassword = Functions.GenerateSecurePassword();
            var info = AutoCodeConfig.Get(AutoCodeType.Student);

            // 1. Map và thêm ApplicationUser
            var appUserEntity = _mapper.Map<ApplicationUser>(m.ApplicationUser);

            // 2. Sử dụng AutoCodeHelper để sinh UserCode và đảm bảo không trùng
            Result userResult = await AutoCodeHelper.CreateWithAutoCodeRetryAsync<Result>(
                info,
                async (code) =>
                {
                    appUserEntity.UserCode = code;

                    // Sinh mới Guid cho ApplicationUser.Id, đảm bảo không trùng (hiếm gặp)
                    do
                    {
                        appUserEntity.Id = Guid.NewGuid();
                    }
                    while (await _db.ApplicationUsers.AnyAsync(x => x.Id == appUserEntity.Id, ct));


                    // Dùng IdentityService để tạo user (đúng chuẩn Identity)
                    Result result = await _identityService.CreateUserAsync(appUserEntity, newPassword);
                    return result;
                },
                (DbContext)_db // Ép kiểu sang DbContext nếu cần cho AutoCodeHelper
            );

            if (!userResult.Succeeded)
            {
                return Result.Failure(_localizer.Format(LocalizationKey.ActionException, _localizer["Create"], _localizer["Student"], userResult.Errors));
            }

            appUserEntity = await _db.ApplicationUsers.Where(t => t.Email == appUserEntity.Email).FirstOrDefaultAsync();
            if (appUserEntity == null)
            {
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Student"]));
            }
            

            //kết thúc tạo user

            var entity = _mapper.Map<Domain.Entities.Student>(m);
            //tạo tài khoản người dùng
            entity.ApplicationUserId = appUserEntity.Id;
            entity.ApplicationUser = appUserEntity;
            // Map collections if provided
            if (m.Contacts != null)
                entity.Contacts = _mapper.Map<List<Domain.Entities.Contact>>(m.Contacts);

            if (m.StudentActivity != null)
                entity.StudentActivity = _mapper.Map<List<Domain.Entities.StudentActivity>>(m.StudentActivity);

            if (m.StudentNote != null)
                entity.StudentNote = _mapper.Map<List<Domain.Entities.StudentNote>>(m.StudentNote);

            if (m.StudentCourse != null)
                entity.StudentCourse = _mapper.Map<List<Domain.Entities.StudentCourse>>(m.StudentCourse);

            if (m.Enrollments != null)
                entity.Enrollments = _mapper.Map<List<Domain.Entities.Enrollment>>(m.Enrollments);

            await _db.Students.AddAsync(entity, ct);
            var ok = await _db.SaveChangesAsync(ct) > 0;
            if (ok)
            {
                var emailModel = new UserModelRequest
                {
                    FullName = entity.ApplicationUser.FullName,
                    Email = entity.ApplicationUser.Email,
                    Password = newPassword // Mật khẩu mới được sinh ngẫu nhiên
                };
                try
                {
                    var body = await _templateService.RenderTemplateAsync("CreateNewStudent", emailModel);

                    await _emailService.SendEmailAsync(entity.ApplicationUser.Email, _localizer[LocalizationKey.CreateNewStudent], body);
                }
                catch (Exception ex)
                {
                    // Log lỗi gửi email nếu cần
                    Console.WriteLine($"Error sending email: {Functions.GetFullExceptionMessage(ex)}");
                    throw new ApplicationException(_localizer.Format(LocalizationKey.ERR_SEND_EMAIL, _localizer["Student"], Functions.GetFullExceptionMessage(ex)));
                }
                return Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, _localizer["Student"]));
            }
            else
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Student"]));
            //return ok
            //    ? Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Student))
            //    : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Student));
        }
    }
}
