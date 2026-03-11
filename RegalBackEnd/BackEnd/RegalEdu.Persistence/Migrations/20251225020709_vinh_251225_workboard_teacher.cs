using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_251225_workboard_teacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkingTimeId",
                table: "TeacherWorkLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherWorkLogs_WorkingTimeId",
                table: "TeacherWorkLogs",
                column: "WorkingTimeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherWorkLogs_WorkingTime_WorkingTimeId",
                table: "TeacherWorkLogs",
                column: "WorkingTimeId",
                principalTable: "WorkingTime",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherWorkLogs_WorkingTime_WorkingTimeId",
                table: "TeacherWorkLogs");

            migrationBuilder.DropIndex(
                name: "IX_TeacherWorkLogs_WorkingTimeId",
                table: "TeacherWorkLogs");

            migrationBuilder.DropColumn(
                name: "WorkingTimeId",
                table: "TeacherWorkLogs");
        }
    }
}
