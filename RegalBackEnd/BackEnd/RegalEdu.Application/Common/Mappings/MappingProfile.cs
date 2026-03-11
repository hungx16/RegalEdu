using Microsoft.AspNetCore.Identity;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;
using UserManagement.Application.Common.Results;
namespace UserManagement.Application.Common.Mappings
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<BaseEntity, BaseEntityModel>().ReverseMap();
            CreateMap<BaseEntityModel, BaseEntity>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<ApplicationUser, UserResult>()
                .ReverseMap();
            CreateMap<BaseEntity, BaseEntityModel>().ReverseMap();
            CreateMap<AccountGroup, AccountGroupModel>().ReverseMap();
            CreateMap<AccountGroupPermission, AccountGroupPermissionModel>().ReverseMap();
            CreateMap<AccountGroupEmployee, AccountGroupEmployeeModel>().ReverseMap();
            CreateMap<Notification, NotificationModel>().ReverseMap();

            CreateMap<UserDetailInfo, UserDetailInfoModel>().ReverseMap();
            CreateMap<IdentityRole, RoleModel>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserModel>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.AvatarString,
                    opt => opt.MapFrom(src => src.Avatar != null ? Convert.ToBase64String(src.Avatar) : null));

            //Hải - Tạo Mapping từ Category sang CategoryModel. Tự động tạo ánh xạ ngược lại
            CreateMap<Category, CategoryModel>().ReverseMap();
            CreateMap<LearningRoadMap, LearningRoadMapModel>().ReverseMap();

            CreateMap<Company, CompanyModel>().ReverseMap();
            //CreateMap<Contract, ContractModel> ( ).ReverseMap ( );
            CreateMap<Holiday, HolidayModel>().ReverseMap();

            CreateMap<WorkingTime, WorkingTimeModel>().ReverseMap();
            CreateMap<Region, RegionModel>().ReverseMap();
            CreateMap<Region, RegionDto>().ReverseMap();



            CreateMap<Division, DivisionModel>().ReverseMap();
            CreateMap<Division, DivisionDto>().ReverseMap();

            CreateMap<Department, DepartmentModel>().ReverseMap();
            CreateMap<Department, DepartmentDto>().ReverseMap();

            CreateMap<DepartmentPosition, DepartmentPositionDto>().ReverseMap();
            CreateMap<DepartmentPosition, DepartmentPositionModel>().ReverseMap();

            CreateMap<Position, PositionModel>().ReverseMap();
            CreateMap<Position, PositionDto>().ReverseMap();

            CreateMap<Company, CompanyModel>().ReverseMap();
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<CompanyModel, CompanyDto>().ReverseMap();
            CreateMap<Image, ImageModel>()
                .ReverseMap();
            CreateMap<Image, ImageDto>()
          .ReverseMap();
            CreateMap<LogRegionCom, LogRegionComModel>().ReverseMap();
            CreateMap<LogRegionCom, LogRegionComDto>().ReverseMap();

            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Employee, EmployeeModel>()
                .ForMember(dest => dest.ApplicationUser, opt => opt.MapFrom(src => src.ApplicationUser))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))  // Explicitly map Company
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
                .ReverseMap();


            CreateMap<ApplicationUser, ApplicationUserModel>()
                .ForMember(dest => dest.Employee, opt => opt.Ignore()) // Không map lại Employee để tránh vòng lặp
                .ReverseMap();
            CreateMap<ApplicationUserModel, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                 .ForMember(dest => dest.NormalizedUserName,
                    opt => opt.MapFrom(src => src.UserName != null ? src.UserName.ToUpper() : null))
                .ForMember(dest => dest.NormalizedEmail,
                    opt => opt.MapFrom(src => src.Email != null ? src.Email.ToUpper() : null))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.UserCode))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.RequireChangePassword, opt => opt.Ignore())
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))

                .ForMember(dest => dest.Avatar,
                    opt => opt.MapFrom(src =>
                        !string.IsNullOrEmpty(src.AvatarString)
                            ? Convert.FromBase64String(src.AvatarString)
                            : src.Avatar))
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiry, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Replace ForAllOtherMembers



            CreateMap<WorkingTimeConfiguration, WorkingTimeConfigurationModel>().ReverseMap();

            CreateMap<WorkingTimeConfigurationCompany, WorkingTimeConfigurationCompanyModel>().ReverseMap();
            CreateMap<WorkingTimeConfigurationCompany, WorkingTimeConfigurationCompanyDto>().ReverseMap();
            CreateMap<WorkingTime, WorkingTimeDto>().ReverseMap();
            CreateMap<Holiday, HolidayDto>().ReverseMap();

            CreateMap<LectureType, LectureTypeModel>().ReverseMap();

            CreateMap<ClassType, ClassTypeModel>()
                .ReverseMap();
            CreateMap<Degree, DegreeModel>().ReverseMap();

            CreateMap<SupportingDocument, SupportingDocumentModel>()
                .ReverseMap();
            CreateMap<LogEmployeePosition, LogEmployeePositionModel>().ReverseMap();
            CreateMap<LogEmployeePosition, LogEmployeePositionDto>().ReverseMap();

            CreateMap<EvaluateTeacher, EvaluateTeacherModel>().ReverseMap();

            CreateMap<RegisterStudy, RegisterStudyModel>().ReverseMap();
            CreateMap<DetailRegisterStudy, DetailRegisterStudyModel>().ReverseMap();

            CreateMap<AdmissionsQuota, AdmissionsQuotaModel>().ReverseMap();
            CreateMap<AdmissionsQuota, AdmissionsQuotaDto>().ReverseMap();

            CreateMap<AdmissionsQuotaAdjustment, AdmissionsQuotaAdjustmentModel>().ReverseMap();

            CreateMap<AdmissionsQuotaCompany, AdmissionsQuotaCompanyModel>().ReverseMap();
            CreateMap<AdmissionsQuotaCompany, AdmissionsQuotaCompanyDto>().ReverseMap();

            CreateMap<AdmissionsQuotaEmployee, AdmissionsQuotaEmployeeModel>().ReverseMap();
            CreateMap<AdmissionsQuotaEmployee, AdmissionsQuotaEmployeeDto>().ReverseMap();

            CreateMap<AdmissionsQuotaRegion, AdmissionsQuotaRegionModel>().ReverseMap();
            CreateMap<AdmissionsQuotaRegion, AdmissionsQuotaRegionDto>().ReverseMap();



            //   CreateMap<PriceList, PriceListModel> ( ).ReverseMap ( );
            //   CreateMap<PriceListDetail, PriceListDetailModel> ( ).ReverseMap ( );

            //Hải

            CreateMap<Course, CourseModel>().ReverseMap();
            CreateMap<Event, EventModel>().ReverseMap();
            CreateMap<Tuition, TuitionModel>().ReverseMap();
            //CreateMap<StudentInClass, StudentInClassModel> ( ).ReverseMap( );//Hải 22.11.2025

            CreateMap<Item, ItemModel>().ReverseMap();

            CreateMap<PartnerType, PartnerTypeModel>().ReverseMap();
            CreateMap<AffiliatePartner, AffiliatePartnerModel>().ReverseMap();
            CreateMap<Attachment, AttachmentModel>().ReverseMap();

            //Hải
            CreateMap<CourseLesson, CourseLessonModel>().ReverseMap();
            CreateMap<AllocationEvent, AllocationEventModel>().ReverseMap().ForMember(dest => dest.AllocationDetails, opt => opt.Ignore()); ;
            CreateMap<AllocationDetailEvent, AllocationDetailEventModel>().ReverseMap();
            // Ignore SessionName khi map từ Model -> Entity
            //vũ

            CreateMap<Promotion, PromotionModel>().ReverseMap();
            CreateMap<PromotionCoupon, PromotionCouponModel>().ReverseMap();
            CreateMap<PromotionFixedPrice, PromotionFixedPriceModel>().ReverseMap();
            CreateMap<Discount, DiscountModel>().ReverseMap();
            CreateMap<DiscountDetail, DiscountDetailModel>().ReverseMap();
            CreateMap<PromotionGift, PromotionGiftModel>().ReverseMap();
            CreateMap<PromotionGiftDetail, PromotionGiftDetailModel>().ReverseMap();
            CreateMap<PromotionGroup, PromotionGroupModel>().ReverseMap();
            CreateMap<Gift, GiftModel>().ReverseMap();
            CreateMap<Coupon, CouponModel>().ReverseMap();
            CreateMap<CouponType, CouponTypeModel>().ReverseMap();
            CreateMap<CouponIssue, CouponIssueModel>().ReverseMap();
            CreateMap<CouponStudent, CouponStudentModel>().ReverseMap();
            CreateMap<CouponTypeCoupon, CouponTypeCouponModel>().ReverseMap();
            CreateMap<CouponTypeDiscount, CouponTypeDiscountModel>().ReverseMap();
            CreateMap<CouponTypeDiscountDetail, CouponTypeDiscountDetailModel>().ReverseMap();
            CreateMap<CouponTypeFixedPrice, CouponTypeFixedPriceModel>().ReverseMap();
            CreateMap<CouponTypeGift, CouponTypeGiftModel>().ReverseMap();

            //CreateMap<CouponTypeGiftDetail, CouponTypeGiftDetailModel> ( ).ReverseMap ( );

            //Vinh

            CreateMap<CompanyLearningRoadMap, CompanyLearningRoadMapModel>().ReverseMap();
            CreateMap<LearningRoadMap, LearningRoadMapDto>().ReverseMap();

            //Thiện
            CreateMap<Teacher, TeacherModel>()
                .ForMember(dest => dest.ApplicationUserId,
                          opt => opt.MapFrom(src => src.ApplicationUserId)).ReverseMap(); ;

            //CreateMap<CouponTypeGiftDetail, CouponTypeGiftDetailModel> ( ).ReverseMap ( );

            //Vinh
            CreateMap<RecruitmentApply, RecruitmentApplyModel>().ReverseMap();

            CreateMap<RecruitmentInfo, RecruitmentInfoModel>().ReverseMap();
            //vũ thêm mapping couponissuestudent
            CreateMap<CouponTypeGiftDetail, CouponTypeGiftDetailModel>().ReverseMap();
            CreateMap<CouponIssueStudent, CouponIssueStudentModel>().ReverseMap();
            CreateMap<RegisterPromotionList, RegisterPromotionListModel>().ReverseMap();
            //vu cập nhật liên quan đến student
            CreateMap<Student, StudentModel>().ReverseMap();
            CreateMap<Customer, CustomerModel>().ReverseMap();
            CreateMap<Contact, ContactModel>().ReverseMap();
            CreateMap<StudentActivity, StudentActivityModel>().ReverseMap();
            CreateMap<StudentCourse, StudentCourseModel>().ReverseMap();
            CreateMap<StudentNote, StudentNoteModel>().ReverseMap();

            //Hải
            CreateMap<Class, ClassModel>().ReverseMap();

            CreateMap<DetailRegisterStudyModel, DetailRegisterStudy>().ReverseMap();
            CreateMap<ReceiptsModel, Receipts>().ReverseMap();
            CreateMap<RegisterGiftModel, RegisterGift>().ReverseMap();
            CreateMap<ProfileModel, Profile>().ReverseMap();
            CreateMap<EnrollmentModel, Enrollment>().ReverseMap();
            //vũ cập nhât
            CreateMap<PromotionStudentModel, PromotionStudent>().ReverseMap();

            CreateMap<AllocationEventHistoryModel, AllocationEventHistory>().ReverseMap();

            CreateMap<CompanyEventModel, CompanyEvent>().ReverseMap();

            CreateMap<EventCashModel, EventCash>().ReverseMap();

            CreateMap<EventParticipantModel, EventParticipant>().ReverseMap();

            CreateMap<EventPublicationModel, EventPublication>().ReverseMap();
            CreateMap<ApproveCompanyEventModel, ApproveCompanyEvent>().ReverseMap();

            CreateMap<CompanyEventReport, CompanyEventReportModel>()
                .ReverseMap()
                .ForMember(dest => dest.CompanyEvent, opt => opt.Ignore())
                .ForMember(dest => dest.EventPublications, opt => opt.Ignore())
                .ForMember(dest => dest.EventCashes, opt => opt.Ignore())
                .ForMember(dest => dest.EventParticipants, opt => opt.Ignore())
                .ForMember(dest => dest.Attachments, opt => opt.Ignore())
                .ForMember(dest => dest.ApproveCompanyEvents, opt => opt.Ignore());

            CreateMap<ApproveCompanyEventReport, ApproveCompanyEventReportModel>().ReverseMap();

            CreateMap<PromotionStudentModel, PromotionStudent>().ReverseMap();

            //Hải update 26.11.2025
            CreateMap<ClassAttendent, ClassAttendentModel>().ReverseMap();
            CreateMap<ClassSchedule, ClassScheduleModel>().ReverseMap();
            CreateMap<ClassScoreBoard, ClassScoreBoardModel>().ReverseMap();
            //Hải bổ sung
            CreateMap<ClassAttendentModel, ClassAttendent>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ClassScheduleId, opt => opt.Ignore()) // KHÔNG cho phép ghi đè
            .ForMember(dest => dest.ClassSchedule, opt => opt.Ignore())   // Navigation property
            .ForMember(dest => dest.StudentId, opt => opt.Ignore())       // KHÔNG cho phép ghi đè
            .ForMember(dest => dest.Student, opt => opt.Ignore())         // Navigation property
            .ForMember(dest => dest.IsTuitionCalculated, opt => opt.Ignore()) // KHÔNG cho phép ghi đè
            ;
            CreateMap<TransferCompany, TransferCompanyModel>().ReverseMap();

            CreateMap<OutputCommitment, OutputCommitmentModel>().ReverseMap();
            CreateMap<TeacherWorkLog, TeacherWorkLogModel>().ReverseMap();
        }
    }
}
