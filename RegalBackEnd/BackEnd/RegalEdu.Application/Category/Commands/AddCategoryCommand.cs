using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Category.Commands
{
    public class AddCategoryCommand : IRequest<Result>
    {
        public required CategoryModel CategoryModel { get; set; }
        public AutoCodeType AutoCodeType { get; set; }
    }

    public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public AddCategoryCommandHandler(IRegalEducationDbContext context, AutoMapper.IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            var info = AutoCodeConfig.Get (request.AutoCodeType);
            // Ensure _context is not null before passing it to the method
            if (_context is not DbContext dbContext)
            {
                throw new InvalidOperationException (_localizer[LocalizationKey.InvalidDbContextInstance]);
            }

            var result = await AutoCodeHelper.CreateWithAutoCodeRetryAsync (
                info,
                async (code) =>
                {
                    var categories = _mapper.Map<Domain.Entities.Category> (request.CategoryModel);
                    categories.CategoryCode = code;
                    categories.CategoryType = (byte)request.CategoryModel.CategoryType; // Gán CategoryType

                    await _context.Categories.AddAsync (categories, cancellationToken);
                    var success = await _context.SaveChangesAsync (cancellationToken) > 0;

                    if (success)
                    {
                        return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Category));
                    }
                    else
                    {
                        return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Category));
                    }
                },
                dbContext // Pass the validated DbContext instance
            );
            return result;
        }
    }
}
