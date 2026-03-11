using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_131225_evaluateTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClassId",
                table: "EvaluateTeachers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClassScheduleId",
                table: "EvaluateTeachers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StarRating",
                table: "EvaluateTeachers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "EvaluateTeachers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluateTeachers_ClassId",
                table: "EvaluateTeachers",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluateTeachers_ClassScheduleId",
                table: "EvaluateTeachers",
                column: "ClassScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluateTeachers_StudentId",
                table: "EvaluateTeachers",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluateTeachers_ClassSchedules_ClassScheduleId",
                table: "EvaluateTeachers",
                column: "ClassScheduleId",
                principalTable: "ClassSchedules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluateTeachers_Class_ClassId",
                table: "EvaluateTeachers",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluateTeachers_Students_StudentId",
                table: "EvaluateTeachers",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EvaluateTeachers_ClassSchedules_ClassScheduleId",
                table: "EvaluateTeachers");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluateTeachers_Class_ClassId",
                table: "EvaluateTeachers");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluateTeachers_Students_StudentId",
                table: "EvaluateTeachers");

            migrationBuilder.DropIndex(
                name: "IX_EvaluateTeachers_ClassId",
                table: "EvaluateTeachers");

            migrationBuilder.DropIndex(
                name: "IX_EvaluateTeachers_ClassScheduleId",
                table: "EvaluateTeachers");

            migrationBuilder.DropIndex(
                name: "IX_EvaluateTeachers_StudentId",
                table: "EvaluateTeachers");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "EvaluateTeachers");

            migrationBuilder.DropColumn(
                name: "ClassScheduleId",
                table: "EvaluateTeachers");

            migrationBuilder.DropColumn(
                name: "StarRating",
                table: "EvaluateTeachers");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "EvaluateTeachers");
        }
    }
}
