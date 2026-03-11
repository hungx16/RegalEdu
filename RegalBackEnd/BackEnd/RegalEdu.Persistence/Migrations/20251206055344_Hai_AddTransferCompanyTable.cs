using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Hai_AddTransferCompanyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransferCompany",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransferCompanyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SourceStudentCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SourceStudentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SourceCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceStudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DestinationCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransferCompanyStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferCompany_Company_DestinationCompanyId",
                        column: x => x.DestinationCompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferCompany_Students_SourceStudentId",
                        column: x => x.SourceStudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferCompany_DestinationCompanyId",
                table: "TransferCompany",
                column: "DestinationCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferCompany_SourceStudentId",
                table: "TransferCompany",
                column: "SourceStudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransferCompany");
        }
    }
}
