using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_291125_companyEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllocationDetailEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyEventCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyEventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AffiliatePartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NumberStudents = table.Column<int>(type: "int", nullable: true),
                    EventContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Propose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EventSize = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyEvent_AffiliatePartner_AffiliatePartnerId",
                        column: x => x.AffiliatePartnerId,
                        principalTable: "AffiliatePartner",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanyEvent_AllocationDetailEvent_AllocationDetailEventId",
                        column: x => x.AllocationDetailEventId,
                        principalTable: "AllocationDetailEvent",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventCash",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCash", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventParticipant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsStudent = table.Column<bool>(type: "bit", nullable: false),
                    ParticipantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParticipantGender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParticipantDateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParticipantAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParticipantPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParticipantContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParticipantEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParticipantSchool = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParticipantSourceKnown = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParticipantJob = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventParticipant_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventPublication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PublicationAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPublication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventPublication_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEvent_AffiliatePartnerId",
                table: "CompanyEvent",
                column: "AffiliatePartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEvent_AllocationDetailEventId",
                table: "CompanyEvent",
                column: "AllocationDetailEventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipant_EmployeeId",
                table: "EventParticipant",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventPublication_ItemId",
                table: "EventPublication",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyEvent");

            migrationBuilder.DropTable(
                name: "EventCash");

            migrationBuilder.DropTable(
                name: "EventParticipant");

            migrationBuilder.DropTable(
                name: "EventPublication");
        }
    }
}
