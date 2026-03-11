using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RegisterStudy.Commands
{
    public class AddRegisterStudyCommand : IRequest<Result>
    {
        public required RegisterStudyModel RegisterStudyModel { get; set; }
    }

    public class AddRegisterStudyCommandHandler : IRequestHandler<AddRegisterStudyCommand, Result>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly ILogger<AddRegisterStudyCommandHandler> _logger;

        private sealed class CommitmentCourseInfo
        {
            public Domain.Entities.Course Course { get; init; } = default!;
            public Guid? ProgramId { get; init; }
            public double TotalMonths { get; set; }
            public double TotalFee { get; set; }
        }

        public AddRegisterStudyCommandHandler(
            IRegalEducationDbContext db,
            IMapper mapper,
            ILocalizationService localizer,
            ILogger<AddRegisterStudyCommandHandler> logger)
        {
            _db = db; _mapper = mapper; _localizer = localizer; _logger = logger;
        }

        public async Task<Result> Handle(AddRegisterStudyCommand request, CancellationToken ct)
        {
            if (_db is not DbContext dbContext)
                throw new InvalidOperationException(_localizer[LocalizationKey.InvalidDbContextInstance]);
            var model = request.RegisterStudyModel;
            if (model.StudentId == null)
            {
                var studentEntity = new Domain.Entities.Student
                {
                    Id = Guid.NewGuid(),
                    FullName = model.StudentFullName,
                    Phone = model.StudentPhone,
                    Email = model.StudentEmail,
                    BirthDate = (DateTime)model.StudentBirthDate,
                    EnglishName = "None",
                    CompanyId = model.CompanyId,
                    EmployeeId = model.EmployeeId,
                    Address = model.ContactAddress,
                    RegionId = model.RegionId,
                    StudentStatus = Domain.Enums.StudentStatus.NewRegister,
                    TotalAvailableAmount = model.TuitionFeesPaid,//số tiền đã đóng
                    StudentCode = await AutoCodeHelper.GenerateCodeAsync(
                         new AutoCodeInfo
                         {
                             TableName = "Students",
                             ColumnName = "StudentCode",
                             Prefix = "HV-RS",
                             Length = 4
                         },
                         dbContext
                     ),

                };


                var contactEntity = new Domain.Entities.Contact
                {
                    FullName = model.ContactFullName,
                    Phone = model.ContactPhone,
                    Email = model.ContactEmail,
                    Address = model.ContactAddress,
                    StudentId = studentEntity.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = model.CreatedBy,
                    Relationship = model.ContactRelationship
                };

                studentEntity.Contacts = new List<Domain.Entities.Contact> { contactEntity };


                model.StudentId = studentEntity.Id;
                await _db.Students.AddAsync(studentEntity, ct);

            }
            if (model.StudentId != null)
            {
               var studentToUpdate = await _db.Students.FirstOrDefaultAsync(s => s.Id == model.StudentId, ct);
                if (studentToUpdate != null)
                {
                    studentToUpdate.TotalAvailableAmount = (studentToUpdate.TotalAvailableAmount ?? 0) + (model.TuitionFeesPaid ?? 0);
                    _db.Students.Update(studentToUpdate);
                }
            }
            // var entity = _mapper.Map<Domain.Entities.RegisterStudy>(model);
            var entity = new Domain.Entities.RegisterStudy
            {

                Type = model.Type,
                Code = await AutoCodeHelper.GenerateCodeAsync(
                         new AutoCodeInfo
                         {
                             TableName = "RegisterStudys",
                             ColumnName = "Code",
                             Prefix = "RS",
                             Length = 4
                         },
                         dbContext
                     ),
                CouponCode = model.CouponCode,
                CodeParent = model.CodeParent,
                StudentId = model.StudentId,
                CompanyId = model.CompanyId,
                RegionId = model.RegionId,
                EmployeeId = model.EmployeeId,
                TeacherId = model.TeacherId,
                PromotionId = model.PromotionId,
                PaymentStatus = model.PaymentStatus,
                TotalAmount = model.TotalAmount, //tổng học phí chưa giảm giá
                TotalDiscount = model.TotalDiscount,//tổng tiền giảm giá
                TuitionFeesPaid = model.TuitionFeesPaid,//số tiền đã đóng
                AmountToBePaid = model.TotalAfterDiscount,//số tiền phải đóng = TotalAmount-TotalDiscount
                RemainingTuitionFees = model.TotalAfterDiscount - model.TuitionFeesPaid,//số tiền còn lại phải đóng

                CreatedBy = model.CreatedBy,
                CreatedAt = model.CreatedAt,
                UpdatedBy = model.UpdatedBy,
                UpdatedAt = model.UpdatedAt,
                Status = model.Status
            };

            // Map child collections if provided
            if (model.DetailRegisterStudys != null)
                entity.DetailRegisterStudys = _mapper.Map<List<Domain.Entities.DetailRegisterStudy>>(model.DetailRegisterStudys);

            // RegisterPromotionList không có collection trên RegisterStudy, nên add qua DbSet
            await _db.RegisterStudys.AddAsync(entity, ct);
            var receiptEnty = new Domain.Entities.Receipts
            {
                RegisterStudyId = entity.Id,
                ReceiptCode = await AutoCodeHelper.GenerateCodeAsync(
                         new AutoCodeInfo
                         {
                             TableName = "Receipts",
                             ColumnName = "ReceiptCode",
                             Prefix = "PR",
                             Length = 4
                         },
                         dbContext
                     ),

                TotalAmount = model.TuitionFeesPaid,
                Status = (Domain.Enums.StatusType)(model.PaymentStatus ?? 0),
                StudentId = model.StudentId,
                //CourseId = model.

                // TuitionFeesPaid = 
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = model.CreatedBy,
                ReceiptType = ReceiptType.Order,
                PaymentMethod = model.PaymentMethod,
                PaymentType = model.PaymentType,
                PaymentMethodType = model.PaymentMethodType,
                EmployeeId = model.EmployeeId,

                CourseId = model.DetailRegisterStudys.FirstOrDefault().CourseId,
            };
            await _db.Receipts.AddAsync(receiptEnty, ct);

            var saved = await _db.SaveChangesAsync(ct) > 0;

            // Sau khi có Id, insert RegisterPromotionList nếu được truyền
            if (saved && model.RegisterPromotion != null && model.RegisterPromotion.Any())
            {
                foreach (var rp in model.RegisterPromotion)
                {
                    var rpEntity = _mapper.Map<Domain.Entities.RegisterPromotionList>(rp);
                    rpEntity.RegisterStudyId = entity.Id;
                    await _db.RegisterPromotionList.AddAsync(rpEntity, ct);
                }
                saved = await _db.SaveChangesAsync(ct) > 0 || saved;

            }


            //thêm quà tặng 
            // Sau khi có Id, insert RegisterPromotionList nếu được truyền
            if (saved && model.RegisterGifts != null && model.RegisterGifts.Any())
            {
                foreach (var rp in model.RegisterGifts)
                {
                    var rpEntity = _mapper.Map<Domain.Entities.RegisterGift>(rp);
                    rpEntity.RegisterStudyId = entity.Id;
                    await _db.RegisterGift.AddAsync(rpEntity, ct);
                }
                saved = await _db.SaveChangesAsync(ct) > 0 || saved;

            }
            //Thêm khóa học đã đăng ký vào Enroll
            // Sau khi có Id, insert RegisterPromotionList nếu được truyền
            if (saved && model.DetailRegisterStudys != null && model.DetailRegisterStudys.Any())
            {
                //tạo danh sách các Enroll
                List<Enrollment> enrollmentList = new List<Enrollment>();
                foreach (var cr in model.DetailRegisterStudys)
                {

                    var erEntity = new Domain.Entities.Enrollment
                    {

                        StudentId = entity.StudentId,
                        CourseId = cr.CourseId,
                        Fee = cr.TuitionFee,
                        Discount = cr.DiscountAmount,//giảm giá
                        FinalFee = cr.TotalAmount, //học phí sau giảm giá
                        PaidAmount = cr.PaidAmount, //số tiền đã đóng
                        PaymentCourseStatus = entity.PaymentStatus,
                        StudentCourseStatus = Domain.Enums.StudentCourseStatus.NotStarted,
                        RegisterStudyId = entity.Id,
                        ClassTypeId = cr.ClassTypeId,

                    };
                    await _db.Enrollments.AddAsync(erEntity, ct);
                }
                saved = await _db.SaveChangesAsync(ct) > 0 || saved;

            }
            // ➤ Sinh ra OutputCommitment nếu thỏa điều kiện
            if (saved && model.DetailRegisterStudys != null && model.DetailRegisterStudys.Any())
            {
                var detailEntries = model.DetailRegisterStudys
                    .Where(d => d.CourseId.HasValue && d.ClassTypeId.HasValue)
                    .ToList();

                if (detailEntries.Any())
                {
                    var courseIds = detailEntries
                        .Select(d => d.CourseId!.Value)
                        .Distinct()
                        .ToList();

                    var courses = await _db.Courses
                        .Where(c => courseIds.Contains(c.Id))
                        .ToListAsync(ct);

                    var courseLookup = courses.ToDictionary(c => c.Id);
                    var courseInfos = new List<CommitmentCourseInfo>();
                    double overallRegisteredFee = 0;
                    foreach (var detail in detailEntries)
                    {
                        double detailMonths = 0;
                        if (!courseLookup.TryGetValue(detail.CourseId!.Value, out var course))
                            continue;
                        overallRegisteredFee += detail.TotalAmount ?? 0;
                        if (course.CommitmentOutputType == CommitmentOutputType.None)
                            continue;

                        var isSelfCommit = course.CommitmentOutputType == CommitmentOutputType.SelfCommitment;

                        var tuition = await _db.Tuition
                            .FirstOrDefaultAsync(t =>
                                t.CourseId.HasValue &&
                                t.CourseId == detail.CourseId &&
                                t.ClassTypeId == detail.ClassTypeId, ct);

                        detailMonths = tuition != null ? Convert.ToDouble(tuition.TotalMonths) : 0;

                        var info = courseInfos.FirstOrDefault(ci => ci.Course.Id == course.Id);
                        if (info == null)
                        {
                            info = new CommitmentCourseInfo { Course = course, ProgramId = course.LearningRoadMapId };
                            courseInfos.Add(info);
                        }

                        info.TotalMonths += detailMonths;
                        info.TotalFee += detail.TotalAmount ?? 0;
                    }

                    if (entity.StudentId != null && courseInfos.Any())
                    {
                        var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == entity.StudentId, ct);
                        var commitmentsToSave = new List<Domain.Entities.OutputCommitment>();

                        // 1) Self-commit courses: always create one commitment per course
                        var selfCommitInfos = courseInfos
                            .Where(ci => ci.Course.CommitmentOutputType == CommitmentOutputType.SelfCommitment)
                            .ToList();

                        foreach (var ci in selfCommitInfos)
                        {
                            var oc = new Domain.Entities.OutputCommitment
                            {
                                StudentId = entity.StudentId,
                                StudentCode = student?.StudentCode ?? string.Empty,
                                OutputCommitmentInfo = ci.Course.CommitmentLevel ?? string.Empty,
                                TotalRegisteredMonths = (int)Math.Ceiling(ci.TotalMonths),
                                TotalRegisteredFee = (float)ci.TotalFee,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = model.CreatedBy
                            };
                            commitmentsToSave.Add(oc);
                        }

                        // 2) Included courses: create commitment if overall months exceed threshold
                        var includedInfos = courseInfos
                            .Where(ci => ci.Course.CommitmentOutputType == CommitmentOutputType.Included)
                            .ToList();

                        var includedGroups = includedInfos.GroupBy(ci => ci.ProgramId);
                        foreach (var group in includedGroups)
                        {
                            var groupMonths = group.Sum(ci => ci.TotalMonths);
                            if (groupMonths < 24)
                                continue;

                            var selectedIncluded = group
                                .Where(ci => !string.IsNullOrWhiteSpace(ci.Course.CommitmentLevel))
                                .OrderByDescending(ci => ci.Course.OrdinalNumber ?? double.MinValue)
                                .FirstOrDefault()
                                ?? group
                                    .OrderByDescending(ci => ci.Course.OrdinalNumber ?? double.MinValue)
                                    .FirstOrDefault();

                            if (selectedIncluded == null)
                                continue;

                            var oc = new Domain.Entities.OutputCommitment
                            {
                                StudentId = entity.StudentId,
                                StudentCode = student?.StudentCode ?? string.Empty,
                                OutputCommitmentInfo = selectedIncluded.Course.CommitmentLevel ?? string.Empty,
                                TotalRegisteredMonths = (int)Math.Ceiling(groupMonths),
                                TotalRegisteredFee = (float)group.Sum(ci => ci.TotalFee),
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = model.CreatedBy
                            };
                            commitmentsToSave.Add(oc);
                        }

                        if (commitmentsToSave.Any())
                        {
                            await _db.OutputCommitments.AddRangeAsync(commitmentsToSave, ct);
                            saved = await _db.SaveChangesAsync(ct) > 0 || saved;
                        }
                    }
                }
            }
            return saved
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.RegisterStudy))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.RegisterStudy));
        }
    }
}
