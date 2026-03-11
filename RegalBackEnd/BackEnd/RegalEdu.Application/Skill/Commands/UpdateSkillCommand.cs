using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Skill.Commands
{
    public class UpdateSkillCommand : IRequest<Result>
    {
        public required CategoryModel CategoryModel { get; set; }
        // Yêu cầu lớp con override để chỉ định loại CategoryType cụ thể         
        public virtual CategoryType? RequiredCategoryType => CategoryType.Skill;
    }

    public class UpdateSkillHandler : IRequestHandler<UpdateSkillCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly ICurrentUserService _currentUserService;

        public UpdateSkillHandler(IRegalEducationDbContext context, AutoMapper.IMapper mapper, ILocalizationService localizer, ICurrentUserService currentUserService)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            _currentUserService = currentUserService ?? throw new ArgumentNullException (nameof (currentUserService));
        }

        public async Task<Result> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
        {
            var model = request.CategoryModel;// Dữ liệu mà client gửi lên

            // 1. Kiểm tra tồn tại
            var category = await _context.Categories.FirstOrDefaultAsync (
                x => x.Id == model.Id,
                cancellationToken); //category là dữ liệu entity ánh xạ từ csdl

            if (category == null)
                return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, "Skill"));

            // 2. Kiểm tra loại CategoryType trong csdl có là SKill hay ko  
            if (category.CategoryType != (byte)CategoryType.Skill)
                return await Fail (_localizer.Format ("InvalidCategoryType", _localizer[EntityName.Category], CategoryType.Skill));

            // 3. Validation dữ liệu đầu vào            

            if (string.IsNullOrWhiteSpace (model.CategoryCode))
                return await Fail (_localizer["AgeGroupCodeRequired"]);

            if (model.CategoryCode.Length > 10)
                return await Fail (_localizer.Format ("AgeGroupCodeMaxLength", 10));

            //if (!Regex.IsMatch (model.CategoryCode, @"^[A-Z]+$"))
            //    return await Fail (_localizer["SkillCodeInvalidFormat"]);

            if (string.IsNullOrWhiteSpace (model.CategoryName))
                return await Fail (_localizer["SkillNameRequired"]);

            if (model.CategoryCode.Length > 10)
                return await Fail (_localizer.Format ("SkillCodeMaxLength", 10));

            if (model.CategoryName.Length > 200)
                return await Fail (_localizer.Format ("SkillNameMaxLength", 200));

            if (!string.IsNullOrEmpty (model.Description) && model.Description.Length > 1000)
                return await Fail (_localizer.Format ("SkillDescriptionMaxLength", 1000));

            // 4. Kiểm tra trùng lặp trong DB
            bool isCodeExists = await _context.Categories.AnyAsync (
                d => d.CategoryCode == model.CategoryCode && d.Id != model.Id && !d.IsDeleted,
                cancellationToken);

            if (isCodeExists)
                return await Fail (_localizer.Format (LocalizationKey.ModelCodeAlreadyExists, _localizer["Skill"], model.CategoryCode));

            bool isNameExists = await _context.Categories.AnyAsync (
                d => d.CategoryName == model.CategoryName && d.Id != model.Id && !d.IsDeleted,
                cancellationToken);

            if (isNameExists)
                return await Fail (_localizer.Format (LocalizationKey.ModelNameAlreadyExists, _localizer["Skill"], model.CategoryName));

            // 5. Nếu qua hết validation -> cập nhật entity
            category.CategoryCode = model.CategoryCode;
            category.CategoryName = model.CategoryName;
            category.Description = model.Description;
            category.Status = model.Status;

            var success = await _context.SaveChangesAsync (cancellationToken) >= 0;//Nếu ko thay đổi gì cũng coi là update thành công
            if (success)
                return Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["Skill"]));
            else
                return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Skill"]));
        }

        private Task<Result> Fail(string message)
            => Task.FromResult (Result.Failure (message));

    }
}
