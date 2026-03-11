
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RegalEdu.Domain.Entities;

namespace RegalEdu.Application.Common.Interfaces
{
    public interface IRegalEducationDbContext
    {
        public DbSet<Domain.Entities.AccountGroupPermission> AccountGroupPermissions { get; set; }

        public DbSet<AccountGroupUser> AccountGroupUsers { get; set; }

        public DbSet<RegalEdu.Domain.Entities.AccountGroup> AccountGroups { get; set; } // This now refers to the type, not the namespace
        public DbSet<UserDetailInfo> UserDetailInfos { get; set; }
        public DbSet<RegalEdu.Domain.Entities.Notification> Notifications { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Domain.Entities.Student> Students { get; set; }
        public DbSet<Domain.Entities.Customer> Customers { get; set; }
        public DbSet<RegalEdu.Domain.Entities.Teacher> Teachers { get; set; }
        public DbSet<RegalEdu.Domain.Entities.Class> Classes { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<PlacementTest> PlacementTests { get; set; }
        public DbSet<Domain.Entities.Promotion> Promotions { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<DataLog> DataLogs { get; set; }
        public DbSet<Domain.Entities.AccountGroupEmployee> AccountGroupEmployees { get; set; }
        //Phàn mới Vinh thêm

        public DbSet<Domain.Entities.WorkingTimeConfiguration> WorkingTimeConfigurations { get; set; } // Cấu hình ca làm việc
        public DbSet<WorkingTimeConfigurationCompany> WorkingTimeConfigurationCompanies { get; set; }

        // DbSet các bảng chính
        public DbSet<Domain.Entities.Division> Divisions { get; set; } // Fixed by resolving namespace conflict
        public DbSet<Domain.Entities.Department> Departments { get; set; }
        public DbSet<Domain.Entities.Position> Positions { get; set; }
        public DbSet<DepartmentPosition> DepartmentPositions { get; set; }

        public DbSet<Domain.Entities.Region> Regions { get; set; }
        public DbSet<Domain.Entities.Company> Companies { get; set; }
        public DbSet<Domain.Entities.LogRegionCom> LogRegionComs { get; set; }



        //Hải - Khai báo DbSet

        public DbSet<Domain.Entities.LearningRoadMap> LearningRoadMaps { get; set; }

        public DbSet<Domain.Entities.Employee> Employees { get; set; }

        public DbSet<Domain.Entities.Category> Categories { get; set; }
        public DbSet<Domain.Entities.WorkingTime> WorkingTimes { get; set; }
        public DbSet<Domain.Entities.Holiday> Holidays { get; set; }

        DbSet<TEntity> SetEntity<TEntity>( ) where TEntity : class;

        int SaveChanges( );
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken ( ));
        EntityEntry<T> EntryEntity<T>(T entity) where T : class;
        object Set(Type entityType);
        void Update(object entity);
        Task<BaseEntity> FindAsync(Type entityType, Guid id);
        void Remove(BaseEntity entity);
        void Remove(ApplicationUser applicationUser);

        public DbSet<Domain.Entities.LectureType> LectureTypes { get; set; }
        public DbSet<Domain.Entities.Degree> Degrees { get; set; }

        public DbSet<Domain.Entities.ClassType> ClassTypes { get; set; }


        public DbSet<Domain.Entities.SupportingDocument> SupportingDocuments { get; set; }
        public DbSet<Domain.Entities.Image> Images { get; set; }

        public DbSet<Domain.Entities.LogEmployeePosition> LogEmployeePositions { get; set; }

        //Vũ thêm phần Tuyển sinh
        public DbSet<Domain.Entities.RegisterStudy> RegisterStudys { get; set; }
        public DbSet<Domain.Entities.DetailRegisterStudy> DetailRegisterStudys { get; set; }
        public DbSet<Domain.Entities.Receipts> Receipts { get; set; }
        public DbSet<Domain.Entities.Course> Courses { get; set; }
        public DbSet<Domain.Entities.AdmissionsQuota> AdmissionsQuotas { get; set; }
        public DbSet<Domain.Entities.AdmissionsQuotaCompany> AdmissionsQuotaCompanies { get; set; }
        public DbSet<Domain.Entities.AdmissionsQuotaRegion> AdmissionsQuotaRegions { get; set; }
        public DbSet<Domain.Entities.AdmissionsQuotaEmployee> AdmissionsQuotaEmployees { get; set; }
        public DbSet<AdmissionsQuotaAdjustment> AdmissionsQuotaAdjustments { get; set; }
        //event
        public DbSet<Domain.Entities.Event> Events { get; set; }

        //   public DbSet<Domain.Entities.PriceList> PriceLists { get; set; }
        //   public DbSet<PriceListDetail> PriceListDetails { get; set; }

        public DbSet<Domain.Entities.Item> Items { get; set; }

        public DbSet<Domain.Entities.AffiliatePartner> AffiliatePartners { get; set; }

        public DbSet<Domain.Entities.PartnerType> PartnerTypes { get; set; }

        public DbSet<Domain.Entities.Attachment> Attachments { get; set; }
        //vũ update 21/06/2024
        public DbSet<Domain.Entities.Discount> Discounts { get; set; }
        public DbSet<Domain.Entities.CourseGift> CourseGifts { get; set; }
        //PromotionCoupon
        public DbSet<Domain.Entities.PromotionCoupon> PromotionCoupon { get; set; }
        //PromotionGift
        public DbSet<Domain.Entities.PromotionGift> PromotionGift { get; set; }
        //PromotionFixedPrice
        public DbSet<Domain.Entities.PromotionFixedPrice> PromotionFixedPrice { get; set; }
        public DbSet<Domain.Entities.Coupon> Coupons { get; set; }
        public DbSet<Domain.Entities.CouponIssue> CouponIssues { get; set; }
        public DbSet<Domain.Entities.CouponType> CouponType { get; set; }
        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = new CancellationToken ( ));

        //Hải update 2409
        public DbSet<Domain.Entities.CourseLesson> CourseLessons { get; set; }
        public DbSet<Domain.Entities.Tuition> Tuition { get; set; }
        public DbSet<Domain.Entities.Contact> Contact { get; set; }
        public DbSet<Domain.Entities.PromotionGroup> PromotionGroup { get; set; }
        public DbSet<Domain.Entities.Gift> Gift { get; set; }

        //Thiện thêm 01/10/2024
        //public DbSet<Domain.Entities.CompanyTeacher> CompanyTeachers { get; set; }
        public DbSet<Domain.Entities.EvaluateTeacher> EvaluateTeachers { get; set; }
        public DbSet<Domain.Entities.PayrollTeacher> PayrollTeachers { get; set; }
        public DbSet<Domain.Entities.WorkBoardTeacher> WorkBoardTeachers { get; set; }
        public DbSet<Domain.Entities.TeacherWorkLog> TeacherWorkLogs { get; set; }
        //vũ khai áo bổ sung 01/10/2025 phần CouponType
        public DbSet<Domain.Entities.CouponStudent> CouponStudent { get; set; }
        public DbSet<Domain.Entities.CouponTypeCoupon> CouponTypeCoupon { get; set; }
        public DbSet<Domain.Entities.CouponTypeFixedPrice> CouponTypeFixedPrice { get; set; }
        public DbSet<Domain.Entities.CouponTypeGift> CouponTypeGift { get; set; }
        public DbSet<Domain.Entities.CouponTypeGiftDetail> CouponTypeGiftDetail { get; set; }
        public DbSet<Domain.Entities.CouponTypeDiscount> CouponTypeDiscount { get; set; }
        public DbSet<Domain.Entities.CouponTypeDiscountDetail> CouponTypeDiscountDetail { get; set; }
        public DbSet<Domain.Entities.CouponIssue> CouponIssue { get; set; }


        //vinh
        public DbSet<Domain.Entities.CompanyLearningRoadMap> CompanyLearningRoadMaps { get; set; }

        //Hải thêm
        public DbSet<Domain.Entities.AllocationEvent> AllocationEvents { get; set; }
        public DbSet<Domain.Entities.AllocationDetailEvent> AllocationDetailEvents { get; set; }
        //vũ thêm
        public DbSet<Domain.Entities.CouponIssueStudent> CouponIssueStudent { get; set; }


        //Vinh

        public DbSet<Domain.Entities.RecruitmentApply> RecruitmentApplies { get; set; }

        public DbSet<Domain.Entities.RecruitmentInfo> RecruitmentInfos { get; set; }

        public DbSet<Domain.Entities.RegisterPromotionList> RegisterPromotionList { get; set; }
        public DbSet<Domain.Entities.StudentActivity> StudentActivity { get; set; }
        public DbSet<Domain.Entities.StudentCourse> StudentCourse { get; set; }
        public DbSet<Domain.Entities.StudentNote> StudentNote { get; set; }
        public DbSet<Domain.Entities.RegisterGift> RegisterGift { get; set; }
        //vũ thêm
        public DbSet<Domain.Entities.PromotionStudent> PromotionStudent { get; set; }


        public DbSet<Domain.Entities.AllocationEventHistory> AllocationEventHistories { get; set; }

        public DbSet<Domain.Entities.CompanyEvent> CompanyEvents { get; set; }
        public DbSet<Domain.Entities.CompanyEventReport> CompanyEventReports { get; set; }
        public DbSet<Domain.Entities.EventCash> EventCashes { get; set; }
        public DbSet<Domain.Entities.EventParticipant> EventParticipants { get; set; }
        public DbSet<Domain.Entities.EventPublication> EventPublications { get; set; }

        public DbSet<Domain.Entities.ApproveCompanyEvent> ApproveCompanyEvents { get; set; }
        public DbSet<Domain.Entities.ApproveCompanyEventReport> ApproveCompanyEventReports { get; set; }
        //public DbSet<Domain.Entities.StudentInClass> StudentInClass { get; set; }
        //Hải update 26.11.2025
        public DbSet<Domain.Entities.ClassAttendent> ClassAttendent { get; set; }
        public DbSet<Domain.Entities.ClassSchedule> ClassSchedule { get; set; }
        public DbSet<Domain.Entities.ClassScoreBoard> ClassScoreBoard { get; set; }

        //public DbSet<Domain.Entities.ClassScoreSummary> ClassScoreSummary { get; set; }

        public DbSet<Domain.Entities.OutputCommitment> OutputCommitments { get; set; }

        public DbSet<Domain.Entities.TransferCompany> TransferCompanies { get; set; }       

    }
}
