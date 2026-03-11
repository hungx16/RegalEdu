using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.AllocationEvent.Commands;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Application.AllocationEvent.Validators
{
    public class UpdateAllocationEventWithDetailsCommandValidator : AbstractValidator<UpdateAllocationEventWithDetailsCommand>
    {
        public UpdateAllocationEventWithDetailsCommandValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {
            // 1. Kiểm tra dữ liệu AllocationEventModel
            RuleFor (x => x.AllocationEventModel)
                .SetValidator (new BaseAllocationEventModelValidator (localizer));

            // 2. Kiểm tra AllocationDetailEventModel
            RuleForEach (x => x.AllocationEventModel.AllocationDetails)
                .SetValidator (new BaseAllocationDetailEventModelValidator (localizer));

            // 3. Kiểm tra AllocationEvent có tồn tại
            RuleFor (x => x.AllocationEventModel.Id)
                .MustAsync (async (id, cancellation) =>
                {
                    return await dbContext.AllocationEvents.AnyAsync (a => a.Id == id && !a.IsDeleted, cancellation);
                })
                .WithMessage (localizer["AllocationEventNotFound"]);



            // 4. Kiểm tra trùng năm + tháng (ngoại trừ bản ghi hiện tại)
            RuleFor (x => new { x.AllocationEventModel.AllocationYear, x.AllocationEventModel.AllocationMonth, x.AllocationEventModel.Id })
                .MustAsync (async (model, cancellation) =>
                {
                    return !await dbContext.AllocationEvents
                        .AnyAsync (a =>
                            a.Id != model.Id &&
                            a.AllocationYear == model.AllocationYear &&
                            a.AllocationMonth == model.AllocationMonth &&
                            !a.IsDeleted,
                            cancellation);
                })
                .WithMessage ((command, model) =>
                    localizer.Format (
                        LocalizationKey.ModelCodeAlreadyExists,
                        localizer[EntityName.AllocationEvent],
                        $"{model.AllocationMonth}/{model.AllocationYear}"
                    ));

            // 5. Kiểm tra trùng mã phân bổ (AllocationCode) ngoại trừ bản ghi hiện tại
            RuleFor (x => new { x.AllocationEventModel.AllocationCode, x.AllocationEventModel.Id })
                .MustAsync (async (model, cancellation) =>
                {
                    return !await dbContext.AllocationEvents
                        .AnyAsync (a =>
                            a.AllocationCode == model.AllocationCode &&
                            a.Id != model.Id &&
                            !a.IsDeleted,
                            cancellation);
                })
                .WithMessage ((command, model) =>
                    localizer.Format (
                        LocalizationKey.ModelCodeAlreadyExists,
                        localizer[EntityName.AllocationEvent],
                        model.AllocationCode
                    ));

            // 6. Kiểm tra danh sách AllocationDetails không trùng (CompanyId + EventId)
            RuleFor (x => x.AllocationEventModel.AllocationDetails)
                .Must (details =>
                {
                    if (details == null || !details.Any ( ))
                        return false;

                    var duplicate = details
                        .GroupBy (d => new { d.CompanyId, d.EventId })
                        .Any (g => g.Count ( ) > 1);
                    return !duplicate;
                })
                .WithMessage (localizer["DuplicateCompanyOrEventInDetails"]);

            // 7. Kiểm tra Company, Region, Event hợp lệ
            // Kiểm tra Company
            //RuleForEach(x => x.AllocationEventModel.AllocationDetails)
            //    .MustAsync(async (parent, detail, cancellation) =>
            //    {
            //        var allocationMonth = parent.AllocationEventModel.AllocationMonth;
            //        var allocationYear = parent.AllocationEventModel.AllocationYear;

            //        return await dbContext.Companies
            //            .AnyAsync(c =>
            //                c.Id == detail.CompanyId &&
            //                c.Status == StatusType.Active &&
            //                c.EstablishmentDate.HasValue &&
            //                (
            //                    c.EstablishmentDate.Value.Year < allocationYear ||
            //                    (c.EstablishmentDate.Value.Year == allocationYear &&
            //                     c.EstablishmentDate.Value.Month <= allocationMonth)
            //                ),
            //                cancellation);
            //    })
            //    .WithMessage(detail => localizer["InvalidCompany"]);

            // Kiểm tra Event
            RuleForEach (x => x.AllocationEventModel.AllocationDetails)
                .MustAsync (async (parent, detail, cancellation) =>
                {
                    return await dbContext.Events
                        .AnyAsync (e =>
                            e.Id == detail.EventId &&
                            e.Status == StatusType.Active &&
                            e.Category == EventCategory.Event,
                            cancellation);
                })
                .WithMessage (detail => localizer["InvalidEvent"]);

            // Kiểm tra Region
            RuleForEach (x => x.AllocationEventModel.AllocationDetails)
                .MustAsync (async (parent, detail, cancellation) =>
                {
                    var now = DateTime.UtcNow;

                    return await dbContext.LogRegionComs
                        .AnyAsync (lrc =>
                            lrc.CompanyId == detail.CompanyId &&
                            lrc.RegionId == detail.RegionId &&
                            lrc.StartedDate <= now &&
                            (lrc.EndDate == null || lrc.EndDate >= now),
                            cancellation);
                })
                .WithMessage (detail => localizer["InvalidRegion"]);
        }
    }
}
