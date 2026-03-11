using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vu_update_Enrollment_22_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Enrollments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PaidAmount",
                table: "Enrollments",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Enrollments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentCourseStatus",
                table: "Enrollments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Times",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "UsableAmount",
                table: "Enrollments",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PaidAmount",
                table: "DetailRegisterStudy",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "StudentCourseStatus",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "Times",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "UsableAmount",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "DetailRegisterStudy");
        }
    }
}
