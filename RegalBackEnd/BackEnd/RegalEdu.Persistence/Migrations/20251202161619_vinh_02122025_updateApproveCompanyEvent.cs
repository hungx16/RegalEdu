using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_02122025_updateApproveCompanyEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedBy",
                table: "ApproveCompanyEvent",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedUserId",
                table: "ApproveCompanyEvent",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApproveCompanyEvent_ApprovedUserId",
                table: "ApproveCompanyEvent",
                column: "ApprovedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApproveCompanyEvent_Employees_ApprovedUserId",
                table: "ApproveCompanyEvent",
                column: "ApprovedUserId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApproveCompanyEvent_Employees_ApprovedUserId",
                table: "ApproveCompanyEvent");

            migrationBuilder.DropIndex(
                name: "IX_ApproveCompanyEvent_ApprovedUserId",
                table: "ApproveCompanyEvent");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "ApproveCompanyEvent");

            migrationBuilder.DropColumn(
                name: "ApprovedUserId",
                table: "ApproveCompanyEvent");
        }
    }
}
