using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Skill.Commands
{
    public class AddSkillCommand : IRequest<Result>
    {
        public required CategoryModel CategoryModel { get; set; }
        public class AddSkillCommandHandler : IRequestHandler<AddSkillCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly AutoMapper.IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public AddSkillCommandHandler(IRegalEducationDbContext context, AutoMapper.IMapper mapper, ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(AddSkillCommand request, CancellationToken cancellationToken)
            {
                if (_context is not DbContext dbContext)
                    throw new InvalidOperationException (_localizer[LocalizationKey.InvalidDbContextInstance]);

                var model = request.CategoryModel;
                if (model == null)
                    return await Fail (_localizer["CategoryModelRequired"]);

                // 1. Required fields
                if (string.IsNullOrWhiteSpace (model.CategoryCode))
                    return await Fail (_localizer["SkillCodeRequired"]);

                if (string.IsNullOrWhiteSpace (model.CategoryName))
                    return await Fail (_localizer["SkillNameRequired"]);

                // 2. Validation SkillCode format (chỉ A-Z, không số)
                //if (!Regex.IsMatch (model.CategoryCode, @"^[A-Z]+$"))
                //    return await Fail (_localizer["SkillCodeInvalidFormat"]);
                // ví dụ message: "SkillCode chỉ được chứa chữ in hoa, không số và ký tự đặc biệt"

                // 3. Length validations
                if (model.CategoryCode.Length > 10)
                    return await Fail (_localizer.Format ("SkillCodeMaxLength", 10));

                if (model.CategoryName.Length > 200)
                    return await Fail (_localizer.Format ("SkillNameMaxLength", 200));

                if (!string.IsNullOrEmpty (model.Description) && model.Description.Length > 1000)
                    return await Fail (_localizer.Format ("SkillDescriptionMaxLength", 1000));

                // 4. Unique check in DB
                bool existsName = await _context.Categories.AnyAsync (
                    d => (d.CategoryName.ToLower ( ) == model.CategoryName.ToLower ( ))
                         && d.CategoryType == (byte)CategoryType.Skill
                         && !d.IsDeleted,
                    cancellationToken);
                bool existsCode = await _context.Categories.AnyAsync (
                    d => (d.CategoryCode.ToLower ( ) == model.CategoryCode.ToLower ( ))
                         && d.CategoryType == (byte)CategoryType.Skill
                         && !d.IsDeleted,
                    cancellationToken);

                if (existsName)
                {
                    return Result.Failure (
                        _localizer.Format (
                            LocalizationKey.ModelNameAlreadyExists,
                            _localizer["Skill"],
                            $"{model.CategoryName}"
                        )
                    );
                }

                if (existsCode)
                {
                    return Result.Failure (
                        _localizer.Format (
                            LocalizationKey.ModelCodeAlreadyExists,
                              _localizer["Skill"],
                            $"{model.CategoryCode}"
                        )
                    );
                }
                // 5. Mapping + save
                var category = _mapper.Map<Domain.Entities.Category> (model);
                category.CategoryType = (byte)CategoryType.Skill;

                await _context.Categories.AddAsync (category, cancellationToken);
                var success = await _context.SaveChangesAsync (cancellationToken) > 0;

                if (success)
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer["Skill"]));

                return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Skill"]));
            }

            private Task<Result> Fail(string message)
                => Task.FromResult (Result.Failure (message));

        }
    }
}





