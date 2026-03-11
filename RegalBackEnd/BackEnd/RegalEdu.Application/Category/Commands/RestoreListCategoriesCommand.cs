using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Category.Commands
{
    public class RestoreListCategoriesCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
        public CategoryType CategoryType { get; set; }
    }

    public class RestoreListCategoriesCommandHandler : IRequestHandler<RestoreListCategoriesCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<RestoreListCategoriesCommandHandler> _logger;
        private readonly ILocalizationService _localizer;

        public RestoreListCategoriesCommandHandler(IRegalEducationDbContext context, ILogger<RestoreListCategoriesCommandHandler> logger, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(RestoreListCategoriesCommand request, CancellationToken cancellationToken)
        {
            // 1. Kiểm tra input: nếu danh sách Id null hoặc rỗng thì trả lỗi luôn
            if (request.ListIds == null || !request.ListIds.Any ( ))
            {
                return Result.Failure (_localizer.Format (
                    LocalizationKey.NoModelToRestore, EntityName.Category));
            }

            // 2. Khởi tạo bộ đếm và danh sách lưu thông điệp lỗi
            int successCount = 0;
            int failCount = 0;
            var failMessages = new List<string> ( );

            // 3. Hàm tiện ích gom lặp logic báo lỗi
            void AddFail(string message)
            {
                failCount++;
                failMessages.Add (message);
                _logger.LogWarning (message);
            }

            // 4. Duyệt từng Id trong danh sách để xử lý khôi phục
            foreach (var id in request.ListIds)
            {
                // 4.1 Truy vấn lấy category, kể cả đã bị soft delete (IgnoreQueryFilters)
                var category = await _context.Categories
                    .IgnoreQueryFilters ( )
                    .FirstOrDefaultAsync (x => x.Id.ToString ( ) == id, cancellationToken);

                // Nếu không tìm thấy entity
                if (category == null)
                {
                    AddFail (_localizer.Format (
                        LocalizationKey.EntityWithIdNotFound,
                        EntityName.Category, id));
                    continue;
                }

                // 4.2 Kiểm tra CategoryType có khớp với loại yêu cầu hay không
                if (category.CategoryType != (byte)request.CategoryType)
                {
                    AddFail (_localizer.Format (
                        LocalizationKey.InvalidCategoryType,
                        EntityName.Category, request.CategoryType));
                    continue;
                }

                // 4.3 Kiểm tra entity có đang bị xóa mềm hay không
                if (!category.IsDeleted)
                {
                    AddFail (_localizer.Format (
                        LocalizationKey.EntityNotDeleted,
                        EntityName.Category, category.Id));
                    continue;
                }

                // 4.4 Kiểm tra trùng mã (CategoryCode) với các entity khác chưa bị xóa
                bool codeExists = await _context.Categories.AnyAsync (
                    x => x.CategoryCode == category.CategoryCode
                      && x.Id != category.Id
                      && !x.IsDeleted,
                    cancellationToken);

                if (codeExists)
                {
                    AddFail (_localizer.Format (
                        LocalizationKey.ModelCodeAlreadyExists,
                        EntityName.Category, category.CategoryCode));
                    continue;
                }

                // 4.5 Kiểm tra trùng tên (CategoryName) với các entity khác chưa bị xóa
                bool nameExists = await _context.Categories.AnyAsync (
                    x => x.CategoryName == category.CategoryName
                      && x.Id != category.Id
                      && !x.IsDeleted,
                    cancellationToken);

                if (nameExists)
                {
                    AddFail (_localizer.Format (
                        LocalizationKey.ModelNameAlreadyExists,
                        EntityName.Category, category.CategoryName));
                    continue;
                }

                // 4.6 Thực hiện khôi phục (bỏ cờ IsDeleted và clear thông tin xóa)
                category.IsDeleted = false;
                // category.DeletedAt = null;
                // category.DeletedBy = null;

                // Đánh dấu cập nhật trong DbContext
                _context.Categories.Update (category);
                successCount++;
            }

            // 5. Lưu thay đổi xuống DB
            var dbResult = await _context.SaveChangesAsync (cancellationToken) > 0;

            // 6. Ghép thông điệp tổng kết
            string summaryMsg = _localizer.Format (
                LocalizationKey.MSG_RESTORE_RESULT,
                EntityName.Category,
                successCount,
                failCount);
            // Ví dụ: "Khôi phục Category: 3 thành công, 2 thất bại."

            // Nếu có lỗi chi tiết thì nối thêm vào cuối thông điệp
            if (failMessages.Any ( ))
            {
                summaryMsg += " " + string.Join (" ", failMessages);
            }

            // 7. Trả về kết quả:
            // - Nếu có ít nhất một entity khôi phục thành công và DB lưu thành công → Success
            // - Ngược lại → Failure
            return (dbResult && successCount > 0)
                ? Result.Success (summaryMsg)
                : Result.Failure (summaryMsg);
        }

    }
}
