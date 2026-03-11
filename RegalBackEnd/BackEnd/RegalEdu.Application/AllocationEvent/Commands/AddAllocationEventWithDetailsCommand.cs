using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AllocationEvent.Commands
{
    // Command: Thêm AllocationEvent cùng AllocationDetailEvent
    public class AddAllocationEventWithDetailsCommand : IRequest<Result>
    {
        public required AllocationEventModel AllocationEventModel { get; set; }
    }

    // Handler: Xử lý logic thêm AllocationEvent + AllocationDetailEvent
    public class AddAllocationEventWithDetailsCommandHandler
        : IRequestHandler<AddAllocationEventWithDetailsCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public AddAllocationEventWithDetailsCommandHandler(
            IRegalEducationDbContext context,
            AutoMapper.IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(
            AddAllocationEventWithDetailsCommand request,
            CancellationToken cancellationToken)
        {
            if (_context is not DbContext dbContext)
                throw new InvalidOperationException (
                    _localizer[LocalizationKey.InvalidDbContextInstance]
                );

            var model = request.AllocationEventModel;

            // Kiểm tra trùng (năm + tháng)
            bool existsParent = await _context.AllocationEvents.AnyAsync (
                x => x.AllocationYear == model.AllocationYear &&
                     x.AllocationMonth == model.AllocationMonth &&
                     !x.IsDeleted,
                cancellationToken
            );

            if (existsParent)
            {
                return Result.Failure (
                    _localizer.Format (
                        LocalizationKey.ERR_DUPLICATE_VALUE,
                        $"AllocationEvent với năm {model.AllocationYear} và tháng {model.AllocationMonth} đã tồn tại."
                    )
                );
            }

            // Sinh mã tự động
            var info = AutoCodeConfig.Get(AutoCodeType.AllocationEvent);
            //info.Year = model.AllocationYear;
            //info.Month = model.AllocationMonth;
            var code = await AutoCodeHelper.GenerateCodeAsync(info, dbContext);

            // Ánh xạ sang entity
            var eventEntity = _mapper.Map<Domain.Entities.AllocationEvent> (model);
            //eventEntity.AllocationCode = code;

            using var transaction = await dbContext.Database.BeginTransactionAsync (cancellationToken);

            try
            {
                // 1️ Lưu bảng cha trước
                await _context.AllocationEvents.AddAsync (eventEntity, cancellationToken);
                await _context.SaveChangesAsync (cancellationToken);

                // 2️ Lưu bảng con (nếu có)
                if (model.AllocationDetails != null && model.AllocationDetails.Any ( ))
                {
                    // Kiểm tra trùng trong danh sách gửi lên (theo cặp CompanyId + EventId)
                    var duplicates = model.AllocationDetails
                        .GroupBy (d => new { d.CompanyId, d.EventId })
                        .Where (g => g.Count ( ) > 1)
                        .ToList ( );

                    if (duplicates.Any ( ))
                    {
                        var list = string.Join (", ", duplicates.Select (
                            g => $"(CompanyId: {g.Key.CompanyId}, EventId: {g.Key.EventId})"
                        ));

                        return Result.Failure (
                            _localizer.Format (
                                LocalizationKey.ERR_DUPLICATE_VALUE,
                                $"Trùng lặp trong danh sách AllocationDetailEvent: {list}"
                            )
                        );
                    }

                    // Ánh xạ sang entity và gán AllocationEventId
                    var detailEntities = model.AllocationDetails.Select (d =>
                    {
                        var detail = _mapper.Map<Domain.Entities.AllocationDetailEvent> (d);
                        detail.Id = Guid.NewGuid ( );
                        detail.AllocationEventId = eventEntity.Id;
                        // 🔹 Tính Budget = EventBudget * Quantity
                        //detail.Budget = eventEntity.EventBudget * d.Quantity;
                        // Budget ở bảng AllocationDetailEvent = Budget ở bảng AllocationEvent
                        detail.Budget = eventEntity.EventBudget;
                        // 🔹 Logic mặc định NoAllocation
                        // Mặc định  = 0 = uncheck là vẫn tính chỉ tiêu
                        detail.NoAllocation = NoAllocation.Unchecked;

                        // Lấy ngày thành lập chi nhánh
                        var company = _context.Companies.FirstOrDefault (c => c.Id == d.CompanyId);
                        if (company != null && company.EstablishmentDate.HasValue)
                        {
                            var est = company.EstablishmentDate.Value;
                            if (est.Year == model.AllocationYear && est.Month == model.AllocationMonth)
                            {
                                // Nếu chi nhánh thành lập cùng tháng phân bổ → đánh dấu "Không tính chỉ tiêu"
                                detail.NoAllocation = NoAllocation.Checked;
                            }
                        }
                        return detail;
                    }).ToList ( );

                    await _context.AllocationDetailEvents.AddRangeAsync (detailEntities, cancellationToken);
                    await _context.SaveChangesAsync (cancellationToken);
                }

                await transaction.CommitAsync (cancellationToken);

                return Result.Success (
                    _localizer.Format (
                        LocalizationKey.MSG_CREATE_SUCCESS,
                        $"{EntityName.AllocationEvent} và {EntityName.AllocationDetailEvent}"
                    )
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync (cancellationToken);
                return Result.Failure ($"Error: {ex.Message}");
            }
        }
    }
}
