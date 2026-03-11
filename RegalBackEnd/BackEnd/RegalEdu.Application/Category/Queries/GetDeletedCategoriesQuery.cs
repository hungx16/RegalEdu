using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Category.Queries
{
    public class GetDeletedCategoriesQuery : IRequest<Result<List<CategoryModel>>>
    {
        public CategoryType CategoryType { get; set; }
    }

    public class GetDeletedCategoriesQueryHandler : IRequestHandler<GetDeletedCategoriesQuery, Result<List<CategoryModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetDeletedCategoriesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
        }

        public async Task<Result<List<CategoryModel>>> Handle(
     GetDeletedCategoriesQuery request,
     CancellationToken cancellationToken)
        {
            // 1. Truy vấn danh sách Category đã bị xóa mềm (IsDeleted == true)
            // - IgnoreQueryFilters(): bỏ qua Global Query Filter để lấy cả bản ghi đã xóa
            // - Lọc theo CategoryType đúng với yêu cầu
            // - AsNoTracking(): chỉ đọc dữ liệu, không cần EF Core tracking
            var categories = await _context.Categories
                .IgnoreQueryFilters()
                .Where(c => c.CategoryType == (byte)request.CategoryType && c.IsDeleted)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // 2. Map từ Entity → DTO (CategoryModel)           
            var result = _mapper.Map<List<CategoryModel>>(categories);

            // 3. Trả về kết quả thành công cùng danh sách CategoryModel
            return Result<List<CategoryModel>>.Success(result);
        }

    }
}
