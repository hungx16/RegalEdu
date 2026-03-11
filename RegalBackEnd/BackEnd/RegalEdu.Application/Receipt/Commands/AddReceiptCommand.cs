using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Receipt.Commands
{
    public class AddReceiptCommand : IRequest<Result>
    {
        public required ReceiptsModel ReceiptModel { get; set; }
    }

    public class AddReceiptCommandHandler : IRequestHandler<AddReceiptCommand, Result>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public AddReceiptCommandHandler(IRegalEducationDbContext db, IMapper mapper, ILocalizationService localizer)
        {
            _db = db; _mapper = mapper; _localizer = localizer;
        }

        public async Task<Result> Handle(AddReceiptCommand request, CancellationToken ct)
        {
            var m = request.ReceiptModel;

            var entity = _mapper.Map<RegalEdu.Domain.Entities.Receipts>(m);

            await _db.Receipts.AddAsync(entity, ct);
            //cập nhật lại trạng thái thanh toán và số tiền thanh toán trên RegisterStudy
            var registerEntity = _db.RegisterStudys.Where(s=>s.Id == m.RegisterStudyId).FirstOrDefault();
            if (registerEntity != null)
            {
                registerEntity.TuitionFeesPaid += m.TotalAmount;
                registerEntity.RemainingTuitionFees -= m.TotalAmount;
                registerEntity.PaymentStatus = PaymentStatus.Paid;
            }
            //Cập nhật số tiền đóng vào TotalAvailableAmount của học viên
            var studentEntity = await _db.Students.Where(s => s.Id == m.StudentId).FirstOrDefaultAsync();
            if (studentEntity != null && m.TotalAmount.HasValue)
            {
                studentEntity.TotalAvailableAmount += m.TotalAmount;
            }
            //lưu thay đổi
            var ok = await _db.SaveChangesAsync(ct) > 0;

            return ok
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Receipt))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Receipt));
        }
    }
}
