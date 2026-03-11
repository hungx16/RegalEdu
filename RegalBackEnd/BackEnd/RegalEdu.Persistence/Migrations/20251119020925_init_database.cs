using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class init_database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Enable = table.Column<bool>(type: "bit", nullable: false),
                    UseDefault = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionsQuota",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    TotalSalesAllocated = table.Column<int>(type: "int", nullable: true),
                    CurrentSales = table.Column<int>(type: "int", nullable: true),
                    Month = table.Column<int>(type: "int", nullable: false),
                    QuotaStage = table.Column<int>(type: "int", nullable: false),
                    CompanyCount = table.Column<int>(type: "int", nullable: false),
                    TotalQuota = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionsQuota", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AllocationEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AllocationMonth = table.Column<int>(type: "int", nullable: false),
                    AllocationYear = table.Column<int>(type: "int", nullable: false),
                    EventBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AllocationEventStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProvinceCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IdentityNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Avatar = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequireChangePassword = table.Column<bool>(type: "bit", nullable: false),
                    PasswordConfirm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EnCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryType = table.Column<byte>(type: "tinyint", nullable: false),
                    From = table.Column<int>(type: "int", nullable: true),
                    To = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassTypeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClassTypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MaxStudents = table.Column<int>(type: "int", nullable: false),
                    MinStudents = table.Column<int>(type: "int", nullable: false),
                    SessionsPerWeek = table.Column<int>(type: "int", nullable: false),
                    HoursPerSession = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClassCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MadeBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ByWhen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentData = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Degree",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DegreeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Degree", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DivisionCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DivisionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DivisionLevel = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gift",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prices = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gift", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LectureType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LectureName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LectureType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartnerType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerTypeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PartnerTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    EnPartnerTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMultilingual = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PositionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsSale = table.Column<bool>(type: "bit", nullable: true),
                    IsSaleLead = table.Column<bool>(type: "bit", nullable: true),
                    IsSupport = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromotionGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportingDocument",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EnDocumentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    WebsiteKeys = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EnWebsiteKeys = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuthorName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EnAuthorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    Topic = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    YearRelease = table.Column<int>(type: "int", nullable: false),
                    IsMultilingual = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportingDocument", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkingTimeConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameConfiguration = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    ApplyToSystem = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingTimeConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountGroupEmployees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountGroupEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountGroupEmployees_AccountGroups_AccountGroupId",
                        column: x => x.AccountGroupId,
                        principalTable: "AccountGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountGroupPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllowAction = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountGroupPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountGroupPermissions_AccountGroups_AccountGroupId",
                        column: x => x.AccountGroupId,
                        principalTable: "AccountGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountGroupUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountGroupUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountGroupUsers_AccountGroups_AccountGroupId",
                        column: x => x.AccountGroupId,
                        principalTable: "AccountGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDetailInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetailInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDetailInfos_AccountGroups_AccountGroupId",
                        column: x => x.AccountGroupId,
                        principalTable: "AccountGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearningRoadMap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LearningRoadMapCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LearningRoadMapName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EnLearningRoadMapName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EnDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgeGrId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitmentOutput = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    IsMultilingual = table.Column<bool>(type: "bit", nullable: false),
                    VotingRate = table.Column<float>(type: "real", nullable: false),
                    NumberOfStudents = table.Column<int>(type: "int", nullable: false),
                    NumberOfSatisfiedStudents = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningRoadMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningRoadMap_Category_AgeGrId",
                        column: x => x.AgeGrId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EnDepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DivisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    IsMultilingual = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Department_Division_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Division",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AffiliatePartner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PartnerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AgencyLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    EnPartnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMultilingual = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliatePartner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AffiliatePartner_PartnerType_PartnerTypeId",
                        column: x => x.PartnerTypeId,
                        principalTable: "PartnerType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Holiday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Frequency = table.Column<byte>(type: "tinyint", nullable: false),
                    WorkingTimeConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holiday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Holiday_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Holiday_WorkingTimeConfiguration_WorkingTimeConfigurationId",
                        column: x => x.WorkingTimeConfigurationId,
                        principalTable: "WorkingTimeConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkingTime",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DayOfWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    IsWorkingDay = table.Column<bool>(type: "bit", nullable: false),
                    WorkingTimeConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkingTime_WorkingTimeConfiguration_WorkingTimeConfigurationId",
                        column: x => x.WorkingTimeConfigurationId,
                        principalTable: "WorkingTimeConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MinAvgScore = table.Column<float>(type: "real", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CommitmentOutput = table.Column<bool>(type: "bit", nullable: false),
                    CommitmentLevel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LearningRoadMapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MidExamIds = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FinalExamIds = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    CourseContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CourseKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrdinalNumber = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsMultilingual = table.Column<bool>(type: "bit", nullable: false),
                    EnCourseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnCourseContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnCourseKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnDuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfStudents = table.Column<int>(type: "int", nullable: true),
                    VotingRate = table.Column<decimal>(type: "decimal(18,1)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Course_LearningRoadMap_LearningRoadMapId",
                        column: x => x.LearningRoadMapId,
                        principalTable: "LearningRoadMap",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DepartmentPosition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentPosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentPosition_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentPosition_Position_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecruitmentInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecruitmentInfoName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Experience = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProvinceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnRecruitmentInfoName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EnDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnExperience = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EnPosition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMultilingual = table.Column<bool>(type: "bit", nullable: false),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    WorkType = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitmentInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecruitmentInfo_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tuition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TuitionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TuitionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClassTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DurationHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalMonths = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    TuitionFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tuition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tuition_ClassType_ClassTypeId",
                        column: x => x.ClassTypeId,
                        principalTable: "ClassType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tuition_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecruitmentApplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CandidateEmail = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CandidatePhone = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    CandidateExperience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CandidateCV = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    RecruitmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitmentApplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecruitmentApplies_RecruitmentInfo_RecruitmentId",
                        column: x => x.RecruitmentId,
                        principalTable: "RecruitmentInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseLesson",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TuitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LectureTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Objective = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Homework = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseLesson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseLesson_LectureType_LectureTypeId",
                        column: x => x.LectureTypeId,
                        principalTable: "LectureType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseLesson_Tuition_TuitionId",
                        column: x => x.TuitionId,
                        principalTable: "Tuition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupportingDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LectureTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecruitmentApplyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_LectureType_LectureTypeId",
                        column: x => x.LectureTypeId,
                        principalTable: "LectureType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attachment_RecruitmentApplies_RecruitmentApplyId",
                        column: x => x.RecruitmentApplyId,
                        principalTable: "RecruitmentApplies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attachment_SupportingDocument_SupportingDocumentId",
                        column: x => x.SupportingDocumentId,
                        principalTable: "SupportingDocument",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AdmissionsQuotaAdjustment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    AdmissionsQuotaRegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdmissionsQuotaCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TotalQuotaBefore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalQuotaAfter = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionsQuotaAdjustment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionsQuotaCompany",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentSales = table.Column<int>(type: "int", nullable: true),
                    AdmissionsQuotaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberOfSalesAllocated = table.Column<int>(type: "int", nullable: false),
                    RevenuePerSale = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdmissionsQuotaRegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionsQuotaCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionsQuotaCompany_AdmissionsQuota_AdmissionsQuotaId",
                        column: x => x.AdmissionsQuotaId,
                        principalTable: "AdmissionsQuota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionsQuotaEmployee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdmissionsQuotaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuotaRole = table.Column<byte>(type: "tinyint", nullable: false),
                    WorkStage = table.Column<byte>(type: "tinyint", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevenuePerSale = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    JoinAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProbationEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdmissionsQuotaCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    AdmissionsQuotaRegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionsQuotaEmployee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionsQuotaEmployee_AdmissionsQuotaCompany_AdmissionsQuotaCompanyId",
                        column: x => x.AdmissionsQuotaCompanyId,
                        principalTable: "AdmissionsQuotaCompany",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdmissionsQuotaEmployee_AdmissionsQuota_AdmissionsQuotaId",
                        column: x => x.AdmissionsQuotaId,
                        principalTable: "AdmissionsQuota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdmissionsQuotaEmployee_Position_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionsQuotaRegion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdmissionsQuotaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentSales = table.Column<int>(type: "int", nullable: false),
                    NumberOfSalesAllocated = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyCount = table.Column<int>(type: "int", nullable: false),
                    RevenuePerSale = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionsQuotaRegion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionsQuotaRegion_AdmissionsQuota_AdmissionsQuotaId",
                        column: x => x.AdmissionsQuotaId,
                        principalTable: "AdmissionsQuota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AllocationDetailEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllocationEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoAllocation = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationDetailEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllocationDetailEvent_AllocationEvent_AllocationEventId",
                        column: x => x.AllocationEventId,
                        principalTable: "AllocationEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllocationDetailEvent_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Class",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrialClass = table.Column<bool>(type: "bit", nullable: false),
                    ClassStatus = table.Column<int>(type: "int", nullable: false),
                    ClassTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClassSchedule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Class", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Class_ClassType_ClassTypeId",
                        column: x => x.ClassTypeId,
                        principalTable: "ClassType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Class_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EnCompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EnCompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EstablishmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProvinceCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    WardCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CompanyEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfStudents = table.Column<int>(type: "int", nullable: false),
                    Convenience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnConvenience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VotingRate = table.Column<decimal>(type: "decimal(18,1)", nullable: false),
                    LeaningRoadMapTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnLeaningRoadMapTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMultilingual = table.Column<bool>(type: "bit", nullable: false),
                    WorkingTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnWorkingTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsHeadQuarters = table.Column<bool>(type: "bit", nullable: false),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyLearningRoadMap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LearningRoadMapId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyLearningRoadMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyLearningRoadMap_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyLearningRoadMap_LearningRoadMap_LearningRoadMapId",
                        column: x => x.LearningRoadMapId,
                        principalTable: "LearningRoadMap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CouponType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    DueType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationInDays = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suffix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CharacterCount = table.Column<int>(type: "int", nullable: true),
                    ApplyWith = table.Column<bool>(type: "bit", nullable: true),
                    IsForAllCompany = table.Column<bool>(type: "bit", nullable: true),
                    IsForAllCourse = table.Column<bool>(type: "bit", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinQuantity = table.Column<int>(type: "int", nullable: true),
                    IsForAllStudents = table.Column<bool>(type: "bit", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CouponType_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeTax = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsSupport = table.Column<bool>(type: "bit", nullable: true),
                    EmployeeStartedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeNewEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PersonalEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Position_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsCover = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AffiliatePartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupportingDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LearningRoadMapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Image_AffiliatePartner_AffiliatePartnerId",
                        column: x => x.AffiliatePartnerId,
                        principalTable: "AffiliatePartner",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Image_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Image_LearningRoadMap_LearningRoadMapId",
                        column: x => x.LearningRoadMapId,
                        principalTable: "LearningRoadMap",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Image_SupportingDocument_SupportingDocumentId",
                        column: x => x.SupportingDocumentId,
                        principalTable: "SupportingDocument",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherNickname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TeacherQualifications = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TeacherSpecialization = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    WorkType = table.Column<byte>(type: "tinyint", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", maxLength: 2000, nullable: true),
                    PreferLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeachingOutside = table.Column<bool>(type: "bit", nullable: false),
                    TeacherAssistant = table.Column<bool>(type: "bit", nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubCompanyIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Teachers_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkingTimeConfigurationCompany",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkingTimeConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingTimeConfigurationCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkingTimeConfigurationCompany_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkingTimeConfigurationCompany_WorkingTimeConfiguration_WorkingTimeConfigurationId",
                        column: x => x.WorkingTimeConfigurationId,
                        principalTable: "WorkingTimeConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CouponIssue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CouponTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IssueType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsForAllStudents = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponIssue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponIssue_CouponType_CouponTypeId",
                        column: x => x.CouponTypeId,
                        principalTable: "CouponType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CouponTypeCoupon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinQuantity = table.Column<double>(type: "float", nullable: true),
                    Limit = table.Column<int>(type: "int", nullable: true),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CouponTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponTypeCoupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponTypeCoupon_CouponType_CouponTypeID",
                        column: x => x.CouponTypeID,
                        principalTable: "CouponType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CouponTypeDiscount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: true),
                    DiscountMax = table.Column<double>(type: "float", nullable: true),
                    CouponTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponTypeDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponTypeDiscount_CouponType_CouponTypeId",
                        column: x => x.CouponTypeId,
                        principalTable: "CouponType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CouponTypeFixedPrice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinPrice = table.Column<double>(type: "float", nullable: true),
                    Limit = table.Column<int>(type: "int", nullable: true),
                    PriceSale = table.Column<double>(type: "float", nullable: true),
                    CouponTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponTypeFixedPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponTypeFixedPrice_CouponType_CouponTypeId",
                        column: x => x.CouponTypeId,
                        principalTable: "CouponType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CouponTypeGift",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GiftType = table.Column<int>(type: "int", nullable: true),
                    GiftCount = table.Column<int>(type: "int", nullable: true),
                    CouponTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponTypeGift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponTypeGift_CouponType_CouponTypeId",
                        column: x => x.CouponTypeId,
                        principalTable: "CouponType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LogEmployeePosition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEmployeePosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogEmployeePosition_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogEmployeePosition_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LogEmployeePosition_Position_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegionCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RegionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Region_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EvaluateTeachers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluateName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EvaluateNick = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EvaluateContent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EvaluateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RatingScore = table.Column<int>(type: "int", nullable: false),
                    EvaluateType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsAnonymous = table.Column<bool>(type: "bit", nullable: false),
                    ResponseContent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsResponded = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluateTeachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluateTeachers_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollTeachers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalaryMonth = table.Column<DateTime>(type: "date", nullable: false),
                    StandardWorkDay = table.Column<int>(type: "int", nullable: false),
                    ActualWorkDay = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SalaryAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollTeachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollTeachers_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkBoardTeachers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    CheckinTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckoutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    WorkHours = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkBoardTeachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkBoardTeachers_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CouponTypeDiscountDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinAmount = table.Column<double>(type: "float", nullable: true),
                    Limit = table.Column<int>(type: "int", nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    DiscountAmount = table.Column<double>(type: "float", nullable: false),
                    CouponTypeDiscountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponTypeDiscountDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponTypeDiscountDetail_CouponTypeDiscount_CouponTypeDiscountId",
                        column: x => x.CouponTypeDiscountId,
                        principalTable: "CouponTypeDiscount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CouponTypeGiftDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GiftName = table.Column<int>(type: "int", nullable: true),
                    QuantityGift = table.Column<int>(type: "int", nullable: true),
                    CouponTypeGiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponTypeGiftDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponTypeGiftDetail_CouponTypeGift_CouponTypeGiftId",
                        column: x => x.CouponTypeGiftId,
                        principalTable: "CouponTypeGift",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LogRegionCom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogRegionCom", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogRegionCom_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LogRegionCom_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentLevel = table.Column<int>(type: "int", nullable: true),
                    ExpectedTime = table.Column<int>(type: "int", nullable: true),
                    LearningGoal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnglishExperience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeadSource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentStatus = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EnglishName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdentifyNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    ExpectedStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ExpectedBudget = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Students_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Students_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Students_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Students_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Relationship = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contact_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contact_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CouponIssueStudent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CouponIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponIssueStudent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponIssueStudent_CouponIssue_CouponIssueId",
                        column: x => x.CouponIssueId,
                        principalTable: "CouponIssue",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CouponIssueStudent_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CouponStudent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CouponTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponStudent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponStudent_CouponType_CouponTypeId",
                        column: x => x.CouponTypeId,
                        principalTable: "CouponType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CouponStudent_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Fee = table.Column<double>(type: "float", nullable: true),
                    Discount = table.Column<double>(type: "float", nullable: true),
                    FinalFee = table.Column<double>(type: "float", nullable: true),
                    PaymentStatus = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Enrollments_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Enrollments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OutputCommitment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommitmentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DetailedBranch = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsultantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutputCommitment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutputCommitment_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutputCommitment_Employees_ConsultantId",
                        column: x => x.ConsultantId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OutputCommitment_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutputCommitment_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlacementTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Listening = table.Column<int>(type: "int", nullable: false),
                    Speaking = table.Column<int>(type: "int", nullable: false),
                    Writing = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    EvaluatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacementTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlacementTests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaymentCompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsultantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProfileStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Profiles_Employees_ConsultantId",
                        column: x => x.ConsultantId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Profiles_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Profiles_Students_Id",
                        column: x => x.Id,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplyWith = table.Column<bool>(type: "bit", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Qtymonth = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CodeUsage = table.Column<bool>(type: "bit", nullable: true),
                    PromoCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    AllCompany = table.Column<bool>(type: "bit", nullable: true),
                    AllCourse = table.Column<bool>(type: "bit", nullable: true),
                    AllStudent = table.Column<bool>(type: "bit", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PromotionGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promotions_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotions_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotions_PromotionGroup_PromotionGroupId",
                        column: x => x.PromotionGroupId,
                        principalTable: "PromotionGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentActivity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Results = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallLogURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonFailed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentActivity_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentActivity_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentCourse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterestLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentCourse_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentCourse_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NoteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoteContext = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentNote_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentNote_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseGifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllCourse = table.Column<bool>(type: "bit", nullable: true),
                    MinQty = table.Column<double>(type: "float", nullable: true),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseGifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseGifts_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: true),
                    DiscountMax = table.Column<double>(type: "float", nullable: true),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discounts_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromotionCoupon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinQuantity = table.Column<double>(type: "float", nullable: true),
                    Limit = table.Column<int>(type: "int", nullable: true),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CouponTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionCoupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionCoupon_CouponType_CouponTypeID",
                        column: x => x.CouponTypeID,
                        principalTable: "CouponType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PromotionCoupon_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromotionFixedPrice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinPrice = table.Column<double>(type: "float", nullable: true),
                    Limit = table.Column<int>(type: "int", nullable: true),
                    PriceSale = table.Column<double>(type: "float", nullable: true),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionFixedPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionFixedPrice_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromotionGift",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GiftType = table.Column<int>(type: "int", nullable: true),
                    GiftCount = table.Column<int>(type: "int", nullable: true),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionGift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionGift_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RegisterStudys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsingCouponCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeParent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentStatus = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<double>(type: "float", nullable: true),
                    TotalDiscount = table.Column<double>(type: "float", nullable: true),
                    AmountToBePaid = table.Column<double>(type: "float", nullable: true),
                    TuitionFeesPaid = table.Column<double>(type: "float", nullable: true),
                    RemainingTuitionFees = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisterStudys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegisterStudys_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegisterStudys_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegisterStudys_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegisterStudys_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegisterStudys_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegisterStudys_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseGiftDeTail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinQty = table.Column<double>(type: "float", nullable: true),
                    CourseGiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseGiftDeTail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseGiftDeTail_CourseGifts_CourseGiftId",
                        column: x => x.CourseGiftId,
                        principalTable: "CourseGifts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseGiftDeTail_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DiscountDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinAmount = table.Column<double>(type: "float", nullable: true),
                    Limit = table.Column<int>(type: "int", nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    DiscountAmount = table.Column<double>(type: "float", nullable: false),
                    DiscountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountDetails_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromotionGiftDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GiftName = table.Column<int>(type: "int", nullable: true),
                    QuantityGift = table.Column<int>(type: "int", nullable: true),
                    PromotionGiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionGiftDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionGiftDetail_PromotionGift_PromotionGiftId",
                        column: x => x.PromotionGiftId,
                        principalTable: "PromotionGift",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegisterStudyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CouponIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coupon_CouponIssue_CouponIssueId",
                        column: x => x.CouponIssueId,
                        principalTable: "CouponIssue",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Coupon_RegisterStudys_RegisterStudyId",
                        column: x => x.RegisterStudyId,
                        principalTable: "RegisterStudys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Coupon_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DetailRegisterStudy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterStudyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Unit = table.Column<int>(type: "int", nullable: true),
                    TuitionFee = table.Column<double>(type: "float", nullable: true),
                    DiscountAmount = table.Column<double>(type: "float", nullable: true),
                    TotalAmount = table.Column<double>(type: "float", nullable: true),
                    ClassTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailRegisterStudy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetailRegisterStudy_ClassType_ClassTypeId",
                        column: x => x.ClassTypeId,
                        principalTable: "ClassType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DetailRegisterStudy_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DetailRegisterStudy_RegisterStudys_RegisterStudyId",
                        column: x => x.RegisterStudyId,
                        principalTable: "RegisterStudys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptType = table.Column<int>(type: "int", nullable: true),
                    ReceiptCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisterStudyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentType = table.Column<int>(type: "int", nullable: true),
                    PaymentMethodType = table.Column<int>(type: "int", nullable: true),
                    PaymentMethod = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<double>(type: "float", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receipts_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receipts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receipts_RegisterStudys_RegisterStudyId",
                        column: x => x.RegisterStudyId,
                        principalTable: "RegisterStudys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receipts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RegisterGift",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegisterStudyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GiftName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisterGift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegisterGift_Gift_GiftId",
                        column: x => x.GiftId,
                        principalTable: "Gift",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegisterGift_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegisterGift_RegisterStudys_RegisterStudyId",
                        column: x => x.RegisterStudyId,
                        principalTable: "RegisterStudys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RegisterPromotionList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegisterStudyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscountAmount = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisterPromotionList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegisterPromotionList_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegisterPromotionList_RegisterStudys_RegisterStudyId",
                        column: x => x.RegisterStudyId,
                        principalTable: "RegisterStudys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountGroupEmployees_AccountGroupId",
                table: "AccountGroupEmployees",
                column: "AccountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountGroupPermissions_AccountGroupId",
                table: "AccountGroupPermissions",
                column: "AccountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountGroupUsers_AccountGroupId",
                table: "AccountGroupUsers",
                column: "AccountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuota_Year_Month",
                table: "AdmissionsQuota",
                columns: new[] { "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaAdjustment_AdmissionsQuotaCompanyId",
                table: "AdmissionsQuotaAdjustment",
                column: "AdmissionsQuotaCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaAdjustment_AdmissionsQuotaRegionId",
                table: "AdmissionsQuotaAdjustment",
                column: "AdmissionsQuotaRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaCompany_AdmissionsQuotaId_CompanyId",
                table: "AdmissionsQuotaCompany",
                columns: new[] { "AdmissionsQuotaId", "CompanyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaCompany_AdmissionsQuotaRegionId",
                table: "AdmissionsQuotaCompany",
                column: "AdmissionsQuotaRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaCompany_CompanyId",
                table: "AdmissionsQuotaCompany",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaEmployee_AdmissionsQuotaCompanyId",
                table: "AdmissionsQuotaEmployee",
                column: "AdmissionsQuotaCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaEmployee_AdmissionsQuotaId",
                table: "AdmissionsQuotaEmployee",
                column: "AdmissionsQuotaId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaEmployee_AdmissionsQuotaRegionId",
                table: "AdmissionsQuotaEmployee",
                column: "AdmissionsQuotaRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaEmployee_CompanyId",
                table: "AdmissionsQuotaEmployee",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaEmployee_EmployeeId",
                table: "AdmissionsQuotaEmployee",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaEmployee_PositionId",
                table: "AdmissionsQuotaEmployee",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaEmployee_RegionId",
                table: "AdmissionsQuotaEmployee",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaRegion_AdmissionsQuotaId_RegionId",
                table: "AdmissionsQuotaRegion",
                columns: new[] { "AdmissionsQuotaId", "RegionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionsQuotaRegion_RegionId",
                table: "AdmissionsQuotaRegion",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AffiliatePartner_PartnerCode",
                table: "AffiliatePartner",
                column: "PartnerCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AffiliatePartner_PartnerTypeId",
                table: "AffiliatePartner",
                column: "PartnerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocationDetailEvent_AllocationEventId",
                table: "AllocationDetailEvent",
                column: "AllocationEventId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocationDetailEvent_CompanyId",
                table: "AllocationDetailEvent",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocationDetailEvent_EventId",
                table: "AllocationDetailEvent",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocationDetailEvent_RegionId",
                table: "AllocationDetailEvent",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocationEvent_AllocationYear_AllocationMonth",
                table: "AllocationEvent",
                columns: new[] { "AllocationYear", "AllocationMonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserCode",
                table: "AspNetUsers",
                column: "UserCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_LectureTypeId",
                table: "Attachment",
                column: "LectureTypeId",
                unique: true,
                filter: "[LectureTypeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_RecruitmentApplyId",
                table: "Attachment",
                column: "RecruitmentApplyId",
                unique: true,
                filter: "[RecruitmentApplyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_SupportingDocumentId",
                table: "Attachment",
                column: "SupportingDocumentId",
                unique: true,
                filter: "[SupportingDocumentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Class_ClassTypeId",
                table: "Class",
                column: "ClassTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Class_CompanyId",
                table: "Class",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Class_CourseId",
                table: "Class",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Class_EmployeeId",
                table: "Class",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Class_TeacherId",
                table: "Class",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_ManagerId",
                table: "Company",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_RegionId",
                table: "Company",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLearningRoadMap_CompanyId",
                table: "CompanyLearningRoadMap",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLearningRoadMap_LearningRoadMapId",
                table: "CompanyLearningRoadMap",
                column: "LearningRoadMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_ApplicationUserId",
                table: "Contact",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_StudentId",
                table: "Contact",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_CouponIssueId",
                table: "Coupon",
                column: "CouponIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_RegisterStudyId",
                table: "Coupon",
                column: "RegisterStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_StudentId",
                table: "Coupon",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponIssue_CouponTypeId",
                table: "CouponIssue",
                column: "CouponTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponIssueStudent_CouponIssueId",
                table: "CouponIssueStudent",
                column: "CouponIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponIssueStudent_StudentId",
                table: "CouponIssueStudent",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponStudent_CouponTypeId",
                table: "CouponStudent",
                column: "CouponTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponStudent_StudentId",
                table: "CouponStudent",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponType_CompanyId",
                table: "CouponType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponType_CourseId",
                table: "CouponType",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponTypeCoupon_CouponTypeID",
                table: "CouponTypeCoupon",
                column: "CouponTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_CouponTypeDiscount_CouponTypeId",
                table: "CouponTypeDiscount",
                column: "CouponTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponTypeDiscountDetail_CouponTypeDiscountId",
                table: "CouponTypeDiscountDetail",
                column: "CouponTypeDiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponTypeFixedPrice_CouponTypeId",
                table: "CouponTypeFixedPrice",
                column: "CouponTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponTypeGift_CouponTypeId",
                table: "CouponTypeGift",
                column: "CouponTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponTypeGiftDetail_CouponTypeGiftId",
                table: "CouponTypeGiftDetail",
                column: "CouponTypeGiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_LearningRoadMapId",
                table: "Course",
                column: "LearningRoadMapId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseGiftDeTail_CourseGiftId",
                table: "CourseGiftDeTail",
                column: "CourseGiftId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseGiftDeTail_CourseId",
                table: "CourseGiftDeTail",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseGifts_PromotionId",
                table: "CourseGifts",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseLesson_LectureTypeId",
                table: "CourseLesson",
                column: "LectureTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseLesson_TuitionId",
                table: "CourseLesson",
                column: "TuitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_DepartmentCode",
                table: "Department",
                column: "DepartmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_DivisionId",
                table: "Department",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentPosition_DepartmentId",
                table: "DepartmentPosition",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentPosition_PositionId",
                table: "DepartmentPosition",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailRegisterStudy_ClassTypeId",
                table: "DetailRegisterStudy",
                column: "ClassTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailRegisterStudy_CourseId",
                table: "DetailRegisterStudy",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailRegisterStudy_RegisterStudyId",
                table: "DetailRegisterStudy",
                column: "RegisterStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountDetails_DiscountId",
                table: "DiscountDetails",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_PromotionId",
                table: "Discounts",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_Division_DivisionCode",
                table: "Division",
                column: "DivisionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ApplicationUserId",
                table: "Employees",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                table: "Employees",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PositionId",
                table: "Employees",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_ClassId",
                table: "Enrollments",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluateTeachers_TeacherId",
                table: "EvaluateTeachers",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventCode",
                table: "Event",
                column: "EventCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Holiday_CategoryId",
                table: "Holiday",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Holiday_WorkingTimeConfigurationId",
                table: "Holiday",
                column: "WorkingTimeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_AffiliatePartnerId",
                table: "Image",
                column: "AffiliatePartnerId",
                unique: true,
                filter: "[AffiliatePartnerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Image_CompanyId",
                table: "Image",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_LearningRoadMapId",
                table: "Image",
                column: "LearningRoadMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_SupportingDocumentId",
                table: "Image",
                column: "SupportingDocumentId",
                unique: true,
                filter: "[SupportingDocumentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LearningRoadMap_AgeGrId",
                table: "LearningRoadMap",
                column: "AgeGrId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningRoadMap_LearningRoadMapCode",
                table: "LearningRoadMap",
                column: "LearningRoadMapCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogEmployeePosition_CompanyId",
                table: "LogEmployeePosition",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LogEmployeePosition_EmployeeId",
                table: "LogEmployeePosition",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LogEmployeePosition_PositionId",
                table: "LogEmployeePosition",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_LogRegionCom_CompanyId",
                table: "LogRegionCom",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LogRegionCom_RegionId",
                table: "LogRegionCom",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_OutputCommitment_CompanyId",
                table: "OutputCommitment",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OutputCommitment_ConsultantId",
                table: "OutputCommitment",
                column: "ConsultantId");

            migrationBuilder.CreateIndex(
                name: "IX_OutputCommitment_RegionId",
                table: "OutputCommitment",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_OutputCommitment_StudentId",
                table: "OutputCommitment",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerType_PartnerTypeCode",
                table: "PartnerType",
                column: "PartnerTypeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTeachers_TeacherId",
                table: "PayrollTeachers",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_PlacementTests_StudentId",
                table: "PlacementTests",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Position_PositionCode",
                table: "Position",
                column: "PositionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_CompanyId",
                table: "Profiles",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_ConsultantId",
                table: "Profiles",
                column: "ConsultantId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_RegionId",
                table: "Profiles",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCoupon_CouponTypeID",
                table: "PromotionCoupon",
                column: "CouponTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCoupon_PromotionId",
                table: "PromotionCoupon",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionFixedPrice_PromotionId",
                table: "PromotionFixedPrice",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionGift_PromotionId",
                table: "PromotionGift",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionGiftDetail_PromotionGiftId",
                table: "PromotionGiftDetail",
                column: "PromotionGiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_CompanyId",
                table: "Promotions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_CourseId",
                table: "Promotions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_PromotionGroupId",
                table: "Promotions",
                column: "PromotionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_StudentId",
                table: "Promotions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_CourseId",
                table: "Receipts",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_EmployeeId",
                table: "Receipts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_RegisterStudyId",
                table: "Receipts",
                column: "RegisterStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_StudentId",
                table: "Receipts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentApplies_RecruitmentId",
                table: "RecruitmentApplies",
                column: "RecruitmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentInfo_DepartmentId",
                table: "RecruitmentInfo",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Region_ManagerId",
                table: "Region",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterGift_GiftId",
                table: "RegisterGift",
                column: "GiftId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterGift_PromotionId",
                table: "RegisterGift",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterGift_RegisterStudyId",
                table: "RegisterGift",
                column: "RegisterStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterPromotionList_PromotionId",
                table: "RegisterPromotionList",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterPromotionList_RegisterStudyId",
                table: "RegisterPromotionList",
                column: "RegisterStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterStudys_CompanyId",
                table: "RegisterStudys",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterStudys_EmployeeId",
                table: "RegisterStudys",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterStudys_PromotionId",
                table: "RegisterStudys",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterStudys_RegionId",
                table: "RegisterStudys",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterStudys_StudentId",
                table: "RegisterStudys",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterStudys_TeacherId",
                table: "RegisterStudys",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentActivity_EmployeeId",
                table: "StudentActivity",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentActivity_StudentId",
                table: "StudentActivity",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourse_CourseId",
                table: "StudentCourse",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourse_StudentId",
                table: "StudentCourse",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNote_EmployeeId",
                table: "StudentNote",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNote_StudentId",
                table: "StudentNote",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ApplicationUserId",
                table: "Students",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_CategoryId",
                table: "Students",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_CompanyId",
                table: "Students",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_EmployeeId",
                table: "Students",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_RegionId",
                table: "Students",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_ApplicationUserId",
                table: "Teachers",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_CompanyId",
                table: "Teachers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tuition_ClassTypeId",
                table: "Tuition",
                column: "ClassTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tuition_CourseId",
                table: "Tuition",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetailInfos_AccountGroupId",
                table: "UserDetailInfos",
                column: "AccountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkBoardTeachers_TeacherId",
                table: "WorkBoardTeachers",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingTime_WorkingTimeConfigurationId",
                table: "WorkingTime",
                column: "WorkingTimeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingTimeConfigurationCompany_CompanyId",
                table: "WorkingTimeConfigurationCompany",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingTimeConfigurationCompany_WorkingTimeConfigurationId",
                table: "WorkingTimeConfigurationCompany",
                column: "WorkingTimeConfigurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionsQuotaAdjustment_AdmissionsQuotaCompany_AdmissionsQuotaCompanyId",
                table: "AdmissionsQuotaAdjustment",
                column: "AdmissionsQuotaCompanyId",
                principalTable: "AdmissionsQuotaCompany",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionsQuotaAdjustment_AdmissionsQuotaRegion_AdmissionsQuotaRegionId",
                table: "AdmissionsQuotaAdjustment",
                column: "AdmissionsQuotaRegionId",
                principalTable: "AdmissionsQuotaRegion",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionsQuotaCompany_AdmissionsQuotaRegion_AdmissionsQuotaRegionId",
                table: "AdmissionsQuotaCompany",
                column: "AdmissionsQuotaRegionId",
                principalTable: "AdmissionsQuotaRegion",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionsQuotaCompany_Company_CompanyId",
                table: "AdmissionsQuotaCompany",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionsQuotaEmployee_AdmissionsQuotaRegion_AdmissionsQuotaRegionId",
                table: "AdmissionsQuotaEmployee",
                column: "AdmissionsQuotaRegionId",
                principalTable: "AdmissionsQuotaRegion",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionsQuotaEmployee_Company_CompanyId",
                table: "AdmissionsQuotaEmployee",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionsQuotaEmployee_Employees_EmployeeId",
                table: "AdmissionsQuotaEmployee",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionsQuotaEmployee_Region_RegionId",
                table: "AdmissionsQuotaEmployee",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionsQuotaRegion_Region_RegionId",
                table: "AdmissionsQuotaRegion",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AllocationDetailEvent_Company_CompanyId",
                table: "AllocationDetailEvent",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AllocationDetailEvent_Region_RegionId",
                table: "AllocationDetailEvent",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Class_Company_CompanyId",
                table: "Class",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Class_Employees_EmployeeId",
                table: "Class",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Class_Teachers_TeacherId",
                table: "Class",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Employees_ManagerId",
                table: "Company",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Region_RegionId",
                table: "Company",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Company_CompanyId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "AccountGroupEmployees");

            migrationBuilder.DropTable(
                name: "AccountGroupPermissions");

            migrationBuilder.DropTable(
                name: "AccountGroupUsers");

            migrationBuilder.DropTable(
                name: "AdmissionsQuotaAdjustment");

            migrationBuilder.DropTable(
                name: "AdmissionsQuotaEmployee");

            migrationBuilder.DropTable(
                name: "AllocationDetailEvent");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "CompanyLearningRoadMap");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropTable(
                name: "CouponIssueStudent");

            migrationBuilder.DropTable(
                name: "CouponStudent");

            migrationBuilder.DropTable(
                name: "CouponTypeCoupon");

            migrationBuilder.DropTable(
                name: "CouponTypeDiscountDetail");

            migrationBuilder.DropTable(
                name: "CouponTypeFixedPrice");

            migrationBuilder.DropTable(
                name: "CouponTypeGiftDetail");

            migrationBuilder.DropTable(
                name: "CourseGiftDeTail");

            migrationBuilder.DropTable(
                name: "CourseLesson");

            migrationBuilder.DropTable(
                name: "DataLogs");

            migrationBuilder.DropTable(
                name: "Degree");

            migrationBuilder.DropTable(
                name: "DepartmentPosition");

            migrationBuilder.DropTable(
                name: "DetailRegisterStudy");

            migrationBuilder.DropTable(
                name: "DiscountDetails");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "EvaluateTeachers");

            migrationBuilder.DropTable(
                name: "Holiday");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropTable(
                name: "LogEmployeePosition");

            migrationBuilder.DropTable(
                name: "LogRegionCom");

            migrationBuilder.DropTable(
                name: "OutputCommitment");

            migrationBuilder.DropTable(
                name: "PayrollTeachers");

            migrationBuilder.DropTable(
                name: "PlacementTests");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "PromotionCoupon");

            migrationBuilder.DropTable(
                name: "PromotionFixedPrice");

            migrationBuilder.DropTable(
                name: "PromotionGiftDetail");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropTable(
                name: "RegisterGift");

            migrationBuilder.DropTable(
                name: "RegisterPromotionList");

            migrationBuilder.DropTable(
                name: "StudentActivity");

            migrationBuilder.DropTable(
                name: "StudentCourse");

            migrationBuilder.DropTable(
                name: "StudentNote");

            migrationBuilder.DropTable(
                name: "UserDetailInfos");

            migrationBuilder.DropTable(
                name: "WorkBoardTeachers");

            migrationBuilder.DropTable(
                name: "WorkingTime");

            migrationBuilder.DropTable(
                name: "WorkingTimeConfigurationCompany");

            migrationBuilder.DropTable(
                name: "AdmissionsQuotaCompany");

            migrationBuilder.DropTable(
                name: "AllocationEvent");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "RecruitmentApplies");

            migrationBuilder.DropTable(
                name: "CouponIssue");

            migrationBuilder.DropTable(
                name: "CouponTypeDiscount");

            migrationBuilder.DropTable(
                name: "CouponTypeGift");

            migrationBuilder.DropTable(
                name: "CourseGifts");

            migrationBuilder.DropTable(
                name: "LectureType");

            migrationBuilder.DropTable(
                name: "Tuition");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "Class");

            migrationBuilder.DropTable(
                name: "AffiliatePartner");

            migrationBuilder.DropTable(
                name: "SupportingDocument");

            migrationBuilder.DropTable(
                name: "PromotionGift");

            migrationBuilder.DropTable(
                name: "Gift");

            migrationBuilder.DropTable(
                name: "RegisterStudys");

            migrationBuilder.DropTable(
                name: "AccountGroups");

            migrationBuilder.DropTable(
                name: "WorkingTimeConfiguration");

            migrationBuilder.DropTable(
                name: "AdmissionsQuotaRegion");

            migrationBuilder.DropTable(
                name: "RecruitmentInfo");

            migrationBuilder.DropTable(
                name: "CouponType");

            migrationBuilder.DropTable(
                name: "ClassType");

            migrationBuilder.DropTable(
                name: "PartnerType");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "AdmissionsQuota");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "PromotionGroup");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "LearningRoadMap");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropTable(
                name: "Division");



        }
    }
}
