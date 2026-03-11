using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Vu_update_couponty_29_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CouponType_Company_CompanyId",
                table: "CouponType");

            migrationBuilder.DropForeignKey(
                name: "FK_CouponType_Course_CourseId",
                table: "CouponType");

            migrationBuilder.DropIndex(
                name: "IX_CouponType_CompanyId",
                table: "CouponType");

            migrationBuilder.DropIndex(
                name: "IX_CouponType_CourseId",
                table: "CouponType");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CouponType");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CouponType");

            migrationBuilder.AddColumn<string>(
                name: "CompanyIds",
                table: "CouponType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseIds",
                table: "CouponType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentIds",
                table: "CouponType",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyIds",
                table: "CouponType");

            migrationBuilder.DropColumn(
                name: "CourseIds",
                table: "CouponType");

            migrationBuilder.DropColumn(
                name: "StudentIds",
                table: "CouponType");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "CouponType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "CouponType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CouponType_CompanyId",
                table: "CouponType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponType_CourseId",
                table: "CouponType",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CouponType_Company_CompanyId",
                table: "CouponType",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CouponType_Course_CourseId",
                table: "CouponType",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id");
        }
    }
}
