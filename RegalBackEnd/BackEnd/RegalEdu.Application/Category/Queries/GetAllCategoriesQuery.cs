using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Category.Queries
{
    public class GetAllCategoriesQuery : IRequest<Result<List<CategoryModel>>>
    {
        public CategoryType CategoryType { get; set; }
    }
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<List<CategoryModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllCategoriesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<List<CategoryModel>>> Handle(GetAllCategoriesQuery request,CancellationToken cancellationToken)
        {
            // 1. Truy vấn dữ liệu Category từ DbContext
            // - Đã có Global Query Filter cho IsDeleted (trong BaseEntity),
            //   nên không cần viết điều kiện !c.IsDeleted.
            // - Include(t => t.LearningRoadmaps) để eager load quan hệ.
            // - AsNoTracking() để tăng hiệu năng (chỉ đọc, không cần tracking).
            var categories = await _context.Categories
                .Include(c => c.LearningRoadMaps)
                .Include(c=>c.Students)
                .Where(c => c.CategoryType == (byte)request.CategoryType)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // 2. Map từ entity sang DTO (CategoryModel)            
            var result = _mapper.Map<List<CategoryModel>>(categories);

            // 3. Trả về kết quả thành công với danh sách CategoryModel
            return Result<List<CategoryModel>>.Success(result);
        }

    }
}
