using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RegisterStudy.Queries
{
    public class GetAllRegisterStudysQuery : IRequest<Result<List<RegisterStudyModel>>> { }

    public class GetAllRegisterStudysQueryHandler : IRequestHandler<GetAllRegisterStudysQuery, Result<List<RegisterStudyModel>>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;

        public GetAllRegisterStudysQueryHandler(IRegalEducationDbContext db, IMapper mapper)
        {
            _db = db; _mapper = mapper;
        }

        public async Task<Result<List<RegisterStudyModel>>> Handle(GetAllRegisterStudysQuery request, CancellationToken ct)
        {
            var data = await _db.RegisterStudys.AsNoTracking()
                .Include(x => x.Student).ThenInclude(x=>x.Contacts)
                .Include(x => x.Student).ThenInclude(x=>x.Profile)
                .Include(x => x.Company)
                .Include(x => x.Region)
                .Include(x => x.Employee)
                .Include(x => x.Teacher)
                .Include(x => x.Promotion)
                .Include(x => x.DetailRegisterStudys) // dòng chi tiết khóa học
                .Include(x => x.RegisterPromotionList)
                .Include(x => x.Receipts)
                .ToListAsync(ct);
            List<RegisterStudyModel> models = new List<RegisterStudyModel>();
            foreach (var item in data)
            {
                var model = new RegisterStudyModel
                {
                    Id = item.Id,
                    Type = item.Type,
                    Code = item.Code,
                    CouponCode = item.CouponCode,
                    //CodeParent = item.CodeParent,
                    StudentId = item.StudentId,
                    CompanyId = item.CompanyId,
                    RegionId = item.RegionId,
                    EmployeeId = item.EmployeeId,
                    TeacherId = item.TeacherId,
                    
                    StudentFullName = item.Student?.FullName,
                    StudentPhone = item.Student?.Phone,
                    StudentEmail = item.Student?.Email,
                    StudentBirthDate = item.Student?.BirthDate,

                    ContactFullName = item.Student?.Contacts?.FirstOrDefault()?.FullName,
                    ContactPhone = item.Student?.Contacts?.FirstOrDefault()?.Phone,
                    ContactEmail = item.Student?.Contacts?.FirstOrDefault()?.Email,
                    ContactAddress = item.Student?.Contacts?.FirstOrDefault()?.Address,
                    ContactRelationship = item.Student?.Contacts?.FirstOrDefault() != null ? (Domain.Enumerations.Relationship)item.Student.Contacts.FirstOrDefault()!.Relationship : Domain.Enumerations.Relationship.Father,
                    //ExpectedCompleteDate = item.ExpectedCompleteDate,
                    //_mapper.Map<List<Domain.Entities.DetailRegisterStudy>>(model.DetailRegisterStudys);
                    
                    DetailRegisterStudys = _mapper.Map<List<Domain.Models.DetailRegisterStudyModel>>(item.DetailRegisterStudys),
                    
                    RegisterPromotion = _mapper.Map<List<RegisterPromotionListModel>>(item.RegisterPromotionList),
                    
                    TotalAmount = item.TotalAmount,
                    PaymentStatus = item.PaymentStatus,
                    TotalAfterDiscount = item.AmountToBePaid,
                    AmountToBePaid = item.AmountToBePaid,
                    
                    TotalDiscount = item.TotalDiscount,
                    TuitionFeesPaid = item.TuitionFeesPaid,
                    RemainingTuitionFees = item.RemainingTuitionFees,
                    //TotalAfterDiscount = item.TotalDiscount,
                    FirstPaymentAmount = item.Receipts?.FirstOrDefault()?.TotalAmount,
                    PaymentMethod = item.Receipts?.FirstOrDefault()?.PaymentMethod,
                    PaymentMethodType = item.Receipts?.FirstOrDefault()?.PaymentMethodType,
                    PaymentType = item.Receipts?.FirstOrDefault()?.PaymentType,
                    Receipts = _mapper.Map<List<Domain.Models.ReceiptsModel>>(item.Receipts),

                };
                models.Add(model);
            }
            return Result<List<RegisterStudyModel>>.Success(models);
        }
    }
}
