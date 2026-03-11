using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.HolidayType.Commands
{
    public class AddHolidayTypeCommand : IRequest<Result>
    {
        public required CategoryModel CategoryModel { get; set; }
        public AutoCodeType AutoCodeType { get; set; }
        public class AddAgeGroupCommandHandler : IRequestHandler<AddHolidayTypeCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly AutoMapper.IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public AddAgeGroupCommandHandler(IRegalEducationDbContext context, AutoMapper.IMapper mapper, ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(AddHolidayTypeCommand request, CancellationToken cancellationToken)
            {
                if (_context is not DbContext dbContext)
                    throw new InvalidOperationException (_localizer[LocalizationKey.InvalidDbContextInstance]);

                var info = AutoCodeConfig.Get (request.AutoCodeType);

                var result = await AutoCodeHelper.CreateWithAutoCodeRetryAsync (
                    info,
                    async (code) =>
                    {
                        var model = request.CategoryModel;
                        if (model == null)
                            return await Fail (_localizer["CategoryModelRequired"]);

                        // 1. Required fields
                        if (string.IsNullOrWhiteSpace (model.CategoryName))
                            return await Fail (_localizer["HolidayTypeRequired"]);

                        if (!string.IsNullOrEmpty (model.Description) && model.Description.Length > 1000)
                            return await Fail (_localizer.Format ("HolidayTypeDescriptionMaxLength", 1000));

                        // 5. Unique check in DB
                        bool exists = await _context.Categories.AnyAsync (
                            d => d.CategoryName.ToLower ( ) == model.CategoryName.ToLower ( )
                                 && d.CategoryType == (byte)CategoryType.HolidayType
                                 && !d.IsDeleted,
                            cancellationToken);

                        if (exists)
                        {
                            return Result.Failure (
                                _localizer.Format (
                                    LocalizationKey.ModelNameAlreadyExists,
                                    "HolidayType",
                                    model.CategoryName
                                )
                            );
                        }

                        // 6. Mapping + save
                        var category = _mapper.Map<Domain.Entities.Category> (model);
                        category.CategoryCode = code; // Auto-generated
                        category.CategoryType = (byte)CategoryType.HolidayType;

                        await _context.Categories.AddAsync (category, cancellationToken);
                        var success = await _context.SaveChangesAsync (cancellationToken) > 0;

                        if (success)
                            return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, "HolidayType"));

                        return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, "HolidayType"));
                    },
                    dbContext
                );

                return result;
            }

            private Task<Result> Fail(string message)
                => Task.FromResult (Result.Failure (message));


        }
    }
}





