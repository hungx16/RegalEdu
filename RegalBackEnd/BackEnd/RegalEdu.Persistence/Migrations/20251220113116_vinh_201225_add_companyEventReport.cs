using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_201225_add_companyEventReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyEventReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberStudents = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CompanyEventStatus = table.Column<int>(type: "int", nullable: true),
                    LinkContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkFanpage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEventReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyEventReport_CompanyEvent_CompanyEventId",
                        column: x => x.CompanyEventId,
                        principalTable: "CompanyEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApproveCompanyEventReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyEventReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApproveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveCompanyEventReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApproveCompanyEventReport_CompanyEventReport_CompanyEventReportId",
                        column: x => x.CompanyEventReportId,
                        principalTable: "CompanyEventReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApproveCompanyEventReport_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApproveCompanyEventReport_CompanyEventReportId",
                table: "ApproveCompanyEventReport",
                column: "CompanyEventReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveCompanyEventReport_EmployeeId",
                table: "ApproveCompanyEventReport",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEventReport_CompanyEventId",
                table: "CompanyEventReport",
                column: "CompanyEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApproveCompanyEventReport");

            migrationBuilder.DropTable(
                name: "CompanyEventReport");
        }
    }
}
