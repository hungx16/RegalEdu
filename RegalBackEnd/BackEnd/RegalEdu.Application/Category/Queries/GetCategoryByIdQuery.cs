using System.Net.Mail;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Category.Queries
{
    public class GetCategoryByIdQuery : IRequest<Result<CategoryModel>>
    {
        public required string Id { get; set; }
        public CategoryType CategoryType { get; set; }

    }

    public class GetCategoriesByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetCategoriesByIdQueryHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result<CategoryModel>> Handle(GetCategoryByIdQuery request,CancellationToken cancellationToken)
        {
            // 1. Truy vấn Category theo Id
            // - Chỉ lấy bản ghi chưa bị xóa mềm (IsDeleted == false).
            // - AsNoTracking() để tăng hiệu năng (vì chỉ đọc, không cần tracking).
            var category = await _context.Categories
                .Include(x => x.LearningRoadMaps)
                .Include(x => x.Students)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id.ToString() == request.Id && !x.IsDeleted,
                    cancellationToken);

            // 2. Nếu không tìm thấy Category → trả về lỗi NotFound
            if (category == null)
            {
                var msg = _localizer.Format(
                    LocalizationKey.EntityWithIdNotFound,
                    _localizer[EntityName.Category], // Lấy tên entity từ localizer
                    request.Id);

                return Result<CategoryModel>.Failure(msg);
            }

            // 3. Nếu CategoryType không khớp với loại yêu cầu → trả về lỗi InvalidType
            if (category.CategoryType != (byte)request.CategoryType)
            {
                var msg = _localizer.Format(
                    LocalizationKey.InvalidCategoryType,
                    EntityName.Category,
                    request.CategoryType);

                return Result<CategoryModel>.Failure(msg);
            }

            // 4. Map entity Category → DTO CategoryModel
            var result = _mapper.Map<CategoryModel>(category);

            // 5. Trả về kết quả thành công
            return Result<CategoryModel>.Success(result);
        }

    }
}
