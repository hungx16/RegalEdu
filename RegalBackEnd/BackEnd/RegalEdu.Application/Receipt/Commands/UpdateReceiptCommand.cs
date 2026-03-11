using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Logging;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Receipt.Commands
{
    public class UpdateReceiptCommand : IRequest<Result>
    {
        public required ReceiptsModel ReceiptModel { get; set; }
    }

    public class UpdateReceiptCommandHandler : IRequestHandler<UpdateReceiptCommand, Result>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateReceiptCommandHandler(IRegalEducationDbContext db, IMapper mapper, ILocalizationService localizer)
        {
            _db = db; _mapper = mapper; _localizer = localizer;
        }

        public async Task<Result> Handle(UpdateReceiptCommand request, CancellationToken ct)
        {
            var m = request.ReceiptModel;

            var entity = await _db.Receipts
                .FirstOrDefaultAsync(x => x.Id == m.Id && !x.IsDeleted, ct);

            if (entity == null)
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, EntityName.Receipt));

            // cập nhật scalar & FK
            entity.ReceiptType = m.ReceiptType;
            entity.ReceiptCode = m.ReceiptCode;
            entity.RegisterStudyId = m.RegisterStudyId;
            entity.StudentId = m.StudentId;
            entity.CourseId = m.CourseId;
            entity.EmployeeId = m.EmployeeId;
            entity.PaymentType = m.PaymentType;
            entity.PaymentMethodType = m.PaymentMethodType;
            entity.PaymentMethod = m.PaymentMethod;
            entity.TotalAmount = m.TotalAmount;
            entity.Note = m.Note;

            _db.Receipts.Update(entity);
            var ok = await _db.SaveChangesAsync(ct) > 0;

            return ok
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.Receipt))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Receipt));
        }
    }
}
