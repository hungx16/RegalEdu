using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.AllocationEvent.Commands;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Application.AllocationEvent.Validators
{
    public class AddAllocationEventWithDetailsCommandValidator : AbstractValidator<AddAllocationEventWithDetailsCommand>
    {
        public AddAllocationEventWithDetailsCommandValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {
            // 1. Kiểm tra dữ liệu của AllocationEventModel (bảng AllocationEvent)
            RuleFor(x => x.AllocationEventModel)
                .SetValidator(new BaseAllocationEventModelValidator(localizer));

            // 2. Kiểm tra từng bản ghi AllocationDetailEvent đi kèm (bảng AllocationDetailEvent)
            RuleForEach(x => x.AllocationEventModel.AllocationDetails)
                .SetValidator(new BaseAllocationDetailEventModelValidator(localizer));

            // 3. Kiểm tra trùng năm + tháng (AllocationYear + AllocationMonth)
            RuleFor(x => new { x.AllocationEventModel.AllocationYear, x.AllocationEventModel.AllocationMonth })
                .MustAsync(async (model, cancellation) =>
                {
                    return !await dbContext.AllocationEvents
                        .AnyAsync(a =>
                            a.AllocationYear == model.AllocationYear &&
                            a.AllocationMonth == model.AllocationMonth &&
                            !a.IsDeleted,
                            cancellation);
                })
                .WithMessage((command, model) =>
                    localizer.Format(
                        LocalizationKey.ModelCodeAlreadyExists,
                        localizer[EntityName.AllocationEvent],
                        $"{model.AllocationMonth}/{model.AllocationYear}"
                    ));

            // 4. Kiểm tra mã phân bổ (AllocationCode) không bị trùng
            RuleFor(x => x.AllocationEventModel.AllocationCode)
                .MustAsync(async (code, cancellation) =>
                {
                    return !await dbContext.AllocationEvents
                        .AnyAsync(a => a.AllocationCode == code && !a.IsDeleted, cancellation);
                })
                .WithMessage((command, code) =>
                    localizer.Format(
                        LocalizationKey.ModelCodeAlreadyExists,
                        localizer[EntityName.AllocationEvent],
                        code
                    ));

            // 5. Kiểm tra các AllocationDetailEvent có trùng Company hay Event không
            RuleFor(x => x.AllocationEventModel.AllocationDetails)
                .Must(details =>
                {
                    if (details == null || !details.Any())
                        return false; // không được để trống

                    // Kiểm tra trùng CompanyId + EventId
                    var duplicate = details
                        .GroupBy(d => new { d.CompanyId, d.EventId })
                        .Any(g => g.Count() > 1);
                    return !duplicate;
                })
                .WithMessage(localizer["DuplicateCompanyOrEventInDetails"]);

            // 6. Kiểm tra Company, Region và Event phải hợp lệ          
            // Kiểm tra Company
            RuleForEach(x => x.AllocationEventModel.AllocationDetails)
                .MustAsync(async (parent, detail, cancellation) =>
                {
                    var allocationMonth = parent.AllocationEventModel.AllocationMonth;
                    var allocationYear = parent.AllocationEventModel.AllocationYear;

                    return await dbContext.Companies
                        .AnyAsync(c =>
                            c.Id == detail.CompanyId &&
                            c.Status == StatusType.Active &&
                            c.EstablishmentDate.HasValue &&
                            (
                                c.EstablishmentDate.Value.Year < allocationYear ||
                                (c.EstablishmentDate.Value.Year == allocationYear &&
                                 c.EstablishmentDate.Value.Month <= allocationMonth)
                            ),
                            cancellation);
                })
                .WithMessage(detail => localizer["InvalidCompany"]);

            // Kiểm tra Event
            RuleForEach(x => x.AllocationEventModel.AllocationDetails)
                .MustAsync(async (parent, detail, cancellation) =>
                {
                    return await dbContext.Events
                        .AnyAsync(e =>
                            e.Id == detail.EventId &&
                            e.Status == StatusType.Active &&
                            e.Category == EventCategory.Event,
                            cancellation);
                })
                .WithMessage(detail => localizer["InvalidEvent"]);

            // Kiểm tra Region
            RuleForEach(x => x.AllocationEventModel.AllocationDetails)
                .MustAsync(async (parent, detail, cancellation) =>
                {
                    var now = DateTime.UtcNow;

                    return await dbContext.LogRegionComs
                        .AnyAsync(lrc =>
                            lrc.CompanyId == detail.CompanyId &&
                            lrc.RegionId == detail.RegionId &&
                            lrc.StartedDate <= now &&
                            (lrc.EndDate == null || lrc.EndDate >= now),
                            cancellation);
                })
                .WithMessage(detail => localizer["InvalidRegion"]);

        }
    }
}
