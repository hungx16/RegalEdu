using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vu_thaydoiStatusEnroll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "Enrollments",
                newName: "PaymentCourseStatus");

            migrationBuilder.AddColumn<string>(
                name: "ExpectedWorkingTime",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClassTypeId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RegisterStudyId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_ClassTypeId",
                table: "Enrollments",
                column: "ClassTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_RegisterStudyId",
                table: "Enrollments",
                column: "RegisterStudyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_ClassType_ClassTypeId",
                table: "Enrollments",
                column: "ClassTypeId",
                principalTable: "ClassType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_RegisterStudys_RegisterStudyId",
                table: "Enrollments",
                column: "RegisterStudyId",
                principalTable: "RegisterStudys",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_ClassType_ClassTypeId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_RegisterStudys_RegisterStudyId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_ClassTypeId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_RegisterStudyId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "ExpectedWorkingTime",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ClassTypeId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "RegisterStudyId",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "PaymentCourseStatus",
                table: "Enrollments",
                newName: "PaymentStatus");
        }
    }
}
