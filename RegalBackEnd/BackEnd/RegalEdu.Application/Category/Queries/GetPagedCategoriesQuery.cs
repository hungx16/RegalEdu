using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Category.Queries
{
    public class CategoryQuery
    {
        public string? CategoryCode { get; set; }
        public string? CategoryName { get; set; }
        [BindNever]
        public byte? CategoryType { get; set; }
        public byte? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedCategoriesQuery : IRequest<Result<PagedResult<CategoryModel>>>
    {
        public CategoryQuery? CategoryQuery { get; set; }
    }

    public class GetPagedCategoriesQueryHandler : IRequestHandler<GetPagedCategoriesQuery, Result<PagedResult<CategoryModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _pagingOptions;
        private readonly IMapper _mapper;
        public GetPagedCategoriesQueryHandler(IRegalEducationDbContext context, PagingOptions pagingOptions, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<PagedResult<CategoryModel>>> Handle(
     GetPagedCategoriesQuery request,
     CancellationToken cancellationToken)
        {
            // 1. Kiểm tra đầu vào: CategoryQuery không được null
            if (request.CategoryQuery == null)
            {
                throw new ArgumentNullException (nameof (request.CategoryQuery));
            }

            // 2. Tạo query cơ bản cho Category
            // - Include LearningRoadmaps để lấy thêm dữ liệu liên quan
            // - AsNoTracking() để tối ưu hiệu năng (chỉ đọc)
            var query = _context.Categories

                .Include(c => c.LearningRoadMaps)
                .Include(c => c.Students)
                .AsNoTracking();

            // 3. Áp dụng các điều kiện lọc theo CategoryQuery

            // 3.1 Lọc theo mã CategoryCode (nếu có nhập)
            if (!string.IsNullOrWhiteSpace (request.CategoryQuery.CategoryCode))
            {
                query = query.Where (c => c.CategoryCode.Contains (request.CategoryQuery.CategoryCode));
            }

            // 3.2 Lọc theo tên CategoryName (nếu có nhập)
            if (!string.IsNullOrWhiteSpace (request.CategoryQuery.CategoryName))
            {
                query = query.Where (c => c.CategoryName.Contains (request.CategoryQuery.CategoryName));
            }

            // 3.3 Lọc theo CategoryType (nếu có giá trị)
            if (request.CategoryQuery.CategoryType.HasValue)
            {
                query = query.Where (c => c.CategoryType == request.CategoryQuery.CategoryType.Value);
            }

            // 4. Lấy tổng số bản ghi (sau khi đã lọc)
            int totalRecords = await query.CountAsync (cancellationToken);

            // 5. Đảm bảo PageSize có giá trị mặc định (tránh null hoặc 0)
            request.CategoryQuery.PageSize = _pagingOptions.DefaultPageSize;

            // 6. Thực hiện phân trang + sắp xếp theo CreatedAt (mới nhất trước)
            var paged = await query
                .OrderByDescending (c => c.CreatedAt)
                .Skip ((request.CategoryQuery.Page - 1) * request.CategoryQuery.PageSize)
                .Take (request.CategoryQuery.PageSize)
                .ToListAsync (cancellationToken);

            // 7. Map dữ liệu entity → DTO (CategoryModel)
            var result = paged
                .Select (c => _mapper.Map<CategoryModel> (c))
                .ToList ( );

            // 8. Tạo đối tượng PagedResult để trả về
            var pagedResult = new PagedResult<CategoryModel>
            {
                Items = result,
                Total = totalRecords
            };

            // 9. Trả về kết quả thành công
            return Result<PagedResult<CategoryModel>>.Success (pagedResult);
        }
    }
}
