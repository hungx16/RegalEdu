using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Entities;
using System.Linq.Expressions;
using System.Text.Json;

namespace RegalEducation.Persistence // Corrected spelling of 'Edu' to 'Education'
{
    public class RegalEducationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IRegalEducationDbContext// Fixed CS0311 by specifying IdentityRole<Guid> and Guid
    {
        private readonly ICurrentUserService _currentUserService;

        public RegalEducationDbContext(DbContextOptions<RegalEducationDbContext> options, ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
        }
        public DbSet<AccountGroupPermission> AccountGroupPermissions { get; set; }

        public DbSet<AccountGroupUser> AccountGroupUsers { get; set; }

        public DbSet<AccountGroup> AccountGroups { get; set; }
        public DbSet<UserDetailInfo> UserDetailInfos { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<PlacementTest> PlacementTests { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<DataLog> DataLogs { get; set; }
        public DbSet<AccountGroupEmployee> AccountGroupEmployees { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<DepartmentPosition> DepartmentPositions { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<LogRegionCom> LogRegionComs { get; set; }

        //Hải

        public DbSet<Category> Categories { get; set; }
        public DbSet<LearningRoadMap> LearningRoadMaps { get; set; }
        public DbSet<Tuition> Tuition { get; set; }
        public DbSet<CourseLesson> CourseLessons { get; set; }
        public DbSet<AllocationEvent> AllocationEvents { get; set; }
        public DbSet<AllocationDetailEvent> AllocationDetailEvents { get; set; }
        //
        public DbSet<WorkingTime> WorkingTimes { get; set; }
        public DbSet<Holiday> Holidays { get; set; }


        //Phàn mới Vinh thêm

        // DbSet các bảng chính
        public DbSet<WorkingTimeConfigurationCompany> WorkingTimeConfigurationCompanies { get; set; }

        public DbSet<WorkingTimeConfiguration> WorkingTimeConfigurations { get; set; }
        public DbSet<LectureType> LectureTypes { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<ClassType> ClassTypes { get; set; }

        public DbSet<SupportingDocument> SupportingDocuments { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<LogEmployeePosition> LogEmployeePositions { get; set; } // Bảng ghi log thay đổi vị trí nhân viên
        // Lucky draw and rewards
        public DbSet<LuckyDraw> LuckyDraws { get; set; }
        public DbSet<CustomerReward> CustomerRewards { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        //Vũ thêm phần Tuyển sinh

        public DbSet<RegisterStudy> RegisterStudys { get; set; }
        public DbSet<DetailRegisterStudy> DetailRegisterStudys { get; set; }
        public DbSet<Receipts> Receipts { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<AdmissionsQuota> AdmissionsQuotas { get; set; }
        public DbSet<AdmissionsQuotaCompany> AdmissionsQuotaCompanies { get; set; }
        public DbSet<AdmissionsQuotaRegion> AdmissionsQuotaRegions { get; set; }
        public DbSet<AdmissionsQuotaEmployee> AdmissionsQuotaEmployees { get; set; }
        public DbSet<AdmissionsQuotaAdjustment> AdmissionsQuotaAdjustments { get; set; }

        //   public DbSet<PriceList> PriceLists { get; set; }
        //     public DbSet<PriceListDetail> PriceListDetails { get; set; }
        //event
        public DbSet<Event> Events { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<AffiliatePartner> AffiliatePartners { get; set; }

        public DbSet<PartnerType> PartnerTypes { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        //Vũ thêm liên quan đến khuyến mại
        //ublic DbSet<Promotion> Promotion { get; set; }
        // public DbSet<Discount> Discount { get; set; }
        public DbSet<DiscountDetail> DiscountDetails { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<CourseGift> CourseGifts { get; set; }
        public DbSet<PromotionCoupon> PromotionCoupon { get; set; }
        public DbSet<PromotionGift> PromotionGift { get; set; }
        public DbSet<PromotionFixedPrice> PromotionFixedPrice { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponIssue> CouponIssues { get; set; }
        public DbSet<CouponType> CouponType { get; set; }

        // Thiện thêm 
        public DbSet<EvaluateTeacher> EvaluateTeachers { get; set; } // bảng đánh giá giáo viên
        //public DbSet<CompanyTeacher> CompanyTeachers { get; set; } // bảng liên kết công ty và giáo viên
        public DbSet<PayrollTeacher> PayrollTeachers { get; set; } // bảng lương giáo viên
        public DbSet<WorkBoardTeacher> WorkBoardTeachers { get; set; } // bảng bảng chấm công giáo viên
        public DbSet<TeacherWorkLog> TeacherWorkLogs { get; set; } // bảng tính công giáo viên
        public DbSet<Profile> Profiles { get; set; } // bảng hồ sơ học viên
        public DbSet<OutputCommitment> OutputCommitments { get; set; } // bảng cam kết đầu ra

        // public DbSet<TEntity> SetEntity<TEntity>( ) where TEntity : class
        public DbSet<Contact> Contact { get; set; }
        public DbSet<PromotionGroup> PromotionGroup { get; set; }
        public DbSet<Gift> Gift { get; set; }
        public DbSet<CouponStudent> CouponStudent { get; set; }
        public DbSet<CouponTypeCoupon> CouponTypeCoupon { get; set; }
        public DbSet<CouponTypeFixedPrice> CouponTypeFixedPrice { get; set; }
        public DbSet<CouponTypeGift> CouponTypeGift { get; set; }
        public DbSet<CouponTypeGiftDetail> CouponTypeGiftDetail { get; set; }
        public DbSet<CouponTypeDiscount> CouponTypeDiscount { get; set; }
        public DbSet<CouponTypeDiscountDetail> CouponTypeDiscountDetail { get; set; }
        public DbSet<CouponIssue> CouponIssue { get; set; }

        public DbSet<CompanyLearningRoadMap> CompanyLearningRoadMaps { get; set; }
        public DbSet<RecruitmentApply> RecruitmentApplies { get; set; }
        public DbSet<RecruitmentInfo> RecruitmentInfos { get; set; }
        public DbSet<CouponIssueStudent> CouponIssueStudent { get; set; }
        public DbSet<RegisterPromotionList> RegisterPromotionList { get; set; }
        public DbSet<StudentActivity> StudentActivity { get; set; }
        public DbSet<StudentCourse> StudentCourse { get; set; }
        public DbSet<StudentNote> StudentNote { get; set; }
        public DbSet<RegisterGift> RegisterGift { get; set; }
        public DbSet<PromotionStudent> PromotionStudent { get; set; }
        public DbSet<AllocationEventHistory> AllocationEventHistories { get; set; }

        public DbSet<CompanyEvent> CompanyEvents { get; set; }
        public DbSet<EventCash> EventCashes { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }
        public DbSet<EventPublication> EventPublications { get; set; }
        public DbSet<ApproveCompanyEvent> ApproveCompanyEvents { get; set; }
        public DbSet<CompanyEventReport> CompanyEventReports { get; set; }
        public DbSet<ApproveCompanyEventReport> ApproveCompanyEventReports { get; set; }

        //public DbSet<StudentInClass> StudentInClass { get; set; }//Hải thêm 22.11.2025       
        public DbSet<ClassSchedule> ClassSchedule { get; set; }//Hải thêm 26.11.2025
        public DbSet<ClassAttendent> ClassAttendent { get; set; }//Hải thêm 26.11.2025
        //public DbSet<ClassScoreSummary> ClassScoreSummary { get; set; }//Hải thêm 26.11.2025
        public DbSet<ClassScoreBoard> ClassScoreBoard { get; set; }//Hải thêm 26.11.2025
        public DbSet<TransferCompany> TransferCompanies { get; set; }//Hải thêm 06.12.2025
        public DbSet<TEntity> SetEntity<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }

        public EntityEntry<T> EntryEntity<T>(T entity) where T : class
        {
            return Entry<T>(entity);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RegalEducationDbContext).Assembly);
            // Cấu hình mặc định cho tất cả bảng kế thừa BaseEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Soft-delete filter
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                                .HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
                }
            }

            // Đảm bảo DivisionCode là unique
            modelBuilder.Entity<Division>()
                .HasIndex(x => x.DivisionCode)
                .IsUnique();
            // Đảm bảo PositionCode là unique
            modelBuilder.Entity<Position>()
                .HasIndex(x => x.PositionCode)
                .IsUnique();

            // Đảm bảo DepartmentCode là unique
            modelBuilder.Entity<Department>()
                .HasIndex(x => x.DepartmentCode)
                .IsUnique();


            // Đảm bảo DepartmentCode là unique
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(x => x.UserCode)
                .IsUnique();

            //Hải - Đảm bảo LearningRoadmap là unique
            modelBuilder.Entity<LearningRoadMap>()
                .HasIndex(x => x.LearningRoadMapCode)
                .IsUnique();
            //Hải - Đảm bảo Event là unique
            modelBuilder.Entity<Event>()
               .HasIndex(x => x.EventCode)
               .IsUnique();


            modelBuilder.Entity<CompanyEventReport> (entity =>
            {
                entity.HasOne (r => r.CompanyEvent)
                      .WithMany (e => e.CompanyEventReports)
                      .HasForeignKey (r => r.CompanyEventId)
                      .OnDelete (DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ApproveCompanyEventReport> (entity =>
            {
                entity.HasOne (a => a.CompanyEventReport)
                      .WithMany (r => r.ApproveCompanyEvents)
                      .HasForeignKey (a => a.CompanyEventReportId)
                      .OnDelete (DeleteBehavior.Restrict);

                entity.HasOne (a => a.Employee)
                      .WithMany ( )
                      .HasForeignKey (a => a.EmployeeId)
                      .OnDelete (DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Employee> ( )
                .HasOne (e => e.Company)
                .WithMany (c => c.Employees) // Corrected navigation property to match the expected collection type
                .HasForeignKey (e => e.CompanyId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Company)
                .WithMany(c => c.Employees) // Corrected navigation property to match the expected collection type
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ApplicationUser>()
           .HasOne(u => u.Employee)
           .WithOne(e => e.ApplicationUser)
           .HasForeignKey<Employee>(e => e.ApplicationUserId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Companies)  // Một Employee có nhiều Regions
                .WithOne(r => r.Manager)  // Mỗi Region có một Manager (Employee)
                .HasForeignKey(r => r.ManagerId)  // Trỏ đến ManagerId trong Region
                .OnDelete(DeleteBehavior.Restrict);  // Tránh xóa cứng Region khi xóa Employee

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Regions)  // Một Employee có nhiều Regions
                .WithOne(r => r.Manager)  // Mỗi Region có một Manager (Employee)
                .HasForeignKey(r => r.ManagerId)  // Trỏ đến ManagerId trong Region
                .OnDelete(DeleteBehavior.Restrict);  // Tránh xóa cứng Region khi xóa Employee

            // Uniques
            modelBuilder.Entity<AdmissionsQuota>()
              .HasIndex(x => new { x.Year, x.Month }).IsUnique();

            modelBuilder.Entity<AdmissionsQuotaRegion>()
              .HasIndex(x => new { x.AdmissionsQuotaId, x.RegionId }).IsUnique();

            modelBuilder.Entity<AdmissionsQuotaCompany>()
              .HasIndex(x => new { x.AdmissionsQuotaId, x.CompanyId }).IsUnique();
            //Hải
            modelBuilder.Entity<Course>().ToTable("Course"); // chắc chắn EF không pluralize
            base.OnModelCreating(modelBuilder);

            // Đảm bảo PriceListCode là unique
            //modelBuilder.Entity<PriceList> ( )
            //    .HasIndex (x => x.PriceListCode)
            //    .IsUnique ( );

            // Thiện
            //modelBuilder.Entity<ApplicationUser>()
            //.HasOne(u => u.Teacher)
            //.WithOne() // hoặc .WithOne(t => t.ApplicationUser) nếu Teacher có navigation ngược
            //.HasForeignKey<ApplicationUser>("TeacherId")
            //.OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Teacher>()
            //.HasOne(t => t.ApplicationUser)
            //.WithOne(u => u.Teacher)
            //.HasForeignKey<Teacher>(t => t.ApplicationUserId) // Sử dụng property có sẵn
            //.OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ApplicationUser>()
              .HasOne(u => u.Teacher)
              .WithOne(e => e.ApplicationUser)
              .HasForeignKey<Teacher>(e => e.ApplicationUserId)
              .OnDelete(DeleteBehavior.Restrict);

            // Giữ lại cấu hình quan hệ thông qua CompanyTeacher
            //modelBuilder.Entity<CompanyTeacher> ( )
            //    .HasOne (ct => ct.Company)
            //    .WithMany (c => c.CompanyTeachers)
            //    .HasForeignKey (ct => ct.CompanyId);


            //Hải - Đảm bảo mỗi cặp Năm, tháng trong AllocationMonth là duy nhất
            modelBuilder.Entity<AllocationEvent>()
                .HasIndex(a => new { a.AllocationYear, a.AllocationMonth })
                .IsUnique();
            // Cấu hình quan hệ 1-1 giữa Student và Profile
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Profile)         // Một Student có một Profile
                .WithOne(p => p.Student)        // Một Profile thuộc về một Student
                .HasForeignKey<Profile>(p => p.Id); // Khóa ngoại là 'Id' trên 'Profile'
        }
        // Filter dùng cho soft delete
        private static LambdaExpression ConvertFilterExpression(Type entityType)
        {
            var param = Expression.Parameter(entityType, "e");
            var prop = Expression.Property(param, nameof(BaseEntity.IsDeleted));
            var condition = Expression.Equal(prop, Expression.Constant(false));
            return Expression.Lambda(condition, param);
        }
        public override int SaveChanges()
        {
            try
            {
                var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

                foreach (var entry in entries)
                {
                    var entity = entry.Entity as BaseEntity; // Corrected type check and casting
                    if (entity != null) // Ensure entity is not null after casting
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entity.CreatedAt = DateTime.Now;
                            entity.CreatedBy = _currentUserService.UserName ?? "(unknown)";
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entity.UpdatedBy = _currentUserService.UserName ?? "(unknown)";
                            entity.UpdatedAt = DateTime.Now;
                            //// Kiểm tra nếu trường IsDeleted có thay đổi và giá trị mới là true
                            //var originalIsDeleted = entry.OriginalValues["IsDeleted"];
                            //var currentIsDeleted = entry.CurrentValues["IsDeleted"];

                            //// Kiểm tra nếu IsDeleted có thay đổi thành true
                            //if (!object.Equals (originalIsDeleted, currentIsDeleted) && (bool)currentIsDeleted == true)
                            //{
                            //    // Thực hiện các logic xử lý thêm ở đây nếu cần, ví dụ: ghi log, thông báo, v.v.
                            //    entity.DeletedBy = _currentUserService.UserName ?? "(unknown)";
                            //    entity.DeletedAt = DateTime.Now;
                            //}
                        }
                        //else if (entry.State == EntityState.Deleted)
                        //{
                        //    entity.DeletedBy = _currentUserService.UserName ?? "(unknown)";
                        //    entity.DeletedAt = DateTime.Now;
                        //}
                        if (entry.Entity.GetType().Name != "DataHistories")
                        {
                            DataLogs.Add(new DataLog
                            {
                                TableName = entry.Entity.GetType().Name ?? "(unknown)",
                                Action = entry.State.ToString(),
                                OriginalData = JsonSerializer.Serialize(entry.OriginalValues?.Clone().ToObject()),
                                MadeBy = _currentUserService.UserName ?? "(unknown)",
                                ByWhen = DateTime.Now,
                                CurrentData = JsonSerializer.Serialize(entry.CurrentValues.Clone().ToObject()),
                            });
                        }
                    }
                    if (entity == null)
                    {
                        var entityUser = entry.Entity as ApplicationUser;
                        if (entityUser != null)
                        {
                            if (entry.State == EntityState.Added)
                            {
                                entityUser.CreatedAt = DateTime.Now;
                                entityUser.CreatedBy = _currentUserService.UserName ?? "(unknown)";
                            }
                            else if (entry.State == EntityState.Modified)
                            {
                                entityUser.UpdatedBy = _currentUserService.UserName ?? "(unknown)";
                                entityUser.UpdatedAt = DateTime.Now;
                                // Kiểm tra nếu trường IsDeleted có thay đổi và giá trị mới là true
                                //var originalIsDeleted = entry.OriginalValues["IsDeleted"];
                                //var currentIsDeleted = entry.CurrentValues["IsDeleted"];

                                //// Kiểm tra nếu IsDeleted có thay đổi thành true
                                //if (!object.Equals (originalIsDeleted, currentIsDeleted) && (bool)currentIsDeleted == true)
                                //{
                                //    // Thực hiện các logic xử lý thêm ở đây nếu cần, ví dụ: ghi log, thông báo, v.v.
                                //    entity.DeletedBy = _currentUserService.UserName ?? "(unknown)";
                                //    entity.DeletedAt = DateTime.Now;
                                //}
                            }
                            //else if (entry.State == EntityState.Deleted)
                            //{
                            //    entityUser.DeletedBy = _currentUserService.UserName ?? "(unknown)";
                            //    entityUser.DeletedAt = DateTime.Now;
                            //}
                            if (entry.Entity.GetType().Name != "DataHistories")
                            {
                                DataLogs.Add(new DataLog
                                {
                                    TableName = entry.Entity.GetType().Name ?? "(unknown)",
                                    Action = entry.State.ToString(),
                                    OriginalData = JsonSerializer.Serialize(entry.OriginalValues?.Clone().ToObject()),
                                    MadeBy = _currentUserService.UserName ?? "(unknown)",
                                    ByWhen = DateTime.Now,
                                    CurrentData = JsonSerializer.Serialize(entry.CurrentValues.Clone().ToObject()),
                                });
                            }
                        }
                    }
                }

                return base.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
            .Where(e => (e.Entity is BaseEntity || e.Entity is ApplicationUser) && (e.State == EntityState.Added || e.State == EntityState.Modified)).ToList();

            foreach (var entry in entries)
            {
                var entity = entry.Entity as BaseEntity; // Corrected type check and casting


                if (entity != null) // Ensure entity is not null after casting
                {
                    if (entry.State == EntityState.Added)
                    {
                        entity.UpdatedBy = _currentUserService.UserName ?? "(unknown)";
                        entity.UpdatedAt = DateTime.Now;
                        entity.CreatedAt = DateTime.Now;
                        entity.CreatedBy = _currentUserService.UserName ?? "(unknown)";
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entity.UpdatedBy = _currentUserService.UserName ?? "(unknown)";
                        entity.UpdatedAt = DateTime.Now;
                        //// Kiểm tra nếu trường IsDeleted có thay đổi và giá trị mới là true
                        //var originalIsDeleted = entry.OriginalValues["IsDeleted"];
                        //var currentIsDeleted = entry.CurrentValues["IsDeleted"];

                        //// Kiểm tra nếu IsDeleted có thay đổi thành true
                        //if (!object.Equals (originalIsDeleted, currentIsDeleted) && (bool)currentIsDeleted == true)
                        //{
                        //    entity.DeletedBy = _currentUserService.UserName ?? "(unknown)";
                        //    entity.DeletedAt = DateTime.Now;
                        //    // Thực hiện các logic xử lý thêm ở đây nếu cần, ví dụ: ghi log, thông báo, v.v.
                        //}
                    }
                    //else if (entry.State == EntityState.Deleted)
                    //{
                    //    entity.DeletedBy = _currentUserService.UserName ?? "(unknown)";
                    //    entity.DeletedAt = DateTime.Now;
                    //}
                    if (entry.Entity.GetType().Name != "DataHistories")
                    {
                        DataLogs.Add(new DataLog
                        {
                            TableName = entry.Entity.GetType().Name ?? "(unknown)",
                            Action = entry.State.ToString(),
                            OriginalData = JsonSerializer.Serialize(entry.OriginalValues?.Clone().ToObject()),
                            MadeBy = _currentUserService.UserName ?? "(unknown)",
                            ByWhen = DateTime.Now,
                            CurrentData = JsonSerializer.Serialize(entry.CurrentValues.Clone().ToObject()),
                        });
                    }
                }
                if (entity == null)
                {
                    var entityUser = entry.Entity as ApplicationUser;
                    if (entityUser != null)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entityUser.UpdatedBy = _currentUserService.UserName ?? "(unknown)";
                            entityUser.UpdatedAt = DateTime.Now;
                            entityUser.CreatedAt = DateTime.Now;
                            entityUser.CreatedBy = _currentUserService.UserName ?? "(unknown)";
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entityUser.UpdatedBy = _currentUserService.UserName ?? "(unknown)";
                            entityUser.UpdatedAt = DateTime.Now;
                            //// Kiểm tra nếu trường IsDeleted có thay đổi và giá trị mới là true
                            //var originalIsDeleted = entry.OriginalValues["IsDeleted"];
                            //var currentIsDeleted = entry.CurrentValues["IsDeleted"];

                            //// Kiểm tra nếu IsDeleted có thay đổi thành true
                            //if (!object.Equals (originalIsDeleted, currentIsDeleted) && (bool)currentIsDeleted == true)
                            //{
                            //    // Thực hiện các logic xử lý thêm ở đây nếu cần, ví dụ: ghi log, thông báo, v.v.
                            //    entityUser.DeletedBy = _currentUserService.UserName ?? "(unknown)";
                            //    entityUser.DeletedAt = DateTime.Now;
                            //}
                        }
                        //else if (entry.State == EntityState.Deleted)
                        //{
                        //    entityUser.DeletedBy = _currentUserService.UserName ?? "(unknown)";
                        //    entityUser.DeletedAt = DateTime.Now;
                        //}
                        if (entry.Entity.GetType().Name != "DataHistories")
                        {
                            DataLogs.Add(new DataLog
                            {
                                TableName = entry.Entity.GetType().Name ?? "(unknown)",
                                Action = entry.State.ToString(),
                                OriginalData = JsonSerializer.Serialize(entry.OriginalValues?.Clone().ToObject()),
                                MadeBy = _currentUserService.UserName ?? "(unknown)",
                                ByWhen = DateTime.Now,
                                CurrentData = JsonSerializer.Serialize(entry.CurrentValues.Clone().ToObject()),
                            });
                        }
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }


        public object Set(Type entityType)
        {
            return base.GetType()
                       .GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!
                       .MakeGenericMethod(entityType)
                       .Invoke(this, null)!;
        }


        void IRegalEducationDbContext.Update(object entity)
        {
            // Cập nhật entity vào DbContext
            base.Update(entity);
        }

        public async Task<BaseEntity> FindAsync(Type entityType, Guid id) // Updated return type to match the interface
        {
            var dbSet = Set(entityType);
            // Use reflection to call FindAsync and await the correct ValueTask
            var method = dbSet.GetType().GetMethod("FindAsync", new[] { typeof(object[]) });
            var valueTask = method?.Invoke(dbSet, new object[] { new object[] { id } });

            if (valueTask == null)
            {
                throw new InvalidOperationException("FindAsync method invocation failed."); // Throw exception instead of returning null
            }

            // Convert ValueTask to Task and await
            var asTaskMethod = valueTask.GetType().GetMethod("AsTask");
            var task = asTaskMethod?.Invoke(valueTask, null) as Task;

            if (task == null)
            {
                throw new InvalidOperationException("Task conversion failed."); // Throw exception instead of returning null
            }

            await task.ConfigureAwait(false);
            var result = task.GetType().GetProperty("Result")?.GetValue(task);

            if (result is BaseEntity baseEntity)
            {
                return baseEntity; // Return the result if it is a BaseEntity
            }

            throw new InvalidOperationException("Entity not found or invalid type."); // Throw exception if the result is not a BaseEntity
        }



        public void Remove(BaseEntity entity)
        {
            // Xoá cứng entity khỏi DbContext
            base.Remove(entity);
        }

        public void Remove(ApplicationUser applicationUser)
        {
            base.Remove(applicationUser);
        }

        public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await base.Database.BeginTransactionAsync(cancellationToken);
        }
    }
}
