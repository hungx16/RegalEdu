using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Category.Commands
{
    public class DeleteListCategoriesCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
        public CategoryType CategoryType { get; set; }
    }

    public class DeleteListCategoriesCommandHandler : IRequestHandler<DeleteListCategoriesCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<DeleteListCategoriesCommandHandler> _logger;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDeleteService;

        public DeleteListCategoriesCommandHandler(IRegalEducationDbContext context, ILogger<DeleteListCategoriesCommandHandler> logger, ILocalizationService localizer, ISoftDeleteService softDeleteService)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            _softDeleteService = softDeleteService ?? throw new ArgumentNullException (nameof (softDeleteService));
        }

        public async Task<Result> Handle(DeleteListCategoriesCommand request, CancellationToken cancellationToken)
        {
            // 1. Kiểm tra đầu vào: nếu danh sách Id null hoặc rỗng thì báo lỗi luôn
            if (request.ListIds == null || !request.ListIds.Any ( ))
                return Result.Failure (_localizer.Format (
                    LocalizationKey.NoModelToDelete, EntityName.Category));

            // 2. Khởi tạo bộ đếm và danh sách thông điệp lỗi
            int successCount = 0;
            int failCount = 0;
            var failMessages = new List<string> ( );

            // 3. Hàm tiện ích để gom logic báo lỗi lặp lại
            void AddFail(string message)
            {
                failCount++;
                failMessages.Add (message);
                _logger.LogWarning (message); // log cảnh báo
            }

            // 4. Duyệt từng Id trong danh sách yêu cầu xóa
            foreach (var id in request.ListIds)
            {
                // 4.1 Tìm category theo Id
                var category = _context.Categories.FirstOrDefault (x => x.Id.ToString ( ) == id);

                // Nếu không tìm thấy → báo lỗi và bỏ qua
                if (category == null)
                {
                    AddFail (_localizer.Format (
                        LocalizationKey.EntityWithIdNotFound,
                        EntityName.Category, id));
                    continue;
                }

                // 4.2 Kiểm tra CategoryType có khớp với loại yêu cầu không
                if (category.CategoryType != (int)request.CategoryType)
                {
                    AddFail (_localizer.Format (
                        LocalizationKey.InvalidCategoryType,
                        EntityName.Category, request.CategoryType));
                    continue;
                }

                // 4.3 Thực hiện soft delete (xóa mềm) đệ quy
                var result = await _softDeleteService.RecursiveSoftDelete (
                    category.Id, typeof (Domain.Entities.Category));

                // Nếu xóa thành công thì tăng successCount
                if (result.Succeeded)
                {
                    successCount++;
                }
                else
                {
                    // Nếu xóa thất bại → báo lỗi chi tiết
                    AddFail (_localizer.Format (
                        LocalizationKey.EntityDeleteFailed,
                        _localizer[EntityName.Category], category.CategoryName, result.Errors));
                }
            }

            // 5. Sau khi duyệt xong, tổng hợp thông điệp kết quả
            var summaryMsg = _localizer.Format (
                LocalizationKey.MSG_DELETE_RESULT,
              _localizer[EntityName.Category], successCount, failCount);
            // Ví dụ: "Xóa Category: 3 thành công, 2 thất bại."

            // Nếu có lỗi chi tiết thì nối thêm vào cuối thông điệp
            if (failMessages.Any ( ))
            {
                summaryMsg += "\n" + string.Join ("\n", failMessages);
            }

            // 6. Trả về kết quả: 
            // - Nếu có ít nhất 1 thành công → Success
            // - Nếu tất cả thất bại → Failure
            return successCount > 0
                ? Result.Success (summaryMsg)
                : Result.Failure (summaryMsg);
        }
    }
}
