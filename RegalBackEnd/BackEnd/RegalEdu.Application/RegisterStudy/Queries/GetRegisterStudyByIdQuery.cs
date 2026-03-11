using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RegisterStudy.Queries
{
    public class GetRegisterStudyByIdQuery : IRequest<Result<RegisterStudyModel>>
    {
        public required string Id { get; set; }
    }

    public class GetRegisterStudyByIdQueryHandler : IRequestHandler<GetRegisterStudyByIdQuery, Result<RegisterStudyModel>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetRegisterStudyByIdQueryHandler(IRegalEducationDbContext db, IMapper mapper, ILocalizationService localizer)
        {
            _db = db; _mapper = mapper; _localizer = localizer;
        }

        public async Task<Result<RegisterStudyModel>> Handle(GetRegisterStudyByIdQuery request, CancellationToken ct)
        {
            var entity = await _db.RegisterStudys.AsNoTracking()
                .Include(x => x.Student)
                .Include(x => x.Company)
                .Include(x => x.Region)
                .Include(x => x.Employee)
                .Include(x => x.Teacher)
                .Include(x => x.Promotion)
                .Include(x => x.DetailRegisterStudys)
                .Include(x => x.RegisterPromotionList)
                .FirstOrDefaultAsync(x => x.Id.ToString() == request.Id && !x.IsDeleted, ct);

            if (entity == null)
                return Result<RegisterStudyModel>.Failure(_localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.RegisterStudy, request.Id));

            // Load RegisterPromotionList (không có collection trên RegisterStudy)
            var rpl = await _db.RegisterPromotionList.AsNoTracking()
                .Where(r => r.RegisterStudyId == entity.Id && !r.IsDeleted)
                .ToListAsync(ct);
            var model = new RegisterStudyModel
            {
                Id = entity.Id,
                Type = entity.Type,
                Code = entity.Code,
                CouponCode = entity.CouponCode,
                //CodeParent = item.CodeParent,
                StudentId = entity.StudentId,
                CompanyId = entity.CompanyId,
                RegionId = entity.RegionId,
                EmployeeId = entity.EmployeeId,
                TeacherId = entity.TeacherId,

                StudentFullName = entity.Student?.FullName,
                StudentPhone = entity.Student?.Phone,
                StudentEmail = entity.Student?.Email,
                StudentBirthDate = entity.Student?.BirthDate,

                ContactFullName = entity.Student?.Contacts?.FirstOrDefault()?.FullName,
                ContactPhone = entity.Student?.Contacts?.FirstOrDefault()?.Phone,
                ContactEmail = entity.Student?.Contacts?.FirstOrDefault()?.Email,
                ContactAddress = entity.Student?.Contacts?.FirstOrDefault()?.Address,
                ContactRelationship = entity.Student?.Contacts?.FirstOrDefault() != null ? (Domain.Enumerations.Relationship)entity.Student.Contacts.FirstOrDefault()!.Relationship : Domain.Enumerations.Relationship.Father,
                //ExpectedCompleteDate = item.ExpectedCompleteDate,
                //_mapper.Map<List<Domain.Entities.DetailRegisterStudy>>(model.DetailRegisterStudys);

                DetailRegisterStudys = _mapper.Map<List<Domain.Models.DetailRegisterStudyModel>>(entity.DetailRegisterStudys),

                RegisterPromotion = _mapper.Map<List<RegisterPromotionListModel>>(entity.RegisterPromotionList),

                TotalAmount = entity.TotalAmount,
                PaymentStatus = entity.PaymentStatus,
                // TotalAfterDiscount = item.TotalAfterDiscount,
                TotalDiscount = entity.TotalDiscount,
                //TotalAfterDiscount = item.TotalDiscount,
                FirstPaymentAmount = entity.Receipts?.FirstOrDefault()?.TotalAmount,
                PaymentMethod = entity.Receipts?.FirstOrDefault()?.PaymentMethod,
                PaymentMethodType = entity.Receipts?.FirstOrDefault()?.PaymentMethodType,
                PaymentType = entity.Receipts?.FirstOrDefault()?.PaymentType,
                Receipts = entity.Receipts != null ? _mapper.Map<List<ReceiptsModel>>(entity.Receipts) : null ,

            };
            //models.Add(model);
            //var model = _mapper.Map<RegisterStudyModel>(entity);
           // model.RegisterPromotion = _mapper.Map<List<RegisterPromotionListModel>>(rpl);

            return Result<RegisterStudyModel>.Success(model);
        }
    }
}
