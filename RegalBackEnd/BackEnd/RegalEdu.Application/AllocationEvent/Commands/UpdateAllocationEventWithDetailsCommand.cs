using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AllocationEvent.Commands
{
    public class UpdateAllocationEventWithDetailsCommand : IRequest<Result>
    {
        public required AllocationEventModel AllocationEventModel { get; set; }
    }

    public class UpdateAllocationEventWithDetailsCommandHandler
        : IRequestHandler<UpdateAllocationEventWithDetailsCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateAllocationEventWithDetailsCommandHandler(
            IRegalEducationDbContext context,
            AutoMapper.IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(UpdateAllocationEventWithDetailsCommand request, CancellationToken cancellationToken)
        {
            if (_context is not DbContext dbContext)
                throw new InvalidOperationException (_localizer[LocalizationKey.InvalidDbContextInstance]);

            var model = request.AllocationEventModel;

            // Tải cha + con ở trạng thái tracking và CHỈ cho phép sửa khi đang Draft
            var existingEvent = await _context.AllocationEvents
                .Include (a => a.AllocationDetails)
                .FirstOrDefaultAsync (a =>
                        a.Id == model.Id &&
                        !a.IsDeleted &&
                        a.AllocationEventStatus == AllocationEventStatus.Draft,
                    cancellationToken);

            if (existingEvent == null)
                return Result.Failure (_localizer["AllocationEventNotFound"]);

            using var tx = await dbContext.Database.BeginTransactionAsync (cancellationToken);
            try
            {
                // ===== 1) Cập nhật scalar của bảng cha (KHÔNG map collection) =====
                existingEvent.AllocationCode = model.AllocationCode;
                existingEvent.AllocationMonth = model.AllocationMonth;
                existingEvent.AllocationYear = model.AllocationYear;
                existingEvent.EventBudget = model.EventBudget;
                existingEvent.AllocationEventStatus = model.AllocationEventStatus;

                // ===== 2) Đồng bộ bảng con =====
                var existingById = existingEvent.AllocationDetails.ToDictionary (d => d.Id);

                // 2.1 Update / Add
                if (model.AllocationDetails != null && model.AllocationDetails.Any ( ))
                {
                    foreach (var m in model.AllocationDetails)
                    {
                        // UPDATE nếu Id trùng
                        if (m.Id != Guid.Empty && existingById.TryGetValue ((Guid)m.Id, out var ent))
                        {
                            // Cập nhật các trường thay đổi
                            ent.Quantity = m.Quantity;
                            ent.CompanyId = m.CompanyId;
                            ent.RegionId = m.RegionId;
                            ent.EventId = m.EventId;

                            // Không cho phép "áp chỉ tiêu" nếu thành lập cùng tháng/năm phân bổ
                            var company = await _context.Companies.AsNoTracking ( )
                                .FirstOrDefaultAsync (c => c.Id == m.CompanyId, cancellationToken);

                            if (company?.EstablishmentDate is DateTime est &&
                                est.Year == model.AllocationYear && est.Month == model.AllocationMonth)
                            {
                                ent.NoAllocation = NoAllocation.Checked;
                            }
                            else
                            {
                                ent.NoAllocation = m.NoAllocation;
                            }

                            // Giữ Budget = EventBudget (1 chi nhánh)
                            ent.Budget = existingEvent.EventBudget;

                            // ❌ KHÔNG ép Modified — để EF tự phát hiện thay đổi
                        }
                        else
                        {
                            // ADD mới nếu không tìm thấy Id
                            var detail = _mapper.Map<Domain.Entities.AllocationDetailEvent> (m);
                            detail.Id = detail.Id == Guid.Empty ? Guid.NewGuid ( ) : detail.Id;
                            detail.AllocationEventId = existingEvent.Id;

                            // Logic NoAllocation theo tháng thành lập
                            var company = await _context.Companies.AsNoTracking ( )
                                .FirstOrDefaultAsync (c => c.Id == m.CompanyId, cancellationToken);

                            if (company?.EstablishmentDate is DateTime est &&
                                est.Year == model.AllocationYear && est.Month == model.AllocationMonth)
                            {
                                detail.NoAllocation = NoAllocation.Checked;
                            }

                            detail.Budget = existingEvent.EventBudget;

                            await _context.AllocationDetailEvents.AddAsync (detail, cancellationToken);
                            existingEvent.AllocationDetails.Add (detail);
                        }
                    }
                }

                // 2.2 (Tuỳ chọn) Remove những detail đang có nhưng không còn trong request
                // Nếu không muốn xoá, có thể comment toàn bộ khối dưới.
                if (model.AllocationDetails != null)
                {
                    var requestIds = new HashSet<Guid> (model.AllocationDetails
                    .Where (x => x.Id.HasValue && x.Id.Value != Guid.Empty)
                    .Select (x => x.Id.Value));
                    var toRemove = existingEvent.AllocationDetails
                        .Where (d => !requestIds.Contains (d.Id))
                        .ToList ( );

                    if (toRemove.Count > 0)
                    {
                        _context.AllocationDetailEvents.RemoveRange (toRemove);
                    }
                }
                if (model.HistoryChanges is { Count: > 0 })
                {
                    foreach (var change in model.HistoryChanges)
                    {

                        var history = new AllocationEventHistory
                        {
                            Id = Guid.NewGuid ( ),
                            AllocationEventId = existingEvent.Id,
                            ActionName = change.ActionName,
                            Description = change.Description,
                            TargetName = change.TargetName,
                        };
                        await _context.AllocationEventHistories.AddAsync (history, cancellationToken);
                    }
                }

                // ===== 3) Lưu =====
                // Để EF tự xác định những entity nào thay đổi, không ép Modified.
                await _context.SaveChangesAsync (cancellationToken);
                await tx.CommitAsync (cancellationToken);

                return Result.Success (
                    _localizer.Format (
                        LocalizationKey.MSG_UPDATE_SUCCESS,
                        $"{EntityName.AllocationEvent} và {EntityName.AllocationDetailEvent}"
                    )
                );
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync (cancellationToken);
                // Trả thông điệp dễ hiểu hơn cho người dùng
                return Result.Failure (_localizer["DataChangedRetry"]); // key dịch: “Dữ liệu đã thay đổi hoặc bị xoá. Vui lòng tải lại và thử lại.”
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync (cancellationToken);
                return Result.Failure ($"Error: {ex.Message}{(ex.InnerException != null ? $" Inner: {ex.InnerException.Message}" : "")}");
            }
        }
    }
}
